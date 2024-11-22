using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class F8Helper : ScriptableObject
    {
        [MenuItem("开发工具/设置Excel存放目录")]
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
        
        [MenuItem("开发工具/F8Run _F8")]
        public static void F8Run()
        {
            F8EditorPrefs.SetBool("compilationFinishedHotUpdateDll", true);
            F8EditorPrefs.SetBool("compilationFinishedBuildAB", true);
            LoadAllExcelData();
        }
        
        [MenuItem("开发工具/生成并复制热更新Dll-F8")]
        public static void GenerateCopyHotUpdateDll()
        {
            // F8EditorPrefs.SetBool("compilationFinishedHotUpdateDll", false);
            // HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
            // FileTools.SafeClearDir(Application.dataPath + "/AssetBundles/Code");
            // FileTools.CheckDirAndCreateWhenNeeded(Application.dataPath + "/AssetBundles/Code");
            // foreach (var dll in HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesExcludePreserved) // 获取HybridCLR设置面板的dll名称
            // {
            //     FileTools.SafeCopyFile(
            //         HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget) + "/" + dll + ".dll",
            //         Application.dataPath + "/AssetBundles/Code/" + dll + ".bytes");
            //     LogF8.LogAsset("生成并复制热更新dll：" + dll);
            // }
            // AssetDatabase.Refresh();
        }
        
        [MenuItem("开发工具/Excel导表-F8")]
        public static void LoadAllExcelData()
        {
            ExcelDataTool.LoadAllExcelData();
        }
        
        [MenuItem("开发工具/打包AssetBundles目录资源-F8")]
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