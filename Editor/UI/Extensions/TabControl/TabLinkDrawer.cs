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

        static public void OnLinkGUI(Rect position, TabGroup group, TabLink link, TabLinkObject focus)
        {
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
                                    link.SetTab(tab);
                                }
                            }
                            else
                            {
                                link.SetTab(tab);
                            }
                        }
                    }
                    else
                    {
                        link.SetTab(tab);
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
                                link.SetPage(page);
                            }
                        }
                        else
                        {
                            link.SetPage(page);
                        }
                    }
                    else
                    {
                        link.SetPage(page);
                    }
                }
            }
        }

        static public void OnTabGUI(Rect rect, TabGroup group, Tab tab)
        {
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
                            tab.SetLinkPage(page);
                        }
                    }
                    else
                    {
                        tab.SetLinkPage(page);
                    }
                }
                else
                {
                    tab.SetLinkPage(page);
                }
            }
        }
    }
}

#endif