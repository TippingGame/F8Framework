using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class CustomTabSample : Tab
    {
        public Text titleText;

        public GameObject[] activeObjs;
        public GameObject[] deactiveObjs;

        public override void OnUpdateData(ITabData data)
        {
            base.OnUpdateData(data);

            CustumTabDataSample sampleData = (CustumTabDataSample)data;

            titleText.text = sampleData.name;
        }

        public override void OnChangeValue(bool active)
        {
            base.OnChangeValue(active);

            if (activeObjs != null)
            {
                for (int i = 0; i < activeObjs.Length; i++)
                {
                    activeObjs[i].SetActive(active == true);
                }
            }

            if (deactiveObjs != null)
            {
                for (int i = 0; i < deactiveObjs.Length; i++)
                {
                    deactiveObjs[i].SetActive(active == false);
                }
            }
        }
    }
}