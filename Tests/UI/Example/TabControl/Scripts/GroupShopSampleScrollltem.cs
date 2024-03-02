using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace F8Framework.Tests
{
    public class GroupShopSampleScrollItemData : InfiniteScrollData
    {
        public string name;
        public List<ShopSampleScrollItemData> dataList;
    }

    public class GroupShopSampleScrollltem : InfiniteScrollItem
    {
        public Text groupName = null;

        public ShopSampleScrollItem itemPrefab;

        public RectTransform parent;

        public LayoutGroup layoutGroup;
        private List<ShopSampleScrollItem> itemList = new List<ShopSampleScrollItem>();

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            GroupShopSampleScrollItemData itemData = scrollData as GroupShopSampleScrollItemData;

            groupName.text = itemData.name;

            for (int index = 0; index < itemData.dataList.Count; index++)
            {
                ShopSampleScrollItem item;
                if (index < itemList.Count)
                {
                    item = itemList[index];
                }
                else
                {
                    item = GameObject.Instantiate<ShopSampleScrollItem>(itemPrefab, parent);
                    itemList.Add(item);
                }

                item.UpdateData(itemData.dataList[index]);
                item.gameObject.SetActive(true);
            }

            for (int index = itemData.dataList.Count; index < itemList.Count; index++)
            {
                itemList[index].gameObject.SetActive(false);
            }

            onUpdate(parent);
        }

        public void onUpdate(RectTransform rect)
        {
            if (rect.sizeDelta == Vector2.zero)
            {
                return;
            }

            RectTransform rectTransform = (RectTransform)transform;
            SetSize(new Vector2(rect.sizeDelta.x + 8, rect.sizeDelta.y - rect.anchoredPosition.y + 8));
        }
    }
}