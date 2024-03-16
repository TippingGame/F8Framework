using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class BuildPkgTool : ScriptableObject
    {
        private static readonly GUILayoutOption NormalWidth = GUILayout.Width(100);
        private static readonly GUILayoutOption ButtonHeight = GUILayout.Height(20);
        private static string _prefBuildPathKey = "PrefBuildPathKey";
        private static string _exportPlatformKey = "ExportPlatformKey";
        private static string _exportCurrentPlatformKey = "ExportCurrentPlatformKey";
        private static string _fromVersionKey = "FromVersionKey";
        private static string _toVersionKey = "ToVersionKey";
        private static string _codeVersionKey = "CodeVersionKey";
        private static string _enableHotUpdateKey = "EnableHotUpdateKey";
        private static string _hotUpdateURLKey = "HotUpdateURLKey";
        
        public static string BuildPath = "";
        private static string fromVersion = "1.0.0";
        private static string toVersion = "1.0.0";
        private static string codeVersion = "0";
        private static bool enableHotUpdate = false;
        private static string hotUpdateURL = "http://127.0.0.1:6789";
        
        private static BuildTarget buildTarget = BuildTarget.NoTarget;

        private static int index = 0;
        private static BuildTarget[] options = Enum.GetValues(typeof(BuildTarget))
            .Cast<BuildTarget>()
            .Select(option => (BuildTarget)Enum.Parse(typeof(BuildTarget), option.ToString()))
            .ToArray();
        private static string[] optionNames = Array.ConvertAll(options, option => option.ToString());
        
        private static bool exportCurrentPlatform = true;
        
        
        private static void BuildUpdate()
        {
        }

        public static void Build()
        {
            string appName = Application.productName;
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.WSAPlayer:
                    appName += ".exe";
                    break;
                case BuildTarget.Android:
                    appName += ".apk";
                    break;
            }

            string locationPathName = BuildPath + "/" + buildTarget.ToString() + "/" + appName;
            FileTools.CheckDirAndCreateWhenNeeded(BuildPath + "/" + buildTarget.ToString());

            PlayerSettings.bundleVersion = toVersion; // 设置显示版本号
            PlayerSettings.Android.bundleVersionCode = int.Parse(codeVersion); // 设置 Android 内部版本号
            PlayerSettings.iOS.buildNumber = codeVersion; // 设置 iOS 内部版本号
            
            BuildReport buildReport =
                BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, buildTarget, BuildOptions.None);
            if (buildReport.summary.result != BuildResult.Succeeded)
            {
                LogF8.LogError($"build pkg fail : {buildReport.summary.result}");
            }

            LogF8.LogAsset("游戏打包成功! " + locationPathName);
        }

        public static void SetBuildTarget()
        {
            GUILayout.Space(20);
            GUILayout.Label("【设置打包平台】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            exportCurrentPlatform = EditorGUILayout.Toggle("根据当前平台导出", EditorPrefs.GetBool(_exportCurrentPlatformKey, true));
            if (EditorPrefs.GetBool(_exportCurrentPlatformKey, true) != exportCurrentPlatform)
            {
                EditorPrefs.SetBool(_exportCurrentPlatformKey, exportCurrentPlatform);
            }
            if (exportCurrentPlatform)
            {
                buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
            
            GUILayout.Space(10);
            if (!exportCurrentPlatform)
            {
                Array enumValues = Enum.GetValues(typeof(BuildTarget));
                for (int i = 0; i < enumValues.Length; i++)
                {
                    BuildTarget target = (BuildTarget)enumValues.GetValue(i); // 获取枚举值
                    if (target.ToString() == EditorPrefs.GetString(_exportPlatformKey, ""))
                    {
                        index = i;
                    }
                }
                
                index = EditorGUILayout.Popup(index, optionNames);
                if (options[index].ToString() != EditorPrefs.GetString(_exportPlatformKey, ""))
                {
                    EditorPrefs.SetString(_exportPlatformKey, options[index].ToString());
                }
                buildTarget = options[index];
            }
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        public static void DrawRootDirectory()
        {
            GUILayout.Space(5);
            GUILayout.Label("【打包输出目录】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("点击设置目录", NormalWidth, ButtonHeight))
            {
                string _buildPath = EditorUtility.OpenFolderPanel("设置打包根目录", BuildPath, BuildPath);
                if (!string.IsNullOrEmpty(_buildPath))
                {
                    BuildPath = _buildPath;
                    EditorPrefs.SetString(_prefBuildPathKey, BuildPath);
                }
            }

            BuildPath = EditorPrefs.GetString(_prefBuildPathKey, "");
           
            GUILayout.Label("输出目录：" + BuildPath);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        public static void DrawVersion()
        {
            GUILayout.Space(5);
            GUILayout.Label("【发布版本】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("发布版本：", GUILayout.Width(60));
            GUILayout.Space(10);
            GUILayout.Label("旧版本");
            string fromVersionValue = EditorPrefs.GetString(_fromVersionKey, "");
            if (string.IsNullOrEmpty(fromVersionValue))
            {
                fromVersionValue = fromVersion;
            }
            fromVersionValue = EditorGUILayout.TextField(fromVersionValue);
            EditorPrefs.SetString(_fromVersionKey, fromVersionValue);
            GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
            
            GUILayout.Label("发布的版本");
            string toVersionValue = EditorPrefs.GetString(_toVersionKey, "");
            if (string.IsNullOrEmpty(toVersionValue))
            {
                toVersionValue = toVersion;
            }
            toVersionValue = EditorGUILayout.TextField(toVersionValue);
            EditorPrefs.SetString(_toVersionKey, toVersionValue);
            GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("构建次数（某些平台需要递增）：");
            string codeVersionValue = EditorPrefs.GetString(_codeVersionKey, "");
            if (string.IsNullOrEmpty(codeVersionValue))
            {
                codeVersionValue = codeVersion;
            }
            codeVersionValue = EditorGUILayout.TextField(codeVersionValue);
            EditorPrefs.SetString(_codeVersionKey, codeVersionValue);
            GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }

        public static void DrawHotUpdate()
        {
            GUILayout.Space(5);
            GUILayout.Label("【热更新管理】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);

            bool enableHotUpdatesValue = EditorPrefs.GetBool(_enableHotUpdateKey, false);
            enableHotUpdate = EditorGUILayout.Toggle("启用热更新", enableHotUpdatesValue);
            if (enableHotUpdatesValue != enableHotUpdate)
            {
                EditorPrefs.SetBool(_enableHotUpdateKey, enableHotUpdate);
            }

            if (enableHotUpdate)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("热更新资源CDN地址：", GUILayout.Width(130));
                string hotUpdateURLValue = EditorPrefs.GetString(_hotUpdateURLKey, "");
                if (string.IsNullOrEmpty(hotUpdateURLValue))
                {
                    hotUpdateURLValue = hotUpdateURL;
                }
                hotUpdateURLValue = EditorGUILayout.TextField(hotUpdateURLValue);
                EditorPrefs.SetString(_hotUpdateURLKey, hotUpdateURLValue);
                GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        public static void DrawBuildPkg()
        {
            GUILayout.Space(5);
            GUILayout.Label("【打包游戏】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打包游戏", NormalWidth))
            {
                if (string.IsNullOrEmpty(BuildPath))
                {
                    EditorUtility.DisplayDialog("打包游戏", "输出目录路径不能为空", "确定");
                }
                else
                {
                    string countent = fromVersion == toVersion
                        ? "确定发布版本 " + toVersion
                        : "确定发布从版本 " + fromVersion + " 到 " + toVersion;
                    if (EditorUtility.DisplayDialog("打包游戏", countent, "确定", "取消"))
                    {
                        EditorApplication.delayCall += WriteGameVersion;
                        EditorApplication.delayCall += F8Helper.F8Run;
                        EditorApplication.delayCall += Build;
                    }
                }
            }

            GUILayout.Space(30);
            if (GUILayout.Button("发布热更新包", NormalWidth))
            {
                if (string.IsNullOrEmpty(BuildPath))
                {
                    EditorUtility.DisplayDialog("发布热更新包", "发布热更新包路径不能为空", "确定");
                }

                else if (fromVersion == toVersion)
                {
                    EditorUtility.DisplayDialog("发布热更新包", "发布热更新包版本不能相同", "确定");
                }
                else
                {
                    string countent = "确定发布从版本 " + fromVersion + " 到 " + toVersion;
                    if (EditorUtility.DisplayDialog("发布热更新包", countent, "确定", "取消"))
                    {
                        EditorApplication.delayCall += BuildUpdate;
                    }
                }
            }
            
            GUILayout.Space(30);
            if (GUILayout.Button("打开输出目录", NormalWidth))
            {
                Process.Start(Path.GetFullPath(BuildPath));
            }
            
            GUILayout.EndHorizontal();
        }
        
        private static void WriteGameVersion()
        {
            string GameVersionPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/AssetMap/Resources/GameVersion.json";
            FileTools.SafeDeleteFile(GameVersionPath);
            FileTools.SafeDeleteFile(GameVersionPath + ".meta");
            FileTools.CheckFileAndCreateDirWhenNeeded(GameVersionPath);
            AssetDatabase.Refresh();
            
            GameVersion gameVersion = new GameVersion(toVersion);
            // 写入到文件
            string filePath = Application.dataPath + "/F8Framework/AssetMap/Resources/GameVersion.json";
            // 序列化对象
            string json = Util.LitJson.ToJson(gameVersion);
            FileTools.SafeDeleteFile(filePath);
            UnityEditor.AssetDatabase.Refresh();
            FileTools.CheckFileAndCreateDirWhenNeeded(filePath);
            File.WriteAllText(filePath, json);
            LogF8.LogAsset("写入游戏版本： " + gameVersion.Version);
            UnityEditor.AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<BuildPkgTool>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);

            // 获取绝对路径并规范化
            string scriptPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptRelativePath));

            return scriptPath;
        }
        
        private static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e != null && e.enabled)
                {
                    names.Add(e.path);
                }
            }

            return names.ToArray();
        }

        public static int StringLen(string str)
        {
            int realLength = 0;
            foreach (char c in str)
            {
                if (c >= 0 && c <= 128)
                    realLength += 1;
                else
                    realLength += 2;
            }

            return realLength;
        }
    }
}