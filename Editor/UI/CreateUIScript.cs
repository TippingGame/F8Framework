using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class CreateUIScript : ScriptableObject
    {
        [MenuItem("Assets/（F8UI界面管理功能）/（BaseItem.cs）", false, -1)]
        static void CreateBaseItemScript()
        {
            string path = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 1)) +
                          "/BaseItemTemplate.cs.txt";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "BaseItem.cs");
        }
        
        [MenuItem("Assets/（F8UI界面管理功能）/（BaseView.cs）", false, -1)]
        static void CreateBaseViewScript()
        {
            string path = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 1)) +
                          "/BaseViewTemplate.cs.txt";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "BaseView.cs");
        }
        
        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<CreateUIScript>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);
            
            return scriptRelativePath;
        }
    }
}
