using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    /// <summary>
    /// 提供资产捆绑包管理工具，包括加载、扩展和卸载。
    /// </summary>
    [UpdateRefresh]
    public class AssetBundleManager : ModuleSingleton<AssetBundleManager>, IModule
    {
        
        private AssetBundleManifest manifest;
        private Dictionary<string, AssetBundleLoader> assetBundleLoaders = new Dictionary<string, AssetBundleLoader>();
#if UNITY_WEBGL
        private DownloadRequest downloadManifest = null;
        private bool isDownloadManifest = false;
#endif
        
        /// <summary>
        /// 通过资产捆绑路径同步加载。
        /// 如果重复加载资产，则将直接从资源池中提供。
        /// </summary>
        /// <param name="assetName">资产名称。</param>
        /// <param name="info">资产信息。</param>
        /// <returns>要完成扩展的对象列表。</returns>
        public AssetBundle Load(string assetName, AssetManager.AssetInfo info)
        {
            AssetBundle result;

            List<string> assetBundlePaths = new List<string>(GetDependenciedAssetBundles(info.AbName));
            
            for (int i = 0; i < assetBundlePaths.Count; i++)
            {
                assetBundlePaths[i] = info.AssetBundlePathWithoutAb + assetBundlePaths[i];
            }

            assetBundlePaths.Add(info.AssetBundlePath);

            int loadedCount = 0;
            foreach (string assetBundlePath in assetBundlePaths)
            {
                AssetBundleLoader loader;
                if (assetBundleLoaders.ContainsKey(assetBundlePath))
                {
                    loader = assetBundleLoaders[assetBundlePath];
                    loader.AddParentBundle(info.AssetBundlePath);
                }
                else
                {
                    loader = new AssetBundleLoader();
                    loader.Init(assetName, assetBundlePath);
                    loader.AddParentBundle(info.AssetBundlePath);

                    assetBundleLoaders.Add(assetBundlePath, loader);
                }
                //同步清理异步
                if (loader.AssetBundleLoadRequest?.assetBundle)
                {
                    loader.AssetBundleLoadRequest?.assetBundle.Unload(false);
                }
                
                loader.Load();
                
                ++loadedCount;
                if (loadedCount == assetBundlePaths.Count)
                {
                    loader.Expand();
                }
            }

            result = GetAssetBundle(info.AssetBundlePath);
            return result;
        }

        /// <summary>
        /// 通过资产捆绑路径异步加载。
        /// 如果重复加载资产，则将直接从资源池中提供。
        /// </summary>
        /// <param name="assetName">资产名称。</param>
        /// <param name="info">资产信息。</param>
        /// <param name="loadCallback">异步加载完成的回调。</param>
        public void LoadAsync(
            string assetName,
            AssetManager.AssetInfo info,
            AssetBundleLoader.OnLoadFinished loadCallback = null)
        {
            List<string> assetBundlePaths = new List<string>(GetDependenciedAssetBundles(info.AbName));

            for (int i = 0; i < assetBundlePaths.Count; i++)
            {
                assetBundlePaths[i] = info.AssetBundlePathWithoutAb + assetBundlePaths[i];
            }
            AssetBundleLoader lastLoader = null;
            assetBundlePaths.Add(info.AssetBundlePath);
            int loadedCount = 0;
            foreach (string assetBundlePath in assetBundlePaths)
            {
                AssetBundleLoader loader;
                if (assetBundleLoaders.ContainsKey(assetBundlePath))
                {
                    loader = assetBundleLoaders[assetBundlePath];
                    loader.AddParentBundle(info.AssetBundlePath);
                }
                else
                {
                    loader = new AssetBundleLoader();
                    loader.Init(assetName, assetBundlePath);
                    loader.AddParentBundle(info.AssetBundlePath);

                    assetBundleLoaders.Add(assetBundlePath, loader);
                }
                if (assetBundlePath == info.AssetBundlePath)
                {
                    for (int i = 0; i < assetBundlePaths.Count; i++)
                    {
                        loader.AddDependentNames(assetBundlePaths[i]);
                    }
                }
                lastLoader = loader; // 获取最后一个 loader
                loader.LoadAsync(
                    (ab) => {
                        ++loadedCount;
                        lastLoader.AddDependentNames(assetBundlePath, true);
                        if (loadedCount == assetBundlePaths.Count)
                        {
                            // 所有依赖项加载完成后，加载主资源
                            lastLoader.ExpandAsync(() =>
                            {
                                // 主资源加载完成后，如果需要展开，则在展开完成后回调
                                loadCallback?.Invoke(GetAssetBundle(info.AssetBundlePath));
                            });
                        }
                    }
                );
            }
        }

        public IEnumerator LoadAsyncCoroutine(string assetName, AssetManager.AssetInfo info)
        {
            List<string> assetBundlePaths = new List<string>(GetDependenciedAssetBundles(info.AbName));

            for (int i = 0; i < assetBundlePaths.Count; i++)
            {
                assetBundlePaths[i] = info.AssetBundlePathWithoutAb + assetBundlePaths[i];
            }
            AssetBundleLoader lastLoader = null;
            assetBundlePaths.Add(info.AssetBundlePath);
            int loadedCount = 0;
            foreach (string assetBundlePath in assetBundlePaths)
            {
                AssetBundleLoader loader;
                if (assetBundleLoaders.ContainsKey(assetBundlePath))
                {
                    loader = assetBundleLoaders[assetBundlePath];
                    loader.AddParentBundle(info.AssetBundlePath);
                }
                else
                {
                    loader = new AssetBundleLoader();
                    loader.Init(assetName, assetBundlePath);
                    loader.AddParentBundle(info.AssetBundlePath);

                    assetBundleLoaders.Add(assetBundlePath, loader);
                }
                if (assetBundlePath == info.AssetBundlePath)
                {
                    for (int i = 0; i < assetBundlePaths.Count; i++)
                    {
                        loader.AddDependentNames(assetBundlePaths[i]);
                    }
                }
                lastLoader = loader; // 获取最后一个 loader
                yield return loader.LoadAsyncCoroutine();
                ++loadedCount;
                lastLoader.AddDependentNames(assetBundlePath, true);
                if (loadedCount == assetBundlePaths.Count)
                {
                    // 所有依赖项加载完成后，加载主资源
                    yield return lastLoader.ExpandAsyncCoroutine();
                    yield break;
                }
            }
        }
        
        /// <summary>
        /// 通过资源包路径同步卸载。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <param name="unloadAllRelated">
        /// 如果设置为 true，将卸载目标依赖的所有资源，
        /// 否则只卸载目标资源本身。
        /// </param>
        public void Unload(
            string assetBundlePath,
            bool unloadAllRelated = false
        ){
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                bool isClearNeeded = 
                    unloadAllRelated ||
                    loader.IsLastParentBundle(assetBundlePath);
                
                loader.RemoveParentBundle(assetBundlePath);
                loader.RemoveDependentNames(assetBundlePath);
                loader.Unload(isClearNeeded);
            }
        }

        /// <summary>
        /// 通过现有的资源包加载器同步卸载。
        /// </summary>
        /// <param name="loader">用于卸载的资源包加载器。</param>
        /// <param name="unloadAllRelated">
        /// 如果设置为 true，将卸载目标依赖的所有资源，
        /// 否则只卸载目标资源本身。
        /// </param>
        public void Unload(
            AssetBundleLoader loader,
            bool unloadAllRelated = false
        ){
            if (loader == null)
                return;

            if (assetBundleLoaders.ContainsValue(loader))
            {
                List<string> keys = new List<string>();
                foreach (var kv in assetBundleLoaders)
                {
                    if (kv.Value == loader)
                    {
                        keys.Add(kv.Key);
                    }
                }

                foreach (string key in keys)
                {
                    Unload(key, unloadAllRelated);
                }
            }
            else
            {
                loader.Clear();
            }
        }

        /// <summary>
        /// 通过已加载的资源包同步卸载。
        /// </summary>
        /// <param name="ab">已加载的资源包。</param>
        /// <param name="unloadAllRelated">
        /// 如果设置为 true，将卸载目标依赖的所有资源，
        /// 否则只卸载目标资源本身。
        /// </param>
        public void Unload(
            AssetBundle ab,
            bool unloadAllRelated = false
        ){
            if (ab == null)
                return;

            List<string> keys = new List<string>();
            foreach (var kv in assetBundleLoaders)
            {
                if (kv.Value.AssetBundleContent == ab)
                {
                    keys.Add(kv.Key);
                }
            }

            foreach (string key in keys)
            {
                Unload(key, unloadAllRelated);
            }

            if (keys.Count == 0 && ab != null)
            {
                ab.Unload(unloadAllRelated);
            }
        }

        /// <summary>
        /// 通过资源包路径异步卸载。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <param name="unloadAllRelated">
        /// 如果设置为 true，将卸载目标依赖的所有资源，
        /// 否则只卸载目标资源本身。
        /// </param>
        /// <param name="callback">异步卸载完成时的回调函数。</param>
        public void UnloadAsync(
            string assetBundlePath,
            bool unloadAllRelated = false,
            AssetBundleLoader.OnUnloadFinished callback = null
        ){
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            int unloadedCount = 0;

            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                bool isClearNeeded =
                    unloadAllRelated ||
                    loader.IsLastParentBundle(assetBundlePath);

                loader.UnloadAsync(
                    isClearNeeded,
                    () => {
                        ++unloadedCount;
                        if (unloadedCount == bundleLoaders.Count)
                        {
                            foreach (AssetBundleLoader l in bundleLoaders)
                            {
                                l.RemoveParentBundle(assetBundlePath);
                                l.RemoveDependentNames(assetBundlePath);
                            }

                            callback?.Invoke();
                        }
                    }
                );
            }
        }

        /// <summary>
        /// 通过现有的资源包加载器异步卸载。
        /// </summary>
        /// <param name="loader">用于卸载的资源包加载器。</param>
        /// <param name="unloadAllRelated">
        /// 如果设置为 true，将卸载目标依赖的所有资源，
        /// 否则只卸载目标资源本身。
        /// </param>
        /// <param name="callback">异步卸载完成时的回调函数。</param>
        public void UnloadAsync(
            AssetBundleLoader loader,
            bool unloadAllRelated = false,
            AssetBundleLoader.OnUnloadFinished callback = null
        ){
            if (loader == null)
                return;

            if (assetBundleLoaders.ContainsValue(loader))
            {
                List<string> keys = new List<string>();
                foreach (var kv in assetBundleLoaders)
                {
                    if (kv.Value == loader)
                    {
                        keys.Add(kv.Key);
                    }
                }

                foreach (string key in keys)
                {
                    UnloadAsync(key, unloadAllRelated, callback);
                }
            }
            else
            {
                loader.UnloadAsync(unloadAllRelated, callback);
            }
        }

        /// <summary>
        /// 通过已加载的资源包异步卸载。
        /// </summary>
        /// <param name="ab">已加载的资源包。</param>
        /// <param name="unloadAllRelated">
        /// 如果设置为 true，将卸载目标依赖的所有资源，
        /// 否则只卸载目标资源本身。
        /// </param>
        /// <param name="callback">异步卸载完成时的回调函数。</param>
        public void UnloadAsync(
            AssetBundle ab,
            bool unloadAllRelated = false,
            AssetBundleLoader.OnUnloadFinished callback = null
        ){
            if (ab == null)
                return;

            List<string> keys = new List<string>();
            foreach (var kv in assetBundleLoaders)
            {
                if (kv.Value.AssetBundleContent == ab)
                {
                    keys.Add(kv.Key);
                }
            }

            foreach (string key in keys)
            {
                UnloadAsync(key, unloadAllRelated, callback);
            }

            if (keys.Count <= 0 && ab != null)
            {
                AsyncOperation op = ab.UnloadAsync(unloadAllRelated);
                if (op != null && callback != null)
                    op.completed +=
                        (op) => callback();
            }
        }

        /// <summary>
        /// 通过资源包路径获取资源包加载器。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <returns>找到的资源包加载器。</returns>
        public AssetBundleLoader GetAssetBundleLoader(string assetBundlePath)
        {
            if (assetBundleLoaders.TryGetValue(assetBundlePath, out AssetBundleLoader loader))
                return loader;

            return null;
        }

        /// <summary>
        /// 通过资源包路径获取已加载的资源包。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <returns>找到的资源包。</returns>
        public AssetBundle GetAssetBundle(string assetBundlePath)
        {
            if (IsLoadFinished(assetBundlePath))
            {
                if (assetBundleLoaders.TryGetValue(assetBundlePath, out AssetBundleLoader loader))
                    return loader.AssetBundleContent;
            }

            return null;
        }

        /// <summary>
        /// 通过资产捆绑加载程序和对象名称获取资产对象。
        /// </summary>
        /// <typeparam name="T">资产对象的目标对象类型。</typeparam>
        /// <param name="assetPath">资产对象的路径。</param>
        /// <returns>找到的资产对象。</returns>
        public T GetAssetObject<T>(string assetPath)
            where T : Object
        {
            if (assetPath == null)
                return null;

            foreach (var kv in assetBundleLoaders)
            {
                AssetBundleLoader loader = kv.Value;
                if (loader != null &&
                    loader.IsLoadFinished &&
                    loader.IsExpandFinished)
                {
                    bool success = loader.TryGetAsset(assetPath, out Object obj);
                    if (success)
                    {
                        if (obj is T t)
                            return t;
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 通过资产捆绑加载程序和对象名称获取资产对象。
        /// </summary>
        /// <param name="assetPath">资产对象的路径。</param>
        /// <param name="assetType">资产对象的目标对象类型。</param>
        /// <returns>找到的资产对象。</returns>
        public Object GetAssetObject(string assetPath, System.Type assetType)
        {
            if (assetPath == null ||
                assetType == null)
                return null;

            foreach (var kv in assetBundleLoaders)
            {
                AssetBundleLoader loader = kv.Value;
                if (loader != null &&
                    loader.IsLoadFinished &&
                    loader.IsExpandFinished)
                {
                    bool success = loader.TryGetAsset(assetPath, out Object obj);
                    if (success)
                    {
                        if (assetType.IsAssignableFrom(obj.GetType()))
                            return obj;
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 通过资产捆绑加载程序和对象名称获取资产对象。
        /// </summary>
        /// <param name="assetPath">资产对象的路径。</param>
        /// <returns>找到的资产对象。</returns>
        public Object GetAssetObject(string assetPath)
        {
            if (assetPath == null)
                return null;

            foreach (var kv in assetBundleLoaders)
            {
                AssetBundleLoader loader = kv.Value;
                if (loader != null &&
                    loader.IsLoadFinished &&
                    loader.IsExpandFinished)
                {
                    bool success = loader.TryGetAsset(assetPath, out Object obj);
                    if (success)
                        return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有加载器的加载进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <returns>加载进度。</returns>
        public float GetLoadProgress()
        {
            float allProgress = 0f;
            int cnt = assetBundleLoaders.Values.Count;
            if (cnt == 0)
            {
                return -1f;
            }

            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
            {
                allProgress += loader.LoadProgress;
            }

            float average = allProgress / cnt;
            return average;
        }

        /// <summary>
        /// 获取所有加载器的扩展进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <returns>加载进度。</returns>
        public float GetExpandProgress()
        {
            float allProgress = 0f;
            int cnt = assetBundleLoaders.Values.Count;
            if (cnt == 0)
            {
                return -1f;
            }

            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
            {
                allProgress += loader.ExpandProgress;
            }

            float average = allProgress / cnt;
            return average;
        }

        /// <summary>
        /// 获取所有加载器的卸载进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <returns>卸载进度。</returns>
        public float GetUnloadProgress()
        {
            int cnt = assetBundleLoaders.Values.Count;
            if (cnt == 0)
            {
                return -1f;
            }

            float allProgress = 0f;
            bool hasUnloadLoader = false;
            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
            {
                if (loader.IsUnloadCalled)
                {
                    hasUnloadLoader = true;
                    allProgress += loader.UnloadProgress;
                }
            }

            if (hasUnloadLoader)
            {
                float average = allProgress / cnt;
                return average;
            }
            else
            {
                return 1f;
            }
        }

        /// <summary>
        /// 通过资源包路径获取加载器的加载进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <returns>加载进度。</returns>
        public float GetLoadProgress(string assetBundlePath)
        {
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            if (bundleLoaders.Count == 0)
            {
                return -1f;
            }
            
            float allProgress = 0f;
            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                allProgress += loader.LoadProgress;
            }

            float average = allProgress / bundleLoaders.Count;
            return average;
        }

        /// <summary>
        /// 通过资源包路径获取加载器的扩展进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <returns>加载进度。</returns>
        public float GetExpandProgress(string assetBundlePath)
        {
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            if (bundleLoaders.Count == 0)
            {
                return -1f;
            }

            float allProgress = 0f;
            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                allProgress += loader.ExpandProgress;
            }

            float average = allProgress / bundleLoaders.Count;
            return average;
        }

        /// <summary>
        /// 通过资源包路径获取加载器的卸载进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <returns>卸载进度。</returns>
        public float GetUnloadProgress(string assetBundlePath)
        {
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            int cnt = bundleLoaders.Count;
            if (cnt == 0)
            {
                return -1f;
            }

            float allProgress = 0f;
            bool hasUnloadLoader = false;
            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                if (loader.IsUnloadCalled)
                {
                    hasUnloadLoader = true;
                    allProgress += loader.UnloadProgress;
                }
            }

            if (hasUnloadLoader)
            {
                float average = allProgress / cnt;
                return average;
            }
            else
            {
                return 1f;
            }
        }

        /// <summary>
        /// 查询所有资源包是否已加载完成。
        /// </summary>
        /// <returns>
        /// 如果所有加载已完成，则返回 true，
        /// 否则返回 false。
        /// </returns>
        public bool IsLoadFinished()
        {
            bool allLoaderFinished = true;
            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
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
        /// 查询所有资产对象是否已加载完成。
        /// </summary>
        /// <returns>
        /// 如果所有加载已完成，则返回 true，
        /// 否则返回 false。
        /// </returns>
        public bool IsExpandFinished()
        {
            bool allLoaderFinished = true;
            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
            {
                if (!loader.IsExpandFinished)
                {
                    allLoaderFinished = false;
                    break;
                }
            }

            return allLoaderFinished;
        }

        /// <summary>
        /// 查询所有资源包是否已卸载完成。
        /// </summary>
        /// <returns>
        /// 如果所有卸载已完成，则返回 true，
        /// 否则返回 false。
        /// </returns>
        public bool IsUnloadFinished()
        {
            bool allLoaderFinished = true;
            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
            {
                if (loader.IsUnloadFinished)
                {
                    allLoaderFinished = false;
                    break;
                }
            }

            return allLoaderFinished;
        }

        /// <summary>
        /// 查询指定的资源包是否已加载完成。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <returns>
        /// 如果指定的加载已完成，则返回 true，
        /// 否则返回 false。
        /// </returns>
        public bool IsLoadFinished(string assetBundlePath)
        {
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            if (bundleLoaders.Count == 0)
                return false;

            bool allLoaderFinished = true;
            foreach (AssetBundleLoader loader in bundleLoaders)
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
        /// 查询指定的资产对象是否已加载完成。
        /// </summary>
        /// <param name="assetBundlePath">资产对象的路径。</param>
        /// <returns>
        /// 如果指定的加载已完成，则返回 true，
        /// 否则返回 false。
        /// </returns>
        public bool IsExpandFinished(string assetBundlePath)
        {
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            if (bundleLoaders.Count == 0)
                return false;

            bool allLoaderFinished = true;
            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                if (!loader.IsExpandFinished)
                {
                    allLoaderFinished = false;
                    break;
                }
            }

            return allLoaderFinished;
        }

        /// <summary>
        /// 查询指定的资源包是否已卸载完成。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <returns>
        /// 如果指定的卸载已完成，则返回 true，
        /// 否则返回 false。
        /// </returns>
        public bool IsUnloadFinished(string assetBundlePath)
        {
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            if (bundleLoaders.Count == 0)
                return true;

            bool allLoaderFinished = true;
            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                if (loader.IsUnloadFinished)
                {
                    allLoaderFinished = false;
                    break;
                }
            }

            return allLoaderFinished;
        }

        public static string GetAssetBundleCompletePath(string regPath = null)
        {
            string persistentABFullPath
                = AssetBundleHelper.GetAssetBundleFullName(regPath, AssetBundleHelper.SourceType.PERSISTENT_DATA_PATH);

            if (File.Exists(persistentABFullPath))
                return persistentABFullPath;
            else
                return AssetBundleHelper.GetAssetBundleFullName(regPath, AssetBundleHelper.SourceType.STREAMING_ASSETS);
        }
        
        public static string GetRemoteAssetBundleCompletePath(string regPath = null)
        {
            if (!string.IsNullOrEmpty(URLSetting.REMOTE_ADDRESS))
            {
                string remoteABFullPath
                    = AssetBundleHelper.GetAssetBundleFullName(regPath, AssetBundleHelper.SourceType.REMOTE_ADDRESS);
                return remoteABFullPath;
            }
            return null;
        }
        
        private string[] GetDependenciedAssetBundles(string abName)
        {
            if (manifest == null)
                return new string[] {};
            return manifest.GetAllDependencies(abName);
        }

        private List<AssetBundleLoader> GetRelatedLoaders(string assetBundlePath)
        {
            List<AssetBundleLoader> result = new List<AssetBundleLoader>();

            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
            {
                if (loader.IsParentBundle(assetBundlePath))
                {
                    result.Add(loader);
                }
            }
            return result;
        }

        private void Clear()
        {
            foreach (var loader in assetBundleLoaders.Values)
            {
                loader.Clear(true);
            }

            assetBundleLoaders.Clear();
        }

        public void OnInit(object createParam)
        {
#if UNITY_WEBGL
            string manifestPath = AssetBundleHelper.GetAssetBundleManifestPath();
            if (manifestPath == null)
                return;
    #if UNITY_EDITOR
            manifestPath = "file://" + manifestPath;
    #endif
            downloadManifest = new DownloadRequest(manifestPath, 0);
#else
            string manifestPath = AssetBundleHelper.GetAssetBundleManifestPath();
            if (manifestPath == null)
                return;
            manifest = AssetBundle.LoadFromFile(manifestPath)?.
                LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifest.GetAllAssetBundles();
#endif
        }
        
        public void OnUpdate()
        {
#if UNITY_WEBGL
            if (isDownloadManifest == false)
            {
                if (downloadManifest != null && downloadManifest.IsFinished)
                {
                    isDownloadManifest = true;
                    manifest = downloadManifest.DownloadedAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    manifest.GetAllAssetBundles();
                }
            }
#endif
            foreach (AssetBundleLoader loader in assetBundleLoaders.Values)
            {
                loader.OnUpdate();
            }
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