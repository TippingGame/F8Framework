using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class UICallbacks
    {
        public delegate void OnAddedEventDelegate(object[] parameters, int uiId);
        
        public OnAddedEventDelegate OnAdded;
        public OnAddedEventDelegate OnRemoved;
        public Action OnBeforeRemove;

        public UICallbacks(
            OnAddedEventDelegate onAdded = null,
            OnAddedEventDelegate onRemoved = null,
            Action onBeforeRemove = null)
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
        public int UIid;
        public string Guid;
        public string PrefabPath;
        public object[] Params;
        public UICallbacks Callbacks;
        public bool Valid;
        public GameObject Go;
        public DelegateComponent DelegateComponent;
        public BaseView BaseView;
    }
}