using System;
using System.Collections.Generic;

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
    */
    //================================================
    public class DownloadManager : ModuleSingleton<DownloadManager>, IModule
    {
        public bool DeleteFileOnAbort
        {
            get { return DownloadDataProxy.DeleteFileOnAbort; }
            set { DownloadDataProxy.DeleteFileOnAbort = value; }
        }

        /// <summary>
        /// 下载器；
        /// </summary>
        Dictionary<string, IDownloader> downloaders;

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
        
        public void OnInit(object createParam)
        {
            downloadUrlHelper = new DefaultDownloadUrlHelper();
            downloadRequester = new DefaultDownloadRequester();
        }
        
        /// <summary>
        /// 创建下载器
        /// </summary>
        /// <param name="downloaderName"></param>
        /// <param name="_downloader"></param>
        public Downloader CreateDownloader(string downloaderName, Downloader _downloader)
        {
            downloaders ??= new Dictionary<string, IDownloader>();
            if (downloaders.TryAdd(downloaderName, _downloader))
            {
                return _downloader;
            }
            LogF8.LogError("已存在同名的下载器！");
            return null;
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
            foreach (var downloader in downloaders.Values)
            {
                downloader.CancelDownload();
            }
            base.Destroy();
        }
    }
}