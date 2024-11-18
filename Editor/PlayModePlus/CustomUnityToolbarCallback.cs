using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace F8Framework.Core.Editor
{
    public static class CustomUnityToolbarCallback
    {
        private static readonly Type MToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

        private static ScriptableObject _mCurrentToolbar;

        private static Action _onToolbarGUI;
        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static CustomUnityToolbarCallback()
        {
            EditorApplication.delayCall -= OnUpdate;
            EditorApplication.delayCall += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (_mCurrentToolbar != null) return;
            var toolbars = Resources.FindObjectsOfTypeAll(MToolbarType);
            _mCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;

            if (_mCurrentToolbar == null) return;
            var root = _mCurrentToolbar.GetType()
                .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            var mRoot = root?.GetValue(_mCurrentToolbar) as VisualElement;

            if (mRoot == null) return;
            var playButton = mRoot.Q("Play");
            var extendedPlayModeToolbar = new PlayModeToolbar();
            mRoot.Q("ToolbarZoneRightAlign").Add(extendedPlayModeToolbar);

            RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
            RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);
        }

        private static void RegisterCallback(string root, Action action)
        {
            var toolbarZone =
                _mCurrentToolbar?.GetType().GetProperty(root)?.GetValue(_mCurrentToolbar) as VisualElement;

            if (toolbarZone == null) return;
            var container = new IMGUIContainer();
            container.onGUIHandler = () => { action?.Invoke(); };
            container.style.flexGrow = 1;
            container.style.flexDirection = FlexDirection.Row;
            toolbarZone.Add(container);
        }
    }
}