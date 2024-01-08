using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class F8Helper : ScriptableObject
    {
        [MenuItem("开发工具/F8Run _F8", false, 100)]
        public static void F8Run()
        {
            LoadAllExcelData();
            BuildAssetBundles();
        }
        
        [MenuItem("开发工具/打包AssetBundles目录资源-F8", false, 102)]
        public static void BuildAssetBundles()
        {
            FileTools.SafeDeleteDir(FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/AssetMap");
            LogF8.LogAsset("生成AssetBundleMap.cs，生成ResourceMap.cs，生成F8Framework.AssetMap.asmdef");
            ABBuildTool.GenerateAssetNames();
            ABBuildTool.GenerateResourceNames();
            ABBuildTool.CreateAsmdefFile();
            LogF8.LogAsset("打包AssetBundle" + URLSetting.GetAssetBundlesOutPath());
            ABBuildTool.BuildAllAB();

            AssetDatabase.Refresh();
        }
        
        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<F8Helper>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);

            // 获取绝对路径并规范化
            string scriptPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptRelativePath));

            return scriptPath;
        }

        [InitializeOnLoadMethod]
        private static void ProjectKeyDownSpace()
        {
            //在Project面板按空格键相当于Show In Explorer
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        }
        
        private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (Event.current.type == EventType.KeyDown
                && Event.current.keyCode == KeyCode.Space
                && selectionRect.Contains(Event.current.mousePosition))
            {
                string strPath = AssetDatabase.GUIDToAssetPath(guid);
 
                if (Path.GetExtension(strPath) == string.Empty) //文件夹
                {
                    Process.Start(Path.GetFullPath(strPath));
                }
                else //文件
                {
                    Process.Start("explorer.exe", "/select," + Path.GetFullPath(strPath));
                }
 
                Event.current.Use();
            }
        }

        [MenuItem("开发工具/Excel导表-F8", false, 101)]
        public static void LoadAllExcelData()
        {
            string targetAssemblyName = "F8Framework.F8ExcelTool.Editor";  // 替换为目标程序集的名称
            string targetClassName = "ExcelDataTool";
            string targetMethod = "LoadAllExcelData";

            // 检查目标程序集是否存在
            if (!AssemblyExists(targetAssemblyName))
            {
                return;
            }

            // 加载目标程序集
            Assembly targetAssembly = Assembly.Load(targetAssemblyName);

            // 获取指定命名空间中的所有类型
            Type[] typesInNamespace = targetAssembly.GetTypes()
                .Where(t => t.Namespace == targetAssemblyName)
                .ToArray();

            // 查找目标类
            Type targetClassType = typesInNamespace.FirstOrDefault(t => t.Name == targetClassName);

            if (targetClassType != null)
            {
                // 创建目标类的实例（如果类不是静态的）
                object instance = null; // 对于静态方法，实例可以为 null

                // 调用静态方法
                MethodInfo methodInfo = targetClassType.GetMethod(targetMethod, BindingFlags.Public | BindingFlags.Static);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(instance, null);
                }
            }
        }
        private static bool AssemblyExists(string assemblyName)
        {
            try
            {
                // 尝试加载程序集，如果存在，则返回 true
                Assembly.Load(assemblyName);
                return true;
            }
            catch (FileNotFoundException)
            {
                // 如果文件未找到，则返回 false
                return false;
            }
        }

    }
}