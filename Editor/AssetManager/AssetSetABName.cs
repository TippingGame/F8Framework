using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class AssetSetABName
    {
        [MenuItem("Assets/（F8资产功能）/（清空所有选中的资产AB名）", false , 1001)]
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
                        EditorUtility.SetDirty(ai);
                    }
                    LogF8.Log("文件" + assetPath);
                }
                else if (Directory.Exists(absolutePath))
                {
                    AssetImporter aiDir = AssetImporter.GetAtPath(assetPath);
                    if (!string.IsNullOrEmpty(aiDir.assetBundleName))
                    {
                        aiDir.assetBundleName = null;
                        EditorUtility.SetDirty(aiDir);
                    }
                    LogF8.Log("文件夹" + assetPath);
                    
                    // 获取所有文件夹
                    string[] folderPaths = Directory.GetDirectories(absolutePath, "*", SearchOption.AllDirectories);
                    foreach (string folderPath in folderPaths)
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(ABBuildTool.GetAssetPath(folderPath));
                        if (!string.IsNullOrEmpty(ai.assetBundleName))
                        {
                            ai.assetBundleName = null;
                            EditorUtility.SetDirty(ai);
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
                            EditorUtility.SetDirty(ai);
                        }
                        LogF8.Log("文件" + _assetPath);
                    }
                }
            }
            AssetDatabase.Refresh();
            LogF8.LogAsset("已清空所有选中的资产AB名");
        }
        
        [MenuItem("Assets/（F8资产功能）/（设置选中的所有资产为相同AB名（AB名取自第一个资产））", false , 1002)]
        private static void SetAssetBundleSameName()
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            string FirstName = null;
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string absolutePath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

                if (File.Exists(absolutePath))
                {
                    AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                    // 使用 Path.ChangeExtension 去掉扩展名
                    string bundleName = Path.ChangeExtension(assetPath, null).Replace(URLSetting.AssetBundlesPath, "");
                    if (!ai.assetBundleName.Equals(bundleName))
                    {
                        if (FirstName == null)
                        {
                            ai.assetBundleName = bundleName;
                            FirstName = bundleName;
                        }
                        else
                        {
                            ai.assetBundleName = FirstName;
                        }
                        EditorUtility.SetDirty(ai);
                    }
                    LogF8.Log("文件" + assetPath);
                }
                else if (Directory.Exists(absolutePath))
                {
                    // 获取所有文件
                    string[] assetPaths = Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories)
                        .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    foreach (string _assetPath in assetPaths)
                    {
                        string getAssetPath = ABBuildTool.GetAssetPath(_assetPath);
                        AssetImporter ai = AssetImporter.GetAtPath(getAssetPath);
                        // 使用 Path.ChangeExtension 去掉扩展名
                        string bundleName = Path.ChangeExtension(getAssetPath, null).Replace(URLSetting.AssetBundlesPath, "");
                        if (!ai.assetBundleName.Equals(bundleName))
                        {
                            if (FirstName == null)
                            {
                                ai.assetBundleName = bundleName;
                                FirstName = bundleName;
                            }
                            else
                            {
                                ai.assetBundleName = FirstName;
                            }
                            EditorUtility.SetDirty(ai);
                        }
                        LogF8.Log("文件" + _assetPath);
                    }
                }
            }
            AssetDatabase.Refresh();
            LogF8.LogAsset("设置所有AB名为：" + FirstName);
        }
    }
}
