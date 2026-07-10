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
                RecordMultiLayout(multiLayout, "Select MultiLayout", true);
                multiLayout.layout.SelectLayout(selectedLayout);
                MarkMultiLayoutDirty(multiLayout, true);
            }

            if (GUILayout.Button(SAVE, GUILayout.Width(MIDDLE_BUTTON_WIDTH)) == true)
            {
                RecordMultiLayout(multiLayout, "Save MultiLayout Layer");
                multiLayout.layout.SaveCurrentLayer();
                MarkMultiLayoutDirty(multiLayout);
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
            if (multiLayout == null)
            {
                return;
            }

            RecordMultiLayout(multiLayout, "Add MultiLayout Layer");
            multiLayout.layout.AddLayout();
            MarkMultiLayoutDirty(multiLayout);
        }

        private void RemoveLayout()
        {
            MultiLayout multiLayout = target as MultiLayout;
            if (multiLayout == null)
            {
                return;
            }

            RecordMultiLayout(multiLayout, "Remove MultiLayout Layer");
            multiLayout.layout.RemoveCurrentLayout();
            MarkMultiLayoutDirty(multiLayout);
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
                RecordMultiLayout(multiLayout, "Add MultiLayout Target");
                multiLayout.layout.AddTarget();
                MarkMultiLayoutDirty(multiLayout);
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
                        RecordMultiLayout(multiLayout, "Change MultiLayout Target");
                        multiLayout.layout.SetTargetRectTransfrom(index, rectTransformNew);
                        MarkMultiLayoutDirty(multiLayout);
                    }

                    if (GUILayout.Button(REMOVE, GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                    {
                        RecordMultiLayout(multiLayout, "Remove MultiLayout Target");
                        multiLayout.layout.RemoveTarget(index);
                        MarkMultiLayoutDirty(multiLayout);
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

        private void RecordMultiLayout(MultiLayout multiLayout, string undoName, bool includeTargets = false)
        {
            if (multiLayout == null)
            {
                return;
            }

            Undo.RecordObject(multiLayout, undoName);
            if (includeTargets == true)
            {
                RecordTargetObjects(multiLayout, undoName);
            }
        }

        private void RecordTargetObjects(MultiLayout multiLayout, string undoName)
        {
            if (multiLayout == null ||
                multiLayout.layout == null ||
                multiLayout.layout.targets == null)
            {
                return;
            }

            for (int index = 0; index < multiLayout.layout.targets.Count; index++)
            {
                RectTransform rectTransform = multiLayout.layout.targets[index].rectTransform;
                if (rectTransform == null)
                {
                    continue;
                }

                Undo.RecordObject(rectTransform, undoName);
                Undo.RecordObject(rectTransform.gameObject, undoName);
            }
        }

        private void MarkMultiLayoutDirty(MultiLayout multiLayout, bool includeTargets = false)
        {
            if (multiLayout == null)
            {
                return;
            }

            MarkObjectDirty(multiLayout);
            if (includeTargets == true)
            {
                MarkTargetObjectsDirty(multiLayout);
            }
        }

        private void MarkTargetObjectsDirty(MultiLayout multiLayout)
        {
            if (multiLayout == null ||
                multiLayout.layout == null ||
                multiLayout.layout.targets == null)
            {
                return;
            }

            for (int index = 0; index < multiLayout.layout.targets.Count; index++)
            {
                RectTransform rectTransform = multiLayout.layout.targets[index].rectTransform;
                if (rectTransform == null)
                {
                    continue;
                }

                MarkObjectDirty(rectTransform);
                MarkObjectDirty(rectTransform.gameObject);
            }
        }

        private void MarkObjectDirty(Object obj)
        {
            if (obj == null)
            {
                return;
            }

            EditorUtility.SetDirty(obj);
            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        }
    }

}

#endif
