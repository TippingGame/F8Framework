using UnityEngine;
using System;
using RequestCallback = System.Action<F8Framework.Core.CacheResult>;

namespace F8Framework.Core
{
    [Serializable]
    public class CacheInfo : IComparable<CacheInfo>
    {
        private const long SECONDS_PER_DAY = 86400;

        public enum State
        {
            NONE = 0,
            REQUEST,
            CACHED,
            EXPIRED,
            REMOVE,
        }

        [NonSerialized] internal CachePackage storage;

        [SerializeField] public string url;

        [SerializeField] public string eTag;

        [SerializeField] public StringToValueHttpTime lastModified;

        [SerializeField] public long lastAccess;

        [SerializeField] public StringToValueHttpTime expires;

        [SerializeField] public long requestTime;

        [SerializeField] public long responseTime;

        [SerializeField] public StringToValue<int> age;

        [SerializeField] public StringToValueHttpTime date;

        [SerializeField] public CacheControl cacheControl;

        [SerializeField] public long contentLength;


        private int initialAge = 0;
        internal bool needCaculateAge = true;

        private int freshnessLifetime = 0;
        internal bool needCaculateLifetime = true;

        [SerializeField] internal State state;

        [SerializeField] internal ulong index;

        internal bool requestInPlay = false;

        private long lastCheckTime = 0;

        private bool lastCheckAccessWeek = false;
        private bool lastCheckAccessMonth = false;

        private event RequestCallback callback;

        public CacheInfo()
        {
        }

        public CacheInfo(CacheInfo info)
        {
            this.storage = info.storage;
            this.url = info.url;
            this.eTag = info.eTag;
            this.lastModified = info.lastModified;
            this.lastAccess = info.lastAccess;
            this.expires = info.expires;
            this.requestTime = info.requestTime;
            this.responseTime = info.responseTime;
            this.age = info.age;
            this.date = info.date;
            this.cacheControl = info.cacheControl;
            this.contentLength = info.contentLength;
            this.initialAge = info.initialAge;
            this.needCaculateAge = info.needCaculateAge;
            this.freshnessLifetime = info.freshnessLifetime;
            this.needCaculateLifetime = info.needCaculateLifetime;
            this.state = info.state;
            this.index = info.index;
            this.requestInPlay = info.requestInPlay;
            this.lastCheckTime = info.lastCheckTime;
            this.lastCheckAccessWeek = info.lastCheckAccessWeek;
            this.lastCheckAccessMonth = info.lastCheckAccessMonth;
        }

        public CacheInfo(CachePackage storage, string url)
        {
            this.storage = storage;
            this.url = url;
        }

        public int CompareTo(CacheInfo other)
        {
            bool lastAccessMonth = IsLastAccessMonth();
            if (lastAccessMonth != other.IsLastAccessMonth())
            {
                if (lastAccessMonth == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            bool lastAccessWeek = IsLastAccessWeek();
            if (lastAccessWeek != other.IsLastAccessWeek())
            {
                if (lastAccessWeek == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            bool expired = IsExpired();
            if (expired != other.IsExpired())
            {
                if (expired == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            return 0;
        }

        public bool IsRequest()
        {
            return storage.IsRequest(this);
        }

        public bool IsCached()
        {
            return index > 0;
        }

        public ulong GetSpaceID()
        {
            return this.index;
        }

        public long GetCacheStartTicks()
        {
            if (date != null)
            {
                return date;
            }

            return responseTime;
        }

        private int FromToSeconds(long fromTicks, long toTicks)
        {
            return (int)((toTicks - fromTicks) / TimeSpan.TicksPerSecond);
        }

        public int GetCurrentAge()
        {
            if (needCaculateAge == true)
            {
                int responseDelay = FromToSeconds(requestTime, responseTime);
                int correctedAgeValue = age + responseDelay;
                if (date != null)
                {
                    int apparentAge = Math.Max(0, FromToSeconds(date, responseTime));
                    initialAge = Math.Max(apparentAge, correctedAgeValue);
                }
                else
                {
                    initialAge = Math.Max(0, correctedAgeValue);
                }

                needCaculateAge = false;
            }

            int residentTime = FromToSeconds(GetCacheStartTicks(), DateTime.UtcNow.Ticks);

            return initialAge + residentTime;
        }

        public bool NeedRequest(CacheRequestType requestType, double reRequestTime = 0)
        {
            if (requestType == CacheRequestType.ALWAYS)
            {
                return true;
            }
            else if (requestType == CacheRequestType.FIRSTPLAY)
            {
                if (requestInPlay == false)
                {
                    return true;
                }
            }
            else if (requestType == CacheRequestType.LOCAL)
            {
                return false;
            }

            if (IsAlways() == true)
            {
                return true;
            }

            if (reRequestTime == 0)
            {
                reRequestTime = CacheStorageImplement.GetReRequestTime();
            }

            if (CheckReRequest(reRequestTime) == true)
            {
                return true;
            }

            return IsExpired();
        }

        public void CheckState()
        {
            if (lastCheckTime != CacheStorageImplement.updateTime)
            {
                lastCheckTime = CacheStorageImplement.updateTime;

                lastCheckAccessWeek = IsLastAccessPeriod(SECONDS_PER_DAY * 7);
                lastCheckAccessMonth = IsLastAccessPeriod(SECONDS_PER_DAY * 30);

                if (state != State.EXPIRED &&
                    IsFresh() == false)
                {
                    state = State.EXPIRED;
                }
            }
        }

        public bool IsExpired()
        {
            CheckState();
            return state == State.EXPIRED;
        }

        public bool IsLastAccessWeek()
        {
            CheckState();

            return lastCheckAccessWeek;
        }

        public bool IsLastAccessMonth()
        {
            CheckState();

            return lastCheckAccessMonth;
        }

        public bool IsLastAccessPeriod(double periodSecond)
        {
            if (lastAccess > 0)
            {
                return DateTime.UtcNow.Ticks - lastAccess > TimeSpan.TicksPerSecond * periodSecond;
            }

            return false;
        }

        public double GetPastTimeFromLastAccess()
        {
            if (lastAccess > 0)
            {
                return (DateTime.UtcNow.Ticks - lastAccess) / TimeSpan.TicksPerSecond;
            }

            return 0;
        }

        public double GetPastTimeFromResponse()
        {
            if (responseTime > 0)
            {
                return (DateTime.UtcNow.Ticks - responseTime) / TimeSpan.TicksPerSecond;
            }

            return 0;
        }

        public bool CheckReRequest(double reRequestTime)
        {
            if (reRequestTime > 0 &&
                responseTime > 0)
            {
                return DateTime.UtcNow.Ticks - responseTime > TimeSpan.TicksPerSecond * reRequestTime;
            }

            return false;
        }

        public bool IsFresh()
        {
            int lifeTime = GetFreshnessLifeTime();
            if (lifeTime > 0)
            {
                return GetCurrentAge() < lifeTime;
            }
            else
            {
                return false;
            }
        }

        public bool IsAlways()
        {
            if (string.IsNullOrEmpty(eTag) == true &&
                lastModified == null)
            {
                return true;
            }

            if (cacheControl != null)
            {
                if (cacheControl.noCache == true)
                {
                    return true;
                }
                else if (cacheControl.maxAge != null)
                {
                    if (cacheControl.maxAge == 0)
                    {
                        return true;
                    }
                }
            }

            int lifeTime = GetFreshnessLifeTime();
            if (lifeTime == 0)
            {
                return true;
            }

            return false;
        }

        public double GetRemainRequestTime()
        {
            double remainTime = 0;

            int lifeTime = GetFreshnessLifeTime();
            if (lifeTime > 0)
            {
                remainTime = lifeTime - GetCurrentAge();
                if (remainTime <= 0)
                {
                    return 0;
                }

                double reRequestTime = CacheStorageImplement.GetReRequestTime();
                if (reRequestTime > 0)
                {
                    double passTime = GetPastTimeFromResponse();
                    double remainReReqeustTime = reRequestTime - passTime;
                    if (remainReReqeustTime <= 0)
                    {
                        return 0;
                    }

                    if (remainTime >= remainReReqeustTime)
                    {
                        remainTime = remainReReqeustTime;
                    }
                }
            }

            return remainTime;
        }

        public int GetFreshnessLifeTime()
        {
            if (needCaculateLifetime == true)
            {
                if (cacheControl != null &&
                    cacheControl.maxAge != null)
                {
                    freshnessLifetime = Math.Max(0, cacheControl.maxAge);
                }
                else if (expires != null)
                {
                    freshnessLifetime = Math.Max(0, FromToSeconds(GetCacheStartTicks(), expires));
                }
                else
                {
                    freshnessLifetime = 0;
                }

                needCaculateLifetime = false;
            }

            return freshnessLifetime;
        }

        internal CacheRequestOperation ReturnResult(byte[] datas, bool updateData, RequestCallback onResult)
        {
            onResult?.SafeCallback(new CacheResult(this, datas, updateData));

            return new CacheRequestOperation(this);
        }

        internal void SendResultAll(byte[] datas, bool updateData)
        {
            if (callback != null)
            {
                CacheResult result = new CacheResult(this, datas, updateData);
                foreach (Delegate invocation in callback.GetInvocationList())
                {
                    ((RequestCallback)invocation)?.SafeCallback(result);
                }

                callback = null;
            }
        }

        internal void AddCallback(RequestCallback addCallback)
        {
            if (addCallback != null)
            {
                callback += addCallback;
            }
        }

        internal void ReleaseCallback(RequestCallback releaseCallback)
        {
            if (releaseCallback != null)
            {
                callback -= releaseCallback;
            }
        }
    }
}