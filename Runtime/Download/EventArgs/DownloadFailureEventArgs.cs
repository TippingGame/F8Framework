using System;

namespace F8Framework.Core
{
    public class DownloadFailureEventArgs : IReference
    {
        public DownloadInfo DownloadInfo { get; private set; }
        public string ErrorMessage { get; private set; }
        public int CurrentDownloadTaskIndex { get; private set; }
        public int DownloadTaskCount { get; private set; }
        public TimeSpan TimeSpan { get; private set; }

        public void Clear()
        {
            DownloadInfo = default;
            CurrentDownloadTaskIndex = 0;
            DownloadTaskCount = 0;
            ErrorMessage = null;
            TimeSpan = TimeSpan.Zero;
        }

        public static DownloadFailureEventArgs Create(DownloadInfo info, int currentTaskIndex, int taskCount,
            string errorMessage, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<DownloadFailureEventArgs>();
            eventArgs.DownloadInfo = info;
            eventArgs.CurrentDownloadTaskIndex = currentTaskIndex;
            eventArgs.DownloadTaskCount = taskCount;
            eventArgs.ErrorMessage = errorMessage;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }

        public static void Release(DownloadFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}