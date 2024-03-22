using System;
using System.Globalization;

namespace F8Framework.Core
{
    public static partial class Util
    {
        /// <summary>
        /// 时间相关工具，提供了不同精度的UTC时间戳等函数
        /// </summary>
        public static class Time
        {
            //1s=1000ms
            //1ms=1000us
            readonly static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            readonly static long epochTicks = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            readonly static long secDivisor = (long)Math.Pow(10, 7);
            readonly static long msecDivisor = (long)Math.Pow(10, 4);

            public static string GetTimeText(long ticks)
            {
                double year = ticks / (TimeSpan.TicksPerDay * 30 * 12);
                double month = ticks / (TimeSpan.TicksPerDay * 30);
                double week = ticks / (TimeSpan.TicksPerDay * 7);
                double day = ticks / TimeSpan.TicksPerDay;
                double hour = ticks / TimeSpan.TicksPerHour;
                double minute = ticks / TimeSpan.TicksPerMinute;
                double second = ticks / TimeSpan.TicksPerSecond;

                if (year > 0)
                {
                    return string.Format("{0:#} year", year);
                }
                else if (month > 0)
                {
                    return string.Format("{0:#} month", month);
                }
                else if (week > 0)
                {
                    return string.Format("{0:#} week", week);
                }
                else if (day > 0)
                {
                    return string.Format("{0:#} day", day);
                }
                else if (hour > 0)
                {
                    second -= minute * 60;
                    minute -= hour * 60;
                    return string.Format("{0:0}:{1:00}:{2:00}", hour, minute, second);
                }
                else if (minute > 0)
                {
                    second -= minute * 60;
                    return string.Format("0:{0:00}:{1:00}", minute, second);
                }
                else
                {
                    return string.Format("0:00:{0:00}", second);
                }
            }

            /// <summary>
            /// 获取秒级别UTC时间戳
            /// </summary>
            /// <returns>秒级别时间戳</returns>
            public static long SecondTimeStamp()
            {
                return (DateTime.UtcNow.Ticks - epochTicks) / secDivisor;
            }

            /// <summary>
            /// 获取毫秒级别的UTC时间戳
            /// </summary>
            /// <returns>毫秒级别时间戳</returns>
            public static long MillisecondTimeStamp()
            {
                return (DateTime.UtcNow.Ticks - epochTicks) / msecDivisor;
            }

            /// <summary>
            /// 获取微秒级别UTC时间戳
            /// </summary>
            /// <returns>微秒级别UTC时间戳</returns>
            public static long MicrosecondTimeStamp()
            {
                return DateTime.UtcNow.Ticks - epochTicks;
            }

            /// <summary>
            ///秒级别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long SecondNow()
            {
                return DateTime.UtcNow.Ticks / secDivisor;
            }

            /// <summary>
            ///毫秒别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long MillisecondNow()
            {
                return DateTime.UtcNow.Ticks / msecDivisor;
            }

            /// <summary>
            ///微秒级别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long MicrosecondNow()
            {
                return DateTime.UtcNow.Ticks;
            }

            /// <summary>
            /// 时间戳转UTC DateTime
            /// </summary>
            /// <returns>UTC DateTime</returns>
            public static DateTime TimestampToDateTime(long timeStamp)
            {
                int length = (int)Math.Floor(Math.Log10(timeStamp));
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                if (length == 9)
                    return dateTime.AddSeconds(timeStamp);
                if (length == 12)
                    return dateTime.AddMilliseconds(timeStamp);
                if (length == 16)
                    return dateTime.AddTicks(timeStamp);
                return DateTime.MinValue;
            }

            /// <summary>
            /// 获取该时间相对于纪元时间的秒数；
            /// </summary>
            /// <returns>秒数</returns>
            public static long GetTotalSeconds()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalSeconds);
            }

            /// <summary>
            /// 获取该时间相对于纪元时间的毫秒数；
            /// </summary>
            /// <returns>毫秒数</returns>
            public static long GetTotalMilliseconds()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalMilliseconds);
            }

            /// <summary>
            /// 获取该时间相对于纪元时间的微秒数；
            /// </summary>
            /// <returns>微秒数</returns>
            public static long GetTotalMicroseconds()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalMilliseconds) / 10;
            }

            /// <summary>
            /// 获取该时间相对于纪元时间的分钟数
            /// </summary>
            public static double GetTotalMinutes()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalMinutes);
            }

            /// <summary>
            /// 获取该时间相对于纪元时间的小时数
            /// </summary>
            public static double GetTotalHours()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalHours);
            }

            /// <summary>
            /// 获取该时间相对于纪元时间的天数
            /// </summary>
            public static double GetTotalDays()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalHours);
            }

            /// <summary>
            /// 获取某一年有多少周；
            /// </summary>
            /// <param name="year">年份</param>
            /// <returns>该年周数</returns>
            public static int GetWeekAmount(int year)
            {
                var end = new DateTime(year, 12, 31); //该年最后一天
                var gc = new GregorianCalendar();
                return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, DayOfWeek.Monday); //该年星期数
            }

            /// <summary>
            /// 得到一年中的某周的起始日和截止日；
            /// 年 nYear
            /// 周数 nNumWeek
            /// 周始 out dtWeekStart
            /// 周终 out dtWeekeEnd
            /// </summary>
            /// <param name="_"></param>
            /// <param name="year">年份</param>
            /// <param name="numWeek">第几周</param>
            /// <param name="dtWeekStart">开始日期</param>
            /// <param name="dtWeekeEnd">结束日期</param>
            public static void GetWeekTime(int year, int numWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
            {
                var dt = new DateTime(year, 1, 1);
                dt += new TimeSpan((numWeek - 1) * 7, 0, 0, 0);
                dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Monday);
                dtWeekeEnd = dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 1);
            }

            /// <summary>
            /// 得到一年中的某周的起始日和截止日    周一到周五  工作日；
            /// </summary>
            /// <param name="_"></param>
            /// <param name="year">年份</param>
            /// <param name="numWeek">第几周</param>
            /// <param name="dtWeekStart">开始日期</param>
            /// <param name="dtWeekeEnd">结束日期</param>
            public static void GetWeekWorkTime(int year, int numWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
            {
                var dt = new DateTime(year, 1, 1);
                dt += new TimeSpan((numWeek - 1) * 7, 0, 0, 0);
                dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Monday);
                dtWeekeEnd = dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 1).AddDays(-2);
            }

            /// <summary>
            /// 返回某年某月最后一天
            /// </summary>
            /// <param name="_"></param>
            /// <param name="year">年份</param>
            /// <param name="month">月份</param>
            /// <returns>日期</returns>
            public static int GetMonthLastDate(int year, int month)
            {
                DateTime lastDay = new DateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
                int day = lastDay.Day;
                return day;
            }

            /// <summary>
            /// 获得一段时间内有多少小时
            /// </summary>
            /// <param name="lhs">起始时间</param>
            /// <param name="rhs">终止时间</param>
            /// <returns>小时差</returns>
            public static string GetTimeDelay(DateTime lhs, DateTime rhs)
            {
                long lTicks = (rhs.Ticks - lhs.Ticks) / 10000000;
                string sTemp = (lTicks / 3600).ToString().PadLeft(2, '0') + ":";
                sTemp += (lTicks % 3600 / 60).ToString().PadLeft(2, '0') + ":";
                sTemp += (lTicks % 3600 % 60).ToString().PadLeft(2, '0');
                return sTemp;
            }

            /// <summary>
            /// 获取指定年份中指定月的天数
            /// </summary>
            public static int GetDaysOfMonth(int year, int month)
            {
                switch (month)
                {
                    case 1:
                        return 31;
                    case 2:
                        return IsLeapYear(year) ? 29 : 28;
                    case 3:
                        return 31;
                    case 4:
                        return 30;
                    case 5:
                        return 31;
                    case 6:
                        return 30;
                    case 7:
                        return 31;
                    case 8:
                        return 31;
                    case 9:
                        return 30;
                    case 10:
                        return 31;
                    case 11:
                        return 30;
                    case 12:
                        return 31;
                    default:
                        return 0;
                }
            }

            /// <summary>
            /// 是否为闰年
            /// </summary>
            /// <param name="year">e.g.2023,2024</param>
            /// <returns>结果</returns>
            public static bool IsLeapYear(int year)
            {
                //形式参数为年份
                //例如：2023
                var n = year;
                return n % 400 == 0 || n % 4 == 0 && n % 100 != 0;
            }

            /// <summary>
            /// 获取第二天的午夜零时
            /// </summary>
            /// <returns>UTC午夜零时</returns>
            public static DateTime GetTomorrowMidnightDateTime()
            {
                DateTime todayDate = DateTime.UtcNow.Date;
                var nextDay = todayDate.AddDays(1);
                return nextDay;
            }

            /// <summary>
            /// 获取明日的午夜零时偏移量
            /// <para>转换时间戳可使用方法：<see cref="DateTimeOffset.ToUnixTimeSeconds"/> </para> 
            /// </summary>
            /// <returns>日期偏移量</returns>
            public static DateTimeOffset GetTomorrowMidnightDateTimeOffset()
            {
                var tomorrowDateTime = GetTomorrowMidnightDateTime();
                return new DateTimeOffset(tomorrowDateTime);
            }

            /// <summary>
            /// 获取指定时间的秒时间戳
            /// </summary>
            /// <param name="dateTime">指定日期</param>
            /// <returns>秒时间戳</returns>
            public static long GetDateSecondTimeStamp(DateTime dateTime)
            {
                return (dateTime.Ticks - epochTicks) / secDivisor;
            }

            /// <summary>
            /// 获取指定时间的毫秒间戳
            /// </summary>
            /// <param name="dateTime">指定日期</param>
            /// <returns>毫秒时间戳</returns>
            public static long GetDateMillisecondTimeStamp(DateTime dateTime)
            {
                return (dateTime.Ticks - epochTicks) / msecDivisor;
            }

            /// <summary>
            /// 获取指定时间的微秒时间戳
            /// </summary>
            /// <param name="dateTime">指定日期</param>
            /// <returns>微秒时间戳</returns>
            public static long GetDateMicrosecondTimeStamp(DateTime dateTime)
            {
                return (dateTime.Ticks - epochTicks);
            }
            
            /// <summary>
            /// 判断是否是同一天
            /// </summary>
            /// <param name="lhs">起始时间</param>
            /// <param name="rhs">终止时间</param>
            /// <returns>是否是同一天</returns>
            public static bool MatchDay(DateTime lhs, DateTime rhs)
            {
                var lhsDay = new DateTime(lhs.Year, lhs.Month, lhs.Day, 0, 0, 0);
                var rhsDay = new DateTime(rhs.Year, rhs.Month, rhs.Day, 0, 0, 0);
                var diff = rhsDay - lhsDay;
                if (Math.Abs(diff.Days) >= 1)
                    return false;
                return true;
            }
        }
    }
}