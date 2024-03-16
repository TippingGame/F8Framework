using System.Collections.Generic;

namespace F8Framework.Core
{
    public class GameVersion
    {
        public string Version;
        public List<string> SubPackage;

        public GameVersion(string version, List<string> subPackage = null)
        {
            this.Version = version;
            this.SubPackage = subPackage;
        }
    }
}
