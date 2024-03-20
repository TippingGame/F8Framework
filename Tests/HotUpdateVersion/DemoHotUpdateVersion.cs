using System.Collections;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoHotUpdateVersion : MonoBehaviour
    {
        IEnumerator Start()
        {
            // 初始化本地版本
            FF8.HotUpdate.InitLocalVersion();

            // 初始化远程版本
            yield return FF8.HotUpdate.InitRemoteVersion();

            // 资源热更新
            FF8.HotUpdate.CheckHotUpdate(() =>
            {
                LogF8.Log("完成");
            }, () =>
            {
                LogF8.Log("失败");
            }, progress =>
            {
                LogF8.Log("进度：" + progress);
            });
            
            // 分包加载
            FF8.HotUpdate.CheckPackageUpdate(() =>
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
