using System;

namespace F8Framework.Core
{
    public class DownloadTasksCompletedEventArgs : IReference
    {
        public DownloadInfo[] SuccessedInfos { get; private set; }
        public DownloadInfo[] FailedInfos { get; private set; }

        /// <summary>
        /// 下载所使用的时间；
        /// </summary>
        public TimeSpan TimeSpan { get; private set; }

        public int DownloadTaskCount { get; private set; }

        public void Clear()
        {
            SuccessedInfos = null;
            FailedInfos = null;
            TimeSpan = TimeSpan.Zero;
            DownloadTaskCount = 0;
        }

        // TODO 下载模块需要新增下载时间差异
        public static DownloadTasksCompletedEventArgs Create(DownloadInfo[] successedInfos, DownloadInfo[] failedInfos,
            TimeSpan timeSpan, int downloadedCount)
        {
            var eventArgs = ReferencePool.Acquire<DownloadTasksCompletedEventArgs>();
            eventArgs.SuccessedInfos = successedInfos;
            eventArgs.FailedInfos = failedInfos;
            eventArgs.TimeSpan = timeSpan;
            eventArgs.DownloadTaskCount = downloadedCount;
            return eventArgs;
        }

        public static void Release(DownloadTasksCompletedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}