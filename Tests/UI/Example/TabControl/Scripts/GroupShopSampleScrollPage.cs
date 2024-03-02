using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;


namespace F8Framework.Tests
{
    public class GroupShopSampleScrollPage : MonoBehaviour
    {
        public Dropdown search;
        public InfiniteScroll scroll;
        public void OnNotify(Tab tab)
        {
            if (tab.IsSelected() == true)
            {
                CategoryGroupData data = (CategoryGroupData)tab.GetData();

                List<string> catrgoryList = new List<string>();

                search.ClearOptions();
                scroll.ClearData();
                foreach(ItemGroupData category in data.category)
                {
                    string catrgoryName = category.name;

                    List<ShopSampleScrollItemData> dataList = new List<ShopSampleScrollItemData>();
                    for (int index = 0; index < category.itemList.Count; index++)
                    {
                        ItemData item = category.itemList[index];

                        ShopSampleScrollItemData itemData = new ShopSampleScrollItemData();
                        itemData.name = item.name;
                        itemData.description = item.description;

                        itemData.buttonEnabled = true;
                        itemData.buttonText = item.buttonText;
                        itemData.buttonEvent = item.buttonEvent;

                        dataList.Add(itemData);
                    }

                    GroupShopSampleScrollItemData groupData = new GroupShopSampleScrollItemData();
                    groupData.name = catrgoryName;
                    groupData.dataList = dataList;
                    scroll.InsertData(groupData);

                    catrgoryList.Add(catrgoryName);
                }

                search.AddOptions(catrgoryList);

                search.onValueChanged.AddListener((option) =>
                {
                    scroll.MoveTo(option, InfiniteScroll.MoveToType.MOVE_TO_TOP, time: 0.6f);
                });
            }
        }
    }
}