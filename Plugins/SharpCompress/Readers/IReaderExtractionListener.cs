#if CSHARP_7_3_OR_NEWER

using SharpCompress.Common;

namespace SharpCompress.Readers
{
    internal interface IReaderExtractionListener : IExtractionListener
    {
        void FireEntryExtractionProgress(Entry entry, long sizeTransferred, int iterations);
    }
}

#endif