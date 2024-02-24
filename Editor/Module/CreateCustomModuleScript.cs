using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class CreateCustomModuleScript : ScriptableObject
    {
        [MenuItem("Assets/（F8模块中心功能）/（Module.cs）", false, -3)]
        static void CreateModuleScript()
        {
            string path = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 1)) +
                          "/ModuleTemplate.cs.txt";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "Module.cs");
        }
        
        [MenuItem("Assets/（F8模块中心功能）/（ModuleMono.cs）", false, -2)]
        static void CreateModuleMonoScript()
        {
            string path = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 1)) +
                          "/ModuleMonoTemplate.cs.txt";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "ModuleMono.cs");
        }
        
        [MenuItem("Assets/（F8模块中心功能）/（StaticModule.cs）", false, -1)]
        static void CreateStaticModuleScript()
        {
            string path = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 1)) +
                          "/StaticModuleTemplate.cs.txt";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "StaticModule.cs");
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
