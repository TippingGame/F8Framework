#if CSHARP_7_3_OR_NEWER

using SharpCompress.Common;

namespace SharpCompress.Archives
{
    internal interface IArchiveExtractionListener : IExtractionListener
    {
        void EnsureEntriesLoaded();
        void FireEntryExtractionBegin(IArchiveEntry entry);
        void FireEntryExtractionEnd(IArchiveEntry entry);
    }
}

#endif