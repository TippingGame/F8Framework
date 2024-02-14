using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class CreateCustomModuleScript : ScriptableObject
    {
        [MenuItem("Assets/（F8模块中心功能）/（CustomModule.cs）", false, -1)]
        static void CreateBaseViewScript()
        {
            string path = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 1)) +
                          "/CustomModuleTemplate.cs.txt";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "CustomModule.cs");
        }
        
        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<CreateCustomModuleScript>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);
            
            return scriptRelativePath;
        }
    }
}
