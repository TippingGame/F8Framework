using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarStartSelectingScene : BaseToolbarElement
    {
        private static GUIContent buttonContent;
        private PlayModeManager _playModeManager;
        private readonly Texture icon = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomPlayButton");
        private readonly Texture iconStop = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomPlayStopButton");
        protected override string Name => "Start Selecting Scene";
        protected override string Tooltip => "Start Selecting Scene";

        public override void OnInit()
        {
            _playModeManager = PlayModeManager.Instance;
            buttonContent = new GUIContent(icon, this.Tooltip);

            this.Enabled = !EditorApplication.isPlayingOrWillChangePlaymode &&
                           SceneUtility.GetBuildIndexByScenePath(SceneUtility.GetScenePathByBuildIndex(0)) != -1;
        }

        public override void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            this.Enabled = (state == PlayModeStateChange.EnteredEditMode);
        }

        public override void OnDrawInToolbar()
        {
            buttonContent.image = EditorApplication.isPlaying ? iconStop : icon;
            if (GUILayout.Button(buttonContent, ToolbarStyles.CommandButtonStyle, GUILayout.Width(this.Width)))
            {
                OnPlayButtonClicked();
            }
        }

        private void OnPlayButtonClicked()
        {
            _playModeManager.PlayScene();
            _playModeManager.LastScene = _playModeManager.SelectedScene;
        }
    }
}