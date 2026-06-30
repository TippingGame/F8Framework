using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    [Serializable]
    public class GameVersion
    {
        public string Version;
        public string AssetRemoteAddress;
        public bool EnableHotUpdate;
        public List<string> HotUpdateVersion;
        public bool EnablePackage;
        public List<string> SubPackage;
        public Dictionary<string, (long size, string md5)> SubPackageInfo;

        public GameVersion(string version, string assetRemoteAddress = null, bool enableHotUpdate = false, 
            List<string> hotUpdateVersion = null, bool enablePackage = false, List<string> subPackage = null,
            Dictionary<string, (long size, string md5)> subPackageInfo = null)
        {
            Version = version;
            AssetRemoteAddress = assetRemoteAddress;
            EnableHotUpdate = enableHotUpdate;
            HotUpdateVersion = hotUpdateVersion;
            EnablePackage = enablePackage;
            SubPackage = subPackage;
            SubPackageInfo = subPackageInfo;
        }
        
        public GameVersion()
        {
            
        }
    }
    
    public class GameConfig
    {
        public static GameVersion LocalGameVersion = new GameVersion();

        public static GameVersion RemoteGameVersion = new GameVersion();

        public static Dictionary<string, AssetBundleMap.AssetMapping> RemoteAssetBundleMap =
            new Dictionary<string, AssetBundleMap.AssetMapping>();

        private static Func<string> _assetRemoteAddressGetter;

        /// <summary>
        /// 设置最终远程资产根地址，返回值会直接作为热更根地址使用，如：https://cdn-test.xxx.com/hotfix/Remote/Android
        /// </summary>
        public static void SetAssetRemoteFinalAddressGetter(Func<string> getter)
        {
            _assetRemoteAddressGetter = getter;
        }

        /// <summary>
        /// 设置 CDN 基础地址，框架会自动追加 Remote/平台，如：https://cdn-test.xxx.com/hotfix -> https://cdn-test.xxx.com/hotfix/Remote/Android
        /// </summary>
        public static void SetAssetRemoteBaseAddressGetter(Func<string> getter)
        {
            if (getter == null)
            {
                _assetRemoteAddressGetter = null;
                return;
            }

            _assetRemoteAddressGetter = () => BuildAssetRemoteAddress(getter());
        }

        public static string GetAssetRemoteAddress()
        {
            if (_assetRemoteAddressGetter != null)
            {
                return NormalizeAssetRemoteAddress(_assetRemoteAddressGetter());
            }

            return NormalizeAssetRemoteAddress(LocalGameVersion.AssetRemoteAddress);
        }

        private static string NormalizeAssetRemoteAddress(string assetRemoteAddress)
        {
            if (string.IsNullOrEmpty(assetRemoteAddress))
            {
                return string.Empty;
            }

            return FileTools.FormatToUnityPath(assetRemoteAddress.Trim()).TrimEnd('/');
        }

        public static string BuildAssetRemoteAddress(string assetRemoteBaseAddress)
        {
            if (string.IsNullOrEmpty(assetRemoteBaseAddress))
            {
                return string.Empty;
            }

            string address = NormalizeAssetRemoteAddress(assetRemoteBaseAddress);
            string remotePlatformSuffix = "/Remote/" + URLSetting.GetPlatformName();
            if (address.EndsWith(remotePlatformSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return address;
            }

            if (address.EndsWith("/Remote", StringComparison.OrdinalIgnoreCase))
            {
                return CombineUrl(address, URLSetting.GetPlatformName());
            }

            return CombineUrl(address, "Remote", URLSetting.GetPlatformName());
        }

        public static string CombineAssetRemoteUrl(params string[] paths)
        {
            string assetRemoteAddress = GetAssetRemoteAddress();
            if (string.IsNullOrEmpty(assetRemoteAddress))
            {
                return string.Empty;
            }

            if (paths == null || paths.Length == 0)
            {
                return assetRemoteAddress;
            }

            string[] urlParts = new string[paths.Length + 1];
            urlParts[0] = assetRemoteAddress;
            Array.Copy(paths, 0, urlParts, 1, paths.Length);
            return CombineUrl(urlParts);
        }

        private static string CombineUrl(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return string.Empty;
            }

            string result = string.Empty;
            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                string value = FileTools.FormatToUnityPath(path.Trim());
                if (string.IsNullOrEmpty(result))
                {
                    result = value.TrimEnd('/');
                }
                else
                {
                    result = result.TrimEnd('/') + "/" + value.Trim('/');
                }
            }

            return result;
        }

        /// <summary>
        /// 判断版本号大小，1为version1大，-1为version2大，0为一样大
        /// </summary>
        /// <param name="version1">版本1</param>
        /// <param name="version2">版本2</param>
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
