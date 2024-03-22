#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace F8Framework.Core.Editor
{
    public class TabGroupReorderable
    {
        const float HEADER_AXIS_X = 8;
        internal TabGroup group;
        internal TabLinkObject focus;

        internal ReorderableList reorderable;
        
        public static string TAB = "Tab";
        public static string PAGE = "Page";

        public TabGroupReorderable(TabGroup group, TabLinkObject focus = null)
        {
            this.group = group;
            this.focus = focus;

            reorderable = new ReorderableList(group.list, typeof(TabLink), true, true, true, true);

            reorderable.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 4;

            reorderable.drawHeaderCallback = (rect) =>
            {
                float space = EditorGUIUtility.singleLineHeight;
                float width = (rect.width - space * 2) * 0.5f;

                Rect tabRect = rect;
                tabRect.x += HEADER_AXIS_X;
                tabRect.width = width;

                Rect pageRect = rect;
                pageRect.x = rect.x + space + space + width + HEADER_AXIS_X;
                pageRect.width = width;
            
                EditorGUI.LabelField(tabRect, TAB);
                EditorGUI.LabelField(pageRect, PAGE);
            };

            reorderable.onAddCallback =
                (list) =>
                {
                    group.Add(null, null);

                    EditorUtility.SetDirty(group.GetController());
                };

            reorderable.onRemoveCallback =
                (list) =>
                {
                    group.Remove(list.index);

                    GUI.changed = true;

                    EditorUtility.SetDirty(group.GetController());
                };

            reorderable.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    bool checkChanged = GUI.changed;
                    if (GUI.changed == false)
                    {
                        checkChanged = true;
                    }

                    foreach (TabLink link in group.list)
                    {
                        LinkDrawer.OnLinkGUI(rect, group, group.list[index], focus);
                    }

                    if (checkChanged == true &&
                        GUI.changed == true)
                    {
                        group.UpdateLink();

                        EditorUtility.SetDirty(group.GetController());
                    }
                };
        }

        public float GetHeight()
        {
            return reorderable.headerHeight + reorderable.footerHeight + ((reorderable.elementHeight + EditorGUIUtility.standardVerticalSpacing) * group.list.Count);
        }

        public void Draw(Rect position)
        {
            reorderable.DoList(position);
        }
    }
}
#endif