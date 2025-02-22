using System;

namespace F8Framework.Core
{
    internal class BoolPrefItem : PrefItem, IReference, IDisposable
    {
        internal bool Value = false;

        internal override int GetRawBytesLength()
        {
            return 1;
        }

        internal override byte[] ExportRawBytes()
        {
            return new byte[] { Value ? (byte)1 : (byte)0 };
        }

        internal override bool ImportRawBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1) return false;
            Value = bytes[0] != 0;
            return true;
        }
        
        public void Clear()
        {
            Value = false;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}