using UnityEditor;

namespace F8Framework.Core.Editor
{
    public class UICollectionLocalization : UnityEditor.Editor
    {
        [MenuItem("Assets/（F8UI界面管理功能）/（收集UI所有的中文放入本地化表）", false, -5)]
        public static void CollectionLocalization()
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;

            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Collection(assetPath);
            }

            AssetDatabase.Refresh();

            LogF8.Log("收集中文完成！");
        }

        public static void Collection(string assetPath)
        {
            
        }
    }
}
