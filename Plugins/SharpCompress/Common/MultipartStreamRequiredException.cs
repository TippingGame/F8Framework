#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Common
{
    public class MultipartStreamRequiredException : ExtractionException
    {
        public MultipartStreamRequiredException(string message)
            : base(message)
        {
        }
    }
}

#endif