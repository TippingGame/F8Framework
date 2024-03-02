#if CSHARP_7_3_OR_NEWER

using System.IO;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace SharpCompress.Common.SevenZip
{
    public class SevenZipVolume : Volume
    {
        public SevenZipVolume(Stream stream, ReaderOptions readerFactoryOptions)
            : base(stream, readerFactoryOptions)
        {
        }
    }
}

#endif