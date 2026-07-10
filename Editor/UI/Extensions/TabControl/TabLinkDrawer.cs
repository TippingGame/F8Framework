#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    [CustomPropertyDrawer(typeof(TabLink))]
    public class LinkDrawer : PropertyDrawer
    {
        public static GUIStyle ping = "PR Ping";
        public const string TAB = "tab";
        public const string TAB_NEXT = "tab_next";
        public const string PAGE = "page";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OnLinkGUIHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float space = EditorGUIUtility.singleLineHeight;
            float width = (position.width - space * 3) * 0.5f;
            float height = EditorGUIUtility.singleLineHeight;

            Rect rect = new Rect(position.x, position.y, width, height);

            GUIContent content = new GUIContent();
            EditorGUI.ObjectField(rect, property.FindPropertyRelative(TAB), content);

            rect = new Rect(rect.x + rect.width + space, rect.y, space, height);
            EditorGUI.LabelField(rect, new GUIContent(EditorGUIUtility.IconContent(TAB_NEXT)));

            rect = new Rect(rect.x + rect.width + space, rect.y, width, height);
            EditorGUI.ObjectField(rect, property.FindPropertyRelative(PAGE), content);
        }

        static public float OnLinkGUIHeight()
        {
            return EditorGUIUtility.singleLineHeight + 4;
        }

        static public bool OnLinkGUI(Rect position, TabGroup group, TabLink link, TabLinkObject focus)
        {
            bool changed = false;
            if (link != null)
            {
                float space = EditorGUIUtility.singleLineHeight;
                float width = (position.width - space * 3) * 0.5f;
                float height = EditorGUIUtility.singleLineHeight;

                Rect rect = new Rect(position.x, position.y, width, height);
                rect.y += 3;

                Tab linkedTab = link.GetTab();
                TabPage linkedPage = link.GetPage();
                
                if (focus != null &&
                    focus == linkedTab)
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        Rect pingRect = rect;

                        pingRect.x -= 4;
                        pingRect.width += 13;
                        pingRect.y -= 1;
                        pingRect.height += 1;


                        ping.Draw(pingRect, false, true, false, false);
                    }
                }
                Tab tab = (Tab)EditorGUI.ObjectField(rect, linkedTab, typeof(Tab), true);
                if(tab != linkedTab)
                {
                    if (tab != null)
                    {
                        if (group.Contain(tab) == true)
                        {
                            EditorUtility.DisplayDialog("tab Error", "Already added.", "Yes", "No");
                        }
                        else
                        {
                            TabGroup linkedGroup = tab.GetGroup();
                            if (linkedGroup != null &&
                                linkedGroup.Check(group) == false)
                            {
                                EditorGUIUtility.PingObject(linkedGroup.GetController());
                                
                                if (EditorUtility.DisplayDialog("tab Error", "This is a linked tab. Are you sure you want to remove and replace the existing connection?", "Yes", "No") == true)
                                {
                                    changed |= SetLinkTab(group, link, tab);
                                }
                            }
                            else
                            {
                                changed |= SetLinkTab(group, link, tab);
                            }
                        }
                    }
                    else
                    {
                        changed |= SetLinkTab(group, link, tab);
                    }
                }
                

                rect = new Rect(rect.x + rect.width + space, rect.y, space, height);

                EditorGUI.LabelField(rect, new GUIContent(EditorGUIUtility.IconContent(TAB_NEXT)));

                rect = new Rect(rect.x + rect.width + space, rect.y, width, height);

                if (focus != null &&
                    focus == linkedPage)
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        Rect pingRect = rect;
                        pingRect.x -= 4;
                        pingRect.width += 13;
                        pingRect.y -= 1;
                        pingRect.height += 1;
                        ping.Draw(pingRect, false, true, false, false);
                    }
                }

                TabPage page = (TabPage)EditorGUI.ObjectField(rect, linkedPage, typeof(TabPage), true);
                if (page != linkedPage)
                {
                    if(page != null)
                    {
                        TabGroup linkedGroup = page.GetGroup();
                        if (linkedGroup != null &&
                            linkedGroup.Check(group) == false)
                        {
                            EditorGUIUtility.PingObject(linkedGroup.GetController());
                            
                            if (EditorUtility.DisplayDialog("tab Error", "This is a linked tab. Are you sure you want to remove and replace the existing connection?", "Yes", "No") == true)
                            {
                                changed |= SetLinkPage(group, link, page);
                            }
                        }
                        else
                        {
                            changed |= SetLinkPage(group, link, page);
                        }
                    }
                    else
                    {
                        changed |= SetLinkPage(group, link, page);
                    }
                }
            }

            return changed;
        }

        static public bool OnTabGUI(Rect rect, TabGroup group, Tab tab)
        {
            bool changed = false;
            float x = rect.x;
            float space = EditorGUIUtility.singleLineHeight;
            float width = (rect.width - space * 3) * 0.5f;
            float height = EditorGUIUtility.singleLineHeight;

            rect = new Rect(x + space, rect.y, width, height);

            EditorGUI.LabelField(rect, "Linked Page");

            rect = new Rect(x + rect.width + space, rect.y, space, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, new GUIContent(EditorGUIUtility.IconContent(TAB_NEXT)));

            rect = new Rect(rect.x + rect.width + space, rect.y, width, height);
            
            TabPage linkedPage = tab.GetLinkedPage();
            TabPage page = (TabPage)EditorGUI.ObjectField(rect, linkedPage, typeof(TabPage), true);
            if (page != linkedPage)
            {
                if (page != null)
                {
                    TabGroup linkedGroup = page.GetGroup();
                    if (linkedGroup != null &&
                        linkedGroup.Check(group) == false)
                    {
                        EditorGUIUtility.PingObject(linkedGroup.GetController());
                        
                        if (EditorUtility.DisplayDialog("tab Error", "This is a linked tab. Are you sure you want to remove and replace the existing connection?", "Yes", "No") == true)
                        {
                            changed |= SetTabLinkPage(group, tab, page);
                        }
                    }
                    else
                    {
                        changed |= SetTabLinkPage(group, tab, page);
                    }
                }
                else
                {
                    changed |= SetTabLinkPage(group, tab, page);
                }
            }

            return changed;
        }

        private static bool SetLinkTab(TabGroup group, TabLink link, Tab tab)
        {
            Tab oldTab = link.GetTab();
            if (oldTab == tab)
            {
                return false;
            }

            TabEditorUtility.RecordLinkChange(group, oldTab, tab, "Change Tab Link");
            TabEditorUtility.MarkLinkChangeDirty(group, oldTab, tab);
            link.SetTab(tab);
            TabEditorUtility.MarkLinkChangeDirty(group, oldTab, tab);
            GUI.changed = true;
            return true;
        }

        private static bool SetLinkPage(TabGroup group, TabLink link, TabPage page)
        {
            TabPage oldPage = link.GetPage();
            if (oldPage == page)
            {
                return false;
            }

            TabEditorUtility.RecordLinkChange(group, oldPage, page, "Change Tab Page Link");
            TabEditorUtility.MarkLinkChangeDirty(group, oldPage, page);
            link.SetPage(page);
            TabEditorUtility.MarkLinkChangeDirty(group, oldPage, page);
            GUI.changed = true;
            return true;
        }

        private static bool SetTabLinkPage(TabGroup group, Tab tab, TabPage page)
        {
            TabPage oldPage = tab.GetLinkedPage();
            if (oldPage == page)
            {
                return false;
            }

            TabEditorUtility.RecordLinkChange(group, oldPage, page, "Change Tab Linked Page");
            TabEditorUtility.RecordLinkObject(tab, "Change Tab Linked Page");
            TabEditorUtility.MarkLinkChangeDirty(group, oldPage, page);
            TabEditorUtility.MarkObjectDirty(tab);
            tab.SetLinkPage(page);
            TabEditorUtility.MarkLinkChangeDirty(group, oldPage, page);
            TabEditorUtility.MarkObjectDirty(tab);
            GUI.changed = true;
            return true;
        }
    }
}

#endif
