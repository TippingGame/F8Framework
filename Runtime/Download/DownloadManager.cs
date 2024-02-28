using System;
namespace F8Framework.Core
{
    //================================================
    /*
     * 1、下载模块提供一键式资源下载、本地写入功能。通过监听开放的接口可
    * 以检测下载进度；
    * 
    * 2、载模块支持localhost文件下载与http地址文件的下载。模块下载到本
    *后，增量写入，以防下载错误导致的整体文件丢失。
    *
    *3、载模块支持断点续下。
    *
    *4、支持动态添加、移除、暂停、恢复下载任务；
    *
    *5、若不自定义设置下载器，(调用SetOrUpdateDownloadHelper方法)，则
    * 使用框架原生默认的下载器；
     */
    //================================================
    internal class DownloadManager : ModuleSingleton<DownloadManager>, IModule
    {
        #region events
        ///<inheritdoc/>
        public event Action<DownloadStartEventArgs> OnDownloadStart
        {
            add { downloader.OnDownloadStart += value; }
            remove { downloader.OnDownloadStart -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { downloader.OnDownloadSuccess += value; }
            remove { downloader.OnDownloadSuccess -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadFailureEventArgs> OnDownloadFailure
        {
            add { downloader.OnDownloadFailure += value; }
            remove { downloader.OnDownloadFailure -= value; }
        }
        ///<inheritdoc/>
        public event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress
        {
            add { downloader.OnDownloadOverallProgress += value; }
            remove { downloader.OnDownloadOverallProgress -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted
        {
            add { downloader.OnAllDownloadTaskCompleted += value; }
            remove { downloader.OnAllDownloadTaskCompleted -= value; }
        }
        #endregion
        ///<inheritdoc/>
        public bool DeleteFileOnAbort
        {
            get { return DownloadDataProxy.DeleteFileOnAbort; }
            set { DownloadDataProxy.DeleteFileOnAbort = value; }
        }
        ///<inheritdoc/>
        public int DownloadTimeout
        {
            get { return DownloadDataProxy.DownloadTimeout; }
            set { DownloadDataProxy.DownloadTimeout = value; }
        }
        ///<inheritdoc/>
        public bool Downloading { get { return downloader.Downloading; } }
        ///<inheritdoc/>
        public int DownloadingCount { get { return downloader.DownloadingCount; } }
        /// <summary>
        /// 下载器；
        /// </summary>
        IDownloader downloader;
        /// <summary>
        /// 下载请求器，专门用于获取文件大小；
        /// </summary>
        IDownloadRequester downloadRequester;
        /// <summary>
        /// 下载资源地址帮助体；用于解析URL中的文件列表；
        /// 支持localhost地址与http地址；
        /// </summary>
        IDownloadUrlHelper downloadUrlHelper;

        ///<inheritdoc/>
        public void SetOrUpdateDownloadHelper(IDownloader newDownloader)
        {
            if (this.downloader != null)
            {
                this.downloader.CancelDownload();
                this.downloader.Release();
                this.downloader = newDownloader;
            }
        }
        ///<inheritdoc/>
        public void SetUrlHelper(IDownloadUrlHelper helper)
        {
            this.downloadUrlHelper = helper;
        }
        ///<inheritdoc/>
        public void SetRequesterHelper(IDownloadRequester helper)
        {
            this.downloadRequester = helper;
        }
        ///<inheritdoc/>
        public long AddDownload(string downloadUri, string downloadPath, long downloadByteOffset, bool downloadAppend)
        {
            Utility.Text.IsStringValid(downloadUri, "URI is invalid !");
            Utility.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
            return downloader.AddDownload(downloadUri, downloadPath, downloadByteOffset, downloadAppend);
        }
        ///<inheritdoc/>
        public bool RemoveDownload(long downloadId)
        {
            return downloader.RemoveDownload(downloadId);
        }
        ///<inheritdoc/>
        public void RemoveDownloads(long[] downloadIds)
        {
            var length = downloadIds.Length;
            for (int i = 0; i < length; i++)
                RemoveDownload(downloadIds[i]);
        }
        ///<inheritdoc/>
        public void RemoveAllDownload()
        {
            downloader.RemoveAllDownload();
        }
        ///<inheritdoc/>
        public void GetUriFileSizeAsync(string downloadUri, Action<long> callback)
        {
            Utility.Text.IsStringValid(downloadUri, "URI is invalid !");
            if (callback == null)
                throw new ArgumentNullException("Callback is invalid !");
            downloadRequester.GetUriFileSizeAsync(downloadUri, callback);
        }
        ///<inheritdoc/>
        public void GetUrlFilesSizeAsync(string downloadUrl, Action<long> callback)
        {
            Utility.Text.IsStringValid(downloadUrl, "URL is invalid !");
            var relUris = downloadUrlHelper.ParseUrlToRelativeUris(downloadUrl);
            downloadRequester.GetUriFilesSizeAsync(relUris, callback);
        }
        ///<inheritdoc/>
        public void LaunchDownload()
        {
            downloader.LaunchDownload();
        }
        ///<inheritdoc/>
        public void CancelDownload()
        {
            downloader.CancelDownload();
        }

        public void OnInit(object createParam)
        {
            var unityWebDownloader = new Downloader();
            downloader = unityWebDownloader;
            downloadUrlHelper = new DefaultDownloadUrlHelper();
            downloadRequester = new DefaultDownloadRequester();
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            downloader.CancelDownload();
            base.Destroy();
        }
    }
}
