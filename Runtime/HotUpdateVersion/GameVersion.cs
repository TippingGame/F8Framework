using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class GameVersion
    {
        public string Version;
        public List<string> SubPackage;
        public string AssetRemoteAddress;
        public bool EnableHotUpdate;
        public string HotUpdateURL;

        public GameVersion(string version, List<string> subPackage = null, string assetRemoteAddress = null, bool enableHotUpdate = false, string hotUpdateURL = null)
        {
            Version = version;
            SubPackage = subPackage;
            AssetRemoteAddress = assetRemoteAddress;
            EnableHotUpdate = enableHotUpdate;
            HotUpdateURL = hotUpdateURL;
        }
        
        public GameVersion()
        {
            
        }
    }
    
    public class GameConfig
    {
        public static GameVersion LocalGameVersion = new GameVersion();

        public static GameVersion RemoteGameVersion = new GameVersion();
        
        /// <summary>
        /// 判断版本号大小
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns>1为version1大，-1为version2大，0为一样大</returns>
        public static int CompareVersions(string version1, string version2)
        {
            // Split the versions into individual components
            string[] version1Components = version1.Split('.');
            string[] version2Components = version2.Split('.');

            // Determine the maximum length of version components
            int maxLength = Math.Max(version1Components.Length, version2Components.Length);

            // Compare each component one by one
            for (int i = 0; i < maxLength; i++)
            {
                int v1 = i < version1Components.Length ? Convert.ToInt32(version1Components[i]) : 0;
                int v2 = i < version2Components.Length ? Convert.ToInt32(version2Components[i]) : 0;

                if (v1 < v2)
                {
                    return -1; // version1 is less than version2
                }
                else if (v1 > v2)
                {
                    return 1; // version1 is greater than version2
                }
            }

            return 0; // versions are equal
        }
    }
}
