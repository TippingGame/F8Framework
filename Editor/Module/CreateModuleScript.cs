using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class CreateModuleScript : ScriptableObject
    {
        [MenuItem("Assets/（F8模块中心功能）/（ModuleCenter.cs）", false, -1)]
        static void CreateBaseViewScript()
        {
            string path = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 1)) +
                          "/ModuleCenterTemplate.cs.txt";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "ModuleCenter.cs");
        }
        
        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<CreateModuleScript>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);
            
            return scriptRelativePath;
        }
    }
}
