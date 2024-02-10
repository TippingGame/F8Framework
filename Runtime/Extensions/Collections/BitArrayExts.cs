using System.Collections;
using System.Text;

namespace F8Framework.Core
{
    public static class BitArrayExts
    {
        public static byte[] ToByteArray(this BitArray @this)
        {
            var result = new byte[@this.GetArrayLength(8)];
            @this.CopyTo(result, 0);
            return result;
        }
        public static int[] ToInt32Array(this BitArray @this)
        {
            var result = new int[@this.GetArrayLength(32)];
            @this.CopyTo(result, 0);
            return result;
        }
        public static int GetArrayLength(this BitArray @this, int div)
        {
            return GetArrayLength(@this.Length, div);
        }
        public static string ToHex(this byte @this)
        {
            return @this.ToString("X2");
        }
        public static string ToHex(this byte[] @this)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in @this)
            {
                stringBuilder.Append(b.ToString("X2"));
            }
            return stringBuilder.ToString();
        }
        public static string ToHex(this byte[] @this, string format)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in @this)
            {
                stringBuilder.Append(b.ToString(format));
            }
            return stringBuilder.ToString();
        }
        public static string ToHex(this byte[] @this, int offset, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = offset; i < offset + count; ++i)
            {
                stringBuilder.Append(@this[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }
        public static string ToString(this byte[] @this)
        {
            return Encoding.Default.GetString(@this);
        }
        public static string ToString(this byte[] @this, int index, int count)
        {
            return Encoding.Default.GetString(@this, index, count);
        }
        private static int GetArrayLength(int n, int div)
        {
            return n > 0 ? (n - 1) / div + 1 : 0;
        }
    }
}
