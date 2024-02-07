using F8Framework.Core;
using UnityEngine;

public class DemoF8Timer : MonoBehaviour
{
    void Start()
    {
        //普通Timer,传入自身this，每1秒执行一次，延迟0秒后开始，执行3次(-1表示循环)
        string timeid = FF8.Timer.Register(this,1f,0,3, () =>
        {
            LogF8.Log("tick");
        }, () =>
        {
            LogF8.Log("完成");
        });
        
        //FrameTimer,传入自身this，每1帧执行一次，延迟0帧后开始，循环执行(-1表示循环)
        timeid = FF8.Timer.RegisterFrame(this,1f,0,-1, () =>
        {
            LogF8.Log("tick");
        }, () =>
        {
            LogF8.Log("完成");
        });
        
        FF8.Timer.UnRegister(timeid);//停止名为timeid的Timer
        
        //自动OnApplicationFocus监听焦点，暂停所有Timer
        FF8.Timer.Pause();
        FF8.Timer.Restart();
        
        FF8.Timer.SetServerTime(1702573904000);//网络游戏，与服务器对表
        FF8.Timer.GetServerTime();
        
        FF8.Timer.GetTime(); //获取游戏中的总时长
    }
}
