using System;
using F8Framework.Core;
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
        
        void Start()
        {
            // 设置下载器回调
            FF8.Download.OnDownloadSuccess += OnDownloadSucess;
            FF8.Download.OnDownloadFailure += OnDownloadFailure;
            FF8.Download.OnDownloadStart += OnDownloadStart;
            FF8.Download.OnDownloadOverallProgress += OnDownloadOverall;
            FF8.Download.OnAllDownloadTaskCompleted += OnDownloadFinish;
            
            int count = 0;
            // 添加下载清单
            foreach (var fileInfo in fileInfos)
            {
                count += 1;
                FF8.Download.AddDownload(fileInfo, Application.persistentDataPath + "/Download" + count + ".png");
            }
            
            // 下载器开始下载
            FF8.Download.LaunchDownload();
        }
        
        // 开始下载
        void OnDownloadStart(DownloadStartEventArgs eventArgs)
        {
            LogF8.Log(eventArgs.DownloadInfo.DownloadUrl);
        }
        
        void OnDownloadOverall(DonwloadUpdateEventArgs eventArgs)
        {
            //部分下载任务本身仅知道下载连接，无法获取需要下载的二进制长度，因此无法获取总下载长度。所以，无法使用 【已经下载的长度 / 需要下载的长度】计算公式。
            var progress = eventArgs.CurrentDownloadTaskIndex / (float)eventArgs.DownloadTaskCount;
            var overallProgress = (float)Math.Round(progress, 1) * 100;
            LogF8.Log(overallProgress);
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
