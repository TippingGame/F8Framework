using UnityEngine;
using UnityEditor;
using System.IO;

namespace F8Framework.Core.Editor
{
    public static class ExcelUpdater
    {
        static FileSystemWatcher watcher;
        static FileSystemEventHandler fileChangedCb;
        static RenamedEventHandler fileRenameCb;

        static FileSystemEventArgs curChangeEventArgs;

        static bool isInitialized = false;

        static string targetFileName = "本地化.xlsx"; // 设置你想要监听的特定文件名
        
        [InitializeOnLoadMethod]
        private static void Init()
        {
            if (isInitialized) return;
            EditorApplication.update += OnUpdate;
            curChangeEventArgs = null;
            FileTools.CheckDirAndCreateWhenNeeded(Application.dataPath + ExcelDataTool.ExcelPath);
            watcher = new FileSystemWatcher(Application.dataPath + ExcelDataTool.ExcelPath, "*.xlsx");
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.EnableRaisingEvents = true;
            fileChangedCb = new FileSystemEventHandler(OnDataTableChanged);
            fileRenameCb = new RenamedEventHandler(OnDataTableChanged);
            watcher.Changed += fileChangedCb;
            watcher.Deleted += fileChangedCb;
            watcher.Renamed += fileRenameCb;
            isInitialized = true;
        }

        private static void OnUpdate()
        {
            if (curChangeEventArgs != null)
            {
                Localization.Instance.Load(true);
                curChangeEventArgs = null;
            }
        }

        private static void OnDataTableChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name == targetFileName)
            {
                curChangeEventArgs = e;
            }
        }
    }
}