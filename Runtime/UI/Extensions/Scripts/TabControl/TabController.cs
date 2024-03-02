using System;
using UnityEngine;
using UnityEngine.Events;

namespace F8Framework.Core
{
    
    [Serializable]
    public class TabController : MonoBehaviour
    {
        private bool initialized = false;

        [Header("Link", order = 0)]
        public TabGroup tabGroup = new TabGroup();

        [Header("Selected", order = 0)]
        [SerializeField]
        private Tab selectedTab;

        [Header("Event", order = 2)]
        public TabEvent onSelected = new TabEvent();
        public TabEvent onBlocked = new TabEvent();

        protected virtual void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized == false)
            {
                tabGroup.Initialize(this);

                Select(selectedTab);
            }

            initialized = true;
        }

        public int GetSelectedIndex()
        {
            if( selectedTab != null &&
                selectedTab.GetActive() == true)
            {
                for (int i = 0; i < GetTabCount(); i++)
                {
                    Tab tab = GetTab(i);
                    if (tab == selectedTab)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public Tab GetSelectedTab()
        {
            if (selectedTab != null &&
                selectedTab.GetActive() == true)
            {
                return selectedTab;
            }

            return null;
        }

        public TabPage GetSelectedPage()
        {
            Tab selectedTab = GetSelectedTab();
            if (selectedTab != null)
            {
                return selectedTab.GetLinkedPage();
            }

            return null;
        }

        public int GetTabCount()
        {
            return tabGroup.list.Count;
        }

        public Tab GetTab(int index)
        {
            if (index < GetTabCount())
            {
                return tabGroup.list[index].GetTab();
            }

            return null;
        }

        public int GetTabIndex(Tab tab)
        {
            for (int i = 0; i < GetTabCount(); i++)
            {
                if(tab == GetTab(i))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Contain(Tab tab)
        {
            return GetTabIndex(tab) != -1;
        }

        public void AddTab(Tab tab, TabPage page)
        {
            if (Contain(tab) == false)
            {
                tab.SetValue(false);

                tabGroup.Add(tab, page);
            }
        }

        public void SelectFirstTab()
        {
            for (int i = 0; i < GetTabCount(); i++)
            {
                Tab tab = GetTab(i);
                if(tab != null)
                {
                    if (tab.GetActive() == true &&
                        tab.IsBlock() == false)
                    {
                        tab.OnClick();
                        break;
                    }
                }
            }
        }

        public void Select(int index = 0)
        {
            if (index < GetTabCount())
            {
                Tab tab = GetTab(index);
                if(tab != null)
                {
                    tab.OnClick();
                }
            }
        }

        public void Select(Tab selectTab)
        {
            if (selectTab != null)
            {
                if (selectTab.GetActive() == false)
                {
                    selectTab = null;
                }
                else if (selectTab.IsBlock() == true)
                {
                    if(onBlocked != null)
                    {
                        onBlocked.Invoke(selectTab);
                    }
                    selectTab.OnBlocked();
                    return;
                }
                
            }

            Tab firstSelectTab = null;
            for (int i = 0; i < GetTabCount(); i++)
            {
                bool isSelected = false;
                Tab tab = GetTab(i);
                if (tab != null)
                {
                    isSelected = tab == selectTab;
                    tab.SetValue(isSelected);

                    if (isSelected == true &&
                        firstSelectTab == null)
                    {
                        firstSelectTab = selectTab;
                    }
                    else
                    {
                        tab.NotifyPage();
                    }
                }
            }

            selectedTab = firstSelectTab;
            if (selectedTab != null)
            {
                if (onSelected != null)
                {
                    onSelected.Invoke(selectTab);
                }
                selectedTab.NotifyPage();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            initialized = false;
            Initialize();
        }
#endif

        [Serializable]
        public class TabEvent : UnityEvent<Tab>
        {
            public TabEvent()
            {

            }
        }
    }
}