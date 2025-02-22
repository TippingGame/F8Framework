using System;

namespace F8Framework.Core
{
    internal class FloatPrefItem : PrefItem, IReference, IDisposable
    {
        internal float Value = 0.0f;

        internal override int GetRawBytesLength()
        {
            return 4;
        }

        internal override byte[] ExportRawBytes()
        {
            return BitConverter.GetBytes(Value);
        }

        internal override bool ImportRawBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4) return false;
            Value = BitConverter.ToSingle(bytes, 0);
            return true;
        }
        
        public void Clear()
        {
            Value = 0.0f;
        }
        
        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}