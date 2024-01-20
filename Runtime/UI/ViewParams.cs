using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class UICallbacks
    {
        public delegate void NodeEventDelegate(object[] parameters, string id);

        public NodeEventDelegate OnAdded;
        public NodeEventDelegate OnRemoved;
        public Action<Action> OnBeforeRemove;

        public UICallbacks(
            NodeEventDelegate onAdded = null,
            NodeEventDelegate onRemoved = null,
            Action<Action> onBeforeRemove = null)
        {
            OnAdded = onAdded;
            OnRemoved = onRemoved;
            OnBeforeRemove = onBeforeRemove;
        }
    }

    /// <summary>
    /// 本类型仅供内部使用，请勿在功能逻辑中使用。
    /// </summary>
    public class ViewParams
    {
        public string Uuid;
        public string PrefabPath;
        public object[] Params;
        public UICallbacks Callbacks;
        public bool Valid;
        public GameObject Go;
        public DelegateComponent DelegateComponent;
    }
}