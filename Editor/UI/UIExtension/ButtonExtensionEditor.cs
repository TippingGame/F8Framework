using UnityEditor;

namespace F8Framework.Core.Editor
{
    [CustomEditor(typeof(ButtonExtension))]
    public sealed class ButtonExtensionEditor : UnityEditor.Editor
    {
        private SerializedProperty _button;
        private SerializedProperty _animationTarget;
        private SerializedProperty _parentScrollRect;

        private SerializedProperty _forwardDragToParentScrollRect;
        private SerializedProperty _forwardScrollWheelToParentScrollRect;
        private SerializedProperty _autoFindParentScrollRect;
        private SerializedProperty _dragStartThreshold;
        private SerializedProperty _cancelClickWhenDragging;
        private SerializedProperty _cancelLongPressWhenBeginDrag;

        private SerializedProperty _enableClickAnimation;
        private SerializedProperty _clickScaleMultiplier;
        private SerializedProperty _clickPressDuration;
        private SerializedProperty _clickReleaseDuration;
        private SerializedProperty _clickPressEase;
        private SerializedProperty _clickReleaseEase;

        private SerializedProperty _enableSelectAnimation;
        private SerializedProperty _selectedScaleMultiplier;
        private SerializedProperty _selectAnimationDuration;
        private SerializedProperty _selectEase;
        private SerializedProperty _deselectAnimationDuration;
        private SerializedProperty _deselectEase;

        private SerializedProperty _playClickSound;
        private SerializedProperty _clickSoundAssetName;
        private SerializedProperty _playSelectSound;
        private SerializedProperty _selectSoundAssetName;

        private SerializedProperty _enableDoubleClick;
        private SerializedProperty _doubleClickInterval;
        private SerializedProperty _onDoubleClick;

        private SerializedProperty _enableLongPress;
        private SerializedProperty _longPressDuration;
        private SerializedProperty _cancelLongPressWhenPointerExit;
        private SerializedProperty _onLongPress;

        private SerializedProperty _enableSubmitInputEnhancement;
        private SerializedProperty _submitActionName;
        private SerializedProperty _enableSubmitLongPress;
        private SerializedProperty _enableSubmitDoubleClick;

        private SerializedProperty _onSelect;
        private SerializedProperty _onDeselect;

        private void OnEnable()
        {
            _button = serializedObject.FindProperty("_button");
            _animationTarget = serializedObject.FindProperty("_animationTarget");
            _parentScrollRect = serializedObject.FindProperty("_parentScrollRect");

            _forwardDragToParentScrollRect = serializedObject.FindProperty("_forwardDragToParentScrollRect");
            _forwardScrollWheelToParentScrollRect = serializedObject.FindProperty("_forwardScrollWheelToParentScrollRect");
            _autoFindParentScrollRect = serializedObject.FindProperty("_autoFindParentScrollRect");
            _dragStartThreshold = serializedObject.FindProperty("_dragStartThreshold");
            _cancelClickWhenDragging = serializedObject.FindProperty("_cancelClickWhenDragging");
            _cancelLongPressWhenBeginDrag = serializedObject.FindProperty("_cancelLongPressWhenBeginDrag");

            _enableClickAnimation = serializedObject.FindProperty("_enableClickAnimation");
            _clickScaleMultiplier = serializedObject.FindProperty("_clickScaleMultiplier");
            _clickPressDuration = serializedObject.FindProperty("_clickPressDuration");
            _clickReleaseDuration = serializedObject.FindProperty("_clickReleaseDuration");
            _clickPressEase = serializedObject.FindProperty("_clickPressEase");
            _clickReleaseEase = serializedObject.FindProperty("_clickReleaseEase");

            _enableSelectAnimation = serializedObject.FindProperty("_enableSelectAnimation");
            _selectedScaleMultiplier = serializedObject.FindProperty("_selectedScaleMultiplier");
            _selectAnimationDuration = serializedObject.FindProperty("_selectAnimationDuration");
            _selectEase = serializedObject.FindProperty("_selectEase");
            _deselectAnimationDuration = serializedObject.FindProperty("_deselectAnimationDuration");
            _deselectEase = serializedObject.FindProperty("_deselectEase");

            _playClickSound = serializedObject.FindProperty("_playClickSound");
            _clickSoundAssetName = serializedObject.FindProperty("_clickSoundAssetName");
            _playSelectSound = serializedObject.FindProperty("_playSelectSound");
            _selectSoundAssetName = serializedObject.FindProperty("_selectSoundAssetName");

            _enableDoubleClick = serializedObject.FindProperty("_enableDoubleClick");
            _doubleClickInterval = serializedObject.FindProperty("_doubleClickInterval");
            _onDoubleClick = serializedObject.FindProperty("_onDoubleClick");

            _enableLongPress = serializedObject.FindProperty("_enableLongPress");
            _longPressDuration = serializedObject.FindProperty("_longPressDuration");
            _cancelLongPressWhenPointerExit = serializedObject.FindProperty("_cancelLongPressWhenPointerExit");
            _onLongPress = serializedObject.FindProperty("_onLongPress");

            _enableSubmitInputEnhancement = serializedObject.FindProperty("_enableSubmitInputEnhancement");
            _submitActionName = serializedObject.FindProperty("_submitActionName");
            _enableSubmitLongPress = serializedObject.FindProperty("_enableSubmitLongPress");
            _enableSubmitDoubleClick = serializedObject.FindProperty("_enableSubmitDoubleClick");

            _onSelect = serializedObject.FindProperty("_onSelect");
            _onDeselect = serializedObject.FindProperty("_onDeselect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();

            DrawSection("References（引用）", DrawReferences);
            DrawSection("ScrollRect Compatibility（滚动穿透）", DrawScrollRectCompatibility);
            DrawSection("Click Animation（点击动画）", DrawClickAnimation);
            DrawSection("Select Animation（选中动画）", DrawSelectAnimation);
            DrawSection("Sound（声音）", DrawSound);
            DrawSection("Double Click（双击）", DrawDoubleClick);
            DrawSection("Long Press（长按）", DrawLongPress);
            DrawSection("Submit Input（键盘手柄输入）", DrawSubmitInput);
            DrawSection("Selection Events（选中事件）", DrawSelectionEvents);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawReferences()
        {
            EditorGUILayout.PropertyField(_button);
            EditorGUILayout.PropertyField(_animationTarget);
        }

        private void DrawScrollRectCompatibility()
        {
            EditorGUILayout.PropertyField(_forwardDragToParentScrollRect);
            EditorGUILayout.PropertyField(_forwardScrollWheelToParentScrollRect);
            EditorGUILayout.PropertyField(_autoFindParentScrollRect);

            bool needsScrollRect = _forwardDragToParentScrollRect.boolValue ||
                                   _forwardScrollWheelToParentScrollRect.boolValue ||
                                   _autoFindParentScrollRect.boolValue;
            if (needsScrollRect)
            {
                EditorGUILayout.PropertyField(_parentScrollRect);
            }

            if (_forwardDragToParentScrollRect.boolValue)
            {
                EditorGUILayout.PropertyField(_dragStartThreshold);
                EditorGUILayout.PropertyField(_cancelClickWhenDragging);
                EditorGUILayout.PropertyField(_cancelLongPressWhenBeginDrag);
            }
        }

        private void DrawClickAnimation()
        {
            EditorGUILayout.PropertyField(_enableClickAnimation);
            if (!_enableClickAnimation.boolValue)
            {
                return;
            }

            EditorGUILayout.PropertyField(_clickScaleMultiplier);
            EditorGUILayout.PropertyField(_clickPressDuration);
            EditorGUILayout.PropertyField(_clickReleaseDuration);
            EditorGUILayout.PropertyField(_clickPressEase);
            EditorGUILayout.PropertyField(_clickReleaseEase);
        }

        private void DrawSelectAnimation()
        {
            EditorGUILayout.PropertyField(_enableSelectAnimation);
            if (!_enableSelectAnimation.boolValue)
            {
                return;
            }

            EditorGUILayout.PropertyField(_selectedScaleMultiplier);
            EditorGUILayout.PropertyField(_selectAnimationDuration);
            EditorGUILayout.PropertyField(_selectEase);
            EditorGUILayout.PropertyField(_deselectAnimationDuration);
            EditorGUILayout.PropertyField(_deselectEase);
        }

        private void DrawSound()
        {
            EditorGUILayout.PropertyField(_playClickSound);
            if (_playClickSound.boolValue)
            {
                EditorGUILayout.PropertyField(_clickSoundAssetName);
            }

            EditorGUILayout.PropertyField(_playSelectSound);
            if (_playSelectSound.boolValue)
            {
                EditorGUILayout.PropertyField(_selectSoundAssetName);
            }
        }

        private void DrawDoubleClick()
        {
            EditorGUILayout.PropertyField(_enableDoubleClick);
            if (!_enableDoubleClick.boolValue)
            {
                return;
            }

            EditorGUILayout.PropertyField(_doubleClickInterval);
            EditorGUILayout.PropertyField(_onDoubleClick);
        }

        private void DrawLongPress()
        {
            EditorGUILayout.PropertyField(_enableLongPress);
            if (!_enableLongPress.boolValue)
            {
                return;
            }

            EditorGUILayout.PropertyField(_longPressDuration);
            EditorGUILayout.PropertyField(_cancelLongPressWhenPointerExit);
            EditorGUILayout.PropertyField(_onLongPress);
        }

        private void DrawSubmitInput()
        {
            EditorGUILayout.PropertyField(_enableSubmitInputEnhancement);
            if (!_enableSubmitInputEnhancement.boolValue)
            {
                return;
            }

            EditorGUILayout.PropertyField(_submitActionName);

            if (_enableLongPress.boolValue)
            {
                EditorGUILayout.PropertyField(_enableSubmitLongPress);
            }

            if (_enableDoubleClick.boolValue)
            {
                EditorGUILayout.PropertyField(_enableSubmitDoubleClick);
            }
        }

        private void DrawSelectionEvents()
        {
            EditorGUILayout.PropertyField(_onSelect);
            EditorGUILayout.PropertyField(_onDeselect);
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
                MonoScript script = MonoScript.FromMonoBehaviour((ButtonExtension)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }
    }
}
