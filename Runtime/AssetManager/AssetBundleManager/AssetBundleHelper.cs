using System.IO;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 为资产捆绑管理器提供资源类型判断和路由帮助。
    /// </summary>
    public static class AssetBundleHelper
    {
        /// <summary>
        /// 源类型的枚举。
        /// </summary>
        public enum SourceType
        {
            NONE,
            STREAMING_ASSETS,
            PERSISTENT_DATA_PATH,
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
                    assetBundlePath = Application.streamingAssetsPath;
                    break;
                case SourceType.PERSISTENT_DATA_PATH:
                    assetBundlePath = Application.persistentDataPath;
                    break;
                case SourceType.REMOTE_ADDRESS:
                    assetBundlePath = URLSetting.REMOTE_ADDRESS;
                    break;
                default:
                    return null;
            }

            assetBundlePath += "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName();
            
            return assetBundlePath;
        }

        /// <summary>
        /// 根据环境获取资产捆绑清单文件的路径。
        /// </summary>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleManifestPath()
        {
            string platformAssetBundlePath = GetAssetBundlePath();
            if (platformAssetBundlePath == null)
                return null;

            string manifestPath = platformAssetBundlePath + "/" + Path.GetFileName(platformAssetBundlePath);
            return manifestPath;
        }

        /// <summary>
        /// 根据环境获取带后缀的资产捆绑清单文件的路径。
        /// </summary>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleManifestPathWithSuffix()
        {
            string manifestPath = GetAssetBundleManifestPath();
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
        public static string GetAssetBundleFullName(string assetBundleFileName, SourceType type = SourceType.STREAMING_ASSETS)
        {
            string assetBundlePath = GetAssetBundlePath(type);
            if (assetBundlePath == null)
                return null;

            return assetBundlePath + "/" + assetBundleFileName;
        }

        /// <summary>
        /// 通过其完整路径确定资产捆绑包的类型。
        /// </summary>
        /// <param name="assetBundleFullName">完整路径。</param>
        /// <returns>源类型。</returns>
        public static SourceType GetAssetBundleSourceType(string assetBundleFullName)
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            string persistentDataPath = Application.persistentDataPath;

            if (assetBundleFullName.Contains(streamingAssetsPath))
            {
                return SourceType.STREAMING_ASSETS;
            }
            else if (assetBundleFullName.Contains(persistentDataPath))
            {
                return SourceType.PERSISTENT_DATA_PATH;
            }
            else
            {
                return SourceType.NONE;
            }
        }
    }
}