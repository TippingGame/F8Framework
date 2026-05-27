using System;
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

        public void Init(int sortOrder = 0, string sortingLayerName = "Default", RenderMode renderMode = RenderMode.ScreenSpaceOverlay, bool pixelPerfect = false, UnityEngine.Camera camera = null)
        {
            _canvas.renderMode = renderMode;
            _canvas.worldCamera = camera;
            _canvas.sortingOrder = sortOrder;
            _canvas.sortingLayerName = sortingLayerName;
            _canvas.pixelPerfect = pixelPerfect;
        }
        
        public void SetCanvasScaler(CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPixelSize,
            float scaleFactor = 1f,
            float referencePixelsPerUnit = 100f)
        {
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.scaleFactor = scaleFactor;
            _canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        }
        
        public void SetCanvasScaler(CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize,
            Vector2? referenceResolution = null,
            CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight,
            float matchWidthOrHeight = 0f,
            float referencePixelsPerUnit = 100f)
        {
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.referenceResolution = referenceResolution ?? new Vector2(800, 600);;
            _canvasScaler.screenMatchMode = screenMatchMode;
            _canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
            _canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        }
        
        public void SetCanvasScaler(CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize,
            CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points,
            float fallbackScreenDPI = 96f,
            float defaultSpriteDPI = 96f,
            float referencePixelsPerUnit = 100f)
        {
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.physicalUnit = physicalUnit;
            _canvasScaler.fallbackScreenDPI = fallbackScreenDPI;
            _canvasScaler.defaultSpriteDPI = defaultSpriteDPI;
            _canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        }

        public string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;

            if (IsDuplicateLoad(prefabPath, out var viewParams))
            {
                return string.Empty;
            }

            viewParams = GetOrCreateViewParams(prefabPath);

            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = callbacks;
            viewParams.State = ViewState.None;
            viewParams.DestroyOnClose = false;
            viewParams.UnloadAllLoadedObjectsOnCancel = false;

            Load(viewParams);

            return viewParams.Guid;
        }

        public UILoader AddAsync(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;

            if (IsDuplicateLoad(prefabPath, out var viewParams))
            {
                return viewParams.UILoader;
            }

            viewParams = GetOrCreateViewParams(prefabPath);

            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = callbacks;
            viewParams.State = viewParams.Go == null && viewParams.DelegateComponent == null ? ViewState.Loading : ViewState.None;
            viewParams.DestroyOnClose = false;
            viewParams.UnloadAllLoadedObjectsOnCancel = false;

            return LoadAsync(viewParams);
        }

        protected bool IsDuplicateLoad(string prefabPath, out ViewParams viewParams)
        {
            if (uiViews.TryGetValue(prefabPath, out viewParams) && viewParams.State != ViewState.None)
            {
                LogF8.LogView($"UI重复加载：{prefabPath}");
                return true;
            }
            return false;
        }

        protected ViewParams GetOrCreateViewParams(string prefabPath, string guid = null)
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
                    viewParams.Guid = guid ?? Guid.NewGuid().ToString();
                }
                viewParams.PrefabPath = prefabPath;
                uiViews.Add(viewParams.PrefabPath, viewParams);
            }
            return viewParams;
        }

        protected void Load(ViewParams viewParams)
        {
            viewParams.State = ViewState.None;
            PrepareUILoader(viewParams);

            if (viewParams != null && viewParams.Go != null)
            {
                CreateNode(viewParams);
                viewParams.UILoader.UILoadSuccess();
            }
            else
            {
                GameObject res = UIManager.Instance.LoadUIPrefab(viewParams.PrefabPath);
                GameObject childNode = Instantiate(res, gameObject.transform, false);
                childNode.name = viewParams.PrefabPath;
                viewParams.Go = childNode;
                
                DelegateComponent comp = childNode.AddComponent<DelegateComponent>();
                viewParams.DelegateComponent = comp;
                viewParams.BaseView = childNode.GetComponent<BaseView>();
                comp.ViewParams = viewParams;
                
                CreateNode(viewParams);
                viewParams.UILoader.UILoadSuccess();
            }
        }

        protected UILoader LoadAsync(ViewParams viewParams)
        {
            PrepareUILoader(viewParams);

            if (viewParams != null && viewParams.Go != null)
            {
                viewParams.State = ViewState.None;
                CreateNode(viewParams);
                viewParams.UILoader.UILoadSuccess();
                return viewParams.UILoader;
            }
            else
            {
                viewParams.State = ViewState.Loading;
                int loadVersion = ++viewParams.LoadVersion;
                UILoader uiLoader = viewParams.UILoader;
                UIManager.Instance.LoadUIPrefabAsync(viewParams.PrefabPath, (res) =>
                {
                    if (!viewParams.Loading || loadVersion != viewParams.LoadVersion || this == null)
                    {
                        if (loadVersion == viewParams.LoadVersion)
                        {
                            viewParams.State = ViewState.None;
                        }
                        UIManager.Instance.UnloadUIPrefab(viewParams.PrefabPath, viewParams.UnloadAllLoadedObjectsOnCancel);
                        uiLoader.UILoadSuccess();
                        return;
                    }

                    if (res == null)
                    {
                        uiViews.Remove(viewParams.PrefabPath);
                        viewParams.State = ViewState.None;
                        uiLoader.UILoadSuccess();
                        return;
                    }

                    viewParams.State = ViewState.None;
                    GameObject childNode = Instantiate(res, gameObject.transform, false);
                    childNode.name = viewParams.PrefabPath;
                    viewParams.Go = childNode;
                
                    DelegateComponent comp = childNode.AddComponent<DelegateComponent>();
                    viewParams.DelegateComponent = comp;
                    viewParams.BaseView = childNode.GetComponent<BaseView>();
                    comp.ViewParams = viewParams;
                    CreateNode(viewParams);
                    uiLoader.UILoadSuccess();
                });
                
                return viewParams.UILoader;
            }
        }

        protected void PrepareUILoader(ViewParams viewParams)
        {
            if (viewParams.UILoader == null || viewParams.UILoader.LoaderSuccess)
            {
                viewParams.UILoader = new UILoader();
            }

            viewParams.UILoader.Guid = viewParams.Guid;
        }
        
        public UILoader CreateNode(ViewParams viewParams)
        {
            UIManager.Instance.CurrentUIs.RemoveAll(value => value == viewParams);
            UIManager.Instance.CurrentUIs.Add(viewParams);
            
            viewParams.State = ViewState.Active;

            var comp = viewParams.DelegateComponent;
            viewParams.Go.transform.SetParent(gameObject.transform, false);
            if (viewParams.Go.activeSelf == false)
            {
                viewParams.Go.SetActive(true);
            }
            
            comp.Add();
            
            return viewParams.UILoader;
        }

        public void Close(string prefabPath, bool isDestroy)
        {
            if (isDestroy)
            {
                RemoveCache(prefabPath);
            }

            if (!uiViews.TryGetValue(prefabPath, out var viewParams))
            {
                return;
            }

            if (IsPendingLoad(viewParams))
            {
                return;
            }

            if (CancelPendingLoad(viewParams, isDestroy))
            {
                uiViews.Remove(viewParams.PrefabPath);
                return;
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

        protected void RemoveCache(string prefabPath)
        {
            if (uiCache.TryGetValue(prefabPath, out var viewParams))
            {
                uiViews.Remove(viewParams.PrefabPath);
                uiCache.Remove(prefabPath);
                DestroyCachedView(viewParams);
            }
        }

        protected void DestroyCachedView(ViewParams viewParams)
        {
            if (viewParams == null)
            {
                return;
            }

            viewParams.State = ViewState.None;

            var childNode = viewParams.Go;
            if (childNode != null)
            {
                Destroy(childNode);
            }

            UIManager.Instance.UnloadUIPrefab(viewParams.PrefabPath, true);
        }

        protected bool CancelPendingLoad(ViewParams viewParams, bool unloadAllLoadedObjects)
        {
            if (!IsPendingLoad(viewParams))
            {
                return false;
            }

            viewParams.State = ViewState.None;
            viewParams.LoadVersion++;
            viewParams.UnloadAllLoadedObjectsOnCancel = unloadAllLoadedObjects;
            return true;
        }

        protected bool IsPendingLoad(ViewParams viewParams)
        {
            return viewParams != null && viewParams.Loading && viewParams.Go == null && viewParams.DelegateComponent == null;
        }

        protected void RemoveView(ViewParams viewParams, DelegateComponent comp, bool isDestroy)
        {
            if (comp.Remove(isDestroy, OnViewRemoved))
            {
                return;
            }

            viewParams.State = ViewState.None;
        }

        protected virtual void OnViewRemoved(ViewParams viewParams)
        {
            if (viewParams == null)
            {
                return;
            }

            RemoveViewRecord(viewParams);

            if (viewParams.DestroyOnClose)
            {
                return;
            }

            CacheView(viewParams);
        }

        protected virtual void RemoveViewRecord(ViewParams viewParams)
        {
            uiViews.Remove(viewParams.PrefabPath);
        }

        protected virtual void CacheView(ViewParams viewParams)
        {
            uiCache[viewParams.PrefabPath] = viewParams;
        }

        public GameObject GetByGuid(string guid)
        {
            var children = GetChildrens();
            foreach (var comp in children)
            {
                if (comp.ViewParams != null && comp.ViewParams.Guid == guid)
                {
                    return comp.gameObject;
                }
            }

            return null;
        }

        public List<GameObject> GetByUIid(int uiid)
        {
            List<GameObject> nodeList = null;
            var children = GetChildrens();
    
            foreach (var comp in children)
            {
                if (comp.ViewParams != null && comp.ViewParams.UIid == uiid)
                {
                    nodeList ??= new List<GameObject>();
                    nodeList.Add(comp.gameObject);
                }
            }

            return nodeList;
        }

        public bool Has(string prefabPathOrGuid)
        {
            var children = GetChildrens();
            foreach (var comp in children)
            {
                if (comp.ViewParams.Guid == prefabPathOrGuid || comp.ViewParams.PrefabPath == prefabPathOrGuid)
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
                if (comp != null && comp.isActiveAndEnabled && comp.ViewParams is { Valid: true })
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
                    uiViews.Remove(value.PrefabPath);
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
    }
}
