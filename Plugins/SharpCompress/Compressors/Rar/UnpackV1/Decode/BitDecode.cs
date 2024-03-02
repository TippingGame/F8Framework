#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Compressors.Rar.UnpackV1.Decode
{
    internal class BitDecode : Decode
    {
        internal BitDecode()
            : base(new int[PackDef.BC])
        {
        }
    }
}

#endif