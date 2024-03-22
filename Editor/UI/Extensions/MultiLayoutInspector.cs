#if UNITY_EDITOR

namespace F8Framework.Core.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(MultiLayout))]
    public class MultiLayoutInspector : Editor
    {
        private const string LAYOUT_TITLE           = "Layout";
        private const string TOTAL_LAYOUT_FORMAT    = "Total Layout : {0}";
        private const string SELECTED_LAYOUT        = "Selected Layout";
        private const string ADD_LAYER              = "Add Layer";
        
        private const string TARGET_TITLE           = "Target";
        private const string TOTAL_TARGET_FORMAT    = "Total Target : {0}";
        private const string ADD_TARGET             = "Add Target";

        private const string SAVE                   = "Save";
        private const string REMOVE                 = "-";
        private const string EMPTY                  = "Empty";

        private const float HORIZONTAL_INDENT       = 20.0f;
        private const float VERTICAL_SPACE          = 5.0f;

        private const float SMALL_BUTTON_WIDTH      = 30.0f;
        private const float MIDDLE_BUTTON_WIDTH     = 80.0f;
        private const float BIG_BUTTON_WIDTH        = 100.0f;
        private const float BUTTON_SPACE            = 10.0f;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            DrawLayout();

            GUILayout.Space(VERTICAL_SPACE);

            DrawTarget();
        }

        private void DrawLayout()
        {
            MultiLayout multiLayout = target as MultiLayout;
            
            GUILayout.BeginVertical();

            GUILayout.Label(LAYOUT_TITLE);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(HORIZONTAL_INDENT);

            string totalLayout = string.Format(TOTAL_LAYOUT_FORMAT, multiLayout.layout.count);
            GUILayout.Label(totalLayout);

            Rect rect = GUILayoutUtility.GetLastRect();
            Vector2 totalLayoutSize = GUI.skin.label.CalcSize(new GUIContent(totalLayout));

            if(GUI.Button(new Rect(rect.x + totalLayoutSize.x + BUTTON_SPACE, rect.y, BIG_BUTTON_WIDTH, rect.height), ADD_LAYER) == true)
            {
                AddLayout();
            }
            
            GUILayout.EndHorizontal();

            GUILayout.Space(VERTICAL_SPACE);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HORIZONTAL_INDENT);

            string[]    layerNumber = new string[multiLayout.layout.count];

            for (int number = 0; number < multiLayout.layout.count; ++number)
            {
                layerNumber[number] = (number+1).ToString();
            }

            int selectedLayout = EditorGUILayout.Popup(SELECTED_LAYOUT, multiLayout.layout.current, layerNumber);
            if (selectedLayout != multiLayout.layout.current)
            {
                multiLayout.layout.SelectLayout(selectedLayout);
            }

            if (GUILayout.Button(SAVE, GUILayout.Width(MIDDLE_BUTTON_WIDTH)) == true)
            {
                multiLayout.layout.SaveCurrentLayer();
            }

            if (multiLayout.layout.count > 1)
            {
                if (GUILayout.Button(REMOVE, GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    RemoveLayout();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        
        private void AddLayout()
        {
            MultiLayout multiLayout = target as MultiLayout;

            multiLayout.layout.AddLayout();
        }

        private void RemoveLayout()
        {
            MultiLayout multiLayout = target as MultiLayout;

            multiLayout.layout.RemoveCurrentLayout();
        }

        private void DrawTarget()
        {
            MultiLayout multiLayout = target as MultiLayout;

            GUILayout.BeginVertical();
            
            GUILayout.Label(TARGET_TITLE);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(HORIZONTAL_INDENT);

            string totalTarget = string.Format(TOTAL_TARGET_FORMAT, multiLayout.layout.targets.Count);
            GUILayout.Label(totalTarget);

            Rect rect = GUILayoutUtility.GetLastRect();
            Vector2 targetSize = GUI.skin.label.CalcSize(new GUIContent(totalTarget));
            if (GUI.Button(new Rect(rect.x + targetSize.x + BUTTON_SPACE, rect.y, BIG_BUTTON_WIDTH, rect.height), ADD_TARGET) == true)
            {
                multiLayout.layout.AddTarget();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(VERTICAL_SPACE);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HORIZONTAL_INDENT);

            var targets = multiLayout.layout.targets;
            if (targets.Count > 0)
            {
                GUILayout.BeginVertical();
                for (int index = 0; index < targets.Count; ++index)
                {
                    GUILayout.BeginHorizontal();

                    RectTransform rectTransformOrigin = targets[index].rectTransform;
                    RectTransform rectTransformNew = EditorGUILayout.ObjectField(rectTransformOrigin, typeof(RectTransform), true) as RectTransform;

                    if (rectTransformNew != null && rectTransformNew.Equals(rectTransformOrigin) == false)
                    {
                        multiLayout.layout.SetTargetRectTransfrom(index, rectTransformNew);
                    }

                    if (GUILayout.Button(REMOVE, GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                    {
                        multiLayout.layout.RemoveTarget(index);
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label(EMPTY);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

}

#endif