using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace F8Framework.Core
{
    public static class StringExts
    {
        /// <summary>
        /// 高级比较，可设定是否忽略大小写
        /// </summary>
        public static bool Contains(this string @this, string toCheck, StringComparison comp)
        {
            return @this.IndexOf(toCheck, comp) >= 0;
        }
        public static bool Contains(this string @this, IEnumerable<string> keys, bool ignoreCase = true)
        {
            if (!(keys is ICollection<string> array))
            {
                array = keys.ToArray();
            }
            if (array.Count == 0 || string.IsNullOrEmpty(@this))
            {
                return false;
            }
            return ignoreCase ? array.Any(item => @this.IndexOf(item, StringComparison.InvariantCultureIgnoreCase) >= 0) : array.Any(@this.Contains);
        }
        /// <summary>
        /// 是否含有中文
        /// </summary>
        public static bool IsContainChinese(this string @this)
        {
            bool flag = false;
            foreach (var a in @this)
            {
                if (a >= 0x4e00 && a <= 0x9fbb)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        /// <summary>
        /// 是否为空字符串
        /// </summary>
        public static bool IsNullOrEmpty(this string @this)
        {
            return string.IsNullOrEmpty(@this);
        }
        /// <summary>
        /// string Base64加密
        /// </summary>
        public static string StringToBase64(this string @this)
        {
            var b = Encoding.Default.GetBytes(@this);
            return Convert.ToBase64String(b);
        }
        public static string Base64ToString(this string @this)
        {
            var b = Convert.FromBase64String(@this);
            return Encoding.Default.GetString(b);
        }
        /// <summary>
        /// 获取字符串之间的内容
        /// </summary>
        /// <param name="start">首部</param>
        /// <param name="end">尾部</param>
        /// <param name="inculdeStartAndEnd">是否包含首位</param>
        /// <returns>首尾之间的字段</returns>
        public static string Between(this string @this, string start, string end, bool inculdeStartAndEnd = false)
        {
            if (start.Equals(end))
                throw new ArgumentException("Start string can't equals a end string.");
            int startIndex = @this.LastIndexOf(start) + 1;
            int endIndex = @this.LastIndexOf(end) - 1 - @this.LastIndexOf(start);
            if (!inculdeStartAndEnd)
                return @this.Substring(startIndex + start.Length, endIndex - end.Length);
            else
                return @this.Substring(startIndex, endIndex + end.Length);
        }
        /// <summary>
		/// 移除首个字符
		/// </summary>
		public static string RemoveFirstChar(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return @this;
            return @this.Substring(1);
        }
        /// <summary>
        /// 移除末尾字符
        /// </summary>
        public static string RemoveLastChar(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return @this;
            return @this.Substring(0, @this.Length - 1);
        }
        /// <summary>
        /// 字符串转时间
        /// </summary>
        public static DateTime ToDateTime(this string @this)
        {
            DateTime.TryParse(@this, out var result);
            return result;
        }
        /// <summary>
        /// 字符串转Guid
        /// </summary>
        public static Guid ToGuid(this string @this)
        {
            return Guid.Parse(@this);
        }
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        public static byte[] ToByteArray(this string @this)
        {
            return Encoding.UTF8.GetBytes(@this);
        }
        /// <summary>
        /// 根据正则替换
        /// </summary>
        public static string Replace(this string @this, Regex regex, string replacement)
        {
            return regex.Replace(@this, replacement);
        }
        /// <summary>
        /// 转半角(DBC case)
        /// </summary>
        /// <param name="this">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32(此处不必转空格)
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(this string @this)
        {
            char[] c = @this.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }
    }
}
