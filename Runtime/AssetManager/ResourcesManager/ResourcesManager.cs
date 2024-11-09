using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 提供资源管理工具 - 包括加载和卸载。
    /// </summary> 
    public class ResourcesManager : ModuleSingleton<ResourcesManager>, IModule
    {
        
        private Dictionary<string, ResourcesLoader> resourceLoaders = new Dictionary<string, ResourcesLoader>();
        
        /// <summary>
        /// 通过相对资源名称同步加载。
        /// 如果资源重复加载，将直接从资源池提供。
        /// </summary>
        /// <typeparam name="T">Asset对象的目标对象类型。</typeparam>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <returns>加载的资源对象。</returns>
        public T Load<T>(string resourcePath)
            where T : Object
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            return loader.Load<T>();
        }

        /// <summary>
        /// 通过相对资源名称同步加载。
        /// 如果资源重复加载，将直接从资源池提供。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <param name="resourceType">Asset对象的目标对象类型。</param>
        /// <returns>加载的资源对象。</returns>
        public Object Load(string resourcePath, System.Type resourceType)
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            return loader.Load(resourceType);
        }

        /// <summary>
        /// 通过相对资源名称同步加载。
        /// 如果资源重复加载，将直接从资源池提供。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <returns>加载的资源对象。</returns>
        public Object Load(string resourcePath)
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            return loader.Load();
        }

        /// <summary>
        /// 通过相对资源名称异步加载。
        /// 如果资源重复加载，将直接从资源池提供。
        /// </summary>
        /// <typeparam name="T">Asset对象的目标对象类型。</typeparam>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <param name="callback">异步加载完成的回调。</param>
        public void LoadAsync<T>(
            string resourcePath,
            OnAssetObject<T> callback = null)
            where T : Object
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            loader.LoadAsync<T>(callback);
        }

        public IEnumerator LoadAsyncCoroutine<T>(string resourcePath) where T : Object
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            yield return loader.LoadAsyncCoroutine<T>();
        }
        
        /// <summary>
        /// 通过相对资源名称异步加载。
        /// 如果资源重复加载，将直接从资源池提供。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <param name="resourceType">Asset对象的目标对象类型。</param>
        /// <param name="callback">异步加载完成的回调。</param>
        public void LoadAsync(
            string resourcePath,
            System.Type resourceType,
            OnAssetObject<Object> callback = null)
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            loader.LoadAsync(resourceType, callback);
        }

        public IEnumerator LoadAsyncCoroutine(string resourcePath, System.Type resourceType = default)
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            yield return loader.LoadAsyncCoroutine(resourceType);
        }
        
        /// <summary>
        /// 通过相对资源名称异步加载。
        /// 如果资源重复加载，将直接从资源池提供。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <param name="callback">异步加载完成的回调。</param>
        public void LoadAsync(
            string resourcePath,
            OnAssetObject<Object> callback = null)
        {
            ResourcesLoader loader;
            if (resourceLoaders.ContainsKey(resourcePath))
            {
                loader = resourceLoaders[resourcePath];
            }
            else
            {
                loader = new ResourcesLoader();
                loader.Init(resourcePath);
                resourceLoaders.Add(resourcePath, loader);
            }

            loader.LoadAsync(callback);
        }

        /// <summary>
        /// 通过相对资源名称同步卸载。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <param name="unloadAllLoadedObjects">完全卸载。</param>
        public void Unload(string resourcePath, bool unloadAllLoadedObjects = true)
        {
            if (unloadAllLoadedObjects == false)
            {
                return;
            }
            if (resourceLoaders.TryGetValue(
                    resourcePath,
                    out ResourcesLoader loader)
                )
            {
                loader.Clear();
                resourceLoaders.Remove(resourcePath);
            }
        }

        /// <summary>
        /// 通过已存在的资源加载器同步卸载。
        /// </summary>
        /// <param name="loader">要卸载的资源加载器。</param>
        /// <param name="unloadAllLoadedObjects">完全卸载。</param>
        public void Unload(ResourcesLoader loader, bool unloadAllLoadedObjects = true)
        {
            if (loader == null)
                return;
            
            if (unloadAllLoadedObjects == false)
            {
                return;
            }

            if (resourceLoaders.ContainsValue(loader))
            {
                List<string> keys = new List<string>();
                foreach (var kv in resourceLoaders)
                {
                    if (kv.Value == loader)
                    {
                        keys.Add(kv.Key);
                    }
                }

                foreach (string key in keys)
                {
                    Unload(key, unloadAllLoadedObjects);
                }
            }
            else
            {
                loader.Clear();
            }
        }

        /// <summary>
        /// 通过已加载的资源对象同步卸载。
        /// </summary>
        /// <param name="obj">已加载的资源对象。</param>
        /// <param name="unloadAllLoadedObjects">完全卸载。</param>
        public void Unload(Object obj, bool unloadAllLoadedObjects = true)
        {
            if (obj == null)
                return;

            if (unloadAllLoadedObjects == false)
            {
                return;
            }
            
            List<string> keys = new List<string>();
            foreach (var kv in resourceLoaders)
            {
                if (kv.Value.ResouceObject == obj)
                {
                    keys.Add(kv.Key);
                }
            }

            foreach (string key in keys)
            {
                Unload(key, unloadAllLoadedObjects);
            }

            if (keys.Count <= 0 && obj != null)
            {
                Resources.UnloadAsset(obj);
            }
        }

        /// <summary>
        /// 加载资源文件夹下的所有资源。
        /// </summary>
        /// <param name="resourceDirectoryPath">资源文件夹的相对目录路径。</param>
        /// <param name="systemTypeInstance">返回对象的类型筛选器。</param>
        /// <returns>加载的资源对象。</returns>
        public Object[] LoadAll(
            string resourceDirectoryPath, 
            System.Type systemTypeInstance = null
        )
        {
            UnityEngine.Object[] result;
            if (systemTypeInstance == null)
            {
                result = Resources.LoadAll(resourceDirectoryPath);
            }
            else
            { 
                result = Resources.LoadAll(resourceDirectoryPath, systemTypeInstance);
            }

            if (result == null)
                return null;

            foreach (UnityEngine.Object obj in result)
            {
                string path = resourceDirectoryPath + "/" + obj.name;
                ResourcesLoader loader = new ResourcesLoader();
                loader.Init(path, obj);

                if (resourceLoaders.ContainsKey(path))
                {
                    resourceLoaders[path] = loader;
                }
                else
                {
                    resourceLoaders.Add(path, loader);
                }
            }

            return result;
        }

        /// <summary>
        /// 通过相对资源名称获取资源对象列表。
        /// </summary>
        /// <typeparam name="T">Asset对象的目标对象类型。</typeparam>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <returns>加载的资源对象。</returns>
        public T GetResouceObject<T>(string resourcePath)
            where T : Object
        {
            if (IsLoadFinished(resourcePath))
            {
                if (resourceLoaders.TryGetValue(
                    resourcePath,
                    out ResourcesLoader loader))
                {
                    return loader.ResouceObject as T;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过相对资源名称获取资源对象列表。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <param name="resourceType">Asset对象的目标对象类型。</param>
        /// <returns>加载的资源对象。</returns>
        public Object GetResouceObject(string resourcePath, System.Type resourceType = default)
        {
            if (IsLoadFinished(resourcePath))
            {
                if (resourceLoaders.TryGetValue(
                    resourcePath,
                    out ResourcesLoader loader))
                {
                    return loader.ResouceObject;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有加载器的加载进度。
        /// 正常值范围从0到1。
        /// 例外情况是如果没有加载器，则返回-1。
        /// </summary>
        /// <returns>加载进度。</returns>
        public float GetLoadProgress()
        {
            float allProgress = 0f;
            int cnt = resourceLoaders.Values.Count;
            if (cnt == 0)
            {
                return -1f;
            }

            foreach (ResourcesLoader loader in resourceLoaders.Values)
            {
                allProgress += loader.LoadProgress;
            }

            float average = allProgress / cnt;
            return average;
        }

        /// <summary>
        /// 获取具有足够名称的加载器的加载进度。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <returns>加载进度。</returns>
        public float GetLoadProgress(string resourcePath)
        {
            if (resourceLoaders.TryGetValue(
                resourcePath,
                out ResourcesLoader loader))
            {
                return loader.LoadProgress;
            }

            return -1f;
        }

        /// <summary>
        /// 查询是否所有资源都已加载。
        /// </summary>
        /// <returns>
        /// 如果所有加载都完成，则返回true，
        /// 否则返回false。
        /// </returns>
        public bool IsLoadFinished()
        {
            bool allLoaderFinished = true;
            foreach (ResourcesLoader loader in resourceLoaders.Values)
            {
                if (!loader.IsLoadFinished)
                {
                    allLoaderFinished = false;
                    break;
                }
            }

            return allLoaderFinished;
        }

        /// <summary>
        /// 查询是否已加载指定的资源。
        /// </summary>
        /// <param name="resourcePath">资源文件夹的相对路径。</param>
        /// <returns>
        /// 如果指定的加载完成，则返回true，
        /// 否则返回false。
        /// </returns>
        public bool IsLoadFinished(string resourcePath)
        {
            if (resourceLoaders.TryGetValue(
                resourcePath,
                out ResourcesLoader loader))
            {
                return loader.IsLoadFinished;
            }

            return false;
        }

        private void Clear()
        {
            foreach (var loader in resourceLoaders.Values)
            {
                loader.Clear();
            }

            resourceLoaders.Clear();
        }

        public void OnInit(object createParam)
        {
            
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            Clear();
            base.Destroy();
        }
    }
}