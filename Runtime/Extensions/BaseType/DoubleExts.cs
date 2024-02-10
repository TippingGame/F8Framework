using System;
namespace F8Framework.Core
{
    public static class DoubleExts
    {
        /// <summary>
        /// 将小数截断为8位
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static double Digits8(this double @this)
        {
            return (long)(@this * 1E+8) * 1e-8;
        }
        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this double @this)
        {
            return @this.ConvertTo<decimal>();
        }
        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="this"></param>
        /// <param name="precision">小数位数</param>
        /// <returns></returns>
        public static decimal ToDecimal(this double @this, int precision)
        {
            return Math.Round(@this.ConvertTo<decimal>(), precision);
        }
        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this float @this)
        {
            return @this.ConvertTo<decimal>();
        }
        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="this"></param>
        /// <param name="precision">小数位数</param>
        /// <returns></returns>
        public static decimal ToDecimal(this float @this, int precision)
        {
            return Math.Round(@this.ConvertTo<decimal>(), precision);
        }
    }
}