#if CSHARP_7_3_OR_NEWER

using System;

namespace SharpCompress.Common
{
    public class ArchiveException : Exception
    {
        public ArchiveException(string message)
            : base(message)
        {
        }
    }
}

#endif