using System;

namespace F8Framework.Core
{
    public static class LongExts
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="this">long</param>
        /// <returns>数组</returns>
        public static byte[] GetBytes(this long @this)
        {
            return BitConverter.GetBytes(@this);
        }
    }
}
