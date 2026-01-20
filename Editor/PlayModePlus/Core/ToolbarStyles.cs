using UnityEngine;

namespace F8Framework.Core.Editor
{
      /// <summary>
      /// Provides reusable style definitions for toolbar elements in Unity editor extensions.
      /// </summary>
      /// <remarks>
      /// This class contains predefined GUIStyle objects specifically tailored for toolbar components.
      /// These styles ensure consistent alignment and appearance across all toolbar elements.
      /// </remarks>
      internal static class ToolbarStyles
      {
          public readonly static GUIStyle CommandLabelStyle = new("ToolbarLabel")
          {
              alignment = TextAnchor.MiddleCenter,
              margin = new RectOffset(2, 2, 2, 2),
          };
          
          public readonly static GUIStyle CommandButtonStyle = new("ToolbarButton")
          {
              alignment = TextAnchor.MiddleCenter,
              margin = new RectOffset(2, 2, 2, 2),
          };

          public readonly static GUIStyle CommandPopupStyle = new("ToolbarButton")
          {
              alignment = TextAnchor.MiddleCenter,
              margin = new RectOffset(2, 2, 2, 2),
          };
      }
}