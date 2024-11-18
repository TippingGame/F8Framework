using System;
using System.Collections.Generic;
using UnityEditor;

namespace F8Framework.Core.Editor
{
    [InitializeOnLoad]
    public static class CustomUnityToolbar
    {
        private static readonly List<Action> LeftToolbarGUI = new();
        private static readonly List<Action> RightToolbarGUI = new();

        static CustomUnityToolbar()
        {
            CustomUnityToolbarCallback.OnToolbarGUILeft = () => { DrawToolbar(LeftToolbarGUI); };
            CustomUnityToolbarCallback.OnToolbarGUIRight = () => { DrawToolbar(RightToolbarGUI); };
        }

        private static void DrawToolbar(List<Action> toolbarActions)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var action in toolbarActions) action();

            EditorGUILayout.EndHorizontal();
        }
    }
}