using F8Framework.Core;
using UnityEngine;
using System.Collections.Generic;

namespace F8Framework.Tests
{
    public class CustumTabDataSample : ITabData
    {
        public string name;
    }

    public class CategoryGroupData : CustumTabDataSample
    {
        public List<ItemGroupData> category = new List<ItemGroupData>();
    }


    public class ItemGroupData : CustumTabDataSample
    {
        public bool isVertical = true;
        public List<ItemData> itemList = new List<ItemData>();

        public void MakeRandomItem(int from, int to)
        {
            isVertical = Random.Range(0, 2) == 0;
            for (int i = 0; i < Random.Range(from, to); i++)
            {
                ItemData item = new ItemData();
                item.name = "Item " + i;
                item.description = "desc " + i;

                item.buttonText = i.ToString();

                itemList.Add(item);
            }
        }
    }

    public class ItemData
    {
        public string name;
        public string description;
        public string buttonText;
        public System.Action<int> buttonEvent;
    }


    public class TabSamplePage : MonoBehaviour
    {
        public TabController control;

        public List<ITabData> tapDataList = new List<ITabData>();


        private void Start()
        {
            tapDataList.Add(MakeTab0());
            tapDataList.Add(MakeTab1());
            tapDataList.Add(MakeTab2());
            tapDataList.Add(MakeTab3());
            tapDataList.Add(MakeTab4());

            for (int i = 0; i < tapDataList.Count; i++)
            {
                Tab tab = control.GetTab(i);
                tab.SetData(tapDataList[i]);
            }
        }

        private CategoryGroupData MakeTab0()
        {
            CategoryGroupData groupData = new CategoryGroupData();
            groupData.name = "dynamic";

            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                ItemGroupData category = new ItemGroupData();
                category.name = "Category " + i;
                category.MakeRandomItem(5, 60);

                groupData.category.Add(category);
            }

            return groupData;
        }


        private ItemGroupData MakeTab1()
        {
            ItemGroupData groupData = new ItemGroupData();
            groupData.name = "vertical";
            groupData.MakeRandomItem(5, 30);

            return groupData;
        }

        private ItemGroupData MakeTab2()
        {
            ItemGroupData groupData = new ItemGroupData();
            groupData.name = "horizontal";
            groupData.MakeRandomItem(5, 30);

            return groupData;
        }

        private CategoryGroupData MakeTab3()
        {
            CategoryGroupData groupData = new CategoryGroupData();
            groupData.name = "vertical(inside)";

            for (int i = 0; i < Random.Range(2, 8); i++)
            {
                ItemGroupData category = new ItemGroupData();
                category.name = "Category " + i;
                category.MakeRandomItem(2, 20);

                groupData.category.Add(category);
            }

            return groupData;
        }

        private CategoryGroupData MakeTab4()
        {
            CategoryGroupData groupData = new CategoryGroupData();
            groupData.name = "horizontal(inside)";

            for (int i = 0; i < Random.Range(2, 8); i++)
            {
                ItemGroupData category = new ItemGroupData();
                category.name = "Category " + i;
                category.MakeRandomItem(2, 20);

                groupData.category.Add(category);
            }

            return groupData;
        }
    }
}