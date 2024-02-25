using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class ABBuildTool : ScriptableObject
    {
        public static void BuildAllAB()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            
            FileTools.SafeDeleteDir(FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/AssetMap");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            
            LogF8.LogAsset("生成AssetBundleMap.cs，生成ResourceMap.cs，生成F8Framework.AssetMap.asmdef");
            GenerateAssetNames();
            GenerateResourceNames();
            CreateAsmdefFile();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            
            // 获取“StreamingAssets”文件夹路径（不一定这个文件夹，可自定义）            
            string strABOutPAthDir = URLSetting.GetAssetBundlesOutPath();
            
            // 清理多余文件夹和ab
            DeleteRemovedAssetBundles();

            FileTools.CheckDirAndCreateWhenNeeded(strABOutPAthDir);
            
            LogF8.LogAsset("打包AssetBundle：" + URLSetting.GetAssetBundlesOutPath() + "  当前打包平台：" + EditorUserBuildSettings.activeBuildTarget);
            // 打包生成AB包 (目标平台自动根据当前平台设置)
            BuildPipeline.BuildAssetBundles(strABOutPAthDir, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            
            LogF8.LogAsset("打包成功!");
        }

        public static void DeleteRemovedAssetBundles()
        {
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesFolder());
            List<string> assetPaths = new List<string>();
            string assetBundlesPath = URLSetting.GetAssetBundlesFolder();
            RecordAssetsAndDirectories(assetBundlesPath, assetBundlesPath, assetPaths, true);
            assetPaths.Add("/" + URLSetting.GetPlatformName().ToLower());
            // LogF8.LogAsset(string.Join("，" ,assetPaths));
            
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesOutPath());
            List<string> abPaths = new List<string>();
            string abBundlesPath = URLSetting.GetAssetBundlesOutPath();
            RecordAssetsAndDirectories(abBundlesPath, abBundlesPath, abPaths);
            // LogF8.LogAsset(string.Join("，" ,abPaths));
            
            foreach (string ab in abPaths)
            {
                if (!assetPaths.Contains(ab))
                {
                    string abpath = URLSetting.GetAssetBundlesOutPath() + ab;
                    if (File.Exists(abpath))
                    {
                        // It's a file, delete the file
                        if (FileTools.SafeDeleteFile(abpath))
                        {
                            FileTools.SafeDeleteFile(abpath + ".meta");
                        }

                        if (FileTools.SafeDeleteFile(abpath + ".manifest"))
                        {
                            FileTools.SafeDeleteFile(abpath+ ".manifest" + ".meta");
                        }
                        LogF8.LogAsset("删除多余AB文件：" + abpath);
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
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        
        public static void RecordAssetsAndDirectories(string basePath, string rootPath, List<string> assetPaths, bool removeExtension = false)
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

                // Check for files
                string[] files = Directory.GetFiles(currentPath);
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();
                    if (extension != ".meta" && extension != ".manifest" && extension != ".ds_store")
                    {
                        // It's a file under AssetBundles, record as "Audio/click11"
                        if (removeExtension)
                        {
                            assetPaths.Add(FileTools.FormatToUnityPath(Path.ChangeExtension(relativePath + "/" + Path.GetFileName(file), null).ToLower()));
                        }
                        else
                        {
                            assetPaths.Add(FileTools.FormatToUnityPath(relativePath + "/" + Path.GetFileName(file)).ToLower());
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
            string bundleName = Path.ChangeExtension(path, null).Replace(URLSetting.AssetBundlesPath, "");
            if (!ai.assetBundleName.Equals(bundleName) && ai.assetBundleName.IsNullOrEmpty())
            {
                ai.assetBundleName = bundleName;
            }
            return ai.assetBundleName;
        }
        
        public static void GenerateAssetNames()
        {
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesFolder());
            if (Directory.Exists(URLSetting.GetAssetBundlesFolder()))
            {
                // 获取文件夹的路径
                string[] folderPaths = Directory.GetDirectories(URLSetting.GetAssetBundlesFolder(), "*", SearchOption.AllDirectories);
                // 获取文件的路径
                string[] filePaths = Directory.GetFiles(URLSetting.GetAssetBundlesFolder(), "*", SearchOption.AllDirectories);
                // 合并文件夹和文件的路径，可以根据需要调整顺序
                string[] allPaths = filePaths.Concat(folderPaths).ToArray();
                
                List<string> tempNames = new List<string>();
                Dictionary<string, List<string>> tempNamesDirectory = new Dictionary<string, List<string>>();
                
                // 创建文本文件
                StringBuilder codeStr = new StringBuilder(
                    "// code generation.\n" +
                    "\n" +
                    "using System.Collections.Generic;\n" +
                    "\n" +
                    "namespace F8Framework.AssetMap\n" +
                    "{\n" +
                    "   public static class AssetBundleMap\n" +
                    "   {\n" +
                    "       public class AssetMapping\n" +
                    "       {\n" +
                    "           public string AbName;\n" +
                    "           public string[] AssetPath;\n" +
                    "       \n" +
                    "           public AssetMapping(string abName, string[] assetPath)\n" +
                    "           {\n" +
                    "               AbName = abName;\n" +
                    "               AssetPath = assetPath;\n" +
                    "           }\n" +
                    "       }\n" +
                    "       \n" +
                    "       public static Dictionary<string, AssetMapping> Mappings\n" +
                    "       {\n" +
                    "           get => mappings;\n" +
                    "       }\n" +
                    "       \n" +
                    "       private static Dictionary<string, AssetMapping> mappings = new Dictionary<string, AssetMapping> {\n");
                
                foreach (string _filePath in allPaths)
                {
                    // 排除.meta文件 .DS_Store文件
                    if (Path.GetExtension(_filePath) == ".meta" || Path.GetExtension(_filePath) == ".DS_Store")
                    {
                        continue;
                    }
                    string filePath = FileTools.FormatToUnityPath(_filePath);

                    // 获取不带扩展名的文件名
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    
                    // 获取GetAssetPath
                    string assetPath = GetAssetPath(filePath);
                    
                    if (File.Exists(filePath)) // 文件
                    {
                        string abName = SetAssetBundleName(assetPath);
                    
                        if (tempNames.Contains(fileNameWithoutExtension))
                        {
                            LogF8.LogError("AssetName重复，请检查资源地址：" + filePath);
                            string id = Guid.NewGuid().ToString(); // 生成一个唯一的ID
                            fileNameWithoutExtension += id;
                        }
                        tempNames.Add(fileNameWithoutExtension);

                        // 修改同一AB名的assetPath
                        List<string> assetPathsForAbName;
                        string _temp = null;
                        if (!tempNamesDirectory.TryGetValue(abName.ToLower(), out assetPathsForAbName))
                        {
                            // 如果该 AssetBundle 名称还没有对应的资源路径列表，就创建一个新的列表
                            assetPathsForAbName = new List<string>();
                            tempNamesDirectory.Add(abName.ToLower(), assetPathsForAbName);
                        }
                        else
                        {
                            _temp = string.Join(", ", assetPathsForAbName.Select(p => "\"" + p + "\""));
                        }
                        
                        // 将当前资源路径添加到列表中，但是只添加一次，确保每个资源路径只出现一次
                        if (!assetPathsForAbName.Contains(assetPath))
                        {
                            assetPathsForAbName.Add(assetPath);
                        }
                        
                        if (_temp != null)
                        {
                            codeStr.Replace(_temp, string.Join(", ", assetPathsForAbName.Select(p => "\"" + p + "\"")));
                        }

                        string mappingLine = string.Format("          {{\"{0}\", new AssetMapping(\"{1}\", new []{{", fileNameWithoutExtension, abName.ToLower());
                        mappingLine += string.Join(", ", assetPathsForAbName.Select(p => "\"" + p + "\"")); // 添加所有资源路径
                        mappingLine += "})},\n";

                        codeStr.Append(mappingLine);
                    }
                    else if (Directory.Exists(filePath)) // 文件夹
                    {
                        string abName = assetPath.Replace(URLSetting.AssetBundlesPath, "");
                       
                        string[] assetPaths = Directory.GetFiles(filePath, "*", SearchOption.TopDirectoryOnly)
                            .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
                            .ToArray();
                        for (int i = 0; i < assetPaths.Length; i++)
                        {
                            assetPaths[i] = GetAssetPath(assetPaths[i]);
                        }
                        
                        if (tempNames.Contains(fileNameWithoutExtension))
                        {
                            LogF8.LogError("AssetName重复，请检查文件夹地址：" + filePath);
                            string id = Guid.NewGuid().ToString(); // 生成一个唯一的ID
                            fileNameWithoutExtension += id;
                        }
                        tempNames.Add(fileNameWithoutExtension);
                            
                        codeStr.Append(string.Format("          {{\"{0}\", new AssetMapping(\"{1}\", new []{{\"{2}\"}})}},\n", fileNameWithoutExtension, abName.ToLower(), string.Join("\", \"", assetPaths)));

                    }
                }

                codeStr.Append("       };\n");
                codeStr.Append("   }\n");
                codeStr.Append("}");

                string AssetBundleMapPath = Application.dataPath + "/F8Framework/AssetMap/AssetBundleMap.cs";
                
                FileTools.CheckFileAndCreateDirWhenNeeded(AssetBundleMapPath);
                
                File.WriteAllText(AssetBundleMapPath, codeStr.ToString());
            }
        }
        
        public static void GenerateResourceNames()
        {
            StringBuilder codeStr = new StringBuilder(
                "// code generation.\n" +
                "\n"+
                "using System.Collections.Generic;\n" +
                "\n" +
                "namespace F8Framework.AssetMap\n" +
                "{\n" +
                "   public static class ResourceMap\n" +
                "   {\n" +
                "       public static Dictionary<string, string> Mappings\n" +
                "       {\n" +
                "           get => mappings;\n" +
                "       }\n" +
                "       \n" +
                "       private static Dictionary<string, string> mappings = new Dictionary<string, string> {\n");

            string[] dics = Directory.GetDirectories(Application.dataPath, "Resources", SearchOption.AllDirectories);
            List<string> tempNames = new List<string>();
            foreach (string dic in dics)
            {
                if (!Directory.Exists(dic))
                    continue;

                string[] files = Directory.GetFiles(dic, "*", SearchOption.AllDirectories);
                
                foreach (string file in files)
                {
                    string filePath = FileTools.FormatToUnityPath(file);
                    if (!File.Exists(filePath))
                        continue;

                    if (filePath.EndsWith(".meta") ||
                        filePath.EndsWith(".DS_Store"))
                        continue;
                    
                    // 获取不带扩展名的文件名
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                    string notSuffix = Path.ChangeExtension(file, null);

                    string resourcesPath = GetResourcesPath(notSuffix);
                    
                    string realPath = resourcesPath.Replace(URLSetting.ResourcesPath, "");
                        
                    if (tempNames.Contains(fileNameWithoutExtension))
                    {
                        LogF8.LogError("ResourceName重复，请检查资源：" + filePath);
                        string id = Guid.NewGuid().ToString(); // 生成一个唯一的ID
                        fileNameWithoutExtension += id;
                    }
                    tempNames.Add(fileNameWithoutExtension);
                    
                    codeStr.Append(string.Format("          {{\"{0}\", \"{1}\"}},\n", fileNameWithoutExtension, realPath));
                }
            }

            codeStr.Append("       };\n");
            codeStr.Append("   }\n");
            codeStr.Append("}");
            
            string ResourceMapPath = Application.dataPath + "/F8Framework/AssetMap/ResourceMap.cs";
                
            FileTools.CheckFileAndCreateDirWhenNeeded(ResourceMapPath);
                
            File.WriteAllText(ResourceMapPath, codeStr.ToString());
        }
        
        public static void CreateAsmdefFile()
        {
            // 创建.asmdef文件的路径
            string asmdefPath = Application.dataPath + "/F8Framework/AssetMap/F8Framework.AssetMap.asmdef";
            
            FileTools.CheckFileAndCreateDirWhenNeeded(asmdefPath);
            // 创建一个新的.asmdef文件
            string asmdefContent = @"{
    ""name"": ""F8Framework.AssetMap"",
    ""references"": [],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";

            // 将内容写入.asmdef文件
            File.WriteAllText(asmdefPath, asmdefContent);
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
    }
}
