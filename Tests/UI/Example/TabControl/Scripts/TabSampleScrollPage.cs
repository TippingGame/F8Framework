using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class TabSampleScrollPage : MonoBehaviour
    {
        public InfiniteScroll scroll;

        public void OnNotify(Tab select, bool selected)
        {
            if (selected == true)
            {
                CategoryGroupData data = (CategoryGroupData)select.GetData();

                scroll.ClearData();
                for (int i = 0; i < data.category.Count; i++)
                {
                    ShopSampleScrollItemData itemData = new ShopSampleScrollItemData();
                    itemData.name = "Item " + i;
                    itemData.description = "desc " + i;

                    itemData.buttonEnabled = true;
                    itemData.buttonText = i.ToString();

                    scroll.InsertData(itemData);
                }

                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}