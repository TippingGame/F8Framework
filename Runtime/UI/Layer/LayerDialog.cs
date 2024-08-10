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
            public string guid;
        }

        private Dictionary<int, Queue<DialogParam>> dialogParams = new Dictionary<int, Queue<DialogParam>>();
        
        public new string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            if (!dialogParams.TryGetValue(uiId, out Queue<DialogParam> dialogQueue))
            {
                dialogQueue = new Queue<DialogParam>();
                dialogParams[uiId] = dialogQueue;
            }
            string guid = Guid.NewGuid().ToString(); // 生成一个唯一的ID
            dialogQueue.Enqueue(new DialogParam
            {
                config = config,
                parameters = parameters,
                callbacks = callbacks,
                guid = guid,
            });
            if (dialogQueue.Count > 1)
            {
                return guid;
            }
            else
            {
                return Show(uiId, dialogQueue.Peek());
            }
        }

        private string Show(int uiId, DialogParam firstElement)
        {
            string prefabPath = firstElement.config.AssetName;
            ViewParams viewParams;
            
            if (!uiViews.TryGetValue(prefabPath, out viewParams))
            {
                if (!uiCache.TryGetValue(prefabPath, out viewParams))
                {
                    viewParams = new ViewParams();
                    viewParams.Guid = firstElement.guid;
                    viewParams.PrefabPath = prefabPath;
                    uiViews.Add(viewParams.PrefabPath, viewParams);
                }
                else
                {
                    viewParams.Guid = firstElement.guid;
                    viewParams.PrefabPath = prefabPath;
                    uiViews.Add(viewParams.PrefabPath, viewParams);
                }
            }

            viewParams.UIid = uiId;
            viewParams.Valid = true;

            viewParams.Callbacks = firstElement.callbacks ?? new UICallbacks();
            UICallbacks.OnAddedEventDelegate onRemoveSource = viewParams.Callbacks.OnRemoved;
            
            viewParams.Callbacks.OnRemoved = (param, id) =>
            {
                onRemoveSource?.Invoke(param, id);
                StartCoroutine(DelayedNext(id));
            };
            
            viewParams.Params = firstElement.parameters;
            Load(viewParams);

            return viewParams.Guid;
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
                    Show(id, nextParam);
                }
            }
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
