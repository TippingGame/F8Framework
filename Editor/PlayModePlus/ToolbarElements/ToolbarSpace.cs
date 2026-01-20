using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarSpace : BaseToolbarElement
    {
        protected override string Name => "Layout Space";

        public override void OnDrawInToolbar()
        {
            GUILayout.Space(25);
        }
    }
}