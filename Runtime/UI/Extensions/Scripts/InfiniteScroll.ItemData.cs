using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DataContext = F8Framework.Core.InfiniteScrollItem.DataContext;

namespace F8Framework.Core
{
    public partial class InfiniteScroll
    {
        protected List<DataContext> dataList = new List<DataContext>();

        protected int selectDataIndex = -1;
        protected Action<InfiniteScrollData> selectCallback = null;

        public int GetDataIndex(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            return dataList.FindIndex((context) => { return context.data.Equals(data); });
        }

        public int GetDataCount()
        {
            return dataList.Count;
        }


        public InfiniteScrollData GetData(int index)
        {
            DataContext context = GetDataContext(index);
            if (context != null)
            {
                return context.data;
            }
            else
            {
                return null;
            }
        }

        internal DataContext GetDataContext(int index)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (IsValidDataIndex(index) == true)
            {
                return dataList[index];
            }
            else
            {
                return null;
            }
        }

        internal DataContext GetDataContext(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            return dataList.Find((context) => { return context.data.Equals(data); });
        }

        protected void AddData(InfiniteScrollData data)
        {
            dataList.Add(new DataContext(data, dataList.Count));
        }

        protected void InsertData(InfiniteScrollData data, int insertIndex)
        {
            if (insertIndex < 0 || insertIndex > dataList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (insertIndex < dataList.Count)
            {
                for (int dataIndex = insertIndex; dataIndex < dataList.Count; dataIndex++)
                {
                    dataList[dataIndex].index++;
                }

                dataList.Insert(insertIndex, new DataContext(data, insertIndex));
            }
            else
            {
                dataList.Add(new DataContext(data, dataList.Count));
            }
        }

        protected bool IsValidDataIndex(int index)
        {
            return (index >= 0 && index < dataList.Count) ? true : false;
        }

        public float GetItemSize(Vector2 sizeDelta)
        {
            float itemSize = 0.0f;
            if (IsVersical() == true)
            {
                itemSize = sizeDelta.y;
            }
            else
            {
                itemSize = sizeDelta.x;
            }

            return itemSize;
        }

        public void AddSelectCallback(Action<InfiniteScrollData> callback)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectCallback += callback;
        }

        public void RemoveSelectCallback(Action<InfiniteScrollData> callback)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectCallback -= callback;
        }

        private void OnSelectItem(InfiniteScrollData data)
        {
            int dataIndex = GetDataIndex(data);
            if (IsValidDataIndex(dataIndex) == true)
            {
                selectDataIndex = dataIndex;

                if (selectCallback != null)
                {
                    selectCallback(data);
                }
            }
        }

        public void OnChangeActiveItem(int dataIndex, bool active)
        {
            onChangeActiveItem.Invoke(dataIndex, active);
        }

        [Serializable]
        public class ItemActiveEvent : UnityEvent<int, bool>
        {
            public ItemActiveEvent()
            {
            }
        }
    }
}