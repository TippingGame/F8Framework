#if CSHARP_7_3_OR_NEWER

using System;

namespace SharpCompress.Common
{
    public class CryptographicException : Exception
    {
        public CryptographicException(string message)
            : base(message)
        {
        }
    }
}

#endif