using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace F8Framework.Core
{
    public partial class InfiniteScroll : MonoBehaviour
    {
        protected bool                          isInitialize            = false;

        protected RectTransform                 content                 = null;

        private bool                            changeValue             = false;

        [Header("Event", order = 4)]
        public ChangeValueEvent                 onChangeValue           = new ChangeValueEvent();
        public ItemActiveEvent                  onChangeActiveItem      = new ItemActiveEvent();
        public StateChangeEvent                 onStartLine             = new StateChangeEvent();
        public StateChangeEvent                 onEndLine               = new StateChangeEvent();

        private Predicate<InfiniteScrollData>   onFilter                = null;

        private void Awake()
        {
            Initialize();
        }

        protected void Initialize()
        {
            if (isInitialize == false)
            {
                scrollRect = GetComponent<ScrollRect>();
                content = scrollRect.content;
                viewport = scrollRect.viewport;

                CheckScrollAxis();
                ClearScrollContent();

                RectTransform itemTransform = (RectTransform)itemPrefab.transform;
                defaultItemPrefabSize = itemTransform.sizeDelta;

                itemObjectList.Clear();
                dataList.Clear();

                scrollRect.onValueChanged.AddListener(OnValueChanged);

                CreateNeedItem();

                CheckScrollData();

                isInitialize = true;

                needReBuildLayout = true;
            }
        }

        public void InsertData(InfiniteScrollData data, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            AddData(data);

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

            UpdateAllData(immediately);
        }

        public void InsertData(InfiniteScrollData[] datas, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (datas == null || datas.Length == 0)
                return;

            // 第一步：批量创建DataContext并初始化
            int startIndex = dataList.Count;
            var newDataContexts = new DataContext[datas.Length];
            var validDataContexts = new List<DataContext>(); // 通过过滤的数据

            for (int i = 0; i < datas.Length; i++)
            {
                DataContext addData = new DataContext(datas[i], startIndex + i);
                InitFitContext(addData);
                newDataContexts[i] = addData;

                // 同时进行过滤检查
                if (onFilter == null || onFilter(addData.data) == false)
                {
                    validDataContexts.Add(addData);
                }
            }

            // 第二步：批量计算itemIndex
            int baseItemIndex = GetLastValidItemIndex();
            for (int i = 0; i < validDataContexts.Count; i++)
            {
                validDataContexts[i].itemIndex = baseItemIndex + i + 1;
            }

            // 第三步：批量更新后续数据的itemIndex（如果有的话）
            if (validDataContexts.Count > 0 && startIndex < dataList.Count)
            {
                int increment = validDataContexts.Count;
                for (int i = startIndex; i < dataList.Count; i++)
                {
                    if (dataList[i].itemIndex != -1)
                    {
                        dataList[i].itemIndex += increment;
                    }
                }
            }

            // 第四步：批量添加到主列表
            dataList.AddRange(newDataContexts);
            itemCount += validDataContexts.Count;

            // 第五步：标记需要重建布局（只标记一次）
            needReBuildLayout = true;

            // 第六步：更新显示
            UpdateAllData(immediately);
        }
        
        // 辅助方法：获取最后一个有效的itemIndex
        private int GetLastValidItemIndex()
        {
            if (itemCount == 0) return -1;
    
            for (int i = dataList.Count - 1; i >= 0; i--)
            {
                if (dataList[i].itemIndex != -1)
                {
                    return dataList[i].itemIndex;
                }
            }
            return -1;
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

            if (datas == null || datas.Length == 0)
                return;

            // 第一步：批量创建DataContext并初始化
            var newDataContexts = new DataContext[datas.Length];
            var validDataContexts = new List<DataContext>(); // 通过过滤的数据

            for (int i = 0; i < datas.Length; i++)
            {
                DataContext addData = new DataContext(datas[i], insertIndex + i);
                InitFitContext(addData);
                newDataContexts[i] = addData;

                // 同时进行过滤检查
                if (onFilter == null || onFilter(addData.data) == false)
                {
                    validDataContexts.Add(addData);
                }
            }

            // 第二步：更新插入位置之后所有数据的index
            if (insertIndex < dataList.Count)
            {
                for (int i = insertIndex; i < dataList.Count; i++)
                {
                    dataList[i].index += datas.Length;
                }
            }

            // 第三步：批量插入到主列表
            dataList.InsertRange(insertIndex, newDataContexts);

            // 第四步：批量计算itemIndex
            if (validDataContexts.Count > 0)
            {
                // 获取插入位置前一个有效的itemIndex
                int baseItemIndex = GetItemIndexBefore(insertIndex);
        
                // 更新插入数据的itemIndex
                for (int i = 0; i < validDataContexts.Count; i++)
                {
                    validDataContexts[i].itemIndex = baseItemIndex + i + 1;
                }

                // 更新插入位置之后所有有效数据的itemIndex
                int increment = validDataContexts.Count;
                int startUpdateIndex = insertIndex + newDataContexts.Length;
                for (int i = startUpdateIndex; i < dataList.Count; i++)
                {
                    if (dataList[i].itemIndex != -1)
                    {
                        dataList[i].itemIndex += increment;
                    }
                }

                itemCount += validDataContexts.Count;
            }

            // 第五步：标记需要重建布局（只标记一次）
            needReBuildLayout = true;

            // 第六步：更新显示
            UpdateAllData(immediately);
        }

        // 辅助方法：获取指定位置前一个有效的itemIndex
        private int GetItemIndexBefore(int targetIndex)
        {
            if (targetIndex == 0) return -1;
    
            for (int i = targetIndex - 1; i >= 0; i--)
            {
                if (dataList[i].itemIndex != -1)
                {
                    return dataList[i].itemIndex;
                }
            }
            return -1;
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
                selectDataIndex = -1;

                int removeShowIndex = -1;
                
                if(dataList[dataIndex].itemIndex != -1)
                {
                    removeShowIndex = dataList[dataIndex].itemIndex;
                }
                dataList[dataIndex].UnlinkItem(true);
                dataList.RemoveAt(dataIndex);
                for(int i= dataIndex; i< dataList.Count;i++)
                {
                    dataList[i].index--;

                    if(removeShowIndex != -1)
                    {
                        if (dataList[i].itemIndex != -1)
                        {
                            dataList[i].itemIndex--;
                        }
                    }
                }

                if (removeShowIndex != -1)
                {
                    if (removeShowIndex < firstItemIndex)
                    {
                        firstItemIndex--;
                    }
                    if (removeShowIndex < lastItemIndex)
                    {
                        lastItemIndex--;
                    }

                    itemCount--;
                }

                needReBuildLayout = true;

                UpdateAllData(immediately);
            }
        }

        public void ClearData(bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            itemCount = 0;
            selectDataIndex = -1;

            dataList.Clear();
            lineLayout.Clear();
            layoutSize = 0;
            lineCount = 0;

            ClearItemsData();

            lastItemIndex = 0;
            firstItemIndex = 0;

            showLineIndex = 0;
            showLineCount = 0;

            isStartLine = false;
            isEndLine = false;

            needUpdateItemList = true;
            needReBuildLayout = true;
            isUpdateArea = true;

            onFilter = null;

            ClearScrollContent();

            cachedData.Clear();

            UpdateAllData(immediately);
        }

        public void Clear()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            itemCount = 0;
            selectDataIndex = -1;
            dataList.Clear();
            lineLayout.Clear();
            layoutSize = 0;
            lineCount = 0;

            ClearItems();

            lastItemIndex = 0;
            firstItemIndex = 0;

            showLineIndex = 0;
            showLineCount = 0;

            isStartLine = false;
            isEndLine = false;

            needUpdateItemList = true;
            needReBuildLayout = true;
            isUpdateArea = true;

            onFilter = null;

            cachedData.Clear();

            ClearScrollContent();
        }

        public void UpdateData(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            var context = GetDataContext(data);
            if (context != null)
            {
                context.UpdateData(data);

                needReBuildLayout = true;
            }
        }

        public void UpdateAllData(bool immediately = true)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            needReBuildLayout = true;
            isUpdateArea = true;

            CreateNeedItem();

            if (immediately == true)
            {
                UpdateShowItem(true);
            }
        }

        public void SetFilter(Predicate<InfiniteScrollData> onFilter)
        {
            this.onFilter = onFilter;
            needUpdateItemList = true;
        }

        public float GetViewportSize()
        {
            return layout.GetMainSize(viewport);
        }

        public float GetContentSize()
        {
            UpdateContentSize();

            return layout.GetMainSize(content);
        }

        public float GetContentPosition()
        {
            return layout.GetAxisPosition(content);
        }

        public void ResizeScrollView()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            UpdateContentSize();
        }
        
        public float GetItemPosition(int itemIndex)
        {
            float distance = GetItemDistance(itemIndex);

            return -layout.GetAxisPostionFromOffset(distance);
        }

        public void RefreshScroll()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (needUpdateItemList == true)
            {
                BuildItemList();

                needUpdateItemList = false;
            }
            if (NeedUpdateItem() == true)
            {
                UpdateShowItem();
            }
        }

        protected float GetCrossSize()
        {
            return layout.GetCrossSize(content.rect);
        }

        protected void ResizeContent()
        {
            cachedData.contentSize = GetItemTotalSize();
            content.sizeDelta = layout.GetAxisVector(-layout.padding, cachedData.contentSize);
        }

        protected void UpdateContentSize()
        {
            if (needReBuildLayout == true)
            {
                BuildLayout();
                needReBuildLayout = false;
            }
        }

        protected bool NeedUpdateItem()
        {
            CheckScrollData();

            if (needReBuildLayout == true ||
                isRebuildLayout == true ||
                isUpdateArea == true)
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

        private void Update()
        {
            if (isInitialize == true)
            {
                RefreshScroll();
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

        [Serializable]
        public class ItemActiveEvent : UnityEvent<int, bool>
        {
            public ItemActiveEvent()
            {
            }
        }

        [Serializable]
        public class StateChangeEvent : UnityEvent<bool>
        {
            public StateChangeEvent()
            {
            }
        }
    }
}