#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Compressors.Rar.VM
{
    internal enum VMStandardFilters
    {
        VMSF_NONE = 0,
        VMSF_E8 = 1,
        VMSF_E8E9 = 2,
        VMSF_ITANIUM = 3,
        VMSF_RGB = 4,
        VMSF_AUDIO = 5,
        VMSF_DELTA = 6,
        VMSF_UPCASE = 7
    }
}

#endif