using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace F8Framework.Core
{
    public class TabLinkObject : MonoBehaviour
    {
        [Header("Tab Setting", order = 1)]
        [SerializeField]
        internal TabRoot root;

        public TabGroup GetGroup()
        {
            return root.GetGroup();
        }

        internal bool SetGroup(TabGroup value, bool change = true)
        {
            TabGroup group = GetGroup();
            if (group != value )
            {
                if (group != null)
                {
                    if (change == true)
                    {
                        if (group.IsLinked(this) == true)
                        {
                            group.Release(this);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                root.SetGroup(value);
            }

            return true;
        }

        internal bool SetGroup(object group, bool change = true)
        {
            if (group is TabGroup)
            {
                return SetGroup(group as TabGroup, change);
            }
            return false;
        }

        public bool IsValidGroup()
        {
            TabGroup group = GetGroup();
            if (group != null)
            {
                if (group.IsLinked(this) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsSameGroup(TabLinkObject linkObject)
        {
            return GetGroup() == linkObject.GetGroup();
        }

        public void UpdateValidGroup()
        {
            if(IsValidGroup() == false)
            {
                root.SetGroup(null);
            }
        }

        public void ReleaseGroup()
        {
            TabGroup group = GetGroup();
            if (group != null)
            {
                group.Release(this);
                root.SetGroup(null);
            }
        }

        public bool UnLink()
        {
            TabGroup group = GetGroup();
            if (group != null)
            {
                if(group.GetLinkCount(this) <= 1)
                {
                    ReleaseGroup();
                    return true;
                }
            }

            return true;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public bool GetActive()
        {
            return gameObject.activeSelf;
        }
    }
}
