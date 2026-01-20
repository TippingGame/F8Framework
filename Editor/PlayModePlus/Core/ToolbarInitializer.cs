using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
      /// <summary>
      /// Initializes and manages custom toolbar elements based on configuration settings.
      /// This class creates toolbar elements from configuration data and handles their lifecycle.
      /// </summary>
      [InitializeOnLoad]
      public static class ToolbarInitializer
      {
            private readonly static List<BaseToolbarElement> LeftElements = new();
            private readonly static List<BaseToolbarElement> RightElements = new();

            static ToolbarInitializer()
            {
                  EditorApplication.delayCall += InitializeToolbar;
            }

            private static void InitializeToolbar()
            {
                  ToolbarConfiguration config = ScriptableObject.CreateInstance<ToolbarConfiguration>();
                  
                  var controlsGroup = new ToolbarGroup { groupName = "Controls", side = ToolbarSide.Left };
                  controlsGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarFpsSlider).AssemblyQualifiedName });
                  controlsGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarTimeSlider).AssemblyQualifiedName });
                  config.groups.Add(controlsGroup);
                  
                  var sceneGroup = new ToolbarGroup { groupName = "Scenes", side = ToolbarSide.Right };
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarStartSelectingScene).AssemblyQualifiedName });
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarSceneName).AssemblyQualifiedName });
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarSceneSelection).AssemblyQualifiedName });
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarScreenshot).AssemblyQualifiedName });
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarFindMissingReferences).AssemblyQualifiedName });
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarSelectPlayModeSettings).AssemblyQualifiedName });
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarOpenBuildWindow).AssemblyQualifiedName });
                  sceneGroup.elements.Add(new ToolbarElement { name = typeof(ToolbarSelectPlayerSettingsPresets).AssemblyQualifiedName });
                  config.groups.Add(sceneGroup);
                  
                  CreateElementsFromConfig(config);

                  // Register the toolbar drawing callbacks
                  ToolbarCallback.OnToolbarGUILeftOfCenter = DrawToolbar(LeftElements, true);
                  ToolbarCallback.OnToolbarGUIRightOfCenter = DrawToolbar(RightElements, false);

                  SubscribeToEditorEvents();

                  EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
                  
                  PlayModeManager.EditorStartScene();
            }

            private static void CreateElementsFromConfig(ToolbarConfiguration config)
            {
                  if (config == null)
                  {
                        return;
                  }

                  // Process each enabled group in the configuration
                  foreach (ToolbarGroup group in config.groups.Where(static g => g.isEnabled))
                  {
                        List<BaseToolbarElement> targetList = group.side == ToolbarSide.Left ? LeftElements : RightElements;

                        targetList.Add(new ToolbarSpace());

                        // Process each enabled element within the current group
                        foreach (ToolbarElement elementConfig in group.elements.Where(static e => e.isEnabled))
                        {
                              var type = Type.GetType(elementConfig.name);

                              if (type != null)
                              {
                                    // Create and add an instance of the toolbar element using Activator
                                    var elementInstance = (BaseToolbarElement)Activator.CreateInstance(type);
                                    targetList.Add(elementInstance);
                              }
                        }
                  }

                  // Add trailing spaces to both sides if they contain any elements
                  if (LeftElements.Any())
                  {
                        LeftElements.Add(new ToolbarSpace());
                  }

                  if (RightElements.Any())
                  {
                        RightElements.Add(new ToolbarSpace());
                  }
            }

            // Creates and returns an Action that draws all toolbar elements in a horizontal layout.
            private static Action DrawToolbar(IReadOnlyList<BaseToolbarElement> elements, bool alignRight)
            {
                  return () =>
                  {
                        GUILayout.BeginHorizontal();

                        if (alignRight)
                        {
                              GUILayout.FlexibleSpace();
                        }

                        foreach (BaseToolbarElement element in elements)
                        {
                              element.OnDrawInToolbar();
                        }

                        GUILayout.EndHorizontal();
                  };
            }

            // Sets up event subscriptions for all toolbar elements.
            private static void SubscribeToEditorEvents()
            {
                  // Combine all toolbar elements from both sides into a single collection
                  List<BaseToolbarElement> allElements = LeftElements.Concat(RightElements).ToList();

                  if (!allElements.Any())
                  {
                        return;
                  }

                  // Subscribe to Unity's play mode state change event
                  EditorApplication.playModeStateChanged += state => { allElements.ForEach(e => e.OnPlayModeStateChanged(state)); };

                  // Initialize all toolbar elements by calling their OnInit method
                  allElements.ForEach(static e => e.OnInit());
            }

            private static void HandlePlayModeStateChange(PlayModeStateChange state)
            {
            }
      }
}