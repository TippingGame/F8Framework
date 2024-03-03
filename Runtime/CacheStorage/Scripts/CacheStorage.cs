using System;

namespace F8Framework.Core
{
    /// <summary>
    /// The GpmCacheStorage class is core of CacheStorage service.
    /// </summary>
    public static class CacheStorage
    {
        public const string NAME = "CacheStorage";
        public const string VERSION = "1.3.1";

        /// <summary>
        /// This function will know that CacheStorage has been initialized.
        /// If not initialized, the CacheStorage function will not work.
        /// @since Added 1.2.0.
        /// </summary>
        /// <returns>Whether CacheStorage has been initialized.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void InitializedSample()
        /// {
        ///     bool Initialized = GpmCacheStorage.Initialized;
        ///     Debug.Log(string.Format("Initialized:{0}", Initialized));
        /// }
        /// </code>
        /// </example>
        public static bool Initialized
        {
            get { return CacheStorageImplement.Initialized; }
        }

        /// <summary>
        /// This function to initialize CacheStorage with parameter.
        /// If this function is not called, the CacheStorage function will not work.
        /// @since Added 1.2.0.
        /// </summary>
        /// <param name="maxCount">
        /// The maxCount is maximum number of CacheStorage.
        /// A value of 0 is unlimited.
        /// </param>
        /// <param name="maxSize">
        /// The maxSize is maximum capacity of CacheStorage.
        /// A value of 0 is unlimited.
        /// </param>
        /// <param name="reRequestTime">
        /// The reRequestTime is the frequency of revalidation requests. 
        /// If the time set when requesting cache has passed, It is revalidated. 
        /// base is seconds. 
        /// A value of 0 is ignored.
        /// </param>
        /// <param name="defaultRequestType">
        /// The defaultRequestType is basically the type used when calling Request.
        /// Validate content according to RequestType.
        /// * ALWAYS
        ///     * Validate that data has changed on the server at every request.
        /// * FIRSTPLAY
        ///     * It is revalidated once every time the app is launched.
        /// * ONCE
        ///     * No revalidation within the validity period.
        /// * LOCAL
        ///     * Uses cached data.
        /// </param>
        /// <param name="unusedPeriodTime">
        /// Caches that have not been used for the number of seconds set by the unusedPeriodTime are automatically discarded.
        /// Automatic deletion works only when unusedPeriodTime and removeCycle are greater than 0.
        /// </param>
        /// <param name="removeCycle">
        /// Removes the cache of the destination to be removed every corresponding number of seconds. 
        /// Reduce the impact by limiting deleting many files at once.
        /// Automatic deletion works only when unusedPeriodTime and removeCycle are greater than 0.
        /// </param>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void InitializeSample()
        /// {
        ///     int maxCount = 10000;
        ///     int maxSize = 5 * 1024 * 1024; // 5 MB
        ///     double reRequestTime = 30 * 24 * 60 * 60; // 30 Days
        ///     CacheRequestType defaultRequestType = CacheRequestType.FIRSTPLAY;
        ///     double unusedPeriodTime = 365 * 24 * 60 * 60; // 1 Years
        ///     double removeCycle = 1; // 1 Seconds
        ///     
        ///     GpmCacheStorage.Initialize(maxCount, maxSize, reRequestTime, defaultRequestType, unusedPeriodTime, removeCycle);
        /// }
        /// </code>
        /// </example>
        public static void Initialize(int maxCount, int maxSize, double reRequestTime,
            CacheRequestType defaultRequestType, double unusedPeriodTime, double removeCycle)
        {
            CacheStorageImplement.Initialize(maxCount, maxSize, reRequestTime, defaultRequestType, unusedPeriodTime,
                removeCycle);
        }

        /// <summary>
        /// Get the number of caches used.
        /// </summary>
        /// <returns>The number of caches used.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetCacheCountSample()
        /// {
        ///     int cacheCount = GpmCacheStorage.GetCacheCount();
        ///     Debug.Log(string.Format("cacheCount : {0}", cacheCount));
        /// }
        /// </code>
        /// </example>
        public static int GetCacheCount()
        {
            return CacheStorageImplement.GetCacheCount();
        }

        /// <summary>
        /// Get the maximum number of caches to manage.
        /// </summary>
        /// <returns>The maximum number of caches to manage.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetMaxCountSample()
        /// {
        ///     int cacheMaxCount = GpmCacheStorage.GetMaxCount();
        ///     Debug.Log(string.Format("cacheMaxCount : {0}", cacheMaxCount));
        /// }
        /// </code>
        /// </example>
        public static int GetMaxCount()
        {
            return CacheStorageImplement.GetMaxCount();
        }

        /// <summary>
        /// Get the size of the cache capacity used.
        /// </summary>
        /// <returns>The size of the cache capacity used.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetCacheSizeSample()
        /// {
        ///     long cacheSize = GpmCacheStorage.GetCacheSize();
        ///     Debug.Log(string.Format("cacheSize : {0}", cacheSize));
        /// }
        /// </code>
        /// </example>
        public static long GetCacheSize()
        {
            return CacheStorageImplement.GetCacheSize();
        }

        /// <summary>
        /// Get the size of the maximum cache capacity to manage.
        /// </summary>
        /// <returns>The size of the maximum cache capacity to manage.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetMaxSizeSample()
        /// {
        ///     long cacheMaxSize = GpmCacheStorage.GetMaxSize();
        ///     Debug.Log(string.Format("cacheMaxSize : {0}", cacheMaxSize));
        /// }
        /// </code>
        /// </example>
        public static long GetMaxSize()
        {
            return CacheStorageImplement.GetMaxSize();
        }

        /// <summary>
        /// Get the period time to be revalidated.
        /// If the time set when requesting cache has passed, It is revalidated. 
        /// base is seconds. 
        /// A value of 0 is ignored.
        /// </summary>
        /// <returns>the period time to be revalidated.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetReRequestTimeSample()
        /// {
        ///     double reRequestTime = GpmCacheStorage.GetReRequestTime();
        ///     Debug.Log(string.Format("reRequestTime : {0} seconds", reRequestTime));
        /// }
        /// </code>
        /// </example>
        public static double GetReRequestTime()
        {
            return CacheStorageImplement.GetReRequestTime();
        }

        /// <summary>
        /// Get the RequestType to use by default when requesting.
        /// Validate content according to RequestType.
        /// * ALWAYS
        ///     * Validate that data has changed on the server at every request.
        /// * FIRSTPLAY
        ///     * It is revalidated once every time the app is launched.
        /// * ONCE
        ///     * No revalidation within the validity period.
        /// * LOCAL
        ///     * Uses cached data.
        /// </summary>
        /// <returns>the period time to be revalidated.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetCacheRequestTypeSample()
        /// {
        ///     CacheRequestType cacheRequestType = GpmCacheStorage.GetCacheRequestType();
        ///     Debug.Log(string.Format("cacheRequestType : {0}", cacheRequestType));
        /// }
        /// </code>
        /// </example>
        public static CacheRequestType GetCacheRequestType()
        {
            return CacheStorageImplement.GetCacheRequestType();
        }

        /// <summary>
        /// Get the cache duration time to automatically delete.
        /// Caches that have not been used for the number of seconds set by the unusedPeriodTime are automatically discarded.
        /// Automatic deletion works only when unusedPeriodTime and removeCycle are greater than 0.
        /// </summary>
        /// <returns>The cache duration time to automatically delete.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetUnusedPeriodTimeSample()
        /// {
        ///     double unusedPeriodTime = GpmCacheStorage.GetUnusedPeriodTime();
        ///     Debug.Log(string.Format("unusedPeriodTime : {0} seconds", unusedPeriodTime));
        /// }
        /// </code>
        /// </example>
        public static double GetUnusedPeriodTime()
        {
            return CacheStorageImplement.GetUnusedPeriodTime();
        }

        /// <summary>
        /// Get cycles that are automatically deleted.
        /// Removes the cache of the destination to be removed every corresponding number of seconds. 
        /// Reduce the impact by limiting deleting many files at once.
        /// Automatic deletion works only when unusedPeriodTime and removeCycle are greater than 0.
        /// </summary>
        /// <returns>The cache duration time to automatically delete.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetRemoveCycleSample()
        /// {
        ///     double removeCycle = GpmCacheStorage.GetRemoveCycle();
        ///     Debug.Log(string.Format("removeCycle : {0} seconds", removeCycle));
        /// }
        /// </code>
        /// </example>
        public static double GetRemoveCycle()
        {
            return CacheStorageImplement.GetUnusedPeriodTime();
        }

        /// <summary>
        /// Get the size of the cache capacity to be deleted.
        /// </summary>
        /// <returns>The size of the cache capacity to be deleted.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetRemoveCacheSizeSample()
        /// {
        ///     long removeCacheSize = GpmCacheStorage.GetRemoveCacheSize();
        ///     Debug.Log(string.Format("removeCacheSize : {0}", removeCacheSize));
        /// }
        /// </code>
        /// </example>
        public static long GetRemoveCacheSize()
        {
            return CacheStorageImplement.GetRemoveCacheSize();
        }

        /// <summary>
        /// This function requests web data to be cached.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestSample()
        /// {
        ///     string url;
        ///     GpmCacheStorage.Request(url, (result) =>
        ///     {
        ///         if (result.IsSuccess() == true)
        ///         {
        ///             bytes[] data = result.Data;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        /// 
        public static CacheRequestOperation Request(string url, Action<CacheResult> onResult)
        {
            return Request(url, GetCacheRequestType(), onResult);
        }

        /// <summary>
        /// This function requests web data to be cached.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="requestType">
        /// RequestType is the type that validates the content when requesting it.
        /// The defaultRequestType set in Initialize is ignored.
        /// Validate content according to RequestType.
        /// * ALWAYS
        ///     * Validate that data has changed on the server at every request.
        /// * FIRSTPLAY
        ///     * It is revalidated once every time the app is launched.
        /// * ONCE
        ///     * No revalidation within the validity period.
        /// * LOCAL
        ///     * Uses cached data.
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestSample()
        /// {
        ///     string url;
        ///     GpmCacheStorage.Request(url, CacheRequestType.ALWAYS, (result) =>
        ///     {
        ///         if (result.IsSuccess() == true)
        ///         {
        ///             bytes[] data = result.Data;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation Request(string url, CacheRequestType requestType,
            Action<CacheResult> onResult)
        {
            return Request(url, requestType, 0, onResult);
        }

        /// <summary>
        /// This function requests web data to be cached.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="reRequestTime">
        /// The period time to be revalidated.
        /// If the time set when requesting cache has passed, It is revalidated. 
        /// base is seconds. 
        /// If the value is about 0, the reRequestTime set in the Initialize function is used.
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestSample()
        /// {
        ///     string url;
        ///     double reRequestTime = 30 * 24 * 60 * 60; // 30 Days
        ///     GpmCacheStorage.Request(url, reRequestTime, (result) =>
        ///     {
        ///         if (result.IsSuccess() == true)
        ///         {
        ///             bytes[] data = result.Data;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation Request(string url, double reRequestTime, Action<CacheResult> onResult)
        {
            return Request(url, GetCacheRequestType(), reRequestTime, onResult);
        }

        /// <summary>
        /// This function requests web data to be cached.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="requestType">
        /// RequestType is the type that validates the content when requesting it.
        /// The defaultRequestType set in Initialize is ignored.
        /// Validate content according to RequestType.
        /// * ALWAYS
        ///     * Validate that data has changed on the server at every request.
        /// * FIRSTPLAY
        ///     * It is revalidated once every time the app is launched.
        /// * ONCE
        ///     * No revalidation within the validity period.
        /// * LOCAL
        ///     * Uses cached data.
        /// </param>
        /// <param name="reRequestTime">
        /// The period time to be revalidated.
        /// If the time set when requesting cache has passed, It is revalidated. 
        /// base is seconds. 
        /// If the value is about 0, the reRequestTime set in the Initialize function is used.
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestSample()
        /// {
        ///     string url;
        ///     double reRequestTime = 30 * 24 * 60 * 60; // 30 Days
        ///     GpmCacheStorage.Request(url, CacheRequestType.ALWAYS, reRequestTime, (result) =>
        ///     {
        ///         if (result.IsSuccess() == true)
        ///         {
        ///             bytes[] data = result.Data;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation Request(string url, CacheRequestType requestType, double reRequestTime,
            Action<CacheResult> onResult)
        {
            return CacheStorageImplement.Request(url, requestType, reRequestTime, onResult);
        }

        /// <summary>
        /// This function requests web data to be cached.
        /// Equivalent to Request function with CacheRequestType of ALWAYS.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestHttpCacheSample()
        /// {
        ///     string url;
        ///     GpmCacheStorage.RequestHttpCache(url, (result) =>
        ///     {
        ///         if (result.IsSuccess() == true)
        ///         {
        ///             bytes[] data = result.Data;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestHttpCache(string url, Action<CacheResult> onResult)
        {
            return CacheStorageImplement.RequestHttpCache(url, onResult);
        }

        /// <summary>
        /// This function request already cached data by url. Fails if not cached.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestLocalCacheSample()
        /// {
        ///     string url;
        ///     GpmCacheStorage.RequestLocalCache(url, (result) =>
        ///     {
        ///         if (result.IsSuccess() == true)
        ///         {
        ///             bytes[] data = result.Data;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestLocalCache(string url, Action<CacheResult> onResult)
        {
            return CacheStorageImplement.RequestLocalCache(url, onResult);
        }

        /// <summary>
        /// This function request an already cached texture by url. If the texture is loaded after running the app, it will be reused.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void GetCachedTextureSample()
        /// {
        ///     string url;
        ///     GpmCacheStorage.GetCachedTexture(url, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation GetCachedTexture(string url, Action<CachedTexture> onResult)
        {
            return CacheStorageImplement.GetCachedTexture(url, onResult);
        }

        /// <summary>
        /// This function Request cached data by url. 
        /// If the texture is loaded after running the app, it will be reused. 
        /// If the cached data and web data are the same data, the cached texture is loaded and used.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestTextureSample()
        /// {
        ///     string url;
        ///     GpmCacheStorage.RequestTexture(url, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestTexture(string url, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, GetCacheRequestType(), 0, false, onResult);
        }

        /// <summary>
        /// This function Request cached data by url. 
        /// If the texture is loaded after running the app, it will be reused. 
        /// If the cached data and web data are the same data, the cached texture is loaded and used.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="preLoad">
        /// The preLoad is Read pre-stored cache before verifying on the web.
        /// If the content has changed since validation, The onResult is called again 
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestTextureSample()
        /// {
        ///     string url;
        ///     bool preLoad = true;
        ///     GpmCacheStorage.RequestTexture(url, preLoad, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestTexture(string url, bool preLoad, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, GetCacheRequestType(), 0, preLoad, onResult);
        }

        /// <summary>
        /// This function Request cached data by url. 
        /// If the texture is loaded after running the app, it will be reused. 
        /// If the cached data and web data are the same data, the cached texture is loaded and used.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="reRequestTime">
        /// The period time to be revalidated.
        /// If the time set when requesting cache has passed, It is revalidated. 
        /// base is seconds. 
        /// If the value is about 0, the reRequestTime set in the Initialize function is used.
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestTextureSample()
        /// {
        ///     string url;
        ///     double reRequestTime = 30 * 24 * 60 * 60; // 30 Days
        ///     GpmCacheStorage.RequestTexture(url, reRequestTime, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestTexture(string url, double reRequestTime,
            Action<CachedTexture> onResult)
        {
            return RequestTexture(url, GetCacheRequestType(), reRequestTime, false, onResult);
        }

        /// <summary>
        /// This function Request cached data by url. 
        /// If the texture is loaded after running the app, it will be reused. 
        /// If the cached data and web data are the same data, the cached texture is loaded and used.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="reRequestTime">
        /// The period time to be revalidated.
        /// If the time set when requesting cache has passed, It is revalidated. 
        /// base is seconds. 
        /// If the value is about 0, the reRequestTime set in the Initialize function is used.
        /// </param>
        /// <param name="preLoad">
        /// The preLoad is Read pre-stored cache before verifying on the web.
        /// If the content has changed since validation, The onResult is called again 
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestTextureSample()
        /// {
        ///     string url;
        ///     double reRequestTime = 30 * 24 * 60 * 60; // 30 Days
        ///     bool preLoad = true;
        ///     GpmCacheStorage.RequestTexture(url, reRequestTime, preLoad, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestTexture(string url, double reRequestTime, bool preLoad,
            Action<CachedTexture> onResult)
        {
            return RequestTexture(url, GetCacheRequestType(), reRequestTime, preLoad, onResult);
        }

        /// <summary>
        /// This function Request cached data by url. 
        /// If the texture is loaded after running the app, it will be reused. 
        /// If the cached data and web data are the same data, the cached texture is loaded and used.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="requestType">
        /// RequestType is the type that validates the content when requesting it.
        /// The defaultRequestType set in Initialize is ignored.
        /// Validate content according to RequestType.
        /// * ALWAYS
        ///     * Validate that data has changed on the server at every request.
        /// * FIRSTPLAY
        ///     * It is revalidated once every time the app is launched.
        /// * ONCE
        ///     * No revalidation within the validity period.
        /// * LOCAL
        ///     * Uses cached data.
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestTextureSample()
        /// {
        ///     string url;
        ///     CacheRequestType requestType = CacheRequestType.ALWAYS
        ///     GpmCacheStorage.RequestTexture(url, requestType, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestTexture(string url, CacheRequestType requestType,
            Action<CachedTexture> onResult)
        {
            return RequestTexture(url, requestType, 0, false, onResult);
        }

        /// <summary>
        /// This function Request cached data by url. 
        /// If the texture is loaded after running the app, it will be reused. 
        /// If the cached data and web data are the same data, the cached texture is loaded and used.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="requestType">
        /// RequestType is the type that validates the content when requesting it.
        /// The defaultRequestType set in Initialize is ignored.
        /// Validate content according to RequestType.
        /// * ALWAYS
        ///     * Validate that data has changed on the server at every request.
        /// * FIRSTPLAY
        ///     * It is revalidated once every time the app is launched.
        /// * ONCE
        ///     * No revalidation within the validity period.
        /// * LOCAL
        ///     * Uses cached data.
        /// </param>
        /// <param name="preLoad">
        /// The preLoad is Read pre-stored cache before verifying on the web.
        /// If the content has changed since validation, The onResult is called again 
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestTextureSample()
        /// {
        ///     string url;
        ///     CacheRequestType requestType = CacheRequestType.ALWAYS
        ///     bool preLoad = true;
        ///     GpmCacheStorage.RequestTexture(url, requestType, preLoad, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestTexture(string url, CacheRequestType requestType, bool preLoad,
            Action<CachedTexture> onResult)
        {
            return RequestTexture(url, requestType, 0, preLoad, onResult);
        }

        /// <summary>
        /// This function Request cached data by url. 
        /// If the texture is loaded after running the app, it will be reused. 
        /// If the cached data and web data are the same data, the cached texture is loaded and used.
        /// </summary>
        /// <param name="url">The url is the web address to request.</param>
        /// <param name="requestType">
        /// RequestType is the type that validates the content when requesting it.
        /// The defaultRequestType set in Initialize is ignored.
        /// Validate content according to RequestType.
        /// * ALWAYS
        ///     * Validate that data has changed on the server at every request.
        /// * FIRSTPLAY
        ///     * It is revalidated once every time the app is launched.
        /// * ONCE
        ///     * No revalidation within the validity period.
        /// * LOCAL
        ///     * Uses cached data.
        /// </param>
        /// <param name="reRequestTime">
        /// The period time to be revalidated.
        /// If the time set when requesting cache has passed, It is revalidated. 
        /// base is seconds. 
        /// If the value is about 0, the reRequestTime set in the Initialize function is used.
        /// </param>
        /// <param name="preLoad">
        /// The preLoad is Read pre-stored cache before verifying on the web.
        /// If the content has changed since validation, The onResult is called again 
        /// </param>
        /// <param name="onResult">This is a callback function that receives the results of cached data.</param>
        /// <returns>Cache information being requested.</returns>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RequestTextureSample()
        /// {
        ///     string url;
        ///     CacheRequestType requestType = CacheRequestType.ALWAYS
        ///     double reRequestTime = 30 * 24 * 60 * 60; // 30 Days
        ///     bool preLoad = true;
        ///     GpmCacheStorage.RequestTexture(url, requestType, reRequestTime, preLoad, (cachedTexture) =>
        ///     {
        ///         if (cachedTexture != null)
        ///         {
        ///             Texture texture = cachedTexture.texture;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public static CacheRequestOperation RequestTexture(string url, CacheRequestType requestType,
            double reRequestTime, bool preLoad, Action<CachedTexture> onResult)
        {
            return CacheStorageImplement.RequestTexture(url, requestType, reRequestTime, preLoad, onResult);
        }

        /// <summary>
        /// This function remove the all managed cache.
        /// </summary>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void ClearCacheSample()
        /// {
        ///     GpmCacheStorage.ClearCache();
        /// }
        /// </code>
        /// </example>
        public static void ClearCache()
        {
            CacheStorageImplement.ClearCache();
        }

        /// <summary>
        /// This function remove the managed cache.
        /// </summary>
        /// @since Added 1.3.0.
        /// <param name="url">The url is the web address to request.</param>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void RemoveCacheSample()
        /// {
        ///     string url;
        ///     GpmCacheStorage.RemoveCache(url);
        /// }
        /// </code>
        /// </example>
        public static bool RemoveCache(string url)
        {
            return CacheStorageImplement.RemoveCache(url);
        }

        /// <summary>
        /// This function adds an event when the cache changes.
        /// </summary>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void AddChangeCacheEvnetSample()
        /// {
        ///     GpmCacheStorage.AddChangeCacheEvnet( () =>
        ///     {
        ///         Debug.Log("ChangeCache");
        ///     });
        /// }
        /// </code>
        /// </example>
        public static void AddChangeCacheEvnet(Action callback)
        {
            CacheStorageImplement.AddChangeCacheEvnet(callback);
        }

        /// <summary>
        /// This function clears the event when the cache is changed.
        /// </summary>
        /// <example> 
        /// Example Usage : 
        /// <code>
        /// public void ClearChangeCacheEventSample()
        /// {
        ///     GpmCacheStorage.ClearChangeCacheEvent();
        /// }
        /// </code>
        /// </example>
        public static void ClearChangeCacheEvent()
        {
            CacheStorageImplement.ClearChangeCacheEvent();
        }
    }
}