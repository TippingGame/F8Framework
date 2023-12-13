using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

public class DemoF8Log : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        F8LogHelper.Instance.OnEnterGame();
        LogF8.GetCrashErrorMessage();
        LogF8.Log(LogF8.UseMemory);
        LogF8.Watch();
        for (int i = 0; i < 11; i++)
        {
            LogF8.Log(11111111);
            LogF8.AddError("test error");
        }
        LogF8.Log(LogF8.UseMemory);
        LogF8.Log(LogF8.UseTime);
        LogF8.Log(this);
        LogF8.Log("测试{0}",1);
        LogF8.LogNet("1123{0}", "测试");
        LogF8.LogEvent(this);
        LogF8.LogConfig(this);
        LogF8.LogColor(Color.red, this);
        LogF8.LogView(this);
        LogF8.LogEntity(this);
    }
}
