using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class BuildPkgTool : EditorWindow
    {
        private static readonly GUILayoutOption NORMAL_WIDTH = GUILayout.Width(100);
        private static readonly GUILayoutOption ButtonHeight = GUILayout.Height(20);
        private static string _prefBuildPath = "PrefBuildPath";
        private static string _exportPlatform = "ExportPlatform";
        private static string _exportCurrentPlatform = "ExportCurrentPlatform";
        private static string buildPath = "";
        private static string tempDisc = "";
        private static string fromVersion = "";
        private static string toVersion = "";

        private static BuildTarget buildTarget = BuildTarget.NoTarget;

        private static string fromVersion1 = "1.0.0";
        private static string toVersion1 = "1.0.0";
        
        private static int index = 0;
        private static BuildTarget[] options = Enum.GetValues(typeof(BuildTarget))
            .Cast<BuildTarget>()
            .Select(option => (BuildTarget)Enum.Parse(typeof(BuildTarget), option.ToString()))
            .ToArray();
        private static string[] optionNames = Array.ConvertAll(options, option => option.ToString());
        
        private static bool exportCurrentPlatform = true;
        
        [UnityEditor.MenuItem("开发工具/打包工具 _F5", false, 99)]
        private static void OpenBuildPkgTool()
        {
            if (HasOpenInstances<BuildPkgTool>())
            {
                EditorWindow.GetWindow<BuildPkgTool>("打包工具").Close();
            }
            else
            {
                BuildPkgTool window = EditorWindow.GetWindow<BuildPkgTool>("打包工具");
                int stringLen = StringLen(buildPath);
                window.minSize = new Vector2(Mathf.Max(stringLen * 11f - 250f, 500f), 500);
            }
        }

        private static void BuildUpdate()
        {
        }

        public static void Build()
        {
            string appName = Application.productName;
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.WSAPlayer:
                    appName += ".exe";
                    break;
                case BuildTarget.Android:
                    appName += ".apk";
                    break;
            }

            string locationPathName = buildPath + "/" + buildTarget.ToString() + "/" + appName;
            FileTools.CheckDirAndCreateWhenNeeded(buildPath + "/" + buildTarget.ToString());

            LogF8.Log(locationPathName);
            BuildReport buildReport =
                BuildPipeline.BuildPlayer(GetBuildScenes(), locationPathName, buildTarget, BuildOptions.None);
            if (buildReport.summary.result != BuildResult.Succeeded)
            {
                LogF8.LogError($"build pkg fail : {buildReport.summary.result}");
            }

            LogF8.Log("打包游戏完成！" + locationPathName);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            SetBuildTarget();
            DrawRootDirectory();
            DrawVersion();
            DrawBuildPkg();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private static void SetBuildTarget()
        {
            GUILayout.Space(20);
            GUILayout.Label("【设置打包平台】");
            GUILayout.Space(10);
            exportCurrentPlatform = EditorGUILayout.Toggle("根据当前平台导出", EditorPrefs.GetBool(_exportCurrentPlatform, true));
            if (EditorPrefs.GetBool(_exportCurrentPlatform, true) != exportCurrentPlatform)
            {
                EditorPrefs.SetBool(_exportCurrentPlatform, exportCurrentPlatform);
            }
            if (exportCurrentPlatform)
            {
                buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
            
            GUILayout.Space(10);
            if (!exportCurrentPlatform)
            {
                Array enumValues = Enum.GetValues(typeof(BuildTarget));
                for (int i = 0; i < enumValues.Length; i++)
                {
                    BuildTarget target = (BuildTarget)enumValues.GetValue(i); // 获取枚举值
                    if (target.ToString() == EditorPrefs.GetString(_exportPlatform, ""))
                    {
                        index = i;
                    }
                }
                
                index = EditorGUILayout.Popup(index, optionNames);
                if (options[index].ToString() != EditorPrefs.GetString(_exportPlatform, ""))
                {
                    EditorPrefs.SetString(_exportPlatform, options[index].ToString());
                }
                buildTarget = options[index];
            }
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        private static void DrawRootDirectory()
        {
            GUILayout.Space(5);
            GUILayout.Label("【打包输出目录】");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("点击设置目录", NORMAL_WIDTH, ButtonHeight))
            {
                string _buildPath = EditorUtility.OpenFolderPanel("设置打包根目录", buildPath, buildPath);
                if (!string.IsNullOrEmpty(_buildPath))
                {
                    buildPath = _buildPath;
                    string discName = buildPath.Substring(0, buildPath.IndexOf('/'));
                    tempDisc = buildPath.Substring(0, discName.Length);
                    EditorPrefs.SetString(_prefBuildPath, buildPath);
                }
            }

            buildPath = EditorPrefs.GetString(_prefBuildPath, "");
           
            GUILayout.Label("输出目录：" + buildPath);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        private static void DrawVersion()
        {
            GUILayout.Space(5);
            GUILayout.Label("【发布版本】");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("发布版本：", GUILayout.Width(60));
            GUILayout.Space(10);
            GUILayout.Label("旧版本");
            fromVersion1 = EditorGUILayout.TextField(fromVersion1);

            GUILayout.Label("发布的版本");
            toVersion1 = EditorGUILayout.TextField(toVersion1);

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("-----------------------------------------------------------------------");
            GUILayout.Space(5);
        }

        private static void DrawBuildPkg()
        {
            GUILayout.Space(5);
            GUILayout.Label("【打包游戏】");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打包游戏", NORMAL_WIDTH))
            {
                if (string.IsNullOrEmpty(buildPath))
                {
                    EditorUtility.DisplayDialog("打包游戏", "输出目录路径不能为空", "确定");
                }
                else
                {
                    string countent = fromVersion == toVersion
                        ? "确定发布版本" + toVersion
                        : "确定发布从版本" + fromVersion + " 到 " + toVersion;
                    if (EditorUtility.DisplayDialog("打包游戏", countent, "确定", "取消"))
                    {
                        EditorApplication.delayCall += Build;
                    }
                }
            }

            GUILayout.Space(30);
            if (GUILayout.Button("发布热更新包", NORMAL_WIDTH))
            {
                if (string.IsNullOrEmpty(buildPath))
                {
                    EditorUtility.DisplayDialog("发布热更新包", "发布热更新包路径不能为空", "确定");
                }

                else if (fromVersion == toVersion)
                {
                    EditorUtility.DisplayDialog("发布热更新包", "发布热更新包版本不能相同", "确定");
                }
                else
                {
                    string countent = "确定发布从版本" + fromVersion + " 到 " + toVersion;
                    if (EditorUtility.DisplayDialog("发布热更新包", countent, "确定", "取消"))
                    {
                        EditorApplication.delayCall += BuildUpdate;
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        private static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e != null && e.enabled)
                {
                    names.Add(e.path);
                }
            }

            return names.ToArray();
        }

        public static int StringLen(string str)
        {
            int realLength = 0;
            foreach (char c in str)
            {
                if (c >= 0 && c <= 128)
                    realLength += 1;
                else
                    realLength += 2;
            }

            return realLength;
        }
    }
}