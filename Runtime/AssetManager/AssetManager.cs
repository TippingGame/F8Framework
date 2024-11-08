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
        
        //资产信息
        public struct AssetInfo
        {
            //目标资产类型
            public readonly AssetTypeEnum AssetType;
            
            //直接资产请求路径相对路径，Assets开头的
            public readonly string[] AssetPath;
            
            //直接资产捆绑请求路径（仅适用于资产捆绑类型），完全路径
            public readonly string AssetBundlePath;
            
            //AB名
            public readonly string AbName;
            
            public AssetInfo(
                AssetTypeEnum assetType = default,
                string[] assetPath = default,
                string assetBundlePathWithoutAb = default,
                string abName = default)
            {
                AssetType = assetType;
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
                    return _isEditorMode;
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
                if (_isEditorMode)
                {
                    return true;
                }
#endif
                if (assetInfo.AssetType == AssetTypeEnum.NONE)
                    return false;

                if (assetInfo.AssetType == AssetTypeEnum.RESOURCE &&
                    assetInfo.AssetPath == null)
                    return false;

                if (assetInfo.AssetType == AssetTypeEnum.ASSET_BUNDLE &&
                    (assetInfo.AssetPath == null || assetInfo.AssetBundlePath == null))
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
                    bool showTip = !_isEditorMode;
                    
                    return GetAssetInfoFromResource(assetName, showTip);
                }
                else if (accessMode.HasFlag(AssetAccessMode.ASSET_BUNDLE))
                {
                    bool showTip = !_isEditorMode;
                    
                    return GetAssetInfoFromAssetBundle(assetName, showTip);
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
            /// <param name="mode">访问模式。</param>
            /// <returns>找到的资产对象。</returns>
            public T GetAssetObject<T>(
                string assetName,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
                where T : Object
            {
                AssetInfo info = GetAssetInfo(assetName, mode);

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, typeof(T), AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetResouceObject<T>(assetPath);
                    return o;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.AssetPath == null ? SearchAsset(assetName, typeof(T)) : info.AssetPath[0]);
                    }
#endif
                    T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]);
                    if (o != null)
                    {
                        return o;
                    }
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    ab.Expand(typeof(T));
                    o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]);
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
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public Object GetAssetObject(
                string assetName,
                System.Type assetType,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, default, AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetResouceObject(assetPath, assetType);
                    return o;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        return UnityEditor.AssetDatabase.LoadAssetAtPath(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], assetType);
                    }
#endif
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType);
                    if (o != null)
                    {
                        return o;
                    }
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    ab.Expand(assetType);
                    o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType);
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
            /// <returns>加载的资产对象。</returns>
            public Object GetAssetObject(
                string assetName,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, default, AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetResouceObject(assetPath);
                    return o;
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        return UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0]);
                    }
#endif
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0]);
                    if (o != null)
                    {
                        return o;
                    }
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    ab.Expand(default);
                    o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0]);
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
            /// <param name="mode">访问模式。</param>
            /// <returns>加载的资产对象。</returns>
            public T Load<T>(
                string assetName,
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
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, typeof(T), AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetResouceObject<T>(assetPath);
                    if (o != null)
                    {
                        return o;
                    }
                      
                    return ResourcesManager.Instance.Load<T>(assetPath);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.AssetPath == null ? SearchAsset(assetName, typeof(T)) : info.AssetPath[0]);
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, typeof(T), ref info);
                        ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
                
                    T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]);
                    if (o != null)
                    {
                        return o;
                    }
                    
                    ab.Expand(typeof(T));
                    return AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]);
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
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                    return;
                
                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    LogF8.LogAsset("Resources不支持加载文件夹功能");
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
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
                            AssetBundleManager.Instance.Load(subAssetName, default, ref subInfo);
                        }
                    }
                }
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
                if (!IsLegal(ref info))
                    return null;

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, default, AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetResouceObject(assetPath, assetType);
                    if (o != null)
                    {
                        return o;
                    }

                    return ResourcesManager.Instance.Load(assetPath, assetType);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        return UnityEditor.AssetDatabase.LoadAssetAtPath(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], assetType);
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, assetType, ref info);
                        ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
            
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType);
                    if (o != null)
                    {
                        return o;
                    }
                
                    ab.Expand(assetType);
                    return AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType);
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
                if (!IsLegal(ref info))
                    return null;

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, default, AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetResouceObject(assetPath);
                    if (o != null)
                    {
                        return o;
                    }

                    return ResourcesManager.Instance.Load(assetPath);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        return UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0]);
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.Load(assetName, default, ref info);
                        ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    }
            
                    Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0]);
                    if (o != null)
                    {
                        return o;
                    }
                    ab.Expand(default);
                    return AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0]);
                }

                return null;
            }
            
            /// <summary>
            /// 异步加载资产对象。
            /// </summary>
            /// <typeparam name="T">目标资产类型。</typeparam>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="mode">访问模式。</param>
            public void LoadAsync<T>(
                string assetName,
                OnAssetObject<T> callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
                where T : Object
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    End();
                    return;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, typeof(T), AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetResouceObject<T>(assetPath);
                    if (o != null)
                    {
                        End(o);
                        return;
                    }
                    ResourcesManager.Instance.LoadAsync<T>(assetPath, callback);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        T o = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.AssetPath == null ? SearchAsset(assetName, typeof(T)) : info.AssetPath[0]);
                        End(o);
                        return;
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null || ab.AssetBundleContent == null || ab.GetDependentNamesLoadFinished() < ab.AddDependentNames())
                    {
                        AssetBundleManager.Instance.LoadAsync(assetName, typeof(T), info, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]));
                        });
                        return;
                    }
                    else
                    {
                        T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]);
                        if (o != null)
                        {
                            End(o);
                            return;
                        }
                        
                        ab.Expand(typeof(T));
                        End(AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]));
                    }
                }

                void End(T o = null)
                {
                    callback?.Invoke(o);
                }
            }
            
            /// <summary>
            /// 协程加载资产对象。
            /// </summary>
            /// <typeparam name="T">目标资产类型。</typeparam>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="mode">访问模式。</param>
            public IEnumerator LoadAsyncCoroutine<T>(string assetName, AssetAccessMode mode = AssetAccessMode.UNKNOWN) where T : Object
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
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, typeof(T), AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    T o = ResourcesManager.Instance.GetResouceObject<T>(assetPath);
                    if (o != null)
                    {
                        yield return o;
                    }
                    else
                    {
                        yield return ResourcesManager.Instance.LoadAsyncCoroutine<T>(assetPath);
                    }
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        T o = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.AssetPath == null ? SearchAsset(assetName, typeof(T)) : info.AssetPath[0]);
                        yield return o;
                        yield break;
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null || ab.AssetBundleContent == null || ab.GetDependentNamesLoadFinished() < ab.AddDependentNames())
                    {
                        yield return AssetBundleManager.Instance.LoadAsyncCoroutine(assetName, typeof(T), info);
                    }
                    else
                    {
                        T o = AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]);
                        if (o != null)
                        {
                            yield return o;
                        }
                        else
                        {
                            ab.Expand(typeof(T));
                            yield return AssetBundleManager.Instance.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath[0]);
                        }
                    }
                }
            }
            
            /// <summary>
            /// 协程加载资产对象。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="assetType">目标资产类型。</param>
            /// <param name="mode">访问模式。</param>
            public IEnumerator LoadAsyncCoroutine(string assetName, System.Type assetType = default, AssetAccessMode mode = AssetAccessMode.UNKNOWN)
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
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, assetType, AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetResouceObject(assetPath, assetType);
                    
                    if (o != null)
                    {
                        yield return o;
                    }
                    else
                    {
                        yield return ResourcesManager.Instance.LoadAsyncCoroutine(assetPath, assetType);
                    }
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        var o = assetType == default ?
                            UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0]) :
                            UnityEditor.AssetDatabase.LoadAssetAtPath(info.AssetPath == null ? SearchAsset(assetName, assetType) : info.AssetPath[0], assetType);
                        yield return o;
                        yield break;
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null || ab.AssetBundleContent == null || ab.GetDependentNamesLoadFinished() < ab.AddDependentNames())
                    {
                        yield return AssetBundleManager.Instance.LoadAsyncCoroutine(assetName, assetType, info);
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType);
                        if (o != null)
                        {
                            yield return o;
                        }
                        else
                        {
                            ab.Expand(assetType);
                            yield return AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType);
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
            public void LoadDirAsync(
                string assetName,
                Action callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    End();
                    return;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    LogF8.LogAsset("Resources不支持加载文件夹功能");
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        LogF8.LogAsset("编辑器模式下无需加载文件夹");
                        End();
                        return;
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
                        AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(subInfo.AssetBundlePath);
                        if (ab == null || ab.AssetBundleContent == null || ab.GetDependentNamesLoadFinished() < ab.AddDependentNames())
                        {
                            AssetBundleManager.Instance.LoadAsync(subAssetName, default,
                                subInfo, (b) =>
                                {
                                    if (++assetCount >= info.AssetPath.Length)
                                    {
                                        End();
                                    }
                                });
                        }
                        else
                        {
                            Object o = AssetBundleManager.Instance.GetAssetObject(subInfo.AssetBundlePath, subInfo.AssetPath[0]);
                            if (o != null)
                            {
                                if (++assetCount >= info.AssetPath.Length)
                                {
                                    End();
                                }
                                continue;
                            }
                            
                            ab.Expand(default);
                            if (++assetCount >= info.AssetPath.Length)
                            {
                                End();
                            }
                        }
                    }
                }

                void End()
                {
                    callback?.Invoke();
                }
            }
            
            /// <summary>
            /// 协程加载资产文件夹。
            /// </summary>
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="mode">访问模式。</param>
            public IEnumerable LoadDirAsyncCoroutine(string assetName, AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    yield break;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    LogF8.LogAsset("Resources不支持加载文件夹功能");
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
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
                        AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(subInfo.AssetBundlePath);
                        if (ab == null || ab.AssetBundleContent == null || ab.GetDependentNamesLoadFinished() < ab.AddDependentNames())
                        {
                            yield return AssetBundleManager.Instance.LoadAsyncCoroutine(subAssetName, default, subInfo);
                            if (++assetCount >= info.AssetPath.Length)
                            {
                                yield break;
                            }
                        }
                        else
                        {
                            Object o = AssetBundleManager.Instance.GetAssetObject(subInfo.AssetBundlePath, subInfo.AssetPath[0]);
                            if (o != null)
                            {
                                if (++assetCount >= info.AssetPath.Length)
                                {
                                    yield break;
                                }
                                continue;
                            }
                            
                            ab.Expand(default);
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
            /// <param name="mode">访问模式。</param>
            public void LoadAsync(
                string assetName,
                System.Type assetType,
                OnAssetObject<Object> callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    End();
                    return;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, default, AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetResouceObject(assetPath, assetType);
                    if (o != null)
                    {
                        End(o);
                        return;
                    }
                    ResourcesManager.Instance.LoadAsync(assetPath, assetType, callback);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        Object o = UnityEditor.AssetDatabase.LoadAssetAtPath(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0], assetType);
                        End(o);
                        return;
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.LoadAsync(assetName, assetType, info, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType));
                        });
                        return;
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType);
                        if (o != null)
                        {
                            End(o);
                            return;
                        }
            
                        ab.Expand(assetType);
                        End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0], assetType));
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
            /// <param name="assetName">资产路径字符串。</param>
            /// <param name="callback">异步加载完成时的回调函数。</param>
            /// <param name="mode">访问模式。</param>
            public void LoadAsync(
                string assetName,
                OnAssetObject<Object> callback = null,
                AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                AssetInfo info = GetAssetInfo(assetName, mode);
                if (!IsLegal(ref info))
                {
                    End();
                    return;
                }

                if (info.AssetType == AssetTypeEnum.RESOURCE)
                {
                    string assetPath = info.AssetPath?[0];
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        assetPath = info.AssetPath == null ? SearchAsset(assetName, default, AssetAccessMode.RESOURCE) : info.AssetPath[0];
                    }
#endif
                    Object o = ResourcesManager.Instance.GetResouceObject(assetPath);
                    if (o != null)
                    {
                        End(o);
                        return;
                    }
                    ResourcesManager.Instance.LoadAsync(assetPath, callback);
                }
                else if (info.AssetType == AssetTypeEnum.ASSET_BUNDLE)
                {
#if UNITY_EDITOR
                    if (_isEditorMode)
                    {
                        Object o = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath[0]);
                        End(o);
                        return;
                    }
#endif
                    AssetBundleLoader ab = AssetBundleManager.Instance.GetAssetBundleLoader(info.AssetBundlePath);
                    if (ab == null ||
                        ab.AssetBundleContent == null)
                    {
                        AssetBundleManager.Instance.LoadAsync(assetName, default, info, (b) => {
                            End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0]));
                        });
                        return;
                    }
                    else
                    {
                        Object o = AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0]);
                        if (o != null)
                        {
                            End(o);
                            return;
                        }
            
                        ab.Expand(default);
                        End(AssetBundleManager.Instance.GetAssetObject(info.AssetBundlePath, info.AssetPath[0]));
                    }
                }

                void End(Object o = null)
                {
                    callback?.Invoke(o);
                }
            }
            
            
            private AssetInfo GetAssetInfoFromResource(string assetName, bool showTip = false)
            {
                if (ResourceMap.Mappings.TryGetValue(assetName, out string value))
                {
                    return new AssetInfo(AssetTypeEnum.RESOURCE, new []{value}, null, null);
                }

                if (showTip)
                {
                    LogF8.LogError("Resource找不到指定资源可用的索引：" + assetName);
                }
                return new AssetInfo(AssetTypeEnum.RESOURCE);
            }
            
            private AssetInfo GetAssetInfoFromAssetBundle(string assetName, bool remote = false, bool showTip = false)
            {
                if (AssetBundleMap.Mappings.TryGetValue(assetName, out AssetBundleMap.AssetMapping assetmpping))
                {
                    if (remote || ForceRemoteAssetBundle)
                    {
                        return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE, assetmpping.AssetPath, AssetBundleManager.GetRemoteAssetBundleCompletePath(), assetmpping.AbName);
                    }
                    else
                    {
                        return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE, assetmpping.AssetPath, AssetBundleManager.GetAssetBundlePathWithoutAb(assetName), assetmpping.AbName);
                    }
                }

                if (showTip)
                {
                    LogF8.LogError("AssetBundle找不到指定资源可用的索引：" + assetName);
                }
                return new AssetInfo(AssetTypeEnum.ASSET_BUNDLE);
            }
            
            /// <summary>
            /// 通过资源名称同步卸载。
            /// </summary>
            /// <param name="assetName">资源名称。</param>
            /// <param name="unloadAllLoadedObjects">完全卸载。</param>
            public void Unload(string assetName, bool unloadAllLoadedObjects = false)
            {
#if UNITY_EDITOR
                if (_isEditorMode)
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
                if (_isEditorMode)
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
                if (_isEditorMode)
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
                if (_isEditorMode)
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
            private Dictionary<string, string> findAssetPaths = new Dictionary<string, string>();
            private string SearchAsset(string assetName, System.Object type = default, AssetAccessMode mode = AssetAccessMode.UNKNOWN)
            {
                // 缓存路径
                if (findAssetPaths.TryGetValue(assetName, out string value))
                {
                    return value;
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
                        }
                    }
                    searchDirs.Add(System.IO.Path.Combine(URLSetting.AssetBundlesPath));
                }
                
                // 查找指定资源
                string[] guids = UnityEditor.AssetDatabase.FindAssets(assetName, searchDirs.ToArray());
                foreach (string guid in guids)
                {
                    // 将 GUID 转换为路径
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
    
                    if (type == default || ReferenceEquals(UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(assetPath), type))
                    {
                        if (Path.GetFileNameWithoutExtension(assetPath) == assetName)
                        {
                            findAssetPaths[assetName] = assetPath;
                            return assetPath;
                        }
                    }
                }
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
                ResourceMap.Mappings = Util.LitJson.ToObject<Dictionary<string, string>>(Resources.Load<TextAsset>(nameof(ResourceMap)).ToString());
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