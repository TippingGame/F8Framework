using System;
namespace F8Framework.Core
{
    public static class RandomExts
    {
        /// <summary>
        /// 生成真正的随机数
        /// </summary>
        public static int StrictNext(this Random @this, int maxValue = int.MaxValue)
        {
            return new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)).Next(maxValue);
        }
        /// <summary>
        /// 产生正态分布的随机数
        /// </summary>
        /// <param name="this"></param>
        /// <param name="mean">均值</param>
        /// <param name="stdDev">方差</param>
        /// <returns>随机数</returns>
        public static double NextGauss(this Random @this, double mean, double stdDev)
        {
            double u1 = 1.0 - @this.NextDouble();
            double u2 = 1.0 - @this.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}
