#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace F8Framework.Core.Editor
{
    internal static class TabEditorUtility
    {
        public static void RecordGroup(TabGroup group, string undoName)
        {
            TabController controller = GetController(group);
            if (controller != null)
            {
                Undo.RecordObject(controller, undoName);
            }
        }

        public static void RecordLinkChange(TabGroup group, TabLinkObject oldObject, TabLinkObject newObject, string undoName)
        {
            RecordGroup(group, undoName);
            RecordLinkObject(oldObject, undoName);
            RecordLinkObject(newObject, undoName);
            RecordLinkedGroup(oldObject, undoName);
            RecordLinkedGroup(newObject, undoName);
        }

        public static void MarkGroupDirty(TabGroup group)
        {
            MarkObjectDirty(GetController(group));
        }

        public static void MarkLinkChangeDirty(TabGroup group, TabLinkObject oldObject, TabLinkObject newObject)
        {
            MarkGroupDirty(group);
            MarkObjectDirty(oldObject);
            MarkObjectDirty(newObject);
            MarkLinkedGroupDirty(oldObject);
            MarkLinkedGroupDirty(newObject);
        }

        public static void RecordLinkObject(TabLinkObject linkObject, string undoName)
        {
            if (linkObject != null)
            {
                Undo.RecordObject(linkObject, undoName);
            }
        }

        public static void MarkObjectDirty(Object obj)
        {
            if (obj == null)
            {
                return;
            }

            EditorUtility.SetDirty(obj);
            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        }

        private static void RecordLinkedGroup(TabLinkObject linkObject, string undoName)
        {
            if (linkObject != null)
            {
                RecordGroup(linkObject.GetGroup(), undoName);
            }
        }

        private static void MarkLinkedGroupDirty(TabLinkObject linkObject)
        {
            if (linkObject != null)
            {
                MarkGroupDirty(linkObject.GetGroup());
            }
        }

        private static TabController GetController(TabGroup group)
        {
            return group != null ? group.GetController() : null;
        }
    }

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
                    TabEditorUtility.RecordGroup(group, "Add Tab Link");
                    group.Add(null, null);
                    TabEditorUtility.MarkGroupDirty(group);
                };

            reorderable.onRemoveCallback =
                (list) =>
                {
                    Tab oldTab = null;
                    TabPage oldPage = null;
                    if (list.index >= 0 && list.index < group.list.Count)
                    {
                        TabLink link = group.list[list.index];
                        oldTab = link.GetTab();
                        oldPage = link.GetPage();
                        TabEditorUtility.RecordLinkChange(group, oldTab, oldPage, "Remove Tab Link");
                        TabEditorUtility.MarkLinkChangeDirty(group, oldTab, oldPage);
                    }
                    else
                    {
                        TabEditorUtility.RecordGroup(group, "Remove Tab Link");
                    }

                    group.Remove(list.index);

                    GUI.changed = true;
                    TabEditorUtility.MarkGroupDirty(group);
                    TabEditorUtility.MarkLinkChangeDirty(group, oldTab, oldPage);
                };

            reorderable.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    if (index < 0 || index >= group.list.Count)
                    {
                        return;
                    }

                    EditorGUI.BeginChangeCheck();
                    bool changed = LinkDrawer.OnLinkGUI(rect, group, group.list[index], focus);
                    if (EditorGUI.EndChangeCheck() == true || changed == true)
                    {
                        group.UpdateLink();
                        TabEditorUtility.MarkGroupDirty(group);
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
