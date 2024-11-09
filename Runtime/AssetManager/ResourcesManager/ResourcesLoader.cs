using System.Collections;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 一个资源加载器。
    /// 管理资源的加载行为。
    /// </summary>
    public class ResourcesLoader
    {
        private string resourcePath;
        private Object resouceObject;

        private LoaderType loadType;
        private LoaderState resourceLoadState;
        private ResourceRequest resourceLoadRequest;
        
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
            }
        }

        /// <summary>
        /// 清除加载器内容。
        /// </summary>
        public void Clear()
        {
            resourcePath = "";
            if(resouceObject != null &&
               !(resouceObject is GameObject))
                Resources.UnloadAsset(resouceObject);

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