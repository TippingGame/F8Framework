using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using DataContext = F8Framework.Core.InfiniteScrollItem.DataContext;

namespace F8Framework.Core
{
    public partial class InfiniteScroll : MonoBehaviour
    {
        public InfiniteScrollItem itemPrefab = null;
        public int padding = 0;
        public int space = 0;
        public bool dynamicItemSize = false;

        public ScrollLayout layout = new ScrollLayout();

        protected bool isInitialize = false;
        protected RectTransform content = null;
        protected Vector2 anchorMin = Vector2.zero;
        protected Vector2 anchorMax = Vector2.zero;

        protected float itemPivot = 0.0f;

        protected bool processing = false;
        protected bool isDirty = true;
        protected bool isChangedSize = true;

        private int firstDataIndex = 0;
        private int lastDataIndex = 0;
        private float firstItemPosition = 0;
        private float lastItemtPosition = 0;

        private bool isStartLine = true;
        private bool isEndLine = true;

        private bool changeValue = false;

        public ChangeValueEvent onChangeValue = new ChangeValueEvent();
        public ItemActiveEvent onChangeActiveItem = new ItemActiveEvent();
        public StateChangeEvent onStartLine = new StateChangeEvent();
        public StateChangeEvent onEndLine = new StateChangeEvent();

        private void Awake()
        {
            Initialize();
        }

        protected void Initialize()
        {
            if (isInitialize == false)
            {
                if (content == null)
                {
                    content = (RectTransform)transform;
                }

                scrollRect = GetComponent<ScrollRect>();
                content = scrollRect.content;
                viewport = scrollRect.viewport;
                isVertical = scrollRect.vertical;

                RectTransform itemTransform = (RectTransform)itemPrefab.transform;
                InitializeItemInformation(itemTransform);

                layout.Initialize(this, isVertical);

                ClearScrollContent();

                SetItemPivot(defaultItemSize, content.pivot);

                dataList.Clear();

                scrollRect.onValueChanged.AddListener(OnValueChanged);

                isInitialize = true;
                isDirty = true;
            }
        }

        public void InsertData(InfiniteScrollData data, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            AddData(data);

            isChangedSize = true;

            UpdateAllData(immediately);
        }

        public void InsertData(InfiniteScrollData data, int insertIndex, bool immediately = false)
        {
            if (insertIndex < 0 || insertIndex > dataList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (isInitialize == false)
            {
                Initialize();
            }

            InsertData(data, insertIndex);

            isChangedSize = true;

            UpdateAllData(immediately);
        }

        public void InsertData(InfiniteScrollData[] datas, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            foreach (InfiniteScrollData data in datas)
            {
                AddData(data);
            }

            isChangedSize = true;

            UpdateAllData(immediately);
        }

        public void InsertData(InfiniteScrollData[] datas, int insertIndex, bool immediately = false)
        {
            if (insertIndex < 0 || insertIndex > dataList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (isInitialize == false)
            {
                Initialize();
            }

            foreach (InfiniteScrollData data in datas)
            {
                InsertData(data, insertIndex++);
            }

            isChangedSize = true;

            UpdateAllData(immediately);
        }

        public void RemoveData(InfiniteScrollData data, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            int dataIndex = GetDataIndex(data);

            RemoveData(dataIndex, immediately);
        }

        public void RemoveData(int dataIndex, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (IsValidDataIndex(dataIndex) == true)
            {
                foreach (InfiniteScrollItem item in items)
                {
                    if (dataIndex == item.GetDataIndex())
                    {
                        item.ClearData(true);
                    }
                }

                selectDataIndex = -1;

                dataList.RemoveAt(dataIndex);
                for (int i = dataIndex; i < dataList.Count; i++)
                {
                    dataList[i].index--;
                }

                if (dataIndex < firstDataIndex)
                {
                    firstDataIndex--;
                }

                if (dataIndex < lastDataIndex)
                {
                    lastDataIndex--;
                }

                isChangedSize = true;

                UpdateAllData(immediately);
            }
        }

        public void ClearData(bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectDataIndex = -1;
            dataList.Clear();

            ClearItemsData();

            isChangedSize = false;

            ClearScrollContent();

            UpdateAllData(immediately);
        }

        public void Clear()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectDataIndex = -1;
            dataList.Clear();

            ClearItems();

            isChangedSize = false;

            ClearScrollContent();
        }

        public void UpdateData(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            DataContext context = GetDataContext(data);
            if (context != null &&
                context.index >= 0)
            {
                if (IsShowDataIndex(context.index) == true)
                {
                    InfiniteScrollItem item = GetItemByDataIndex(context.index);
                    if (item != null)
                    {
                        if (item.IsActive() == true)
                        {
                            item.UpdateItem(context);
                        }
                    }
                }
            }
        }

        public void UpdateAllData(bool immediately = true)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            SetDirty();
            if (immediately == true)
            {
                UpdateShowItem(true);
            }
        }

        public bool IsVersical()
        {
            return isVertical;
        }

        protected float GetViewportSize()
        {
            float size = 0;
            if (isVertical == true)
            {
                size = viewport.rect.height;
            }
            else
            {
                size = viewport.rect.width;
            }

            return size;
        }

        protected void ResizeContent()
        {
            Vector2 currentSize = content.sizeDelta;
            float size = GetItemTotalSize();

            if (isVertical == true)
            {
                content.sizeDelta = new Vector2(currentSize.x, size);
            }
            else
            {
                content.sizeDelta = new Vector2(size, currentSize.y);
            }
        }

        protected float GetContentSize()
        {
            if (isChangedSize == true)
            {
                ResizeContent();
            }


            float size = 0;
            if (isVertical == true)
            {
                size = content.rect.height;
            }
            else
            {
                size = content.rect.width;
            }

            return size;
        }

        protected float GetContentPosition()
        {
            float position = 0;
            if (isVertical == true)
            {
                position = content.anchoredPosition.y;
            }
            else
            {
                position = -content.anchoredPosition.x;
            }

            return position;
        }

        public void ResizeScrollView()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            CheckNeedMoreItem();
        }

        public float GetItemPosition(int dataIndex)
        {
            float itemPivot = GetItemPivot();
            float passingItemSize = GetItemSizeSumToIndex(dataIndex);

            if (isVertical == true)
            {
                return itemPivot - passingItemSize;
            }
            else
            {
                return itemPivot + passingItemSize;
            }
        }

        public int GetItemCount()
        {
            return GetDataCount();
        }

        protected void SetItemPivot(float itemSize, Vector2 pivot)
        {
            if (isVertical == true)
            {
                itemPivot = itemSize * pivot.y - itemSize - padding;
            }
            else
            {
                itemPivot = itemSize * pivot.x + padding;
            }
        }

        public float GetItemPivot()
        {
            return itemPivot;
        }


        protected void RefreshScroll()
        {
            if (isDirty == true)
            {
                UpdateShowItem();
            }
        }

        protected bool NeedUpdateItem()
        {
            if (isDirty == true)
            {
                return true;
            }

            float contentsPosition = GetContentPosition();
            if (updatedContentsPosition != contentsPosition)
            {
                return true;
            }

            return false;
        }

        protected void UpdateShowItem(bool forceUpdateData = false)
        {
            if (forceUpdateData == false &&
                processing == true)
            {
                return;
            }

            if (NeedUpdateItem() == false)
            {
                return;
            }

            updatedContentsPosition = GetContentPosition();
            isDirty = false;

            processing = true;

            int prevFirstDataIndex = firstDataIndex;
            int prevLastDataIndex = lastDataIndex;

            firstDataIndex = GetShowFirstDataIndex();
            lastDataIndex = GetShowLastDataIndex();

            if (prevFirstDataIndex != firstDataIndex)
            {
                for (int dataIndex = prevFirstDataIndex; dataIndex < firstDataIndex; dataIndex++)
                {
                    InfiniteScrollItem item = GetItemByDataIndex(dataIndex);
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                }

                changeValue = true;
            }

            if (prevLastDataIndex != lastDataIndex)
            {
                for (int dataIndex = lastDataIndex + 1; dataIndex <= prevLastDataIndex; dataIndex++)
                {
                    InfiniteScrollItem item = GetItemByDataIndex(dataIndex);
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                }
            }

            int lineIndex = 0;

            float itemPosition = 0;
            float lineSize = layout.GetLineSize(lineIndex);

            for (int dataIndex = 0; dataIndex < firstDataIndex; ++dataIndex)
            {
                int dataLineIndex = layout.GetLineIndex(dataIndex);
                if (lineIndex != dataLineIndex)
                {
                    itemPosition += lineSize + space;

                    lineIndex = dataLineIndex;
                    lineSize = layout.GetLineSize(lineIndex);
                }
            }

            float contentPosition = GetContentPosition();
            float viewportSize = GetViewportSize();

            for (int dataIndex = firstDataIndex; dataIndex < dataList.Count; ++dataIndex)
            {
                int dataLineIndex = layout.GetLineIndex(dataIndex);
                if (lineIndex != dataLineIndex)
                {
                    itemPosition += lineSize + space;

                    lineIndex = dataLineIndex;
                    lineSize = layout.GetLineSize(lineIndex);
                }

                if (dataIndex == firstDataIndex)
                {
                    firstItemPosition = itemPosition;
                }

                if (IsShowAfterPosition(itemPosition, contentPosition, viewportSize) == true)
                {
                    break;
                }

                InfiniteScrollItem item = PullItemByDataIndex(dataIndex);

                bool needUpdateData = false;

                if (item.IsActive() == false ||
                    item.GetDataIndex() != dataIndex)
                {
                    item.SetData(dataIndex);

                    needUpdateData = true;

                    changeValue = true;
                }

                if (needUpdateData == true || forceUpdateData == true)
                {
                    item.UpdateItem(dataList[dataIndex]);
                }

                RectTransform itemTransform = (RectTransform)item.transform;
                if (item.IsUpdateItemSize() == true)
                {
                    float size = GetItemSize(dataIndex);
                    if (size > 0 &&
                        size < minItemSize)
                    {
                        minItemSize = size;
                    }

                    layout.FitItemSize(itemTransform, dataIndex, size);

                    isChangedSize = true;
                    item.UpdatedItemSize();
                }

                layout.FitItemPosition(itemTransform, dataIndex);

                lastDataIndex = dataIndex;
                lastItemtPosition = itemPosition;
            }

            if (prevLastDataIndex != lastDataIndex)
            {
                for (int dataIndex = lastDataIndex + 1; dataIndex <= prevLastDataIndex; dataIndex++)
                {
                    InfiniteScrollItem item = GetItemByDataIndex(dataIndex);
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                }

                changeValue = true;
            }

            if (changeValue == true)
            {
                onChangeValue.Invoke(firstDataIndex, lastDataIndex, isStartLine, isEndLine);
                changeValue = false;
            }

            if (isChangedSize == true)
            {
                ResizeContent();
            }

            isChangedSize = false;

            processing = false;
        }

        protected bool IsShowDataIndex(int dataIndex)
        {
            if (dataIndex >= firstDataIndex && dataIndex <= lastDataIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool IsShowPosition(float position)
        {
            float contentPosition = GetContentPosition();
            float viewportSize = GetViewportSize();

            float viewPosition = position - contentPosition;
            if (IsShowBeforePosition(position, contentPosition) == false &&
                IsShowAfterPosition(position, contentPosition, viewportSize) == false)
            {
                return true;
            }

            return false;
        }

        protected bool IsShowBeforePosition(float position, float contentPosition)
        {
            float viewPosition = position - contentPosition;
            if (viewPosition < 0)
            {
                return true;
            }

            return false;
        }

        protected bool IsShowAfterPosition(float position, float contentPosition, float viewportSize)
        {
            float viewPosition = position - contentPosition;
            if (viewPosition >= viewportSize)
            {
                return true;
            }

            return false;
        }

        protected int GetShowFirstDataIndex()
        {
            return layout.GetLineFirstItemIndex(GetShowFirstLine());
        }

        protected int GetShowFirstLine()
        {
            float contentPosition = GetContentPosition();

            float lastPosition = 0.0f;

            int lineCount = layout.GetLineCount();
            for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                lastPosition += layout.GetLineSize(lineIndex);

                if (IsShowBeforePosition(lastPosition, contentPosition) == false)
                {
                    return lineIndex;
                }

                lastPosition += space;
            }

            return lineCount;
        }

        protected int GetShowLastDataIndex()
        {
            return layout.GetLineLastItemIndex(GetShowLastLine());
        }

        protected int GetShowLastLine()
        {
            float contentPosition = GetContentPosition();
            float viewportSize = GetViewportSize();

            float linePosition = 0.0f;

            int lineCount = layout.GetLineCount();
            for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                if (IsShowAfterPosition(linePosition, contentPosition, viewportSize) == true)
                {
                    return lineIndex;
                }

                linePosition += layout.GetLineSize(lineIndex);
                linePosition += space;
            }

            return lineCount;
        }

        public void SetDirty()
        {
            isDirty = true;
        }

        private void Update()
        {
            if (NeedUpdateItem() == true)
            {
                UpdateShowItem();
            }
        }

        private void OnValidate()
        {
            layout.SetDefaults();
        }


        [Serializable]
        public class ChangeValueEvent : UnityEvent<int, int, bool, bool>
        {
            public ChangeValueEvent()
            {
            }
        }
    }
}