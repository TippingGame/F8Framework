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
        }
    }
}
