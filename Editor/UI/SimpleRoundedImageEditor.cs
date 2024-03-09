using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace F8Framework.Core.Editor
{
    [CustomEditor(typeof(SimpleRoundedImage), true)]
    //[CanEditMultipleObjects]
    public class SimpleRoundedImageEditor : ImageEditor
    {
        SerializedProperty m_Radius;
        SerializedProperty m_TriangleNum;
        SerializedProperty m_Sprite;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_Radius = serializedObject.FindProperty("Radius");
            m_TriangleNum = serializedObject.FindProperty("TriangleNum");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpriteGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();
            bool showNativeSize = m_Sprite.objectReferenceValue != null;
            m_ShowNativeSize.target = showNativeSize;
            NativeSizeButtonGUI();
            EditorGUILayout.PropertyField(m_Radius);
            EditorGUILayout.PropertyField(m_TriangleNum);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}