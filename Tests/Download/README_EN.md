# F8 Download

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Download Component**  
Supports file downloads from both localhost and HTTP sources with:
* Local file writing
* Download progress monitoring
* Resumable transfers
* Dynamic management (add/remove/pause/resume downloads)

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
private string[] fileInfos = new[]
{
    "https://raw.githubusercontent.com/TippingGame/F8Framework/refs/heads/main/Tests/Logo.png"
};

private Downloader downloader;

void Start()
{
    // Create downloader instance
    downloader = FF8.Download.CreateDownloader("Download", new Downloader());

    // Set timeout (default: no timeout)
    downloader.DownloadTimeout = 30; // seconds
    
    // Configure event handlers
    downloader.OnDownloadSuccess += OnDownloadSuccess;
    downloader.OnDownloadFailure += OnDownloadFailure;
    downloader.OnDownloadStart += OnDownloadStart;
    downloader.OnDownloadOverallProgress += OnDownloadOverall;
    downloader.OnAllDownloadTaskCompleted += OnDownloadFinish;
    
    int count = 0;
    // Add download tasks
    foreach (var fileInfo in fileInfos)
    {
        count++;
        downloader.AddDownload(
            fileInfo, 
            Application.persistentDataPath + "F8Download/download" + count + ".png"
        );
    }
    
    // Start download process
    downloader.LaunchDownload();
    
    // Get remote file size (Note: Some downloads may not report total size upfront)
    FF8.Download.GetUrlFilesSizeAsync("", size => LogF8.Log(size));
    
    // Cancel all downloads
    downloader.CancelDownload();
}

// Download started callback
void OnDownloadStart(DownloadStartEventArgs eventArgs)
{
    LogF8.Log(eventArgs.DownloadInfo.DownloadUrl);
}

// Progress update callback
void OnDownloadOverall(DownloadUpdateEventArgs eventArgs)
{
    // Note: Some downloads may not know total file size, limiting progress accuracy
    float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
    float taskCount = (float)eventArgs.DownloadTaskCount;

    // Calculate percentage progress
    float progress = currentTaskIndex / taskCount * 100f;
    // LogF8.Log(progress);
}

// Download success callback
void OnDownloadSuccess(DownloadSuccessEventArgs eventArgs)
{
    LogF8.Log($"DownloadSuccess {eventArgs.DownloadInfo.DownloadUrl}");
}

// Download failure callback
void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
{
    LogF8.LogError($"DownloadFailure {eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
}

// All downloads completed
void OnDownloadFinish(DownloadTasksCompletedEventArgs eventArgs)
{
    LogF8.Log($"DownloadFinish - Total duration: {eventArgs.TimeSpan}");
}
```


