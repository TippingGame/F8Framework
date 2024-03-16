using System.Collections.Generic;

namespace F8Framework.Core
{
    public class GameVersion
    {
        public string Version;
        public List<string> SubPackage;
        public bool EnableHotUpdate;
        public string HotUpdateURL;

        public GameVersion(string version, List<string> subPackage = null, bool enableHotUpdate = false, string hotUpdateURL = null)
        {
            Version = version;
            SubPackage = subPackage;
            EnableHotUpdate = enableHotUpdate;
            HotUpdateURL = hotUpdateURL;
        }
        
        public GameVersion()
        {
            
        }
    }
}
