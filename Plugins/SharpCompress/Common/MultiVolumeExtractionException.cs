#if CSHARP_7_3_OR_NEWER

using System;

namespace SharpCompress.Common
{
    public class MultiVolumeExtractionException : ExtractionException
    {
        public MultiVolumeExtractionException(string message)
            : base(message)
        {
        }

        public MultiVolumeExtractionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

#endif