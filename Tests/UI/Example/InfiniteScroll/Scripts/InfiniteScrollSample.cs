using F8Framework.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class InfiniteScrollSample : MonoBehaviour
    {
        public InfiniteScroll verticalScrollList = null;
        public InfiniteScroll horizontalScrollList = null;
        public Text logText = null;
        public Text dataCount = null;
        public ScrollRect logScrollRect = null;
        public Dropdown moveDataSelect = null;
        public Dropdown moveDataTypeSelect = null;
        public InputField moveDataTime = null;

        private int index = 0;
        private int insertCount = 0;
        private int removeCount = 0;

        private List<TestItemData> dataList = new List<TestItemData>();
        private StringBuilder log = new StringBuilder();

        private void Start()
        {
            verticalScrollList.AddSelectCallback((data) =>
            {
                AddLog(string.Format("vertical select data : {0}", ((TestItemData)data).index.ToString()));
            });

            horizontalScrollList.AddSelectCallback((data) =>
            {
                AddLog(string.Format("horizontal select data : {0}", ((TestItemData)data).index.ToString()));
            });

            moveDataSelect.onValueChanged.AddListener((option) => { MoveTo(option); });
        }

        public void IsMoveToLastData()
        {
            AddLog(string.Format("Is move to last data vertical:{0},horizontal:{1}",
                verticalScrollList.IsMoveToLastData().ToString(), horizontalScrollList.IsMoveToLastData().ToString()));
        }

        public void InsertData()
        {
            TestItemData data = new TestItemData();
            data.index = index++;
            dataList.Add(data);

            verticalScrollList.InsertData(data);
            horizontalScrollList.InsertData(data);

            var options = new List<Dropdown.OptionData>() { new Dropdown.OptionData(index.ToString()) };
            moveDataSelect.AddOptions(options);

            ++insertCount;
            UpdateDataCount();

            AddLog(string.Format("Insert Data : {0}", index - 1));
        }

        public void Clear()
        {
            verticalScrollList.Clear();
            horizontalScrollList.Clear();

            removeCount += dataList.Count;
            dataList.Clear();

            moveDataSelect.ClearOptions();

            UpdateDataCount();

            AddLog("Clear Data");
        }

        public void Remove()
        {
            if (dataList.Count <= 0)
            {
                return;
            }

            int dataIndex = GetDataIndexByRandom();
            TestItemData data = dataList[dataIndex];
            verticalScrollList.RemoveData(data);
            horizontalScrollList.RemoveData(data);

            dataList.Remove(data);

            int optionIndex = moveDataSelect.options.FindIndex(p => p.text.Equals(data.index.ToString()));
            if (optionIndex != -1)
            {
                moveDataSelect.options.RemoveAt(optionIndex);
            }

            removeCount++;
            UpdateDataCount();

            AddLog(string.Format("Remove data : {0}", data.index));
        }

        public void UpdateData()
        {
            if (dataList.Count <= 0)
            {
                return;
            }

            int dataIndex = GetDataIndexByRandom();

            TestItemData data = dataList[dataIndex];
            data.description = string.Format("Updated : {0}", DateTime.Now.ToString("T"));

            verticalScrollList.UpdateData(data);
            horizontalScrollList.UpdateData(data);

            AddLog(string.Format("Update data : {0}", data.index));
        }

        public void UpdateAllData()
        {
            for (int index = 0; index < dataList.Count; ++index)
            {
                dataList[index].description = string.Format("Updated : {0}", DateTime.Now.ToString("T"));
            }

            verticalScrollList.UpdateAllData();
            horizontalScrollList.UpdateAllData();

            AddLog("Update all data");
        }

        private void UpdateDataCount()
        {
            dataCount.text = string.Format("Data Count : {0}  (insert[{1}] remove[{2}])", dataList.Count, insertCount,
                removeCount);
        }

        public void ResizeScrollVeiw()
        {
            verticalScrollList.ResizeScrollView();
            horizontalScrollList.ResizeScrollView();

            AddLog("Resize Scroll View");
        }

        public void MoveToFirstData()
        {
            verticalScrollList.MoveToFirstData();
            horizontalScrollList.MoveToFirstData();

            AddLog("Move to first data");
        }

        public void MoveToLastData()
        {
            verticalScrollList.MoveToLastData();
            horizontalScrollList.MoveToLastData();

            AddLog("Move to last data");
        }

        private void MoveTo(int index)
        {
            float time = 0;
            if (float.TryParse(moveDataTime.text, out time) == false)
            {
                AddLog("Time is not Number");
            }

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                verticalScrollList.MoveTo(index, (InfiniteScroll.MoveToType)moveDataTypeSelect.value, time);
                horizontalScrollList.MoveTo(index, (InfiniteScroll.MoveToType)moveDataTypeSelect.value, time);
            }
            else
            {
                TestItemData data = dataList[index];

                verticalScrollList.MoveTo(data, (InfiniteScroll.MoveToType)moveDataTypeSelect.value, time);
                horizontalScrollList.MoveTo(data, (InfiniteScroll.MoveToType)moveDataTypeSelect.value, time);
            }

            AddLog(string.Format("Move to {0}", index));
        }

        private void AddLog(string text)
        {
            log.AppendLine(text);
            logText.text = log.ToString();
            logScrollRect.verticalNormalizedPosition = 0.0f;
        }

        private int GetDataIndexByRandom()
        {
            return UnityEngine.Random.Range(0, dataList.Count);
        }
    }
}