using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class BuildPkgToolWindow : EditorWindow
    {
        [UnityEditor.MenuItem("开发工具/打包工具 _F5", false, 99)]
        private static void OpenBuildPkgTool()
        {
            if (HasOpenInstances<BuildPkgToolWindow>())
            {
                EditorWindow.GetWindow<BuildPkgToolWindow>("打包工具 F5").Close();
            }
            else
            {
                BuildPkgToolWindow window = EditorWindow.GetWindow<BuildPkgToolWindow>("打包工具 F5");
                int stringLen = BuildPkgTool.StringLen(BuildPkgTool.BuildPath);
                window.minSize = new Vector2(Mathf.Max(stringLen * 11f - 250f, 500f), 777f);
            }
        }
        
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            BuildPkgTool.SetBuildTarget();
            BuildPkgTool.DrawRootDirectory();
            BuildPkgTool.DrawVersion();
            BuildPkgTool.DrawHotUpdate();
            BuildPkgTool.DrawBuildPkg();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
