using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    [CustomEditor(typeof(TextExtension))]
    public class TextExtensionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
            DrawSection("References（引用）", DrawReferences);
            DrawSection("Spacing（间距）", DrawSpacing);
            DrawSection("Vertex Gradient（渐变）", DrawVertexGradient);
            DrawSection("Shadow（投影）", DrawShadow);
            DrawSection("Outline（描边）", DrawOutline);
            DrawSection("Hyperlink（超链接）", DrawHyperlink);
            DrawSection("Source Text（原始文本）", DrawSourceText);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawReferences()
        {
            DrawProperty("_text");
        }

        private void DrawSpacing()
        {
            DrawProperty("_characterSpacing");
            DrawProperty("_lineSpacing");
        }

        private void DrawVertexGradient()
        {
            DrawProperty("_enableVertexGradient");
            if (FindProperty("_enableVertexGradient").boolValue)
            {
                DrawProperty("_gradientMode");
                DrawProperty("_gradientBlendMode");
                DrawGradientFields((TextExtension.GradientMode)FindProperty("_gradientMode").enumValueIndex);
                DrawProperty("_gradientOffsetX");
                DrawProperty("_gradientOffsetY");
            }
        }

        private void DrawShadow()
        {
            DrawProperty("_enableShadow");
            if (FindProperty("_enableShadow").boolValue)
            {
                DrawProperty("_shadowColor");
                DrawProperty("_shadowOffset");
            }

            DrawProperty("_enableSoftShadow");
            if (FindProperty("_enableSoftShadow").boolValue)
            {
                DrawProperty("_softShadowColor");
                DrawProperty("_softShadowOffset");
                DrawProperty("_softShadowBlur");
                DrawProperty("_softShadowSamples");
            }
        }

        private void DrawOutline()
        {
            DrawProperty("_enableOutline");
            if (FindProperty("_enableOutline").boolValue)
            {
                DrawProperty("_outlineColor");
                DrawProperty("_outlineDistance");
            }

            DrawProperty("_enableSoftOutline");
            if (FindProperty("_enableSoftOutline").boolValue)
            {
                DrawProperty("_softOutlineColor");
                DrawProperty("_softOutlineDistance");
                DrawProperty("_softOutlineSamples");
            }
        }

        private void DrawHyperlink()
        {
            DrawProperty("_enableHyperlink");
            if (FindProperty("_enableHyperlink").boolValue)
            {
                DrawProperty("_tintHyperlink");
                if (FindProperty("_tintHyperlink").boolValue)
                {
                    DrawProperty("_hyperlinkColor");
                }

                DrawProperty("_openUrlOnClick");
                DrawProperty("_onHyperlinkClick");
                DrawHyperlinkUsageExamples();
            }
        }

        private void DrawSourceText()
        {
            DrawReadOnlyTextArea("_sourceText", "Source Text");
        }

        private void DrawGradientFields(TextExtension.GradientMode gradientMode)
        {
            switch (gradientMode)
            {
                case TextExtension.GradientMode.FourCorner:
                    DrawProperty("_topLeft");
                    DrawProperty("_topRight");
                    DrawProperty("_bottomLeft");
                    DrawProperty("_bottomRight");
                    break;
                case TextExtension.GradientMode.HorizontalTwoColor:
                case TextExtension.GradientMode.VerticalTwoColor:
                    DrawProperty("_startColor");
                    DrawProperty("_endColor");
                    break;
                case TextExtension.GradientMode.HorizontalThreeColor:
                case TextExtension.GradientMode.VerticalThreeColor:
                    DrawProperty("_startColor");
                    DrawProperty("_middleColor");
                    DrawProperty("_endColor");
                    DrawProperty("_threeColorPivot");
                    break;
            }
        }

        private SerializedProperty FindProperty(string propertyName)
        {
            return serializedObject.FindProperty(propertyName);
        }

        private void DrawProperty(string propertyName)
        {
            SerializedProperty property = FindProperty(propertyName);
            if (property != null)
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        private void DrawReadOnlyTextArea(string propertyName, string label)
        {
            SerializedProperty property = FindProperty(propertyName);
            if (property == null)
            {
                return;
            }
            GUIStyle style = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };

            float height = Mathf.Max(60f, style.CalcHeight(new GUIContent(property.stringValue), EditorGUIUtility.currentViewWidth - 40f));
            EditorGUILayout.SelectableLabel(property.stringValue, style, GUILayout.MinHeight(height));
        }

        private void DrawHyperlinkUsageExamples()
        {
            const string examples =
                "<link=https://github.com/TippingGame/F8Framework>打开官网</link>\n" +
                "[link=item_1001]查看物品详情[/link]";

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("Hyperlink Examples（例子）");

            GUIStyle style = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };

            float height = Mathf.Max(44f, style.CalcHeight(new GUIContent(examples), EditorGUIUtility.currentViewWidth - 40f));
            EditorGUILayout.SelectableLabel(examples, style, GUILayout.MinHeight(height));
        }

        private static void DrawSection(string title, System.Action drawer)
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            drawer.Invoke();
            EditorGUILayout.EndVertical();
        }

        private void DrawScriptField()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                MonoScript script = MonoScript.FromMonoBehaviour((TextExtension)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }
    }
}
