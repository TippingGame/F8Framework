using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class LayerNotify : LayerUI
    {
        public new string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;
            string guid = Guid.NewGuid().ToString(); // 生成一个唯一的ID
            ViewParams viewParams = GetOrCreateViewParams(prefabPath, guid);
            
            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = callbacks;
            viewParams.State = ViewState.None;
            viewParams.DestroyOnClose = false;
            viewParams.UnloadAllLoadedObjectsOnCancel = false;
            
            Load(viewParams);
            return viewParams.Guid;
        }
        
        public new UILoader AddAsync(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;
            string guid = Guid.NewGuid().ToString(); // 生成一个唯一的ID
            ViewParams viewParams = GetOrCreateViewParams(prefabPath, guid);

            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = callbacks;
            viewParams.State = viewParams.Go == null && viewParams.DelegateComponent == null ? ViewState.Loading : ViewState.None;
            viewParams.DestroyOnClose = false;
            viewParams.UnloadAllLoadedObjectsOnCancel = false;
            
            return LoadAsync(viewParams);
        }
        
        protected new ViewParams GetOrCreateViewParams(string prefabPath, string guid)
        {
            if (!uiViews.TryGetValue(guid, out var viewParams))
            {
                string lastKey = "";
                using (var enumerator = uiCache.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Value.PrefabPath == prefabPath)
                        {
                            lastKey = enumerator.Current.Key;
                        }
                    }
                }
                if (uiCache.TryGetValue(lastKey, out ViewParams value))
                {
                    viewParams = value;
                    uiCache.Remove(lastKey);
                }
                else
                {
                    viewParams = new ViewParams();
                    viewParams.Guid = guid;
                }
                viewParams.PrefabPath = prefabPath;
                uiViews.Add(viewParams.Guid, viewParams);
            }
            return viewParams;
        }
        
        public string Show(int uiId, UIConfig config, string content, UICallbacks callbacks = null)
        {
            return this.Add(uiId, config, new object[] { content }, callbacks);
        }
        
        public UILoader ShowAsync(int uiId, UIConfig config, string content, UICallbacks callbacks = null)
        {
            return this.AddAsync(uiId, config, new object[] { content }, callbacks);
        }
        
        public int CloseByGuid(string guid, bool isDestroy)
        {
            if (uiViews.TryGetValue(guid, out var viewParams))
            {
                if (IsPendingLoad(viewParams))
                {
                    return 0;
                }

                if (CancelPendingLoad(viewParams, isDestroy))
                {
                    uiViews.Remove(guid);
                    return viewParams.UIid;
                }

                var comp = viewParams.DelegateComponent;
                if (comp != null)
                {
                    RemoveView(viewParams, comp, isDestroy);
                }
                else
                {
                    viewParams.State = ViewState.None;
                }
                return viewParams.UIid;
            }

            if (isDestroy && uiCache.TryGetValue(guid, out var cachedViewParams))
            {
                int uiId = cachedViewParams.UIid;
                RemoveCache(guid);
                return uiId;
            }

            return 0;
        }
        
        protected new void RemoveCache(string guid)
        {
            if (uiCache.TryGetValue(guid, out var viewParams))
            {
                uiViews.Remove(guid);
                uiCache.Remove(guid);
                DestroyCachedView(viewParams);
            }
        }
        
        public new void Close(string prefabPath, bool isDestroy)
        {
            if (isDestroy)
            {
                RemoveCachesByPrefab(prefabPath);
            }

            var values = new List<ViewParams>();
            foreach (var viewParams in uiViews.Values)
            {
                if (viewParams.PrefabPath == prefabPath)
                {
                    values.Add(viewParams);
                }
            }

            foreach (var viewParams in values)
            {
                if (IsPendingLoad(viewParams))
                {
                    continue;
                }

                if (CancelPendingLoad(viewParams, isDestroy))
                {
                    uiViews.Remove(viewParams.Guid);
                    continue;
                }

                var comp = viewParams.DelegateComponent;
                if (comp != null)
                {
                    RemoveView(viewParams, comp, isDestroy);
                }
                else
                {
                    viewParams.State = ViewState.None;
                }
            }
        }

        private void RemoveCachesByPrefab(string prefabPath)
        {
            var keys = new List<string>();
            foreach (var pair in uiCache)
            {
                if (pair.Value.PrefabPath == prefabPath)
                {
                    keys.Add(pair.Key);
                }
            }

            foreach (var key in keys)
            {
                RemoveCache(key);
            }
        }
        
        public new void Clear(bool isDestroy)
        {
            if (isDestroy)
            {
                foreach (var value in uiCache.Values)
                {
                    DestroyCachedView(value);
                }
                uiCache.Clear();
            }
            
            var values = new List<ViewParams>(uiViews.Values);
            uiViews.Clear();

            foreach (var value in values)
            {
                if (CancelPendingLoad(value, isDestroy))
                {
                    uiViews.Remove(value.Guid);
                    continue;
                }

                var comp = value.DelegateComponent;
                if (comp != null)
                {
                    RemoveView(value, comp, isDestroy);
                }
                else
                {
                    value.State = ViewState.None;
                }
            }
    
        }

        protected override void RemoveViewRecord(ViewParams viewParams)
        {
            uiViews.Remove(viewParams.Guid);
        }

        protected override void CacheView(ViewParams viewParams)
        {
            uiCache[viewParams.Guid] = viewParams;
        }
    }
}
