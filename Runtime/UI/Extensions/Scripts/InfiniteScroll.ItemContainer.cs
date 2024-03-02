using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public partial class InfiniteScroll : IItemContainer
    {
        private const float NEED_MORE_ITEM_RATE = 2;
        private const float UPDOWN_MULTIPLY = 2.0f;

        private float defaultItemSize = 0.0f;
        private float minItemSize = 0.0f;

        private int needItemNumber = -1;
        private int madeItemNumber = 0;
        private List<InfiniteScrollItem> items = new List<InfiniteScrollItem>();

        private InfiniteScrollItem CreateItem()
        {
            InfiniteScrollItem item = Instantiate(itemPrefab);
            RectTransform itemTransform = (RectTransform)item.transform;

            itemTransform.anchorMin = anchorMin;
            itemTransform.anchorMax = anchorMax;
            itemTransform.pivot = content.pivot;

            if (isVertical == true)
            {
                itemTransform.sizeDelta = new Vector2(0, itemTransform.sizeDelta.y);
            }
            else
            {
                itemTransform.sizeDelta = new Vector2(itemTransform.sizeDelta.x, 0);
            }

            itemTransform.SetParent(content, false);

            items.Add(item);

            item.Initalize(this, madeItemNumber);
            item.AddSelectCallback(OnSelectItem);

            ++madeItemNumber;

            item.SetActive(false, false);

            return item;
        }

        private void InitializeItemInformation(RectTransform itemTransform)
        {
            if (isVertical == true)
            {
                defaultItemSize = itemTransform.rect.height;
            }
            else
            {
                defaultItemSize = itemTransform.rect.width;
            }

            SetItemPivot(defaultItemSize, itemTransform.pivot);

            minItemSize = defaultItemSize;
            needItemNumber = GetNeedItemNumber();

            items.Clear();
        }

        private float GetNeedSize()
        {
            float needItemSize = 0;
            if (isVertical == true)
            {
                needItemSize = viewport.rect.height * NEED_MORE_ITEM_RATE;
            }
            else
            {
                needItemSize = viewport.rect.width * NEED_MORE_ITEM_RATE;
            }

            return needItemSize;
        }

        private void CreateNeedItem()
        {
            int itemCount = GetItemCount();
            if (madeItemNumber > itemCount)
            {
                return;
            }

            if (dynamicItemSize == true)
            {
                float itemSizeSum = 0;
                float needItemSize = GetNeedSize();

                if (layout.IsGrid() == true)
                {
                    int lineCount = layout.GetLineCount();
                    for (int lineIndex = 0; lineIndex < lineCount; ++lineIndex)
                    {
                        itemSizeSum += layout.GetLineSize(lineIndex);

                        if (lineIndex + 1 < lineCount)
                        {
                            itemSizeSum += space;
                        }

                        if (itemSizeSum > needItemSize)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    for (int sizeIndex = 0; sizeIndex < itemCount; ++sizeIndex)
                    {
                        itemSizeSum += GetItemSize(sizeIndex);

                        if (sizeIndex + 1 < itemCount)
                        {
                            itemSizeSum += space;
                        }

                        if (itemSizeSum > needItemSize)
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                if (madeItemNumber > 0 &&
                    madeItemNumber == needItemNumber)
                {
                    return;
                }

                if (madeItemNumber > itemCount)
                {
                    return;
                }
            }

            CreateItem();
        }

        private void CheckNeedMoreItem()
        {
            int itemNumber = GetNeedItemNumber();

            if (needItemNumber < itemNumber)
            {
                int gap = itemNumber - needItemNumber;
                needItemNumber = itemNumber;

                if (GetItemCount() > 0)
                {
                    firstDataIndex = GetShowFirstDataIndex() + madeItemNumber;
                    int dataIndex = 0;

                    for (int count = 0; count < gap; ++count)
                    {
                        dataIndex = firstDataIndex + count;
                        CreateNeedItem();
                    }
                }

                SetDirty();
            }
        }

        private void ClearItemsData()
        {
            for (int index = 0; index < items.Count; ++index)
            {
                items[index].ClearData(false);
            }
        }

        private void ClearItems()
        {
            ClearItemsData();
            for (int index = 0; index < items.Count; ++index)
            {
                items[index].Clear();
                GameObject.Destroy(items[index].gameObject);
            }

            items.Clear();
        }

        public float GetItemSize(int dataIndex)
        {
            float size = minItemSize;

            if (dynamicItemSize == true)
            {
                if (dataIndex < dataList.Count)
                {
                    float ItemSize = dataList[dataIndex].GetItemSize();
                    if (ItemSize == 0)
                    {
                        ItemSize = minItemSize;
                        dataList[dataIndex].SetItemSize(ItemSize);
                    }

                    return ItemSize;
                }
            }
            else
            {
                size = defaultItemSize;
            }

            return size;
        }

        private float GetItemTotalSize()
        {
            float itemTotalSize = GetItemSizeSum(GetItemCount());

            return itemTotalSize + padding * UPDOWN_MULTIPLY;
        }

        private InfiniteScrollItem PullItemByDataIndex(int dataIndex)
        {
            InfiniteScrollItem item = null;

            int itemIndex = GetItemIndexByDataIndex(dataIndex, true);
            if (itemIndex == -1)
            {
                item = CreateItem();
            }
            else
            {
                item = items[itemIndex];
            }

            return item;
        }

        private InfiniteScrollItem GetItemByDataIndex(int dataIndex)
        {
            int itemIndex = GetItemIndexByDataIndex(dataIndex, false);
            if (itemIndex != -1)
            {
                return items[itemIndex];
            }

            return null;
        }

        private int GetItemIndexByDataIndex(int dataIndex, bool findEmptyIndex = false)
        {
            int emptyIndex = -1;
            for (int index = 0; index < items.Count; ++index)
            {
                if (items[index].GetDataIndex() == dataIndex)
                {
                    return index;
                }

                if (findEmptyIndex == true)
                {
                    if (emptyIndex == -1 &&
                        items[index].IsActive() == false)
                    {
                        emptyIndex = index;
                    }
                }
            }

            return emptyIndex;
        }

        public bool IsDynamicItemSize()
        {
            return dynamicItemSize;
        }

        public void OnUpdateItemSize(InfiniteScrollData data, RectTransform itemTransform)
        {
            if (dynamicItemSize == true)
            {
                int dataIndex = GetDataIndex(data);

                if (IsValidDataIndex(dataIndex) == true)
                {
                    float size = GetItemSize(dataIndex);
                    if (dataIndex == 0)
                    {
                        SetItemPivot(size, itemTransform.pivot);
                    }

                    if (size > 0)
                    {
                        if (minItemSize == 0 ||
                            minItemSize > size)
                        {
                            minItemSize = size;
                        }
                    }

                    UpdateAllData(false);
                }
            }
        }

        private float GetItemSizeSum(int toIndex)
        {
            int itemCount = GetItemCount();
            if (toIndex >= itemCount)
            {
                toIndex = itemCount - 1;
            }

            float sizeSum = 0.0f;

            int lineCount = layout.GetLineCount(toIndex);
            if (dynamicItemSize == true)
            {
                for (int lineIdx = 0; lineIdx < lineCount; lineIdx++)
                {
                    sizeSum += layout.GetLineSize(lineIdx);
                }
            }
            else
            {
                sizeSum = defaultItemSize * lineCount;
            }

            if (lineCount > 0)
            {
                int spaceCount = lineCount;

                int maxLineCount = layout.GetLineCount();
                if (lineCount == maxLineCount)
                {
                    spaceCount--;
                }

                sizeSum = sizeSum + space * spaceCount;
            }

            return sizeSum;
        }

        private float GetItemSizeSumToIndex(int toIndex)
        {
            int itemCount = GetItemCount();
            if (toIndex >= itemCount)
            {
                toIndex = itemCount - 1;
            }

            float sizeSum = 0.0f;

            int lineCount = layout.GetLineCount(toIndex) - 1;
            if (dynamicItemSize == true)
            {
                for (int lineIdx = 0; lineIdx < lineCount; lineIdx++)
                {
                    sizeSum += layout.GetLineSize(lineIdx);
                }
            }
            else
            {
                sizeSum = defaultItemSize * lineCount;
            }

            if (lineCount > 0)
            {
                int spaceCount = lineCount;

                int maxLineCount = layout.GetLineCount();
                if (lineCount == maxLineCount)
                {
                    spaceCount--;
                }

                sizeSum = sizeSum + space * spaceCount;
            }

            return sizeSum;
        }
    }
}