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
        private static string _toVersionKey = "ToVersionKey";
        private static string _codeVersionKey = "CodeVersionKey";
        private static string _enableHotUpdateKey = "EnableHotUpdateKey";
        private static string _enableFullPackageKey = "EnableFullPackageKey";
        private static string _enableOptionalPackageKey = "EnableOptionalPackageKey";
        private static string _enableNullPackageKey = "EnableNullPackageKey";
        private static string _optionalPackageKey = "OptionalPackageKey";
        private static string _assetRemoteAddressKey = "AssetRemoteAddressKey";
        private static string _enablePackageKey = "EnablePackageKey";
        private static string _locationPathNameKey = "LocationPathNameKey";
        
        private static string _buildPath = "";
        private static string _toVersion = "1.0.0";
        private static string _codeVersion = "1";
        private static bool _enableHotUpdate = false;
        private static bool _enableFullPackage = true;
        private static bool _enableOptionalPackage = false;
        private static bool _enableNullPackage = false;
        private static string _optionalPackage = "0_1_2_3";
        private static string _assetRemoteAddress = "http://127.0.0.1:6789" + HotUpdateManager.RemoteDirName;
        private static bool _enablePackage = false;
        
        private static BuildTarget _buildTarget = BuildTarget.NoTarget;

        private static int _index = 0;
        private static BuildTarget[] _options = Enum.GetValues(typeof(BuildTarget))
            .Cast<BuildTarget>()
            .Select(option => (BuildTarget)Enum.Parse(typeof(BuildTarget), option.ToString()))
            .ToArray();
        private static string[] _optionNames = Array.ConvertAll(_options, option => option.ToString());
        
        private static bool _exportCurrentPlatform = true;


        public static string BuildPath => F8EditorPrefs.GetString(_prefBuildPathKey, null) ?? _buildPath;
        public static string ToVersion => F8EditorPrefs.GetString(_toVersionKey, null) ?? _toVersion;
        
        // 构建热更版本
        public static void BuildUpdate()
        {
            string buildPath = F8EditorPrefs.GetString(_prefBuildPathKey, "");
            
            string toVersion = F8EditorPrefs.GetString(_toVersionKey, "");
            
            string gameVersionPath = buildPath + HotUpdateManager.RemoteDirName + "/" + nameof(GameVersion) + ".json";
            string assetBundleMapPath = buildPath + HotUpdateManager.RemoteDirName + "/" + nameof(AssetBundleMap) + ".json";
            string hotUpdateMapPath = buildPath + HotUpdateManager.RemoteDirName + "/HotUpdate" +
                                      HotUpdateManager.Separator + nameof(AssetBundleMap) + ".json";
            if (!File.Exists(gameVersionPath) || !File.Exists(assetBundleMapPath))
            {
                LogF8.LogError("请先构建一个游戏版本，再构建热更新文件！~");
                return;
            }

            var resAssetBundleMappings = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(Resources.Load<TextAsset>(nameof(AssetBundleMap)).ToString());

            var assetBundleMappings = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(FileTools.SafeReadAllText(assetBundleMapPath));
            if (File.Exists(hotUpdateMapPath))
            {
                assetBundleMappings = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(FileTools.SafeReadAllText(hotUpdateMapPath));
            }

            Dictionary<string, AssetBundleMap.AssetMapping> generateAssetBundleMappings = new Dictionary<string, AssetBundleMap.AssetMapping>();
            foreach (var resAssetMapping in resAssetBundleMappings)
            {
                assetBundleMappings.TryGetValue(resAssetMapping.Key, out AssetBundleMap.AssetMapping assetMapping);
                
                if (assetMapping == null || resAssetMapping.Value.MD5 != assetMapping.MD5) // 新增资源，MD5不同则需更新
                {
                    generateAssetBundleMappings.TryAdd(resAssetMapping.Key, resAssetMapping.Value);
                }
            }
            
            string hotUpdatePath = buildPath + HotUpdateManager.RemoteDirName + HotUpdateManager.HotUpdateDirName + HotUpdateManager.Separator + toVersion;

            FileTools.CheckDirAndCreateWhenNeeded(hotUpdatePath);
            FileTools.SafeClearDir(hotUpdatePath);
            CopyHotUpdateAb(URLSetting.GetAssetBundlesOutPath(), generateAssetBundleMappings,
                hotUpdatePath);

            GameVersion remoteGameVersion = Util.LitJson.ToObject<GameVersion>(FileTools.SafeReadAllText(gameVersionPath));
            remoteGameVersion.Version = toVersion;
            if (!remoteGameVersion.HotUpdateVersion.Contains(toVersion))
                remoteGameVersion.HotUpdateVersion.Add(toVersion);
            FileTools.SafeWriteAllText(gameVersionPath, Util.LitJson.ToJson(remoteGameVersion));
            
            FileTools.SafeCopyFile(Application.dataPath + "/F8Framework/AssetMap/Resources/" + nameof(AssetBundleMap) + ".json",
                hotUpdateMapPath);
            
            LogF8.LogVersion("构建热更新包版本成功！版本：" + toVersion);
            
            AssetDatabase.Refresh();
        }
        
        // 运行导出的游戏
        public static void RunExportedGame()
        {
            string path = F8EditorPrefs.GetString(_locationPathNameKey, "");
            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
                LogF8.LogVersion("已运行导出的游戏：" + path);
            }
            else
            {
                LogF8.LogError("无法打开导出的游戏文件：" + path);
            }
        }
        
        /// <summary>
        /// 构建项目
        /// </summary>
        public static void Build()
        {
            string appName = Application.productName;
            
            string buildPath = F8EditorPrefs.GetString(_prefBuildPathKey, "");
            
            Array enumValues = Enum.GetValues(typeof(BuildTarget));
            int index = Array.FindIndex((BuildTarget[])enumValues, target => 
                target.ToString() == F8EditorPrefs.GetString(_exportPlatformKey, ""));

            BuildTarget buildTarget = F8EditorPrefs.GetBool(_exportCurrentPlatformKey, true) ? EditorUserBuildSettings.activeBuildTarget : _options[index];
            
            string toVersion = F8EditorPrefs.GetString(_toVersionKey, "");
            
            string codeVersion = F8EditorPrefs.GetString(_codeVersionKey, "");
            
            string optionalPackage = F8EditorPrefs.GetString(_optionalPackageKey, "");
            
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
            
            PlayerSettings.bundleVersion = toVersion; // 设置显示版本号
            PlayerSettings.Android.bundleVersionCode = int.Parse(codeVersion); // 设置 Android 内部版本号
            PlayerSettings.iOS.buildNumber = codeVersion; // 设置 iOS 内部版本号

            bool enableFullPackage = F8EditorPrefs.GetBool(_enableFullPackageKey, true);
            bool enableOptionalPackage = F8EditorPrefs.GetBool(_enableOptionalPackageKey, false);
            bool enableNullPackage = F8EditorPrefs.GetBool(_enableNullPackageKey, false);
            
            // 全量包
            if (enableFullPackage)
            {
                string locationPathName = buildPath + "/" + buildTarget.ToString() + "_Full_" + toVersion  + "/" + appName;
                F8EditorPrefs.SetString(_locationPathNameKey, locationPathName);
                FileTools.CheckFileAndCreateDirWhenNeeded(locationPathName);
                BuildReport buildReport =
                    BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, buildTarget, BuildOptions.None);
                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    LogF8.LogError($"导出失败了，检查一下 Unity 内置的 Build Settings 导出的路径是否存在，Unity 没有给我清理缓存！: {buildReport.summary.result}");
                }

                LogF8.LogVersion("游戏全量包打包成功! " + locationPathName);
            }
            
            // 分包
            if (enableOptionalPackage)
            {
                string toPath = FileTools.TruncatePath(Application.dataPath, 1) + "/temp_OptionalPackage";
                FileTools.SafeDeleteDir(toPath);
                Dictionary<string, AssetBundleMap.AssetMapping> mappings = 
                    Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(Resources.Load<TextAsset>(nameof(AssetBundleMap)).ToString());
                string packagePath = buildPath + HotUpdateManager.RemoteDirName + HotUpdateManager.PackageDirName;
                FileTools.SafeDeleteDir(packagePath);
                // 分别打包Package
                string[] packages = optionalPackage.Split(HotUpdateManager.Separator);
                foreach (var package in packages)
                {
                    if (string.IsNullOrEmpty(package))
                    {
                        continue;
                    }
                    CopyDeleteUnnecessaryAb(URLSetting.GetAssetBundlesOutPath(), mappings, toPath, packagePath, package);
                    
                    Util.ZipHelper.IZipCallback zipCb = new Util.ZipHelper.ZipResult();
                    string[] paths = { packagePath };
                    string zipName = packagePath + HotUpdateManager.Separator + package + ".zip";
                    Util.ZipHelper.Zip(paths, zipName, null, zipCb);

                    FileTools.SafeDeleteDir(packagePath);
                    LogF8.LogVersion("分包输出目录：" + zipName + " ，手动上传至CDN资源服务器。");
                }
               
                AssetDatabase.Refresh();
                string locationPathName = buildPath + "/" + buildTarget.ToString() + "_Optional_" + toVersion  + "/" + appName;
                F8EditorPrefs.SetString(_locationPathNameKey, locationPathName);
                FileTools.CheckFileAndCreateDirWhenNeeded(locationPathName);
                BuildReport buildReport =
                    BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, buildTarget, BuildOptions.None);
                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    LogF8.LogError($"导出失败了，检查一下 Unity 内置的 Build Settings 导出的路径是否存在，Unity 没有给我清理缓存！: {buildReport.summary.result}");
                }

                if (Directory.Exists(toPath))
                {
                    FileTools.SafeCopyDirectory(toPath, Application.streamingAssetsPath, true);
                    FileTools.SafeDeleteDir(toPath);
                }
                LogF8.LogVersion("游戏分包打包成功! " + locationPathName);
            }
            
            // 空包
            if (enableNullPackage)
            {
                string toPath = FileTools.TruncatePath(Application.dataPath, 1) + "/temp_NullPackage";
                FileTools.SafeDeleteDir(toPath);
                FileTools.SafeCopyDirectory(URLSetting.GetAssetBundlesOutPath(), toPath, true, new[] { URLSetting.GetPlatformName(), URLSetting.GetPlatformName() + ".manifest" });
                FileTools.SafeDeleteDir(URLSetting.GetAssetBundlesOutPath(),
                    new[] { URLSetting.GetPlatformName(), URLSetting.GetPlatformName() + ".manifest" });
                AssetDatabase.Refresh();
                string locationPathName = buildPath + "/" + buildTarget.ToString() + "_Null_" + toVersion  + "/" + appName;
                F8EditorPrefs.SetString(_locationPathNameKey, locationPathName);
                FileTools.CheckFileAndCreateDirWhenNeeded(locationPathName);
                BuildReport buildReport =
                    BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, buildTarget, BuildOptions.None);
                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    LogF8.LogError($"导出失败了，检查一下 Unity 内置的 Build Settings 导出的路径是否存在，Unity 没有给我清理缓存！: {buildReport.summary.result}");
                }
                FileTools.SafeCopyDirectory(toPath, URLSetting.GetAssetBundlesOutPath(), true, new[] { URLSetting.GetPlatformName(), URLSetting.GetPlatformName() + ".manifest" });
                FileTools.SafeDeleteDir(toPath);
                LogF8.LogVersion("游戏空包打包成功! " + locationPathName);
            }
            
            AssetDatabase.Refresh();
        }

        // 设置打包平台
        public static void SetBuildTarget()
        {
            GUILayout.Space(20);
            GUILayout.Label("【设置打包平台】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            _exportCurrentPlatform = EditorGUILayout.Toggle("根据当前平台导出", F8EditorPrefs.GetBool(_exportCurrentPlatformKey, true));
            if (F8EditorPrefs.GetBool(_exportCurrentPlatformKey, true) != _exportCurrentPlatform)
            {
                F8EditorPrefs.SetBool(_exportCurrentPlatformKey, _exportCurrentPlatform);
            }
            if (_exportCurrentPlatform)
            {
                _buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
            
            GUILayout.Space(10);
            if (!_exportCurrentPlatform)
            {
                Array enumValues = Enum.GetValues(typeof(BuildTarget));
                _index = Array.FindIndex((BuildTarget[])enumValues, target => 
                    target.ToString() == F8EditorPrefs.GetString(_exportPlatformKey, ""));

                _index = EditorGUILayout.Popup(_index, _optionNames);
                if (_options[_index].ToString() != F8EditorPrefs.GetString(_exportPlatformKey, ""))
                {
                    F8EditorPrefs.SetString(_exportPlatformKey, _options[_index].ToString());
                    EditorUserBuildSettings.SwitchActiveBuildTarget(GetBuildTargetGroup(_options[_index]), _options[_index]);
                }
                _buildTarget = _options[_index];
            }
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        // 打包输出目录
        public static void DrawRootDirectory()
        {
            GUILayout.Space(5);
            GUILayout.Label("【打包输出目录】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("点击设置目录", NormalWidth, ButtonHeight))
            {
                string _buildPath = EditorUtility.OpenFolderPanel("设置打包根目录", BuildPkgTool._buildPath, BuildPkgTool._buildPath);
                if (!string.IsNullOrEmpty(_buildPath))
                {
                    BuildPkgTool._buildPath = _buildPath;
                    F8EditorPrefs.SetString(_prefBuildPathKey, BuildPkgTool._buildPath);
                }
            }

            _buildPath = F8EditorPrefs.GetString(_prefBuildPathKey, "");
           
            GUILayout.Label("输出目录：" + _buildPath);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        // 构建版本
        public static void DrawVersion()
        {
            GUILayout.Space(5);
            GUILayout.Label("【构建版本】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("构建的版本：", GUILayout.Width(80));
            string toVersionValue = F8EditorPrefs.GetString(_toVersionKey, "");
            if (string.IsNullOrEmpty(toVersionValue))
            {
                toVersionValue = _toVersion;
            }
            _toVersion = EditorGUILayout.TextField(toVersionValue);
            F8EditorPrefs.SetString(_toVersionKey, _toVersion);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("构建次数（某些平台需要递增）：", GUILayout.Width(185));
            string codeVersionValue = F8EditorPrefs.GetString(_codeVersionKey, "");
            if (string.IsNullOrEmpty(codeVersionValue))
            {
                codeVersionValue = _codeVersion;
            }
            _codeVersion = EditorGUILayout.TextField(codeVersionValue);
            F8EditorPrefs.SetString(_codeVersionKey, _codeVersion);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }

        // 热更新管理
        public static void DrawHotUpdate()
        {
            GUILayout.Space(5);
            GUILayout.Label("【热更新管理】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("资产远程地址/游戏远程版本：", GUILayout.Width(160));
            string assetRemoteAddressValue = F8EditorPrefs.GetString(_assetRemoteAddressKey, "");
            if (string.IsNullOrEmpty(assetRemoteAddressValue))
            {
                assetRemoteAddressValue = _assetRemoteAddress;
            }
            _assetRemoteAddress = EditorGUILayout.TextField(assetRemoteAddressValue);
            F8EditorPrefs.SetString(_assetRemoteAddressKey, _assetRemoteAddress);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            bool enableHotUpdatesValue = F8EditorPrefs.GetBool(_enableHotUpdateKey, false);
            _enableHotUpdate = EditorGUILayout.Toggle("启用热更新", enableHotUpdatesValue);
            if (enableHotUpdatesValue != _enableHotUpdate)
            {
                F8EditorPrefs.SetBool(_enableHotUpdateKey, _enableHotUpdate);
            }
            GUILayout.Space(10);

            bool enablePackageValue = F8EditorPrefs.GetBool(_enablePackageKey, false);
            _enablePackage = EditorGUILayout.Toggle("启用分包", enablePackageValue);
            if (enablePackageValue != _enablePackage)
            {
                F8EditorPrefs.SetBool(_enablePackageKey, _enablePackage);
            }

            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        // 打包游戏
        public static void DrawBuildPkg()
        {
            GUILayout.Space(5);
            GUILayout.Label("【打包游戏】", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Space(10);
            
            bool enableFullPackage = F8EditorPrefs.GetBool(_enableFullPackageKey, true);
            bool enableOptionalPackage = F8EditorPrefs.GetBool(_enableOptionalPackageKey, false);
            bool enableNullPackage = F8EditorPrefs.GetBool(_enableNullPackageKey, false);
            
            GUILayout.BeginHorizontal();
            _enableFullPackage = EditorGUILayout.Toggle("全量资源打包进游戏", enableFullPackage);
            if (enableFullPackage != _enableFullPackage)
            {
                F8EditorPrefs.SetBool(_enableFullPackageKey, _enableFullPackage);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            
            GUILayout.BeginHorizontal();
            _enableOptionalPackage = EditorGUILayout.Toggle("选择分包资源包,分隔符：" + HotUpdateManager.Separator, enableOptionalPackage, GUILayout.Width(180));
            if (enableOptionalPackage != _enableOptionalPackage)
            {
                F8EditorPrefs.SetBool(_enableOptionalPackageKey, _enableOptionalPackage);
            }
            
            
            string optionalPackageValue = F8EditorPrefs.GetString(_optionalPackageKey, "");
            if (string.IsNullOrEmpty(optionalPackageValue))
            {
                optionalPackageValue = _optionalPackage;
            }
            _optionalPackage = EditorGUILayout.TextField(optionalPackageValue);
            F8EditorPrefs.SetString(_optionalPackageKey, _optionalPackage);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            _enableNullPackage = EditorGUILayout.Toggle("打空包游戏", enableNullPackage);
            if (enableNullPackage != _enableNullPackage)
            {
                F8EditorPrefs.SetBool(_enableNullPackageKey, _enableNullPackage);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打包游戏", NormalWidth))
            {
                if (string.IsNullOrEmpty(_buildPath))
                {
                    EditorUtility.DisplayDialog("打包游戏", "输出目录路径不能为空", "确定");
                }
                else
                {
                    string countent = "确定构建版本 " + _toVersion;
                    if (EditorUtility.DisplayDialog("打包游戏", countent, "确定", "取消"))
                    {
                        F8EditorPrefs.SetBool("compilationFinishedBuildPkg", true);
                        EditorApplication.delayCall += WriteGameVersion;
                        EditorApplication.delayCall += F8Helper.F8Run;
                    }
                }
            }

            GUILayout.Space(30);
            if (GUILayout.Button("构建热更新包", NormalWidth))
            {
                if (string.IsNullOrEmpty(_buildPath))
                {
                    EditorUtility.DisplayDialog("构建热更新包", "构建热更新包路径不能为空", "确定");
                }
                else
                {
                    string countent = "确定构建热更新包版本 " + _toVersion;
                    if (EditorUtility.DisplayDialog("构建热更新包", countent, "确定", "取消"))
                    {
                        F8EditorPrefs.SetBool("compilationFinishedBuildUpdate", true);
                        EditorApplication.delayCall += F8Helper.F8Run;
                    }
                }
            }
            
            GUILayout.Space(30);
            if (GUILayout.Button("打开输出目录", NormalWidth))
            {
                Process.Start(Path.GetFullPath(_buildPath));
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打包游戏并运行", NormalWidth))
            {
                if (string.IsNullOrEmpty(_buildPath))
                {
                    EditorUtility.DisplayDialog("打包游戏", "输出目录路径不能为空", "确定");
                }
                else
                {
                    string countent = "确定构建版本 " + _toVersion;
                    if (EditorUtility.DisplayDialog("打包游戏", countent, "确定", "取消"))
                    {
                        F8EditorPrefs.SetBool("compilationFinishedBuildPkg", true);
                        F8EditorPrefs.SetBool("compilationFinishedBuildRun", true);
                        EditorApplication.delayCall += WriteGameVersion;
                        EditorApplication.delayCall += F8Helper.F8Run;
                    }
                }
            }
            
            GUILayout.Space(163);
            
            if (GUILayout.Button("打开沙盒目录", NormalWidth))
            {
                System.Diagnostics.Process.Start(Application.persistentDataPath);
            }
            GUILayout.EndHorizontal();
        }
        
        // 复制需要热更新的AB
        private static void CopyHotUpdateAb(string assetBundlesOutPath, Dictionary<string, AssetBundleMap.AssetMapping> mappings, string toPath)
        {
            Dictionary<string, AssetBundleMap.AssetMapping> temp_mappings =
                new Dictionary<string, AssetBundleMap.AssetMapping>();
            foreach (var mapping in mappings)
            {
                temp_mappings.TryAdd(mapping.Value.AbName, mapping.Value);
            }
            
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
                        string abName = filePath.Replace(assetBundlesOutPath + "/", "");
                        if (temp_mappings.TryGetValue(abName, out AssetBundleMap.AssetMapping assetMapping))
                        {
                            FileTools.SafeCopyFile(filePath,
                                FileTools.FormatToUnityPath(toPath + "/" + ABBuildTool.GetAssetBundlesPath(filePath)));
                                
                            FileTools.SafeCopyFile(filePathManifest,
                                FileTools.FormatToUnityPath(toPath + "/" + ABBuildTool.GetAssetBundlesPath(filePathManifest)));
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        
        // 复制并删除不需要打进包里的AB
        private static void CopyDeleteUnnecessaryAb(string assetBundlesOutPath, Dictionary<string, AssetBundleMap.AssetMapping> mappings, string toPath, string toPath2, string package)
        {
            Dictionary<string, AssetBundleMap.AssetMapping> temp_mappings =
                new Dictionary<string, AssetBundleMap.AssetMapping>();
            foreach (var mapping in mappings)
            {
                temp_mappings.TryAdd(mapping.Value.AbName, mapping.Value);
            }
            
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
                        string abName = filePath.Replace(assetBundlesOutPath + "/", "");
                        if (temp_mappings.TryGetValue(abName, out AssetBundleMap.AssetMapping assetMapping))
                        {
                            if (abName == assetMapping.AbName && package.Equals(assetMapping.Package))
                            {
                                FileTools.SafeCopyFile(filePath,
                                    FileTools.FormatToUnityPath(toPath + "/" + ABBuildTool.GetAssetBundlesPath(filePath)));
                                FileTools.SafeCopyFile(filePath,
                                    FileTools.FormatToUnityPath(toPath2 + "/" + ABBuildTool.GetAssetBundlesPath(filePath)));
                                FileTools.SafeDeleteFile(filePath);
                                FileTools.SafeDeleteFile(filePath + ".meta");
                                
                                FileTools.SafeCopyFile(filePathManifest,
                                    FileTools.FormatToUnityPath(toPath + "/" + ABBuildTool.GetAssetBundlesPath(filePathManifest)));
                                FileTools.SafeCopyFile(filePathManifest,
                                    FileTools.FormatToUnityPath(toPath2 + "/" + ABBuildTool.GetAssetBundlesPath(filePathManifest)));
                                FileTools.SafeDeleteFile(filePathManifest);
                                FileTools.SafeDeleteFile(filePathManifest + ".meta");
                            }
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        
        // 写入资产版本
        public static void WriteAssetVersion()
        {
            string buildPath = F8EditorPrefs.GetString(_prefBuildPathKey, "");
            
            string assetBundleMapPath = Application.dataPath + "/F8Framework/AssetMap/Resources/" + nameof(AssetBundleMap) + ".json";
            FileTools.SafeCopyFile(assetBundleMapPath, buildPath + HotUpdateManager.RemoteDirName + "/" + nameof(AssetBundleMap) + ".json");
            UnityEditor.AssetDatabase.Refresh();
        }

        // 写入游戏版本
        public static void WriteGameVersion()
        {
            string optionalPackage = F8EditorPrefs.GetString(_optionalPackageKey, "");
            
            string toVersion = F8EditorPrefs.GetString(_toVersionKey, "");
            
            string assetRemoteAddress = F8EditorPrefs.GetString(_assetRemoteAddressKey, "");
            
            bool enableHotUpdate = F8EditorPrefs.GetBool(_enableHotUpdateKey, false);
            
            bool _enablePackage = F8EditorPrefs.GetBool(_enablePackageKey, false);
            
            string buildPath = F8EditorPrefs.GetString(_prefBuildPathKey, "");
            
            string gameVersionPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/AssetMap/Resources/" + nameof(GameVersion) + ".json";
            FileTools.SafeDeleteFile(gameVersionPath);
            FileTools.SafeDeleteFile(gameVersionPath + ".meta");
            FileTools.CheckFileAndCreateDirWhenNeeded(gameVersionPath);
            AssetDatabase.Refresh();
            
            List<string> packageList;
            if (!string.IsNullOrEmpty(optionalPackage))
            {
                packageList = new List<string>(optionalPackage.Split(HotUpdateManager.Separator));
            }
            else
            {
                packageList = new List<string>();
            }
            GameVersion gameVersion = new GameVersion(toVersion, assetRemoteAddress, enableHotUpdate, new List<string>(), _enablePackage, packageList);
            // 写入到文件
            string gameVersionResourcesPath = Application.dataPath + "/F8Framework/AssetMap/Resources/" + nameof(GameVersion) + ".json";
            // 序列化对象
            string json = Util.LitJson.ToJson(gameVersion);
            FileTools.SafeDeleteFile(gameVersionResourcesPath);
            FileTools.SafeDeleteFile(gameVersionResourcesPath + ".meta");
            UnityEditor.AssetDatabase.Refresh();
            FileTools.CheckFileAndCreateDirWhenNeeded(gameVersionResourcesPath);
            FileTools.SafeWriteAllText(gameVersionResourcesPath, json);
            // 复制到导出目录
            FileTools.CheckDirAndCreateWhenNeeded(buildPath + HotUpdateManager.RemoteDirName);
            FileTools.SafeCopyFile(gameVersionResourcesPath, buildPath + HotUpdateManager.RemoteDirName + "/" + nameof(GameVersion) + ".json");
            LogF8.LogVersion("写入游戏版本： " + gameVersion.Version);
            UnityEditor.AssetDatabase.Refresh();
        }
        
        // 获取构建平台Group
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