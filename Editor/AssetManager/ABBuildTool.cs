using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class ABBuildTool : ScriptableObject
    {
        private static Dictionary<string, AssetBundleMap.AssetMapping> assetMapping;
        private static Dictionary<string, string[]> resourceMapping;
        // AssetBundle名与资产文件名不同时查找
        private static Dictionary<string, string> DiscrepantAssetPathMapping = new Dictionary<string, string>();

        // 打包后AB名加上MD5（微信小游戏使用）
        private static bool appendHashToAssetBundleName = false;
        
        public static void BuildAllAB()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            
            // 获取“StreamingAssets”文件夹路径（不一定这个文件夹，可自定义）
            string strABOutPAthDir = URLSetting.GetAssetBundlesOutPath();
            
            GenerateAssetNames();
            GenerateResourceNames();
            LogF8.LogAsset("自动设置AssetBundleName（AB名为空时）");
            AssetDatabase.Refresh();
            
            FileTools.CheckDirAndCreateWhenNeeded(strABOutPAthDir);
            AssetDatabase.Refresh();

            Caching.ClearCache();

            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            options |= BuildAssetBundleOptions.DisableLoadAssetByFileName;
            options |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
            if (appendHashToAssetBundleName)
            {
                options |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
            }
            // 打包生成AB包 (目标平台自动根据当前平台设置，WebGL不可使用BuildAssetBundleOptions.None压缩)
            BuildPipeline.BuildAssetBundles(strABOutPAthDir, options, EditorUserBuildSettings.activeBuildTarget);
            LogF8.LogAsset("打包AssetBundle：" + URLSetting.GetAssetBundlesOutPath() + "  当前打包平台：" + EditorUserBuildSettings.activeBuildTarget);
            
            AssetDatabase.Refresh();
            
            // 等待AB打包完成，再写入数据
            GenerateAssetNames(true);
            GenerateResourceNames(true);

            AssetDatabase.Refresh();
            
            // 清理多余文件夹和ab
            DeleteRemovedAssetBundles();
            
            //复制AB到steam打包目录
            string outpath = URLSetting.GetAssetBundlesStreamPath();
            FileTools.SafeClearDir(outpath);
            FileTools.CheckDirAndCreateWhenNeeded(outpath);
            FileTools.SafeCopyDirectory(strABOutPAthDir, outpath, true);
            AssetDatabase.Refresh();
            
            LogF8.LogAsset("资产打包成功!");
        }

        public static void DeleteRemovedAssetBundles()
        {
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesFolder());
            List<string> assetPaths = new List<string>();
            string assetBundlesPath = URLSetting.GetAssetBundlesFolder();
            RecordAssetsAndDirectories(assetBundlesPath, assetBundlesPath, assetPaths, true, true);
            foreach (var pair in assetMapping)
            {
                AssetBundleMap.AssetMapping mapping = pair.Value;
                if (!string.IsNullOrEmpty(mapping.AbName))
                {
                    assetPaths.Add("/" + mapping.AbName);
                }
            }
            
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesOutPath());
            List<string> abPaths = new List<string>();
            string abBundlesPath = URLSetting.GetAssetBundlesOutPath();
            RecordAssetsAndDirectories(abBundlesPath, abBundlesPath, abPaths);
            
            foreach (string ab in abPaths)
            {
                if (!assetPaths.Contains(ab) && !AssetPathsContainsDiscrepantAssetBundle(assetPaths, ab)) 
                {
                    string abpath = URLSetting.GetAssetBundlesOutPath() + ab;
                    if (File.Exists(abpath))
                    {
                        // It's a file, delete the file
                        if (FileTools.SafeDeleteFile(abpath))
                        {
                            FileTools.SafeDeleteFile(abpath + ".meta");
                        }

                        if (!abpath.EndsWith(".manifest"))
                        {
                            LogF8.LogAsset("删除多余AB文件：" + abpath);
                        }
                    }
                    else if (Directory.Exists(abpath))
                    {
                        // It's a folder, delete the folder
                        if (FileTools.SafeDeleteDir(abpath))
                        {
                            LogF8.LogAsset("删除多余AB文件夹：" + abpath);
                            // If the folder is deleted successfully, handle .meta file
                            string metaFilePath = abpath + ".meta";
                            if (File.Exists(metaFilePath))
                            {
                                FileTools.SafeDeleteFile(metaFilePath);
                            }
                        }
                    }
                    else
                    {
                        LogF8.LogAsset("AB文件路径不存在，已经被删除了：" + abpath);
                    }
                }
            }
            LogF8.LogAsset("删除全部的.manifest文件");
            
            AssetDatabase.Refresh();
        }
        
        public static void RecordAssetsAndDirectories(string basePath, string rootPath, List<string> assetPaths, bool removeExtension = false, bool notAddFiles = false)
        {
            Stack<string> stack = new Stack<string>();
            stack.Push(rootPath);

            while (stack.Count > 0)
            {
                string currentPath = stack.Pop();
                string relativePath = currentPath.Replace(basePath, "");

                // Check for directories
                string[] directories = Directory.GetDirectories(currentPath);
                foreach (string directory in directories)
                {
                    stack.Push(directory);
                    assetPaths.Add(FileTools.FormatToUnityPath(directory.Replace(basePath, "").ToLower()));
                }

                if (notAddFiles)
                {
                    continue;
                }
                // Check for files
                string[] files = Directory.GetFiles(currentPath);
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();
                    if (extension != ".meta" && extension != ".ds_store")
                    {
                        // It's a file under AssetBundles, record as "Audio/click11"
                        if (removeExtension)
                        {
                            assetPaths.Add(FileTools.FormatToUnityPath(Path.ChangeExtension(relativePath + "/" + Path.GetFileName(file), null)));
                        }
                        else
                        {
                            assetPaths.Add(FileTools.FormatToUnityPath(relativePath + "/" + Path.GetFileName(file)));
                        }
                    }
                }
            }
        }

        
        //设置资源AB名字
        public static string SetAssetBundleName(string path)
        {
            AssetImporter ai = AssetImporter.GetAtPath(path);
            // 使用 Path.ChangeExtension 去掉扩展名
            string bundleName = Path.ChangeExtension(path, null).Replace(URLSetting.AssetBundlesPath, "").ToLower();
            if (!ai.assetBundleName.Equals(bundleName))
            {
                if (ai.assetBundleName.IsNullOrEmpty())
                {
                    ai.assetBundleName = bundleName;
                    EditorUtility.SetDirty(ai);
                }
                else if (DiscrepantAssetPathMapping != null)
                {
                    // 资产名和ab包名不相等
                    if (!AssetGetParentPath(ai.assetBundleName).Equals(AssetGetParentPath(bundleName)))
                    {
                        LogF8.LogError("资产父路径和AB名不相等，检查是否迁移过文件路径，并清理AB名：" + ai.assetBundleName + " -> " + bundleName + "，资产路径：" + path);
                    }
                    DiscrepantAssetPathMapping["/" + ai.assetBundleName] = "/" + bundleName.ToLower();
                }
            }
            return ai.assetBundleName;
        }
        
        //得到上级路径
        private static string AssetGetParentPath(string path)
        {
            string parentPath = path.Substring(0, path.LastIndexOf('/'));
            return parentPath;
        }
        
        private static bool AssetPathsContainsDiscrepantAssetBundle(List<string> assetPaths, string ab)
        {
            if (DiscrepantAssetPathMapping.TryGetValue(ab, out string disPath))
                return assetPaths.Contains(disPath);
            return false;
        }

        //清除AssetBundleNames
        public static void ClearAllAssetNames()
        {
            ClearAssetNames();
        }

        public static void ClearAssetNames()
        {
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesFolder());
            if (Directory.Exists(URLSetting.GetAssetBundlesFolder()))
            {
                var allPaths = Directory.EnumerateFileSystemEntries(
                    URLSetting.GetAssetBundlesFolder(), 
                    "*", 
                    SearchOption.AllDirectories
                ).Where(str => !str.EndsWith(".meta") && !str.EndsWith(".DS_Store"));

                assetMapping = new Dictionary<string, AssetBundleMap.AssetMapping>();

                foreach (string _filePath in allPaths)
                {
                    string filePath = FileTools.FormatToUnityPath(_filePath);

                    // 获取GetAssetPath
                    string assetPath = GetAssetPath(filePath);
                    
                    if (File.Exists(filePath)) // 文件
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                        ai.assetBundleName = "";
                        EditorUtility.SetDirty(ai);
                    }
                }
            }
        }

        private static IEnumerable<string> allAssetBundlesPaths;
        public static void GenerateAssetNames(bool isWrite = false)
        {
            if (!isWrite)
                DiscrepantAssetPathMapping.Clear();

            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesFolder());
            if (Directory.Exists(URLSetting.GetAssetBundlesFolder()))
            {
                if (allAssetBundlesPaths == null || isWrite == false)
                {
                    allAssetBundlesPaths = Directory.EnumerateFileSystemEntries(
                        URLSetting.GetAssetBundlesFolder(),
                        "*",
                        SearchOption.AllDirectories
                    ).Where(str => !str.EndsWith(".meta") && !str.EndsWith(".DS_Store"));
                }
                
                List<string> tempNames = new List<string>();

                assetMapping = new Dictionary<string, AssetBundleMap.AssetMapping>();

                foreach (string _filePath in allAssetBundlesPaths)
                {
                    string filePath = FileTools.FormatToUnityPath(_filePath);

                    // 获取不带扩展名的文件名
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    
                    // 获取GetAssetPath
                    string assetPath = GetAssetPath(filePath);
                    
                    if (File.Exists(filePath)) // 文件
                    {
                        if (!isWrite)
                        {
                            SetAssetBundleName(assetPath);
                            continue;
                        }
                        
                        string abName = AssetImporter.GetAtPath(assetPath).assetBundleName;

                        if (tempNames.Contains(fileNameWithoutExtension.ToLower()))
                        {
                            string id = Util.Encryption.MD5Encrypt(assetPath);
                            fileNameWithoutExtension += id;
                            LogF8.Log("AB资源名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                        }
                        tempNames.Add(fileNameWithoutExtension.ToLower());

                        // 只留下一个assetPath
                        List<string> assetPathsForAbName = new List<string>();
                        assetPathsForAbName.Add(assetPath.ToLower());
                        
                        string hash = appendHashToAssetBundleName ? ParseManifestFile(URLSetting.GetAssetBundlesOutPath() + "/" + abName) : null;
                        
                        string realAbName = InsertBeforeLastDot(abName, hash);
                        
                        assetMapping.Add(fileNameWithoutExtension, new AssetBundleMap.AssetMapping(realAbName, assetPathsForAbName.ToArray(),
                            BuildPkgTool.ToVersion, FileTools.GetFileSize(URLSetting.GetAssetBundlesOutPath() + "/" + realAbName).ToString(),
                            FileTools.CreateMd5ForFile(URLSetting.GetAssetBundlesOutPath() + "/" + realAbName), GetPackage(filePath), ""));

                        if (filePath.IsContainChinese())
                        {
                            LogF8.LogError("AssetBundle名中不推荐含有中文： " + filePath);
                        }
                    }
                    else if (Directory.Exists(filePath)) // 文件夹
                    {
                        if (!isWrite)
                        {
                            continue;
                        }
                       
                        // 文件夹资产信息，使用资产名名代替
                        string[] assetNameDir = Directory.GetFiles(filePath, "*", SearchOption.TopDirectoryOnly)
                            .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
                            .Select(path => Path.GetFileNameWithoutExtension(path))
                            .ToArray();
                        
                        fileNameWithoutExtension += AssetManager.DirSuffix;
                        
                        if (tempNames.Contains(fileNameWithoutExtension))
                        {
                            string id = Util.Encryption.MD5Encrypt(assetPath);
                            fileNameWithoutExtension += id;
                            LogF8.Log("AB文件夹名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                        }
                        tempNames.Add(fileNameWithoutExtension);
                        
                        assetMapping.Add(fileNameWithoutExtension, new AssetBundleMap.AssetMapping("", assetNameDir,
                            BuildPkgTool.ToVersion, "", "", "", ""));

                        if (filePath.IsContainChinese())
                        {
                            LogF8.LogError("AssetBundle文件夹中不推荐含有中文： " + filePath);
                        }
                    }
                }

                if (isWrite)
                {
                    // 把总的manifest加上
                    if (tempNames.Contains(URLSetting.GetPlatformName()))
                    {
                        LogF8.LogError("总AssetBundleManifest和其他资产名重复，请检查资产：" + URLSetting.GetPlatformName());
                    }
                    else
                    {
                        if (File.Exists(URLSetting.GetAssetBundlesOutPath() + "/" + URLSetting.GetPlatformName()))
                        {
                            assetMapping.Add(URLSetting.GetPlatformName(), new AssetBundleMap.AssetMapping(URLSetting.GetPlatformName(), new string[]{},
                                BuildPkgTool.ToVersion, FileTools.GetFileSize(URLSetting.GetAssetBundlesOutPath() + "/" + URLSetting.GetPlatformName()).ToString(),
                                FileTools.CreateMd5ForFile(URLSetting.GetAssetBundlesOutPath() + "/" + URLSetting.GetPlatformName()), "", ""));
                        }
                    }

                    WriteAssetNames();
                }
            }
        }
        
        private static void WriteAssetNames()
        {
            string assetMapPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/AssetMap/Resources/" + nameof(AssetBundleMap) + ".json";
            FileTools.SafeDeleteFile(assetMapPath);
            FileTools.SafeDeleteFile(assetMapPath + ".meta");
            FileTools.CheckFileAndCreateDirWhenNeeded(assetMapPath);
            AssetDatabase.Refresh();
                
            string AssetBundleMapPath = Application.dataPath + "/F8Framework/AssetMap/Resources/" + nameof(AssetBundleMap) + ".json";
            FileTools.CheckFileAndCreateDirWhenNeeded(AssetBundleMapPath);
            FileTools.SafeWriteAllText(AssetBundleMapPath, Util.LitJson.ToJson(assetMapping));
            AssetDatabase.Refresh();
            
            LogF8.LogAsset("写入AssetBundles资产数据 生成：" + AssetBundleMapPath);
        }
        
        public static void GenerateResourceNames(bool isWrite = false)
        {
            if (!isWrite)
            {
                return;
            }
            string[] dics = Directory.GetDirectories(Application.dataPath, "Resources", SearchOption.AllDirectories);
            
            List<string> tempNames = new List<string>();
            
            resourceMapping = new Dictionary<string, string[]>();
            
            foreach (string dic in dics)
            {
                var allPaths = Directory.EnumerateFileSystemEntries(
                    dic, 
                    "*", 
                    SearchOption.AllDirectories
                ).Where(str => !str.EndsWith(".meta") && !str.EndsWith(".DS_Store"));
                
                foreach (string _filePath in allPaths)
                {
                    string filePath = FileTools.FormatToUnityPath(_filePath);

                    string assetPath = GetAssetPath(filePath);
                    
                    // 获取不带扩展名的文件名
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                    if (File.Exists(filePath)) // 文件
                    {
                        string notSuffix = Path.ChangeExtension(filePath, null);

                        string resourcesPath = GetResourcesPath(notSuffix);

                        string realPath = resourcesPath.Replace(URLSetting.ResourcesPath, "");

                        if (tempNames.Contains(fileNameWithoutExtension))
                        {
                            string id = Util.Encryption.MD5Encrypt(assetPath);
                            fileNameWithoutExtension += id;
                            LogF8.Log("Resources资源名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                        }

                        tempNames.Add(fileNameWithoutExtension);

                        resourceMapping.Add(fileNameWithoutExtension, new[] { realPath });
                    }
                    else if (Directory.Exists(filePath)) // 文件夹
                    {
                        // 文件夹资产信息，使用资产名名代替
                        string[] assetNameDir = Directory.GetFiles(filePath, "*", SearchOption.TopDirectoryOnly)
                            .Where(path =>
                                !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) &&
                                !path.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
                            .Select(path => Path.GetFileNameWithoutExtension(path))
                            .ToArray();

                        fileNameWithoutExtension += AssetManager.DirSuffix;

                        if (tempNames.Contains(fileNameWithoutExtension))
                        {
                            string id = Util.Encryption.MD5Encrypt(assetPath);
                            fileNameWithoutExtension += id;
                            LogF8.Log("Resources文件夹名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                        }

                        tempNames.Add(fileNameWithoutExtension);

                        resourceMapping.Add(fileNameWithoutExtension, assetNameDir);
                    }
                }
            }

            WriteResourceNames();
        }
        
        private static void WriteResourceNames()
        {
            string resourceMapPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/AssetMap/Resources/" + nameof(ResourceMap) + ".json";
            FileTools.SafeDeleteFile(resourceMapPath);
            FileTools.SafeDeleteFile(resourceMapPath + ".meta");
            FileTools.CheckFileAndCreateDirWhenNeeded(resourceMapPath);
            AssetDatabase.Refresh();
            
            string ResourceMapPath = Application.dataPath + "/F8Framework/AssetMap/Resources/" + nameof(ResourceMap) + ".json";
            FileTools.CheckFileAndCreateDirWhenNeeded(ResourceMapPath);
            FileTools.SafeWriteAllText(ResourceMapPath, Util.LitJson.ToJson(resourceMapping));
            AssetDatabase.Refresh();
            
            LogF8.LogAsset("写入Resources资产数据 生成：" + ResourceMapPath);
        }

        private static string GetPackage(string path)
        {
            // 使用正则表达式切割地址
            string[] packages = Regex.Split(path, @"[\\/]");
            
            foreach (var package in packages)
            {
                // 判断地址中是否包含"Package_"
                int index = package.IndexOf(HotUpdateManager.PackageSplit);
                if (index != -1)
                {
                    // 如果包含，则获取"Package_"后面的所有数据
                    string part = package.Substring(index + HotUpdateManager.PackageSplit.Length);
                    return part;
                }
            }

            return "";
        }
        
        public static string GetAssetBundlesPath(string fullPath)
        {
            Regex rgx = new Regex(@"AssetBundles[\\/].+$");
            Match matches = rgx.Match(fullPath);

            string assetPath = "";
            if (matches.Success)
                assetPath = matches.Value;

            assetPath = FileTools.FormatToUnityPath(assetPath);
            return assetPath;
        }
        
        public static string GetAssetPath(string fullPath)
        {
            Regex rgx = new Regex(@"Assets[\\/].+$");
            Match matches = rgx.Match(fullPath);

            string assetPath = "";
            if (matches.Success)
                assetPath = matches.Value;

            assetPath = FileTools.FormatToUnityPath(assetPath);
            return assetPath;
        }
        
        public static string GetResourcesPath(string fullPath)
        {
            Regex rgx = new Regex(@"Resources[\\/].+$");
            Match matches = rgx.Match(fullPath);

            string assetPath = "";
            if (matches.Success)
                assetPath = matches.Value;

            assetPath = FileTools.FormatToUnityPath(assetPath);
            return assetPath;
        }
        
        
        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<ABBuildTool>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);

            // 获取绝对路径并规范化
            string scriptPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptRelativePath));

            return scriptPath;
        }
        
        private static string ParseManifestFile(string manifestPath)
        {
            manifestPath += ".manifest";
            if (!File.Exists(manifestPath))
            {
                LogF8.LogError($"Manifest 文件不存在: {manifestPath}");
                return null;
            }

            string[] lines = File.ReadAllLines(manifestPath);

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("Hash:"))
                {
                    // 找到 "Hash:" 行，提取哈希值
                    return line.Trim().Substring(5).Trim();
                }
            }
            LogF8.LogError("Manifest 文件内不存在 Hash:");
            return null;
        }
        
        private static string InsertBeforeLastDot(string original, string insertStr = null)
        {
            if (string.IsNullOrEmpty(original) || insertStr == null)
                return original;

            insertStr = "_" + insertStr;
            
            // 找到最后一个 '.' 的位置
            int lastDotIndex = original.LastIndexOf('.');
        
            if (lastDotIndex < 0)
            {
                // 如果没有 '.'，直接附加到末尾
                return original + insertStr;
            }

            // 分割字符串：左边 + 插入内容 + 右边
            string leftPart = original.Substring(0, lastDotIndex);
            string rightPart = original.Substring(lastDotIndex); // 包括 '.'

            return leftPart + insertStr + rightPart;
        }
    }
}
