using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public interface IF8OrderedEditorExtension
    {
        int Order { get; }
    }

    public interface IF8BuildPipelineContributor : IF8OrderedEditorExtension
    {
        void Configure(F8EditorPipelineBuilder builder, F8BuildRequest request);
    }

    public interface IF8BuildCommandLineConfigurator : IF8OrderedEditorExtension
    {
        void Apply(string[] arguments);
    }

    public interface IF8BuildSettingsSection : IF8OrderedEditorExtension
    {
        void DrawGUI();
    }

    [Serializable]
    public sealed class F8BuildRequest
    {
        public string DisplayName = "F8 Build Pipeline";
        public bool IncludeExtensions = true;
        public bool WriteGameVersion;
        public bool GenerateHotUpdateDll;
        public bool BuildAssetBundles;
        public bool BuildPlayer;
        public bool BuildUpdate;
        public bool AutoRunPlayer;
        public bool WriteAssetVersion;

        public void Validate()
        {
            if (BuildPlayer && BuildUpdate)
            {
                throw new InvalidOperationException("一次构建请求不能同时构建 Player 和热更新包。");
            }

            if (AutoRunPlayer && !BuildPlayer)
            {
                throw new InvalidOperationException("只有构建 Player 时才能启用自动运行。");
            }
        }

        public static F8BuildRequest CreateF8Run()
        {
            return new F8BuildRequest
            {
                DisplayName = "F8Run",
                IncludeExtensions = true,
                GenerateHotUpdateDll = true,
                BuildAssetBundles = true,
            };
        }

        public static F8BuildRequest CreatePlayerBuild(bool autoRunPlayer)
        {
            return new F8BuildRequest
            {
                DisplayName = autoRunPlayer ? "构建并运行游戏" : "构建游戏",
                IncludeExtensions = true,
                WriteGameVersion = true,
                GenerateHotUpdateDll = true,
                BuildAssetBundles = true,
                BuildPlayer = true,
                AutoRunPlayer = autoRunPlayer,
                WriteAssetVersion = true,
            };
        }

        public static F8BuildRequest CreateUpdateBuild()
        {
            return new F8BuildRequest
            {
                DisplayName = "构建热更新包",
                IncludeExtensions = true,
                GenerateHotUpdateDll = true,
                BuildAssetBundles = true,
                BuildUpdate = true,
            };
        }
    }

    public static class F8BuildPipelineOrder
    {
        public const int Prepare = 100;
        public const int ConfigurationGenerate = 1000;
        public const int ConfigurationSerialize = 1100;
        public const int GenerateHotUpdateDll = 2000;
        public const int BuildAssetBundles = 3000;
        public const int BuildPlayerOrUpdate = 4000;
        public const int Finalize = 5000;
    }

    public static class F8BuildPipeline
    {
        private static bool startScheduled;

        public static void Start(F8BuildRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            F8EditorPipelineBuilder builder = CreateBuilder(request);
            F8EditorPipeline.Start(builder);
        }

        /// <summary>
        /// 在当前 Editor GUI 事件结束后启动流水线，避免脚本刷新破坏 IMGUI 布局栈。
        /// </summary>
        public static void StartDeferred(F8BuildRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (startScheduled)
            {
                LogF8.LogWarning("已有构建流水线等待启动，请勿重复提交。");
                return;
            }

            startScheduled = true;
            EditorApplication.delayCall += () =>
            {
                try
                {
                    Start(request);
                }
                finally
                {
                    startScheduled = false;
                }
            };
        }

        public static F8EditorPipelineBuilder CreateBuilder(F8BuildRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Validate();

            F8EditorPipelineBuilder builder = new F8EditorPipelineBuilder(request.DisplayName);

            if (request.WriteGameVersion)
            {
                builder.Add(
                    F8CoreBuildStepIds.WriteGameVersion,
                    F8BuildPipelineOrder.Prepare,
                    "写入游戏版本");
            }

            if (request.IncludeExtensions)
            {
                foreach (IF8BuildPipelineContributor contributor in
                         DiscoverExtensions<IF8BuildPipelineContributor>())
                {
                    contributor.Configure(builder, request);
                }
            }

            if (request.GenerateHotUpdateDll)
            {
                builder.Add(
                    F8CoreBuildStepIds.GenerateHotUpdateDll,
                    F8BuildPipelineOrder.GenerateHotUpdateDll,
                    "生成并复制热更新 DLL");
            }

            if (request.BuildAssetBundles)
            {
                builder.Add(
                    F8CoreBuildStepIds.BuildAssetBundles,
                    F8BuildPipelineOrder.BuildAssetBundles,
                    "构建 AssetBundle");
            }

            if (request.BuildPlayer)
            {
                builder.Add(
                    F8CoreBuildStepIds.BuildPlayer,
                    F8BuildPipelineOrder.BuildPlayerOrUpdate,
                    "构建 Player",
                    new F8BuildPlayerPayload { AutoRunPlayer = request.AutoRunPlayer });
            }

            if (request.BuildUpdate)
            {
                builder.Add(
                    F8CoreBuildStepIds.BuildUpdate,
                    F8BuildPipelineOrder.BuildPlayerOrUpdate,
                    "构建热更新包");
            }

            if (request.WriteAssetVersion)
            {
                builder.Add(
                    F8CoreBuildStepIds.WriteAssetVersion,
                    F8BuildPipelineOrder.Finalize,
                    "写入资源版本");
            }

            return builder;
        }

        /// <summary>
        /// Jenkins 第一个 Unity 进程入口。流水线状态持久化在 Library，脚本重载或进程退出都不会丢失。
        /// </summary>
        public static void JenkinsStart()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            BuildPkgTool.ApplyCommandLineArguments(arguments);
            ABBuildTool.ApplyCommandLineArguments(arguments);

            foreach (IF8BuildCommandLineConfigurator configurator in
                     DiscoverExtensions<IF8BuildCommandLineConfigurator>())
            {
                configurator.Apply(arguments);
            }

            Start(F8BuildRequest.CreatePlayerBuild(false));
        }

        /// <summary>
        /// Jenkins 第二个 Unity 进程入口，用新编译的程序集恢复之前的流水线。
        /// 若第一个进程已经完成流水线，此调用安全地保持空操作。
        /// </summary>
        public static void JenkinsResume()
        {
            F8EditorPipeline.ResumePending(true);
        }

        public static void DrawExtensionSettings()
        {
            foreach (IF8BuildSettingsSection section in DiscoverExtensions<IF8BuildSettingsSection>())
            {
                try
                {
                    section.DrawGUI();
                }
                catch (Exception exception)
                {
                    EditorGUILayout.HelpBox(
                        $"扩展构建设置绘制失败：{section.GetType().FullName}\n{exception.Message}",
                        MessageType.Error);
                    LogF8.LogException(exception);
                }
            }
        }

        private static IEnumerable<TExtension> DiscoverExtensions<TExtension>()
            where TExtension : IF8OrderedEditorExtension
        {
            List<TExtension> extensions = new List<TExtension>();
            foreach (Type type in TypeCache.GetTypesDerivedFrom<TExtension>())
            {
                if (type.IsAbstract || type.IsInterface || type.ContainsGenericParameters)
                {
                    continue;
                }

                if (Activator.CreateInstance(type, true) is TExtension extension)
                {
                    extensions.Add(extension);
                }
            }

            return extensions
                .OrderBy(extension => extension.Order)
                .ThenBy(extension => extension.GetType().FullName, StringComparer.Ordinal)
                .ToArray();
        }
    }

    public static class F8EditorCommandLine
    {
        public static string GetValue(string[] arguments, string name)
        {
            return TryGetValue(arguments, name, out string value) ? value : null;
        }

        public static string GetRequiredValue(string[] arguments, string name)
        {
            string value = GetValue(arguments, name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"缺少必需的命令行参数：{name} <value>");
            }

            return value;
        }

        public static bool TryGetValue(string[] arguments, string name, out string value)
        {
            value = null;
            if (arguments == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            for (int index = 0; index < arguments.Length; index++)
            {
                if (!string.Equals(arguments[index], name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (index + 1 >= arguments.Length || LooksLikeValueName(arguments[index + 1]))
                {
                    return false;
                }

                value = arguments[index + 1];
                return true;
            }

            return false;
        }

        public static bool GetBool(string[] arguments, string name, bool defaultValue = false)
        {
            return TryGetBool(arguments, name, out bool value) ? value : defaultValue;
        }

        public static bool TryGetBool(string[] arguments, string name, out bool value)
        {
            value = false;
            if (!TryGetValue(arguments, name, out string rawValue))
            {
                return false;
            }

            if (!bool.TryParse(rawValue, out value))
            {
                throw new ArgumentException(
                    $"命令行参数 {name} 的值无效：{rawValue}，应为 true 或 false。");
            }

            return true;
        }

        private static bool LooksLikeValueName(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.EndsWith("-", StringComparison.Ordinal))
            {
                return false;
            }

            return value.IndexOf('/') < 0 && value.IndexOf('\\') < 0;
        }
    }

    internal static class F8CoreBuildStepIds
    {
        public const string WriteGameVersion = "f8.build.write-game-version";
        public const string GenerateHotUpdateDll = "f8.build.generate-hot-update-dll";
        public const string BuildAssetBundles = "f8.build.asset-bundles";
        public const string BuildPlayer = "f8.build.player";
        public const string BuildUpdate = "f8.build.update";
        public const string WriteAssetVersion = "f8.build.write-asset-version";
    }

    [Serializable]
    internal sealed class F8BuildPlayerPayload
    {
        public bool AutoRunPlayer;
    }

    internal sealed class F8WriteGameVersionStep : IF8EditorPipelineStep
    {
        public string Id => F8CoreBuildStepIds.WriteGameVersion;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            BuildPkgTool.WriteGameVersion();
            return F8EditorPipelineStepResult.Completed;
        }
    }

    internal sealed class F8GenerateHotUpdateDllStep : IF8EditorPipelineStep
    {
        public string Id => F8CoreBuildStepIds.GenerateHotUpdateDll;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            F8Helper.GenerateCopyHotUpdateDll();
            return F8EditorPipelineStepResult.Completed;
        }
    }

    internal sealed class F8BuildAssetBundlesStep : IF8EditorPipelineStep
    {
        public string Id => F8CoreBuildStepIds.BuildAssetBundles;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            ABBuildTool.BuildAllAB();
            return F8EditorPipelineStepResult.Completed;
        }
    }

    internal sealed class F8BuildPlayerStep : IF8EditorPipelineStep
    {
        public string Id => F8CoreBuildStepIds.BuildPlayer;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            F8BuildPlayerPayload payload = context.GetPayload<F8BuildPlayerPayload>();
            BuildPkgTool.Build(payload.AutoRunPlayer);
            return F8EditorPipelineStepResult.Completed;
        }
    }

    internal sealed class F8BuildUpdateStep : IF8EditorPipelineStep
    {
        public string Id => F8CoreBuildStepIds.BuildUpdate;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            BuildPkgTool.BuildUpdate();
            return F8EditorPipelineStepResult.Completed;
        }
    }

    internal sealed class F8WriteAssetVersionStep : IF8EditorPipelineStep
    {
        public string Id => F8CoreBuildStepIds.WriteAssetVersion;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            BuildPkgTool.WriteAssetVersion();
            return F8EditorPipelineStepResult.Completed;
        }
    }
}
