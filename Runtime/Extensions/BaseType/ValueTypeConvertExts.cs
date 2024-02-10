using System;

namespace F8Framework.Core
{
    public  static  class ValueTypeConvertExts
    {
        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="this">源字符串</param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns>int类型的数字</returns>
        public static int ToInt32(this string @this, int defaultValue = 0)
        {
            return @this.TryConvertTo(defaultValue);
        }

        /// <summary>
        /// 字符串转long
        /// </summary>
        /// <param name="this">源字符串</param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns>int类型的数字</returns>
        public static long ToInt64(this string @this, long defaultValue = 0)
        {
            return @this.TryConvertTo(defaultValue);
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="this">源字符串</param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns>double类型的数据</returns>
        public static double ToDouble(this string @this, double defaultValue = 0)
        {
            return @this.TryConvertTo(defaultValue);
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="this">源字符串</param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns>int类型的数字</returns>
        public static decimal ToDecimal(this string @this, decimal defaultValue = 0)
        {
            return @this.TryConvertTo(defaultValue);
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="this">源字符串</param>
        /// <param name="round">小数位数</param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns>int类型的数字</returns>
        public static decimal ToDecimal(this string @this, int round, decimal defaultValue = 0)
        {
            return Math.Round(@this.TryConvertTo(defaultValue), round, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 转double
        /// </summary>
        /// <param name="this"></param>
        /// <returns>double类型的数据</returns>
        public static double ToDouble(this decimal @this)
        {
            return (double)@this;
        }

        /// <summary>
        /// 将double转换成int
        /// </summary>
        /// <param name="this">double类型</param>
        /// <returns>int类型</returns>
        public static int ToInt32(this double @this)
        {
            return (int)Math.Floor(@this);
        }

        /// <summary>
        /// 将double转换成int
        /// </summary>
        /// <param name="@this">double类型</param>
        /// <returns>int类型</returns>
        public static int ToInt32(this decimal @this)
        {
            return (int)Math.Floor(@this);
        }

        /// <summary>
        /// 将int转换成double
        /// </summary>
        /// <param name="@this">int类型</param>
        /// <returns>int类型</returns>
        public static double ToDouble(this int @this)
        {
            return @this * 1.0;
        }

        /// <summary>
        /// 将int转换成decimal
        /// </summary>
        /// <param name="@this">int类型</param>
        /// <returns>int类型</returns>
        public static decimal ToDecimal(this int @this)
        {
            return new decimal(@this);
        }

        /// <summary>
        /// 保留小数
        /// </summary>
        /// <param name="@this"></param>
        /// <param name="decimals"></param>
        /// <param name="mode">四舍五入策略</param>
        /// <returns></returns>
        public static decimal Round(this decimal @this, int decimals, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            @this = Math.Round(@this, decimals, mode);
            return @this;
        }

        /// <summary>
        /// 保留小数
        /// </summary>
        /// <param name="@this"></param>
        /// <param name="decimals"></param>
        /// <param name="mode">四舍五入策略</param>
        /// <returns></returns>
        public static double Round(this double @this, int decimals, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            @this = Math.Round(@this, decimals, mode);
            return @this;
        }

        /// <summary>
        /// 保留小数
        /// </summary>
        /// <param name="@this"></param>
        /// <param name="decimals"></param>
        /// <param name="mode">四舍五入策略</param>
        /// <returns></returns>
        public static decimal? Round(this decimal? @this, int decimals, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            if (@this.HasValue)
            {
                @this = Math.Round(@this.Value, decimals, mode);
            }
            return @this;
        }

        /// <summary>
        /// 保留小数
        /// </summary>
        /// <param name="@this"></param>
        /// <param name="decimals"></param>
        /// <param name="mode">四舍五入策略</param>
        /// <returns></returns>
        public static double? Round(this double? @this, int decimals, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            if (@this.HasValue)
            {
                @this = Math.Round(@this.Value, decimals, mode);
            }
            return @this;
        }
    }
}
