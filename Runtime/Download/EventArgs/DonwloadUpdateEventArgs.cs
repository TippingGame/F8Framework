using System;

namespace F8Framework.Core
{
    public class DonwloadUpdateEventArgs : IReference
    {
        /// <summary>
        /// current download info
        /// </summary>
        public DownloadInfo DownloadInfo { get; private set; }

        public int CurrentDownloadTaskIndex { get; private set; }
        public int DownloadTaskCount { get; private set; }
        public TimeSpan TimeSpan { get; private set; }

        public void Clear()
        {
            DownloadInfo = default;
            CurrentDownloadTaskIndex = 0;
            DownloadTaskCount = 0;
            TimeSpan = TimeSpan.Zero;
        }

        public static DonwloadUpdateEventArgs Create(DownloadInfo downloadInfo, int currentTaskIndex, int taskCount,
            TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<DonwloadUpdateEventArgs>();
            eventArgs.DownloadInfo = downloadInfo;
            eventArgs.CurrentDownloadTaskIndex = currentTaskIndex;
            eventArgs.DownloadTaskCount = taskCount;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }

        public static void Release(DonwloadUpdateEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}