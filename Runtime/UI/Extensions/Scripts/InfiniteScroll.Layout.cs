using UnityEngine;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public partial class InfiniteScroll
    {
        [Header("Layout", order = 3)]
        public ScrollLayout layout = new ScrollLayout();

        public class LineLayout
        {
            public LineLayout(int index)
            {
                this.index = index;
            }

            public void Clear()
            {
                dataList.Clear();
                offset = 0;
                size = 0;
            }

            public float Add(DataContext context)
            {
                context.offset = offset;
                dataList.Add(context);

                float contextSize = context.GetItemSize();
                if (size < contextSize)
                {
                    size = contextSize;
                }

                return offset + size;
            }

            internal int index = 0;
            internal List<DataContext> dataList = new List<DataContext>();

            internal float offset = 0;
            internal float size = 0;

            public int GetCount()
            {
                return dataList.Count;
            }
        }

        public class CachedScrollData
        {
            public float contentPosition = 0;
            public float contentSize = 0;
            public float viewportSize = 0;
            
            public Vector2 padding;
            public Vector2 space;

            public int gridCount = 0;

            public ScrollAxis axis;
            public bool topToBotton = true;
            public bool leftToRight = true;

            public List<ScrollLayout.LayoutValue> values = new List<ScrollLayout.LayoutValue>();

            public bool IsVertical = false;

            public Vector2 anchorMin = Vector2.zero;
            public Vector2 anchorMax = Vector2.zero;
            public Vector2 itemPivot = Vector2.zero;

            public Vector2 contentPivot = Vector2.zero;

            public void Clear()
            {
                contentPosition = 0;
                contentSize = 0;
                viewportSize = 0;
            }
        }

        internal List<LineLayout> lineLayout = new List<LineLayout>();
        internal float layoutSize = 0;
        internal int lineCount = 0;

        private int showLineIndex = 0;
        private int showLineCount = 0;

        private int firstItemIndex = 0;
        private int lastItemIndex = 0;

        private bool isUpdateArea = false;
        private bool isRebuildLayout = false;

        protected bool needReBuildLayout = true;

        private bool isStartLine = true;
        private bool isEndLine = true;

        internal bool processing = false;

        private CachedScrollData cachedData = new CachedScrollData();

        internal bool anchorUpdate = false;

        [SerializeField]
        [HideInInspector]
        [System.Obsolete("padding is obsolete. Use GetMainPadding() instead (UnityUpgradable) -> GetMainPadding()", false)]
        private int padding = 0;

        [SerializeField]
        [HideInInspector]
        [System.Obsolete("space is obsolete. Use GetMainSpace() instead (UnityUpgradable) -> GetMainSpace()", false)]
        private int space = 0;

        public void SetScrollAxis(ScrollAxis axis)
        {
            layout.axis = axis;

            CheckScrollAxis();
            CheckScrollData();
        }

        public ScrollAxis GetScrollAxis()
        {
            return layout.axis;
        }

        public void SetPadding(Vector2 padding)
        {
            layout.padding = padding;
        }

        public void SetSpace(Vector2 space)
        {
            layout.space = space;
        }
        public Vector2 GetPadding()
        {
            return layout.padding;
        }

        public Vector2 GetSpace()
        {
            return layout.space;
        }

        public float GetMainPadding()
        {
#pragma warning disable 618
            if (padding > 0)
            {
                layout.MainPadding = padding;
                padding = 0;
            }
#pragma warning restore 618
            return layout.MainPadding;
        }

        public float GetMainSpace()
        {
#pragma warning disable 618
            if (space > 0)
            {
                layout.MainSpace = space;
                space = 0;
            }
#pragma warning restore 618
            return layout.MainSpace;
        }

        public float GetCrossPadding()
        {
            return layout.CrossPadding;
        }


        public float GetCrossSpace()
        {
            return layout.CrossSpace;
        }

        protected void CheckScrollData()
        {
            float contentPosition = GetContentPosition();
            float viewportSize = GetViewportSize();
            float contentSize = GetContentSize();

            if (cachedData.contentPosition != contentPosition)
            {
                cachedData.contentPosition = contentPosition;
                isUpdateArea = true;
            }

            if (cachedData.contentSize != contentSize)
            {
                cachedData.contentSize = contentSize;
                isUpdateArea = true;
            }

            if (cachedData.viewportSize != viewportSize)
            {
                cachedData.viewportSize = viewportSize;
                isUpdateArea = true;
            }

            if (cachedData.space != layout.space)
            {
                cachedData.space = layout.space;
                needReBuildLayout = true;
            }

            if (cachedData.padding != layout.padding)
            {
                cachedData.padding = layout.padding;
                needReBuildLayout = true;
            }

            if (cachedData.gridCount != layout.values.Count)
            {
                cachedData.gridCount = layout.values.Count;

                needReBuildLayout = true;
            }

            if (layout.values.Count > 0)
            {
                for(int i=0;i< layout.values.Count;i++)
                {
                    if(cachedData.values.Count < i+1)
                    {
                        cachedData.values.Add(new ScrollLayout.LayoutValue());
                        needReBuildLayout = true;
                    }

                    if( cachedData.values[i].valueType != layout.values[i].valueType ||
                        cachedData.values[i].value != layout.values[i].value)
                    {
                        cachedData.values[i].valueType = layout.values[i].valueType;
                        cachedData.values[i].value = layout.values[i].value;

                        needReBuildLayout = true;
                    }
                }
            }

            bool isUpdateLayout = false;
            if (cachedData.axis != layout.axis)
            {
                cachedData.axis = layout.axis;

                bool IsVertical = layout.IsVertical();
                if (cachedData.IsVertical != IsVertical)
                {
                    cachedData.IsVertical = IsVertical;

                    if (layout.IsVertical() == true)
                    {
                        scrollRect.vertical = true;
                    }
                    else
                    {
                        scrollRect.horizontal = true;
                    }
                }

                isUpdateLayout = true;
            }

            if (cachedData.topToBotton != layout.topToBotton)
            {
                cachedData.topToBotton = layout.topToBotton;
                isUpdateLayout = true;
            }

            if (cachedData.leftToRight != layout.leftToRight)
            {
                cachedData.leftToRight = layout.leftToRight;
                isUpdateLayout = true;
            }

            if (isUpdateLayout == true)
            {
                anchorUpdate = true;

                Rect anchor = layout.GetItemAnchor();
                cachedData.anchorMin = new Vector2(anchor.xMin, anchor.yMin);
                cachedData.anchorMax = new Vector2(anchor.xMax, anchor.yMax);

                cachedData.itemPivot = layout.GetItemPivot();
                cachedData.contentPivot = layout.GetAxisPivot();

                UpdateAxis();

                isUpdateArea = true;

                isUpdateLayout = false;
                anchorUpdate = false;
            }
        }

        private void UpdateAxis()
        {
            Vector2 normalizedPosition = scrollRect.normalizedPosition;

            content.anchorMin = cachedData.anchorMin;
            content.anchorMax = cachedData.anchorMax;
            content.pivot = cachedData.contentPivot;

            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                itemObjectList[index].SetAxis(cachedData.anchorMin, cachedData.anchorMax, cachedData.itemPivot);
            }

            scrollRect.normalizedPosition = normalizedPosition;
        }

        

        public void CheckScrollAxis()
        {
            layout.CheckAxis(scrollRect);
        }

        protected float GetItemTotalSize()
        {
            return GetTotalDistance();
        }

        protected float GetItemDistance(int itemIndex)
        {
            int lineIndex = GetLineIndex(itemIndex);
            return GetLineDistance(GetLineOffset(lineIndex), lineIndex);
        }


        protected void BuildLayout()
        {
            ClearLayout();

            for (int dataIndex = 0; dataIndex < dataList.Count; dataIndex++)
            {
                var context = dataList[dataIndex];

                if (context.itemIndex == -1)
                {
                    continue;
                }

                AddItem(context);
            }

            ResizeContent();

            isRebuildLayout = true;
        }

        protected void CheckShowLine()
        {
            bool showLine = false;

            showLineIndex = 0;
            showLineCount = 0;

            for (int lineIndex = 0; lineIndex < GetLineCount(); lineIndex++)
            {
                var line = GetLine(lineIndex);
                if (showLine == false)
                {
                    if (IsShowBeforePosition(GetLineDistance(line.offset + line.size, lineIndex), cachedData.contentPosition) == false)
                    {
                        showLineIndex = lineIndex;
                        showLine = true;

                        showLineCount++;
                    }
                }
                else
                {
                    if (IsShowAfterPosition(GetLineDistance(line.offset, lineIndex), cachedData.contentPosition, cachedData.viewportSize) == true)
                    {
                        break;
                    }

                    showLineCount++;
                }
            }

            ResizeScrollView();
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

            processing = true;

            if (forceUpdateData == true ||
                needReBuildLayout == true)
            {
                BuildLayout();
                needReBuildLayout = false;
            }

            if (forceUpdateData == true ||
                isUpdateArea == true ||
                isRebuildLayout == true)
            {
                CheckShowLine();

                isUpdateArea = false;
                isRebuildLayout = false;
            }

            int prevFirstItemIndex = firstItemIndex;
            firstItemIndex = GetLineFirstItemIndex(showLineIndex);

            int prevLastItemIndex = lastItemIndex;
            if (showLineCount > 0)
            {
                int lastLineIndex = showLineIndex + showLineCount - 1;
                lastItemIndex = GetLineLastItemIndex(lastLineIndex);
            }
            else
            {
                lastItemIndex = firstItemIndex;
            }

            if (prevFirstItemIndex < firstItemIndex ||
                prevLastItemIndex > lastItemIndex)
            {
                changeValue = true;
            }

            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                int linkedIndex = itemObjectList[index].GetItemIndex();
                if (linkedIndex < firstItemIndex ||
                    linkedIndex > lastItemIndex)
                {
                    if (itemObjectList[index].IsActive() == true)
                    {
                        itemObjectList[index].SetActive(false);
                    }
                }
            }

            for (int lineIndex = showLineIndex; lineIndex < showLineIndex + showLineCount; lineIndex++)
            {
                if (lineIndex >= GetLineCount())
                {
                    break;
                }

                var line = GetLine(lineIndex);
                for (int i = 0; i < line.GetCount(); i++)
                {
                    var context = line.dataList[i];

                    InfiniteScrollItem item = PullItem(context);

                    bool needUpdateItemData = false;

                    if (item.IsActive() == false ||
                        item.GetDataIndex() != context.index ||
                        context.IsNeedUpdateItemData() == true)
                    {
                        needUpdateItemData = true;
                        changeValue = true;
                    }

                    if (needUpdateItemData == true || forceUpdateData == true)
                    {
                        item.UpdateItem(context);
                    }

                    if (item.IsActive() == false)
                    {
                        item.SetActive(true, true);
                    }

                    RectTransform itemTransform = (RectTransform)item.transform;

                    itemTransform.anchorMin = cachedData.anchorMin;
                    itemTransform.anchorMax = cachedData.anchorMax;
                    itemTransform.pivot = cachedData.itemPivot;

                    if (item.needUpdateItemSize == true)
                    {
                        float size = context.GetItemSize();

                        layout.FitItemSize(itemTransform, context.itemIndex, size);

                        item.needUpdateItemSize = false;
                    }

                    FitItemPosition(itemTransform, context.itemIndex);
                }
            }

            if (changeValue == true)
            {
                onChangeValue.Invoke(firstItemIndex, lastItemIndex, isStartLine, isEndLine);
                changeValue = false;
            }

            ResizeContent();

            processing = false;
        }

        protected void FitItemPosition(RectTransform rectTransform, int itemIndex)
        {
            layout.FitItemInlinePosition(rectTransform, itemIndex, GetCrossSize());

            float itemPosition = GetItemPosition(itemIndex);
            rectTransform.anchoredPosition = layout.GetAxisVector(Vector2.zero, itemPosition);
        }

        protected float ItemPostionFromOffset(float offset)
        {
            float postion = layout.GetAxisPostionFromOffset(offset);

            postion += GetPivotPostion();

            return postion;
        }

        protected float GetPivotPostion()
        {
            float contentSize = GetContentSize();
            if (layout.IsVertical() == true)
            {
                return contentSize * (cachedData.contentPivot.y - cachedData.itemPivot.y);
            }
            else
            {
                return contentSize * (cachedData.contentPivot.x - cachedData.itemPivot.x);
            }
        }

        protected void ClearLayout()
        {
            layoutSize = 0;
            lineCount = 0;
        }

        protected void AddItem(DataContext context)
        {
            bool newLine = false;
            LineLayout currentLine = null;

            int lineIndex = lineCount - 1;
            if (lineCount == 0)
            {
                newLine = true;
            }
            else if (lineIndex < lineCount)
            {
                currentLine = lineLayout[lineIndex];
                if (currentLine.GetCount() >= layout.GridCount())
                {
                    newLine = true;
                }
            }
            else
            {
                newLine = true;
            }

            if (newLine == true)
            {
                lineIndex = lineCount;

                if (lineIndex < lineLayout.Count)
                {
                    currentLine = lineLayout[lineIndex];
                }
                else
                {
                    currentLine = new LineLayout(lineIndex);
                    lineLayout.Add(currentLine);
                }

                currentLine.Clear();
                currentLine.offset = layoutSize;

                newLine = false;
                lineCount++;
            }
            else
            {
                currentLine = lineLayout[lineIndex];
            }

            layoutSize = currentLine.Add(context);
        }

        protected DataContext GetItem(int itemIndex)
        {
            if (layout.IsGrid() == true)
            {
                int gridCount = layout.GridCount();

                int line = itemIndex / gridCount;
                int index = itemIndex % gridCount;

                return lineLayout[line].dataList[index];
            }
            else
            {
                return lineLayout[itemIndex].dataList[0];
            }
        }

        protected LineLayout GetLine(int lineIndex)
        {
            return lineLayout[lineIndex];
        }

        protected float GetLineOffset(int lineIndex)
        {
            return lineLayout[lineIndex].offset;
        }

        protected float GetItemOffset(int itemIndex)
        {
            int lineIndex = GetLineIndex(itemIndex);

            return lineLayout[lineIndex].offset;
        }

        protected int GetLineIndex(int itemIndex)
        {
            if (layout.IsGrid() == true)
            {
                return itemIndex / layout.GridCount();
            }
            return itemIndex;
        }

        protected bool IsLast(int itemIndex)
        {
            if (layout.IsGrid() == true)
            {
                if ((itemIndex + 1) % layout.GridCount() == 0)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        protected int GetLineCount()
        {
            return lineCount;
        }

        protected float GetTotalDistance()
        {
            int lineCount = GetLineCount();

            float size = layoutSize + GetMainPadding() * UPDOWN_MULTIPLY;
            if (lineCount > 1)
            {
                size += ((lineCount - 1) * GetMainSpace());
            }

            return size;
        }

        protected float GetLineDistance(float offset, int lineIndex)
        {
            float distance = offset + GetMainPadding();
            if (lineIndex > 0)
            {
                distance += lineIndex * GetMainSpace();
            }

            return distance;
        }


        protected int GetLineFirstItemIndex(int lineIndex)
        {
            int lineCount = GetLineCount();
            if (lineCount == 0)
            {
                return 0;
            }

            if (lineIndex >= lineCount)
            {
                lineIndex = lineCount - 1;
            }

            int firstItemIndex = lineIndex;
            if (layout.IsGrid() == true)
            {
                firstItemIndex = firstItemIndex * layout.GridCount();
            }

            if (firstItemIndex < 0)
            {
                firstItemIndex = 0;
            }

            return firstItemIndex;
        }

        protected int GetLineLastItemIndex(int lineIndex)
        {
            int lineCount = GetLineCount();
            if(lineCount == 0)
            {
                return 0;
            }
            if (lineIndex >= lineCount)
            {
                lineIndex = lineCount - 1;
            }

            int lastItemIndex = 0;
            if (layout.IsGrid() == true)
            {
                if (lineIndex > 0)
                {
                    lastItemIndex = lineIndex * layout.GridCount();
                }
                
                lastItemIndex += lineLayout[lineIndex].GetCount() - 1;
            }
            else
            {
                lastItemIndex = lineIndex;
            }

            return lastItemIndex;
        }

        protected float GetLineSize(int lineIndex)
        {
            int lineCount = GetLineCount();
            if (lineCount == 0)
            {
                return 0;
            }

            if (lineIndex >= lineCount)
            {
                lineIndex = lineCount - 1;
            }

            return lineLayout[lineIndex].size;
        }
    }
}