using System;
using UnityEngine;
using DataContext = F8Framework.Core.InfiniteScroll.DataContext;

namespace F8Framework.Core
{
    public class InfiniteScrollData
    {
    }

    public class InfiniteScrollItem : MonoBehaviour
    {
        public bool autoApplySize = false;

        protected RectTransform cachedRectTransform = null;

        protected bool activeItem;
        protected InfiniteScroll scroll = null;

        public RectTransform rectTransform
        {
            get
            {
                if (System.Object.ReferenceEquals(cachedRectTransform, null) == true)
                {
                    cachedRectTransform = transform as RectTransform;
                }

                return cachedRectTransform;
            }
        }

        protected InfiniteScrollData scrollData
        {
            get
            {
                if (dataContext != null)
                {
                    return dataContext.data;
                }
                else
                {
                    return null;
                }
            }
        }
        protected DataContext dataContext = null;
        protected Action<InfiniteScrollData> selectCallback = null;
        protected Action<InfiniteScrollData, RectTransform> updateSizeCallback = null;

        protected int itemObjectIndex = -1;

        internal bool needUpdateItemSize = true;
        
        public void Initalize(InfiniteScroll scroll, int itemObjectIndex)
        {
            this.scroll = scroll;
            this.itemObjectIndex = itemObjectIndex;
            this.needUpdateItemSize = true;
        }
        
        public int GetItemIndex()
        {
            if (dataContext != null)
            {
                return dataContext.itemIndex;
            }
            else
            {
                return -1;
            }
        }

        public int GetDataIndex()
        {
            if (dataContext != null)
            {
                return dataContext.index;
            }
            else
            {
                return -1;
            }
        }

        public bool IsActive()
        {
            return activeItem;
        }

        public void AddSelectCallback(Action<InfiniteScrollData> callback)
        {
            selectCallback += callback;
        }

        public void RemoveSelectCallback(Action<InfiniteScrollData> callback)
        {
            selectCallback -= callback;
        }

        public virtual void UpdateData(InfiniteScrollData scrollData)
        {
        }

        internal void SetAxis(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            bool autoApplySize = this.autoApplySize;

            this.autoApplySize = false;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;

            needUpdateItemSize = true;

            this.autoApplySize = autoApplySize;
        }

        protected void OnSelect()
        {
            if (selectCallback != null)
            {
                selectCallback(scrollData);
            }
        }

        public virtual void SetActive(bool active, bool notifyEvent = true)
        {
            activeItem = active;

            gameObject.SetActive(activeItem);

            if (notifyEvent == true)
            {
                if (scroll != null)
                {
                    scroll.OnChangeActiveItem(GetDataIndex(), activeItem);
                }
            }
        }

        public void SetSize(float itemSize, bool notity = true)
        {
            if (scrollData == null)
            {
                return;
            }

            if (scroll != null &&
                scroll.dynamicItemSize == false)
            {
                return;
            }

            if (dataContext != null &&
                dataContext.GetItemSize() != itemSize)
            {
                dataContext.SetItemSize(itemSize);

                if (notity == true)
                {
                    OnUpdateItemSize();
                }

                this.needUpdateItemSize = true;
            }
        }

        public void SetSize(Vector2 sizeDelta, bool notity = true)
        {
            if (scrollData == null)
            {
                return;
            }

            if (scroll != null &&
                scroll.dynamicItemSize == false)
            {
                return;
            }

            float itemSize = scroll.layout.GetMainSize(sizeDelta);
            SetSize(itemSize, notity);
        }

        internal void ClearData(bool notifyEvent = true)
        {
            SetActive(false, activeItem && notifyEvent);

            if (dataContext != null)
            {
                dataContext.itemObject = null;
                dataContext = null;
            }
        }

        internal void Clear()
        {
            ClearData(false);

            selectCallback = null;
            updateSizeCallback = null;
        }
        internal void UpdateItem(DataContext context)
        {
            this.dataContext = context;
            this.dataContext.itemObject = this;

            UpdateData(this.dataContext.data);

            this.dataContext.needUpdateItemData = false;

            this.needUpdateItemSize = true;
        }


        public void AddUpdateSizeCallback(Action<InfiniteScrollData, RectTransform> callback)
        {
            updateSizeCallback += callback;
        }

        public void RemoveUpdateSizeCallback(Action<InfiniteScrollData, RectTransform> callback)
        {
            updateSizeCallback -= callback;
        }

        protected void OnUpdateItemSize()
        {
            if (scroll != null &&
                scroll.dynamicItemSize == false)
            {
                return;
            }

            if (scroll != null)
            {
                scroll.OnUpdateItemSize(dataContext);
            }

            if (updateSizeCallback != null)
            {
                updateSizeCallback(scrollData, rectTransform);
            }
        }

        protected bool CanAutoSizeCheck()
        {
            if (autoApplySize == false ||
                activeItem == false)
            {
                return false;
            }

            if(scroll == null)
            {
                return false;
            }

            if( scroll.processing == true || 
                scroll.anchorUpdate == true)
            {
                return false;
            }

            if (rectTransform == null)
            {
                return false;
            }

            return true;
        }
        protected void OnRectTransformDimensionsChange()
        {
            if (CanAutoSizeCheck() == true)
            {
                SetSize(rectTransform.sizeDelta);
            }
        }
    }
}