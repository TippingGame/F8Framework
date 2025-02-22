using System;

namespace F8Framework.Core
{
    internal static class ByteHelper
    {
        // 切片字节数组
        internal static byte[] SliceByteArray(byte[] bytes, int index, int length)
        {
            // 输入验证
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes), "The byte array cannot be null.");
            }
            if (index < 0 || length < 0 || index + length > bytes.Length)
            {
                throw new ArgumentException("Invalid index or length for slicing the byte array.", nameof(index));
            }

            // 使用 Span 提高性能
            return bytes.AsSpan(index, length).ToArray();
        }

        // 连接多个字节数组
        internal static byte[] ConcatenateByteArrays(params byte[][] byteArrays)
        {
            // 处理空输入
            if (byteArrays is null || byteArrays.Length == 0)
            {
                return Array.Empty<byte>();
            }

            // 计算总长度
            int totalLength = 0;
            foreach (byte[] array in byteArrays)
            {
                totalLength += array.Length;
            }

            // 创建结果数组
            byte[] result = new byte[totalLength];
            int offset = 0;

            // 复制每个字节数组到结果数组
            foreach (byte[] array in byteArrays)
            {
                array.CopyTo(result, offset);
                offset += array.Length;
            }

            return result;
        }

        // 将整数转换为小端字节序的字节数组
        internal static byte[] IntToBytesLittleEndian(int x, int length)
        {
            // 直接使用 BitConverter 转换为小端字节序
            byte[] bytes = BitConverter.GetBytes(x);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            // 切片字节数组
            return bytes.AsSpan(0, length).ToArray();
        }

        // 将小端字节序的字节数组转换为整数
        internal static int BytesToIntLittleEndian(byte[] bytes)
        {
            // 输入验证
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes), "The byte array cannot be null.");
            }
            if (bytes.Length < 4)
            {
                throw new ArgumentException("The byte array must be at least 4 bytes long.", nameof(bytes));
            }

            // 确保是小端字节序
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            // 转换为整数
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}