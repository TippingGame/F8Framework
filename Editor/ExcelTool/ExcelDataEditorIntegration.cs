using System;
using F8Framework.Core;
using F8Framework.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace F8Framework.ExcelData.Editor
{
    public static class ExcelDataSettings
    {
        public const string EnabledKey = "UseExcelDataTool";
        public const string SourcePathKey = "ExcelPath";
        public const string ExportFormatKey = "ConvertExcelToOtherFormatsKey";
        public const string OutputPathKey = "ExcelBinDataFolderKey";
        public const string BinaryFormat = "binary";
        public const string JsonFormat = "json";

        public static readonly string[] ExportFormats = { JsonFormat, BinaryFormat };

        [InitializeOnLoadMethod]
        private static void RegisterExcelDataSourceInEditor()
        {
            ConfigDataSourceRegistry.Register(new ExcelConfigDataSource());
        }

        public static bool Enabled
        {
            get => F8EditorPrefs.GetBool(EnabledKey, true);
            set => F8EditorPrefs.SetBool(EnabledKey, value);
        }

        public static string SourcePath
        {
            get
            {
                EnsureDefaults();
                return URLSetting.AddRootPath(F8EditorPrefs.GetString(SourcePathKey, string.Empty));
            }
            set => F8EditorPrefs.SetString(SourcePathKey, URLSetting.RemoveRootPath(value));
        }

        public static string OutputPath
        {
            get
            {
                EnsureDefaults();
                return URLSetting.AddRootPath(F8EditorPrefs.GetString(OutputPathKey, string.Empty));
            }
            set => F8EditorPrefs.SetString(OutputPathKey, URLSetting.RemoveRootPath(value));
        }

        public static string ExportFormat
        {
            get
            {
                EnsureDefaults();
                string value = F8EditorPrefs.GetString(ExportFormatKey, BinaryFormat);
                return string.Equals(value, JsonFormat, StringComparison.OrdinalIgnoreCase)
                    ? JsonFormat
                    : BinaryFormat;
            }
            set => F8EditorPrefs.SetString(
                ExportFormatKey,
                string.Equals(value, JsonFormat, StringComparison.OrdinalIgnoreCase)
                    ? JsonFormat
                    : BinaryFormat);
        }

        public static void EnsureDefaults()
        {
            if (F8EditorPrefs.GetString(SourcePathKey, string.Empty).IsNullOrEmpty())
            {
                SourcePath = Application.dataPath + ExcelDataTool.ExcelPath;
            }

            if (F8EditorPrefs.GetString(OutputPathKey, string.Empty).IsNullOrEmpty())
            {
                OutputPath = Application.dataPath + ExcelDataTool.BinDataFolder;
            }

            if (F8EditorPrefs.GetString(ExportFormatKey, string.Empty).IsNullOrEmpty())
            {
                ExportFormat = BinaryFormat;
            }
        }

        public static void ApplyCommandLineArguments(string[] arguments)
        {
            if (F8EditorCommandLine.TryGetBool(arguments, "UseExcelDataTool-", out bool enabled))
            {
                Enabled = enabled;
            }

            string sourcePath = F8EditorCommandLine.GetValue(arguments, "ExcelPath-");
            string exportFormat = F8EditorCommandLine.GetValue(arguments, "ConvertExcelToOtherFormats-");
            string outputPath = F8EditorCommandLine.GetValue(arguments, "ExcelBinDataFolder-");

            if (!string.IsNullOrEmpty(sourcePath))
            {
                SourcePath = sourcePath;
            }

            if (!string.IsNullOrEmpty(exportFormat))
            {
                if (!string.Equals(exportFormat, JsonFormat, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(exportFormat, BinaryFormat, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        "命令行参数 ConvertExcelToOtherFormats- 的值无效：" +
                        exportFormat + "，应为 json 或 binary。");
                }

                ExportFormat = exportFormat;
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                OutputPath = outputPath;
            }

            EnsureDefaults();
        }

        [MenuItem("开发工具/设置Excel存放目录", false, 104)]
        private static void SetSourcePathMenu()
        {
            string selectedPath = EditorUtility.OpenFolderPanel(
                "设置Excel存放目录",
                SourcePath ?? Application.dataPath,
                string.Empty);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                SourcePath = selectedPath;
                LogF8.LogConfig("设置Excel存放目录：" + selectedPath);
            }
        }

        [MenuItem("开发工具/2: Excel导表-F8", false, 205)]
        private static void ImportMenu()
        {
            ExcelDataTool.LoadAllExcelData();
        }
    }

    public sealed class ExcelBuildPipelineContributor : IF8BuildPipelineContributor
    {
        public int Order => 100;

        public void Configure(F8EditorPipelineBuilder builder, F8BuildRequest request)
        {
            if (!ExcelDataSettings.Enabled)
            {
                return;
            }

            builder.Add(
                ExcelPipelineStepIds.GenerateCode,
                F8BuildPipelineOrder.ConfigurationGenerate,
                "从 Excel 生成配置代码");
            builder.Add(
                ExcelPipelineStepIds.SerializeData,
                F8BuildPipelineOrder.ConfigurationSerialize,
                "序列化 Excel 配置数据");
        }
    }

    public sealed class ExcelBuildCommandLineConfigurator : IF8BuildCommandLineConfigurator
    {
        public int Order => 100;

        public void Apply(string[] arguments)
        {
            ExcelDataSettings.ApplyCommandLineArguments(arguments);
        }
    }

    public sealed class ExcelBuildSettingsSection : IF8BuildSettingsSection
    {
        private static readonly GUILayoutOption ButtonWidth = GUILayout.Width(100);
        private static readonly GUILayoutOption ButtonHeight = GUILayout.Height(20);

        public int Order => 100;

        public void DrawGUI()
        {
            ExcelDataSettings.EnsureDefaults();

            GUILayout.Space(5);
            GUILayout.Label("【Excel 配置数据】",
                new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);

            ExcelDataSettings.Enabled = EditorGUILayout.ToggleLeft(
                "构建前自动生成 Excel 配置",
                ExcelDataSettings.Enabled);
            if (!ExcelDataSettings.Enabled)
            {
                EditorGUILayout.HelpBox(
                    "Excel 模块仍可保留在项目中，但不会参与 F8Run、Player 或热更新构建流水线。",
                    MessageType.Info);
                GUILayout.Space(5);
                GUILayout.Box(string.Empty, GUILayout.Height(2), GUILayout.ExpandWidth(true));
                GUILayout.Space(5);
                return;
            }

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("设置Excel目录", ButtonWidth, ButtonHeight))
                {
                    string selectedPath = EditorUtility.OpenFolderPanel(
                        "设置Excel存放目录",
                        ExcelDataSettings.SourcePath,
                        ExcelDataSettings.SourcePath);
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        ExcelDataSettings.SourcePath = selectedPath;
                    }
                }

                DrawPath(ExcelDataSettings.SourcePath, "未设置Excel目录");
            }
            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("设置导表目录", ButtonWidth, ButtonHeight))
                {
                    string selectedPath = EditorUtility.OpenFolderPanel(
                        "设置导出配置表目录（仅替换配置数据文件）",
                        ExcelDataSettings.OutputPath,
                        ExcelDataSettings.OutputPath);
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        ExcelDataSettings.OutputPath = selectedPath;
                    }
                }

                DrawPath(ExcelDataSettings.OutputPath, "未设置导表目录");
            }
            GUILayout.Space(10);

            int currentIndex = Array.IndexOf(
                ExcelDataSettings.ExportFormats,
                ExcelDataSettings.ExportFormat);
            if (currentIndex < 0)
            {
                currentIndex = Array.IndexOf(ExcelDataSettings.ExportFormats, ExcelDataSettings.BinaryFormat);
            }

            int selectedIndex;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("选择配置表格式：", GUILayout.Width(120));
                selectedIndex = EditorGUILayout.Popup(currentIndex, ExcelDataSettings.ExportFormats);
            }
            if (selectedIndex != currentIndex && selectedIndex >= 0)
            {
                ExcelDataSettings.ExportFormat = ExcelDataSettings.ExportFormats[selectedIndex];
            }

            GUILayout.Space(5);
            GUILayout.Box(string.Empty, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
        }

        private static void DrawPath(string path, string warning)
        {
            if (string.IsNullOrEmpty(path))
            {
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
            else
            {
                GUILayout.Label(path);
            }
        }
    }

    internal static class ExcelPipelineStepIds
    {
        public const string GenerateCode = "f8.excel.generate-code";
        public const string SerializeData = "f8.excel.serialize-data";
    }

    internal sealed class ExcelGenerateCodeStep : IF8EditorPipelineStep
    {
        public string Id => ExcelPipelineStepIds.GenerateCode;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            return ExcelDataTool.GenerateCode()
                ? F8EditorPipelineStepResult.RequestScriptReload
                : F8EditorPipelineStepResult.Completed;
        }
    }

    internal sealed class ExcelSerializeDataStep : IF8EditorPipelineStep
    {
        public string Id => ExcelPipelineStepIds.SerializeData;

        public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
        {
            ExcelDataTool.SerializeGeneratedData();
            return F8EditorPipelineStepResult.Completed;
        }
    }
}
