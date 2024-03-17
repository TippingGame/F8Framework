using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using F8Framework.AssetMap;
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
        private static string _enableFullPackageKey = "EnableFullPackageKey";
        private static string _enableOptionalPackageKey = "EnableOptionalPackageKey";
        private static string _enableNullPackageKey = "EnableNullPackageKey";
        private static string _optionalPackageKey = "OptionalPackageKey";
        private static string _assetRemoteAddressKey = "AssetRemoteAddressKey";
        
        public static string BuildPath = "";
        private static string _fromVersion = "1.0.0";
        public static string ToVersion = "1.0.0";
        private static string _codeVersion = "1";
        private static bool _enableHotUpdate = false;
        private static string _hotUpdateURL = "http://127.0.0.1:6789/remote";
        private static bool _enableFullPackage = true;
        private static bool _enableOptionalPackage = false;
        private static bool _enableNullPackage = false;
        private static string _optionalPackage = "0_1_2_3";
        private static string _assetRemoteAddress = "http://127.0.0.1:6789/remote";
        
        private static BuildTarget _buildTarget = BuildTarget.NoTarget;

        private static int _index = 0;
        private static BuildTarget[] _options = Enum.GetValues(typeof(BuildTarget))
            .Cast<BuildTarget>()
            .Select(option => (BuildTarget)Enum.Parse(typeof(BuildTarget), option.ToString()))
            .ToArray();
        private static string[] _optionNames = Array.ConvertAll(_options, option => option.ToString());
        
        private static bool _exportCurrentPlatform = true;
        
        
        private static void BuildUpdate()
        {
        }

        public static void Build()
        {
            string appName = Application.productName;
            switch (_buildTarget)
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
            
            PlayerSettings.bundleVersion = ToVersion; // 设置显示版本号
            PlayerSettings.Android.bundleVersionCode = int.Parse(_codeVersion); // 设置 Android 内部版本号
            PlayerSettings.iOS.buildNumber = _codeVersion; // 设置 iOS 内部版本号

            if (_enableFullPackage)
            {
                string locationPathName = BuildPath + "/" + _buildTarget.ToString() + "_Full_" + ToVersion  + "/" + appName;
                BuildReport buildReport =
                    BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, _buildTarget, BuildOptions.None);
                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    LogF8.LogError($"build pkg fail : {buildReport.summary.result}");
                }

                LogF8.LogAsset("游戏全量包打包成功! " + locationPathName);
            }
            
            if (_enableOptionalPackage)
            {
                string toPath = FileTools.TruncatePath(Application.dataPath, 1) + "/temp_OptionalPackage";
                Dictionary<string, AssetBundleMap.AssetMapping> mappings = 
                    Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(Resources.Load<TextAsset>("AssetBundleMap").ToString());
                CopyDeleteUnnecessaryAb(URLSetting.GetAssetBundlesOutPath(), mappings, toPath);
                FileTools.SafeCopyDirectory(toPath, BuildPath + "/OptionalPackage_" + ToVersion, true);
                AssetDatabase.Refresh();
                string locationPathName = BuildPath + "/" + _buildTarget.ToString() + "_Optional_" + ToVersion  + "/" + appName;
                BuildReport buildReport =
                    BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, _buildTarget, BuildOptions.None);
                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    LogF8.LogError($"build pkg fail : {buildReport.summary.result}");
                }
                FileTools.SafeCopyDirectory(toPath, Application.streamingAssetsPath, true);
                FileTools.SafeDeleteDir(toPath);
                LogF8.LogAsset("游戏可选资源包打包成功! " + locationPathName);
            }
            
            if (_enableNullPackage)
            {
                string toPath = FileTools.TruncatePath(Application.dataPath, 1) + "/temp_NullPackage";
                FileTools.SafeCopyDirectory(URLSetting.GetAssetBundlesOutPath(), toPath, true, new[] { URLSetting.GetPlatformName(), URLSetting.GetPlatformName() + ".manifest" });
                FileTools.SafeDeleteDir(URLSetting.GetAssetBundlesOutPath(),
                    new[] { URLSetting.GetPlatformName(), URLSetting.GetPlatformName() + ".manifest" });
                AssetDatabase.Refresh();
                string locationPathName = BuildPath + "/" + _buildTarget.ToString() + "_Null_" + ToVersion  + "/" + appName;
                BuildReport buildReport =
                    BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, _buildTarget, BuildOptions.None);
                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    LogF8.LogError($"build pkg fail : {buildReport.summary.result}");
                }
                FileTools.SafeCopyDirectory(toPath, URLSetting.GetAssetBundlesOutPath(), true, new[] { URLSetting.GetPlatformName(), URLSetting.GetPlatformName() + ".manifest" });
                FileTools.SafeDeleteDir(toPath);
                LogF8.LogAsset("游戏空包打包成功! " + locationPathName);
            }
            
            AssetDatabase.Refresh();
        }

        public static void SetBuildTarget()
        {
            GUILayout.Space(20);
            GUILayout.Label("【设置打包平台】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            _exportCurrentPlatform = EditorGUILayout.Toggle("根据当前平台导出", EditorPrefs.GetBool(_exportCurrentPlatformKey, true));
            if (EditorPrefs.GetBool(_exportCurrentPlatformKey, true) != _exportCurrentPlatform)
            {
                EditorPrefs.SetBool(_exportCurrentPlatformKey, _exportCurrentPlatform);
            }
            if (_exportCurrentPlatform)
            {
                _buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
            
            GUILayout.Space(10);
            if (!_exportCurrentPlatform)
            {
                Array enumValues = Enum.GetValues(typeof(BuildTarget));
                for (int i = 0; i < enumValues.Length; i++)
                {
                    BuildTarget target = (BuildTarget)enumValues.GetValue(i); // 获取枚举值
                    if (target.ToString() == EditorPrefs.GetString(_exportPlatformKey, ""))
                    {
                        _index = i;
                    }
                }
                
                _index = EditorGUILayout.Popup(_index, _optionNames);
                if (_options[_index].ToString() != EditorPrefs.GetString(_exportPlatformKey, ""))
                {
                    EditorPrefs.SetString(_exportPlatformKey, _options[_index].ToString());
                    EditorUserBuildSettings.SwitchActiveBuildTarget(GetBuildTargetGroup(_options[_index]), _options[_index]);
                }
                _buildTarget = _options[_index];
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
                fromVersionValue = _fromVersion;
            }
            _fromVersion = EditorGUILayout.TextField(fromVersionValue);
            EditorPrefs.SetString(_fromVersionKey, _fromVersion);
            GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
            
            GUILayout.Label("发布的版本");
            string toVersionValue = EditorPrefs.GetString(_toVersionKey, "");
            if (string.IsNullOrEmpty(toVersionValue))
            {
                toVersionValue = ToVersion;
            }
            ToVersion = EditorGUILayout.TextField(toVersionValue);
            EditorPrefs.SetString(_toVersionKey, ToVersion);
            GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("构建次数（某些平台需要递增）：");
            string codeVersionValue = EditorPrefs.GetString(_codeVersionKey, "");
            if (string.IsNullOrEmpty(codeVersionValue))
            {
                codeVersionValue = _codeVersion;
            }
            _codeVersion = EditorGUILayout.TextField(codeVersionValue);
            EditorPrefs.SetString(_codeVersionKey, _codeVersion);
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
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("资产加载远程地址：", GUILayout.Width(130));
            string assetRemoteAddressValue = EditorPrefs.GetString(_assetRemoteAddressKey, "");
            if (string.IsNullOrEmpty(assetRemoteAddressValue))
            {
                assetRemoteAddressValue = _hotUpdateURL;
            }
            _assetRemoteAddress = EditorGUILayout.TextField(assetRemoteAddressValue);
            EditorPrefs.SetString(_assetRemoteAddressKey, _assetRemoteAddress);
            GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            bool enableHotUpdatesValue = EditorPrefs.GetBool(_enableHotUpdateKey, false);
            _enableHotUpdate = EditorGUILayout.Toggle("启用热更新", enableHotUpdatesValue);
            if (enableHotUpdatesValue != _enableHotUpdate)
            {
                EditorPrefs.SetBool(_enableHotUpdateKey, _enableHotUpdate);
            }

            if (_enableHotUpdate)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("热更新资源CDN地址：", GUILayout.Width(130));
                string hotUpdateURLValue = EditorPrefs.GetString(_hotUpdateURLKey, "");
                if (string.IsNullOrEmpty(hotUpdateURLValue))
                {
                    hotUpdateURLValue = _hotUpdateURL;
                }
                _hotUpdateURL = EditorGUILayout.TextField(hotUpdateURLValue);
                EditorPrefs.SetString(_hotUpdateURLKey, _hotUpdateURL);
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
            
            bool enableFullPackage = EditorPrefs.GetBool(_enableFullPackageKey, true);
            bool enableOptionalPackage = EditorPrefs.GetBool(_enableOptionalPackageKey, false);
            bool enableNullPackage = EditorPrefs.GetBool(_enableNullPackageKey, false);
            
            GUILayout.BeginHorizontal();
            _enableFullPackage = EditorGUILayout.Toggle("全量资源打包进游戏", enableFullPackage);
            if (enableFullPackage != _enableFullPackage)
            {
                EditorPrefs.SetBool(_enableFullPackageKey, _enableFullPackage);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            
            GUILayout.BeginHorizontal();
            _enableOptionalPackage = EditorGUILayout.Toggle("手动选择资源包,分隔符：_", enableOptionalPackage);
            if (enableOptionalPackage != _enableOptionalPackage)
            {
                EditorPrefs.SetBool(_enableOptionalPackageKey, _enableOptionalPackage);
            }
            
            
            string optionalPackageValue = EditorPrefs.GetString(_optionalPackageKey, "");
            if (string.IsNullOrEmpty(optionalPackageValue))
            {
                optionalPackageValue = _optionalPackage;
            }
            _optionalPackage = EditorGUILayout.TextField(optionalPackageValue);
            EditorPrefs.SetString(_optionalPackageKey, _optionalPackage);
            GUILayout.FlexibleSpace(); // 添加 FlexibleSpace 来实现左对齐
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            _enableNullPackage = EditorGUILayout.Toggle("打空包游戏", enableNullPackage);
            if (enableNullPackage != _enableNullPackage)
            {
                EditorPrefs.SetBool(_enableNullPackageKey, _enableNullPackage);
            }
            GUILayout.EndHorizontal();
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
                    string countent = _fromVersion == ToVersion
                        ? "确定发布版本 " + ToVersion
                        : "确定发布从版本 " + _fromVersion + " 到 " + ToVersion;
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

                else if (_fromVersion == ToVersion)
                {
                    EditorUtility.DisplayDialog("发布热更新包", "发布热更新包版本不能相同", "确定");
                }
                else
                {
                    string countent = "确定发布从版本 " + _fromVersion + " 到 " + ToVersion;
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
        
        private static void CopyDeleteUnnecessaryAb(string assetBundlesOutPath, Dictionary<string, AssetBundleMap.AssetMapping> mappings, string toPath)
        {
            string[] packages = _optionalPackage.Split('_');
            Stack<string> stack = new Stack<string>();
            stack.Push(assetBundlesOutPath);

            while (stack.Count > 0)
            {
                string currentPath = stack.Pop();

                // Check for directories
                string[] directories = Directory.GetDirectories(currentPath);
                foreach (string directory in directories)
                {
                    stack.Push(directory);
                }

                // Check for files
                string[] files = Directory.GetFiles(currentPath);
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();
                    if (extension != ".meta" && extension != ".manifest" && extension != ".ds_store")
                    {
                        string filePath = FileTools.FormatToUnityPath(file);
                        string filePathManifest = FileTools.FormatToUnityPath(file) + ".manifest";
                        string abName = Path.ChangeExtension(filePath, null).Replace(assetBundlesOutPath + "/", "");
                        foreach (var value in mappings.Values)
                        {
                            if (abName == value.AbName && !packages.Contains(value.Package))
                            {
                                FileTools.SafeCopyFile(filePath,
                                    FileTools.FormatToUnityPath(toPath + "/" + ABBuildTool.GetAssetBundlesPath(filePath)));
                                FileTools.SafeDeleteFile(filePath);
                                FileTools.SafeDeleteFile(filePath + ".meta");
                                
                                FileTools.SafeCopyFile(filePathManifest,
                                    FileTools.FormatToUnityPath(toPath + "/" + ABBuildTool.GetAssetBundlesPath(filePathManifest)));
                                FileTools.SafeDeleteFile(filePathManifest);
                                FileTools.SafeDeleteFile(filePathManifest + ".meta");
                            }
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        
        private static void WriteGameVersion()
        {
            string GameVersionPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/AssetMap/Resources/GameVersion.json";
            FileTools.SafeDeleteFile(GameVersionPath);
            FileTools.SafeDeleteFile(GameVersionPath + ".meta");
            FileTools.CheckFileAndCreateDirWhenNeeded(GameVersionPath);
            AssetDatabase.Refresh();
            
            GameVersion gameVersion = new GameVersion(ToVersion, subPackage: null, "", _enableHotUpdate, _hotUpdateURL);
            // 写入到文件
            string filePath = Application.dataPath + "/F8Framework/AssetMap/Resources/GameVersion.json";
            // 序列化对象
            string json = Util.LitJson.ToJson(gameVersion);
            FileTools.SafeDeleteFile(filePath);
            UnityEditor.AssetDatabase.Refresh();
            FileTools.CheckFileAndCreateDirWhenNeeded(filePath);
            FileTools.SafeWriteAllText(filePath, json);
            LogF8.LogAsset("写入游戏版本： " + gameVersion.Version);
            UnityEditor.AssetDatabase.Refresh();
        }
        
        private static BuildTargetGroup GetBuildTargetGroup(BuildTarget target)
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(target);

            if (targetGroup != BuildTargetGroup.Unknown)
            {
                return targetGroup;
            }
            else
            {
                LogF8.LogError($"Could not find BuildTargetGroup for BuildTarget {target}");
                return default;
            }
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