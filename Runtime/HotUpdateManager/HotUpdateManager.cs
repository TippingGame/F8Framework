using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    public class HotUpdateManager : ModuleSingleton<HotUpdateManager>, IModule
    {
        [Serializable]
        public class DownloadTaskInfo
        {
            public string Name;
            public string LocalPath;
            public string RemoteUrl;
            public string ExpectedVersion;
            public string ExpectedMd5;
            public long ExpectedSize;
            public long DownloadOffset;
            public bool Append;
        }

        [Serializable]
        private class PartialDownloadState
        {
            public string Version;
            public string Md5;
            public long Size;
        }

        public static string Separator = "_";
        public static string PackageSplit = "Package" + Separator;
        public static string RemoteDirName = "/Remote/" + URLSetting.GetPlatformName();
        public static string HotUpdateDirName = "/HotUpdate";
        public static string PackageDirName = "/Package";
        private const string PartialDownloadStateExtension = ".downloadstate";
        private Downloader hotUpdateDownloader;
        
        private Downloader packageDownloader;
        
        public void OnInit(object createParam)
        {
            LogF8.Log($"读取默认版本配置，来源：Resources/{nameof(GameVersion)}");
            GameVersion gameVersion = Util.LitJson.ToObject<GameVersion>(F8JsonEncryption.ReadJsonFromTextAsset(Resources.Load<TextAsset>(nameof(GameVersion))));
            GameConfig.LocalGameVersion = gameVersion;
        }
        
        // 初始化本地版本
        public void InitLocalVersion()
        {
            string localGameVersionPath = Application.persistentDataPath + "/" + nameof(GameVersion) + ".json";
            
            if (File.Exists(localGameVersionPath))
            {
                LogF8.Log($"读取本地版本配置，来源：{localGameVersionPath}");
                string json = F8JsonEncryption.ReadJsonFromFile(localGameVersionPath);
                GameConfig.LocalGameVersion = Util.LitJson.ToObject<GameVersion>(json);
            }
            else
            {
                LogF8.Log($"本地版本配置不存在，写入默认版本配置，目标：{localGameVersionPath}");
                F8JsonEncryption.WriteJsonToFile(localGameVersionPath,
                    Util.LitJson.ToJson(GameConfig.LocalGameVersion));
            }

            LogF8.Log($"初始化本地版本成功：Version={GameConfig.LocalGameVersion.Version}，EnableHotUpdate={GameConfig.LocalGameVersion.EnableHotUpdate}，EnablePackage={GameConfig.LocalGameVersion.EnablePackage}");
        }
        
        // 初始化远程版本
        public IEnumerator InitRemoteVersion()
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate && !GameConfig.LocalGameVersion.EnablePackage)
            {
                yield break;
            }
            
            string path = GameConfig.CombineAssetRemoteUrl(nameof(GameVersion) + ".json");
            if (path.IsNullOrEmpty())
            {
                LogF8.LogError("获取游戏远程版本失败：未配置远程地址 AssetRemoteAddress");
                yield break;
            }
            LogF8.Log($"请求远程版本配置，来源：{path}");
            
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                LogF8.LogError($"获取游戏远程版本失败：{path} ，错误：{webRequest.error}");
            }
            else
            {
                string text = F8JsonEncryption.DecryptJsonIfNeeded(webRequest.downloadHandler.text);
                GameVersion gameVersion = Util.LitJson.ToObject<GameVersion>(text);
                GameConfig.RemoteGameVersion = gameVersion;
                LogF8.Log($"读取远程版本配置成功，来源：{path}，Version={GameConfig.RemoteGameVersion.Version}，Address={GameConfig.RemoteGameVersion.AssetRemoteAddress}");
            }
            webRequest.Dispose();
            webRequest = null;
        }

        // 初始化热更资产配置
        public IEnumerator InitAssetVersion()
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate && !GameConfig.LocalGameVersion.EnablePackage)
            {
                yield break;
            }

            string path = GameConfig.CombineAssetRemoteUrl(HotUpdateDirName + Separator + nameof(AssetBundleMap) + ".json");
            if (path.IsNullOrEmpty())
            {
                LogF8.LogError("请求热更资产配置失败：未配置远程地址 AssetRemoteAddress");
                yield break;
            }
            LogF8.Log($"请求热更资产配置，来源：{path}");
            
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                LogF8.LogError($"请求热更资产配置失败，来源：{path}，错误：{webRequest.error}");
            }
            else
            {
                string text = F8JsonEncryption.DecryptJsonIfNeeded(webRequest.downloadHandler.text);
                Dictionary<string, AssetBundleMap.AssetMapping> assetBundleMap = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(text) ?? new Dictionary<string, AssetBundleMap.AssetMapping>();
                GameConfig.RemoteAssetBundleMap = assetBundleMap;
                int assetCount = GameConfig.RemoteAssetBundleMap.Count(mapping => mapping.Value != null && !mapping.Value.AbName.IsNullOrEmpty());
                LogF8.Log($"读取热更资产配置成功：资源总数={assetCount}");
            }
            webRequest.Dispose();
            webRequest = null;
            
            string localAssetBundleMapPath = Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json";
            if (File.Exists(localAssetBundleMapPath))
            {
                LogF8.Log($"读取本地资产配置，来源：{localAssetBundleMapPath}");
                string json = F8JsonEncryption.ReadJsonFromFile(localAssetBundleMapPath);
                AssetBundleMap.Mappings = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(json);
            }
        }

        private static Dictionary<string, AssetBundleMap.AssetMapping> MergeAssetBundleMappings(
            Dictionary<string, AssetBundleMap.AssetMapping> baseMappings,
            Dictionary<string, AssetBundleMap.AssetMapping> deltaMappings)
        {
            Dictionary<string, AssetBundleMap.AssetMapping> mergedMappings = baseMappings != null
                ? new Dictionary<string, AssetBundleMap.AssetMapping>(baseMappings)
                : new Dictionary<string, AssetBundleMap.AssetMapping>();

            if (deltaMappings == null)
            {
                return mergedMappings;
            }

            foreach (var mapping in deltaMappings)
            {
                mergedMappings[mapping.Key] = mapping.Value;
            }

            return mergedMappings;
        }
        
        // 游戏修复，资源清理
        public void GameRepairAssetClean()
        {
            FileTools.SafeClearDir(Application.persistentDataPath + HotUpdateDirName);
            FileTools.SafeClearDir(Application.persistentDataPath + PackageDirName);
            FileTools.SafeDeleteFile(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json");
            FileTools.SafeDeleteFile(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json");
        }
        
        /// <summary>
        /// 检查需要热更的资源
        /// </summary>
        /// <returns></returns>
        public (List<DownloadTaskInfo> DownloadInfos, long AllSize) CheckHotUpdate()
        {
            long allSize = 0;
            List<DownloadTaskInfo> downloadInfos = new List<DownloadTaskInfo>();
            if (!GameConfig.LocalGameVersion.EnableHotUpdate) // 启用热更
            {
                return (downloadInfos, allSize);
            }

            if (GameConfig.RemoteAssetBundleMap.Count <= 0) // 热更资产Map数量
            {
                return (downloadInfos, allSize);
            }
            
            int result = GameConfig.CompareVersions(GameConfig.LocalGameVersion.Version,
                GameConfig.RemoteGameVersion.Version);
            if (result >= 0) // 此版本无需热更新
            {
                return (downloadInfos, allSize);
            }
            
            var resAssetBundleMappings = GameConfig.RemoteAssetBundleMap;
            
            var assetBundleMappings = AssetBundleMap.Mappings;
            
            foreach (var resAssetMapping in resAssetBundleMappings)
            {
                assetBundleMappings.TryGetValue(resAssetMapping.Key, out AssetBundleMap.AssetMapping assetMapping);
                
                if ((assetMapping == null || resAssetMapping.Value.MD5 != assetMapping.MD5) // 新增资源，MD5不同则需更新
                    && !resAssetMapping.Value.AbName.IsNullOrEmpty() && !resAssetMapping.Value.MD5.IsNullOrEmpty())
                {
                    string abPath = resAssetMapping.Value.Version + "/" + URLSetting.AssetBundlesName + "/" +
                                    URLSetting.GetPlatformName() + "/" + resAssetMapping.Value.AbName;
                    string remoteUrl = GameConfig.CombineAssetRemoteUrl(HotUpdateDirName + Separator + abPath);
                    if (remoteUrl.IsNullOrEmpty())
                    {
                        LogF8.LogError("检查热更资源失败：未配置远程地址 AssetRemoteAddress");
                        return (downloadInfos, allSize);
                    }

                    int index = abPath.IndexOf('/');
                    string relativePath = abPath.Substring(index + 1);
                    string localPath = Application.persistentDataPath + HotUpdateDirName + "/" + relativePath;

                    long expectedSize = 0;
                    long.TryParse(resAssetMapping.Value.Size, out expectedSize);

                    long downloadOffset = 0;
                    bool append = false;
                    if (File.Exists(localPath))
                    {
                        long localSize = new FileInfo(localPath).Length;
                        PartialDownloadState partialDownloadState = ReadPartialDownloadState(localPath);
                        bool canResume = IsSamePartialDownload(partialDownloadState, resAssetMapping.Value, expectedSize);

                        if (expectedSize > 0 && localSize >= expectedSize)
                        {
                            FileTools.SafeDeleteFile(localPath);
                            DeletePartialDownloadState(localPath);
                        }
                        else if (canResume)
                        {
                            downloadOffset = localSize;
                            append = downloadOffset > 0;
                        }
                        else
                        {
                            FileTools.SafeDeleteFile(localPath);
                            DeletePartialDownloadState(localPath);
                        }
                    }

                    long remainingSize = expectedSize > 0 ? Math.Max(0, expectedSize - downloadOffset) : 0;
                    allSize += remainingSize;

                    downloadInfos.Add(new DownloadTaskInfo
                    {
                        Name = resAssetMapping.Key,
                        LocalPath = localPath,
                        RemoteUrl = remoteUrl,
                        ExpectedVersion = resAssetMapping.Value.Version,
                        ExpectedMd5 = resAssetMapping.Value.MD5,
                        ExpectedSize = expectedSize,
                        DownloadOffset = downloadOffset,
                        Append = append
                    });
                }
            }
            
            return (downloadInfos, allSize);
        }
        
        /// <summary>
        /// 开始热更新包下载
        /// </summary>
        /// <param name="downloadInfos"></param>
        /// <param name="completed"></param>
        /// <param name="failure"></param>
        /// <param name="overallProgress"></param>
        public void StartHotUpdate(List<DownloadTaskInfo> downloadInfos, Action completed = null, Action failure = null, Action<DonwloadUpdateEventArgs> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate || downloadInfos == null || downloadInfos.Count <= 0 || F8GamePrefs.GetBool(nameof(F8GameConfig.ForceRemoteAssetBundle)))
            {
                WriteVersion();
                completed?.Invoke();
                return;
            }

            hotUpdateDownloader ??= DownloadManager.Instance.CreateDownloader("hotUpdateDownloader", new Downloader());
            hotUpdateDownloader.Release();
            bool networkFailure = false;
            
            // 设置热更下载器回调
            hotUpdateDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                DeletePartialDownloadState(eventArgs.DownloadInfo.DownloadPath);
                LogF8.LogVersion($"获取热更资源完成！：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadFailure += (eventArgs) =>
            {
                networkFailure = true;
                LogF8.LogError($"获取热更资源失败。：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
            };
            hotUpdateDownloader.OnDownloadStart += (eventArgs) =>
            {
                LogF8.LogVersion($"开始获取热更资源...：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                overallProgress?.Invoke(eventArgs);
            };
            hotUpdateDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
                if (networkFailure || eventArgs.FailedInfos.Length > 0)
                {
                    failure?.Invoke();
                    return;
                }
                var invalidDownloadInfos = CheckMD5(downloadInfos);
                if (invalidDownloadInfos != null)
                {
                    LogF8.LogError(string.Join("\n", invalidDownloadInfos.Select(info => info.LocalPath + " MD5校验失败！")));
                    failure?.Invoke();
                    return;
                }
                LogF8.LogVersion($"所有热更资源获取完成！，用时：{eventArgs.TimeSpan}");
                WriteVersion();
                completed?.Invoke();
            };

            foreach (var downloadInfo in downloadInfos)
            {
                if (downloadInfo == null || downloadInfo.LocalPath.IsNullOrEmpty() || downloadInfo.RemoteUrl.IsNullOrEmpty())
                {
                    continue;
                }

                WritePartialDownloadState(downloadInfo);
                hotUpdateDownloader.AddDownload(downloadInfo.RemoteUrl, downloadInfo.LocalPath, downloadInfo.DownloadOffset, downloadInfo.Append);
            }

            if (hotUpdateDownloader.DownloadingCount <= 0)
            {
                WriteVersion();
                completed?.Invoke();
                return;
            }

            void WriteVersion()
            {
                if (!GameConfig.RemoteGameVersion.Version.IsNullOrEmpty())
                {
                    GameConfig.LocalGameVersion.Version = GameConfig.RemoteGameVersion.Version;
                    GameConfig.LocalGameVersion.HotUpdateVersion = new List<string>();
                    string localGameVersionPath = Application.persistentDataPath + "/" + nameof(GameVersion) + ".json";
                    LogF8.Log($"写入热更后的本地版本配置，目标：{localGameVersionPath}");
                    F8JsonEncryption.WriteJsonToFile(localGameVersionPath,
                        Util.LitJson.ToJson(GameConfig.LocalGameVersion));
                }
                
                if (GameConfig.RemoteAssetBundleMap.Count > 0)
                {
                    AssetBundleMap.Mappings = MergeAssetBundleMappings(AssetBundleMap.Mappings, GameConfig.RemoteAssetBundleMap);
                    string localAssetBundleMapPath = Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json";
                    LogF8.Log($"写入合并后的本地资产配置，目标：{localAssetBundleMapPath}");
                    F8JsonEncryption.WriteJsonToFile(localAssetBundleMapPath,
                        Util.LitJson.ToJson(AssetBundleMap.Mappings));
                }
            }
            // 下载器开始下载
            hotUpdateDownloader.LaunchDownload();
        }
        
        // 检查需要下载的分包
        public (List<DownloadTaskInfo> DownloadInfos, long AllSize) CheckPackageUpdate(List<string> subPackages)
        {
            List<DownloadTaskInfo> downloadInfos = new List<DownloadTaskInfo>();
            long allSize = 0;
            if (!GameConfig.LocalGameVersion.EnablePackage || subPackages == null || subPackages.Count <= 0)
            {
                return (downloadInfos, allSize);
            }

            foreach (var package in subPackages)
            {
                if (!GameConfig.LocalGameVersion.SubPackage.Contains(package))
                {
                    continue;
                }

                string persistentPackagePath = Application.persistentDataPath + "/" + PackageSplit + package + ".zip";
                string remotePackageUrl = GameConfig.CombineAssetRemoteUrl(PackageSplit + package + ".zip");
                if (remotePackageUrl.IsNullOrEmpty())
                {
                    LogF8.LogError("检查分包资源失败：未配置远程地址 AssetRemoteAddress");
                    return (downloadInfos, allSize);
                }

                string expectedMd5 = null;
                long expectedSize = 0;
                TryGetPackageInfo(package, out expectedMd5, out expectedSize);

                long downloadOffset = 0;
                bool append = false;
                if (File.Exists(persistentPackagePath))
                {
                    long fileSizeInBytes = new FileInfo(persistentPackagePath).Length;
                    PartialDownloadState partialDownloadState = ReadPartialDownloadState(persistentPackagePath);
                    bool canResume = IsSamePartialDownload(partialDownloadState, null, expectedSize, expectedMd5);
                    if (expectedSize > 0)
                    {
                        if (fileSizeInBytes > expectedSize)
                        {
                            FileTools.SafeDeleteFile(persistentPackagePath);
                            DeletePartialDownloadState(persistentPackagePath);
                        }
                        else if (fileSizeInBytes < expectedSize && canResume)
                        {
                            downloadOffset = fileSizeInBytes;
                            append = downloadOffset > 0;
                        }
                        else if (fileSizeInBytes < expectedSize)
                        {
                            FileTools.SafeDeleteFile(persistentPackagePath);
                            DeletePartialDownloadState(persistentPackagePath);
                        }
                    }
                    else
                    {
                        if (canResume)
                        {
                            downloadOffset = fileSizeInBytes;
                            append = downloadOffset > 0;
                        }
                        else
                        {
                            FileTools.SafeDeleteFile(persistentPackagePath);
                            DeletePartialDownloadState(persistentPackagePath);
                        }
                    }
                }

                long remainingSize = expectedSize > 0 ? Math.Max(0, expectedSize - downloadOffset) : 0;
                allSize += remainingSize;

                downloadInfos.Add(new DownloadTaskInfo
                {
                    Name = package,
                    LocalPath = persistentPackagePath,
                    RemoteUrl = remotePackageUrl,
                    ExpectedMd5 = expectedMd5,
                    ExpectedSize = expectedSize,
                    DownloadOffset = downloadOffset,
                    Append = append
                });
            }

            return (downloadInfos, allSize);
        }

        /// <summary>
        /// 开始分包下载
        /// </summary>
        /// <param name="downloadInfos"></param>
        /// <param name="completed"></param>
        /// <param name="failure"></param>
        /// <param name="overallProgress"></param>
        public void StartPackageUpdate(List<DownloadTaskInfo> downloadInfos, Action completed = null, Action failure = null, Action<DonwloadUpdateEventArgs> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnablePackage || downloadInfos == null || downloadInfos.Count <= 0 || F8GamePrefs.GetBool(nameof(F8GameConfig.ForceRemoteAssetBundle)))
            {
                completed?.Invoke();
                return;
            }

            packageDownloader ??= DownloadManager.Instance.CreateDownloader("packageDownloader", new Downloader());
            packageDownloader.Release();
            bool networkFailure = false;
            List<string> downloadPaths = new List<string>(downloadInfos.Count);
            
            // 设置分包下载器回调
            packageDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                DeletePartialDownloadState(eventArgs.DownloadInfo.DownloadPath);
                LogF8.LogVersion($"获取分包资源完成！：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            packageDownloader.OnDownloadFailure += (eventArgs) =>
            {
                networkFailure = true;
                LogF8.LogError($"获取分包资源失败。：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
            };
            packageDownloader.OnDownloadStart += (eventArgs) =>
            {
                LogF8.LogVersion($"开始获取分包资源...：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            packageDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                overallProgress?.Invoke(eventArgs);
            };
            packageDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
                if (networkFailure || eventArgs.FailedInfos.Length > 0)
                {
                    failure?.Invoke();
                    return;
                }
                var invalidDownloadInfos = CheckMD5(downloadInfos);
                if (invalidDownloadInfos != null)
                {
                    LogF8.LogError(string.Join("\n", invalidDownloadInfos.Select(info => "MD5校验失败！" + info.LocalPath)));
                    failure?.Invoke();
                    return;
                }
                LogF8.LogVersion($"所有分包资源获取完成！，用时：{eventArgs.TimeSpan}");
#if UNITY_WEBGL
                // 使用协程
				Util.Unity.StartCoroutine(UnZipPackagePathsCo(downloadPaths, completed));
#else
                // 使用多线程
                _ = UnZipPackagePaths(downloadPaths, completed);
#endif
            };
            
            // 添加下载清单
            foreach (var downloadInfo in downloadInfos)
            {
                if (downloadInfo == null || downloadInfo.LocalPath.IsNullOrEmpty() || downloadInfo.RemoteUrl.IsNullOrEmpty())
                {
                    continue;
                }

                string persistentPackagePath = downloadInfo.LocalPath;
                downloadPaths.Add(persistentPackagePath);

                WritePartialDownloadState(downloadInfo);
                packageDownloader.AddDownload(downloadInfo.RemoteUrl, persistentPackagePath, downloadInfo.DownloadOffset, downloadInfo.Append);
            }

            if (packageDownloader.DownloadingCount <= 0)
            {
#if UNITY_WEBGL
                Util.Unity.StartCoroutine(UnZipPackagePathsCo(downloadPaths, completed));
#else
                _ = UnZipPackagePaths(downloadPaths, completed);
#endif
                return;
            }

            packageDownloader.LaunchDownload();
        }

        private List<DownloadTaskInfo> CheckMD5(List<DownloadTaskInfo> downloadInfos)
        {
            if (downloadInfos == null || downloadInfos.Count <= 0)
            {
                return null;
            }

            List<DownloadTaskInfo> invalidDownloadInfos = null;
            foreach (var downloadInfo in downloadInfos)
            {
                if (downloadInfo == null || downloadInfo.LocalPath.IsNullOrEmpty() || downloadInfo.ExpectedMd5.IsNullOrEmpty())
                {
                    continue;
                }

                if (!TryValidateFileMd5(downloadInfo.LocalPath, downloadInfo.ExpectedMd5))
                {
                    invalidDownloadInfos ??= new List<DownloadTaskInfo>();
                    invalidDownloadInfos.Add(downloadInfo);
                }
            }

            return invalidDownloadInfos;
        }

        private string GetPartialDownloadStatePath(string localPath)
        {
            return localPath + PartialDownloadStateExtension;
        }

        private PartialDownloadState ReadPartialDownloadState(string localPath)
        {
            string statePath = GetPartialDownloadStatePath(localPath);
            if (!File.Exists(statePath))
            {
                return null;
            }

            string json = FileTools.SafeReadAllText(statePath);
            if (json.IsNullOrEmpty())
            {
                return null;
            }

            try
            {
                return Util.LitJson.ToObject<PartialDownloadState>(json);
            }
            catch (Exception e)
            {
                LogF8.LogWarning($"读取热更断点状态失败，已忽略：{statePath}\n{e}");
                return null;
            }
        }

        private bool IsSamePartialDownload(PartialDownloadState partialDownloadState, AssetBundleMap.AssetMapping remoteAssetMapping, long expectedSize, string expectedMd5 = null)
        {
            if (partialDownloadState == null)
            {
                return false;
            }

            string targetVersion = remoteAssetMapping?.Version;
            string targetMd5 = expectedMd5 ?? remoteAssetMapping?.MD5;
            return string.Equals(partialDownloadState.Version, targetVersion, StringComparison.Ordinal) &&
                   string.Equals(partialDownloadState.Md5, targetMd5, StringComparison.OrdinalIgnoreCase) &&
                   partialDownloadState.Size == expectedSize;
        }

        private void WritePartialDownloadState(DownloadTaskInfo downloadInfo)
        {
            if (downloadInfo == null || downloadInfo.LocalPath.IsNullOrEmpty())
            {
                return;
            }

            string statePath = GetPartialDownloadStatePath(downloadInfo.LocalPath);
            PartialDownloadState partialDownloadState = new PartialDownloadState
            {
                Version = downloadInfo.ExpectedVersion,
                Md5 = downloadInfo.ExpectedMd5,
                Size = downloadInfo.ExpectedSize
            };

            FileTools.SafeWriteAllText(statePath, Util.LitJson.ToJson(partialDownloadState));
        }

        private void DeletePartialDownloadState(string localPath)
        {
            if (localPath.IsNullOrEmpty())
            {
                return;
            }

            FileTools.SafeDeleteFile(GetPartialDownloadStatePath(localPath));
        }

        private bool TryGetPackageInfo(string package, out string expectedMd5, out long expectedSize)
        {
            expectedMd5 = null;
            expectedSize = 0;

            Dictionary<string, (long size, string md5)> packageInfoDict = GameConfig.RemoteGameVersion.SubPackageInfo;
            if (packageInfoDict == null || !packageInfoDict.TryGetValue(package, out (long size, string md5) packageInfo))
            {
                return false;
            }

            expectedMd5 = packageInfo.md5;
            expectedSize = packageInfo.size;

            return true;
        }

        private bool TryValidateFileMd5(string filePath, string expectedMd5)
        {
            if (expectedMd5.IsNullOrEmpty() || !File.Exists(filePath))
            {
                return false;
            }

            string fileMd5 = FileTools.CreateMd5ForFile(filePath);
            return string.Equals(fileMd5, expectedMd5, StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerator UnZipPackagePathsCo(List<string> downloadPaths, Action completed = null)
        {
            string optionalPackagePassword = F8GamePrefs.GetString(nameof(F8GameConfig.OptionalPackagePassword), "");
            foreach (var downloadPath in downloadPaths)
            {
                yield return Util.ZipHelper.UnZipFileCoroutine(downloadPath,
                    Application.persistentDataPath, optionalPackagePassword, true);
                string package = Path.GetFileNameWithoutExtension(downloadPath).Replace(PackageSplit, "");
                int subPackageCount = GameConfig.LocalGameVersion.SubPackage.Count;
                for (int i = subPackageCount - 1; i >= 0; i--)
                {
                    if (GameConfig.LocalGameVersion.SubPackage[i] == package)
                    {
                        GameConfig.LocalGameVersion.SubPackage.RemoveAt(i);
                    }
                }
                string localGameVersionPath = Application.persistentDataPath + "/" + nameof(GameVersion) + ".json";
                LogF8.Log($"写入分包更新后的本地版本配置，目标：{localGameVersionPath}");
                F8JsonEncryption.WriteJsonToFile(localGameVersionPath,
                    Util.LitJson.ToJson(GameConfig.LocalGameVersion));
            }
            
            completed?.Invoke();
        }
        
        public async Task UnZipPackagePaths(List<string> downloadPaths, Action completed = null)
        {
            string optionalPackagePassword = F8GamePrefs.GetString(nameof(F8GameConfig.OptionalPackagePassword), "");
            var tasks = downloadPaths.Select(async path =>
            {
                await Util.ZipHelper.UnZipFileAsync(path, Application.persistentDataPath, optionalPackagePassword, true);
                return Path.GetFileNameWithoutExtension(path).Replace(PackageSplit, "");
            }).ToList();
            
            string[] packages = await Task.WhenAll(tasks);
            
            for (int i = GameConfig.LocalGameVersion.SubPackage.Count - 1; i >= 0; i--)
            {
                string subPkg = GameConfig.LocalGameVersion.SubPackage[i];
                if (packages.Contains(subPkg))
                {
                    GameConfig.LocalGameVersion.SubPackage.RemoveAt(i);
                }
            }
            
            string localGameVersionPath = Application.persistentDataPath + "/" + nameof(GameVersion) + ".json";
            LogF8.Log($"写入分包更新后的本地版本配置，目标：{localGameVersionPath}");
            F8JsonEncryption.WriteJsonToFile(localGameVersionPath,
                Util.LitJson.ToJson(GameConfig.LocalGameVersion));

            completed?.Invoke();
        }
        
        public static string GetAssetBundlesPath(string fullPath)
        {
            Regex rgx = new Regex(@"AssetBundles[\\/].+$");
            Match matches = rgx.Match(fullPath);

            string assetPath = "";
            if (matches.Success)
                assetPath = matches.Value;

            assetPath = FileTools.FormatToUnityPath(assetPath);
            return assetPath;
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
            hotUpdateDownloader.CancelDownload();
            hotUpdateDownloader = null;
            packageDownloader.CancelDownload();
            packageDownloader = null;
            base.Destroy();
        }
    }
}
