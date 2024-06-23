using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    public class HotUpdateManager : ModuleSingleton<HotUpdateManager>, IModule
    {
        public static string Separator = "_";
        public static string PackageSplit = "Package" + Separator;
        public static string RemoteDirName = "/Remote";
        public static string HotUpdateDirName = "/HotUpdate";
        public static string PackageDirName = "/Package";

        private Downloader hotUpdateDownloader;
        
        private Downloader packageDownloader;
        
        public void OnInit(object createParam)
        {
            GameVersion gameVersion = Util.LitJson.ToObject<GameVersion>(Resources.Load<TextAsset>(nameof(GameVersion)).ToString());
            GameConfig.LocalGameVersion = gameVersion;
        }
        
        // 初始化本地版本
        public void InitLocalVersion()
        {
            if (File.Exists(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json"))
            {
                string json =
                    FileTools.SafeReadAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json");
                GameConfig.LocalGameVersion = Util.LitJson.ToObject<GameVersion>(json);
            }
            else
            {
                FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
                    Util.LitJson.ToJson(GameConfig.LocalGameVersion));
            }
        }
        
        // 初始化远程版本
        public IEnumerator InitRemoteVersion()
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate && !GameConfig.LocalGameVersion.EnablePackage)
            {
                yield break;
            }
            
            string path = GameConfig.LocalGameVersion.AssetRemoteAddress + "/" + nameof(GameVersion) + ".json";
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            yield return webRequest.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
            {
                LogF8.LogError($"获取游戏远程版本失败：{path} ，错误：{webRequest.error}");
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

        // 初始化资源版本
        public IEnumerator InitAssetVersion()
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate && !GameConfig.LocalGameVersion.EnablePackage)
            {
                yield break;
            }

            string path = GameConfig.LocalGameVersion.AssetRemoteAddress + "/HotUpdate" + Separator + nameof(AssetBundleMap) + ".json";
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            yield return webRequest.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
            {
                LogF8.LogError($"获取游戏资源版本失败：{path} ，错误：{webRequest.error}");
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                Dictionary<string, AssetBundleMap.AssetMapping> assetBundleMap = Util.LitJson.ToObject<Dictionary<string, AssetBundleMap.AssetMapping>>(text);
                GameConfig.RemoteAssetBundleMap = assetBundleMap;
            }
            webRequest.Dispose();
            webRequest = null;
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
        public Tuple<Dictionary<string, string>, long> CheckHotUpdate()
        {
            long allSize = 0;
            Dictionary<string, string> hotUpdateAssetUrl = new Dictionary<string, string>();
            if (!GameConfig.LocalGameVersion.EnableHotUpdate) // 启用热更
            {
                return Tuple.Create(hotUpdateAssetUrl, allSize);
            }

            if (GameConfig.RemoteAssetBundleMap.Count <= 0) // 热更资产Map数量
            {
                return Tuple.Create(hotUpdateAssetUrl, allSize);
            }
            
            int result = GameConfig.CompareVersions(GameConfig.LocalGameVersion.Version,
                GameConfig.RemoteGameVersion.Version);
            if (result >= 0) // 此版本无需热更新
            {
                return Tuple.Create(hotUpdateAssetUrl, allSize);
            }
            
            var resAssetBundleMappings = GameConfig.RemoteAssetBundleMap;
            
            var assetBundleMappings = AssetBundleMap.Mappings;
            
            foreach (var resAssetMapping in resAssetBundleMappings)
            {
                assetBundleMappings.TryGetValue(resAssetMapping.Key, out AssetBundleMap.AssetMapping assetMapping);
                
                if (assetMapping == null || resAssetMapping.Value.MD5 != assetMapping.MD5) // 新增资源，MD5不同则需更新
                {
                    string abPath = resAssetMapping.Value.Version + "/" + URLSetting.AssetBundlesName + "/" +
                                              URLSetting.GetPlatformName() + "/" + resAssetMapping.Value.AbName;
                    
                    string persistentAbPath = Application.persistentDataPath + HotUpdateDirName + Separator + abPath;
                    
                    // 校验本地热更资源文件md5
                    if (File.Exists(persistentAbPath) && FileTools.CreateMd5ForFile(persistentAbPath) == resAssetMapping.Value.MD5)
                    {
                        continue;
                    }
                    allSize += string.IsNullOrEmpty(resAssetMapping.Value.Size) ? 0 : long.Parse(resAssetMapping.Value.Size) ;
                    hotUpdateAssetUrl.TryAdd(resAssetMapping.Key, abPath);
                }
                else if(GameConfig.CompareVersions(assetMapping.Version, resAssetMapping.Value.Version) < 0) // 版本修正
                {
                    resAssetMapping.Value.Version = assetMapping.Version;
                }
            }
            
            return Tuple.Create(hotUpdateAssetUrl, allSize);
        }
        
        /// <summary>
        /// 开始热更新包下载
        /// </summary>
        /// <param name="hotUpdateAssetUrl"></param>
        /// <param name="completed"></param>
        /// <param name="failure"></param>
        /// <param name="overallProgress"></param>
        public void StartHotUpdate(Dictionary<string, string> hotUpdateAssetUrl, Action completed = null, Action failure = null, Action<float> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate)
            {
                completed?.Invoke();
                return;
            }
            
            if (hotUpdateAssetUrl.Count <= 0)
            {
                completed?.Invoke();
                return;
            }
            
            // 创建热更下载器
            hotUpdateDownloader = DownloadManager.Instance.CreateDownloader("hotUpdateDownloader", new Downloader());
            
            // 设置热更下载器回调
            hotUpdateDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                LogF8.LogVersion($"获取热更资源完成！：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadFailure += (eventArgs) =>
            {
                LogF8.LogError($"获取热更资源失败。：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
                failure?.Invoke();
            };
            hotUpdateDownloader.OnDownloadStart += (eventArgs) =>
            {
                LogF8.LogVersion($"开始获取热更资源...：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
                float taskCount = (float)eventArgs.DownloadTaskCount;

                // 计算进度百分比
                float progress = currentTaskIndex / taskCount * 100f;
                // LogF8.LogVersion(progress);
                overallProgress?.Invoke(progress);
            };
            hotUpdateDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
                LogF8.LogVersion($"所有热更资源获取完成！，用时：{eventArgs.TimeSpan}");
                GameConfig.LocalGameVersion.Version = GameConfig.RemoteGameVersion.Version;
                GameConfig.LocalGameVersion.HotUpdateVersion = new List<string>();
                FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
                    Util.LitJson.ToJson(GameConfig.LocalGameVersion));
                foreach (var asssetName in hotUpdateAssetUrl.Keys)
                {
                    if (GameConfig.RemoteAssetBundleMap.TryGetValue(asssetName, out AssetBundleMap.AssetMapping assetMapping))
                    {
                        if (assetMapping != null)
                        {
                            assetMapping.Updated = "1";
                        }
                    }
                }
                AssetBundleMap.Mappings = GameConfig.RemoteAssetBundleMap;
                FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json",
                    Util.LitJson.ToJson(AssetBundleMap.Mappings));
                
                completed?.Invoke();
            };
            
            // 添加下载清单
            foreach (var assetUrl in hotUpdateAssetUrl.Values)
            {
                int index = assetUrl.IndexOf('/');
                string result = assetUrl.Substring(index + 1);
                hotUpdateDownloader.AddDownload(GameConfig.LocalGameVersion.AssetRemoteAddress + HotUpdateDirName + Separator + assetUrl,
                    Application.persistentDataPath + "/HotUpdate/" + result);
            }
            
            // 下载器开始下载
            hotUpdateDownloader.LaunchDownload();
        }
        
        // 检查需要下载的分包
        public List<string> CheckPackageUpdate(List<string> subPackage)
        {
            List<string> temp = new List<string>();
            foreach (var package in subPackage)
            {
                if (GameConfig.LocalGameVersion.SubPackage.Contains(package))
                {
                    temp.Add(package);
                }
            }
            return temp;
        }
        
        /// <summary>
        /// 开始分包下载
        /// </summary>
        /// <param name="subPackages"></param>
        /// <param name="completed"></param>
        /// <param name="failure"></param>
        /// <param name="overallProgress"></param>
        public void StartPackageUpdate(List<string> subPackages, Action completed = null, Action failure = null, Action<float> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnablePackage)
            {
                completed?.Invoke();
                return;
            }

            if (subPackages.Count <= 0)
            {
                completed?.Invoke();
                return;
            }
            
            List<string> downloadPaths = new List<string>();
            
            // 创建分包下载器
            packageDownloader = DownloadManager.Instance.CreateDownloader("packageDownloader", new Downloader());
            
            // 设置分包下载器回调
            packageDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                LogF8.LogVersion($"获取分包资源完成！：{eventArgs.DownloadInfo.DownloadUrl}");
                downloadPaths.Add(eventArgs.DownloadInfo.DownloadPath);
            };
            packageDownloader.OnDownloadFailure += (eventArgs) =>
            {
                LogF8.LogError($"获取分包资源失败。：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
                failure?.Invoke();
            };
            packageDownloader.OnDownloadStart += (eventArgs) =>
            {
                LogF8.LogVersion($"开始获取分包资源...：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            packageDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
                float taskCount = (float)eventArgs.DownloadTaskCount;

                // 计算进度百分比
                float progress = currentTaskIndex / taskCount * 100f;
                // LogF8.LogVersion(progress);
                overallProgress?.Invoke(progress);
            };
            packageDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
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
            foreach (var package in subPackages)
            {
                string persistentPackagePath = Application.persistentDataPath + "/" + PackageSplit + package + ".zip";
                long fileSizeInBytes = 0;
                if (File.Exists(persistentPackagePath))
                {
                    FileInfo fileInfo = new FileInfo(persistentPackagePath);
                    fileSizeInBytes = fileInfo.Length;
                }
                // 断点续传
                packageDownloader.AddDownload(GameConfig.LocalGameVersion.AssetRemoteAddress + "/" + PackageSplit + package + ".zip",
                    persistentPackagePath, fileSizeInBytes, true);
            }
            
            packageDownloader.LaunchDownload();
        }
        
        public IEnumerator UnZipPackagePathsCo(List<string> downloadPaths, Action completed = null)
        {
            foreach (var downloadPath in downloadPaths)
            {
                // 使用协程
                yield return Util.ZipHelper.UnZipFileCoroutine(downloadPath,
                    Application.persistentDataPath, null, true);
                string package = Path.GetFileNameWithoutExtension(downloadPath).Replace(PackageSplit, "");
                int subPackageCount = GameConfig.LocalGameVersion.SubPackage.Count;
                for (int i = subPackageCount - 1; i >= 0; i--)
                {
                    if (GameConfig.LocalGameVersion.SubPackage[i] == package)
                    {
                        GameConfig.LocalGameVersion.SubPackage.RemoveAt(i);
                    }
                }
                FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
                    Util.LitJson.ToJson(GameConfig.LocalGameVersion));
            }
            
            completed?.Invoke();
        }
        
        public async Task UnZipPackagePaths(List<string> downloadPaths, Action completed = null)
        {
            foreach (var downloadPath in downloadPaths)
            {
                // 使用多线程
                await Util.ZipHelper.UnZipFileAsync(downloadPath, Application.persistentDataPath, null, true);
                string package = Path.GetFileNameWithoutExtension(downloadPath).Replace(PackageSplit, "");
                int subPackageCount = GameConfig.LocalGameVersion.SubPackage.Count;
                for (int i = subPackageCount - 1; i >= 0; i--)
                {
                    if (GameConfig.LocalGameVersion.SubPackage[i] == package)
                    {
                        GameConfig.LocalGameVersion.SubPackage.RemoveAt(i);
                    }
                }
                FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
                    Util.LitJson.ToJson(GameConfig.LocalGameVersion));
            }
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
