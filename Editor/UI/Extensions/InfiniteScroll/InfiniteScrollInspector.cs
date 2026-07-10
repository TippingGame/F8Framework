#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core.Editor
{
    [CustomEditor(typeof(InfiniteScroll))]
    [CanEditMultipleObjects]
    public class InfiniteScrollInspector : UnityEditor.Editor
    {
        private const string PREVIEW_TITLE = "Item Preview";
        private const string PREVIEW_LAYOUT = "Preview Layout";
        private const string CLEAR_PREVIEW = "Clear Preview";
        private const string PREVIEW_NAME_PREFIX = "[InfiniteScroll Preview]";
        private const int DEFAULT_PREVIEW_COUNT = 10;
        private const float DEFAULT_ITEM_SIZE = 100.0f;

        private static readonly HideFlags PreviewHideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(PREVIEW_TITLE, EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(PREVIEW_LAYOUT))
                {
                    PreviewSelectedTargets();
                }

                if (GUILayout.Button(CLEAR_PREVIEW))
                {
                    ClearSelectedTargets();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Preview is available in edit mode only.", MessageType.Info);
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            bool propertyChanged = EditorGUI.EndChangeCheck();
            propertyChanged |= serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying == false && propertyChanged)
            {
                RefreshExistingPreviews();
            }
        }

        [MenuItem("CONTEXT/InfiniteScroll/Preview Layout")]
        private static void PreviewLayoutContext(MenuCommand command)
        {
            PreviewLayout(command.context as InfiniteScroll);
        }

        [MenuItem("CONTEXT/InfiniteScroll/Clear Preview")]
        private static void ClearPreviewContext(MenuCommand command)
        {
            ClearPreview(command.context as InfiniteScroll);
        }

        private void PreviewSelectedTargets()
        {
            foreach (Object selectedTarget in targets)
            {
                PreviewLayout(selectedTarget as InfiniteScroll);
            }
        }

        private void ClearSelectedTargets()
        {
            foreach (Object selectedTarget in targets)
            {
                ClearPreview(selectedTarget as InfiniteScroll);
            }
        }

        private void RefreshExistingPreviews()
        {
            foreach (Object selectedTarget in targets)
            {
                InfiniteScroll scroll = selectedTarget as InfiniteScroll;
                if (HasPreview(scroll))
                {
                    PreviewLayout(scroll);
                }
            }
        }

        private static void PreviewLayout(InfiniteScroll scroll)
        {
            if (scroll == null)
            {
                return;
            }

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Preview InfiniteScroll Layout");
            ClearPreview(scroll, false);

            ScrollRect scrollRect;
            RectTransform content;
            RectTransform itemPrefab;
            string error;
            if (TryGetPreviewContext(scroll, out scrollRect, out content, out itemPrefab, out error) == false)
            {
                Undo.CollapseUndoOperations(undoGroup);
                LogF8.LogWarning(error, scroll);
                return;
            }

            int previewCount = scroll.needItemCount > 0 ? scroll.needItemCount : DEFAULT_PREVIEW_COUNT;

            ScrollLayout previewLayout = CreatePreviewLayout(scroll.layout, scrollRect);
            PreviewContent(scroll, previewLayout, content, itemPrefab, previewCount);

            Undo.CollapseUndoOperations(undoGroup);
            EditorUtility.SetDirty(content);
            SceneView.RepaintAll();
        }

        private static void PreviewContent(InfiniteScroll scroll, ScrollLayout layout, RectTransform content, RectTransform itemPrefab, int previewCount)
        {
            Vector2 itemAnchorMin;
            Vector2 itemAnchorMax;
            GetItemAnchor(layout, out itemAnchorMin, out itemAnchorMax);

            Vector2 itemPivot = layout.GetItemPivot();
            Vector2 contentPivot = layout.GetAxisPivot();
            float itemSize = GetPreviewItemSize(layout, itemPrefab);
            int lineCount = GetPreviewLineCount(layout, previewCount);
            float contentSize = GetPreviewContentSize(layout, itemSize, lineCount);

            Undo.RecordObject(content, "Preview InfiniteScroll Content");
            content.anchorMin = itemAnchorMin;
            content.anchorMax = itemAnchorMax;
            content.pivot = contentPivot;
            content.sizeDelta = layout.GetAxisVector(-layout.padding, contentSize);

            float crossSize = Mathf.Max(1.0f, layout.GetCrossSize(content.rect));
            for (int itemIndex = 0; itemIndex < previewCount; itemIndex++)
            {
                RectTransform previewItem = CreatePreviewItem(scroll, content, itemIndex);
                if (previewItem == null)
                {
                    continue;
                }

                previewItem.anchorMin = itemAnchorMin;
                previewItem.anchorMax = itemAnchorMax;
                previewItem.pivot = itemPivot;

                layout.FitItemSize(previewItem, itemIndex, itemSize);
                layout.FitItemInlinePosition(previewItem, itemIndex, crossSize);
                previewItem.anchoredPosition = layout.GetAxisVector(Vector2.zero, GetPreviewItemPosition(layout, itemSize, itemIndex));
            }
        }

        private static RectTransform CreatePreviewItem(InfiniteScroll scroll, RectTransform content, int itemIndex)
        {
            GameObject source = scroll.itemPrefab.gameObject;
            GameObject previewObject = InstantiatePreviewObject(source, content);
            if (previewObject == null)
            {
                return null;
            }

            Undo.RegisterCreatedObjectUndo(previewObject, "Create InfiniteScroll Preview Item");

            previewObject.name = string.Format("{0} {1}", PREVIEW_NAME_PREFIX, itemIndex);
            previewObject.SetActive(true);
            SetHideFlags(previewObject.transform);

            RectTransform rectTransform = previewObject.transform as RectTransform;
            if (rectTransform == null)
            {
                Undo.DestroyObjectImmediate(previewObject);
                return null;
            }

            rectTransform.SetParent(content, false);
            rectTransform.SetAsLastSibling();

            return rectTransform;
        }

        private static GameObject InstantiatePreviewObject(GameObject source, Transform parent)
        {
            return Object.Instantiate(source, parent, false);
        }

        private static void ClearPreview(InfiniteScroll scroll)
        {
            ClearPreview(scroll, true);
        }

        private static void ClearPreview(InfiniteScroll scroll, bool collapseUndo)
        {
            if (scroll == null)
            {
                return;
            }

            RectTransform content = GetContent(scroll);
            if (content == null)
            {
                return;
            }

            int undoGroup = Undo.GetCurrentGroup();
            if (collapseUndo)
            {
                Undo.SetCurrentGroupName("Clear InfiniteScroll Preview");
            }

            for (int index = content.childCount - 1; index >= 0; index--)
            {
                Transform child = content.GetChild(index);
                if (IsPreviewItem(child) == true)
                {
                    Undo.DestroyObjectImmediate(child.gameObject);
                }
            }

            if (collapseUndo)
            {
                Undo.CollapseUndoOperations(undoGroup);
                SceneView.RepaintAll();
            }
        }

        private static bool HasPreview(InfiniteScroll scroll)
        {
            RectTransform content = GetContent(scroll);
            if (content == null)
            {
                return false;
            }

            for (int index = 0; index < content.childCount; index++)
            {
                if (IsPreviewItem(content.GetChild(index)) == true)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetPreviewContext(InfiniteScroll scroll, out ScrollRect scrollRect, out RectTransform content, out RectTransform itemPrefab, out string error)
        {
            scrollRect = null;
            content = null;
            itemPrefab = null;
            error = null;

            if (scroll == null)
            {
                error = "InfiniteScroll preview target is missing.";
                return false;
            }

            scrollRect = scroll.GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                error = "InfiniteScroll preview needs a ScrollRect component on the same GameObject.";
                return false;
            }

            content = scrollRect.content;
            if (content == null)
            {
                error = "InfiniteScroll preview needs ScrollRect.content assigned.";
                return false;
            }

            if (scroll.itemPrefab == null)
            {
                error = "InfiniteScroll preview needs itemPrefab assigned.";
                return false;
            }

            itemPrefab = scroll.itemPrefab.transform as RectTransform;
            if (itemPrefab == null)
            {
                error = "InfiniteScroll preview itemPrefab must use RectTransform.";
                return false;
            }

            return true;
        }

        private static RectTransform GetContent(InfiniteScroll scroll)
        {
            if (scroll == null)
            {
                return null;
            }

            ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                return null;
            }

            return scrollRect.content;
        }

        private static ScrollLayout CreatePreviewLayout(ScrollLayout source, ScrollRect scrollRect)
        {
            if (source == null)
            {
                source = new ScrollLayout();
            }

            ScrollLayout layout = new ScrollLayout();
            layout.axis = source.axis;
            layout.padding = source.padding;
            layout.space = source.space;
            layout.topToBotton = source.topToBotton;
            layout.leftToRight = source.leftToRight;

            if (source.values != null)
            {
                for (int index = 0; index < source.values.Count; index++)
                {
                    ScrollLayout.LayoutValue sourceValue = source.values[index];
                    ScrollLayout.LayoutValue value = new ScrollLayout.LayoutValue();
                    value.valueType = sourceValue.valueType;
                    value.value = sourceValue.value;
                    layout.values.Add(value);
                }
            }

            layout.SetDefaults();
            if (layout.axis == ScrollAxis.DEFAULT)
            {
                layout.axis = scrollRect.vertical ? ScrollAxis.VERTICAL_TOP : ScrollAxis.HORIZONTAL_LEFT;
            }

            return layout;
        }

        private static void GetItemAnchor(ScrollLayout layout, out Vector2 anchorMin, out Vector2 anchorMax)
        {
            Rect anchor = layout.GetItemAnchor();
            anchorMin = new Vector2(anchor.xMin, anchor.yMin);
            anchorMax = new Vector2(anchor.xMax, anchor.yMax);
        }

        private static float GetPreviewItemSize(ScrollLayout layout, RectTransform itemPrefab)
        {
            float itemSize = Mathf.Abs(layout.GetMainSize(itemPrefab.sizeDelta));
            if (itemSize <= 0.0f)
            {
                itemSize = Mathf.Abs(layout.GetMainSize(itemPrefab.rect));
            }

            return itemSize > 0.0f ? itemSize : DEFAULT_ITEM_SIZE;
        }

        private static int GetPreviewLineCount(ScrollLayout layout, int previewCount)
        {
            if (layout.IsGrid() == false)
            {
                return previewCount;
            }

            return Mathf.CeilToInt((float)previewCount / layout.GridCount());
        }

        private static float GetPreviewContentSize(ScrollLayout layout, float itemSize, int lineCount)
        {
            float contentSize = itemSize * lineCount + layout.MainPadding * 2.0f;
            if (lineCount > 1)
            {
                contentSize += (lineCount - 1) * layout.MainSpace;
            }

            return contentSize;
        }

        private static float GetPreviewItemPosition(ScrollLayout layout, float itemSize, int itemIndex)
        {
            int lineIndex = layout.IsGrid() ? itemIndex / layout.GridCount() : itemIndex;
            float offset = lineIndex * itemSize;
            float distance = offset + layout.MainPadding;
            if (lineIndex > 0)
            {
                distance += lineIndex * layout.MainSpace;
            }

            return -layout.GetAxisPostionFromOffset(distance);
        }

        private static bool IsPreviewItem(Transform transform)
        {
            return transform != null && transform.name.StartsWith(PREVIEW_NAME_PREFIX);
        }

        private static void SetHideFlags(Transform root)
        {
            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            for (int index = 0; index < children.Length; index++)
            {
                children[index].gameObject.hideFlags = PreviewHideFlags;
            }
        }
    }
}
#endif
