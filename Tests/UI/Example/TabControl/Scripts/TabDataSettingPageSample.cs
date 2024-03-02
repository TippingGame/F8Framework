using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class TabDataSettingPageSample : MonoBehaviour
    {
        public Text text;

        public void DataSetting(Tab tab)
        {
            if (tab.IsSelected() == true)
            {
                TabDataSettingSampleData sampleData = (TabDataSettingSampleData)tab.GetData();

                text.text = sampleData.text;
            }
        }
    }
}