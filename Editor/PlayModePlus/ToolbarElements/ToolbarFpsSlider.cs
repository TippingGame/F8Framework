using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarFpsSlider : BaseToolbarElement
    {
        private const int MinFpsValue = 0;
        private const int MaxFpsValue = 999;
        private const string ToolbarFpsSliderKey = "CustomToolbar.ToolbarFpsSlider.Value";

        private int currentFPS;
        private GUIContent buttonContent;

        protected override string Name => "FPS Slider";
        protected override string Tooltip => "Controls Application.targetFrameRate. Set to 0 for unlimited FPS.";

        public override void OnInit()
        {
            this.Width = 200;
            buttonContent = new GUIContent("FPS", this.Tooltip);

            currentFPS = F8EditorPrefs.GetInt(ToolbarFpsSliderKey, 0);

            Application.targetFrameRate = currentFPS;
        }

        public override void OnDrawInToolbar()
        {
            buttonContent.text = currentFPS == MinFpsValue ? "FPS (∞)" : "FPS";

            EditorGUILayout.LabelField(buttonContent,
                currentFPS == MinFpsValue ? GUILayout.Width(50) : GUILayout.Width(25));

            EditorGUI.BeginChangeCheck();

            currentFPS = Mathf.RoundToInt(EditorGUILayout.Slider(currentFPS, MinFpsValue, MaxFpsValue,
                GUILayout.Width(this.Width - 85)));

            if (EditorGUI.EndChangeCheck())
            {
                Application.targetFrameRate = (currentFPS == MinFpsValue) ? -1 : currentFPS;
                F8EditorPrefs.SetInt(ToolbarFpsSliderKey, currentFPS);
            }
        }
    }
}