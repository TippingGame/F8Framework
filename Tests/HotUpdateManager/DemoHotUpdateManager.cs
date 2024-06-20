using System;
using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoHotUpdateManager : MonoBehaviour
    {
        IEnumerator Start()
        {
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
            }, progress =>
            {
                LogF8.Log("进度：" + progress);
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
            }, progress =>
            {
                LogF8.Log("进度：" + progress);
            });
        }
    }
}
