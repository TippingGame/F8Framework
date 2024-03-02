#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Compressors.Xz
{
    public enum CheckType : byte
    {
        NONE =   0x00,
        CRC32 =  0x01,
        CRC64 =  0x04,
        SHA256 = 0x0A
    }
}

#endif