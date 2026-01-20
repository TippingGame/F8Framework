using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarOpenBuildWindow : BaseToolbarElement
    {
        private BuildManager _buildManager;
        private static GUIContent buttonContent;
        private readonly Texture icon = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomBuildButtonOutlined");
        protected override string Name => "Open Build Window";
        protected override string Tooltip => "Open Build Window";

        public override void OnInit()
        {
            _buildManager = BuildManager.Instance;
            buttonContent = new GUIContent(icon, this.Tooltip);
        }

        public override void OnDrawInToolbar()
        {
            if (GUILayout.Button(buttonContent, ToolbarStyles.CommandButtonStyle, GUILayout.Width(this.Width)))
            {
                _buildManager.OpenBuildWindow();
            }
        }
    }
}