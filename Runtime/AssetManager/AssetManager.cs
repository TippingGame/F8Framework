using F8Framework.AssetMap;
using UnityEngine;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    //异步加载完成的回调
    public delegate void OnAssetObject<T>(T obj)
        where T : Object;
    
    public class AssetManager : Singleton<AssetManager>
    {
        //资产信息
        public class AssetInfo
        {
            //目标资产类型
            public readonly AssetTypeEnum AssetType;

            //直接资产请求路径相对路径，Assets开头的
            public readonly string AssetPath;

            //直接资产捆绑请求路径（仅适用于资产捆绑类型），完全路径
            public readonly string AssetBundlePath;
            
            public AssetInfo(
                AssetTypeEnum assetType,
                string assetPath,
                string assetBundlePath)
            {
                AssetType = assetType;
                AssetPath = assetPath;
                AssetBundlePath = assetBundlePath;
            }

            //如果信息合法，则该值为真
            public bool IsLegal
            {
                get
                {
                    if (AssetType == AssetTypeEnum.NONE)
                        return false;

                    if (AssetType == AssetTypeEnum.RESOURCE &&
                        AssetPath == null)
                        return false;

                    if (AssetType == AssetTypeEnum.ASSET_BUNDLE &&
                        (AssetPath == null || AssetBundlePath == null))
                        return false;

                    return true;
                }
            }
        }
             //资产访问标志
            [System.Flags]
            public enum AssetAccessMode
            {
                NONE = 0b1,
                UNKNOWN = 0b10,
                RESOURCE = 0b100,
                ASSET_BUNDLE = 0b1000,
                REMOTE_ASSET_BUNDLE = 0b10000
            }

            //资产类型
            public enum AssetTypeEnum
            {
                NONE,
                RESOURCE,
                ASSET_BUNDLE
            }

            /// <summary>
            /// 根据提供的资产路径和访问选项推断资产类型。
            /// </summary>
            /// <param name="assetPath">资产路径字符串。</param>
            /// <param name="accessMode">访问模式。</param>
            /// <returns>资产信息。</returns>
            public AssetInfo GetAssetInfo(string assetPath,
                AssetAccessMode accessMode = AssetAccessMode.UNKNOWN)
            {
                if (accessMode.HasFlag(AssetAccessMode.RESOURCE))
                {
                    return GetAssetInfoFromResource(assetPath);
                }
                else if (accessMode.HasFlag(AssetAccessMode.ASSET_BUNDLE))
                {
                    return GetAssetInfoFromAssetBundle(assetPath);
                }
                else if (accessMode.HasFlag(AssetAccessMode.UNKNOWN))
                {
                    AssetInfo r = GetAssetInfoFromAssetBundle(assetPath);
                    if (r != null && r.IsLegal)
                        return r;
                    else
                        return GetAssetInfoFromResource(assetPath);
                }
                else if (accessMode.HasFlag(AssetAccessMode.REMOTE_ASSET_BUNDLE))
                {
                    AssetInfo r = GetAssetInfoFromAssetBundle(assetPath, true);
                    if (r != null && r.IsLegal)
                        return r;
                    else
                        return GetAssetInfoFromAssetBundle(assetPath);
                }
                return null;
            }

            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <typeparam name="T">目标资产类型。</typeparam>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public T Load<T>(
                string assetName,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
                where T : Object
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (info == null || !info.IsLegal)
                    return null;

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    T o = ResourcesManager.Instance.GetResouceObject<T>(info.AssetPath);
                    if (o != null)
                    {
                        return o;
                    }
                      
                    return ResourcesManager.Instance.Load<T>(info.AssetPath);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, info.AssetBundlePath, true);
                        ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
                
                    T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetPath);
                    if (o != null)
                    {
                        return o;
                    }
                    
                    ab.Expand();
                    return AssetBundleManager.Instance.GetAssetObject<T>(info.AssetPath);
                }

                return null;
            }
            
            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="assetType">目标资产类型。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public Object Load(
                string assetName,
                System.Type assetType,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (info == null || !info.IsLegal)
                    return null;

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    Object o = ResourcesManager.Instance.GetResouceObject(info.AssetPath, assetType);
                    if (o != null)
                    {
                        return o;
                    }

                    return ResourcesManager.Instance.Load(info.AssetPath, assetType);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, info.AssetBundlePath, true);
                        ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
            
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetPath, assetType);
                    if (o != null)
                    {
                        return o;
                    }
                
                    ab.Expand();
                    return AssetBundleManager.Instance.GetAssetObject(info.AssetPath, assetType);
                }

                return null;
            }

            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public Object Load(
                string assetName,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (info == null || !info.IsLegal)
                    return null;

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    Object o = ResourcesManager.Instance.GetResouceObject(info.AssetPath);
                    if (o != null)
                    {
                        return o;
                    }

                    return ResourcesManager.Instance.Load(info.AssetPath);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, info.AssetBundlePath, true);
                        ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
            
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetPath);
                    if (o != null)
                    {
                        return o;
                    }
                
                    ab.Expand();
                    return AssetBundleManager.Instance.GetAssetObject(info.AssetPath);
                }

                return null;
            }
            
            /// <summary>
            /// 异步加载资产对象。
            /// </summary>
            /// <typeparam name="T">目标资产类型。</typeparam>
            /// <param name="assetPath">资产路径字符串。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="mode">访问模式。</param>
            public void LoadAsync<T>(
                string assetPath,
                OnAssetObject<T> callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
                where T : Object
            {
                AssetInfo info = GetAssetInfo(assetPath, mode);
                if (info == null || !info.IsLegal)
                {
                    End();
                    return;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    T o = ResourcesManager.Instance.GetResouceObject<T>(info.AssetPath);
                    if (o != null)
                    {
                        End(o);
                        return;
                    }
                    ResourcesManager.Instance.LoadAsync<T>(info.AssetPath, callback);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.LoadAsync(assetPath, info.AssetBundlePath, true, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject<T>(info.AssetPath));
                        });
                        return;
                    }
                    else
                    {
                        T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetPath);
                        if (o != null)
                        {
                            End(o);
                            return;
                        }
                        ab.Expand();
                        End(AssetBundleManager.Instance.GetAssetObject<T>(info.AssetPath));
                    }
                }

                void End(T o = null)
                {
                    callback?.Invoke(o);
                }
            }

            /// <summary>
            /// 异步加载资产对象。
            /// </summary>
            /// <param name="assetPath">资产路径字符串。</param>
            /// <param name="assetType">目标资产类型。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="mode">访问模式。</param>
            public void LoadAsync(
                string assetPath,
                System.Type assetType,
                OnAssetObject<Object> callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetPath, mode);
                if (info == null || !info.IsLegal)
                {
                    End();
                    return;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    Object o = ResourcesManager.Instance.GetResouceObject(info.AssetPath, assetType);
                    if (o != null)
                    {
                        End(o);
                        return;
                    }
                    ResourcesManager.Instance.LoadAsync(info.AssetPath, assetType, callback);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.LoadAsync(assetPath, info.AssetBundlePath, true, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject(info.AssetPath, assetType));
                        });
                        return;
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetPath, assetType);
                        if (o != null)
                        {
                            End(o);
                            return;
                        }
            
                        ab.Expand();
                        End(AssetBundleManager.Instance.GetAssetObject(info.AssetPath, assetType));
                    }
                }

                void End(Object o = null)
                {
                    callback?.Invoke(o);
                }
            }
            
            /// <summary>
            /// 异步加载资产对象。
            /// </summary>
            /// <param name="assetPath">资产路径字符串。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="mode">访问模式。</param>
            public void LoadAsync(
                string assetPath,
                OnAssetObject<Object> callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetPath, mode);
                if (info == null || !info.IsLegal)
                {
                    End();
                    return;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    Object o = ResourcesManager.Instance.GetResouceObject(info.AssetPath);
                    if (o != null)
                    {
                        End(o);
                        return;
                    }
                    ResourcesManager.Instance.LoadAsync(info.AssetPath, callback);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.LoadAsync(assetPath, info.AssetBundlePath, true, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject(info.AssetPath));
                        });
                        return;
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetPath);
                        if (o != null)
                        {
                            End(o);
                            return;
                        }
            
                        ab.Expand();
                        End(AssetBundleManager.Instance.GetAssetObject(info.AssetPath));
                    }
                }

                void End(Object o = null)
                {
                    callback?.Invoke(o);
                }
            }
            
            
            private AssetInfo GetAssetInfoFromResource(string path)
            {
                if (ResourceMap.Mappings.TryGetValue(path, out string value))
                {
                    return new AssetInfo(AssetTypeEnum.RESOURCE, value, null);
                }
                return null;
            }
            
            private AssetInfo GetAssetInfoFromAssetBundle(string path, bool remote = false)
            {
                if (AssetBundleMap.Mappings.TryGetValue(path, out AssetBundleMap.AssetMapping assetmpping))
                {
                    if (remote)
                    {
                        return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE, assetmpping.AssetPath, AssetBundleManager.GetRemoteAssetBundleCompletePath(assetmpping.AbName));
                    }
                    else
                    {
                        return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE, assetmpping.AssetPath, AssetBundleManager.GetAssetBundleCompletePath(assetmpping.AbName));
                    }
                }
                return null;
            }
            
            /// <summary>
            /// 通过资源名称同步卸载。
            /// </summary>
            /// <param name="assetName">资源名称。</param>
            /// <param name="unloadAllRelated">
            /// 如果设置为 true，将卸载目标依赖的所有资源，
            /// 否则只卸载目标资源本身。
            /// </param>
            public void Unload(string assetName, bool unloadAllRelated = false)
            {
                AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
                if (ab != null && ab.IsLegal)
                {
                    AssetBundleManager.Instance.Unload(ab.AssetBundlePath, unloadAllRelated);
                }
                AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
                if (abRemote != null && abRemote.IsLegal)
                {
                    AssetBundleManager.Instance.Unload(abRemote.AssetBundlePath, unloadAllRelated);
                }
                AssetInfo res = GetAssetInfoFromResource(assetName);
                if (res != null && res.IsLegal)
                {
                    ResourcesManager.Instance.Unload(res.AssetPath);
                }
            }
            
            /// <summary>
            /// 通过资源名称异步卸载。
            /// </summary>
            /// <param name="assetName">资源名称。</param>
            /// <param name="unloadAllRelated">
            /// 如果设置为 true，将卸载目标依赖的所有资源，
            /// 否则只卸载目标资源本身。
            /// </param>
            /// <param name="callback">异步卸载完成时的回调函数。</param>
            public void UnloadAsync(string assetName, bool unloadAllRelated = false, AssetBundleLoader.OnUnloadFinished callback = null)
            {
                AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
                if (ab != null && ab.IsLegal)
                {
                    AssetBundleManager.Instance.UnloadAsync(ab.AssetBundlePath, unloadAllRelated, callback);
                }
                AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
                if (abRemote != null && abRemote.IsLegal)
                {
                    AssetBundleManager.Instance.UnloadAsync(abRemote.AssetBundlePath, unloadAllRelated, callback);
                }
            }
            
            /// <summary>
            /// 通过资源名称获取加载器的加载进度。
            /// 正常值范围从 0 到 1。
            /// 但如果没有加载器，则返回 -1。
            /// </summary>
            /// <param name="assetName">资源名称。</param>
            /// <returns>加载进度。</returns>
            public float GetLoadProgress(string assetName)
            {
                float progress = 2.1f;
                
                AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
                if (ab != null && ab.IsLegal)
                {
                    float abProgress = AssetBundleManager.Instance.GetLoadProgress(ab.AssetBundlePath);
                    if (abProgress > -1f)
                    {
                        progress = Mathf.Min(progress, abProgress);
                    }
                }
                AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
                if (abRemote != null && abRemote.IsLegal)
                { 
                    float remoteProgress = AssetBundleManager.Instance.GetLoadProgress(abRemote.AssetBundlePath);
                    if (remoteProgress > -1f)
                    {
                        progress = Mathf.Min(progress, remoteProgress);
                    }
                }
                AssetInfo res = GetAssetInfoFromResource(assetName);
                if (res != null && res.IsLegal)
                {
                    float resProgress = ResourcesManager.Instance.GetLoadProgress(res.AssetPath);
                    if (resProgress > -1f)
                    {
                        progress = Mathf.Min(progress, resProgress);
                    }
                }
                if (progress >= 2f)
                {
                    progress = -1f;
                }
                return progress;
            }
            
            /// <summary>
            /// 获取所有加载器的加载进度。
            /// 正常值范围从 0 到 1。
            /// 但如果没有加载器，则返回 -1。
            /// </summary>
            /// <returns>加载进度。</returns>
            public float GetLoadProgress()
            {
                float progress = 2.1f;
                float abProgress = AssetBundleManager.Instance.GetLoadProgress();
                if (abProgress > -1f)
                {
                    progress = Mathf.Min(progress, abProgress);
                }
                float resProgress = ResourcesManager.Instance.GetLoadProgress();
                if (resProgress > -1f)
                {
                    progress = Mathf.Min(progress, resProgress);
                }
                if (progress >= 2f)
                {
                    progress = -1f;
                }
                return progress;
            }
    }
}