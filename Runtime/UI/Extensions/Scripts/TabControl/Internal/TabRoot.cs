using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace F8Framework.Core
{
    [Serializable]
    public struct TabRoot
    {
        [NonSerialized]
        private TabGroup group;

        public TabGroup GetGroup()
        {
            if (group != null)
            {
                TabController controller = group.GetController();
                if (controller != null &&
                    controller.tabGroup != group)
                {
                    group = controller.tabGroup;
                }
                return group;
            }

            return null;
        }

        public void SetGroup(TabGroup value)
        {
            group = value;
        }
    }
}
