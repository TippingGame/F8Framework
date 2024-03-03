using System.Net;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using RequestCallback = System.Action<F8Framework.Core.CacheResult>;

namespace F8Framework.Core
{
    public enum CacheRequestType
    {
        ALWAYS,
        FIRSTPLAY,
        ONCE,
        LOCAL,
    }

    public enum CacheValidType
    {
        VALID = 0,
        NO_CACHED,
        NO_EXIST,
        OTHER_SIZE,
    }

    public enum CacheUpdateType
    {
        NONE = 0,
        NEW,
        RESTORE
    }

    [Serializable]
    public class CachePackage
    {
        public const string PACKAGE_NAME = "CacheStoragePackage";

        [SerializeField] public List<CacheInfo> cacheStorage = new List<CacheInfo>();

        [SerializeField] internal ulong lastIndex = 0;

        [SerializeField] internal long cachedSize = 0;

        [SerializeField] internal long removeCacheSize = 0;

        [SerializeField] public List<CacheInfo> removeCache = new List<CacheInfo>();

        private List<CacheInfo> requestCache = new List<CacheInfo>();

        private bool dirty = false;

        public StringToValueHttpTime lastRemoveTime;

        static private string cachePath;

        private void OnAfterDeserialize()
        {
            foreach (var info in cacheStorage)
            {
                info.storage = this;
            }

            foreach (var info in removeCache)
            {
                info.storage = this;
            }
        }

        public CacheInfo GetCacheInfo(string url)
        {
            foreach (CacheInfo cachInfo in cacheStorage)
            {
                if (cachInfo.url.Equals(url) == true)
                {
                    return cachInfo;
                }
            }

            return null;
        }

        internal CacheInfo GetRequestCache(string url)
        {
            foreach (var rq in requestCache)
            {
                if (rq.url.Equals(url) == true)
                {
                    return rq;
                }
            }

            return null;
        }

        public CacheRequestOperation RequestLocal(string url, RequestCallback onResult)
        {
            CacheInfo info = GetCacheInfo(url);

            if (onResult != null)
            {
                byte[] datas = null;
                if (info != null &&
                    IsValidCacheData(info) == true)
                {
                    datas = GetCacheData(info);
                }

                onResult.SafeCallback(new CacheResult(info, datas, false));
            }

            return new CacheRequestOperation(info);
        }

        public CacheRequestOperation Request(string url, CacheRequestType requestType, double reRequestTime,
            RequestCallback onResult)
        {
            bool onlyLocalCache = false;

            if (requestType == CacheRequestType.LOCAL ||
                Application.internetReachability == NetworkReachability.NotReachable)
            {
                onlyLocalCache = true;
            }
            else
            {
                CacheInfo requestInfo = GetRequestCache(url);
                if (requestInfo != null)
                {
                    requestInfo.AddCallback(onResult);
                    return new CacheRequestOperation(requestInfo, true, onResult);
                }
            }

            bool useCache = true;
            CacheInfo info = GetCacheInfo(url);
            if (info == null)
            {
                info = new CacheInfo(this, url);
                useCache = false;
            }

            if (onlyLocalCache == true)
            {
                byte[] datas = null;
                if (IsValidCacheData(info) == true)
                {
                    datas = GetCacheData(info);
                }

                return info.ReturnResult(datas, false, onResult);
            }

            if (useCache == true &&
                info.NeedRequest(requestType, reRequestTime) == false)
            {
                if (IsValidCacheData(info) == true)
                {
                    byte[] datas = GetCacheData(info);
                    if (datas != null)
                    {
                        return info.ReturnResult(datas, false, onResult);
                    }
                }

                useCache = false;
            }

            GpmWebRequest request = new GpmWebRequest();
            if (useCache == true)
            {
                if (string.IsNullOrEmpty(info.eTag) == false)
                {
                    request.SetRequestHeader("If-None-Match", info.eTag);
                }

                if (string.IsNullOrEmpty(info.lastModified) == false)
                {
                    request.SetRequestHeader("If-Modified-Since", info.lastModified);
                }
            }

            info.AddCallback(onResult);

            info.requestInPlay = true;
            info.requestTime = DateTime.UtcNow.Ticks;
            info.state = CacheInfo.State.REQUEST;

            requestCache.Add(info);

            request.Get(url, (requestResult) =>
            {
                info.state = CacheInfo.State.NONE;
                requestCache.Remove(info);

                if (requestResult.isSuccess == true)
                {
                    if (requestResult.responseCode == (long)HttpStatusCode.NotModified)
                    {
                        byte[] cachedDatas = null;
                        if (IsValidCacheData(info) == true)
                        {
                            cachedDatas = GetCacheData(info);
                        }

                        if (cachedDatas != null)
                        {
                            info.state = CacheInfo.State.CACHED;

                            Dictionary<string, string> cacheControlElements =
                                CacheControl.GetElements(
                                    requestResult.request.GetResponseHeader(HttpElement.CACHE_CONTROL));

                            CacheControl cacheControl = null;
                            bool noStore = false;
                            if (cacheControlElements.Count > 0)
                            {
                                noStore = cacheControlElements.ContainsKey(HttpElement.NO_STORE) == true;
                                cacheControl = new CacheControl(cacheControlElements);

                                info.cacheControl = cacheControl;
                            }

                            string expires = requestResult.request.GetResponseHeader(HttpElement.EXPIRES);
                            if (string.IsNullOrEmpty(expires) == false)
                            {
                                info.expires = expires;
                            }

                            string age = requestResult.request.GetResponseHeader(HttpElement.AGE);
                            if (string.IsNullOrEmpty(age) == false)
                            {
                                info.age = age;
                            }

                            string date = requestResult.request.GetResponseHeader(HttpElement.DATE);
                            if (string.IsNullOrEmpty(date) == false)
                            {
                                info.date = date;
                            }

                            info.responseTime = DateTime.UtcNow.Ticks;
                            info.needCaculateAge = true;
                            info.needCaculateLifetime = true;

                            info.SendResultAll(cachedDatas, false);
                        }
                        else
                        {
                            info.eTag = string.Empty;
                            info.lastModified = string.Empty;
                            Request(url, CacheRequestType.ALWAYS, 0, null);
                        }
                    }
                    else if (requestResult.responseCode == (long)HttpStatusCode.OK)
                    {
                        info.responseTime = DateTime.UtcNow.Ticks;

                        CacheControl cacheControl = null;
                        Dictionary<string, string> cacheControlElements =
                            CacheControl.GetElements(
                                requestResult.request.GetResponseHeader(HttpElement.CACHE_CONTROL));

                        bool noStore = false;
                        if (cacheControlElements.Count > 0)
                        {
                            noStore = cacheControlElements.ContainsKey(HttpElement.NO_STORE) == true;
                            cacheControl = new CacheControl(cacheControlElements);
                        }

                        CacheUpdateType updateType = CacheUpdateType.NEW;
                        byte[] datas = requestResult.request.downloadHandler.data;

                        if (noStore == true)
                        {
                            if (info.IsCached() == true)
                            {
                                RemoveCacheData(info);
                            }
                        }
                        else
                        {
                            if (info.IsCached() == true)
                            {
                                if (info.contentLength == datas.Length)
                                {
                                    CacheValidType vaildType = CheckValidCacheData(info);

                                    if (vaildType == CacheValidType.VALID)
                                    {
                                        byte[] cachedDatas = GetCacheData(info);

                                        if (ByteArraysEqual(cachedDatas, datas) == true)
                                        {
                                            updateType = CacheUpdateType.NONE;
                                        }
                                    }
                                    else if (vaildType == CacheValidType.NO_EXIST)
                                    {
                                        updateType = CacheUpdateType.RESTORE;
                                    }
                                    else
                                    {
                                        updateType = CacheUpdateType.NEW;
                                    }
                                }

                                if (updateType.Equals(CacheUpdateType.NEW) == true)
                                {
                                    RemoveCacheData(info);
                                }
                            }

                            info.cacheControl = cacheControl;

                            info.eTag = requestResult.request.GetResponseHeader(HttpElement.ETAG);
                            info.lastModified = requestResult.request.GetResponseHeader(HttpElement.LAST_MODIFIED);

                            info.expires = requestResult.request.GetResponseHeader(HttpElement.EXPIRES);

                            info.age = requestResult.request.GetResponseHeader(HttpElement.AGE);
                            info.date = requestResult.request.GetResponseHeader(HttpElement.DATE);

                            info.needCaculateAge = true;
                            info.needCaculateLifetime = true;

                            info.contentLength = 0;
                            if (datas != null)
                            {
                                info.contentLength = datas.LongLength;

                                if (updateType == CacheUpdateType.NEW)
                                {
                                    AddCacheData(info, datas);
                                }
                                else if (updateType == CacheUpdateType.RESTORE)
                                {
                                    SaveCacheData(info, datas);
                                }
                                else
                                {
                                    CacheStorageImplement.SetDirty();
                                }
                            }
                        }

                        bool updateData = updateType == CacheUpdateType.NEW;
                        info.SendResultAll(datas, updateData);
                    }
                    else
                    {
                        info.SendResultAll(null, false);
                    }
                }
                else
                {
                    info.SendResultAll(null, false);
                }
            });

            return new CacheRequestOperation(info, true, onResult);
        }

        private bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2)
            {
                return true;
            }

            if (b1 == null || b2 == null)
            {
                return false;
            }

            if (b1.Length != b2.Length)
            {
                return false;
            }

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }

            return true;
        }

        internal bool IsRequest(CacheInfo info)
        {
            return requestCache.Contains(info);
        }

        private bool IsCached(CacheInfo info)
        {
            return info.index > 0;
        }

        internal string GetCacheDataPath(CacheInfo info)
        {
            if (IsCached(info) == true)
            {
                return Path.Combine(GetCachePath(), info.index.ToString());
            }

            return string.Empty;
        }

        private void SaveCacheData(CacheInfo info, byte[] data)
        {
            if (IsCached(info) == true)
            {
                if (Directory.Exists(GetCachePath()) == false)
                {
                    Directory.CreateDirectory(GetCachePath());
                }

                string filePath = GetCacheDataPath(info);

                File.WriteAllBytes(filePath, data);
            }
        }

        public CacheValidType CheckValidCacheData(CacheInfo info)
        {
            if (IsCached(info) == true)
            {
                string filePath = GetCacheDataPath(info);

                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists == false)
                {
                    return CacheValidType.NO_EXIST;
                }
                else if (fileInfo.Length != info.contentLength)
                {
                    return CacheValidType.OTHER_SIZE;
                }
                else
                {
                    return CacheValidType.VALID;
                }
            }
            else
            {
                return CacheValidType.NO_CACHED;
            }
        }

        public bool IsValidCacheData(CacheInfo info)
        {
            return CheckValidCacheData(info).Equals(CacheValidType.VALID);
        }

        public byte[] GetCacheData(CacheInfo info)
        {
            byte[] data = null;
            if (IsCached(info) == true)
            {
                try
                {
                    string filePath = GetCacheDataPath(info);
                    data = File.ReadAllBytes(filePath);
                }
                catch (Exception exception)
                {
                    LogF8.LogWarning(exception.Message + "（GetCacheData）");
                }
            }

            return data;
        }

        public string GetCacheData(CacheInfo info, System.Text.Encoding encoding = null)
        {
            byte[] data = GetCacheData(info);

            if (data != null)
            {
                if (encoding == null)
                {
                    encoding = System.Text.Encoding.Default;
                }

                return encoding.GetString(data);
            }

            return string.Empty;
        }

        private void AddCacheData(CacheInfo info, byte[] datas)
        {
            long maxCount = CacheStorageImplement.GetMaxCount();
            if (maxCount > 0)
            {
                SecuringStorageCount(1);
            }

            long maxSize = CacheStorageImplement.GetMaxSize();
            if (maxSize > 0)
            {
                SecuringStorage(maxSize, datas.LongLength);
            }

            info.index = ++lastIndex;

            cachedSize += info.contentLength;
            info.state = CacheInfo.State.CACHED;

            SaveCacheData(info, datas);

            cacheStorage.Add(info);

            CacheStorageImplement.SetDirty();
        }

        private void CacheSort()
        {
            cacheStorage.Sort();
        }

        internal ulong GetLastIndex()
        {
            return lastIndex;
        }

        internal bool IsDirty()
        {
            return dirty;
        }

        internal void SetDirty(bool value = true)
        {
            this.dirty = value;
        }

        internal void SecuringStorageLastAccess(double unusedTime)
        {
            List<CacheInfo> newExpired = new List<CacheInfo>();
            for (int i = 0; i < cacheStorage.Count; i++)
            {
                if (cacheStorage[i].IsLastAccessPeriod(unusedTime) == true)
                {
                    newExpired.Add(cacheStorage[i]);
                }
            }

            for (int i = 0; i < newExpired.Count; i++)
            {
                if (RemoveCacheData(newExpired[i]) == false)
                {
                    break;
                }
            }
        }

        private void SecuringStorageCount(int addCount = 0)
        {
            long maxCount = CacheStorageImplement.GetMaxCount();
            if (maxCount <= 0)
            {
                return;
            }

            if (cacheStorage.Count + addCount > maxCount)
            {
                CacheSort();
            }

            while (cacheStorage.Count + addCount > maxCount)
            {
                if (RemoveCacheData(cacheStorage.Last<CacheInfo>(), true) == false)
                {
                    break;
                }
            }
        }


        private void SecuringStorage(long maxSize, long addSize = 0)
        {
            if (maxSize == 0)
            {
                return;
            }

            if (cachedSize + removeCacheSize + addSize > maxSize)
            {
                if (removeCache.Count > 0)
                {
                    while (removeCache.Count > 0 &&
                           cachedSize + removeCacheSize + addSize > maxSize)
                    {
                        DeleteRemoveCache();
                    }

                    if (removeCache.Count == 0)
                    {
                        removeCacheSize = 0;
                    }
                }

                if (cacheStorage.Count > 0)
                {
                    CacheSort();

                    while (cacheStorage.Count > 0 &&
                           cachedSize + addSize > maxSize)
                    {
                        if (RemoveCacheData(cacheStorage.Last<CacheInfo>(), true) == false)
                        {
                            break;
                        }
                    }
                }
            }
        }

        internal bool RemoveCacheData(CacheInfo info, bool immediately = false)
        {
            if (cacheStorage.Remove(info) == true)
            {
                if (info.IsCached() == true)
                {
                    if (immediately == true)
                    {
                        DeleteCacheData(info);
                    }
                    else
                    {
                        CacheInfo removeInfo = new CacheInfo(info);
                        removeInfo.state = CacheInfo.State.REMOVE;
                        removeCache.Add(removeInfo);

                        removeCacheSize += removeInfo.contentLength;
                    }

                    cachedSize -= info.contentLength;

                    CacheStorageImplement.SetDirty();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool DeleteCacheData(CacheInfo info)
        {
            try
            {
                if (info.IsCached() == true)
                {
                    string filePath = GetCacheDataPath(info);

                    File.Delete(filePath);
                }

                return true;
            }
            catch (DirectoryNotFoundException)
            {
                return true;
            }
            catch (Exception exception)
            {
                LogF8.LogWarning(exception.Message + "（DeleteCacheData）");
            }

            return false;
        }

        internal void RemoveAll()
        {
            foreach (CacheInfo info in cacheStorage)
            {
                DeleteCacheData(info);
            }

            lastIndex = 0;
            cachedSize = 0;
            cacheStorage.Clear();

            CacheStorageImplement.SetDirty();
        }

        internal void Update()
        {
            if (CanRemove() == true)
            {
                DeleteRemoveCache();
            }
        }

        private CacheInfo DeleteRemoveCache()
        {
            int removeIndex = 0;
            for (int idx = 0; idx < removeCache.Count; idx++)
            {
                if (removeCache[removeIndex].index < removeCache[idx].index)
                {
                    removeIndex = idx;
                }
            }

            CacheInfo removeInfo = null;

            if (removeIndex < removeCache.Count)
            {
                removeInfo = removeCache[removeIndex];

                DeleteCacheData(removeInfo);
                removeCache.RemoveAt(removeIndex);
                removeCacheSize -= removeInfo.contentLength;

                lastRemoveTime = DateTime.UtcNow;

                CacheStorageImplement.SetDirty();
            }

            return removeInfo;
        }

        private bool CanRemove()
        {
            if (removeCache.Count > 0)
            {
                double removePeriodTime = CacheStorageImplement.GetRemoveCycle();
                if (removePeriodTime > 0)
                {
                    if (lastRemoveTime == null ||
                        (DateTime.UtcNow - lastRemoveTime).TotalSeconds > removePeriodTime)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        internal static string GetCachePath()
        {
            if (string.IsNullOrEmpty(cachePath) == true)
            {
                cachePath = Path.Combine(Application.persistentDataPath, CacheStorage.NAME);
#if UNITY_IOS
                UnityEngine.iOS.Device.SetNoBackupFlag(cachePath);
#endif
            }

            return cachePath;
        }

        internal static string PackagePath()
        {
            return Path.Combine(GetCachePath(), PACKAGE_NAME);
        }

        internal static CachePackage Load()
        {
            try
            {
                string path = PackagePath();

                byte[] bytes = File.ReadAllBytes(path);

                CachePackage cachePackage = Util.MessagePack.Deserialize<CachePackage>(bytes);
                cachePackage.OnAfterDeserialize();

                return cachePackage;
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception exception)
            {
                LogF8.LogWarning(exception.Message + "（Load）");
            }


            return null;
        }

        internal void Save()
        {
            try
            {
                string cachePath = GetCachePath();
                if (Directory.Exists(cachePath) == false)
                {
                    Directory.CreateDirectory(cachePath);
                }

                string path = PackagePath();

                byte[] bytes = Util.MessagePack.SerializeUnsafe<CachePackage>(this);

                File.WriteAllBytes(path, bytes);

                dirty = false;
            }
            catch (Exception exception)
            {
                LogF8.LogWarning(exception.Message + "（Save）");
            }
        }
    }
}