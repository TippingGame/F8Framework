using System;
using UnityEngine;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    /// <summary>
    /// 资源文件下载器。
    /// 使用 UnityWebRequest（UWR）提供网络内容下载功能。
    /// </summary>
    public class DownloadRequest : IDisposable
    {
        private DownloadType type;
        private UnityWebRequest uwr;
        
        private enum DownloadType
        {
            NONE,
            FILE,
            ASSET_BUNDLE
        }

        /// <summary>
        /// 创建一个文件下载请求。
        /// </summary>
        /// <param name="uri">请求的URI。</param>
        public DownloadRequest(string uri)
        {
            type = DownloadType.FILE;
            SendFileDownloadRequest(uri);
        }

        /// <summary>
        /// 创建一个AssetBundle下载请求。
        /// </summary>
        /// <param name="uri">请求的URI。</param>
        /// <param name="version">
        /// 一个整数版本号，将与AssetBundle的缓存版本进行比较以确定是否下载。
        /// 将此数字递增以强制Unity重新下载缓存的AssetBundle。
        /// 如果为零，则忽略版本分配。
        /// </param>
        /// <param name="crc">
        /// 如果非零，将与已下载AssetBundle数据的校验和进行比较。
        /// 如果CRC不匹配，将记录错误并不加载AssetBundle。
        /// 如果设置为零，将跳过CRC检查。
        /// </param>
        public DownloadRequest(
            string uri,
            uint version,
            uint crc = 0)
        {
            type = DownloadType.ASSET_BUNDLE;
            SendAssetBundleDownloadRequest(uri, version, crc);
        }

        /// <summary>
        /// 终止正在进行的下载。
        /// </summary>
        public void Abort()
        {
            if (!IsFinished)
                uwr.Abort();
        }

        private void SendFileDownloadRequest(string uri)
        {
            try
            {
                if (FileTools.IsLegalURI(uri))
                {
                    uwr = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET, new DownloadHandlerBuffer(), null);
                    uwr.SendWebRequest();
                }
                else
                    LoadFail();
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to send file download request for uri: {0}. Exception: {1}", uri, e.Message);
                LogF8.LogError(message);
                LoadFail();
            }
        }

        private void SendAssetBundleDownloadRequest(string uri, uint version = 0, uint crc = 0)
        {
            try
            {
                if (FileTools.IsLegalURI(uri))
                {
                    if (version == 0)
                        uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri);
                    else
                        uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri, version, crc);
                    uwr.SendWebRequest();
                }
                else
                    LoadFail();
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to asset bundle download request for uri: {0}. Exception: {1}", uri, e.Message);
                LogF8.LogError(message);
                LoadFail();
            }
        }

        private void LoadFail()
        {
            uwr?.Dispose();
            uwr = null;
        }

        /// <summary>
        /// 如果请求完成，则返回 true，否则返回 false。
        /// </summary>
        public bool IsFinished
        {
            get
            {
                if (uwr == null)
                {
                    return true;
                }
                else
                {
                    return uwr.isDone;
                }
            }
        }

        /// <summary>
        /// 请求的下载进度。
        /// </summary>
        public float Progress
        {
            get
            {
                if (uwr != null)
                    return uwr.downloadProgress;
                return 0f;
            }
        }

        /// <summary>
        /// 获取已下载的字节。
        /// 根据请求类型返回相应的值。
        /// </summary>
        public byte[] DownloadedBytes
        {
            get
            {
                if (!IsFinished)
                    return null;

                if ((type == DownloadType.FILE ||
                    type == DownloadType.ASSET_BUNDLE) &&
                    uwr != null)
                    return uwr.downloadHandler.data;

                return null;
            }
        }

        /// <summary>
        /// 获取已下载的文件字节。
        /// </summary>
        public byte[] DownloadedFileBytes
        {
            get
            {
                if (!IsFinished)
                    return null;

                if (type == DownloadType.FILE &&
                    uwr != null)
                    return uwr.downloadHandler.data;

                return null;
            }
        }

        /// <summary>
        /// 获取已下载的AssetBundle。
        /// </summary>
        public AssetBundle DownloadedAssetBundle
        {
            get
            {
                if (!IsFinished)
                    return null;

                if (type == DownloadType.ASSET_BUNDLE &&
                    uwr != null)
                    return DownloadHandlerAssetBundle.GetContent(uwr);

                return null;
            }
        }

        /// <summary>
        /// 获取已下载的文件文本。
        /// </summary>
        public string DownloadedFileText
        {
            get
            {
                if (!IsFinished)
                    return null;

                if (type == DownloadType.FILE &&
                    uwr != null)
                    return uwr.downloadHandler.text;

                return null;
            }
        }

        /// <summary>
        /// 获取已下载的AssetBundle字节。
        /// </summary>
        public byte[] DownloadedAssetBundleBytes
        {
            get
            {
                if (!IsFinished)
                    return null;

                if (type == DownloadType.ASSET_BUNDLE &&
                    uwr != null)
                    return uwr.downloadHandler.data;

                return null;
            }
        }

        public void Dispose()
        {
            uwr?.Dispose();
            uwr = null;
        }
    }
}