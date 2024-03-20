# F8 Download

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Download组件，支持localhost与http地址文件的下载，可**本地写入、监听下载进度、断点续传**，支持动态添加、移除、暂停、恢复下载。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
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
                downloader.AddDownload(fileInfo, Application.persistentDataPath + "F8Download/download" + count + ".png");
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
```


