#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Compressors.Rar.UnpackV1.Decode
{
    internal class LowDistDecode : Decode
    {
        internal LowDistDecode()
            : base(new int[PackDef.LDC])
        {
        }
    }
}

#endif