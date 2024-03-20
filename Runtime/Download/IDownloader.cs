using System;

namespace F8Framework.Core
{
    /// <summary>
    /// 下载模块下载器接口；
    /// </summary>
    public interface IDownloader
    {
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        event Action<DownloadStartEventArgs> OnDownloadStart;

        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        event Action<DownloadSuccessEventArgs> OnDownloadSuccess;

        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        event Action<DownloadFailureEventArgs> OnDownloadFailure;

        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress;

        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted;

        /// <summary>
        /// 是否正在下载；
        /// </summary>
        bool Downloading { get; }

        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        int DownloadingCount { get; }

        /// <summary>
        /// 添加URI下载；
        /// </summary>
        /// <param name="downloadUri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        /// <param name="downloadByteOffset">下载byte的偏移量，用于断点续传</param>
        /// <param name="downloadAppend">当本地存在时，下载时追加写入</param>
        /// <returns>下载序列号</returns>
        long AddDownload(string downloadUri, string downloadPath, long downloadByteOffset = 0, bool downloadAppend = false);

        /// <summary>
        /// 移除URI下载；
        /// </summary>
        /// <param name="downloadId">下载序号</param>
        /// <returns>移除结果</returns>
        bool RemoveDownload(long downloadId);

        /// <summary>
        /// 移除所有下载；
        /// </summary>
        void RemoveAllDownload();

        /// <summary>
        /// 启动下载；
        /// </summary>
        void LaunchDownload();

        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        void CancelDownload();

        /// <summary>
        /// 释放下载器；
        /// </summary>
        void Release();
    }
}