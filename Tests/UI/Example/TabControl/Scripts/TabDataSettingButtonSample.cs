using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class TabDataSettingButtonSample : MonoBehaviour
    {
        public Text title;

        public void DataSetting(ITabData data)
        {
            TabDataSettingSampleData sampleData = (TabDataSettingSampleData)data;

            title.text = sampleData.name;
        }
    }
}