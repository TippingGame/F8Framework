using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class AssetSetABName : MonoBehaviour
    {
        [MenuItem("Assets/（F8资产功能）/（清空选中的所有资产AB名）", false , 1001)]
        private static void SetAssetBundleNameIsFolderName()
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string absolutePath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

                if (File.Exists(absolutePath))
                {
                    AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                    if (!string.IsNullOrEmpty(ai.assetBundleName))
                    {
                        ai.assetBundleName = null;
                    }
                    LogF8.Log("文件"+assetPath);
                }
                else if (Directory.Exists(absolutePath))
                {
                    AssetImporter aiDir = AssetImporter.GetAtPath(assetPath);
                    if (!string.IsNullOrEmpty(aiDir.assetBundleName))
                    {
                        aiDir.assetBundleName = null;
                    }
                    LogF8.Log("文件夹"+assetPath);
                    
                    // 获取所有文件夹
                    string[] folderPaths = Directory.GetDirectories(absolutePath, "*", SearchOption.AllDirectories);
                    foreach (string folderPath in folderPaths)
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(ABBuildTool.GetAssetPath(folderPath));
                        if (!string.IsNullOrEmpty(ai.assetBundleName))
                        {
                            ai.assetBundleName = null;
                        }
                    }
                    // 获取所有文件
                    string[] assetPaths = Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories)
                        .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    foreach (string _assetPath in assetPaths)
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(ABBuildTool.GetAssetPath(_assetPath));
                        if (!string.IsNullOrEmpty(ai.assetBundleName))
                        {
                            ai.assetBundleName = null;
                        }
                        LogF8.Log("文件"+_assetPath);
                    }
                }
            }
        
            LogF8.LogAsset("设置完成");
        }
        
        public static string SetAssetBundleName(string path)
        {
            AssetImporter ai = AssetImporter.GetAtPath(path);
            // 使用 Path.ChangeExtension 去掉扩展名
            string bundleName = Path.ChangeExtension(path, null).Replace(URLSetting.AssetBundlesPath, "");
            if (!ai.assetBundleName.Equals(bundleName))
            {
                ai.assetBundleName = bundleName;
            }
            return bundleName;
        }
    }
}
