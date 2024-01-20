using System.Collections.Generic;
using F8Framework.AssetMap;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 资产捆绑加载程序。
    /// 管理资产的加载、扩展和卸载行为。
    /// </summary>
    public class AssetBundleLoader
    {
        private string assetBundlePath = "";
        private string assetName = "";
        private List<string> assetPaths = new List<string>();
        private AssetBundle assetBundleContent;
        private Dictionary<string, Object> assetObjects = new Dictionary<string, Object>();

        private LoaderType loadType;
        private LoaderType unloadType;

        private LoaderState assetBundleLoadState = LoaderState.NONE;
        private LoaderState assetBundleExpandState = LoaderState.NONE;
        private LoaderState assetBundleUnloadState = LoaderState.NONE;

        private AssetBundleCreateRequest assetBundleLoadRequest;
        private DownloadRequest assetBundleDownloadRequest;
        private AsyncOperation assetBundleUnloadRequest;
        private int expandCount = 0;

        private event OnLoadFinished onLoadFinishedImpl;
        private event OnExpandFinished onExpandFinishedImpl;
        private event OnUnloadFinished onUnloadFinishedImpl;

        private List<string> parentBundleNames = new List<string>();

        private Dictionary<string, bool> dependentNames = new Dictionary<string, bool>();
        
        /// <summary>
        /// 异步资产捆绑包加载完成的回调。
        /// </summary>
        /// <param name="ab">已加载资产捆绑包。</param>
        public delegate void OnLoadFinished(AssetBundle ab);

        /// <summary>
        /// 异步扩展完成的回调。
        /// </summary>
        public delegate void OnExpandFinished();

        /// <summary>
        /// 异步扩展完成的回调。
        /// </summary>
        public delegate void OnUnloadFinished();

        /// <summary>
        /// 加载程序的状态枚举。
        /// </summary>
        public enum LoaderState
        {
            NONE,
            WORKING,
            FINISHED
        }

        private enum LoaderType
        {
            NONE,
            LOCAL_SYNC,
            LOCAL_ASYNC,
            REMOTE_ASYNC
        }

        /// <summary>
        /// 初始化加载程序。
        /// 派生类型的初始化行为可以通过重写来实现。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="assetBundlePath">资产捆绑包的路径。</param>
        public virtual void Init(string assetName, string assetBundlePath)
        {
            Clear();
            this.assetName = assetName;
            this.assetBundlePath = assetBundlePath;
            assetPaths = GetAssetPaths();
        }

        /// <summary>
        /// 同步加载资产。
        /// </summary>
        /// <returns>已加载资产捆绑包。</returns>
        public virtual AssetBundle Load()
        {
            ClearUnloadData();
            if (assetBundleLoadState == LoaderState.FINISHED &&
                assetBundleContent == null)
                assetBundleLoadState = LoaderState.NONE;

            if (assetBundleLoadState == LoaderState.FINISHED)
                return assetBundleContent;

            loadType = LoaderType.LOCAL_SYNC;
            if (FileTools.IsLegalHTTPURI(assetBundlePath))
            {
                DownloadRequest d = new DownloadRequest(assetBundlePath, 0);
                while (!d.IsFinished) ;
                assetBundleContent = d.DownloadedAssetBundle;
            }
            else
            {
                assetBundleContent = AssetBundle.LoadFromFile(assetBundlePath);
            }

            assetBundleLoadState = LoaderState.FINISHED;
            return assetBundleContent;
        }

        /// <summary>
        /// 异步加载资产。
        /// </summary>
        /// <param name="callback">异步加载完成的回调。</param>
        public virtual void LoadAsync(OnLoadFinished callback = null)
        {
            ClearUnloadData();
            if (assetBundleLoadState == LoaderState.FINISHED &&
                assetBundleContent == null)
                assetBundleLoadState = LoaderState.NONE;

            onLoadFinished += callback;

            if (assetBundleLoadState == LoaderState.NONE)
            {
                assetBundleLoadState = LoaderState.WORKING;
                if (FileTools.IsLegalHTTPURI(assetBundlePath))
                {
                    loadType = LoaderType.REMOTE_ASYNC;
                    assetBundleDownloadRequest = new DownloadRequest(assetBundlePath, 0);
                    if (assetBundleDownloadRequest == null)
                    {
                        assetBundleLoadState = LoaderState.FINISHED;
                        string errMsg = string.Format("Remote asset bundle {0} can't be found, please check it", assetBundlePath);
                        LogF8.LogError(errMsg);
                    }
                }
                else
                {
                    loadType = LoaderType.LOCAL_ASYNC;
                    assetBundleLoadRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
                    if (assetBundleLoadRequest == null)
                    {
                        assetBundleLoadState = LoaderState.FINISHED;
                        string errMsg = string.Format("Local asset bundle {0} can't be found, please check it", assetBundlePath);
                        LogF8.LogError(errMsg);
                    }
                }
            }
        }

        /// <summary>
        /// 同步扩展资产。
        /// 对于Unity中无法扩展的流场景资产包类型，
        /// 此扩展函数将忽略它，并直接将其标记为已展开。
        /// </summary>
        public virtual void Expand()
        {
            if (assetBundleContent == null)
            {
                assetBundleExpandState = LoaderState.NONE;
                return;
            }

            if (assetBundleExpandState == LoaderState.FINISHED &&
                assetObjects.Count != assetPaths.Count)
                assetBundleExpandState = LoaderState.NONE;

            if (assetBundleExpandState == LoaderState.FINISHED)
                return;

            expandCount = 0;
            foreach (string path in assetPaths)
            {
                LoadAssetObject(path);
            }
            expandCount = assetPaths.Count;

            assetBundleExpandState = LoaderState.FINISHED;
            return;
        }

        /// <summary>
        /// 异步展开资产。
        /// 对于无法在Unity中展开的流场景资产束类型，
        /// 此扩展函数将忽略它，并直接将其标记为已扩展。
        /// </summary>
        public virtual void ExpandAsync(OnExpandFinished callback = null)
        {
            if (!assetBundleContent)
            {
                assetBundleExpandState = LoaderState.NONE;
                return;
            }

            if (assetBundleExpandState == LoaderState.FINISHED &&
                assetObjects.Count != assetPaths.Count)
                assetBundleExpandState = LoaderState.NONE;

            onExpandFinished += callback;

            if (assetBundleExpandState == LoaderState.NONE)
            {
                expandCount = 0;
                assetBundleExpandState = LoaderState.WORKING;
                foreach (string path in assetPaths)
                {
                    LoadAssetObjectAsync(path, OnOneExpandCallBack);
                }
            }
        }

        /// <summary>
        /// 同步卸载资产。
        /// </summary>
        /// <param name="unloadAllRelated">
        /// 如果设置为true，则目标所依赖的所有资产也将被卸载，
        /// 否则将仅卸载目标资产。
        /// </param>
        public virtual void Unload(bool unloadAllRelated = true)
        {
            if (assetBundleContent != null)
            {
                assetBundleContent.Unload(unloadAllRelated);
                ClearLoadedData();
            }

            assetBundleUnloadState = LoaderState.FINISHED;
        }

        /// <summary>
        /// 异步卸载资产。
        /// </summary>
        /// <param name="unloadAllRelated">
        /// 如果设置为true，则目标所依赖的所有资产也将被卸载，
        /// 否则将仅卸载目标资产。
        /// </param>
        /// <param name="callback">异步卸载完成的回调。</param>
        public virtual void UnloadAsync(bool unloadAllRelated = true, OnUnloadFinished callback = null)
        {
            if (assetBundleContent == null)
                assetBundleUnloadState = LoaderState.FINISHED;

            onUnloadFinished += callback;

            if (assetBundleUnloadState == LoaderState.NONE)
            {
                assetBundleUnloadState = LoaderState.WORKING;
                unloadType = LoaderType.LOCAL_ASYNC;
                assetBundleUnloadRequest = assetBundleContent.UnloadAsync(unloadAllRelated);
                ClearLoadedData();
            }
        }

        /// <summary>
        /// 尝试获取资产对象。
        /// </summary>
        /// <param name="assetPath">资产捆绑包的路径。</param>
        /// <param name="obj">资产对象。</param>
        /// <returns></returns>
        public bool TryGetAsset(string assetPath, out Object obj)
        {
            if (assetObjects.ContainsKey(assetPath))
            {
                obj = assetObjects[assetPath];
                return true;
            }
            obj = null;
            return false;
        }

        /// <summary>
        /// 按资源对象名称加载资源对象。
        /// </summary>
        /// <typeparam name="T">资产对象的目标对象类型。</typeparam>
        /// <param name="assetPath">资源对象的路径。</param>
        /// <returns>找到资产对象。</returns>
        public T LoadAssetObject<T>(string assetPath)
            where T : Object
        {
            if (assetBundleContent == null)
                return null;

            // 流化场景资产包不需要扩展，
            // 但必须通过UnityEngine进行访问。场景管理。场景管理器。
            if (assetBundleContent.isStreamedSceneAssetBundle)
                return null;

            T t = assetBundleContent.LoadAsset<T>(assetPath);
            SetAssetObject(assetPath, t);
            return t;
        }

        /// <summary>
        /// 按资源对象名称加载资源对象。
        /// </summary>
        /// <param name="assetPath">资源对象的路径。</param>
        /// <param name="assetType">资产对象的目标对象类型。</param>
        /// <returns>找到资产对象。</returns>
        public Object LoadAssetObject(string assetPath, System.Type assetType)
        {
            if (assetBundleContent == null)
                return null;

            // 流化场景资产包不需要扩展，
            // 但必须通过UnityEngine进行访问。场景管理。场景管理器。
            if (assetBundleContent.isStreamedSceneAssetBundle)
                return null;

            Object o = assetBundleContent.LoadAsset(assetPath, assetType);
            SetAssetObject(assetPath, o);
            if (assetType.IsAssignableFrom(o.GetType()))
            {
                return o;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 按资源对象名称加载资源对象。
        /// </summary>
        /// <param name="assetPath">资源对象的路径。</param>
        /// <returns>找到资产对象。</returns>
        public Object LoadAssetObject(string assetPath)
        {
            if (assetBundleContent == null)
                return null;

            // 流化场景资产包不需要扩展，
            // 但必须通过UnityEngine进行访问。场景管理。场景管理器。
            if (assetBundleContent.isStreamedSceneAssetBundle)
                return null;

            Object o = assetBundleContent.LoadAsset(assetPath);
            SetAssetObject(assetPath, o);
            return o;
        }

        /// <summary>
        /// 通过资产捆绑路径异步加载。
        /// </summary>
        /// <typeparam name="T">资产对象的目标对象类型。</typeparam>
        /// <param name="assetPath">资源对象的路径。</param>
        /// <param name="callback">异步加载完成的回调。</param>
        public void LoadAssetObjectAsync<T>(
            string assetPath,
            OnAssetObject<T> callback = null)
            where T : Object
        {
            // 流化场景资产包不需要扩展，
            // 但必须通过UnityEngine进行访问。场景管理。场景管理器。
            if (assetBundleContent == null ||
                assetBundleContent.isStreamedSceneAssetBundle)
            {
                End();
                return;
            }

            AssetBundleRequest rq = assetBundleContent.LoadAssetAsync<T>(assetPath);
            rq.completed +=
                ao =>
                {
                    Object o = rq.asset;
                    SetAssetObject(assetPath, o);

                    T t = o as T;
                    End(t);
                };

            void End(T o = null)
            {
                if (callback != null)
                    callback(o);
            }
        }

        /// <summary>
        /// 通过资产捆绑路径异步加载。
        /// </summary>
        /// <param name="assetPath">资源对象的路径。</param>
        /// <param name="assetType">资产对象的目标对象类型。</param>
        /// <param name="callback">异步加载完成的回调。</param>
        public void LoadAssetObjectAsync(
            string assetPath,
            System.Type assetType,
            OnAssetObject<Object> callback = null)
        {
            // 流化场景资产包不需要扩展，
            // 但必须通过UnityEngine进行访问。场景管理。场景管理器。
            if (assetBundleContent == null ||
                assetBundleContent.isStreamedSceneAssetBundle)
            {
                End();
                return;
            }

            AssetBundleRequest rq = assetBundleContent.LoadAssetAsync(assetPath, assetType);
            rq.completed +=
                ao => {
                    Object o = rq.asset;
                    SetAssetObject(assetPath, o);

                    if (assetType.IsAssignableFrom(o.GetType()))
                    {
                        End(o);
                    }
                    else
                    {
                        End();
                    }
                };

            void End(Object o = null)
            {
                if (callback != null)
                    callback(o);
            }
        }

        /// <summary>
        /// 通过资产捆绑路径异步加载。
        /// </summary>
        /// <param name="assetPath">资源对象的路径。</param>
        /// <param name="callback">异步加载完成的回调。</param>
        public void LoadAssetObjectAsync(
            string assetPath,
            OnAssetObject<Object> callback = null)
        {
            // 流化场景资产包不需要扩展，
            // 但必须通过UnityEngine进行访问。场景管理。场景管理器。
            if (!assetBundleContent||
                assetBundleContent.isStreamedSceneAssetBundle)
            {
                End();
                return;
            }

            AssetBundleRequest rq = assetBundleContent.LoadAssetAsync(assetPath);
            rq.completed +=
                ao => {
                    Object o = rq.asset;
                    SetAssetObject(assetPath, o);

                    End(o);
                };

            void End(Object o = null)
            {
                callback?.Invoke(o);
            }
        }

        /// <summary>
        /// 派生类型的更新行为可以通过重写来实现。
        /// </summary>
        public virtual void OnUpdate()
        {
            if (assetBundleLoadState == LoaderState.WORKING)
            {
                switch (loadType)
                {
                    case LoaderType.LOCAL_ASYNC:
                        if (assetBundleLoadRequest != null)
                        {
                            if (assetBundleLoadRequest.isDone)
                            {
                                if (!assetBundleLoadRequest.assetBundle)
                                {
                                    assetBundleLoadState = LoaderState.FINISHED;
                                    string errMsg = string.Format("Load asset bundle {0} can't be loaded.", assetBundlePath);
                                    LogF8.LogError(errMsg);
                                }
                                else
                                {
                                    assetBundleContent = assetBundleLoadRequest.assetBundle;
                                    assetBundleLoadState = LoaderState.FINISHED;
                                }
                            }
                        }
                        break;
                    case LoaderType.REMOTE_ASYNC:
                        if (assetBundleDownloadRequest != null)
                        {
                            if (assetBundleDownloadRequest.IsFinished)
                            {
                                if (!assetBundleDownloadRequest.DownloadedAssetBundle)
                                {
                                    assetBundleLoadState = LoaderState.FINISHED;
                                    string errMsg = string.Format("Remote asset bundle {0} can't be loaded, try reload", assetBundlePath);
                                    LogF8.LogError(errMsg);
                                }
                                else
                                {
                                    assetBundleContent = assetBundleDownloadRequest.DownloadedAssetBundle;
                                    assetBundleLoadState = LoaderState.FINISHED;
                                }
                            }
                        }
                        break;
                }

                if (assetBundleLoadState == LoaderState.FINISHED &&
                    onLoadFinishedImpl != null)
                {
                    onLoadFinishedImpl(assetBundleContent);
                    onLoadFinishedImpl = null;
                }
            }

            if (assetBundleUnloadState == LoaderState.WORKING)
            {
                switch (unloadType)
                {
                    case LoaderType.LOCAL_ASYNC:
                        if (assetBundleUnloadRequest != null)
                        {
                            if (assetBundleUnloadRequest.isDone)
                            {
                                assetBundleUnloadState = LoaderState.FINISHED;
                            }
                        }
                        break;
                }

                if (assetBundleUnloadState == LoaderState.FINISHED &&
                    onUnloadFinishedImpl != null)
                {
                    onUnloadFinishedImpl();
                }
            }
        }

        /// <summary>
        /// 清除加载程序内容。
        /// </summary>
        /// <param name="unloadAllRelated">
        /// 如果设置为true，则目标所依赖的所有资产也将被卸载，
        /// 否则将仅卸载目标资产。
        /// </param>
        public void Clear(bool unloadAllRelated = false)
        {
            assetBundlePath = "";
            assetPaths.Clear();
            if (assetBundleContent != null)
                Unload(unloadAllRelated);

            loadType = LoaderType.NONE;
            unloadType = LoaderType.NONE;

            assetBundleLoadState = LoaderState.NONE;
            assetBundleExpandState = LoaderState.NONE;
            assetBundleUnloadState = LoaderState.NONE;

            assetBundleLoadRequest = null;
            assetBundleDownloadRequest?.Dispose();
            assetBundleDownloadRequest = null;
            assetBundleUnloadRequest = null;
            expandCount = 0;

            onLoadFinishedImpl = null;
            onExpandFinishedImpl = null;
            onUnloadFinishedImpl = null;

            assetObjects.Clear();
            parentBundleNames.Clear();
            dependentNames.Clear();
        }
        
        public int GetDependentNamesLoadFinished()
        {
            int loadFinishedCount = 0;
            foreach (var item in dependentNames)
            {
                if (item.Value == true)
                {
                    loadFinishedCount += 1;
                }
            }
            
            return loadFinishedCount;
        }
        
        public int AddDependentNames(string name = null, bool loadFinished = false)
        {
            if (name == null)
                return dependentNames.Count;

            if (loadFinished && dependentNames.ContainsKey(name))
            {
                dependentNames[name] = loadFinished;
            }
            else
            {
                if (loadFinished == false)
                {
                    dependentNames.TryAdd(name, loadFinished);
                }
            }

            return dependentNames.Count;
        }

        public int RemoveDependentNames(string name)
        {
            if (dependentNames.TryGetValue(name, out bool loadFinished))
                dependentNames.Remove(name);

            return dependentNames.Count;
        }
        
        public int AddParentBundle(string name = null)
        {
            if (name == null)
                return parentBundleNames.Count;

            if (!parentBundleNames.Contains(name))
                parentBundleNames.Add(name);

            return parentBundleNames.Count;
        }

        public int RemoveParentBundle(string name)
        {
            if (parentBundleNames.Contains(name))
                parentBundleNames.Remove(name);

            return parentBundleNames.Count;
        }

        public bool IsParentBundle(string name)
        {
            return parentBundleNames.Contains(name);
        }

        public bool IsLastParentBundle(string name)
        {
            return parentBundleNames.Count == 1 &&
                   parentBundleNames.Contains(name);
        }

        private void ClearLoadedData()
        {
            loadType = LoaderType.NONE;
            assetBundleLoadState = LoaderState.NONE;
            assetBundleExpandState = LoaderState.NONE;
            assetBundleContent = null;
            assetObjects.Clear();
            assetBundleLoadRequest = null;
            assetBundleDownloadRequest?.Dispose();
            assetBundleDownloadRequest = null;
            expandCount = 0;
            onLoadFinishedImpl = null;
            onExpandFinishedImpl = null;
        }

        private void ClearUnloadData()
        {
            unloadType = LoaderType.NONE;
            assetBundleUnloadState = LoaderState.NONE;
            assetBundleUnloadRequest = null;
            onUnloadFinishedImpl = null;
        }

        private void OnOneExpandCallBack(Object o)
        {
            ++expandCount;
            if (expandCount == assetPaths.Count)
            {
                assetBundleExpandState = LoaderState.FINISHED;
                if (onExpandFinishedImpl != null)
                {
                    onExpandFinishedImpl();
                    onExpandFinishedImpl = null;
                }
            }
        }

        private List<string> GetAssetPaths()
        {
            List<string> paths = new List<string>();
            
            if (AssetBundleMap.Mappings.TryGetValue(assetName, out AssetBundleMap.AssetMapping assetmpping))
            {
                foreach (var assetPath in assetmpping.AssetPath)
                {
                    paths.Add(assetPath);
                }
            }
            
            return paths;
        }

        private void SetAssetObject(string assetPath, Object obj)
        {
            if (assetPath == null ||
                obj == null)
                return;

            if (assetObjects.ContainsKey(assetPath))
            {
                assetObjects[assetPath] = obj;
            }
            else
            {
                assetObjects.Add(assetPath, obj);
            }
        }
        
        /// <summary>
        /// 异步加载请求。
        /// </summary>
        public AssetBundleCreateRequest AssetBundleLoadRequest
        {
            get => assetBundleLoadRequest;
        }
            
        /// <summary>
        /// 此加载程序的资产捆绑包路径。
        /// </summary>
        public string AssetBundlePath
        {
            get => assetBundlePath;
        }

        /// <summary>
        /// 已加载资产捆绑包内容。
        /// </summary>
        public AssetBundle AssetBundleContent
        {
            get => assetBundleContent;
        }

        /// <summary>
        /// 确定资产捆绑包的加载是否完成。
        /// </summary>
        public bool IsLoadFinished
        {
            get => assetBundleLoadState == LoaderState.FINISHED;
        }

        /// <summary>
        /// 确定资源对象的加载是否已完成。
        /// </summary>
        public bool IsExpandFinished
        {
            get => assetBundleExpandState == LoaderState.FINISHED;
        }

        /// <summary>
        /// 确定资产包的卸载是否完成。
        /// </summary>
        public bool IsUnloadFinished
        {
            get => assetBundleUnloadState == LoaderState.FINISHED;
        }

        /// <summary>
        /// 确定资产是否已开始卸载。
        /// </summary>
        public bool IsUnloadCalled
        {
            get => assetBundleUnloadState != LoaderState.NONE;
        }

        /// <summary>
        /// 加载进度。
        /// 值的范围从0到1。
        /// </summary>
        public float LoadProgress
        {
            get
            {
                switch (loadType)
                {
                    case LoaderType.LOCAL_SYNC:
                        if (assetBundleLoadState == LoaderState.FINISHED)
                            return 1f;
                        break;
                    case LoaderType.LOCAL_ASYNC:
                        if (assetBundleLoadRequest != null)
                            return assetBundleLoadRequest.progress;
                        break;
                    case LoaderType.REMOTE_ASYNC:
                        if (assetBundleDownloadRequest != null)
                            return assetBundleDownloadRequest.Progress;
                        break;
                }
                return 0f;
            }
        }

        /// <summary>
        /// 扩大进度。
        /// 值的范围从0到1。
        /// </summary>
        public float ExpandProgress
        {
            get
            {
                if (assetPaths == null ||
                    assetPaths.Count == 0)
                    return 0;

                return expandCount / assetPaths.Count;
            }
        }

        /// <summary>
        /// 卸载进度。
        /// 值的范围从0到1。
        /// </summary>
        public float UnloadProgress
        {
            get
            {
                switch (unloadType)
                {
                    case LoaderType.LOCAL_SYNC:
                        if (assetBundleUnloadState == LoaderState.FINISHED)
                            return 1f;
                        break;
                    case LoaderType.LOCAL_ASYNC:
                        if (assetBundleUnloadRequest != null)
                            return assetBundleUnloadRequest.progress;
                        break;
                }
                return 0f;
            }
        }

        private event OnLoadFinished onLoadFinished
        {
            add 
            {
                if (value == null)
                    return;

                if (assetBundleLoadState == LoaderState.FINISHED)
                    value(assetBundleContent);
                else
                    onLoadFinishedImpl += value;
            }
            remove
            {
                if (value != null)
                    onLoadFinishedImpl -= value;
            }
        }

        private event OnExpandFinished onExpandFinished
        {
            add
            {
                if (value == null)
                    return;

                if (assetBundleExpandState == LoaderState.FINISHED)
                    value();
                else
                    onExpandFinishedImpl += value;
            }
            remove
            {
                if (value != null)
                    onExpandFinishedImpl -= value;
            }
        }

        private event OnUnloadFinished onUnloadFinished
        {
            add
            {
                if (value == null)
                    return;

                if (assetBundleUnloadState == LoaderState.FINISHED)
                    value();
                else
                    onUnloadFinishedImpl += value;
            }
            remove
            {
                if (value != null)
                    onUnloadFinishedImpl -= value;
            }
        }
    }
}