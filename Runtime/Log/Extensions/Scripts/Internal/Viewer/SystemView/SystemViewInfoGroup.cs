using UnityEngine;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class SystemViewInfoGroup : MonoBehaviour
    {
        [SerializeField] private SystemViewInfo info = null;

        private List<SystemViewInfo> infoList = new List<SystemViewInfo>();

        public int infoCount = 0;

        public void Awake()
        {
            info.gameObject.SetActive(false);
        }

        public void ClearInfo()
        {
            for (int i = 0; i < infoList.Count; i++)
            {
                infoList[i].gameObject.SetActive(false);
            }

            infoCount = 0;
        }

        public void AddInfo(string heading, string detail)
        {
            SystemViewInfo addInfo = null;

            if (infoCount < infoList.Count)
            {
                addInfo = infoList[infoCount];
            }
            else
            {
                addInfo = GameObject.Instantiate<SystemViewInfo>(this.info, this.info.transform.parent);
                infoList.Add(addInfo);
            }

            addInfo.gameObject.SetActive(true);
            addInfo.Set(heading, detail);

            infoCount++;
        }
    }
}