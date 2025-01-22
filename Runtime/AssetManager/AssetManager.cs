using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    //异步加载完成的回调
    public delegate void OnAssetObject<T>(T obj)
        where T : Object;
    
    public class AssetManager : ModuleSingleton<AssetManager>, IModule
    {
        private AssetBundleManager _assetBundleManager;

        private ResourcesManager _resourcesManager;
        
        //强制更改资产加载模式为远程（微信小游戏使用）
        public static bool ForceRemoteAssetBundle = false;
        
        public const string DirSuffix = "_Directory";
        
        //资产信息
        public struct AssetInfo
        {
            //目标资产类型
            public readonly AssetTypeEnum AssetType;
            
            //AssetName
            public readonly string AssetName;
            
            //直接资产请求路径相对路径，Assets开头的
            public readonly string[] AssetPath;
            
            //直接资产捆绑请求路径（仅适用于资产捆绑类型），完全路径
            public readonly string AssetBundlePath;
            
            //AB名
            public readonly string AbName;
            public AssetInfo(
                AssetTypeEnum assetType = default,
                string assetName = default,
                string[] assetPath = default,
                string assetBundlePathWithoutAb = default,
                string abName = default)
            {
                AssetType = assetType;
                AssetName = assetName;
                AssetPath = assetPath;
                AssetBundlePath = assetBundlePathWithoutAb + abName;
                AbName = abName;
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
            
            // 是否采用编辑器模式
            private bool _isEditorMode = false;
            public bool IsEditorMode
            {
                get
                {
#if UNITY_EDITOR
                    return _isEditorMode || UnityEditor.EditorPrefs.GetBool(Application.dataPath.GetHashCode() + "IsEditorMode", false);
#else
                    return false;
#endif
                }
                set
                {
                    _isEditorMode = value;
                }
            }
            
            
            //如果信息合法，则该值为真
            public bool IsLegal(ref AssetInfo assetInfo)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    if (assetInfo.AssetType == AssetTypeEnum.RESOURCE)
                        if (assetInfo.AssetPath != default || SearchAsset(assetInfo.AssetName, AssetAccessMode.RESOURCE) != null)
                        {
                            return true;
                        }
                    
                    if (assetInfo.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                        if ((assetInfo.AssetPath != default && assetInfo.AssetBundlePath != default) || SearchAsset(assetInfo.AssetName, AssetAccessMode.ASSET_BUNDLE) != null)
                        {
                            return true;
                        }
                    
                    return false;
                }
#endif
                if (assetInfo.AssetType == AssetTypeEnum.NONE)
                    return false;

                if (assetInfo.AssetType == AssetTypeEnum.RESOURCE &&
                    assetInfo.AssetPath == default)
                    return false;

                if (assetInfo.AssetType == AssetTypeEnum.ASSET_BUNDLE &&
                    (assetInfo.AssetPath == default || assetInfo.AssetBundlePath == default))
                    return false;

                return true;
            }
            
            /// <summary>
            /// 根据提供的资产路径和访问选项推断资产类型。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="accessMode">访问模式。</param>
            /// <returns>资产信息。</returns>
            public AssetInfo GetAssetInfo(string assetName,
                AssetAccessMode accessMode = AssetAccessMode.UNKNOWN)
            {
                if (ForceRemoteAssetBundle)
                {
                    accessMode = AssetAccessMode.REMOTE_ASSET_BUNDLE;
                }
                
                if (accessMode.HasFlag(AssetAccessMode.RESOURCE))
                {
                    bool showTip = !IsEditorMode;
                    
                    return GetAssetInfoFromResource(assetName, showTip);
                }
                else if (accessMode.HasFlag(AssetAccessMode.ASSET_BUNDLE))
                {
                    bool showTip = !IsEditorMode;
                    
                    return GetAssetInfoFromAssetBundle(assetName, false, showTip);
                }
                else if (accessMode.HasFlag(AssetAccessMode.UNKNOWN))
                {
                    AssetInfo r = GetAssetInfoFromAssetBundle(assetName);
                    if (!IsLegal(ref r))
                    {
                        r = GetAssetInfoFromResource(assetName);
                    }

                    if (IsLegal(ref r))
                    {
                        return r;
                    }
                    else
                    {
                        LogF8.LogError("AssetBundle和Resource都找不到指定资源可用的索引：" + assetName);
                        return new AssetInfo();
                    }
                }
                else if (accessMode.HasFlag(AssetAccessMode.REMOTE_ASSET_BUNDLE))
                {
                    AssetInfo r = GetAssetInfoFromAssetBundle(assetName, true);
                    if (!IsLegal(ref r))
                    {
                        r = GetAssetInfoFromResource(assetName);
                    }
                    
                    if (IsLegal(ref r))
                    {
                        return r;
                    }
                    else
                    {
                        LogF8.LogError("AssetBundle找不到指定远程资源可用的索引：" + assetName);
                        return new AssetInfo();
                    }
                }
                return new AssetInfo();
            }

            /// <summary>
            /// 通过资产捆绑加载程序和对象名称获取资产对象。
            /// </summary>
            /// <typeparam name="T">资产对象的目标对象类型。</typeparam>
            /// <param name="assetName">资产对象的路径。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>找到的资产对象。</returns>
            public T GetAssetObject<T>(
                string assetName,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
                where T : Object
            {
                AssetInfo info = GetAssetInfo(assetName, mode);

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                    return o;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        return EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], subAssetName, out EditorLoader editorLoader);
                    }
#endif
                    T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader);
                    if (o != null)
                    {
                        return o;
                    }
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    ab.Expand(info.AssetPath[0], typeof(T), subAssetName);
                    o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        return o;
                    }
                    LogF8.LogError("获取不到资产或者类型错误！");
                }

                return null;
            }

            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="assetType">目标资产类型。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public Object GetAssetObject(
                string assetName,
                System.Type assetType,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);
                    return o;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        return EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], out EditorLoader editorLoader, assetType, subAssetName);
                    }
#endif
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader);
                    if (o != null)
                    {
                        return o;
                    }
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    ab.Expand(info.AssetPath[0], assetType, subAssetName);
                    o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        return o;
                    }
                    LogF8.LogError("获取不到资产或者类型错误！");
                }

                return null;
            }

            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="mode">访问模式。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <returns>加载的资产对象。</returns>
            public Object GetAssetObject(
                string assetName,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetAssetObject(assetPath, default, subAssetName, out ResourcesLoader loader);
                    return o;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        return EditorLoadAsset<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], subAssetName, out EditorLoader editorLoader);
                    }
#endif
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader);
                    if (o != null)
                    {
                        return o;
                    }
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    ab.Expand(info.AssetPath[0], default, subAssetName);
                    o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        return o;
                    }
                    LogF8.LogError("获取不到资产！");
                }

                return null;
            }
            
            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <typeparam name="T">目标资产类型。</typeparam>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public T Load<T>(
                string assetName,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
                where T : Object
            {
                
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                    return null;
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                    if (o != null)
                    {
                        return o;
                    }

                    if (subAssetName.IsNullOrEmpty())
                    {
                        return ResourcesManager.Instance.Load<T>(assetPath);
                    }
                    return ResourcesManager.Instance.LoadAll<T>(assetPath, subAssetName, out ResourcesLoader loader2);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        return EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], subAssetName, out EditorLoader editorLoader);
                    }
#endif
                    T o2 = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        return o2;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, typeof(T), ref info, subAssetName);
                        loader = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
                
                    T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        return o;
                    }
                    
                    loader.Expand(info.AssetPath[0], typeof(T), subAssetName);
                    return AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader3);
                }

                return null;
            }
            
            /// <summary>
            /// 同步加载资源文件夹。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="mode">访问模式。</param>
            public void LoadDir(
                string assetName,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                assetName += DirSuffix;
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                    return;
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        LogF8.LogAsset("编辑器模式下无需加载文件夹");
                        return;
                    }
#endif
                    string[] assetPaths = info.AssetPath;
                    if (assetPaths == default || assetPaths.Length <= 0)
                    {
                        return;
                    }

                    foreach (var subAssetName in assetPaths)
                    {
                        AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                        string assetPath = subInfo.AssetPath?[0];
                        Object o = ResourcesManager.Instance.GetAssetObject(assetPath, default, null, out ResourcesLoader loader);
                    
                        if (o == null)
                        {
                            ResourcesManager.Instance.Load(assetPath);
                        }
                    }
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        return;
                    }
#endif
                    foreach (var subAssetName in info.AssetPath)
                    {
                        if (string.IsNullOrEmpty(subAssetName))
                        {
                            continue;
                        }
                        AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                        AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(subInfo.AssetBundlePath);
                        if (ab == null || ab.AssetBundleContent == null)
                        {
                            AssetBundleManager.Instance.Load(subAssetName, default, ref subInfo, "");
                        }
                    }
                }
            }
            
            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="assetType">目标资产类型。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public Object Load(
                string assetName,
                System.Type assetType,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                    return null;

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);
                    if (o != null)
                    {
                        return o;
                    }
                    
                    if (subAssetName.IsNullOrEmpty())
                    {
                        return ResourcesManager.Instance.Load(assetPath, assetType);
                    }
                    return ResourcesManager.Instance.LoadAll(assetPath, assetType, subAssetName, out ResourcesLoader loader2);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        return EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], out EditorLoader editorLoader, assetType, subAssetName);
                    }
#endif
                    Object o2 = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        return o2;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, assetType, ref info, subAssetName);
                        loader = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
            
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        return o;
                    }
                
                    loader.Expand(info.AssetPath[0], assetType, subAssetName);
                    return AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader3);
                }

                return null;
            }

            /// <summary>
            /// 同步加载资源对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public Object Load(
                string assetName,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                    return null;

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetAssetObject(assetPath, default, subAssetName, out ResourcesLoader loader);
                    if (o != null)
                    {
                        return o;
                    }

                    if (subAssetName.IsNullOrEmpty())
                    {
                        return ResourcesManager.Instance.Load(assetPath);
                    }
                    return ResourcesManager.Instance.LoadAll(assetPath, default, subAssetName, out ResourcesLoader loader2);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        return EditorLoadAsset<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], subAssetName, out EditorLoader editorLoader);
                    }
#endif
                    Object o2 = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        return o2;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, default, ref info, subAssetName);
                        loader = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
            
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        return o;
                    }
                    
                    loader.Expand(info.AssetPath[0], default, subAssetName);
                    return AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader3);
                }

                return null;
            }
            
            /// <summary>
            /// 异步加载资产对象。
            /// </summary>
            /// <typeparam name="T">目标资产类型。</typeparam>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            public BaseLoader LoadAsync<T>(
                string assetName,
                OnAssetObject<T> callback = null,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
                where T : Object
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    End();
                    return new BaseLoader();
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                    if (o != null)
                    {
                        End(o);
                        return loader;
                    }

                    if (loader == null || loader.LoaderSuccess == false || o == null)
                    {
                        if (subAssetName.IsNullOrEmpty())
                        {
                            return ResourcesManager.Instance.LoadAsync<T>(assetPath, callback);
                        }
                        else
                        {
                            T subAsset = ResourcesManager.Instance.LoadAll<T>(assetPath, subAssetName, out ResourcesLoader loader2);
                            End(subAsset);
                            return loader2;
                        }
                    }
                    return null;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        T o = EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], subAssetName, out EditorLoader editorLoader);
                        End(o);
                        return editorLoader;
                    }
#endif
                    T o2 = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        End(o2);
                        return loader;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                    {
                        loader = AssetBundleManager.Instance.LoadAsync(assetName, typeof(T), info, subAssetName, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader2));
                        });
                        return loader;
                    }
                    else
                    {
                        T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader3);
                        if (o != null)
                        {
                            End(o);
                            return loader3;
                        }
                        
                        loader.Expand(info.AssetPath[0], typeof(T), subAssetName);
                        End(AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader4));
                        return loader4;
                    }
                }
                
                void End(T o = null)
                {
                    callback?.Invoke(o);
                }
                
                return null;
            }
            
            /// <summary>
            /// 协程加载资产对象。
            /// </summary>
            /// <typeparam name="T">目标资产类型。</typeparam>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            public IEnumerator LoadAsyncCoroutine<T>(string assetName, string subAssetName = null, AssetAccessMode mode = AssetAccessMode.UNKNOWN) where T : Object
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    yield break;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                    if (o != null)
                    {
                        yield return o;
                    }
                    else
                    {
                        if (subAssetName.IsNullOrEmpty())
                        {
                            yield return ResourcesManager.Instance.LoadAsyncCoroutine<T>(assetPath);
                        }
                        else
                        {
                            yield return ResourcesManager.Instance.LoadAll<T>(assetPath, subAssetName, out ResourcesLoader loader2);
                        }
                    }
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        T o = EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], subAssetName, out EditorLoader editorLoader);
                        yield return o;
                        yield break;
                    }
#endif
                    T o2 = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        yield return o2;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                    {
                        yield return AssetBundleManager.Instance.LoadAsyncCoroutine(assetName, typeof(T), info, subAssetName);
                    }
                    else
                    {
                        T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader2);
                        if (o != null)
                        {
                            yield return o;
                        }
                        else
                        {
                            loader.Expand(info.AssetPath[0], typeof(T), subAssetName);
                            yield return AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0], subAssetName, out AssetBundleLoader loader3);
                        }
                    }
                }
            }
            
            /// <summary>
            /// 协程加载资产对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="assetType">目标资产类型。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            public IEnumerator LoadAsyncCoroutine(string assetName, System.Type assetType = default, string subAssetName = null, AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    yield break;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);
                    
                    if (o != null)
                    {
                        yield return o;
                    }
                    else
                    {
                        if (subAssetName.IsNullOrEmpty())
                        {
                            yield return ResourcesManager.Instance.LoadAsyncCoroutine(assetPath, assetType);
                        }
                        else
                        {
                            yield return ResourcesManager.Instance.LoadAll(assetPath, assetType, subAssetName, out ResourcesLoader loader2);
                        }
                    }
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        var o = EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], out EditorLoader editorLoader, assetType, subAssetName);
                        yield return o;
                        yield break;
                    }
#endif
                    Object o2 = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        yield return o2;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                    {
                        yield return AssetBundleManager.Instance.LoadAsyncCoroutine(assetName, assetType, info, subAssetName);
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader2);
                        if (o != null)
                        {
                            yield return o;
                        }
                        else
                        {
                            loader.Expand(info.AssetPath[0], assetType, subAssetName);
                            yield return AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader3);
                        }
                    }
                }
            }
             
            /// <summary>
            /// 异步加载资产文件夹。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="mode">访问模式。</param>
            public BaseDirLoader LoadDirAsync(
                string assetName,
                Action callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                assetName += DirSuffix;
                AssetInfo info = GetAssetInfo(assetName, mode);
                BaseDirLoader dirLoader = new BaseDirLoader();
                if (!IsLegal(ref info))
                {
                    End();
                    return dirLoader;
                }
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        LogF8.LogAsset("编辑器模式下无需加载文件夹");
                        End();
                        return dirLoader;
                    }
#endif
                    string[] assetPaths = info.AssetPath;
                    if (assetPaths == default || assetPaths.Length <= 0)
                    {
                        End();
                        return dirLoader;
                    }
                    
                    int assetCount = 0;
                    foreach (var subAssetName in assetPaths)
                    {
                        AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                        string assetPath = subInfo.AssetPath?[0];
                        Object o = ResourcesManager.Instance.GetAssetObject(assetPath, default, null, out ResourcesLoader loader);
                        if (o != null)
                        {
                            dirLoader.Loaders.Add(loader);
                            if (++assetCount >= info.AssetPath?.Length)
                            {
                                End();
                                dirLoader.OnComplete();
                            }
                        }
                        else
                        {
                            BaseLoader loader2 = ResourcesManager.Instance.LoadAsync(assetPath, (b) =>
                            {
                                if (++assetCount >= info.AssetPath?.Length)
                                {
                                    End();
                                    dirLoader.OnComplete();
                                }
                            });
                            dirLoader.Loaders.Add(loader2);
                        }
                    }
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        LogF8.LogAsset("编辑器模式下无需加载文件夹");
                        End();
                        return dirLoader;
                    }
#endif
                    int assetCount = 0;
                    foreach (var subAssetName in info.AssetPath)
                    {
                        if (string.IsNullOrEmpty(subAssetName))
                        {
                            continue;
                        }
                        AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                        AssetBundleLoader loader = AssetBundleManager.Instance.GetAssetBundleLoader(subInfo.AssetBundlePath);
                        if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                        {
                            loader = AssetBundleManager.Instance.LoadAsync(subAssetName, default,
                                subInfo, "", (b) =>
                                {
                                    if (++assetCount >= info.AssetPath.Length)
                                    {
                                        End();
                                        dirLoader.OnComplete();
                                    }
                                });
                            dirLoader.Loaders.Add(loader);
                        }
                        else
                        {
                            Object o = AssetBundleManager.Instance.GetAssetObject(subInfo.AssetBundlePath, subInfo.AssetPath[0], default, null, out AssetBundleLoader loader2);
                            if (o != null)
                            {
                                dirLoader.Loaders.Add(loader2);
                                if (++assetCount >= info.AssetPath.Length)
                                { 
                                    End();
                                    dirLoader.OnComplete();
                                }
                                continue;
                            }
                            
                            loader.Expand(subInfo.AssetPath[0], default, "");
                            dirLoader.Loaders.Add(loader);
                            if (++assetCount >= info.AssetPath.Length)
                            {
                                End();
                                dirLoader.OnComplete();
                            }
                        }
                    }
                }

                void End()
                {
                    callback?.Invoke();
                }
                
                return dirLoader;
            }
            
            /// <summary>
            /// 协程加载资产文件夹。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="mode">访问模式。</param>
            public IEnumerable LoadDirAsyncCoroutine(string assetName, AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                assetName += DirSuffix;
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    yield break;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        LogF8.LogAsset("编辑器模式下无需加载文件夹");
                        yield break;
                    }
#endif
                    string[] assetPaths = info.AssetPath;
                    if (assetPaths == default || assetPaths.Length <= 0)
                    {
                        yield break;
                    }

                    foreach (var subAssetName in assetPaths)
                    {
                        AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                        string assetPath = subInfo.AssetPath?[0];
                        Object o = ResourcesManager.Instance.GetAssetObject(assetPath, default, null, out ResourcesLoader loader);
                    
                        if (o != null)
                        {
                            yield return o;
                        }
                        else
                        {
                            yield return ResourcesManager.Instance.LoadAsyncCoroutine(assetPath);
                        }
                    }
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        LogF8.LogAsset("编辑器模式下无需加载文件夹");
                        yield break;
                    }
#endif
                    int assetCount = 0;
                    foreach (var subAssetName in info.AssetPath)
                    {
                        if (string.IsNullOrEmpty(subAssetName))
                        {
                            continue;
                        }
                        AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                        AssetBundleLoader loader = AssetBundleManager.Instance.GetAssetBundleLoader(subInfo.AssetBundlePath);
                        if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                        {
                            yield return AssetBundleManager.Instance.LoadAsyncCoroutine(subAssetName, default, subInfo, null);
                            if (++assetCount >= info.AssetPath.Length)
                            {
                                yield break;
                            }
                        }
                        else
                        {
                            Object o = AssetBundleManager.Instance.GetAssetObject(subInfo.AssetBundlePath, subInfo.AssetPath[0], default, null, out AssetBundleLoader loader2);
                            if (o != null)
                            {
                                if (++assetCount >= info.AssetPath.Length)
                                {
                                    yield break;
                                }
                                continue;
                            }
                            
                            loader.Expand(subInfo.AssetPath[0], default, "");
                            if (++assetCount >= info.AssetPath.Length)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// 异步加载资产对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="assetType">目标资产类型。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            public BaseLoader LoadAsync(
                string assetName,
                System.Type assetType,
                OnAssetObject<Object> callback = null,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    End();
                    return new BaseLoader();
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);
                    if (o != null)
                    {
                        End(o);
                        return loader;
                    }
                    
                    if (loader == null || loader.LoaderSuccess == false || o == null)
                    {
                        if (subAssetName.IsNullOrEmpty())
                        {
                            return ResourcesManager.Instance.LoadAsync(assetPath, assetType, callback);
                        }
                        else
                        {
                            Object subAsset = ResourcesManager.Instance.LoadAll(assetPath, assetType, subAssetName, out ResourcesLoader loader2);
                            End(subAsset);
                            return loader2;
                        }
                    }
                    return null;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        Object o = EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], out EditorLoader editorLoader, assetType, subAssetName);
                        End(o);
                        return editorLoader;
                    }
#endif
                    Object o2 = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        End(o2);
                        return loader;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                    {
                        loader = AssetBundleManager.Instance.LoadAsync(assetName, assetType, info, subAssetName, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader2));
                        });
                        return loader;
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader3);
                        if (o != null)
                        {
                            End(o);
                            return loader3;
                        }
            
                        loader3.Expand(info.AssetPath[0], assetType, subAssetName);
                        End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType, subAssetName, out AssetBundleLoader loader4));
                        return loader4;
                    }
                }

                void End(Object o = null)
                {
                    callback?.Invoke(o);
                }
                
                return null;
            }
            
            /// <summary>
            /// 异步加载资产对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="subAssetName">子资产名称。</param>
            /// <param name="mode">访问模式。</param>
            public BaseLoader LoadAsync(
                string assetName,
                OnAssetObject<Object> callback = null,
                string subAssetName = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    End();
                    return new BaseLoader();
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetAssetObject(assetPath, default, subAssetName, out ResourcesLoader loader);
                    if (o != null)
                    {
                        End(o);
                        return loader;
                    }
                    
                    if (loader == null || loader.LoaderSuccess == false || o == null)
                    {
                        if (subAssetName.IsNullOrEmpty())
                        {
                            return ResourcesManager.Instance.LoadAsync(assetPath, callback);
                        }
                        else
                        {
                            Object subAsset = ResourcesManager.Instance.LoadAll(assetPath, default, subAssetName, out ResourcesLoader loader2);
                            End(subAsset);
                            return loader2;
                        }
                    }
                    return null;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (IsEditorMode)
                    {
                        Object o = EditorLoadAsset<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], subAssetName, out EditorLoader editorLoader);
                        End(o);
                        return editorLoader;
                    }
#endif
                    Object o2 = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader);
                    if (o2 != null)
                    {
                        End(o2);
                        return loader;
                    }
                    
                    if (loader == null || loader.AssetBundleContent == null)
                    {
                        loader = AssetBundleManager.Instance.LoadAsync(assetName, default, info, subAssetName, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader2));
                        });
                        return loader;
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader3);
                        if (o != null)
                        {
                            End(o);
                            return loader3;
                        }
            
                        loader3.Expand(info.AssetPath[0], default, subAssetName);
                        End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], default, subAssetName, out AssetBundleLoader loader4));
                        return loader4;
                    }
                }

                void End(Object o = null)
                {
                    callback?.Invoke(o);
                }
                
                return null;
            }
            
            
            private AssetInfo GetAssetInfoFromResource(string assetName, bool showTip = false)
            {
                if (ResourceMap.Mappings.TryGetValue(assetName, out string[] value))
                {
                    return new AssetInfo(AssetTypeEnum.RESOURCE, assetName, value, default, default);
                }

                if (showTip)
                {
                    LogF8.LogError("Resource找不到指定资源可用的索引：" + assetName);
                }
                return new AssetInfo(AssetTypeEnum.RESOURCE, assetName);
            }
            
            private AssetInfo GetAssetInfoFromAssetBundle(string assetName, bool remote = false, bool showTip = false)
            {
                if (AssetBundleMap.Mappings.TryGetValue(assetName, out AssetBundleMap.AssetMapping assetmpping))
                {
                    if (remote || ForceRemoteAssetBundle)
                    {
                        return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE, assetName, assetmpping.AssetPath, AssetBundleManager.GetRemoteAssetBundleCompletePath(), assetmpping.AbName);
                    }
                    else
                    {
                        return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE, assetName, assetmpping.AssetPath, AssetBundleManager.GetAssetBundlePathWithoutAb(assetName), assetmpping.AbName);
                    }
                }

                if (showTip)
                {
                    LogF8.LogError("AssetBundle找不到指定资源可用的索引：" + assetName);
                }
                return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE, assetName);
            }
            
            /// <summary>
            /// 通过资源名称同步卸载。
            /// </summary>
            /// <param name="assetName">资源名称。</param>
            /// <param name="unloadAllLoadedObjects">完全卸载。</param>
            public void Unload(string assetName, bool unloadAllLoadedObjects = false)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return;
                }
#endif
                AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
                if (IsLegal(ref ab))
                {
                    AssetBundleManager.Instance.Unload(ab.AssetBundlePath, unloadAllLoadedObjects);
                }
                AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
                if (IsLegal(ref abRemote))
                {
                    AssetBundleManager.Instance.Unload(abRemote.AssetBundlePath, unloadAllLoadedObjects);
                }
                AssetInfo res = GetAssetInfoFromResource(assetName);
                if (IsLegal(ref res))
                {
                    ResourcesManager.Instance.Unload(res.AssetPath[0], unloadAllLoadedObjects);
                }
            }
            
            /// <summary>
            /// 通过资源名称异步卸载。
            /// </summary>
            /// <param name="assetName">资源名称。</param>
            /// <param name="unloadAllLoadedObjects">
            /// 完全卸载。
            /// </param>
            /// <param name="callback">异步卸载完成时的回调函数。</param>
            public void UnloadAsync(string assetName, bool unloadAllLoadedObjects = false, AssetBundleLoader.OnUnloadFinished callback = null)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return;
                }
#endif
                AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
                if (IsLegal(ref ab))
                {
                    AssetBundleManager.Instance.UnloadAsync(ab.AssetBundlePath, unloadAllLoadedObjects, callback);
                }
                AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
                if (IsLegal(ref abRemote))
                {
                    AssetBundleManager.Instance.UnloadAsync(abRemote.AssetBundlePath, unloadAllLoadedObjects, callback);
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
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return 1f;
                }
#endif
                float progress = 2.1f;

                string assetBundlePath = "";
                string assetBundlePathRemote = "";
                
                AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
                if (IsLegal(ref ab))
                {
                    assetBundlePath = ab.AssetBundlePath;
                }

                AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
                if (IsLegal(ref abRemote))
                {
                    assetBundlePathRemote = abRemote.AssetBundlePath;
                }

                AssetInfo res = GetAssetInfoFromResource(assetName);
                if (IsLegal(ref res))
                {
                    float resProgress = ResourcesManager.Instance.GetLoadProgress(res.AssetPath[0]);
                    if (resProgress > -1f)
                    {
                        progress = Mathf.Min(progress, resProgress);
                    }
                }
                
                float bundleProgress = AssetBundleManager.Instance.GetLoadProgress(assetBundlePath);
                if (bundleProgress > -1f)
                {
                    progress = Mathf.Min(progress, bundleProgress);
                }
                
                float bundleProgressRemote = AssetBundleManager.Instance.GetLoadProgress(assetBundlePathRemote);
                if (bundleProgressRemote > -1f)
                {
                    progress = Mathf.Min(progress, bundleProgressRemote);
                }

                if (progress >= 2f)
                {
                    progress = 0f;
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
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return 1f;
                }
#endif
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
                    progress = 0f;
                }
                return progress;
            }
            
#if UNITY_EDITOR
            private List<string> searchDirs = new List<string>();
            private List<string> resourcesDirs = new List<string>();
            private List<string> assetBundlesDirs = new List<string>();
            private Dictionary<string, string> findAssetPaths = new Dictionary<string, string>();
            private Dictionary<string, string> resourcesFindAssetPaths = new Dictionary<string, string>();
            private Dictionary<string, string> assetBundlesFindAssetPaths = new Dictionary<string, string>();
            private string SearchAsset(string assetName, AssetAccessMode accessMode = AssetAccessMode.UNKNOWN)
            {
                // 缓存路径
                if (accessMode == AssetAccessMode.UNKNOWN)
                {
                    if (findAssetPaths.TryGetValue(assetName, out string value))
                    {
                        return value;
                    }
                }
                else if (accessMode == AssetAccessMode.RESOURCE)
                {
                    if (resourcesFindAssetPaths.TryGetValue(assetName, out string value))
                    {
                        return value;
                    }
                }
                else if (accessMode == AssetAccessMode.ASSET_BUNDLE)
                {
                    if (assetBundlesFindAssetPaths.TryGetValue(assetName, out string value))
                    {
                        return value;
                    }
                }
                
                if (searchDirs.Count <= 0)
                {
                    // 获取项目中的所有文件夹路径
                    string[] allFolders = UnityEditor.AssetDatabase.GetAllAssetPaths();
                    foreach (string folderPath in allFolders)
                    {
                        if (System.IO.Directory.Exists(folderPath) && folderPath.Contains("/Resources"))
                        {
                            searchDirs.Add(folderPath);
                            resourcesDirs.Add(folderPath);
                        }
                    }
                    searchDirs.Add(System.IO.Path.Combine(URLSetting.AssetBundlesPath));
                    assetBundlesDirs.Add(System.IO.Path.Combine(URLSetting.AssetBundlesPath));
                }
                
                // 查找指定资源
                string[] dirs = null;
                if (accessMode == AssetAccessMode.UNKNOWN)
                {
                    dirs = searchDirs.ToArray();
                }
                else if (accessMode == AssetAccessMode.RESOURCE)
                {
                    dirs = resourcesDirs.ToArray();
                }
                else if (accessMode == AssetAccessMode.ASSET_BUNDLE)
                {
                    dirs = assetBundlesDirs.ToArray();
                }
                
                string[] guids = UnityEditor.AssetDatabase.FindAssets(assetName, dirs);
                foreach (string guid in guids)
                {
                    // 将 GUID 转换为路径
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
    
                    if (Path.GetFileNameWithoutExtension(assetPath) == assetName)
                    {
                        if (accessMode == AssetAccessMode.UNKNOWN)
                        {
                            findAssetPaths[assetName] = assetPath;
                        }
                        else if (accessMode == AssetAccessMode.RESOURCE)
                        {
                            resourcesFindAssetPaths[assetName] = assetPath;
                        }
                        else if (accessMode == AssetAccessMode.ASSET_BUNDLE)
                        {
                            assetBundlesFindAssetPaths[assetName] = assetPath;
                        }
                        
                        LogF8.LogAsset(assetPath);
                        return assetPath;
                    }
                }
                return null;
            }
#endif

#if UNITY_EDITOR
            // 编辑器下加载资源
            private T EditorLoadAsset<T>(string assetPath, string subAssetName, out EditorLoader loader) where T : Object
            {
                if (subAssetName.IsNullOrEmpty())
                {
                    T o = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    loader = new EditorLoader(o);
                    return o;
                }
                else
                {
                    var objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                    foreach (var obj in objs)
                    {
                        if (obj.name == subAssetName)
                        {
                            loader = new EditorLoader(obj);
                            return obj as T;
                        }
                    }
                }
                loader = null;
                return null;
            }
                
            private Object EditorLoadAsset(string assetPath, out EditorLoader loader, System.Type assetType = default, string subAssetName = null)
            {
                if (subAssetName.IsNullOrEmpty())
                {
                    if (assetType == default)
                    {
                        Object o = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                        loader = new EditorLoader(o);
                        return o;
                    }
                    Object o2 =  UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                    loader = new EditorLoader(o2);
                    return o2;
                }
                else
                {
                    var objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                    foreach (var obj in objs)
                    {
                        if (obj.name == subAssetName)
                        {
                            loader = new EditorLoader(obj);
                            return obj;
                        }
                    }
                }

                loader = null;
                return null;
            }
#endif
        
            public void OnInit(object createParam)
            {
                _assetBundleManager = ModuleCenter.CreateModule<AssetBundleManager>();
                _resourcesManager = ModuleCenter.CreateModule<ResourcesManager>();
                if (File.Exists(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json"))
                {
                    string json =
                        FileTools.SafeReadAllText(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json");
                    AssetBundleMap.Mappings = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(json);
                }
                else
                {
                    AssetBundleMap.Mappings = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(Resources.Load<TextAsset>(nameof(AssetBundleMap)).ToString());
                }
                ResourceMap.Mappings = Util.LitJson.ToObject<Dictionary<string, string[]>>(Resources.Load<TextAsset>(nameof(ResourceMap)).ToString());
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
                base.Destroy();
            }
    }
}