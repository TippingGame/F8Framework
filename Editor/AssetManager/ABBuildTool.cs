using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class ABBuildTool : ScriptableObject
    {
        private static Dictionary<string, AssetBundleMap.AssetMapping> assetMapping;
        private static Dictionary<string, string[]> resourceMapping;
        private static Dictionary<string, string> manifestLogicalPathByBundlePath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        // AssetBundle名与资产文件名不同时查找
        private static Dictionary<string, string> DiscrepantAssetPathMapping = new Dictionary<string, string>();
        
        public static void JenkinsBuildAllAB()
        {
            SessionState.SetBool("compilationFinishedBuildAB", false);
            string[] args = Environment.GetCommandLineArgs();
            bool enableFullPathAssetLoading = string.Equals(BuildPkgTool.GetArgValue(args, "EnableFullPathAssetLoading-"), "true", StringComparison.OrdinalIgnoreCase);
            bool enableFullPathExtensionAssetLoading = string.Equals(BuildPkgTool.GetArgValue(args, "EnableFullPathExtensionAssetLoading-"), "true", StringComparison.OrdinalIgnoreCase);
            bool forceRebuildAssetBundle = string.Equals(BuildPkgTool.GetArgValue(args, "ForceRebuildAssetBundle-"), "true", StringComparison.OrdinalIgnoreCase);
            bool appendHashToAssetBundleName = string.Equals(BuildPkgTool.GetArgValue(args, "AppendHashToAssetBundleName-"), "true", StringComparison.OrdinalIgnoreCase);
            bool forceRemoteAssetBundle = string.Equals(BuildPkgTool.GetArgValue(args, "ForceRemoteAssetBundle-"), "true", StringComparison.OrdinalIgnoreCase);
            bool disableUnityCacheOnWebGL = string.Equals(BuildPkgTool.GetArgValue(args, "DisableUnityCacheOnWebGL-"), "true", StringComparison.OrdinalIgnoreCase);
            string assetManifestEncryptKey = BuildPkgTool.GetArgValue(args, "AssetManifestEncryptKey-") ?? "";
            string assetBundleNameSuffix = BuildPkgTool.GetArgValue(args, "AssetBundleNameSuffix-") ?? "";
            int assetBundleOffset = 0;
            if (int.TryParse(BuildPkgTool.GetArgValue(args, "AssetBundleOffset-"), out int intValue))
            {
                assetBundleOffset = Math.Clamp(intValue, 0, 245);
            }
            int assetBundleXorKey = 0;
            if (int.TryParse(BuildPkgTool.GetArgValue(args, "AssetBundleXorKey-"), out int intValue2))
            {
                assetBundleXorKey = Math.Clamp(intValue2, 0, 245);
            }
            F8EditorPrefs.SetBool(BuildPkgTool.EnableFullPathAssetLoadingKey, enableFullPathAssetLoading);
            F8EditorPrefs.SetBool(BuildPkgTool.EnableFullPathExtensionAssetLoadingKey, enableFullPathExtensionAssetLoading);
            F8EditorPrefs.SetBool(BuildPkgTool.ForceRebuildAssetBundleKey, forceRebuildAssetBundle);
            F8GamePrefs.SetBool(nameof(F8GameConfig.AppendHashToAssetBundleName), appendHashToAssetBundleName);
            F8GamePrefs.SetBool(nameof(F8GameConfig.ForceRemoteAssetBundle), forceRemoteAssetBundle);
            F8GamePrefs.SetBool(nameof(F8GameConfig.DisableUnityCacheOnWebGL), disableUnityCacheOnWebGL);
            F8GamePrefs.SetInt(nameof(F8GameConfig.AssetBundleOffset), assetBundleOffset);
            F8GamePrefs.SetInt(nameof(F8GameConfig.AssetBundleXorKey), assetBundleXorKey);
            F8GamePrefs.SetString(nameof(F8GameConfig.AssetManifestEncryptKey), assetManifestEncryptKey);
            F8EditorPrefs.SetString(BuildPkgTool.AssetBundleNameSuffixKey, assetBundleNameSuffix);
            BuildAllAB();
        }

        public static void BuildAllAB()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            string assetBundleNameSuffix = GetValidatedAssetBundleNameSuffix();
            
            // 获取“StreamingAssets”文件夹路径（不一定这个文件夹，可自定义）
            string strABOutPAthDir = URLSetting.GetAssetBundlesOutPath();
            
            GenerateAssetNames();
            F8EditorPrefs.SetString(BuildPkgTool.AppliedAssetBundleNameSuffixKey, assetBundleNameSuffix);
            GenerateResourceNames();
            LogF8.LogAsset("自动设置AssetBundleName（AB名为空时）");
            AssetDatabase.Refresh();

            ValidateAssetBundleNamesOrThrow(strABOutPAthDir);
            
            FileTools.CheckDirAndCreateWhenNeeded(strABOutPAthDir);
            AssetDatabase.Refresh();

            Caching.ClearCache();

            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            options |= BuildAssetBundleOptions.DisableLoadAssetByFileName;
            options |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
            options |= BuildAssetBundleOptions.StrictMode;
            if (F8GamePrefs.GetBool(nameof(F8GameConfig.AppendHashToAssetBundleName)))
            {
                options |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
            }
            if (F8EditorPrefs.GetBool(BuildPkgTool.ForceRebuildAssetBundleKey, false))
            {
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }
            // 打包生成AB包 (目标平台自动根据当前平台设置，WebGL不可使用BuildAssetBundleOptions.None压缩)
            BuildPipeline.BuildAssetBundles(strABOutPAthDir, options, EditorUserBuildSettings.activeBuildTarget);
            LogF8.LogAsset("打包AssetBundle：" + URLSetting.GetAssetBundlesOutPath() + "  当前打包平台：" + EditorUserBuildSettings.activeBuildTarget);
            
            AssetDatabase.Refresh();
            
            // 加密AB
            AssetBundleEncrypt(strABOutPAthDir);
            
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
            FileTools.SafeCopyDirectory(strABOutPAthDir, outpath, true, new[] { ".manifest" });
            AssetDatabase.Refresh();
            
            LogF8.LogAsset("资产打包成功!");
        }

        private static void ValidateAssetBundleNamesOrThrow(string outputPath)
        {
            string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
            List<KeyValuePair<string, string>> pathConflicts = FindAssetBundlePathConflicts(bundleNames);
            List<string> mixedSceneBundleNames = new List<string>();

            foreach (string bundleName in bundleNames)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                    .Where(path => !AssetDatabase.IsValidFolder(path))
                    .ToArray();
                bool hasScene = assetPaths.Any(path => path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase));
                bool hasNonSceneAsset = assetPaths.Any(path => !path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase));
                if (hasScene && hasNonSceneAsset)
                {
                    mixedSceneBundleNames.Add(bundleName);
                }
            }

            if (pathConflicts.Count == 0 && mixedSceneBundleNames.Count == 0)
            {
                return;
            }

            StringBuilder message = new StringBuilder();
            message.AppendLine("AssetBundle打包预检失败，已停止打包。");

            if (pathConflicts.Count > 0)
            {
                message.AppendLine("检测到AB文件与文件夹路径冲突：");
                foreach (KeyValuePair<string, string> conflict in pathConflicts)
                {
                    string conflictOutputPath = FileTools.FormatToUnityPath(Path.Combine(outputPath, conflict.Key));
                    message.AppendLine($"- AB文件：{conflict.Key}");
                    message.AppendLine($"  目录内AB：{conflict.Value}");
                    message.AppendLine($"  冲突路径：{conflictOutputPath}");
                    message.AppendLine("  原因：该路径既需要作为AB文件，又需要作为其他AB的输出目录。");
                    AppendAssetBundlePaths(message, conflict.Key, "  AB文件资源");
                    AppendAssetBundlePaths(message, conflict.Value, "  目录内AB资源");
                }

                message.AppendLine("解决建议：在打包工具的“自动生成AB名的自定义后缀”中填写“.bundle”，或调整AB名，避免一个完整AB名成为另一个AB名的目录前缀。必要时请清理旧的手动/文件夹AB名后重试。");
            }

            if (mixedSceneBundleNames.Count > 0)
            {
                message.AppendLine("检测到场景与普通资源被分配到同一个AB：");
                foreach (string bundleName in mixedSceneBundleNames)
                {
                    message.AppendLine($"- AB名：{bundleName}");
                    AppendAssetBundlePaths(message, bundleName, "  包内资源");
                }

                message.AppendLine("解决建议：Unity不允许把显式标记的场景和普通资源打进同一个AB，请为它们设置不同的AB名，并检查文件夹上的AB名是否被子资源继承。");
            }

            string errorMessage = message.ToString();
            LogF8.LogError(errorMessage);
            throw new BuildFailedException(errorMessage);
        }

        private static List<KeyValuePair<string, string>> FindAssetBundlePathConflicts(IEnumerable<string> bundleNames)
        {
            string[] normalizedBundleNames = bundleNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => FileTools.FormatToUnityPath(name).Trim('/'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            HashSet<string> bundleNameSet = new HashSet<string>(normalizedBundleNames, StringComparer.OrdinalIgnoreCase);
            List<KeyValuePair<string, string>> conflicts = new List<KeyValuePair<string, string>>();

            foreach (string bundleName in normalizedBundleNames)
            {
                int separatorIndex = bundleName.IndexOf('/');
                while (separatorIndex >= 0)
                {
                    string parentPath = bundleName.Substring(0, separatorIndex);
                    if (bundleNameSet.Contains(parentPath))
                    {
                        conflicts.Add(new KeyValuePair<string, string>(parentPath, bundleName));
                    }

                    separatorIndex = bundleName.IndexOf('/', separatorIndex + 1);
                }
            }

            return conflicts;
        }

        private static void AppendAssetBundlePaths(StringBuilder message, string bundleName, string title)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                .Where(path => !AssetDatabase.IsValidFolder(path))
                .ToArray();
            const int maxDisplayedAssetCount = 5;
            foreach (string assetPath in assetPaths.Take(maxDisplayedAssetCount))
            {
                message.AppendLine($"{title}：{assetPath}");
            }

            if (assetPaths.Length > maxDisplayedAssetCount)
            {
                message.AppendLine($"{title}：还有{assetPaths.Length - maxDisplayedAssetCount}个资源未显示");
            }
        }

        public static bool TryValidateAssetBundleNameSuffix(string suffix, out string errorMessage)
        {
            suffix ??= "";
            errorMessage = null;
            if (suffix.Length == 0)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(suffix) || !suffix.Equals(suffix.Trim(), StringComparison.Ordinal))
            {
                errorMessage = "自动AB名后缀不能只包含空白，也不能以空白开头或结尾。";
                return false;
            }

            if (suffix.Contains('/') || suffix.Contains('\\') || suffix.Contains(".."))
            {
                errorMessage = "自动AB名后缀不能包含路径分隔符或连续的点号。";
                return false;
            }

            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            const string crossPlatformInvalidFileNameChars = "<>:\"|?*";
            if (suffix.IndexOfAny(invalidFileNameChars) >= 0 ||
                suffix.IndexOfAny(crossPlatformInvalidFileNameChars.ToCharArray()) >= 0 ||
                suffix.EndsWith(".", StringComparison.Ordinal))
            {
                errorMessage = "自动AB名后缀包含文件名不允许使用的字符。";
                return false;
            }

            string extension = Path.GetExtension("asset" + suffix);
            if (extension.Equals(".manifest", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".meta", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".ds_store", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = $"自动AB名后缀不能以保留扩展名 {extension} 结尾。";
                return false;
            }

            return true;
        }

        private static string GetValidatedAssetBundleNameSuffix()
        {
            string suffix = F8EditorPrefs.GetString(BuildPkgTool.AssetBundleNameSuffixKey, "") ?? "";
            if (TryValidateAssetBundleNameSuffix(suffix, out string errorMessage))
            {
                return suffix;
            }

            string message = "AssetBundle打包预检失败，已停止打包。\n" + errorMessage;
            LogF8.LogError(message);
            throw new BuildFailedException(message);
        }

        public static void DeleteRemovedAssetBundles()
        {
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesFolder());
            HashSet<string> expectedDirectoryPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> expectedBundlePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> expectedManifestPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string assetBundlesPath = URLSetting.GetAssetBundlesFolder();
            List<string> sourceAssetPaths = new List<string>();
            RecordAssetsAndDirectories(assetBundlesPath, assetBundlesPath, sourceAssetPaths, true, true);
            foreach (string assetPath in sourceAssetPaths)
            {
                expectedDirectoryPaths.Add(assetPath);
            }

            foreach (var pair in assetMapping)
            {
                AssetBundleMap.AssetMapping mapping = pair.Value;
                if (!string.IsNullOrEmpty(mapping.AbName))
                {
                    string bundlePath = "/" + mapping.AbName;
                    expectedBundlePaths.Add(bundlePath);
                    expectedManifestPaths.Add(GetExpectedManifestPath(bundlePath));
                }
            }
            
            string abBundlesPath = URLSetting.GetAssetBundlesOutPath();
            FileTools.CheckDirAndCreateWhenNeeded(abBundlesPath);

            foreach (string filePath in Directory.GetFiles(abBundlesPath, "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".meta" || extension == ".ds_store")
                {
                    continue;
                }

                string tempFilePath = FileTools.FormatToUnityPath(filePath);
                string relativePath = ToOutputRelativePath(abBundlesPath, tempFilePath);
                if (extension == ".manifest")
                {
                    string manifestLogicalPath = FileTools.FormatToUnityPath(Path.ChangeExtension(relativePath, null));
                    if (!expectedManifestPaths.Contains(manifestLogicalPath) &&
                        !AssetPathsContainsDiscrepantAssetBundle(expectedManifestPaths, manifestLogicalPath))
                    {
                        DeleteFileAndMeta(tempFilePath);
                        LogF8.LogAsset("删除多余AB.manifest文件：" + tempFilePath);
                    }
                }
                else
                {
                    if (!expectedBundlePaths.Contains(relativePath) &&
                        !AssetPathsContainsDiscrepantAssetBundle(expectedBundlePaths, relativePath))
                    {
                        DeleteFileAndMeta(tempFilePath);
                        LogF8.LogAsset("删除多余AB文件：" + tempFilePath);
                    }
                }
            }

            foreach (string directoryPath in Directory.GetDirectories(abBundlesPath, "*", SearchOption.AllDirectories)
                         .OrderByDescending(path => path.Length))
            {
                string tempDirectoryPath = FileTools.FormatToUnityPath(directoryPath);
                string relativePath = ToOutputRelativePath(abBundlesPath, tempDirectoryPath);
                if (!expectedDirectoryPaths.Contains(relativePath))
                {
                    if (FileTools.SafeDeleteDir(tempDirectoryPath))
                    {
                        DeleteFileIfExists(tempDirectoryPath + ".meta");
                        LogF8.LogAsset("删除多余AB文件夹：" + tempDirectoryPath);
                    }
                }
            }

            foreach (string metaPath in Directory.GetFiles(abBundlesPath, "*.meta", SearchOption.AllDirectories))
            {
                string ownerPath = metaPath.Substring(0, metaPath.Length - ".meta".Length);
                if (!File.Exists(ownerPath) && !Directory.Exists(ownerPath))
                {
                    DeleteFileIfExists(metaPath);
                }
            }
            
            AssetDatabase.Refresh();
        }

        private static bool DeleteFileIfExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            return FileTools.SafeDeleteFile(filePath);
        }

        private static void DeleteFileAndMeta(string filePath)
        {
            if (DeleteFileIfExists(filePath))
            {
                DeleteFileIfExists(filePath + ".meta");
            }
        }

        private static string ToOutputRelativePath(string basePath, string targetPath)
        {
            return targetPath.Replace(basePath, "");
        }

        private static string GetExpectedManifestPath(string bundlePath)
        {
            if (string.IsNullOrEmpty(bundlePath))
            {
                return string.Empty;
            }

            if (manifestLogicalPathByBundlePath.TryGetValue(bundlePath, out string manifestLogicalPath))
            {
                return manifestLogicalPath;
            }

            return bundlePath;
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
                        if (removeExtension || extension == ".manifest")
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
            string defaultBundleName = Path.ChangeExtension(path, null).Replace(URLSetting.AssetBundlesPath, "").ToLowerInvariant();
            string assetBundleNameSuffix = GetValidatedAssetBundleNameSuffix();
            string bundleName = (defaultBundleName + assetBundleNameSuffix).ToLowerInvariant();
            if (!ai.assetBundleName.Equals(bundleName))
            {
                string appliedSuffix = F8EditorPrefs.GetString(BuildPkgTool.AppliedAssetBundleNameSuffixKey, "") ?? "";
                string previousAutoBundleName = (defaultBundleName + appliedSuffix).ToLowerInvariant();
                bool isDefaultAutoBundleName = ai.assetBundleName.Equals(defaultBundleName, StringComparison.OrdinalIgnoreCase) ||
                                               F8EditorPrefs.HasKey(BuildPkgTool.AppliedAssetBundleNameSuffixKey) &&
                                               ai.assetBundleName.Equals(previousAutoBundleName, StringComparison.OrdinalIgnoreCase);
                if (ai.assetBundleName.IsNullOrEmpty() || isDefaultAutoBundleName)
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

        public static string GetAutoAssetBundleName(string path)
        {
            string defaultBundleName = Path.ChangeExtension(path, null)
                .Replace(URLSetting.AssetBundlesPath, "")
                .ToLowerInvariant();
            return (defaultBundleName + GetValidatedAssetBundleNameSuffix()).ToLowerInvariant();
        }
        
        //得到上级路径
        private static string AssetGetParentPath(string path)
        {
            int index = path.LastIndexOf('/');
            return index >= 0 ? path.Substring(0, index) : "";
        }
        
        private static bool AssetPathsContainsDiscrepantAssetBundle(ICollection<string> assetPaths, string ab)
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
                manifestLogicalPathByBundlePath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
            bool _enableFullPathAssetLoading = F8EditorPrefs.GetBool(BuildPkgTool.EnableFullPathAssetLoadingKey, false);
            bool _enableFullPathExtensionAssetLoading = F8EditorPrefs.GetBool(BuildPkgTool.EnableFullPathExtensionAssetLoadingKey, false);
            
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
                            if (!_enableFullPathAssetLoading && !_enableFullPathExtensionAssetLoading)
                            {
                                LogF8.Log("AB资源名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                            }
                        }
                        tempNames.Add(fileNameWithoutExtension.ToLower());

                        // 只留下一个assetPath
                        List<string> assetPathsForAbName = new List<string>();
                        assetPathsForAbName.Add(assetPath.ToLower());
                        
                        string hash =  null;
                        if (F8GamePrefs.GetBool(nameof(F8GameConfig.AppendHashToAssetBundleName)))
                        {
                            BuildPipeline.GetHashForAssetBundle(URLSetting.GetAssetBundlesOutPath() + "/" + abName, out Hash128 hash128);
                            hash = hash128.ToString();
                        }
                        
                        string realAbName = InsertBeforeLastDot(abName, hash);
                        manifestLogicalPathByBundlePath["/" + realAbName] = "/" + abName;
                        string[] assetPathsArray = assetPathsForAbName.ToArray();
                        string version = BuildPkgTool.ToVersion;
                        string abFullPath = URLSetting.GetAssetBundlesOutPath() + "/" + realAbName;
                        string fileSize = FileTools.GetFileSize(abFullPath).ToString();
                        string md5 = FileTools.CreateMd5ForFile(abFullPath);
                        string package = GetPackage(filePath);
                        assetMapping.Add(fileNameWithoutExtension, new AssetBundleMap.AssetMapping(realAbName, assetPathsArray, version, fileSize,
                            md5, package, ""));

                        if (_enableFullPathAssetLoading)
                        {
                            assetMapping.TryAdd(Path.ChangeExtension(GetAssetBundlesPath(filePath), null), 
                                new AssetBundleMap.AssetMapping(realAbName, assetPathsArray, version, fileSize,
                                md5, package, ""));
                        }

                        if (_enableFullPathExtensionAssetLoading)
                        {
                            assetMapping.TryAdd(GetAssetBundlesPath(filePath), new AssetBundleMap.AssetMapping(realAbName, assetPathsArray, version, fileSize,
                                md5, package, ""));
                        }
                        
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
                            if (!_enableFullPathAssetLoading && !_enableFullPathExtensionAssetLoading)
                            {
                                LogF8.Log("AB文件夹名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                            }
                        }
                        tempNames.Add(fileNameWithoutExtension);
                        
                        assetMapping.Add(fileNameWithoutExtension, new AssetBundleMap.AssetMapping("", assetNameDir,
                            BuildPkgTool.ToVersion, "", "", "", ""));
                        
                        if (_enableFullPathAssetLoading || _enableFullPathExtensionAssetLoading)
                        {
                            assetMapping.TryAdd(GetAssetBundlesPath(filePath) + AssetManager.DirSuffix, new AssetBundleMap.AssetMapping("", assetNameDir,
                                BuildPkgTool.ToVersion, "", "", "", ""));
                        }
                        
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
                        string platformManifestPath = URLSetting.GetAssetBundlesOutPath() + "/" + URLSetting.GetPlatformName();
                        if (File.Exists(platformManifestPath) && assetMapping.Count > 0)
                        {
                            string platformManifestAbName = URLSetting.GetPlatformName();
                            if (F8GamePrefs.GetBool(nameof(F8GameConfig.AppendHashToAssetBundleName)))
                            {
                                platformManifestAbName = RenamePlatformManifestWithMd5(platformManifestPath);
                                platformManifestPath = URLSetting.GetAssetBundlesOutPath() + "/" + platformManifestAbName;
                            }

                            manifestLogicalPathByBundlePath["/" + platformManifestAbName] = "/" + URLSetting.GetPlatformName();
                            assetMapping.Add(URLSetting.GetPlatformName(), new AssetBundleMap.AssetMapping(platformManifestAbName, new string[]{},
                                BuildPkgTool.ToVersion, FileTools.GetFileSize(platformManifestPath).ToString(),
                                FileTools.CreateMd5ForFile(platformManifestPath), "", ""));
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
            F8JsonEncryption.WriteJsonToFile(AssetBundleMapPath, Util.LitJson.ToJson(assetMapping));
            AssetDatabase.Refresh();
            
            LogF8.LogAsset("写入AssetBundles资产数据 生成：" + AssetBundleMapPath);
        }
        
        public static void GenerateResourceNames(bool isWrite = false)
        {
            bool _enableFullPathAssetLoading = F8EditorPrefs.GetBool(BuildPkgTool.EnableFullPathAssetLoadingKey, false);
            bool _enableFullPathExtensionAssetLoading = F8EditorPrefs.GetBool(BuildPkgTool.EnableFullPathExtensionAssetLoadingKey, false);
            
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
                            if (!_enableFullPathAssetLoading && !_enableFullPathExtensionAssetLoading)
                            {
                                LogF8.Log("Resources资源名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                            }
                        }

                        tempNames.Add(fileNameWithoutExtension);

                        resourceMapping.Add(fileNameWithoutExtension, new[] { realPath });

                        if (_enableFullPathAssetLoading)
                        {
                            resourceMapping.TryAdd(Path.ChangeExtension(GetResourcesPath(filePath), null), new[] { realPath });
                        }

                        if (_enableFullPathExtensionAssetLoading)
                        {
                            resourceMapping.TryAdd(GetResourcesPath(filePath), new[] { realPath });
                        }
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
                            if (!_enableFullPathAssetLoading && !_enableFullPathExtensionAssetLoading)
                            {
                                LogF8.Log("Resources文件夹名称重复（大小写不敏感）：" + filePath + "，增加唯一识别ID后为：" + fileNameWithoutExtension);
                            }
                        }

                        tempNames.Add(fileNameWithoutExtension);

                        resourceMapping.Add(fileNameWithoutExtension, assetNameDir);
                        
                        if (_enableFullPathAssetLoading || _enableFullPathExtensionAssetLoading)
                        {
                            resourceMapping.TryAdd(GetResourcesPath(filePath) + AssetManager.DirSuffix, assetNameDir);
                        }
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
            F8JsonEncryption.WriteJsonToFile(ResourceMapPath, Util.LitJson.ToJson(resourceMapping));
            AssetDatabase.Refresh();
            
            LogF8.LogAsset("写入Resources资产数据 生成：" + ResourceMapPath);
        }

        private static void AssetBundleEncrypt(string sourceDir)
        {
            int offsetValue = F8GamePrefs.GetInt(nameof(F8GameConfig.AssetBundleOffset));
            int xorKey = F8GamePrefs.GetInt(nameof(F8GameConfig.AssetBundleXorKey));
            if (xorKey == 0 && offsetValue == 0)
                return;

            if (!Directory.Exists(sourceDir))
            {
                LogF8.LogError($"目录不存在: {sourceDir}");
                return;
            }

            List<string> excludeExtensions = new List<string> { ".meta", ".manifest", ".DS_Store" };
            List<string> excludeFileNames = new List<string> { };

            try
            {
                string[] allFiles = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
                int abCount = 0;
                int encryptedCount = 0;

                foreach (string filePath in allFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    string fileExtension = Path.GetExtension(filePath);

                    if (excludeExtensions.Contains(fileExtension) ||
                        excludeFileNames.Contains(fileName))
                    {
                        continue;
                    }
                    abCount++;
                    
                    if (EncryptFile(filePath))
                        encryptedCount++;
                }

                LogF8.LogAsset($"加密完成！总共有 {abCount} 个AB文件，加密处理 {encryptedCount} 个文件（不会重复加密）");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                LogF8.LogError($"加密过程中发生错误: {e.Message}");
            }
        }

        /// <summary>
        /// 加密单个文件
        /// </summary>
        private static bool EncryptFile(string filePath)
        {
            try
            {
                byte[] fileHeader = new byte[32];
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int bytesRead = fs.Read(fileHeader, 0, fileHeader.Length);
                    if (bytesRead < 6)
                    {
                        LogF8.LogAsset($"文件过小，跳过加密: {filePath}");
                        return false;
                    }
                }

                string headerString = System.Text.Encoding.UTF8.GetString(fileHeader, 0, 6);
                if (!headerString.StartsWith("Unity"))
                {
                    return false;
                }

                int offsetValue = F8GamePrefs.GetInt(nameof(F8GameConfig.AssetBundleOffset));
                int xorKey = F8GamePrefs.GetInt(nameof(F8GameConfig.AssetBundleXorKey));

                byte[] plainBytes = File.ReadAllBytes(filePath);
                
                if (offsetValue != 0)
                {
                    var dst = new byte[plainBytes.Length + offsetValue];
                    Buffer.BlockCopy(plainBytes, 0, dst, offsetValue, plainBytes.Length);
                    File.WriteAllBytes(filePath, dst);
                    return true;
                }
                else if (xorKey != 0)
                {
                    for (int i = 0; i < plainBytes.Length; i++)
                    {
                        plainBytes[i] ^= (byte)xorKey;
                    }
                    File.WriteAllBytes(filePath, plainBytes);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LogF8.LogError($"加密文件失败 {filePath}: {e.Message}");
                throw;
            }
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

        private static string RenamePlatformManifestWithMd5(string platformManifestPath)
        {
            string md5 = FileTools.CreateMd5ForFile(platformManifestPath);
            string targetFileName = URLSetting.GetPlatformName() + "_" + md5;
            string targetPath = Path.Combine(URLSetting.GetAssetBundlesOutPath(), targetFileName);

            if (!platformManifestPath.Equals(targetPath, StringComparison.OrdinalIgnoreCase))
            {
                if (!FileTools.SafeRenameFile(platformManifestPath, targetPath))
                {
                    return Path.GetFileName(platformManifestPath);
                }
            }

            return targetFileName;
        }
    }
}
