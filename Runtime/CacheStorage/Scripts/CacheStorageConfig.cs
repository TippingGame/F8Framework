using System;

namespace F8Framework.Core
{
    internal class CacheStorageConfig
    {
        internal int maxCount = 0;
        internal long maxSize = 0;
        internal double reRequestTime = 0;
        internal double unusedPeriodTime = 0;
        internal double removeCycle = 1;
        internal CacheRequestType defaultRequestType = CacheRequestType.FIRSTPLAY;
        internal bool setting = false;

        internal CacheStorageConfig()
        {
        }

        internal CacheStorageConfig(int maxCount, int maxSize, double reRequestTime, double unusedPeriodTime,
            double removeCycle, CacheRequestType defaultRequestType)
        {
            this.maxCount = maxCount;
            this.maxSize = maxSize;
            this.reRequestTime = reRequestTime;
            this.unusedPeriodTime = unusedPeriodTime;
            this.removeCycle = removeCycle;
            this.defaultRequestType = defaultRequestType;

            this.setting = true;
        }
    }
}