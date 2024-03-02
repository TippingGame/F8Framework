#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Common.Zip.Headers
{
    internal enum ZipHeaderType
    {
        Ignore,
        LocalEntry,
        DirectoryEntry,
        DirectoryEnd,
        Split,
        Zip64DirectoryEnd,
        Zip64DirectoryEndLocator
    }
}

#endif