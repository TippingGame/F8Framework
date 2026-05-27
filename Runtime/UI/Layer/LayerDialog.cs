using System;
using System.Collections;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class LayerDialog : LayerUI
    {
        private class DialogParam
        {
            public UIConfig config;
            public object[] parameters;
            public UICallbacks callbacks;
            public string requestId;
            public UILoader uiLoader;
        }

        private Dictionary<int, Queue<DialogParam>> dialogParams = new Dictionary<int, Queue<DialogParam>>();
        
        public new string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            if (!dialogParams.TryGetValue(uiId, out Queue<DialogParam> dialogQueue))
            {
                dialogQueue = new Queue<DialogParam>();
                dialogParams[uiId] = dialogQueue;
            }
            var dialogParam = new DialogParam
            {
                config = config,
                parameters = parameters,
                callbacks = callbacks,
                requestId = Guid.NewGuid().ToString(),
                uiLoader = null,
            };
            dialogQueue.Enqueue(dialogParam);
            if (dialogQueue.Count > 1)
            {
                return GetCurrentOrCachedGuid(config.AssetName);
            }
            else
            {
                return Show(uiId, dialogParam);
            }
        }

        public new UILoader AddAsync(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            if (!dialogParams.TryGetValue(uiId, out Queue<DialogParam> dialogQueue))
            {
                dialogQueue = new Queue<DialogParam>();
                dialogParams[uiId] = dialogQueue;
            }
            var dialogParam = new DialogParam
            {
                config = config,
                parameters = parameters,
                callbacks = callbacks,
                requestId = Guid.NewGuid().ToString(),
                uiLoader = new UILoader(),
            };
            dialogParam.uiLoader.Guid = GetCurrentOrCachedGuid(config.AssetName);
            dialogQueue.Enqueue(dialogParam);
            if (dialogQueue.Count > 1)
            {
                return dialogParam.uiLoader;
            }
            else
            {
                return ShowAsync(uiId, dialogParam);
            }
        }
        
        private string Show(int uiId, DialogParam firstElement)
        {
            string prefabPath = firstElement.config.AssetName;
            ViewParams viewParams = GetOrCreateViewParams(prefabPath);

            viewParams.UIid = uiId;
            viewParams.State = ViewState.None;
            viewParams.DestroyOnClose = false;
            viewParams.UnloadAllLoadedObjectsOnCancel = false;

            viewParams.Callbacks = CloneCallbacks(firstElement.callbacks);
            UICallbacks.OnAddedEventDelegate onRemoveSource = viewParams.Callbacks.OnRemoved;
            
            viewParams.Callbacks.OnRemoved = (param, id) =>
            {
                onRemoveSource?.Invoke(param, id);
                StartCoroutine(DelayedNext(id, firstElement.requestId));
            };
            
            viewParams.Params = firstElement.parameters;
            Load(viewParams);
            firstElement.uiLoader = viewParams.UILoader;
            return viewParams.Guid;
        }

        protected new ViewParams GetOrCreateViewParams(string prefabPath, string guid = null)
        {
            if (!uiViews.TryGetValue(prefabPath, out var viewParams))
            {
                if (uiCache.TryGetValue(prefabPath, out viewParams))
                {
                    uiCache.Remove(prefabPath);
                }
                else
                {
                    viewParams = new ViewParams();
                }

                viewParams.Guid ??= guid ?? Guid.NewGuid().ToString();
                viewParams.PrefabPath = prefabPath;
                uiViews.Add(viewParams.PrefabPath, viewParams);
            }
            return viewParams;
        }
        
        private UILoader ShowAsync(int uiId, DialogParam firstElement)
        {
            string prefabPath = firstElement.config.AssetName;
            ViewParams viewParams = GetOrCreateViewParams(prefabPath);

            viewParams.UIid = uiId;
            viewParams.State = viewParams.Go == null && viewParams.DelegateComponent == null ? ViewState.Loading : ViewState.None;
            viewParams.DestroyOnClose = false;
            viewParams.UnloadAllLoadedObjectsOnCancel = false;

            viewParams.Callbacks = CloneCallbacks(firstElement.callbacks);
            UICallbacks.OnAddedEventDelegate onRemoveSource = viewParams.Callbacks.OnRemoved;
            
            viewParams.Callbacks.OnRemoved = (param, id) =>
            {
                onRemoveSource?.Invoke(param, id);
                StartCoroutine(DelayedNext(id, firstElement.requestId));
            };
            
            viewParams.Params = firstElement.parameters;
            viewParams.UILoader = firstElement.uiLoader;
            PrepareUILoader(viewParams);
            firstElement.uiLoader = viewParams.UILoader;
            return LoadAsync(viewParams);
        }

        private string GetCurrentOrCachedGuid(string prefabPath)
        {
            if (uiViews.TryGetValue(prefabPath, out var viewParams))
            {
                return viewParams.Guid;
            }

            if (uiCache.TryGetValue(prefabPath, out viewParams))
            {
                return viewParams.Guid;
            }

            return null;
        }

        private UICallbacks CloneCallbacks(UICallbacks callbacks)
        {
            if (callbacks == null)
            {
                return new UICallbacks();
            }

            return new UICallbacks(callbacks.OnAdded, callbacks.OnRemoved, callbacks.OnBeforeRemove);
        }
        
        private IEnumerator DelayedNext(int id, string requestId)
        {
            // 延迟一帧
            yield return null;
            Next(id, requestId);
        }
        
        private void Next(int id, string requestId)
        {
            if (dialogParams.TryGetValue(id, out var dialogQueue) && dialogQueue.Count > 0)
            {
                if (dialogQueue.Peek().requestId != requestId)
                {
                    return;
                }

                dialogQueue.Dequeue();
                if (dialogQueue.Count > 0)
                {
                    DialogParam nextParam = dialogQueue.Peek();
                    if (nextParam.uiLoader == null)
                    {
                        Show(id, nextParam);
                    }
                    else
                    {
                        ShowAsync(id, nextParam);
                    }
                }
            }
        }
        
        public new void Clear(bool isDestroy)
        {
            foreach (var dialogQueue in dialogParams.Values)
            {
                foreach (var dialogParam in dialogQueue)
                {
                    dialogParam.uiLoader?.UILoadSuccess();
                }

                dialogQueue.Clear();
            }
            
            base.Clear(isDestroy);
        }
    }
}
