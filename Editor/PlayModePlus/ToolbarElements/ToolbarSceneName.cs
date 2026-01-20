using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarSceneName : BaseToolbarElement
    {
        private static GUIContent labelContent;
        private static PlayModeManager _playModeManager;
        protected override string Name => "Current Scene Name";
        protected override string Tooltip => "Current Scene Name";

        public override void OnInit()
        {
            this.Width = 100;
            _playModeManager = PlayModeManager.Instance;
            labelContent = new GUIContent("No Scene", this.Tooltip);
            UpdateSceneLabel();
        }
        
        public static void UpdateSceneLabel()
        {
            if (_playModeManager.SelectedScene != null)
            {
                labelContent.text = _playModeManager.SelectedScene.name;
            }
            else if (_playModeManager.LastScene != null)
            {
                labelContent.text = _playModeManager.LastScene.name;
            }
            else
            {
                var currentScene = GetCurrentSceneAsset();
                labelContent.text = string.IsNullOrEmpty(currentScene?.name) ? "No Scene" : currentScene.name;
            }
        }
        
        private static SceneAsset GetCurrentSceneAsset()
        {
            var currentScene = EditorSceneManager.GetActiveScene();
            if (currentScene.IsValid() && !string.IsNullOrEmpty(currentScene.path))
            {
                return AssetDatabase.LoadAssetAtPath<SceneAsset>(currentScene.path);
            }
            return null;
        }
        
        public override void OnDrawInToolbar()
        {
            if (labelContent != null)
            {
                Rect labelRect = GUILayoutUtility.GetRect(labelContent, ToolbarStyles.CommandLabelStyle, GUILayout.ExpandWidth(false));

                bool isHovering = labelRect.Contains(Event.current.mousePosition);
                bool isClicking = false;

                if (Event.current.type == EventType.MouseDown &&
                    Event.current.button == 0 &&
                    isHovering)
                {
                    isClicking = true;
                    Event.current.Use();
                }

                if (isHovering)
                {
                    ToolbarStyles.CommandLabelStyle.hover.textColor = new Color(89f / 255f, 158f / 255f, 94f / 255f);
                    ToolbarStyles.CommandLabelStyle.fontStyle = FontStyle.Bold;
                    EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.Link);
                }
                else
                {
                    ToolbarStyles.CommandLabelStyle.normal.textColor = Color.white;
                    ToolbarStyles.CommandLabelStyle.fontStyle = FontStyle.Bold;
                    EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.Arrow);
                }

                GUI.Label(labelRect, labelContent, ToolbarStyles.CommandLabelStyle);

                if (isClicking)
                {
                    HandleClick();
                }
            }
        }

        private void HandleClick()
        {
            if (_playModeManager.SelectedScene != null)
            {
                var projectWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
                var projectWindow = EditorWindow.GetWindow(projectWindowType);
                if (projectWindow != null)
                {
                    projectWindow.Focus();
                }
                EditorGUIUtility.PingObject(_playModeManager.SelectedScene);
            }
        }
    }
}