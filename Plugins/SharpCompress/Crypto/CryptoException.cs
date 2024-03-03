#if CSHARP_7_3_OR_NEWER

using System;

namespace SharpCompress.Crypto
{
    public class CryptoException
        : Exception
    {
        public CryptoException()
        {
        }

        public CryptoException(
            string message)
            : base(message)
        {
        }

        public CryptoException(
            string message,
            Exception exception)
            : base(message, exception)
        {
        }
    }
}

#endif