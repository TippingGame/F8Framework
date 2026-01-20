using System;
using System.Collections.Generic;

namespace F8Framework.Core.Editor
{
      [Serializable]
      public class ToolbarGroup
      {
            public string groupName = "New Group";
            public ToolbarSide side = ToolbarSide.Left;
            public List<ToolbarElement> elements = new();
            public bool isEnabled = true;
      }
}