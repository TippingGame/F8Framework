#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Common
{
    public enum CompressionType
    {
        None,
        GZip,
        BZip2,
        PPMd,
        Deflate,
        Rar,
        LZMA,
        BCJ,
        BCJ2,
        LZip,
        Xz,
        Unknown,
        Deflate64
    }
}

#endif