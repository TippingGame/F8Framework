#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    [CustomPropertyDrawer(typeof(TabRoot))]
    public class TabRootDrawer : PropertyDrawer
    {
        static private bool openController = false;
        private bool needOpenController = false;
        private const string CONTROLLER = "Controller";
        private const string ADDLINK = "Add Link";
        private const string LINK = "Link";
        private const string EXPAND = "Expand";

        private TabController targetController = null;

        private TabGroupReorderable tabGroupReorderable;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TabLinkObject tabObject = property.serializedObject.targetObject as TabLinkObject;
            if(tabObject != null)
            {
                float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                TabGroup linkedGroup = tabObject.GetGroup();
                if (linkedGroup != null)
                {
                    if (linkedGroup.IsLinked(tabObject) == true)
                    {
                        targetController = linkedGroup.GetController();
                    }
                    else
                    {
                        tabObject.UnLink();
                        linkedGroup = null;
                    }
                }

                if (targetController != null)
                {
                    TabGroup tabGroup = targetController.tabGroup;
                    bool isLinked = tabGroup.IsLinked(tabObject);
                    if (isLinked == false ||
                        tabObject is Tab)
                    {
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }

                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    if (IsOpenController() == true)
                    {
                        if (tabGroupReorderable == null ||
                            tabGroupReorderable.group != tabGroup)
                        {
                            tabGroupReorderable = new TabGroupReorderable(tabGroup, tabObject);
                        }

                        height += tabGroupReorderable.GetHeight();
                    }
                    else
                    {
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                return height;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        private bool IsOpenController()
        {
            return openController || needOpenController;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TabLinkObject tabObject = property.serializedObject.targetObject as TabLinkObject;
            if (tabObject != null)
            {
                if (targetController != null)
                {
                    TabGroup tabGroup = targetController.tabGroup;
                    bool isLinked = tabGroup.IsLinked(tabObject);

                    position.x += 10;
                    position.width -= 10;
                    Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    TabController controller = (TabController)EditorGUI.ObjectField(rect, CONTROLLER, targetController, typeof(TabController), true);
                    if (controller != targetController)
                    {
                        if (isLinked == true)
                        {
                            if (controller == null)
                            {
                                EditorGUIUtility.PingObject(targetController);
                                
                                if (EditorUtility.DisplayDialog("tab Error", "This is a linked tap. Are you sure you want to disconnect?", "Yes", "No") == true)
                                {
                                    isLinked = false;
                                    tabGroup.Release(tabObject);
                                    targetController = null;
                                    return;
                                }
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("tab Error", "This is a linked tab. Are you sure you want to remove and replace the existing connection?", "Yes", "No");
                            }
                        }
                        else
                        {
                            targetController = controller;
                        }
                    }

                    if (isLinked == true)
                    {
                        if (tabObject is Tab)
                        {
                            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

                            Tab tab = tabObject as Tab;
                            LinkDrawer.OnTabGUI(rect, tabGroup, tab);
                        }
                    }
                    else
                    {
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        if (GUI.Button(rect, ADDLINK) == true)
                        {
                            if (tabObject is Tab)
                            {
                                Tab tab = tabObject as Tab;
                                targetController.tabGroup.Add(tab, null);
                            }
                            else if(tabObject is TabPage)
                            {
                                TabPage tabPage = tabObject as TabPage;
                                targetController.tabGroup.Add(null, tabPage);
                            }
                        }
                    }

                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                    rect = new Rect(rect.x + EditorGUIUtility.singleLineHeight, rect.y, rect.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

                    bool isOpen = EditorGUI.Foldout(rect, IsOpenController(), LINK);
                    if(isOpen != IsOpenController())
                    {
                        openController = isOpen;
                    }
                    if (isOpen == true)
                    {
                        if (tabGroupReorderable == null)
                        {
                            tabGroupReorderable = new TabGroupReorderable(tabGroup, tabObject);
                        }

                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        rect = new Rect(rect.x, rect.y, rect.width, tabGroupReorderable.GetHeight());
                        tabGroupReorderable.Draw(rect);
                    }
                    else
                    {
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

                        rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        if (GUI.Button(rect, EXPAND) == true)
                        {
                            openController = true;
                        }
                    }
                }
                else
                {
                    Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    TabController tabController = (TabController)EditorGUI.ObjectField(rect, CONTROLLER, targetController, typeof(TabController), true);
                    if (tabController != null)
                    {
                        targetController = tabController;

                        needOpenController = true;
                    }

                    rect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight);
                }
            }
            else
            {
                base.OnGUI(position, property, label);
            }
        }
    }
}

#endif