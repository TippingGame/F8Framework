using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class LayerUI : MonoBehaviour
    {
        protected Dictionary<string, ViewParams> uiViews = new Dictionary<string, ViewParams>();
        protected Dictionary<string, ViewParams> uiCache = new Dictionary<string, ViewParams>();
        
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;
        public CanvasScaler CanvasScaler => _canvasScaler;
        private GraphicRaycaster _graphicRaycaster;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        
        private void Awake()
        {
            // 获取组件
            _canvas = gameObject.AddComponent<Canvas>();
            _canvasScaler = gameObject.AddComponent<CanvasScaler>();
            _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        public void Init(int sortOrder, RenderMode renderMode = RenderMode.ScreenSpaceOverlay)
        {
            _canvas.sortingOrder = sortOrder;
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
        public string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;
            var uuid = prefabPath; // 暂时和prefabPath相同
            if (uiViews.TryGetValue(uuid, out var viewParams) && viewParams.Valid)
            {
                LogF8.LogView($"UI重复加载：{prefabPath}");
                return string.Empty;
            }

            if (!uiViews.TryGetValue(uuid, out viewParams))
            {
                if (!uiCache.TryGetValue(prefabPath, out viewParams))
                {
                    viewParams = new ViewParams();
                    viewParams.Uuid = uuid;
                    viewParams.PrefabPath = prefabPath;
                    uiViews.Add(viewParams.Uuid, viewParams);
                }
                else
                {
                    viewParams.Uuid = uuid;
                    viewParams.PrefabPath = prefabPath;
                    uiViews.Add(viewParams.Uuid, viewParams);
                }
            }

            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = new UICallbacks();
            viewParams.Valid = true;

            Load(viewParams);

            return uuid;
        }

        protected void Load(ViewParams viewParams)
        {
            var vp = uiCache.GetValueOrDefault(viewParams.PrefabPath);
            if (vp != null && vp.Go != null)
            {
                CreateNode(vp);
            }
            else
            {
                AssetManager.Instance.LoadAsync<GameObject>(viewParams.PrefabPath, (res) =>
                {
                    AssetManager.Instance.Unload(viewParams.PrefabPath, false);
                    
                    GameObject childNode = Instantiate(res);
                    childNode.name = viewParams.Uuid;
                    viewParams.Go = childNode;
                
                    DelegateComponent comp = childNode.AddComponent<DelegateComponent>();
                    viewParams.DelegateComponent = comp;
                    viewParams.BaseView = childNode.GetComponent<BaseView>();
                    comp.ViewParams = viewParams;
                
                    CreateNode(viewParams);
                });
            }
        }

        public void CreateNode(ViewParams viewParams)
        {
            if (viewParams.Uuid != null)
            {
                UIManager.Instance.GetCurrentUuids().Add(viewParams.Uuid);
            }
            
            viewParams.Valid = true;

            var comp = viewParams.DelegateComponent;
            comp.Add();
            viewParams.Go.transform.SetParent(gameObject.transform, false);
            viewParams.Go.transform.localPosition = Vector3.zero;
            viewParams.Go.transform.localRotation = Quaternion.identity;
            if (viewParams.Go.activeSelf == false)
            {
                viewParams.Go.SetActive(true);
            }
        }

        public void Close(string prefabPath, bool isDestroy)
        {
            if (isDestroy)
            {
                RemoveCache(prefabPath);
            }
            
            var children = GetChildrens();
            foreach (var comp in children)
            {
                var viewParams = comp.ViewParams;
                if (viewParams.PrefabPath == prefabPath)
                {
                    uiViews.Remove(viewParams.Uuid);
                    if (!isDestroy)
                    {
                        uiCache[viewParams.PrefabPath] = viewParams;
                    }
                    comp.Remove(isDestroy);
                    viewParams.Valid = false;
                }
            }
        }

        protected void RemoveByUuid(string uuid, bool isDestroy)
        {
            if (uiViews.TryGetValue(uuid, out var viewParams))
            {
                if (isDestroy)
                {
                    uiViews.Remove(viewParams.Uuid);
                }
                
                var comp = viewParams.DelegateComponent;
                comp.Remove(isDestroy);
            }
        }

        protected void RemoveCache(string prefabPath)
        {
            if (uiCache.TryGetValue(prefabPath, out var viewParams))
            {
                uiViews.Remove(viewParams.Uuid);
                uiCache.Remove(prefabPath);
                var childNode = viewParams.Go;
                Destroy(childNode);
            }
        }

        public GameObject GetByUuid(string uuid)
        {
            var children = GetChildrens();
            foreach (var comp in children)
            {
                if (comp.ViewParams != null && comp.ViewParams.Uuid == uuid)
                {
                    return comp.gameObject;
                }
            }

            return null;
        }

        public List<GameObject> Get(string prefabPath)
        {
            var nodeList = new List<GameObject>();
            var children = GetChildrens();
            foreach (var comp in children)
            {
                if (comp.ViewParams.PrefabPath == prefabPath)
                {
                    nodeList.Add(comp.gameObject);
                }
            }

            return nodeList;
        }

        public bool Has(string prefabPathOrUuid)
        {
            var children = GetChildrens();
            foreach (var comp in children)
            {
                if (comp.ViewParams.Uuid == prefabPathOrUuid || comp.ViewParams.PrefabPath == prefabPathOrUuid)
                {
                    return true;
                }
            }

            return false;
        }
        
        protected List<DelegateComponent> GetChildrens()
        {
            var result = new List<DelegateComponent>();
            var children = gameObject.transform.childCount;
            for (var i = 0; i < children; i++)
            {
                var comp = gameObject.transform.GetChild(i).GetComponent<DelegateComponent>();
                if (comp != null && comp.ViewParams != null && comp.ViewParams.Valid && comp.isActiveAndEnabled)
                {
                    result.Add(comp);
                }
            }

            return result;
        }

        public void Clear(bool isDestroy)
        {
            if (isDestroy)
            {
                foreach (var value in uiViews.Values)
                {
                    var comp = value.DelegateComponent;
                    comp.Remove(true);
                    value.Valid = false;
                }
                
                foreach (var value in uiCache.Values)
                {
                    var childNode = value.Go;
                    Destroy(childNode);
                }
                uiCache.Clear();
            }
            else
            {
                foreach (var value in uiViews.Values)
                {
                    uiCache[value.PrefabPath] = value;
                    var comp = value.DelegateComponent;
                    comp.Remove(false);
                    value.Valid = false;
                }
            }
            
            uiViews.Clear();
        }
    }
}
