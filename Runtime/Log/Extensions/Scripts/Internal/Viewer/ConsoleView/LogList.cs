using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class LogList : MonoBehaviour
    {
        public GameObject toBottomButton = null;

        private LogItemData selectItemData = null;
        private Action<Log.LogData> selectCallback = null;
        private InfiniteScroll infiniteScroll = null;
        private List<LogItemData> logItemDatas = new List<LogItemData>();

        private void Awake()
        {
            toBottomButton.SetActive(false);
            infiniteScroll = GetComponent<InfiniteScroll>();
            infiniteScroll.AddSelectCallback(OnSelect);
        }

        private void Update()
        {
            if (infiniteScroll.IsMoveToLastData() == false)
            {
                SetToBottomButtonEnable(true);
            }
            else
            {
                SetToBottomButtonEnable(false);
            }
        }

        public void Resize()
        {
            infiniteScroll.ResizeScrollView();
        }

        public void AddSelectCallback(Action<Log.LogData> callback)
        {
            selectCallback = callback;
        }

        public void RemoveSelectCallback(Action<Log.LogData> callback)
        {
            selectCallback -= callback;
        }

        public void ShowPlayTime(bool show)
        {
            for (int index = 0; index < logItemDatas.Count; ++index)
            {
                logItemDatas[index].showPlayTime = show;
            }

            infiniteScroll.UpdateAllData();
        }

        public void ShowSceneName(bool show)
        {
            for (int index = 0; index < logItemDatas.Count; ++index)
            {
                logItemDatas[index].showSceneName = show;
            }

            infiniteScroll.UpdateAllData();
        }

        public void ClearList()
        {
            logItemDatas.Clear();
            infiniteScroll.Clear();
        }

        public void Insert(Log.LogData data, bool showPlayTime, bool showSceneName)
        {
            LogItemData itemData = new LogItemData()
            {
                logData = data,
                index = logItemDatas.Count,
                showPlayTime = showPlayTime,
                showSceneName = showSceneName,
                isSelect = false
            };

            logItemDatas.Add(itemData);

            bool isMoveToLastData = infiniteScroll.IsMoveToLastData();

            infiniteScroll.InsertData(itemData);

            if (isMoveToLastData == true)
            {
                infiniteScroll.MoveToLastData();
            }
        }

        private void OnSelect(InfiniteScrollData scrollData)
        {
            LogItemData itemData = (LogItemData)scrollData;
            if (selectItemData == itemData)
            {
                return;
            }

            if (selectItemData != null)
            {
                selectItemData.isSelect = false;
                infiniteScroll.UpdateData(selectItemData);
            }

            selectItemData = itemData;
            selectCallback(selectItemData.logData);
        }

        public void MoveToBottom()
        {
            infiniteScroll.MoveToLastData();
        }

        private void SetToBottomButtonEnable(bool enable)
        {
            toBottomButton.SetActive(enable);
        }
    }
}