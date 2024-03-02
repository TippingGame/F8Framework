using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace F8Framework.Tests
{
    public class DynamicTabSample : MonoBehaviour
    {
        public InputField categoryName;

        public InputField itemName;
        public InputField itemDescript;

        public Text selectIndexText;
        public int selectIndex;

        public CategoryGroupData data = new CategoryGroupData();

        public DynamicTabPage sample;

        private void Start()
        {
            sample.SetData(data);
        }

        public void CreateCategory()
        {
            ItemGroupData add = new ItemGroupData();
            add.name = categoryName.text;

            data.category.Add(add);

            sample.SetData(data);

            sample.UpdatePage();
        }

        public void RemoveCategory()
        {
            int index = sample.tabController.GetSelectedIndex();
            if (index >= 0 && index < data.category.Count)
            {
                data.category.RemoveAt(index);
            }

            sample.SetData(data);

            sample.UpdatePage();
        }

        public ItemGroupData GetCurrentCategoryData()
        {
            int index = sample.tabController.GetSelectedIndex();
            if (index >= 0 && index < data.category.Count)
            {
                return data.category[index];
            }

            return null;
        }


        public void AddItem()
        {
            ItemGroupData categoryData = GetCurrentCategoryData();
            if (categoryData != null)
            {
                ItemData addItem = new ItemData();
                addItem.name = itemName.text;
                addItem.description = itemDescript.text;
                addItem.buttonText = "select";
                addItem.buttonEvent = SelectItemIndex;

                categoryData.itemList.Add(addItem);

                ShopSampleScrollPage page = sample.GetSelectedScrollPage();
                if (page != null)
                {
                    page.AddData(addItem);
                }
            }
        }

        public void RemoveItem()
        {
            if (selectIndex != -1)
            {
                ItemGroupData categoryData = GetCurrentCategoryData();
                if (categoryData != null)
                {
                    if (selectIndex < categoryData.itemList.Count)
                    {
                        categoryData.itemList.RemoveAt(selectIndex);
                    }

                    ShopSampleScrollPage page = sample.GetSelectedScrollPage();
                    if (page != null)
                    {
                        page.RemoveData(selectIndex);
                    }

                    SelectItemIndex(-1);
                }
            }
        }

        public void SelectItemIndex(int index)
        {
            selectIndex = index;
            if (selectIndex == -1)
            {
                selectIndexText.text = "None Selected";
            }
            else
            {
                selectIndexText.text = "Selected Index : " + selectIndex;
            }
        }
    }
}