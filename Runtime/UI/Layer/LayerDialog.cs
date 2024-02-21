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
        }

        private Dictionary<int, Queue<DialogParam>> dialogParams = new Dictionary<int, Queue<DialogParam>>();
        
        public new string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            string uuid = config.AssetName;
            if (!dialogParams.TryGetValue(uiId, out Queue<DialogParam> dialogQueue))
            {
                dialogQueue = new Queue<DialogParam>();
                dialogParams[uiId] = dialogQueue;
            }
            dialogQueue.Enqueue(new DialogParam
            {
                config = config,
                parameters = parameters,
                callbacks = callbacks
            });
            if (dialogQueue.Count > 1)
            {
                return uuid;
            }
            else
            {
                return Show(uiId, config, parameters, callbacks);
            }
        }

        private string Show(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            string prefabPath = config.AssetName;
            string uuid = prefabPath; // 暂时和prefabPath相同
            ViewParams viewParams = uiViews.GetValueOrDefault(uuid);
            if (viewParams == null)
            {
                viewParams = new ViewParams
                {
                    UIid = uiId,
                    Uuid = uuid,
                    PrefabPath = prefabPath,
                    Valid = true
                };

                uiViews[viewParams.Uuid] = viewParams;
            }

            viewParams.Callbacks = callbacks ?? new UICallbacks();
            UICallbacks.OnAddedEventDelegate onRemoveSource = viewParams.Callbacks.OnRemoved;
            
            viewParams.Callbacks.OnRemoved = (param, id) =>
            {
                onRemoveSource?.Invoke(param, id);
                StartCoroutine(DelayedNext(id));
            };
            
            viewParams.Params = parameters;
            Load(viewParams);

            return uuid;
        }
        private IEnumerator DelayedNext(int id)
        {
            // 延迟一帧
            yield return null;
            Next(id);
        }
        private void Next(int id)
        {
            if (dialogParams[id] != null && dialogParams[id].Count > 0)
            {
                dialogParams[id].Dequeue();
                if (dialogParams[id].Count > 0)
                {
                    DialogParam nextParam = dialogParams[id].Peek();
                    Show(id, nextParam.config, nextParam.parameters, nextParam.callbacks);
                }
            }
        }
        
        public void Close(int uiId, string prefabPath, bool isDestroy)
        {
            if (isDestroy)
            {
                dialogParams.TryGetValue(uiId, out Queue<DialogParam> dialogQueue);
                if (dialogQueue is { Count: > 0 })
                {
                    dialogQueue.Dequeue();
                }
            }
            
            base.Close(prefabPath, isDestroy);
        }
        
        public new void Clear(bool isDestroy)
        {
            foreach (var key in dialogParams.Keys)
            {
                dialogParams[key].Clear();
            }
            
            base.Clear(isDestroy);
        }
    }
}
