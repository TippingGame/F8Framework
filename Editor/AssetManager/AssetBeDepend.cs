using System.Collections.Generic;
using UnityEditor;

namespace F8Framework.Core.Editor
{
    public class AssetBeDepend
    {
        // 存储所有依赖关系
        private static Dictionary<string, List<string>> referenceCacheDic;
        
        private static List<string> referenceCacheList = new List<string>();
        
        [MenuItem("Assets/（F8资产功能）/（寻找资源是否被引用）", false , 1000)]
        private static void FindReferences()
        {
            referenceCacheList.Clear();
            if (referenceCacheDic == null)
            {
                referenceCacheDic = new Dictionary<string, List<string>>();
                CollectDepend();
            }

            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                IsBeDepend(assetPath);
            }
        
            LogF8.LogAsset("引用搜索完成");
        }
        
        // 收集项目中所有依赖关系
        private static void CollectDepend()
        {
            int count = 0;
            // 获取 AssetBundles 文件夹下所有资源
            string[] uiDirs = { System.IO.Path.Combine("Assets") };
            string[] guids = AssetDatabase.FindAssets("", uiDirs);
            foreach (string guid in guids)
            {
                // 将 GUID 转换为路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 获取文件所有直接依赖的资源
                string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);
        
                foreach (var filePath in dependencies)
                {
                    // dependency 被 assetPath 依赖了
                    // 将所有依赖关系存储到字典中
                    List<string> list = null;
                    if (!referenceCacheDic.TryGetValue(filePath, out list))
                    {
                        list = new List<string>();
                        referenceCacheDic[filePath] = list;
                    }
        
                    list.Add(assetPath);
                }
        
                count++;
                EditorUtility.DisplayProgressBar("引用查找", "引用查找中",
                    (float)(count * 1.0f / guids.Length));
            }
        
            EditorUtility.ClearProgressBar();
        }
        
        // 判断文件是否被依赖
        private static bool IsBeDepend(string filePath)
        {
            List<string> list = null;
            if (!referenceCacheDic.TryGetValue(filePath, out list))
            {
                return false;
            }
        
            // 将依赖关系打印出来
            foreach (var file in list)
            {
                if (!referenceCacheList.Contains(file))
                {
                    referenceCacheList.Add(file);
                    LogF8.LogAsset(filePath + "---->被：<color=#FFFF00>" + file + "</color> 引用");
                }
            }
        
            return true;
        }

    }
}