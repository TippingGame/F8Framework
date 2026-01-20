using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core.Editor
{
      public sealed class ToolbarConfiguration : ScriptableObject
      {
            public List<ToolbarGroup> groups = new();
            public List<ToolboxShortcut> toolboxShortcuts = new();
      }
}