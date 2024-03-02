using System;
using UnityEngine;

namespace F8Framework.Core
{
    [Serializable]
    public class TabLink
    {
        [NonSerialized]
        private TabGroup group;
        
        [SerializeField]
        internal Tab tab;

        [SerializeField]
        internal TabPage page;

        public TabLink(TabGroup group, Tab tab, TabPage page)
        {
            this.group = group;

            SetTab(tab);
            SetPage(page);
        }

        public Tab GetTab()
        {
            return tab;
        }

        public void SetTab(Tab value)
        {
            if (tab != null &&
                group != null)
            {
                if (value == null ||
                    group != value.GetGroup())
                {
                    group.Release(tab);
                }
            }

            tab = value;

            if (tab != null &&
                group != null)
            {
                if (tab.SetGroup(group, true) == false)
                {
                    tab = null;
                }
            }
        }

        public TabPage GetPage()
        {
            return page;
        }

        public void SetPage(TabPage value)
        {
            if (page != null &&
                page != value)
            {
                if (value == null ||
                    group != value.GetGroup())
                {
                    group.Release(page);
                }
            }

            page = value;

            if (page != null && 
                group != null)
            {
                if (page.SetGroup(group, true) == false)
                {
                    page = null;
                }
            }
        }

        public bool IsLinked()
        {
            return (tab != null || page != null);
        }

        public void UnLink()
        {
            if (tab != null)
            {
                tab.UnLink();
                tab = null;
            }

            if (page != null)
            {
                page.UnLink();
                page = null;
            }
        }

        internal void Link(TabGroup group, bool change = true)
        {
            this.group = group;
            if (tab != null)
            {
                if (tab.SetGroup(group, change) == false)
                {
                    tab = null;
                }
                
            }

            if (page != null)
            {
                if (page.SetGroup(group, change) == false)
                {
                    page = null;
                }
            }
        }
    }
}

