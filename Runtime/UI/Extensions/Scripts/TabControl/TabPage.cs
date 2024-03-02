using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace F8Framework.Core
{
    public interface ITabPage
    {
        void Notify(Tab select);
    }

    public class TabPage : TabLinkObject, ITabPage
    {
        [Header("Option", order = 0)]
        public bool immediatelyActive = true;

        [Header("Event", order = 2)]
        public NotifyTabPageEvent onNotify = new NotifyTabPageEvent();

        public void Notify(Tab tab)
        {
            if (immediatelyActive == true)
            {
                gameObject.SetActive(tab.IsSelected());
            }

            onNotify.Invoke(tab);
        }

        [System.Serializable]
        public class NotifyTabPageEvent : UnityEvent<Tab>
        {
            public NotifyTabPageEvent()
            {

            }
        }
    }
}
