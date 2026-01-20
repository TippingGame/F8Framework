using System;

namespace F8Framework.Core.Editor
{
      [Serializable]
      public class ToolboxShortcut
      {
            public string displayName = "New Shortcut";
            public string subMenuPath = "";
            public string menuItemPath = "Window/...";
            public bool isEnabled = true;
      }
}