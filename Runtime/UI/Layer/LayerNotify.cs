using System;

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
            viewParams.Valid = true;
            
            Load(viewParams);
            return guid;
        }
        
        public new UILoader AddAsync(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;
            string guid = Guid.NewGuid().ToString(); // 生成一个唯一的ID
            ViewParams viewParams = GetOrCreateViewParams(prefabPath, guid);

            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = callbacks;
            viewParams.Valid = true;
            
            return LoadAsync(viewParams);
        }
        
        protected new ViewParams GetOrCreateViewParams(string prefabPath, string guid)
        {
            if (!uiViews.TryGetValue(prefabPath, out var viewParams))
            {
                if (!uiCache.TryGetValue(prefabPath, out viewParams))
                {
                    viewParams = new ViewParams();
                }
                viewParams.Guid = guid;
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
                if (isDestroy)
                {
                    RemoveCache(viewParams.PrefabPath);
                }
                else
                {
                    uiCache[viewParams.PrefabPath] = viewParams;
                }
                var comp = viewParams.DelegateComponent;
                comp.Remove(isDestroy);
                viewParams.Valid = false;
                return viewParams.UIid;
            }

            return default;
        }
    }
}