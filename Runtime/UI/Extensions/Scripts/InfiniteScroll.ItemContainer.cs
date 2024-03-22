using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public partial class InfiniteScroll
    {
        [Header("Scroll Item", order = 2)]
        public int needItemCount = 0;

        public InfiniteScrollItem itemPrefab = null;

        public bool dynamicItemSize = false;

        private const float NEED_MORE_ITEM_RATE = 2;

        private Vector2 defaultItemPrefabSize = Vector2.zero;

        private List<InfiniteScrollItem> itemObjectList = new List<InfiniteScrollItem>();

        public float GetItemSize(int itemIndex)
        {
            float size = 0;

            if (dynamicItemSize == true)
            {
                if (itemIndex < itemCount)
                {
                    DataContext context = GetContextFromItem(itemIndex);
                    if (context != null)
                    {
                        size = context.GetItemSize();
                    }
                }
            }
            else
            {
                size = layout.GetMainSize(defaultItemPrefabSize);
            }

            return size;
        }
        public bool IsDynamicItemSize()
        {
            return dynamicItemSize;
        }

        private InfiniteScrollItem CreateItem()
        {
            InfiniteScrollItem itemObject = Instantiate(itemPrefab, content, false);

            itemObject.Initalize(this, itemObjectList.Count);
            itemObject.SetActive(false, false);

            itemObject.SetAxis(cachedData.anchorMin, cachedData.anchorMax, cachedData.itemPivot);

            itemObject.AddSelectCallback(OnSelectItem);
            
            RectTransform itemTransform = itemObject.rectTransform;
            itemTransform.sizeDelta = layout.GetAxisVector(layout.GetMainSize(itemTransform.sizeDelta));

            itemObjectList.Add(itemObject);

            

            return itemObject;
        }

        private float GetNeedSize()
        {
            return layout.GetMainSize(viewport) * NEED_MORE_ITEM_RATE;
        }

        private void CreateNeedItem()
        {
            for (int itemNumber = itemObjectList.Count; itemNumber < needItemCount; itemNumber++)
            {
                CreateItem();
            }
        }

        private void ClearItemsData()
        {
            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                itemObjectList[index].ClearData(false);
            }
        }

        private void ClearItems()
        {
            ClearItemsData();
            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                itemObjectList[index].Clear();
                GameObject.Destroy(itemObjectList[index].gameObject);
            }
            itemObjectList.Clear();
        }

        private InfiniteScrollItem PullItem(DataContext context)
        {
            InfiniteScrollItem item = context.itemObject;

            if( item == null || 
                item.GetDataIndex() != context.index)
            {
                context.itemObject = null;
                int itemObjectIndex = GetItemIndexFromDataIndex(context.index, true);
                if (itemObjectIndex == -1)
                {
                    item = CreateItem();
                }
                else
                {
                    item = itemObjectList[itemObjectIndex];
                }
            }

            return item;
        }

        private int GetItemIndexFromDataIndex(int dataIndex, bool findEmptyIndex = false)
        {
            int emptyIndex = -1;
            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                if (itemObjectList[index].GetDataIndex() == dataIndex)
                {
                    return index;
                }

                if (findEmptyIndex == true)
                {
                    if (emptyIndex == -1 &&
                        itemObjectList[index].IsActive() == false )
                    {
                        emptyIndex = index;
                    }
                }
            }

            return emptyIndex;
        }
        
        internal void OnUpdateItemSize(DataContext context)
        {
            if (dynamicItemSize == true)
            {
                if(context.itemObject != null)
                {
                    UpdateAllData(false);
                }

                needReBuildLayout = true;
            }
        }
    }
}