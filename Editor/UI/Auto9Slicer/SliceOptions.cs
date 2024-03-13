using System;

namespace F8Framework.Core.Editor
{
    [Serializable]
    public class SliceOptions
    {
        public int Tolerate = 0;
        public int CenterSize = 2;
        public int Margin = 2;

        public static SliceOptions Default => new SliceOptions();
    }
}