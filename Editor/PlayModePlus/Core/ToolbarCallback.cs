using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace F8Framework.Core.Editor
{
      /// <summary>
      /// Provides a callback system for injecting custom GUI elements into Unity's toolbar.
      /// This class uses reflection to access Unity's internal toolbar system and allows
      /// adding custom controls to the left and right of the play mode buttons.
      /// </summary>
      [InitializeOnLoad]
      public static class ToolbarCallback
      {
            public static Action OnToolbarGUILeftOfCenter;
            public static Action OnToolbarGUIRightOfCenter;

#if UNITY_6000_3_OR_NEWER
            private static int setupAttempts;
            private const int MaxSetupAttempts = 200;

            static ToolbarCallback()
            {
                  EditorApplication.update -= Initialize;
                  EditorApplication.update += Initialize;
            }

            private static void Initialize()
            {
                  setupAttempts++;

                  Type mainToolbarWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.MainToolbarWindow");

                  if (mainToolbarWindowType == null)
                  {
                        EditorApplication.update -= Initialize;

                        return;
                  }

                  Object[] toolbars = Resources.FindObjectsOfTypeAll(mainToolbarWindowType);

                  if (toolbars.Length == 0)
                  {
                        if (setupAttempts > MaxSetupAttempts)
                        {
                              LogF8.LogWarning("[CustomToolbar] Could not find MainToolbarWindow instance after multiple attempts. Aborting.");
                              EditorApplication.update -= Initialize;
                        }

                        return;
                  }

                  var toolbarWindow = (EditorWindow)toolbars[0];
                  VisualElement root = toolbarWindow.rootVisualElement;

                  if (root == null)
                  {
                        EditorApplication.update -= Initialize;

                        return;
                  }

                  VisualElement middleContainer = root.Q(className: "unity-overlay-container__middle-container");

                  if (middleContainer == null)
                  {
                        if (setupAttempts > MaxSetupAttempts)
                        {
                              LogF8.LogWarning("[CustomToolbar] Found MainToolbarWindow, but its middle-container is not ready. Aborting.");
                              EditorApplication.update -= Initialize;
                        }

                        return;
                  }

                  VisualElement parentContainer = middleContainer.parent;

                  if (parentContainer == null)
                  {
                        EditorApplication.update -= Initialize;

                        return;
                  }

                  var leftDock = new VisualElement
                  {
                              name = "OpalStudio_LeftDock",
                              style =
                              {
                                          flexGrow = 1,
                                          flexDirection = FlexDirection.Row,
                                          justifyContent = Justify.FlexEnd,
                                          alignItems = Align.Center
                              }
                  };

                  var rightDock = new VisualElement
                  {
                              name = "OpalStudio_RightDock",
                              style =
                              {
                                          flexGrow = 1,
                                          flexDirection = FlexDirection.Row,
                                          justifyContent = Justify.FlexStart,
                                          alignItems = Align.Center
                              }
                  };

                  parentContainer.Insert(parentContainer.IndexOf(middleContainer), leftDock);
                  parentContainer.Insert(parentContainer.IndexOf(middleContainer) + 1, rightDock);

                  leftDock.Add(new IMGUIContainer(static () => OnToolbarGUILeftOfCenter?.Invoke()));
                  rightDock.Add(new IMGUIContainer(static () => OnToolbarGUIRightOfCenter?.Invoke()));

                  EditorApplication.update -= Initialize;
            }

#else

            private readonly static Type UnityToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            private readonly static FieldInfo UnityToolbarRootField = UnityToolbarType?.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            private static ScriptableObject currentToolbar;

            static ToolbarCallback()
            {
                  EditorApplication.update -= TryInitialize;
                  EditorApplication.update += TryInitialize;
            }

            private static void TryInitialize()
            {
                  if (currentToolbar == null)
                  {
                        Object[] toolbars = Resources.FindObjectsOfTypeAll(UnityToolbarType);

                        if (toolbars.Length == 0)
                        {
                              return;
                        }

                        currentToolbar = (ScriptableObject)toolbars[0];
                  }

                  InjectToolbarElements();
                  EditorApplication.update -= TryInitialize;
            }

            private static void InjectToolbarElements()
            {
                  if (UnityToolbarRootField?.GetValue(currentToolbar) is not VisualElement root)
                  {
                        return;
                  }

                  VisualElement zoneLeft = root.Q("ToolbarZoneLeftAlign");
                  VisualElement zoneRight = root.Q("ToolbarZoneRightAlign");

                  if (zoneLeft == null || zoneRight == null)
                  {
                        LogF8.LogError("[CUSTOM TOOLBAR]: Could not find Toolbar containers. Elements will not be drawn.");

                        return;
                  }

                  var leftContainer = new IMGUIContainer(static () => OnToolbarGUILeftOfCenter?.Invoke())
                  {
                              style =
                              {
                                          flexGrow = 1,
                                          flexDirection = FlexDirection.Row,
                                          justifyContent = Justify.FlexEnd,
                                          alignItems = Align.Center
                              }
                  };
                  zoneLeft.Add(leftContainer);

                  var rightContainer = new IMGUIContainer(static () => OnToolbarGUIRightOfCenter?.Invoke())
                  {
                              style =
                              {
                                          flexGrow = 1,
                                          flexDirection = FlexDirection.Row,
                                          justifyContent = Justify.FlexStart,
                                          alignItems = Align.Center
                              }
                  };
                  rightContainer.style.flexGrow = 1;
                  zoneRight.Add(rightContainer);
            }

#endif
      }
}