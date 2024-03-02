#if CSHARP_7_3_OR_NEWER

using System;

#if !NO_FILE
using System.IO;
#endif

namespace SharpCompress.Common
{
    public interface IVolume : IDisposable
    {
    }
}

#endif