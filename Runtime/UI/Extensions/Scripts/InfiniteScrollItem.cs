using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class InfiniteScrollData
    {
    }

    public class InfiniteScrollItem : MonoBehaviour
    {
        public class DataContext
        {
            public DataContext(InfiniteScrollData data, int index)
            {
                this.index = index;
                this.data = data;
            }

            internal InfiniteScrollData data;
            internal int index = -1;

            internal float scrollItemSize = 0;
            internal bool updateItemSize = false;

            public float GetItemSize()
            {
                return scrollItemSize;
            }

            public void SetItemSize(float value)
            {
                scrollItemSize = value;
                updateItemSize = true;
            }
        }

        public bool autoApplySize = false;

        protected RectTransform rectTransform = null;

        protected bool activeItem;
        protected InfiniteScroll scroll = null;

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

        internal int itemIndex = -1;

        public void Initalize(InfiniteScroll scroll, int itemIndex)
        {
            this.scroll = scroll;
            this.itemIndex = itemIndex;

            this.rectTransform = transform as RectTransform;
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

        public bool IsUpdateItemSize()
        {
            if (dataContext != null)
            {
                return dataContext.updateItemSize;
            }
            else
            {
                return false;
            }
        }

        public void UpdatedItemSize()
        {
            if (dataContext != null)
            {
                dataContext.updateItemSize = false;
            }
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

        public void SetSize(Vector2 sizeDelta, bool notity = true)
        {
            if (scrollData == null)
            {
                return;
            }

            float itemSize = 0;
            if (scroll.IsVersical() == true)
            {
                itemSize = sizeDelta.y;
            }
            else
            {
                itemSize = sizeDelta.x;
            }

            if (dataContext.GetItemSize() != itemSize)
            {
                dataContext.SetItemSize(itemSize);

                if (notity == true)
                {
                    OnUpdateItemSize();
                }
            }
        }

        internal void SetData(int dataIndex, bool notifyEvent = true)
        {
            this.dataContext = scroll.GetDataContext(dataIndex);

            SetActive(true, notifyEvent);

            dataContext.updateItemSize = true;
        }

        internal void ClearData(bool notifyEvent = true)
        {
            SetActive(false, notifyEvent);

            this.dataContext = null;
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
            this.dataContext.updateItemSize = true;

            UpdateData(this.dataContext.data);
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
            if (scroll != null)
            {
                scroll.OnUpdateItemSize(scrollData, rectTransform);
            }

            if (updateSizeCallback != null)
            {
                updateSizeCallback(scrollData, rectTransform);
            }
        }

        protected void OnRectTransformDimensionsChange()
        {
            if (autoApplySize == true)
            {
                if (rectTransform != null)
                {
                    SetSize(rectTransform.sizeDelta);
                }
            }
        }
    }
}