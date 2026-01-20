using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarTimeSlider : BaseToolbarElement
    {
        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 10f;
        private const string ToolbarTimeSliderKey = "CustomToolbar.ToolbarTimeSlider.Value";

        private float currentTimeScale;
        private GUIContent buttonContent;

        protected override string Name => "Timescale Slider";
        protected override string Tooltip => "Controls Time.timeScale to slow down or speed up the game.";

        public override void OnInit()
        {
            this.Width = 200;

            currentTimeScale = F8EditorPrefs.GetFloat(ToolbarTimeSliderKey, 1.0f);
            Time.timeScale = currentTimeScale;
            buttonContent = new GUIContent("Time", this.Tooltip);
        }

        public override void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.EnteredEditMode)
            {
                currentTimeScale = 1.0f;
                Time.timeScale = currentTimeScale;

                F8EditorPrefs.SetFloat(ToolbarTimeSliderKey, currentTimeScale);
            }

            this.Enabled = (state == PlayModeStateChange.EnteredPlayMode);
        }

        public override void OnDrawInToolbar()
        {
            using (new EditorGUI.DisabledScope(!this.Enabled))
            {
                EditorGUILayout.LabelField(buttonContent, GUILayout.Width(35));

                EditorGUI.BeginChangeCheck();

                currentTimeScale = EditorGUILayout.Slider(currentTimeScale, MinTimeScale, MaxTimeScale,
                    GUILayout.Width(this.Width - 85));

                if (EditorGUI.EndChangeCheck())
                {
                    Time.timeScale = currentTimeScale;

                    F8EditorPrefs.SetFloat(ToolbarTimeSliderKey, currentTimeScale);
                }
            }
        }
    }
}