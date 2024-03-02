using System;

namespace F8Framework.Core
{
    //================================================
    /*
    * 1、下载模块提供一键式资源下载、本地写入功能。通过监听开放的接口可
    * 以检测下载进度；
    * 
    * 2、载模块支持localhost文件下载与http地址文件的下载。模块下载到本地
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
    public class DownloadManager : ModuleSingleton<DownloadManager>, IModule
    {
        public event Action<DownloadStartEventArgs> OnDownloadStart
        {
            add { downloader.OnDownloadStart += value; }
            remove { downloader.OnDownloadStart -= value; }
        }


        public event Action<DownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { downloader.OnDownloadSuccess += value; }
            remove { downloader.OnDownloadSuccess -= value; }
        }


        public event Action<DownloadFailureEventArgs> OnDownloadFailure
        {
            add { downloader.OnDownloadFailure += value; }
            remove { downloader.OnDownloadFailure -= value; }
        }


        public event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress
        {
            add { downloader.OnDownloadOverallProgress += value; }
            remove { downloader.OnDownloadOverallProgress -= value; }
        }


        public event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted
        {
            add { downloader.OnAllDownloadTaskCompleted += value; }
            remove { downloader.OnAllDownloadTaskCompleted -= value; }
        }


        public bool DeleteFileOnAbort
        {
            get { return DownloadDataProxy.DeleteFileOnAbort; }
            set { DownloadDataProxy.DeleteFileOnAbort = value; }
        }


        public int DownloadTimeout
        {
            get { return DownloadDataProxy.DownloadTimeout; }
            set { DownloadDataProxy.DownloadTimeout = value; }
        }


        public bool Downloading
        {
            get { return downloader.Downloading; }
        }


        public int DownloadingCount
        {
            get { return downloader.DownloadingCount; }
        }

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


        /// <summary>
        /// 设置或更新下载器。
        /// </summary>
        /// <param name="newDownloader">要设置或更新的新下载器。</param>
        public void SetOrUpdateDownloadHelper(IDownloader newDownloader)
        {
            if (this.downloader != null)
            {
                this.downloader.CancelDownload();
                this.downloader.Release();
                this.downloader = newDownloader;
            }
        }

        /// <summary>
        /// 设置下载资源地址帮助体。
        /// </summary>
        /// <param name="helper">要设置的下载资源地址帮助体。</param>
        public void SetUrlHelper(IDownloadUrlHelper helper)
        {
            this.downloadUrlHelper = helper;
        }

        /// <summary>
        /// 设置下载请求器帮助体。
        /// </summary>
        /// <param name="helper">要设置的下载请求器帮助体。</param>
        public void SetRequesterHelper(IDownloadRequester helper)
        {
            this.downloadRequester = helper;
        }

        /// <summary>
        /// 添加下载任务。
        /// </summary>
        /// <param name="downloadUri">下载资源的URI。</param>
        /// <param name="downloadPath">下载资源保存的本地路径。</param>
        /// <param name="downloadByteOffset">下载偏移字节。</param>
        /// <param name="downloadAppend">是否追加下载。</param>
        /// <returns>下载任务的唯一标识符。</returns>
        public long AddDownload(string downloadUri, string downloadPath, long downloadByteOffset = 0, bool downloadAppend = false)
        {
            Util.Text.IsStringValid(downloadUri, "URI is invalid !");
            Util.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
            return downloader.AddDownload(downloadUri, downloadPath, downloadByteOffset, downloadAppend);
        }

        /// <summary>
        /// 移除下载任务。
        /// </summary>
        /// <param name="downloadId">要移除的下载任务的唯一标识符。</param>
        /// <returns>移除是否成功。</returns>
        public bool RemoveDownload(long downloadId)
        {
            return downloader.RemoveDownload(downloadId);
        }

        /// <summary>
        /// 移除多个下载任务。
        /// </summary>
        /// <param name="downloadIds">要移除的下载任务的唯一标识符数组。</param>
        public void RemoveDownloads(long[] downloadIds)
        {
            var length = downloadIds.Length;
            for (int i = 0; i < length; i++)
                RemoveDownload(downloadIds[i]);
        }

        /// <summary>
        /// 移除所有下载任务。
        /// </summary>
        public void RemoveAllDownload()
        {
            downloader.RemoveAllDownload();
        }

        /// <summary>
        /// 异步获取URI文件大小。
        /// </summary>
        /// <param name="downloadUri">下载资源的URI。</param>
        /// <param name="callback">获取完成后的回调函数。</param>
        public void GetUriFileSizeAsync(string downloadUri, Action<long> callback)
        {
            Util.Text.IsStringValid(downloadUri, "URI is invalid !");
            if (callback == null)
                throw new ArgumentNullException("Callback is invalid !");
            downloadRequester.GetUriFileSizeAsync(downloadUri, callback);
        }

        /// <summary>
        /// 异步获取URL中文件的总大小。
        /// </summary>
        /// <param name="downloadUrl">下载资源的URL。</param>
        /// <param name="callback">获取完成后的回调函数。</param>
        public void GetUrlFilesSizeAsync(string downloadUrl, Action<long> callback)
        {
            Util.Text.IsStringValid(downloadUrl, "URL is invalid !");
            var relUris = downloadUrlHelper.ParseUrlToRelativeUris(downloadUrl);
            downloadRequester.GetUriFilesSizeAsync(relUris, callback);
        }

        /// <summary>
        /// 启动下载。
        /// </summary>
        public void LaunchDownload()
        {
            downloader.LaunchDownload();
        }

        /// <summary>
        /// 取消下载。
        /// </summary>
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