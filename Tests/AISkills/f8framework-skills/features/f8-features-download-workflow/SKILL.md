---
name: f8-features-download-workflow
description: Use when implementing or troubleshooting Download feature workflows — HTTP file downloads, progress tracking, and resumable transfers in F8Framework.
---

# Download Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about downloading files from HTTP/localhost.
- The user needs progress tracking, resumable downloads, or batch downloads.
- Troubleshooting download failures or timeout issues.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. For usage docs, read: Assets/F8Framework/Tests/Download/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Download
- Test docs: Assets/F8Framework/Tests/Download

## Key classes and interfaces

| Class | Role |
|-------|------|
| `DownloadManager` | Core module. Access via `FF8.Download`. Creates and manages downloaders. |
| `Downloader` | Individual download task manager. Handles queue, callbacks, retry. |
| `DownloadStartEventArgs` | Event data for download start. |
| `DonwloadUpdateEventArgs` | Event data for progress updates (Note: naming typo in framework). |
| `DownloadSuccessEventArgs` | Event data for successful download. |
| `DownloadFailureEventArgs` | Event data for failed download. |
| `DownloadTasksCompletedEventArgs` | Event data when all tasks complete. |

## API quick reference

```csharp
// Create a downloader instance
Downloader downloader = FF8.Download.CreateDownloader("Download", new Downloader());
downloader.DownloadTimeout = 30;  // Timeout in seconds (default: no timeout)

// Set callbacks
downloader.OnDownloadStart += (DownloadStartEventArgs e) => { };
downloader.OnDownloadSuccess += (DownloadSuccessEventArgs e) => { };
downloader.OnDownloadFailure += (DownloadFailureEventArgs e) => { };
downloader.OnDownloadOverallProgress += (DonwloadUpdateEventArgs e) =>
{
    float progress = (float)e.CurrentDownloadTaskIndex / e.DownloadTaskCount * 100f;
    ulong downloadedBytes = e.DownloadInfo.DownloadedLength;
    double speed = downloadedBytes / e.DownloadInfo.DownloadTimeSpan.TotalSeconds;
    double downloadedMB = downloadedBytes / (1024.0 * 1024.0);
    double speedMB = speed / (1024.0 * 1024.0);
    LogF8.Log($"Progress: {progress:F1}%, {downloadedMB:F2}MB, Speed: {speedMB:F2}MB/s");
};
downloader.OnAllDownloadTaskCompleted += (DownloadTasksCompletedEventArgs e) => { };

// Add download tasks
downloader.AddDownload(url, Application.persistentDataPath + "/file.png");

// Start downloading
downloader.LaunchDownload();

// Cancel download
downloader.CancelDownload();

// Get remote file size
FF8.Download.GetUrlFilesSizeAsync(url, (long size) => LogF8.Log(size));
```

## Workflow

1. Create a downloader with `FF8.Download.CreateDownloader()`.
2. Configure timeout if needed.
3. Register all callbacks before adding tasks.
4. Add download items with source URL and local save path.
5. Call `LaunchDownload()` to begin.
6. Monitor progress via `OnDownloadOverallProgress`.
7. Handle success/failure per file.
8. Cancel with `CancelDownload()` if needed.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Download timeout | Server slow or unreachable | Increase `DownloadTimeout` or check network |
| File write fails | Invalid save path or no permission | Ensure path is in `Application.persistentDataPath` |
| Progress shows 0% | Some servers don't provide Content-Length | Use task index ratio instead of byte-based progress |
| Resume not working | Server doesn't support Range headers | Check server configuration |

## Cross-module dependencies

- **HotUpdateManager**: Uses Download for hot update asset retrieval.
- **AssetManager**: Downloaded assets may need to be loaded via AssetManager.

## Output checklist

- Downloader created with appropriate callbacks.
- Download paths verified.
- Files changed and why.
- Validation status and remaining risks.
