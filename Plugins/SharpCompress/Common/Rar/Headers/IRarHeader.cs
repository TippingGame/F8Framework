#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Common.Rar.Headers
{
    internal interface IRarHeader 
    {
        HeaderType HeaderType { get; }
    }
}

#endif