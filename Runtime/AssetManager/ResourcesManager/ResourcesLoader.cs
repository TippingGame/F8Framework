using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 一个资源加载器。
    /// 管理资源的加载行为。
    /// </summary>
    public class ResourcesLoader : BaseLoader
    {
        private string resourcePath = "";
        private string subAssetName = "";
        private Object resouceObject;
        private Dictionary<string, Object> resouceObjects = new Dictionary<string, Object>();
        
        private LoaderType loadType;
        private LoaderState resourceLoadState;
        private ResourceRequest resourceLoadRequest;
        
        public override bool LoaderSuccess => resourceLoadState == LoaderState.FINISHED;
        
        /// <summary>
        /// 加载类型：同步或异步本地加载。
        /// </summary>
        public enum LoaderType
        {
            NONE,
            SYNC,
            ASYNC
        }

        /// <summary>
        /// 加载器的状态枚举。
        /// </summary>
        public enum LoaderState
        {
            NONE,
            WORKING,
            FINISHED
        }

        /// <summary>
        /// 初始化加载器。
        /// 您的派生类型的初始化行为可以通过覆盖来实现。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        public virtual void Init(string resourcePath)
        {
            this.resourcePath = resourcePath;
            loadType = LoaderType.NONE;
            resourceLoadState = LoaderState.NONE;
            resourceLoadRequest = null;
        }

        /// <summary>
        /// 使用预加载的资源对象初始化加载器。
        /// 您的派生类型的初始化行为可以通过覆盖来实现。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <param name="obj">预加载的资源对象。</param>
        public virtual void Init(
            string resourcePath, 
            Object obj)
        {
            this.resourcePath = resourcePath;
            loadType = LoaderType.SYNC;
            resouceObject = obj;
            resourceLoadState = LoaderState.FINISHED;
            resourceLoadRequest = null;
        }

        public override T GetAssetObject<T>(string subAssetName = null)
        {
            if (IsLoadFinished)
            {
                if (subAssetName.IsNullOrEmpty())
                {
                    if (this.subAssetName.IsNullOrEmpty())
                    {
                        return this.ResouceObject as T;
                    }
                    bool success = TryGetAsset(resourcePath + this.subAssetName, out Object obj);
                    if (success)
                    {
                        return obj as T;
                    }
                }
                else
                {
                    bool success = TryGetAsset(resourcePath + subAssetName, out Object obj);
                    if (success)
                    {
                        return obj as T;
                    }
                }
            }
            
            return null;
        }
        
        public override Object GetAssetObject(string subAssetName = null)
        {
            if (IsLoadFinished)
            {
                if (subAssetName.IsNullOrEmpty())
                {
                    if (this.subAssetName.IsNullOrEmpty())
                    {
                        return this.ResouceObject;
                    }
                    bool success = TryGetAsset(resourcePath + this.subAssetName, out Object obj);
                    if (success)
                    {
                        return obj;
                    }
                }
                else
                {
                    bool success = TryGetAsset(resourcePath + subAssetName, out Object obj);
                    if (success)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <typeparam name="T">Asset对象的目标对象类型。</typeparam>
        /// <returns>加载的资源对象。</returns>
        public virtual T Load<T>()
            where T : Object
        {
            if (resourceLoadState == LoaderState.FINISHED)
                return resouceObject as T;

            loadType = LoaderType.SYNC;
            resourceLoadState = LoaderState.WORKING;
            resouceObject = Resources.Load<T>(resourcePath);
            resourceLoadState = LoaderState.FINISHED;
            return resouceObject as T;
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="resourceType">Asset对象的目标对象类型。</param>
        /// <returns>加载的资源对象。</returns>
        public virtual Object Load(System.Type resourceType)
        {
            if (resourceLoadState == LoaderState.FINISHED)
                return resouceObject;

            loadType = LoaderType.SYNC;
            resourceLoadState = LoaderState.WORKING;
            resouceObject = Resources.Load(resourcePath, resourceType);
            resourceLoadState = LoaderState.FINISHED;
            return resouceObject;
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <returns>加载的资源对象。</returns>
        public virtual Object Load()
        {
            if (resourceLoadState == LoaderState.FINISHED)
                return resouceObject;

            loadType = LoaderType.SYNC;
            resourceLoadState = LoaderState.WORKING;
            resouceObject = Resources.Load(resourcePath);
            resourceLoadState = LoaderState.FINISHED;
            return resouceObject;
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <typeparam name="T">Asset对象的目标对象类型。</typeparam>
        /// <param name="callback">异步加载完成的回调。</param>
        public virtual void LoadAsync<T>(OnAssetObject<T> callback = null)
            where T : Object
        {
            if (resourceLoadState == LoaderState.NONE)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadState = LoaderState.WORKING;
                resourceLoadRequest = Resources.LoadAsync<T>(resourcePath);
                resourceLoadRequest.completed +=
                    ao => {
                        resouceObject = resourceLoadRequest.asset;
                        resourceLoadState = LoaderState.FINISHED;

                        T t = resouceObject as T;
                        End(t);
                    };
            }
            else if (resourceLoadState == LoaderState.WORKING)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadRequest.completed +=
                    ao => {
                        T t = resourceLoadRequest.asset as T;
                        End(t);
                    };
            }
            
            void End(T o = null)
            {
                if (callback != null)
                    callback(o);
                base.OnComplete();
            }
        }

        public virtual IEnumerator LoadAsyncCoroutine<T>() where T : Object
        {
            if (resourceLoadState == LoaderState.NONE)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadState = LoaderState.WORKING;
                resourceLoadRequest = Resources.LoadAsync<T>(resourcePath);

                // Wait until the resource is loaded
                yield return resourceLoadRequest;

                resouceObject = resourceLoadRequest.asset;
                resourceLoadState = LoaderState.FINISHED;
                
                // 返回加载完成的资源对象
                yield return resouceObject;
            }
            else if (resourceLoadState == LoaderState.WORKING)
            {
                loadType = LoaderType.ASYNC;
                yield return new WaitUntil(() => IsLoadFinished);
                yield return resouceObject;
            }
        }
        
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="callback">异步加载完成的回调。</param>
        public virtual void LoadAsync(System.Type resourceType, OnAssetObject<Object> callback = null)
        {
            if (resourceLoadState == LoaderState.NONE)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadState = LoaderState.WORKING;
                resourceLoadRequest = Resources.LoadAsync(resourcePath, resourceType);
                resourceLoadRequest.completed +=
                    ao => {
                        resouceObject = resourceLoadRequest.asset;
                        resourceLoadState = LoaderState.FINISHED;

                        if (resourceType.IsAssignableFrom(resouceObject.GetType()))
                        {
                            End(resouceObject);
                        }
                        else
                        {
                            LogF8.LogError("与输入的资产类型不一致：" + resourcePath);
                            End();
                        }
                    };
            }
            else if (resourceLoadState == LoaderState.WORKING)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadRequest.completed +=
                    ao => {
                        End(resourceLoadRequest.asset);
                    };
            }

            void End(Object o = null)
            {
                if (callback != null)
                    callback(o);
                base.OnComplete();
            }
        }

        public virtual IEnumerator LoadAsyncCoroutine(System.Type resourceType = default)
        {
            if (resourceLoadState == LoaderState.NONE)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadState = LoaderState.WORKING;
                resourceLoadRequest = resourceType == default ?
                    Resources.LoadAsync(resourcePath):
                    Resources.LoadAsync(resourcePath, resourceType);

                // Wait until the resource is loaded
                yield return resourceLoadRequest;

                resouceObject = resourceLoadRequest.asset;
                resourceLoadState = LoaderState.FINISHED;
                
                // 返回加载完成的资源对象
                yield return resouceObject;
            }
            else if (resourceLoadState == LoaderState.WORKING)
            {
                loadType = LoaderType.ASYNC;
                yield return new WaitUntil(() => IsLoadFinished);
                yield return resouceObject;
            }
        }
        
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="callback">异步加载完成的回调。</param>
        public virtual void LoadAsync(OnAssetObject<Object> callback = null)
        {
            if (resourceLoadState == LoaderState.NONE)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadState = LoaderState.WORKING;
                resourceLoadRequest = Resources.LoadAsync(resourcePath);
                resourceLoadRequest.completed +=
                    ao => {
                        resouceObject = resourceLoadRequest.asset;
                        resourceLoadState = LoaderState.FINISHED;
                        End(resouceObject);
                    };
            }
            else if (resourceLoadState == LoaderState.WORKING)
            {
                loadType = LoaderType.ASYNC;
                resourceLoadRequest.completed +=
                    ao => {
                        End(resourceLoadRequest.asset);
                    };
            }
            
            void End(Object o = null)
            {
                if (callback != null)
                    callback(o);
                base.OnComplete();
            }
        }

        public virtual T LoadAll<T>(string subAssetName = null)
            where T : Object
        {
            if (!subAssetName.IsNullOrEmpty())
            {
                this.subAssetName = subAssetName;
            }
            
            if (resourceLoadState == LoaderState.FINISHED)
            {
                if (subAssetName.IsNullOrEmpty())
                {
                    return resouceObject as T;
                }
                bool success = TryGetAsset(resourcePath + subAssetName, out Object obj);
                if (success)
                {
                    return obj as T;
                }
            }

            loadType = LoaderType.SYNC;
            resourceLoadState = LoaderState.WORKING;
            resouceObject = Resources.Load<T>(resourcePath);
            T[] result = Resources.LoadAll<T>(resourcePath);
            Object _resouceObject = null;
            foreach (T obj in result)
            {
                SetResouceObject(resourcePath + obj.name, obj);
                if (!subAssetName.IsNullOrEmpty() && obj.name.Equals(subAssetName))
                {
                    _resouceObject = obj;
                }
            }
            resourceLoadState = LoaderState.FINISHED;
            return _resouceObject != null ? _resouceObject as T : resouceObject as T;
        }
        
        public virtual Object LoadAll(System.Type assetType = default, string subAssetName = null)
        {
            if (!subAssetName.IsNullOrEmpty())
            {
                this.subAssetName = subAssetName;
            }
            
            if (resourceLoadState == LoaderState.FINISHED)
            {
                if (subAssetName.IsNullOrEmpty())
                {
                    return resouceObject;
                }
                bool success = TryGetAsset(resourcePath + subAssetName, out Object obj);
                if (success)
                {
                    return obj;
                }
            }

            loadType = LoaderType.SYNC;
            resourceLoadState = LoaderState.WORKING;
            Object[] result;
            if (assetType == default)
            {
                resouceObject = Resources.Load(resourcePath);
                result = Resources.LoadAll(resourcePath);
            }
            else
            {
                resouceObject = Resources.Load(resourcePath, assetType);
                result = Resources.LoadAll(resourcePath, assetType);
            }
            Object _resouceObject = null;
            foreach (Object obj in result)
            {
                SetResouceObject(resourcePath + obj.name, obj);
                if (!subAssetName.IsNullOrEmpty() && obj.name.Equals(subAssetName))
                {
                    _resouceObject = obj;
                }
            }
            resourceLoadState = LoaderState.FINISHED;
            return _resouceObject != null ? _resouceObject : resouceObject;
        }
        
        /// <summary>
        /// 清除加载器内容。
        /// </summary>
        public void Clear()
        {
            resourcePath = "";
            subAssetName = "";
            if(resouceObject != null &&
               !(resouceObject is GameObject))
                Resources.UnloadAsset(resouceObject);

            foreach (var re in resouceObjects.Values)
            {
                if(re != null &&
                   !(re is GameObject))
                    Resources.UnloadAsset(re);
            }
            
            loadType = LoaderType.NONE;
            resourceLoadState = LoaderState.NONE;
            resourceLoadRequest = null;
        }

        /// <summary>
        /// 加载的资源对象。
        /// </summary>
        public Object ResouceObject
        {
            get => resouceObject;
        }

        public bool TryGetAsset(string resourcePath, out Object obj)
        {
            if (resouceObjects.ContainsKey(resourcePath))
            {
                obj = resouceObjects[resourcePath];
                return true;
            }
            obj = null;
            return false;
        }
        
        public void SetResouceObject(string resourcePath, Object obj)
        {
            if (resourcePath == null ||
                obj == null)
            {
                LogF8.LogError("加载资产对象Object为空，请检查类型和路径：" + resourcePath);
                return;
            }

            resouceObjects[resourcePath] = obj;
        }
        
        /// <summary>
        /// 确定资源加载是否完成。
        /// </summary>
        public bool IsLoadFinished
        {
            get => resourceLoadState == LoaderState.FINISHED;
        }

        /// <summary>
        /// 加载进度。
        /// 值范围从0到1。
        /// </summary>
        public float LoadProgress
        {
            get
            {
                switch (loadType)
                {
                    case LoaderType.SYNC:
                        if (resourceLoadState == LoaderState.FINISHED)
                            return 1f;
                        break;
                    case LoaderType.ASYNC:
                        if (resourceLoadRequest != null)
                            return resourceLoadRequest.progress;
                        break;
                }
                return 0f;
            }
        }
    }
}