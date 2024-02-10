using System;

namespace F8Framework.Core
{
    public static class ShortExts
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="@this">short</param>
        /// <returns>组内容</returns>
        public static byte[] GetBytes(this short @this)
        {
            return BitConverter.GetBytes(@this);
        }
    }
}
