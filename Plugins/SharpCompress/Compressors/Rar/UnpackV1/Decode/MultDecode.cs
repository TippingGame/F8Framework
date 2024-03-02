#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Compressors.Rar.UnpackV1.Decode
{
    internal class MultDecode : Decode
    {
        internal MultDecode()
            : base(new int[PackDef.MC20])
        {
        }
    }
}

#endif