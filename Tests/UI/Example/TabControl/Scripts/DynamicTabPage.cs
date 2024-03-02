using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;


namespace F8Framework.Tests
{
    public class DynamicTabPage : MonoBehaviour
    {
        public TabController tabController;

        public Tab tabOriginalPrefab;
        public Transform tabParent;

        public TabPage verticalScrollPage;
        public TabPage horizontalScrollPage;

        public void OnNotify(Tab tab)
        {
            if (tab.IsSelected() == true)
            {
                CategoryGroupData data = (CategoryGroupData)tab.GetData();
                SetData(data);

                tabController.SelectFirstTab();

                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void SetData(CategoryGroupData data)
        {
            for (int i = data.category.Count; i < tabController.GetTabCount(); i++)
            {
                Tab tab = tabController.GetTab(i);
                tab.SetActive(false);
                tab.NotifyPage();
            }

            for (int i = 0; i < data.category.Count; i++)
            {
                Tab tab = null;
                TabPage page = null;

                if (data.category[i].isVertical == true)
                {
                    page = verticalScrollPage;
                }
                else
                {
                    page = horizontalScrollPage;
                }

                if (i < tabController.GetTabCount())
                {
                    tab = tabController.GetTab(i);
                    tab.SetLinkPage(page);
                }
                else
                {
                    tab = Instantiate<Tab>(tabOriginalPrefab, tabParent);
                    tabController.AddTab(tab, page);
                }

                tab.SetData(data.category[i]);
                tab.SetActive(true);
            }
        }

        public void UpdatePage()
        {
            Tab selectedTab = tabController.GetSelectedTab();
            if(selectedTab != null)
            {
                selectedTab.NotifyPage();

                if (selectedTab.GetActive() == false)
                {
                    tabController.SelectFirstTab();
                }
            }
            else
            {
                tabController.SelectFirstTab();
            }
        }

        public ShopSampleScrollPage GetSelectedScrollPage()
        {
            TabPage selectedPage = tabController.GetSelectedPage();
            if (selectedPage != null)
            {
                return selectedPage.GetComponent<ShopSampleScrollPage>();
            }


            return null;
        }
    }
}