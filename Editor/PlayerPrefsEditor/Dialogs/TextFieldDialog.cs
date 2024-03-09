using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class TextFieldDialog : EditorWindow
    {
        [NonSerialized]
        private string resultString = string.Empty;

        [NonSerialized]
        private Action<string> callback;

        [NonSerialized]
        private string description;

        [NonSerialized]
        private List<TextValidator> validatorList = new List<TextValidator>();

        [NonSerialized]
        private TextValidator errorValidator = null;

        public static void OpenDialog(string title, string description, List<TextValidator> validatorList, Action<string> callback, EditorWindow targetWin = null)
        {
            TextFieldDialog window = ScriptableObject.CreateInstance<TextFieldDialog>();

            window.name = "TextFieldDialog '" + title + "'";
            window.titleContent =  new GUIContent (title);
            window.description = description;
            window.callback = callback;
            window.validatorList = validatorList;
            window.position = new Rect(0, 0, 350, 140);

            window.ShowUtility();

            window.CenterOnWindow(targetWin);
            window.Focus();
            EditorWindow.FocusWindowIfItsOpen<TextFieldDialog>();
        }

        void OnGUI()
        {
            errorValidator = null;

            Color defaultColor = GUI.contentColor;

            GUILayout.Space(20);
            EditorGUILayout.LabelField(description);
            GUILayout.Space(20);

            GUI.SetNextControlName(name+"_textInput");
            resultString = EditorGUILayout.TextField(resultString, GUILayout.ExpandWidth(true));
//            GUILayout.Space(20);
            GUILayout.FlexibleSpace();


            foreach(TextValidator val in validatorList)
            {
                if (!val.Validate(resultString))
                {
                    errorValidator = val;
                    break;
                }
            }
            bool lockOkButton = !(errorValidator != null && errorValidator.m_errorType == TextValidator.ErrorType.Error);

            GUILayout.BeginHorizontal();

            if(errorValidator != null)
            {
                switch (errorValidator.m_errorType)
                {
                    case TextValidator.ErrorType.Info:
                        GUI.contentColor = Styles.Colors.Blue;
                        GUILayout.Box(new GUIContent(ImageManager.Info, errorValidator.m_failureMsg), Styles.icon);
                        break;
                    case TextValidator.ErrorType.Warning:
                        GUI.contentColor = Styles.Colors.Yellow;
                        GUILayout.Box(new GUIContent(ImageManager.Exclamation, errorValidator.m_failureMsg), Styles.icon);
                        break;
                    case TextValidator.ErrorType.Error:
                        GUI.contentColor = Styles.Colors.Red;
                        GUILayout.Box(new GUIContent(ImageManager.Exclamation, errorValidator.m_failureMsg), Styles.icon);
                        break;
                }
                GUI.contentColor = defaultColor;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(75.0f)))
                this.Close();

            GUI.enabled = lockOkButton;

            if (GUILayout.Button("OK", GUILayout.Width(75.0f)))
            {
                callback(resultString);
                Close();
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            // set focus only if element exist
            try
            { 
                EditorGUI.FocusTextInControl(name+"_textInput");
            }
            catch (MissingReferenceException)
            { } 

            if (Event.current != null && Event.current.isKey)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                        if (lockOkButton)
                        {
                            callback(resultString);
                            Close();
                        }
                        break;
                    case KeyCode.Escape:
                        Close();
                        break;
                }

            }
        }
    }
}