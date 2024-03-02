#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Compressors.Rar.VM
{
    internal enum VMFlags
    {
        None = 0,
        VM_FC = 1,
        VM_FZ = 2,
        VM_FS = 80000000
    }
}

#endif