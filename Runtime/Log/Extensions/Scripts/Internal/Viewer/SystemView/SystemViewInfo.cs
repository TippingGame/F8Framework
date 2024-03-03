using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class SystemViewInfo : MonoBehaviour
    {
        public Text infoHeading = null;
        public Text infoDetail = null;

        public void Set(string heading, string detail)
        {
            infoHeading.text = heading;
            infoDetail.text = detail;
        }
    }
}