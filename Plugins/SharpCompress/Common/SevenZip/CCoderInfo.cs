#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Common.SevenZip
{
    internal class CCoderInfo
    {
        internal CMethodId _methodId;
        internal byte[] _props;
        internal int _numInStreams;
        internal int _numOutStreams;
    }
}

#endif