using System;
using System.IO;

namespace F8Framework.Core
{
    public class BundleStream : FileStream
    {
        private byte xorKey = 0;

        public BundleStream(byte xorKey, string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access,
            share)
        {
            this.xorKey = xorKey;
        }

        public BundleStream(byte xorKey, string path, FileMode mode) : base(path, mode)
        {
            this.xorKey = xorKey;
        }
        
        public override int Read(byte[] array, int offset, int count)
        {
            int bytesRead = base.Read(array, offset, count);
            var span = new Span<byte>(array, offset, bytesRead);
            for (int i = 0; i < span.Length; i++)
            {
                span[i] ^= xorKey;
            }
            return bytesRead;
        }
    }
}
