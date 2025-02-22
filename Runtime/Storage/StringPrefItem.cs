using System;
using System.Text;

namespace F8Framework.Core
{
    internal class StringPrefItem : PrefItem, IReference, IDisposable
    {
        internal string Value = "";
        private Encoding encoding = Encoding.UTF8;

        internal override int GetRawBytesLength()
        {
            if (string.IsNullOrEmpty(Value)) return 4;
            return 4 + encoding.GetByteCount(Value);
        }

        internal override byte[] ExportRawBytes()
        {
            if (string.IsNullOrEmpty(Value)) return ByteHelper.IntToBytesLittleEndian(encoding.CodePage, 4);
            return ByteHelper.ConcatenateByteArrays(ByteHelper.IntToBytesLittleEndian(encoding.CodePage, 4),
                encoding.GetBytes(Value));
        }

        internal override bool ImportRawBytes(byte[] bytes)
        {
            if (bytes.Length < 4) return false;
            encoding = Encoding.GetEncoding(ByteHelper.BytesToIntLittleEndian(ByteHelper.SliceByteArray(bytes, 0, 4)));
            if (encoding == null) return false;
            if (bytes.Length == 4) Value = "";
            else
                Value = encoding.GetString(ByteHelper.SliceByteArray(bytes, 4, bytes.Length - 4));
            return true;
        }
        
        public void Clear()
        {
            Value = "";
        }
        
        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}