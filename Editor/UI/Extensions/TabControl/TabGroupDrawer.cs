#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    [CustomPropertyDrawer(typeof(TabGroup))]
    public class TabGroupDrawer : PropertyDrawer
    {
        private TabGroup group;
        private TabGroupReorderable tabGroupReorderable;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            Object obj = property.serializedObject.targetObject;
            
            if (obj is TabController)
            {
                TabController tabController = obj as TabController;
                group = tabController.tabGroup;
            }
            else if(obj is TabLinkObject)
            {
                TabLinkObject tabLinkObject = obj as TabLinkObject;
                group = tabLinkObject.GetGroup();
            }
        
            if (group != null)
            {
                if (tabGroupReorderable == null ||
                    tabGroupReorderable.group != group)
                {
                    tabGroupReorderable = new TabGroupReorderable(group, null);
                }

                height += EditorGUIUtility.standardVerticalSpacing;
                height += tabGroupReorderable.GetHeight();
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);
            if (tabGroupReorderable == null)
            {
                base.OnGUI(position, property, label);
            }
            else
            {
                Rect rect = new Rect(position.x, position.y + EditorGUIUtility.standardVerticalSpacing, position.width, tabGroupReorderable.GetHeight());

                tabGroupReorderable.Draw(rect);
            }
        }
    }
}

#endif