#if CSHARP_7_3_OR_NEWER

using System.IO;

namespace SharpCompress.Archives
{
    internal interface IWritableArchiveEntry
    {
        Stream Stream { get; }
    }
}

#endif