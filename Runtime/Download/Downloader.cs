using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    /// <summary>
    /// 文件下载器；
    /// </summary>
    public class Downloader : IDownloader
    {
        static long downloadId = 0;
        
        private static int downloadTimeout = 0;

        #region events

        Action<DownloadStartEventArgs> onDownloadStart;
        Action<DownloadSuccessEventArgs> onDownloadSuccess;
        Action<DownloadFailureEventArgs> onDownloadFailure;
        Action<DonwloadUpdateEventArgs> onDownloadOverall;
        Action<DownloadTasksCompletedEventArgs> onAllDownloadTaskCompleted;

        /// <summary>
        /// 设置超时时间，默认为无超时时间
        /// </summary>
        public int DownloadTimeout
        {
            get { return downloadTimeout; }
            set { downloadTimeout = value; }
        }
        
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        public event Action<DownloadStartEventArgs> OnDownloadStart
        {
            add { onDownloadStart += value; }
            remove { onDownloadStart -= value; }
        }

        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        public event Action<DownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { onDownloadSuccess += value; }
            remove { onDownloadSuccess -= value; }
        }

        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        public event Action<DownloadFailureEventArgs> OnDownloadFailure
        {
            add { onDownloadFailure += value; }
            remove { onDownloadFailure -= value; }
        }

        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        public event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress
        {
            add { onDownloadOverall += value; }
            remove { onDownloadOverall -= value; }
        }

        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        public event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted
        {
            add { onAllDownloadTaskCompleted += value; }
            remove { onAllDownloadTaskCompleted -= value; }
        }

        #endregion

        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; private set; }

        /// <summary>
        /// 下载中的资源总数；
        /// </summary>
        public int DownloadingCount
        {
            get { return pendingTasks.Count; }
        }

        /// <summary>
        /// 下载任务数量；
        /// </summary>
        int downloadTaskCount = 0;

        /// <summary>
        /// 挂起的下载任务；
        /// </summary>
        readonly List<DownloadTask> pendingTasks;

        /// <summary>
        /// 挂起的下载任务字典缓存；
        /// </summary>
        readonly Dictionary<long, DownloadTask> pendingTaskDict;

        /// <summary>
        /// 下载成功的任务;
        /// </summary>
        readonly List<DownloadInfo> successedInfos;

        /// <summary>
        /// 下载失败的任务;
        /// </summary>
        readonly List<DownloadInfo> failedInfos;

        /// <summary>
        /// 下载开始时间；
        /// </summary>
        DateTime downloadStartTime;

        /// <summary>
        /// 下载结束时间；
        /// </summary>
        DateTime downloadEndTime;

        /// <summary>
        /// web下载client;
        /// </summary>
        UnityWebRequest unityWebRequest;

        /// <summary>
        /// 当前下载的序号；
        /// </summary>
        int currentDownloadTaskIndex = 0;

        /// <summary>
        /// 当前是否可下载；
        /// </summary>
        bool canDownload;

        /// <summary>
        /// 总共需要下载的文件大小
        /// </summary>
        long totalRequirementDownloadLength;

        /// <summary>
        /// 已经下载的文件大小
        /// </summary>
        long completedDownloadLength;

        public Downloader()
        {
            pendingTasks = new List<DownloadTask>();
            pendingTaskDict = new Dictionary<long, DownloadTask>();
            successedInfos = new List<DownloadInfo>();
            failedInfos = new List<DownloadInfo>();
        }
        
        /// <summary>
        /// 添加下载任务，并返回下载任务的唯一标识符。
        /// </summary>
        /// <param name="downloadUri">下载资源的URI。</param>
        /// <param name="downloadPath">下载资源保存的本地路径。</param>
        /// <param name="downloadByteOffset">下载偏移字节。</param>
        /// <param name="downloadAppend">是否追加下载。</param>
        /// <returns>新添加的下载任务的唯一标识符。</returns>
        public long AddDownload(string downloadUri, string downloadPath, long downloadByteOffset = 0, bool downloadAppend = false)
        {
            Util.Text.IsStringValid(downloadUri, "URI is invalid !");
            Util.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
            // 创建新的下载任务
            var downloadTask = new DownloadTask(downloadId++, downloadUri, downloadPath, downloadByteOffset,
                downloadAppend);
            // 将下载任务添加到待处理任务字典和列表中
            pendingTaskDict.Add(downloadTask.DownloadId, downloadTask);
            pendingTasks.Add(downloadTask);
            downloadTaskCount++;
            return downloadTask.DownloadId;
        }
        
        /// <summary>
        /// 移除指定唯一标识符的下载任务。
        /// </summary>
        /// <param name="downloadIds">要移除的下载任务的唯一标识符。</param>
        public void RemoveDownloads(long[] downloadIds)
        {
            var length = downloadIds.Length;
            for (int i = 0; i < length; i++)
                RemoveDownload(downloadIds[i]);
        }
        
        /// <summary>
        /// 移除指定唯一标识符的下载任务。
        /// </summary>
        /// <param name="downloadId">要移除的下载任务的唯一标识符。</param>
        /// <returns>移除是否成功。</returns>
        public bool RemoveDownload(long downloadId)
        {
            if (pendingTaskDict.Remove(downloadId, out var downloadTask))
            {
                pendingTasks.Remove(downloadTask);
                downloadTaskCount--;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 启动下载任务。
        /// </summary>
        public void LaunchDownload()
        {
            // 如果正在下载，则不执行任何操作
            if (Downloading)
                return;
            // 标记可以开始下载
            canDownload = true;
            // 如果没有待处理任务或者不能下载，则返回
            if (pendingTasks.Count == 0 || !canDownload)
            {
                canDownload = false;
                return;
            }

            // 记录下载开始时间
            downloadStartTime = DateTime.Now;
            // 开始下载多个文件
            Util.Unity.StartCoroutine(RunDownloadMultipleFiles());
        }

        /// <summary>
        /// 移除所有下载任务。
        /// </summary>
        public void RemoveAllDownload()
        {
            OnCancelDownload();
        }

        /// <summary>
        /// 取消当前正在进行的下载任务。
        /// </summary>
        public void CancelDownload()
        {
            OnCancelDownload();
        }

        /// <summary>
        /// 释放下载器资源。
        /// </summary>
        public void Release()
        {
            // 清空事件委托
            onDownloadStart = null;
            onDownloadSuccess = null;
            onDownloadFailure = null;
            onDownloadOverall = null;
            onAllDownloadTaskCompleted = null;
            // 重置下载任务计数
            downloadTaskCount = 0;
        }

        /// <summary>
        /// 多文件下载迭代器方法；
        /// </summary>
        /// <returns>迭代器接口</returns>
        IEnumerator RunDownloadMultipleFiles()
        {
            Downloading = true;
            while (pendingTasks.Count > 0)
            {
                var downloadTask = pendingTasks.RemoveFirst();
                currentDownloadTaskIndex = downloadTaskCount - pendingTasks.Count;
                yield return RunDownloadSingleFile(downloadTask);
                pendingTaskDict.Remove(downloadTask.DownloadId);
            }

            OnPendingTasksCompleted();
            Downloading = false;
        }

        /// <summary>
        /// 单文件下载迭代器方法
        /// </summary>
        /// <param name="downloadTask">下载任务对象</param>
        /// <returns>迭代器接口</returns>
        IEnumerator RunDownloadSingleFile(DownloadTask downloadTask)
        {
            var downloadUrl = downloadTask.DownloadUrl;
            var downloadPath = downloadTask.DownloadPath;
            var fileDownloadStartTime = DateTime.Now;
            var startTime = DateTime.Now;
            using (UnityWebRequest request = UnityWebRequest.Get(downloadUrl))
            {
                var append = downloadTask.DownloadAppend;
                var deleteFailureFile = DownloadDataProxy.DeleteFileOnAbort;
#if UNITY_2019_1_OR_NEWER
                var handler = new DownloadHandlerFile(downloadTask.DownloadPath, append)
                    { removeFileOnAbort = deleteFailureFile };
#elif UNITY_2018_1_OR_NEWER
                var handler = new DownloadHandlerFile(downloadTask.DownloadPath) {  removeFileOnAbort =
 deleteFailureFile};
#endif
                request.SetRequestHeader("Range", "bytes=" + downloadTask.DownloadByteOffset + "-");
                request.downloadHandler = handler;
                unityWebRequest = request;
                request.timeout = downloadTimeout;
                request.redirectLimit = DownloadDataProxy.RedirectLimit;
                {
                    var timeSpan = DateTime.Now - fileDownloadStartTime;
                    var downloadInfo = new DownloadInfo(downloadTask.DownloadId, downloadUrl, downloadPath, 0, 0,
                        timeSpan);
                    var startEventArgs =
                        DownloadStartEventArgs.Create(downloadInfo, currentDownloadTaskIndex, downloadTaskCount);
                    onDownloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);
                }
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    var timeSpan = DateTime.Now - fileDownloadStartTime;
                    var downloadInfo = new DownloadInfo(downloadTask.DownloadId, downloadUrl, downloadPath,
                        request.downloadedBytes, operation.progress, timeSpan);
                    OnFileDownloading(downloadInfo);
                    yield return null;
                }
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError &&
                    request.result != UnityWebRequest.Result.ProtocolError && canDownload)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError && canDownload)
#endif
                {
                    if (request.isDone)
                    {
                        var timeSpan = DateTime.Now - fileDownloadStartTime;
                        var downloadInfo = new DownloadInfo(downloadTask.DownloadId, downloadUrl, downloadPath,
                            request.downloadedBytes, 1, timeSpan);
                        var successEventArgs = DownloadSuccessEventArgs.Create(downloadInfo, currentDownloadTaskIndex,
                            downloadTaskCount, timeSpan);
                        onDownloadSuccess?.Invoke(successEventArgs);
                        OnFileDownloading(downloadInfo);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successedInfos.Add(downloadInfo);
                    }
                }
                else
                {
                    var timeSpan = DateTime.Now - fileDownloadStartTime;
                    var downloadInfo = new DownloadInfo(downloadTask.DownloadId, downloadUrl, downloadPath,
                        request.downloadedBytes, operation.progress, timeSpan);
                    var failureEventArgs = DownloadFailureEventArgs.Create(downloadInfo, currentDownloadTaskIndex,
                        downloadTaskCount, request.error, timeSpan);
                    onDownloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failedInfos.Add(downloadInfo);
                    OnFileDownloading(downloadInfo);
                    unityWebRequest = null;
                }
            }
        }

        /// <summary>
        /// 请求header以获取文件大小
        /// </summary>
        /// <param name="downloadTask">下载任务对象</param>
        /// <returns>迭代器接口</returns>
        IEnumerator RunRequestHeader(DownloadTask downloadTask)
        {
            var downloadUrl = downloadTask.DownloadUrl;
            using (UnityWebRequest request = UnityWebRequest.Head(downloadUrl))
            {
                //这部分通过header获取需要下载的文件大小
                unityWebRequest = request;
                request.timeout = downloadTimeout;
                request.redirectLimit = DownloadDataProxy.RedirectLimit;
                yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError &&
                    request.result != UnityWebRequest.Result.ProtocolError && canDownload)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    var fileLength = long.Parse(request.GetRequestHeader("Content-Length"));
                    //totalRequiredDownloadLength += fileLength;
                }
                else
                {
                    //yield break;
                }
            }
        }

        /// <summary>
        /// 处理整体进度；
        /// </summary>
        void OnFileDownloading(DownloadInfo info)
        {
            var timeSpan = DateTime.Now - downloadStartTime;
            var eventArgs = DonwloadUpdateEventArgs.Create(info, currentDownloadTaskIndex, downloadTaskCount, timeSpan);
            onDownloadOverall?.Invoke(eventArgs);
            DonwloadUpdateEventArgs.Release(eventArgs);
        }

        /// <summary>
        /// 当Pending uri的文件全部下载完成；
        /// </summary>
        void OnPendingTasksCompleted()
        {
            canDownload = false;
            downloadEndTime = DateTime.Now;
            var eventArgs = DownloadTasksCompletedEventArgs.Create(successedInfos.ToArray(), failedInfos.ToArray(),
                downloadEndTime - downloadStartTime, downloadTaskCount);
            onAllDownloadTaskCompleted?.Invoke(eventArgs);
            DownloadTasksCompletedEventArgs.Release(eventArgs);
            //清理下载配置缓存；
            failedInfos.Clear();
            successedInfos.Clear();
            pendingTasks.Clear();
            downloadTaskCount = 0;
            pendingTaskDict.Clear();
        }

        void OnCancelDownload()
        {
            if (Downloading)
                unityWebRequest?.Abort();
            else
                unityWebRequest?.Dispose();
            //foreach (var task in pendingTasks)
            //{
            //    var completedInfo = new DownloadCompletedInfo(task.URI, task.DownloadPath, 0, TimeSpan.Zero);
            //    failedInfos.Add(completedInfo);
            //}
            //todo 这里需要将pending列表中的任务变更为下载失败
            pendingTasks.Clear();
            pendingTaskDict.Clear();
            downloadTaskCount = 0;
            failedInfos.Clear();
            successedInfos.Clear();
            canDownload = false;
            Downloading = false;
        }
    }
}