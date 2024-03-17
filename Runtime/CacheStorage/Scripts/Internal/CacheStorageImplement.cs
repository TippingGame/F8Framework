using System;
using System.Collections;
using System.Collections.Generic;

namespace F8Framework.Core
{
    internal static class CacheStorageImplement
    {
        internal const string NAME = "CacheStorageImplement";

        internal static event Action onChangeCache;

        internal static CacheStorageConfig cacheConfig = new CacheStorageConfig();
        internal static CachePackage cachePackage;

        internal static long updateTime = 0;

        internal static bool Initialized = false;

        public static void Initialize(int maxCount, int maxSize, double reRequestTime,
            CacheRequestType defaultRequestType, double unusedPeriodTime, double removeCycle)
        {
            if (cacheConfig.setting == false)
            {
                cacheConfig = new CacheStorageConfig(maxCount, maxSize, reRequestTime, unusedPeriodTime, removeCycle,
                    defaultRequestType);
            }

            InitializeCore();
        }

        internal static void InitializeCore()
        {
            if (Initialized == false)
            {
                Util.MessagePack.Initialize(CacheStorageResolver.Instance);

                cachePackage = CachePackage.Load();
                if (cachePackage == null)
                {
                    cachePackage = new CachePackage();
                }

                updateTime = DateTime.UtcNow.Ticks;

                UnityEngine.Application.focusChanged += OnFocusChanged;
                UnityEngine.Application.quitting += OnQuit;

                ManagedCoroutine.Start(UpdateRoutine());

                Initialized = true;
            }
        }

        internal static bool IsSetting()
        {
            return cacheConfig.setting;
        }

        internal static int GetCacheCount()
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return 0;
            }

            return cachePackage.cacheStorage.Count;
        }

        internal static int GetMaxCount()
        {
            return cacheConfig.maxCount;
        }

        internal static long GetCacheSize()
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return 0;
            }

            return cachePackage.cachedSize;
        }

        internal static long GetMaxSize()
        {
            return cacheConfig.maxSize;
        }

        internal static double GetReRequestTime()
        {
            return cacheConfig.reRequestTime;
        }

        internal static CacheRequestType GetCacheRequestType()
        {
            return cacheConfig.defaultRequestType;
        }

        internal static double GetUnusedPeriodTime()
        {
            return cacheConfig.unusedPeriodTime;
        }

        internal static double GetRemoveCycle()
        {
            return cacheConfig.removeCycle;
        }

        internal static long GetRemoveCacheSize()
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return 0;
            }

            return cachePackage.removeCacheSize;
        }

        internal static CacheRequestOperation Request(string url, CacheRequestType requestType, double reRequestTime,
            Action<CacheResult> onResult)
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return null;
            }

            return cachePackage.Request(url, requestType, reRequestTime, onResult);
        }

        internal static CacheRequestOperation RequestHttpCache(string url, Action<CacheResult> onResult)
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return null;
            }

            return Request(url, CacheRequestType.ALWAYS, 0, onResult);
        }

        internal static CacheRequestOperation RequestLocalCache(string url, Action<CacheResult> onResult)
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return null;
            }

            return cachePackage.RequestLocal(url, onResult);
        }

        internal static CacheRequestOperation GetCachedTexture(string url, Action<CachedTexture> onResult)
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return null;
            }

            CacheInfo info = cachePackage.GetCacheInfo(url);
            if (info != null)
            {
                CachedTexture cachedTexture = CachedTextureManager.Get(info);
                if (cachedTexture != null &&
                    cachedTexture.texture != null)
                {
                    onResult?.SafeCallback(cachedTexture);
                    return new CacheRequestOperation(info);
                }
            }

            return RequestLocalCache(url, (result) =>
            {
                if (result.IsSuccess() == true)
                {
                    onResult?.SafeCallback(CachedTextureManager.Cache(result.Info, false, false, result.Data));
                }
                else
                {
                    onResult?.SafeCallback(null);
                }
            });
        }

        internal static CacheRequestOperation RequestTexture(string url, CacheRequestType requestType,
            double reRequestTime, bool preLoad, Action<CachedTexture> onResult)
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return null;
            }

            CacheInfo cachedInfo = cachePackage.GetCacheInfo(url);
            if (cachedInfo != null)
            {
                bool loaded = false;

                bool needRequest = cachedInfo.NeedRequest(requestType, reRequestTime);
                if (preLoad == true || needRequest == false)
                {
                    CachedTexture cachedTexture = CachedTextureManager.Get(cachedInfo);
                    if (cachedTexture != null &&
                        cachedTexture.texture != null)
                    {
                        onResult?.SafeCallback(cachedTexture);

                        cachedInfo.lastAccess = DateTime.UtcNow.Ticks;
                        SetDirty();

                        loaded = true;
                    }
                    else
                    {
                        if (cachePackage.IsValidCacheData(cachedInfo) == true)
                        {
                            byte[] datas = cachePackage.GetCacheData(cachedInfo);

                            onResult?.SafeCallback(CachedTextureManager.Cache(cachedInfo, false, false, datas));

                            cachedInfo.lastAccess = DateTime.UtcNow.Ticks;
                            SetDirty();

                            loaded = true;
                        }
                        else
                        {
                            cachedInfo.eTag = string.Empty;
                            cachedInfo.lastModified = string.Empty;
                            requestType = CacheRequestType.ALWAYS;
                        }
                    }
                }

                if (loaded == true &&
                    needRequest == false)
                {
                    return new CacheRequestOperation(cachedInfo);
                }
            }

            bool subRequest = false;
            if (cachePackage.GetRequestCache(url) != null)
            {
                subRequest = true;
            }

            return Request(url, requestType, 0, (result) =>
            {
                if (result.IsSuccess() == true)
                {
                    CachedTexture resultTexture = null;
                    if (subRequest == true)
                    {
                        resultTexture = CachedTextureManager.Get(result.Info);
                    }

                    if (resultTexture == null ||
                        resultTexture.texture == null)
                    {
                        resultTexture = CachedTextureManager.Cache(result.Info, true, result.UpdateData, result.Data);
                    }

                    onResult?.SafeCallback(resultTexture);
                }
                else
                {
                    onResult?.SafeCallback(null);
                }
            });
        }

        internal static void ClearCache()
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return;
            }

            cachePackage.RemoveAll();
        }

        internal static bool RemoveCache(string url)
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return false;
            }

            CacheInfo cachInfo = cachePackage.GetCacheInfo(url);
            if (cachInfo != null)
            {
                return cachePackage.RemoveCacheData(cachInfo, true);
            }

            return false;
        }

        private static IEnumerator UpdateRoutine()
        {
            while (cachePackage != null)
            {
                updateTime = DateTime.UtcNow.Ticks;

                cachePackage.Update();

                AutoDeleteUnusedCache();

                yield return null;
            }
        }

        private static void AutoDeleteUnusedCache()
        {
            if (GetUnusedPeriodTime() > 0 &&
                GetRemoveCycle() > 0)
            {
                cachePackage.SecuringStorageLastAccess(GetUnusedPeriodTime());
            }
        }

        internal static void AddChangeCacheEvnet(Action callback)
        {
            onChangeCache += callback;
        }

        internal static void ClearChangeCacheEvent()
        {
            onChangeCache = null;
        }

        internal static bool IsDirty()
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return false;
            }

            return cachePackage.IsDirty();
        }


        internal static void SetDirty(bool value = true)
        {
            if (Initialized == false)
            {
                LogF8.LogError("Not initialized" + NAME + typeof(CacheStorageImplement));
                return;
            }

            cachePackage.SetDirty(value);

            try
            {
                onChangeCache?.Invoke();
            }
            catch (Exception e)
            {
                LogF8.LogError(e);
            }
        }


        internal static void SavePackage()
        {
            cachePackage.Save();

            try
            {
                onChangeCache?.Invoke();
            }
            catch (Exception e)
            {
                LogF8.LogError(e);
            }
        }


        internal static void OnFocusChanged(bool focus)
        {
            if (focus == false)
            {
                if (cachePackage.IsDirty() == true)
                {
                    SavePackage();
                }
            }
        }

        internal static void OnQuit()
        {
            if (cachePackage.IsDirty() == true)
            {
                SavePackage();
            }
        }
    }
}