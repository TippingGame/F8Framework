using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoF8Log : MonoBehaviour
    {
        void Start()
        {
            /*----------Log基础功能----------*/
            LogF8.Log(this);
            LogF8.Log("测试{0}", 1);
            LogF8.Log("3123测试", this);
            LogF8.LogNet("1123{0}", "测试");
            LogF8.LogEvent(this);
            LogF8.LogConfig(this);
            LogF8.LogView(this);
            LogF8.LogEntity(this);


            /*----------Log其他功能----------*/
            // 开启写入log文件
            FF8.LogWriter.OnEnterGame();
            // 开启捕获错误日志
            LogF8.GetCrashErrorMessage();
            // 开始监视代码使用情况
            LogF8.Watch();
            LogF8.Log(LogF8.UseMemory);
            LogF8.Log(LogF8.UseTime);
        }
    }
}
