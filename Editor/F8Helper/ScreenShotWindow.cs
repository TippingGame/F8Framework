using UnityEngine;
using UnityEditor;
using System;

namespace F8Framework.Core.Editor
{
    public class CaptureWindow : EditorWindow
    {
        private string saveFileName = string.Empty;
        private string saveDirPathKey = "CaptureSaveDirPathKey";

        [UnityEditor.MenuItem("开发工具/截图工具", false, 102)]
        private static void Capture()
        {
            if (HasOpenInstances<CaptureWindow>())
            {
                EditorWindow.GetWindow<CaptureWindow>("截图工具").Close();
            }
            else
            {
                EditorWindow.GetWindow<CaptureWindow>("截图工具");
            }
        }

        private void OnGUI()
        {
            string buildPath = F8EditorPrefs.GetString(saveDirPathKey, Application.dataPath);
            EditorGUILayout.LabelField("输出目录 : ");
            EditorGUILayout.LabelField(buildPath);

            if (GUILayout.Button("选择目录"))
            {
                string path = EditorUtility.OpenFolderPanel("选择目录", buildPath, Application.dataPath);
                if (!string.IsNullOrEmpty(path))
                {
                    F8EditorPrefs.SetString(saveDirPathKey, path);
                }
            }

            if (GUILayout.Button("打开目录"))
            {
                string openPath = F8EditorPrefs.GetString(saveDirPathKey, Application.dataPath);
                System.Diagnostics.Process.Start(openPath);
            }

            // insert blank line
            GUILayout.Label("");

            if (GUILayout.Button("截图"))
            {
                var resolution = GetMainGameViewSize();
                int x = (int)resolution.x;
                int y = (int)resolution.y;
                LogF8.LogConfig(buildPath);
                var outputPath = buildPath + "/" + DateTime.Now.ToString($"{x}x{y}_yyyy_MM_dd_HH_mm_ss") + ".png";
                ScreenCapture.CaptureScreenshot(outputPath);
                LogF8.Log("保存路径：" + outputPath);
            }
        }

        public static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }
    }
}