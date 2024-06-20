using System;
using System.IO;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoDownload : MonoBehaviour
    {
        private string[] fileInfos = new[]
        {
            "https://raw.githubusercontent.com/TippingGame/F8Framework/main/Tests/AssetManager/ui_20240216212631.png",
            "https://raw.githubusercontent.com/TippingGame/F8Framework/main/Tests/AssetManager/ui_20240205230012.png"
        };

        private Downloader downloader;
        void Start()
        {
            // 创建下载器
            downloader = FF8.Download.CreateDownloader("Download", new Downloader());

            // 设置超时时间，默认为无超时时间
            downloader.DownloadTimeout = 30;
            
            // 设置下载器回调
            downloader.OnDownloadSuccess += OnDownloadSucess;
            downloader.OnDownloadFailure += OnDownloadFailure;
            downloader.OnDownloadStart += OnDownloadStart;
            downloader.OnDownloadOverallProgress += OnDownloadOverall;
            downloader.OnAllDownloadTaskCompleted += OnDownloadFinish;
            
            int count = 0;
            // 添加下载清单
            foreach (var fileInfo in fileInfos)
            {
                count += 1;
                // 获取文件大小，用于断点续传（可选）
                FileInfo file = new FileInfo(Application.persistentDataPath + "F8Download/download" + count + ".png");
                long fileSizeInBytes = file.Length;
                downloader.AddDownload(fileInfo, Application.persistentDataPath + "F8Download/download" + count + ".png", fileSizeInBytes, true);
            }
            
            // 下载器开始下载
            downloader.LaunchDownload();
            
            // 获取URL中文件的总大小，部分下载任务本身仅知道下载连接，无法获取需要下载的二进制长度
            FF8.Download.GetUrlFilesSizeAsync("", l => LogF8.Log(l));
            
            // 取消下载
            downloader.CancelDownload();
        }
        
        // 开始下载
        void OnDownloadStart(DownloadStartEventArgs eventArgs)
        {
            LogF8.Log(eventArgs.DownloadInfo.DownloadUrl);
        }
        
        void OnDownloadOverall(DonwloadUpdateEventArgs eventArgs)
        {
            // 部分下载任务本身仅知道下载连接，无法获取需要下载的二进制长度，无法使用更精准的进度。
            float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
            float taskCount = (float)eventArgs.DownloadTaskCount;

            // 计算进度百分比
            float progress = currentTaskIndex / taskCount * 100f;
            // LogF8.Log(progress);
        }
        
        // 下载成功
        void OnDownloadSucess(DownloadSuccessEventArgs eventArgs)
        {
            LogF8.Log($"DownloadSuccess {eventArgs.DownloadInfo.DownloadUrl}");
        }
        
        // 下载失败
        void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
        {
            LogF8.LogError($"DownloadFailure {eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
        }
        
        // 所有下载完成
        void OnDownloadFinish(DownloadTasksCompletedEventArgs eventArgs)
        {
            LogF8.Log($"DownloadFinish {eventArgs.TimeSpan}");
        }
    }
}
