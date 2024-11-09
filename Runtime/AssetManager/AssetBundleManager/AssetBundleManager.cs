using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// 通过资产捆绑路径同步加载。
        /// 如果重复加载资产，则将直接从资源池中提供。
        /// </summary>
        /// <param name="assetName">资产名称。</param>
        /// <param name="assetType">资产类型。</param>
        /// <param name="info">资产信息。</param>
        /// <returns>要完成扩展的对象列表。</returns>
        public AssetBundle Load(string assetName, System.Type assetType, ref AssetManager.AssetInfo info)
        {
            AssetBundle result;

            List<string> assetBundlePaths = new List<string>(GetDependenciedAssetBundles(info.AbName));
            
            for (int i = 0; i < assetBundlePaths.Count; i++)
            {
                assetBundlePaths[i] = GetAssetBundlePathWithoutAbByAbName(assetBundlePaths[i]) + assetBundlePaths[i];
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
                    loader.Init(assetBundlePath);
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
                    loader.Expand(assetType);
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
        /// <param name="assetType">资产类型。</param>
        /// <param name="info">资产信息。</param>
        /// <param name="loadCallback">异步加载完成的回调。</param>
        public void LoadAsync(
            string assetName,
            System.Type assetType,
            AssetManager.AssetInfo info,
            AssetBundleLoader.OnLoadFinished loadCallback = null)
        {
            List<string> assetBundlePaths = new List<string>(GetDependenciedAssetBundles(info.AbName));

            for (int i = 0; i < assetBundlePaths.Count; i++)
            {
                assetBundlePaths[i] = GetAssetBundlePathWithoutAbByAbName(assetBundlePaths[i]) + assetBundlePaths[i];
            }
            AssetBundleLoader lastLoader = null;
            assetBundlePaths.Add(info.AssetBundlePath);
            int loadedCount = 0;
            int endIndex = assetBundlePaths.Count - 1;
            for (int i = endIndex; i >= 0; i--)
            {
                string assetBundlePath = assetBundlePaths[i];
                AssetBundleLoader loader;
                if (assetBundleLoaders.ContainsKey(assetBundlePath))
                {
                    loader = assetBundleLoaders[assetBundlePath];
                    loader.AddParentBundle(info.AssetBundlePath);
                }
                else
                {
                    loader = new AssetBundleLoader();
                    loader.Init(assetBundlePath);
                    loader.AddParentBundle(info.AssetBundlePath);
                    
                    assetBundleLoaders.Add(assetBundlePath, loader);
                }
                if (lastLoader == null)
                {
                    lastLoader = loader; // 获取最后一个 loader
                    for (int j = 0; j < assetBundlePaths.Count; j++)
                    {
                        loader.AddDependentNames(assetBundlePaths[j]);
                    }
                }
                loader.LoadAsync(
                    (ab) => {
                        ++loadedCount;
                        lastLoader.AddDependentNames(assetBundlePath, true);
                        if (loadedCount == assetBundlePaths.Count)
                        {
                            // 所有依赖项加载完成后，加载主资源
                            lastLoader.ExpandAsync(assetType, () =>
                            {
                                // 主资源加载完成后，如果需要展开，则在展开完成后回调
                                loadCallback?.Invoke(GetAssetBundle(info.AssetBundlePath));
                            });
                        }
                    }
                );
            }
        }

        public IEnumerator LoadAsyncCoroutine(string assetName, System.Type assetType, AssetManager.AssetInfo info)
        {
            List<string> assetBundlePaths = new List<string>(GetDependenciedAssetBundles(info.AbName));

            for (int i = 0; i < assetBundlePaths.Count; i++)
            {
                assetBundlePaths[i] = GetAssetBundlePathWithoutAbByAbName(assetBundlePaths[i]) + assetBundlePaths[i];
            }
            AssetBundleLoader lastLoader = null;
            assetBundlePaths.Add(info.AssetBundlePath);
            int endIndex = assetBundlePaths.Count - 1;
            for (int i = endIndex; i >= 0; i--)
            {
                string assetBundlePath = assetBundlePaths[i];
                AssetBundleLoader loader;
                if (assetBundleLoaders.ContainsKey(assetBundlePath))
                {
                    loader = assetBundleLoaders[assetBundlePath];
                    loader.AddParentBundle(info.AssetBundlePath);
                }
                else
                {
                    loader = new AssetBundleLoader();
                    loader.Init(assetBundlePath);
                    loader.AddParentBundle(info.AssetBundlePath);

                    assetBundleLoaders.Add(assetBundlePath, loader);
                }
                if (lastLoader == null)
                {
                    lastLoader = loader; // 获取最后一个 loader
                    for (int j = 0; j < assetBundlePaths.Count; j++)
                    {
                        loader.AddDependentNames(assetBundlePaths[j]);
                    }
                }
                loader.LoadAsync((ab) =>
                {
                    lastLoader.AddDependentNames(assetBundlePath, true);
                });
            }
            
            yield return new WaitUntil(() =>
            {
                int finishedCount = 0;
                for (int j = 0; j < assetBundlePaths.Count; j++)
                {
                    if (!assetBundleLoaders.TryGetValue(assetBundlePaths[j], out AssetBundleLoader value)) continue;
                    if (value.IsLoadFinished)
                    {
                        finishedCount += 1;
                    }
                }
                return finishedCount > endIndex;
            });
            
            yield return lastLoader!.ExpandAsyncCoroutine(assetType);
        }
        
        /// <summary>
        /// 通过资源包路径同步卸载。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <param name="unloadAllLoadedObjects">
        /// 完全卸载。
        /// </param>
        public void Unload(
            string assetBundlePath,
            bool unloadAllLoadedObjects = false
        ){
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                if (unloadAllLoadedObjects)
                {
                    loader.RemoveParentBundle(assetBundlePath);
                    loader.RemoveDependentNames(assetBundlePath);
                }
                loader.Unload(unloadAllLoadedObjects);
            }
        }

        /// <summary>
        /// 通过现有的资源包加载器同步卸载。
        /// </summary>
        /// <param name="loader">用于卸载的资源包加载器。</param>
        /// <param name="unloadAllLoadedObjects">
        /// 完全卸载。
        /// </param>
        public void Unload(
            AssetBundleLoader loader,
            bool unloadAllLoadedObjects = false
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
                    Unload(key, unloadAllLoadedObjects);
                }
            }
            else
            {
                loader.Clear(unloadAllLoadedObjects);
            }
        }

        /// <summary>
        /// 通过已加载的资源包同步卸载。
        /// </summary>
        /// <param name="ab">已加载的资源包。</param>
        /// <param name="unloadAllLoadedObjects">
        /// 完全卸载。
        /// </param>
        public void Unload(
            AssetBundle ab,
            bool unloadAllLoadedObjects = false
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
                Unload(key, unloadAllLoadedObjects);
            }

            if (keys.Count == 0 && ab != null)
            {
                ab.Unload(unloadAllLoadedObjects);
            }
        }

        /// <summary>
        /// 通过资源包路径异步卸载。
        /// </summary>
        /// <param name="assetBundlePath">资源包的路径。</param>
        /// <param name="unloadAllLoadedObjects">
        /// 完全卸载。
        /// </param>
        /// <param name="callback">异步卸载完成时的回调函数。</param>
        public void UnloadAsync(
            string assetBundlePath,
            bool unloadAllLoadedObjects = false,
            AssetBundleLoader.OnUnloadFinished callback = null
        ){
            List<AssetBundleLoader> bundleLoaders = GetRelatedLoaders(assetBundlePath);
            int unloadedCount = 0;

            foreach (AssetBundleLoader loader in bundleLoaders)
            {
                loader.UnloadAsync(
                    unloadAllLoadedObjects,
                    () => {
                        ++unloadedCount;
                        if (unloadedCount == bundleLoaders.Count)
                        {
                            foreach (AssetBundleLoader l in bundleLoaders)
                            {
                                if (unloadAllLoadedObjects)
                                {
                                    l.RemoveParentBundle(assetBundlePath);
                                    l.RemoveDependentNames(assetBundlePath);
                                }
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
        /// <param name="unloadAllLoadedObjects">
        /// 完全卸载。
        /// </param>
        /// <param name="callback">异步卸载完成时的回调函数。</param>
        public void UnloadAsync(
            AssetBundleLoader loader,
            bool unloadAllLoadedObjects = false,
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
                    UnloadAsync(key, unloadAllLoadedObjects, callback);
                }
            }
            else
            {
                loader.UnloadAsync(unloadAllLoadedObjects, callback);
            }
        }

        /// <summary>
        /// 通过已加载的资源包异步卸载。
        /// </summary>
        /// <param name="ab">已加载的资源包。</param>
        /// <param name="unloadAllLoadedObjects">
        /// 完全卸载。
        /// </param>
        /// <param name="callback">异步卸载完成时的回调函数。</param>
        public void UnloadAsync(
            AssetBundle ab,
            bool unloadAllLoadedObjects = false,
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
                UnloadAsync(key, unloadAllLoadedObjects, callback);
            }

            if (keys.Count <= 0 && ab != null)
            {
                AsyncOperation op = ab.UnloadAsync(unloadAllLoadedObjects);
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
        /// <param name="assetBundlePath">assetBundle路径。</param>
        /// <param name="assetPath">assetPath名。（小写）</param>
        /// <returns>找到的资产对象。</returns>
        public T GetAssetObject<T>(string assetBundlePath, string assetPath)
            where T : Object
        {
            if (assetBundleLoaders.TryGetValue(assetBundlePath, out AssetBundleLoader loader))
            {
                if (loader != null &&
                    loader.IsLoadFinished &&
                    loader.IsExpandFinished)
                {
                    bool success = loader.TryGetAsset(assetPath, out Object obj);
                    if (success)
                    {
                        return obj as T;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 通过资产捆绑加载程序和对象名称获取资产对象。
        /// </summary>
        /// <param name="assetBundlePath">assetBundle路径。</param>
        /// <param name="assetPath">assetPath名。（小写）</param>
        /// <param name="assetType">资产对象的目标对象类型。</param>
        /// <returns>找到的资产对象。</returns>
        public Object GetAssetObject(string assetBundlePath, string assetPath, System.Type assetType = default)
        {
            if (assetBundleLoaders.TryGetValue(assetBundlePath, out AssetBundleLoader loader))
            {
                if (loader != null &&
                    loader.IsLoadFinished &&
                    loader.IsExpandFinished)
                {
                    bool success = loader.TryGetAsset(assetPath, out Object obj);
                    if (success)
                    {
                        return obj;
                    }
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

        /// <summary>
        /// 获取没有ab名的路径，传入assetName
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string GetAssetBundlePathWithoutAb(string assetName)
        {
            string fullPath;
            
            if (GameConfig.LocalGameVersion.EnableHotUpdate &&
                AssetBundleMap.Mappings.TryGetValue(assetName, out AssetBundleMap.AssetMapping assetMapping) &&
                assetMapping != null &&
                !string.IsNullOrEmpty(assetMapping.Updated))
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.HOT_UPDATE_PATH);
            }
            else if (GameConfig.LocalGameVersion.EnablePackage &&
                     AssetBundleMap.Mappings.TryGetValue(assetName, out AssetBundleMap.AssetMapping assetMappingPackage) &&
                     assetMappingPackage != null &&
                     !string.IsNullOrEmpty(assetMappingPackage.Package))
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.PACKAGE_PATH);
            }
            else if (AssetManager.ForceRemoteAssetBundle)
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.REMOTE_ADDRESS);
            }
            else
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.STREAMING_ASSETS);
            }
            
            return fullPath;
        }
        
        /// <summary>
        /// 获取没有ab名的路径，传入abName
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static string GetAssetBundlePathWithoutAbByAbName(string abName)
        {
            string fullPath;
            
            if (GameConfig.LocalGameVersion.EnableHotUpdate && File.Exists(AssetBundleHelper.GetAssetBundleFullName(abName, AssetBundleHelper.SourceType.HOT_UPDATE_PATH)))
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.HOT_UPDATE_PATH);
            }
            else if (GameConfig.LocalGameVersion.EnablePackage && File.Exists(AssetBundleHelper.GetAssetBundleFullName(abName, AssetBundleHelper.SourceType.PACKAGE_PATH)))
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.PACKAGE_PATH);
            }
            else if (AssetManager.ForceRemoteAssetBundle)
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.REMOTE_ADDRESS);
            }
            else
            {
                fullPath = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.STREAMING_ASSETS);
            }
            
            return fullPath;
        }
        
        public static string GetRemoteAssetBundleCompletePath()
        {
            if (!string.IsNullOrEmpty(GameConfig.LocalGameVersion.AssetRemoteAddress))
            {
                string remoteABFullPath
                    = AssetBundleHelper.GetAssetBundleFullName(null, AssetBundleHelper.SourceType.REMOTE_ADDRESS);
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

        public Hash128 GetAssetBundleHash(string abName)
        {
            if (manifest == null)
                return default(Hash128);
            return manifest.GetAssetBundleHash(abName);
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

        // WebGL专用异步加载AssetBundleManifest
        public IEnumerator LoadAssetBundleManifest()  
        {
            string manifestPath = AssetBundleHelper.GetAssetBundleManifestPath();
            if (manifestPath == null)
                yield break;
#if UNITY_EDITOR
                manifestPath = "file://" + manifestPath;
#endif
            DownloadRequest assetBundleDownloadRequest = new DownloadRequest(manifestPath, default);
            yield return assetBundleDownloadRequest.SendAssetBundleDownloadRequestCoroutine(manifestPath);
            manifest = assetBundleDownloadRequest.DownloadedAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifest.GetAllAssetBundles();
            assetBundleDownloadRequest.DownloadedAssetBundle.Unload(false);
        }
        
        public void OnInit(object createParam)
        {
#if UNITY_WEBGL
            LogF8.LogAsset("（提示）由于WebGL异步加载AssetBundleManifest，请在创建资产模块之后加上：yield return AssetBundleManager.Instance.LoadAssetBundleManifest();");
#else
            string manifestPath = AssetBundleHelper.GetAssetBundleManifestPath(AssetBundleHelper.SourceType.STREAMING_ASSETS);
            if (manifestPath == null)
                return;
            var assetBundle = AssetBundle.LoadFromFile(manifestPath);
            if (assetBundle)
            {
                manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                manifest.GetAllAssetBundles();
                assetBundle.Unload(false);
            }
#endif
        }
        
        public void OnUpdate()
        {
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