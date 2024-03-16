using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class F8Helper
    {
        [MenuItem("开发工具/F8Run _F8")]
        public static void F8Run()
        {
            LoadAllExcelData();
            BuildAssetBundles();
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        [MenuItem("开发工具/Excel导表-F8")]
        public static void LoadAllExcelData()
        {
            ExcelDataTool.LoadAllExcelData();
        }
        
        [MenuItem("开发工具/打包AssetBundles目录资源-F8")]
        public static void BuildAssetBundles()
        {
            ABBuildTool.BuildAllAB();
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
                
                if (Directory.Exists(strPath)) //文件夹
                {
                    Process.Start(Path.GetFullPath(strPath));
                }
                else //文件
                {
                    Process.Start(Path.GetDirectoryName(Path.GetFullPath(strPath)) ?? string.Empty);
                }
 
                Event.current.Use();
            }
        }
    }
}