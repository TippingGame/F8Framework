using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class F8Helper : ScriptableObject
    {
        [MenuItem("开发工具/设置Excel存放目录", false, 104)]
        public static void SetExcelPath()
        {
            string lastExcelPath = F8EditorPrefs.GetString("ExcelPath", default);
            string tempExcelPath = EditorUtility.OpenFolderPanel("设置Excel存放目录", lastExcelPath ?? Application.dataPath, "");
            if (!tempExcelPath.IsNullOrEmpty())
            {
                F8EditorPrefs.SetString("ExcelPath", tempExcelPath);
            }
            LogF8.LogConfig("设置Excel存放目录：" + tempExcelPath);
        }
        
        [MenuItem("开发工具/编辑器模式（勾选）", true)]
        public static bool SetIsEditorMode()
        {
            bool isEditorMode = F8EditorPrefs.GetBool("IsEditorMode", false);
            Menu.SetChecked("开发工具/编辑器模式（勾选）", isEditorMode);
            return true;
        }
        
        [MenuItem("开发工具/编辑器模式（勾选）")]
        public static void SwitchIsEditorMode()
        {
            bool isEditorMode = F8EditorPrefs.GetBool("IsEditorMode", false);
            F8EditorPrefs.SetBool("IsEditorMode", !isEditorMode);
        }
        
        
        [MenuItem("开发工具/清除AssetBundleNames")]
        public static void ClearAssetBundlesName()
        {
            ABBuildTool.ClearAllAssetNames();
        }

        [MenuItem("开发工具/1: F8Run _F8", false, 200)]
        public static void F8Run()
        {
            LoadAllExcelData();
            F8EditorPrefs.SetBool("compilationFinishedHotUpdateDll", true);
            F8EditorPrefs.SetBool("compilationFinishedBuildAB", true);
        }

        // 补充元数据，不会热更新此处的dll，一般在{project}/HybridCLRData/AssembliesPostIl2CppStrip/{target}目录下
        public static List<string> AOTDllList = new List<string>
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll", // 如果使用了Linq，需要这个
            // "Newtonsoft.Json.dll", 
            // "protobuf-net.dll",
            "F8Framework.Core.dll", // 为了能使用框架中的泛型
        };
        
        [MenuItem("开发工具/3: 生成并复制热更新Dll-F8", false, 210)]
        public static void GenerateCopyHotUpdateDll()
        {
            // F8EditorPrefs.SetBool("compilationFinishedHotUpdateDll", false);
            // HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
            //
            // string outpath = Application.dataPath + "/AssetBundles/Code/";
            //
            // FileTools.SafeClearDir(outpath);
            // FileTools.CheckDirAndCreateWhenNeeded(outpath);
            // foreach (var dll in HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesExcludePreserved) // 获取HybridCLR设置面板的dll名称
            // {
            //     var path =
            //         HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings
            //             .activeBuildTarget) + "/" + dll + ".dll";
            //     FileTools.SafeCopyFile(
            //         HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget) + "/" + dll + ".dll",
            //         outpath + dll + ".bytes");
            //     LogF8.LogAsset("生成并复制热更新dll：" + dll);
            // }
            //
            // foreach (var aotDllName in F8Helper.AOTDllList)
            // {
            //     var mscorlibsouPath =
            //         HybridCLR.Editor.SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings
            //             .activeBuildTarget) + "/" + aotDllName;
            //     
            //     FileTools.SafeCopyFile(
            //         mscorlibsouPath,
            //         outpath + aotDllName + "by.bytes");
            //     LogF8.LogAsset("生成并复制补充元数据dll：" + aotDllName);
            // }
            //
            // AssetDatabase.Refresh();
        }

        [MenuItem("开发工具/2: Excel导表-F8", false, 205)]
        public static void LoadAllExcelData()
        {
            ExcelDataTool.LoadAllExcelData();
        }

        [MenuItem("开发工具/4: 打包AssetBundles目录资源-F8", false, 215)]
        public static void BuildAssetBundles()
        {
            F8EditorPrefs.SetBool("compilationFinishedBuildAB", false);
            ABBuildTool.BuildAllAB();
        }

        // 加载时初始化方法，统一管理调用顺序
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethods()
        {
            // 在Project面板按空格键相当于Show In Explorer
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;

            LocalizationEditorSettings.LoadEditorSettings();

            TMPIntegrationSwitcher.EnsureIntegrationState();
            
            // 注册编辑器退出事件
            EditorApplication.quitting += OnEditorQuit;
        }
        
        private static void OnEditorQuit()
        {
            F8EditorPrefs.SetBool("compilationFinished", false);
            F8EditorPrefs.SetBool("compilationFinishedHotUpdateDll", false);
            F8EditorPrefs.SetBool("compilationFinishedBuildAB", false);
            F8EditorPrefs.SetBool("compilationFinishedBuildPkg", false);
            F8EditorPrefs.SetBool("compilationFinishedBuildRun", false);
            F8EditorPrefs.SetBool("compilationFinishedBuildUpdate", false);
        }

        private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (Event.current.type == EventType.KeyDown
                && Event.current.keyCode == KeyCode.Space
                && selectionRect.Contains(Event.current.mousePosition))
            {
                string strPath = AssetDatabase.GUIDToAssetPath(guid);
                
                EditorUtility.RevealInFinder(strPath);
                
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(strPath);
                if (obj != null)
                {
                    EditorGUIUtility.PingObject(obj);
                }
                Event.current.Use();
            }
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
    }
}