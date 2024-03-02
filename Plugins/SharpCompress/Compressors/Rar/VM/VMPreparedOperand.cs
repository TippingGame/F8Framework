#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Compressors.Rar.VM
{
    internal class VMPreparedOperand
    {
        internal VMOpType Type { get; set; }
        internal int Data { get; set; }
        internal int Base { get; set; }
        internal int Offset { get; set; }
    }
}

#endif