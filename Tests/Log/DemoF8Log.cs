using F8Framework.Core;
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

            LogF8.LogStackTrace("日志堆栈跟踪");

            LogF8.LogToMainThread("打印日志到主线程");
            LogF8.LogErrorToMainThread("打印错误日志到主线程");

            /*----------Log其他功能----------*/
            // 启用或禁用Log
            LogF8.EnabledLog();
            LogF8.DisableLog();
            
            // 开启写入log文件
            FF8.LogWriter.OnEnterGame();
            
            // 开启捕获错误日志
            LogF8.GetCrashErrorMessage();
            
            // 开始监视代码使用情况
            LogF8.Watch();
            LogF8.Log(LogF8.UseMemory);
            LogF8.Log(LogF8.UseTime);
            
            /*----------LogViewer界面功能----------*/
            // 为LogViewer界面添加一条命令，TestLog为this类的方法
            Function.Instance.AddCommand(this, "TestLog", new object[] { 2 });
            
            // 为LogViewer界面添加作弊码
            Function.Instance.AddCheatKeyCallback((cheatKey) =>
            {
                LogF8.Log("Call cheat key callback with : " + cheatKey);
            });
        }
    }
}
