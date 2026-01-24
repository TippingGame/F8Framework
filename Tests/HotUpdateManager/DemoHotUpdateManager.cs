using System;
using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoHotUpdateManager : MonoBehaviour
    {
        IEnumerator Start()
        {
            // 启动必须要的模块，热更新版本管理-->使用了资产模块-->使用了下载模块
            FF8.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();
            FF8.Asset = ModuleCenter.CreateModule<AssetManager>();
            FF8.Download = ModuleCenter.CreateModule<DownloadManager>();
            
            // 初始化本地版本
            FF8.HotUpdate.InitLocalVersion();

            // 初始化远程版本
            yield return FF8.HotUpdate.InitRemoteVersion();
            
            // 初始化资源版本
            yield return FF8.HotUpdate.InitAssetVersion();
            
            // 检查需要热更的资源，总大小
            Tuple<Dictionary<string, string>, long> result  = FF8.HotUpdate.CheckHotUpdate();
            var hotUpdateAssetUrl = result.Item1;
            var allSize = result.Item2;
            
            // 资源热更新
            FF8.HotUpdate.StartHotUpdate(hotUpdateAssetUrl, () =>
            {
                LogF8.Log("完成");
            }, () =>
            {
                LogF8.Log("失败");
            }, eventArgs =>
            {
                // 已下载大小（字节）
                ulong downloadedBytes = eventArgs.DownloadInfo.DownloadedLength;
    
                // 总大小（字节）- 需要累加之前已完成的任务大小
                long totalBytes = allSize;
    
                // 下载速度计算（字节/秒）
                double speedBytesPerSecond = downloadedBytes / eventArgs.DownloadInfo.DownloadTimeSpan.TotalSeconds;
    
                // 单位转换：字节 -> MB
                double downloadedMB = downloadedBytes / (1024.0 * 1024.0);
                double totalMB = totalBytes / (1024.0 * 1024.0);
                double speedMBPerSecond = speedBytesPerSecond / (1024.0 * 1024.0);
    
                // 日志输出：进度，速度
                LogF8.Log($"进度：{downloadedMB:F2}MB/{totalMB:F2}MB, 速度：{speedMBPerSecond:F2}MB/s");
            });

            // 检查未加载的分包
            List<string> subPackage = FF8.HotUpdate.CheckPackageUpdate(GameConfig.LocalGameVersion.SubPackage);
            
            // 分包加载
            FF8.HotUpdate.StartPackageUpdate(subPackage, () =>
            {
                LogF8.Log("完成");
            }, () =>
            {
                LogF8.Log("失败");
            }, eventArgs =>
            {
                // 同上
            });
        }
    }
}
