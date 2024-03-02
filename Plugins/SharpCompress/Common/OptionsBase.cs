#if CSHARP_7_3_OR_NEWER

namespace SharpCompress.Common
{
    public class OptionsBase
    {
        /// <summary>
        /// SharpCompress will keep the supplied streams open.  Default is true.
        /// </summary>
        public bool LeaveStreamOpen { get; set; } = true;

        public ArchiveEncoding ArchiveEncoding { get; set; } = new ArchiveEncoding();
    }
}

#endif