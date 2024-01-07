using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class ABBuildTool
    {
        public static void BuildAllAB()
        {
            // 打包AB输出路径
            string strABOutPAthDir = string.Empty;
 
            // 获取“StreamingAssets”文件夹路径（不一定这个文件夹，可自定义）            
            strABOutPAthDir = URLSetting.GetAssetBundlesOutPath();

            FileTools.CheckDirAndCreateWhenNeeded(strABOutPAthDir);
            
            // 打包生成AB包 (目标平台自动根据当前平台设置)
            BuildPipeline.BuildAssetBundles(strABOutPAthDir, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        
        //设置资源AB名字
        public static string SetAssetBundleName(string path)
        {
            AssetImporter ai = AssetImporter.GetAtPath(path);
            // 使用 Path.ChangeExtension 去掉扩展名
            string bundleName = Path.ChangeExtension(path, null).Replace(URLSetting.AssetBundlesPath, "");
            ai.assetBundleName = bundleName;
            return bundleName;
        }

        public static void GenerateAssetNames()
        {
            FileTools.CheckDirAndCreateWhenNeeded(URLSetting.GetAssetBundlesFolder());
            if (Directory.Exists(URLSetting.GetAssetBundlesFolder()))
            {
                // 获取文件夹下的所有文件路径
                string[] filePaths = Directory.GetFiles(URLSetting.GetAssetBundlesFolder(), "*", SearchOption.AllDirectories);

                List<string> tempNames = new List<string>();
                
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
                    "           public string AssetPath;\n" +
                    "       \n" +
                    "           public AssetMapping(string abName, string assetPath)\n" +
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
                
                foreach (string _filePath in filePaths)
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
                    
                    string abName = SetAssetBundleName(assetPath);
                    
                    if (tempNames.Contains(fileNameWithoutExtension))
                    {
                        LogF8.LogError("AssetName重复，请检查资源：" + fileNameWithoutExtension);
                    }
                    tempNames.Add(fileNameWithoutExtension);
                    
                    codeStr.Append(string.Format("          {{\"{0}\", new AssetMapping(\"{1}\", \"{2}\")}},\n", fileNameWithoutExtension, abName.ToLower(), assetPath));
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
                        LogF8.LogError("ResourceName重复，请检查资源：" + fileNameWithoutExtension);
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
    }
}
