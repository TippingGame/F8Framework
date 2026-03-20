using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class BuildPkgToolWindow : EditorWindow
    {
        private const string ScrollKey = "BuildPkgToolWindow.ScrollPosition";

        [SerializeField] private Vector2 scrollPosition;

        [MenuItem("开发工具/打包工具 _F5", false, 99)]
        private static void OpenBuildPkgTool()
        {
            if (HasOpenInstances<BuildPkgToolWindow>())
            {
                GetWindow<BuildPkgToolWindow>("打包工具 F5").Close();
            }
            else
            {
                BuildPkgToolWindow window = GetWindow<BuildPkgToolWindow>("打包工具 F5");
                int stringLen = BuildPkgTool.StringLen(BuildPkgTool.BuildPath);
                window.minSize = new Vector2(Mathf.Max(stringLen * 11f - 250f, 565f), 930f);
            }
        }

        private void OnEnable()
        {
            scrollPosition = SessionState.GetVector3(ScrollKey, Vector3.zero);
            scrollPosition = new Vector2(scrollPosition.x, scrollPosition.y);
        }

        private void OnDisable()
        {
            SessionState.SetVector3(ScrollKey, new Vector3(scrollPosition.x, scrollPosition.y, 0));
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer);

            var newScroll = EditorGUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Height(Mathf.Min(1080, position.height)));

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            BuildPkgTool.SetBuildTarget();
            BuildPkgTool.DrawRootDirectory();
            BuildPkgTool.DrawVersion();
            BuildPkgTool.DrawAssetSetting();
            BuildPkgTool.DrawHotUpdate();
            BuildPkgTool.DrawBuildPkg();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            if (newScroll != scrollPosition)
            {
                scrollPosition = newScroll;
                SessionState.SetVector3(ScrollKey, new Vector3(scrollPosition.x, scrollPosition.y, 0));
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}