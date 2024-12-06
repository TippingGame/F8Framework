using System.IO;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 为资产捆绑管理器提供资源类型判断和路由帮助。
    /// </summary>
    public static class AssetBundleHelper
    {
        private static string _streamingAssetsPath;

        private static string GetStreamingAssetsPath()
        {
            if (_streamingAssetsPath == null)
            {
                _streamingAssetsPath = Application.streamingAssetsPath + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _streamingAssetsPath;
        }
        
        private static string _remoteAddress;
        
        private static string GetRemoteAddress()
        {
            if (_remoteAddress == null)
            {
                _remoteAddress = GameConfig.LocalGameVersion.AssetRemoteAddress + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _remoteAddress;
        }

        private static string _hotUpdatePath;
        
        private static string GetHotUpdatePath()
        {
            if (_hotUpdatePath == null)
            {
                _hotUpdatePath = Application.persistentDataPath + HotUpdateManager.HotUpdateDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _hotUpdatePath;
        }

        private static string _packagePath;
        
        private static string GetPackagePath()
        {
            if (_packagePath == null)
            {
                _packagePath = Application.persistentDataPath + HotUpdateManager.PackageDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _packagePath;
        }
        
        /// <summary>
        /// 源类型的枚举。
        /// </summary>
        public enum SourceType
        {
            NONE,
            STREAMING_ASSETS,
            HOT_UPDATE_PATH,
            PACKAGE_PATH,
            REMOTE_ADDRESS
        }

        /// <summary>
        /// 根据环境获取资产捆绑包的路径。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundlePath(SourceType type = SourceType.STREAMING_ASSETS)
        {
            string assetBundlePath;
            switch (type)
            {
                case SourceType.STREAMING_ASSETS:
                    assetBundlePath = GetStreamingAssetsPath();
                    break;
                case SourceType.HOT_UPDATE_PATH:
                    assetBundlePath = GetHotUpdatePath();
                    break;
                case SourceType.PACKAGE_PATH:
                    assetBundlePath = GetPackagePath();
                    break;
                case SourceType.REMOTE_ADDRESS:
                    if (string.IsNullOrEmpty(GameConfig.LocalGameVersion.AssetRemoteAddress))
                    {
                        LogF8.LogError("加载远程包需要配置远程地址：AssetRemoteAddress");
                    }
                    assetBundlePath = GetRemoteAddress();
                    break;
                default:
                    return null;
            }
            
            return assetBundlePath;
        }

        /// <summary>
        /// 根据环境获取资产捆绑清单文件的路径。
        /// </summary>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleManifestPath(SourceType type = SourceType.STREAMING_ASSETS)
        {
            string platformAssetBundlePath = GetAssetBundlePath(type);
            if (platformAssetBundlePath == null)
                return null;

            string manifestPath = platformAssetBundlePath + URLSetting.GetPlatformName();
            return manifestPath;
        }

        /// <summary>
        /// 根据环境获取带后缀的资产捆绑清单文件的路径。
        /// </summary>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleManifestPathWithSuffix(SourceType type = SourceType.STREAMING_ASSETS)
        {
            string manifestPath = GetAssetBundleManifestPath(type);
            if (manifestPath == null)
                return null;

            return manifestPath + ".manifest";
        }

        /// <summary>
        /// 获取资产捆绑包的完整路径。
        /// </summary>
        /// <param name="assetBundleFileName">资产捆绑包的文件名。</param>
        /// <param name="type">源类型。</param>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleFullName(string assetBundleFileName = null, SourceType type = SourceType.STREAMING_ASSETS)
        {
            string assetBundlePath = GetAssetBundlePath(type);
            if (assetBundlePath == null)
                return null;

            return assetBundlePath + assetBundleFileName;
        }

        /// <summary>
        /// 通过其完整路径确定资产捆绑包的类型。
        /// </summary>
        /// <param name="assetBundleFullName">完整路径。</param>
        /// <returns>源类型。</returns>
        public static SourceType GetAssetBundleSourceType(string assetBundleFullName)
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            string hotUpdatePath = Application.persistentDataPath + HotUpdateManager.HotUpdateDirName;
            string packagePath = Application.persistentDataPath + HotUpdateManager.PackageDirName;

            if (assetBundleFullName.Contains(streamingAssetsPath))
            {
                return SourceType.STREAMING_ASSETS;
            }
            else if (assetBundleFullName.Contains(hotUpdatePath))
            {
                return SourceType.HOT_UPDATE_PATH;
            }
            else if (assetBundleFullName.Contains(packagePath))
            {
                return SourceType.PACKAGE_PATH;
            }
            else if (FileTools.IsLegalHTTPURI(assetBundleFullName))
            {
                return SourceType.REMOTE_ADDRESS;
            }
            else
            {
                return SourceType.NONE;
            }
        }
    }
}