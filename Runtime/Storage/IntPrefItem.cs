using System;

namespace F8Framework.Core
{
    internal class IntPrefItem : PrefItem, IReference, IDisposable
    {
        internal int Value = 0;

        internal override int GetRawBytesLength()
        {
            return 4;
        }

        internal override byte[] ExportRawBytes()
        {
            return ByteHelper.IntToBytesLittleEndian(Value, 4);
        }

        internal override bool ImportRawBytes(byte[] bytes)
        {
            Value = ByteHelper.BytesToIntLittleEndian(bytes);
            return true;
        }
        
        public void Clear()
        {
            Value = 0;
        }
        
        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}