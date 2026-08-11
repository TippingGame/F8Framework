using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public enum F8EditorPipelineStepResult
    {
        Completed,
        RequestScriptReload,
    }

    public interface IF8EditorPipelineStep
    {
        string Id { get; }
        F8EditorPipelineStepResult Execute(F8EditorPipelineContext context);
    }

    [Serializable]
    public sealed class F8EditorPipelineStepDefinition
    {
        public string Id;
        public string DisplayName;
        public string Payload;
        public int Order;
        public int Sequence;
    }

    public sealed class F8EditorPipelineContext
    {
        internal F8EditorPipelineContext(
            string pipelineId,
            string pipelineName,
            F8EditorPipelineStepDefinition step)
        {
            PipelineId = pipelineId;
            PipelineName = pipelineName;
            Step = step;
        }

        public string PipelineId { get; }
        public string PipelineName { get; }
        public F8EditorPipelineStepDefinition Step { get; }

        public TPayload GetPayload<TPayload>() where TPayload : new()
        {
            return string.IsNullOrEmpty(Step.Payload)
                ? new TPayload()
                : JsonUtility.FromJson<TPayload>(Step.Payload);
        }
    }

    public sealed class F8EditorPipelineBuilder
    {
        private readonly List<F8EditorPipelineStepDefinition> steps =
            new List<F8EditorPipelineStepDefinition>();
        private int sequence;

        public F8EditorPipelineBuilder(string displayName)
        {
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "F8 Editor Pipeline" : displayName;
        }

        public string DisplayName { get; }
        public int Count => steps.Count;

        public F8EditorPipelineBuilder Add(
            string id,
            int order,
            string displayName = null,
            string payload = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("流水线步骤 Id 不能为空。", nameof(id));
            }

            steps.Add(new F8EditorPipelineStepDefinition
            {
                Id = id,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? id : displayName,
                Payload = payload,
                Order = order,
                Sequence = sequence++,
            });
            return this;
        }

        public F8EditorPipelineBuilder Add<TPayload>(
            string id,
            int order,
            string displayName,
            TPayload payload)
        {
            return Add(id, order, displayName,
                ReferenceEquals(payload, null) ? null : JsonUtility.ToJson(payload));
        }

        public IReadOnlyList<F8EditorPipelineStepDefinition> GetOrderedSteps()
        {
            return steps
                .OrderBy(step => step.Order)
                .ThenBy(step => step.Sequence)
                .ToArray();
        }
    }

    [Serializable]
    internal sealed class F8EditorPipelineState
    {
        public int Version = 1;
        public string PipelineId;
        public string DisplayName;
        public string StartedAtUtc;
        public List<F8EditorPipelineStepDefinition> Steps =
            new List<F8EditorPipelineStepDefinition>();
        public int NextStepIndex;
        public bool WaitingForScriptReload;
        public string ReloadRequestedByDomain;
        public bool Failed;
        public string Failure;
    }

    /// <summary>
    /// 可跨 Domain Reload 和 Editor 重启恢复的 Editor 流水线。
    /// 持久化的是步骤 Id，而不是会在 Domain Reload 后失效的委托。
    /// </summary>
    [InitializeOnLoad]
    public static class F8EditorPipeline
    {
        private const int StateVersion = 1;
        private static readonly string DomainId = Guid.NewGuid().ToString("N");
        private static bool isExecuting;
        private static bool resumeScheduled;

        static F8EditorPipeline()
        {
            ScheduleResume();
        }

        public static bool HasPendingPipeline =>
            File.Exists(StatePath) || File.Exists(StateTempPath);

        public static string PendingPipelineName
        {
            get
            {
                try
                {
                    return TryLoadState(out F8EditorPipelineState state)
                        ? state.DisplayName
                        : null;
                }
                catch (Exception)
                {
                    return "无效的流水线状态";
                }
            }
        }

        public static void Start(F8EditorPipelineBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (builder.Count == 0)
            {
                LogF8.LogConfig($"流水线没有可执行步骤：{builder.DisplayName}");
                return;
            }

            if (TryLoadState(out F8EditorPipelineState existingState))
            {
                if (!existingState.Failed)
                {
                    throw new InvalidOperationException(
                        $"已有流水线正在执行：{existingState.DisplayName}。请等待完成或先取消。");
                }

                LogF8.LogWarning($"覆盖上一次失败的流水线：{existingState.DisplayName}");
            }

            F8EditorPipelineState state = new F8EditorPipelineState
            {
                Version = StateVersion,
                PipelineId = Guid.NewGuid().ToString("N"),
                DisplayName = builder.DisplayName,
                StartedAtUtc = DateTime.UtcNow.ToString("O"),
                Steps = builder.GetOrderedSteps().ToList(),
            };

            SaveState(state);
            ExecutePending(state, Application.isBatchMode);
        }

        public static bool ResumePending(bool throwOnFailure = false)
        {
            if (!TryLoadState(out F8EditorPipelineState state))
            {
                return false;
            }

            if (state.Failed)
            {
                string message = $"流水线处于失败状态：{state.DisplayName}\n{state.Failure}";
                LogF8.LogError(message);
                if (throwOnFailure)
                {
                    throw new InvalidOperationException(message);
                }

                return false;
            }

            if (state.WaitingForScriptReload &&
                string.Equals(state.ReloadRequestedByDomain, DomainId, StringComparison.Ordinal))
            {
                return false;
            }

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                ScheduleResume();
                return false;
            }

            if (state.WaitingForScriptReload)
            {
                state.WaitingForScriptReload = false;
                state.ReloadRequestedByDomain = null;
                SaveState(state);
            }

            ExecutePending(state, throwOnFailure || Application.isBatchMode);
            return true;
        }

        public static bool RetryFailed()
        {
            if (!TryLoadState(out F8EditorPipelineState state) || !state.Failed)
            {
                return false;
            }

            state.Failed = false;
            state.Failure = null;

            bool reloadPendingInCurrentDomain = state.WaitingForScriptReload &&
                                                string.Equals(
                                                    state.ReloadRequestedByDomain,
                                                    DomainId,
                                                    StringComparison.Ordinal);
            if (state.WaitingForScriptReload && !reloadPendingInCurrentDomain)
            {
                state.WaitingForScriptReload = false;
                state.ReloadRequestedByDomain = null;
            }

            SaveState(state);

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                ScheduleResume();
                return true;
            }

            if (reloadPendingInCurrentDomain)
            {
                try
                {
                    RequestCompilationAndRefresh();
                    return true;
                }
                catch (Exception exception)
                {
                    MarkFailed(state, exception);
                    if (Application.isBatchMode)
                    {
                        throw;
                    }

                    DisplayFailure(state, exception);
                    return false;
                }
            }

            ExecutePending(state, Application.isBatchMode);
            return true;
        }

        public static bool CancelPending()
        {
            if (!HasPendingPipeline)
            {
                return false;
            }

            string name = PendingPipelineName;
            DeleteStateFile(StatePath);
            DeleteStateFile(StateTempPath);
            TryDeleteEmptyStateDirectory();
            LogF8.LogWarning($"已取消流水线：{name ?? "未知流水线"}");
            return true;
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            ScheduleResume();
        }

        [MenuItem("开发工具/流水线/重试失败任务", false, 900)]
        private static void RetryFailedMenu()
        {
            RetryFailed();
        }

        [MenuItem("开发工具/流水线/重试失败任务", true)]
        private static bool ValidateRetryFailedMenu()
        {
            try
            {
                return TryLoadState(out F8EditorPipelineState state) && state.Failed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [MenuItem("开发工具/流水线/取消当前任务", false, 901)]
        private static void CancelPendingMenu()
        {
            CancelPending();
        }

        [MenuItem("开发工具/流水线/取消当前任务", true)]
        private static bool ValidateCancelPendingMenu()
        {
            return HasPendingPipeline;
        }

        private static void ExecutePending(F8EditorPipelineState state, bool throwOnFailure)
        {
            if (isExecuting)
            {
                return;
            }

            isExecuting = true;
            try
            {
                Dictionary<string, IF8EditorPipelineStep> handlers = DiscoverStepHandlers();
                while (state.NextStepIndex < state.Steps.Count)
                {
                    F8EditorPipelineStepDefinition definition = state.Steps[state.NextStepIndex];
                    if (!handlers.TryGetValue(definition.Id, out IF8EditorPipelineStep handler))
                    {
                        throw new InvalidOperationException(
                            $"找不到流水线步骤处理器：{definition.Id}。对应模块可能已被移除。");
                    }

                    LogF8.LogConfig(
                        $"流水线 [{state.DisplayName}] 执行步骤 " +
                        $"{state.NextStepIndex + 1}/{state.Steps.Count}：{definition.DisplayName}");

                    F8EditorPipelineContext context = new F8EditorPipelineContext(
                        state.PipelineId,
                        state.DisplayName,
                        definition);
                    F8EditorPipelineStepResult result = handler.Execute(context);

                    state.NextStepIndex++;
                    if (result == F8EditorPipelineStepResult.RequestScriptReload)
                    {
                        state.WaitingForScriptReload = true;
                        state.ReloadRequestedByDomain = DomainId;
                        SaveState(state);

                        // 状态必须先落盘；Domain Reload 随时可能中断当前调用栈。
                        RequestCompilationAndRefresh();
                        return;
                    }

                    SaveState(state);
                }

                DeleteState();
                LogF8.LogConfig($"<color=green>流水线执行完成：{state.DisplayName}</color>");
            }
            catch (Exception exception)
            {
                MarkFailed(state, exception);

                if (throwOnFailure)
                {
                    throw;
                }

                DisplayFailure(state, exception);
            }
            finally
            {
                isExecuting = false;
            }
        }

        private static Dictionary<string, IF8EditorPipelineStep> DiscoverStepHandlers()
        {
            Dictionary<string, IF8EditorPipelineStep> handlers =
                new Dictionary<string, IF8EditorPipelineStep>(StringComparer.Ordinal);

            foreach (Type type in TypeCache.GetTypesDerivedFrom<IF8EditorPipelineStep>())
            {
                if (type.IsAbstract || type.IsInterface || type.ContainsGenericParameters)
                {
                    continue;
                }

                IF8EditorPipelineStep handler =
                    Activator.CreateInstance(type, true) as IF8EditorPipelineStep;
                if (handler == null || string.IsNullOrWhiteSpace(handler.Id))
                {
                    continue;
                }

                if (handlers.ContainsKey(handler.Id))
                {
                    throw new InvalidOperationException(
                        $"流水线步骤 Id 重复：{handler.Id}（{handlers[handler.Id].GetType().FullName} / {type.FullName}）");
                }

                handlers.Add(handler.Id, handler);
            }

            return handlers;
        }

        private static void ScheduleResume()
        {
            if (resumeScheduled)
            {
                return;
            }

            resumeScheduled = true;
            EditorApplication.delayCall += ResumeWhenEditorReady;
        }

        private static void ResumeWhenEditorReady()
        {
            resumeScheduled = false;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                ScheduleResume();
                return;
            }

            try
            {
                ResumePending(Application.isBatchMode);
            }
            catch (Exception exception)
            {
                LogF8.LogException(exception);
                if (Application.isBatchMode)
                {
                    throw;
                }
            }
        }

        private static string StatePath => Path.GetFullPath(Path.Combine(
            Application.dataPath,
            "../Library/F8EditorPipeline/state.json"));

        private static string StateTempPath => StatePath + ".tmp";

        private static bool TryLoadState(out F8EditorPipelineState state)
        {
            state = null;
            string[] paths = { StatePath, StateTempPath };
            List<Exception> failures = new List<Exception>();
            bool stateFileExists = false;
            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                stateFileExists = true;
                try
                {
                    string json = File.ReadAllText(path, Encoding.UTF8);
                    F8EditorPipelineState candidate =
                        JsonUtility.FromJson<F8EditorPipelineState>(json);
                    ValidateState(candidate);
                    state = candidate;
                    return true;
                }
                catch (Exception exception)
                {
                    failures.Add(new InvalidDataException(
                        "无法读取流水线状态文件：" + path,
                        exception));
                }
            }

            if (!stateFileExists)
            {
                return false;
            }

            throw new InvalidDataException(
                "无法读取流水线状态文件。" +
                "可通过 开发工具/流水线/取消当前任务 清理该状态。",
                failures.Count == 1 ? failures[0] : new AggregateException(failures));
        }

        private static void SaveState(F8EditorPipelineState state)
        {
            string directory = Path.GetDirectoryName(StatePath);
            if (string.IsNullOrEmpty(directory))
            {
                throw new InvalidOperationException("无法确定流水线状态目录。");
            }

            Directory.CreateDirectory(directory);
            File.WriteAllText(
                StateTempPath,
                JsonUtility.ToJson(state, true),
                new UTF8Encoding(false));

            if (File.Exists(StatePath))
            {
                try
                {
                    File.Replace(StateTempPath, StatePath, null);
                    return;
                }
                catch (PlatformNotSupportedException)
                {
                    // 某些 Unity 目标平台不支持 File.Replace，回退到同目录移动。
                }
                catch (IOException)
                {
                    // 文件系统不支持原子替换时回退。
                }

                File.Delete(StatePath);
            }

            File.Move(StateTempPath, StatePath);
        }

        private static void DeleteState()
        {
            DeleteStateFile(StatePath);
            DeleteStateFile(StateTempPath);

            TryDeleteEmptyStateDirectory();
        }

        private static void ValidateState(F8EditorPipelineState state)
        {
            if (state == null || state.Version != StateVersion ||
                string.IsNullOrWhiteSpace(state.PipelineId) || state.Steps == null ||
                state.NextStepIndex < 0 || state.NextStepIndex > state.Steps.Count)
            {
                throw new InvalidDataException("流水线状态内容无效。");
            }

            foreach (F8EditorPipelineStepDefinition step in state.Steps)
            {
                if (step == null || string.IsNullOrWhiteSpace(step.Id))
                {
                    throw new InvalidDataException("流水线状态包含无效步骤。");
                }
            }
        }

        private static void RequestCompilationAndRefresh()
        {
            CompilationPipeline.RequestScriptCompilation();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static void MarkFailed(F8EditorPipelineState state, Exception exception)
        {
            state.Failed = true;
            state.Failure = exception.ToString();
            SaveState(state);
            LogF8.LogException(exception);
        }

        private static void DisplayFailure(
            F8EditorPipelineState state,
            Exception exception)
        {
            EditorUtility.DisplayDialog(
                "F8 流水线执行失败",
                $"{state.DisplayName}\n\n{exception.Message}\n\n" +
                "修复问题后可从 开发工具/流水线/重试失败任务 继续。",
                "确定");
        }

        private static void DeleteStateFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static void TryDeleteEmptyStateDirectory()
        {
            string directory = Path.GetDirectoryName(StatePath);
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory) &&
                !Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory);
            }
        }
    }
}
