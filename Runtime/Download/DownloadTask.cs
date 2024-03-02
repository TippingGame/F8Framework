using System;

namespace F8Framework.Core
{
    internal struct DownloadTask : IEquatable<DownloadTask>
    {
        public long DownloadId { get; private set; }

        /// <summary>
        /// URL绝对路径；
        /// </summary>
        public string DownloadUrl { get; private set; }

        /// <summary>
        /// 本地资源的绝对路径；
        /// </summary>
        public string DownloadPath { get; private set; }

        /// <summary>
        /// 下载byte的偏移量，用于断点续传
        /// </summary>
        public long DownloadByteOffset { get; private set; }

        /// <summary>
        /// 当本地存在时，下载时追加写入
        /// </summary>
        public bool DownloadAppend { get; private set; }

        /// <summary>
        /// 下载任务的构造函数；
        /// </summary>
        /// <param name="dwnloadId">下载Id</param>
        /// <param name="downloadUrl">URL绝对路径</param>
        /// <param name="downloadPath">本地资源的绝对路径</param>
        /// <param name="downloadByteOffset">下载byte的偏移量，用于断点续传，全部重下则使用0</param>
        /// <param name="downloadAppend">当本地存在时，下载时追加写入</param>
        public DownloadTask(long dwnloadId, string downloadUrl, string downloadPath, long downloadByteOffset,
            bool downloadAppend)
        {
            DownloadId = dwnloadId;
            DownloadUrl = downloadUrl;
            DownloadPath = downloadPath;
            DownloadByteOffset = downloadByteOffset;
            DownloadAppend = downloadAppend;
        }

        public bool Equals(DownloadTask other)
        {
            bool result = false;
            if (this.GetType() == other.GetType())
            {
                result = this.DownloadUrl == other.DownloadUrl &&
                         this.DownloadPath == other.DownloadPath &&
                         this.DownloadId == other.DownloadId &&
                         this.DownloadByteOffset == other.DownloadByteOffset &&
                         this.DownloadAppend == other.DownloadAppend;
            }

            return result;
        }
    }
}