using System;
using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class TabDataSettingSampleData : ITabData
    {
        public TabDataSettingSampleData(int level)
        {
            openLevel = level;
            name = level.ToString();
            text = "openLevel : " + level;
        }

        public string name;

        public int openLevel;

        public string text;
    }

    public class TabDataSettingSample : MonoBehaviour
    {
        public TabController tabController;

        public Text levelText;
        public Slider levelSlider;
        public int level = 20;

        public SampleMessageBox messageBox;

        private void Awake()
        {
            tabController.GetTab(0).SetData(new TabDataSettingSampleData(1));
            tabController.GetTab(1).SetData(new TabDataSettingSampleData(8));
            tabController.GetTab(2).SetData(new TabDataSettingSampleData(17));
            tabController.GetTab(3).SetData(new TabDataSettingSampleData(33));
            tabController.GetTab(4).SetData(new TabDataSettingSampleData(53));

            SetLevel(level);
        }

        public void SetLevel(int value)
        {
            level = value;
            levelText.text = "Level : " + level.ToString();

            for (int index = 0; index < tabController.GetTabCount(); index++)
            {
                Tab tab = tabController.GetTab(index);
                TabDataSettingSampleData data = (TabDataSettingSampleData)tab.GetData();
                if (data != null)
                {
                    tab.SetBlockTab(level < data.openLevel);
                }
            }
        }

        public void OnChangeLevel(float rate)
        {
            level = (int)(rate);
            SetLevel(level);
        }

        public void OnSelected(Tab selectedTab)
        {
            TabDataSettingSampleData data = (TabDataSettingSampleData)selectedTab.GetData();
            if (data != null)
            {
                LogF8.Log("Selected : " + data.name);
            }
            
        }

        public void OnBlock(Tab blockTab)
        {
            TabDataSettingSampleData data = (TabDataSettingSampleData)blockTab.GetData();
            if (data != null)
            {
                messageBox.Show("Openable Laevel : " + data.openLevel);
            }
        }
    }
}