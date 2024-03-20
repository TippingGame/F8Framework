using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    public class HotUpdateVersion : ModuleSingleton<HotUpdateVersion>, IModule
    {
        public static string PackageSplit = "Package_";
        public static string HotUpdateDirName = "/HotUpdate";
        public static string PackageDirName = "/Package";
        
        private Downloader hotUpdateDownloader;
        
        private Downloader PackageDownloader;
        
        public void OnInit(object createParam)
        {
            
        }
        
        // 初始化本地版本
        public void InitLocalVersion()
        {
            GameVersion gameVersion = Util.LitJson.ToObject<GameVersion>(Resources.Load<TextAsset>("GameVersion").ToString());
            GameConfig.LocalGameVersion = gameVersion;
        }
        
        // 初始化远程版本
        public IEnumerator InitRemoteVersion()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(GameConfig.LocalGameVersion.AssetRemoteAddress + "/GameVersion.json");
            yield return webRequest.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
            {
                LogF8.LogError($"获取游戏远程版本失败：{GameConfig.LocalGameVersion.AssetRemoteAddress} ，错误：{webRequest.error}");
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                GameVersion gameVersion = Util.LitJson.ToObject<GameVersion>(text);
                GameConfig.RemoteGameVersion = gameVersion;
            }
            webRequest.Dispose();
            webRequest = null;
        }

        public void CheckHotUpdate(Action completed = null, Action failure = null, Action<float> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate)
            {
                return;
            }
            // 创建热更下载器
            hotUpdateDownloader = DownloadManager.Instance.CreateDownloader("hotUpdateDownloader", new Downloader());
            
            // 设置热更下载器回调
            hotUpdateDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                LogF8.Log($"获取热更资源完成：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadFailure += (eventArgs) =>
            {
                LogF8.LogError($"获取热更资源失败：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
                failure?.Invoke();
            };
            hotUpdateDownloader.OnDownloadStart += (eventArgs) =>
            {
                LogF8.Log($"开始获取热更资源：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
                float taskCount = (float)eventArgs.DownloadTaskCount;

                // 计算进度百分比
                float progress = currentTaskIndex / taskCount * 100f;
                // LogF8.Log(progress);
                overallProgress?.Invoke(progress);
            };
            hotUpdateDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
                LogF8.Log($"所有热更资源获取完成：{eventArgs.TimeSpan}");
                completed?.Invoke();
            };
            
            string[] fileInfos = new[]
            {
                GameConfig.LocalGameVersion.HotUpdateURL + "/Package.zip"
            };
            // 添加下载清单
            foreach (var fileInfo in fileInfos)
            {
                hotUpdateDownloader.AddDownload(fileInfo, Application.persistentDataPath + "/Package233.zip");
            }
            
            // 下载器开始下载
            hotUpdateDownloader.LaunchDownload();
        }
        
        public void CheckPackageUpdate(Action completed = null, Action failure = null, Action<float> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnablePackage)
            {
                return;
            }
            // 创建分包下载器
            PackageDownloader = DownloadManager.Instance.CreateDownloader("hotUpdateDownloader", new Downloader());
            
            // 设置分包下载器回调
            PackageDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                LogF8.Log($"获取分包资源完成：{eventArgs.DownloadInfo.DownloadUrl}");
#if UNITY_WEBGL
                // 使用协程
				Util.Unity.StartCoroutine(Util.ZipHelper.UnZipFileCoroutine(eventArgs.DownloadInfo.DownloadPath,
                    Application.persistentDataPath, PackageDirName, true));
#else
                // 使用多线程
                _ = Util.ZipHelper.UnZipFileAsync(eventArgs.DownloadInfo.DownloadPath,
                    Application.persistentDataPath, PackageDirName, true);
#endif
            };
            PackageDownloader.OnDownloadFailure += (eventArgs) =>
            {
                LogF8.LogError($"获取分包资源失败：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
                failure?.Invoke();
            };
            PackageDownloader.OnDownloadStart += (eventArgs) =>
            {
                LogF8.Log($"开始获分包资源：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            PackageDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
                float taskCount = (float)eventArgs.DownloadTaskCount;

                // 计算进度百分比
                float progress = currentTaskIndex / taskCount * 100f;
                // LogF8.Log(progress);
                overallProgress?.Invoke(progress);
            };
            PackageDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
                LogF8.Log($"所有分包资源获取完成，用时：{eventArgs.TimeSpan}");
                completed?.Invoke();
            };
        
            string[] fileInfos = new[]
            {
                GameConfig.LocalGameVersion.PackageURL + PackageDirName + ".zip"
            };
            // 添加下载清单
            foreach (var fileInfo in fileInfos)
            {
                PackageDownloader.AddDownload(fileInfo, Application.persistentDataPath + PackageDirName + ".zip");
            }
            
            PackageDownloader.LaunchDownload();
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
            base.Destroy();
        }
    }
}
