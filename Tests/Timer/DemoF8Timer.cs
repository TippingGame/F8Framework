using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoF8Timer : MonoBehaviour
    {
        void Start()
        {
            // 普通Timer,传入自身this，每1秒执行一次，延迟0秒后开始，执行3次(-1表示循环)
            int timeid = FF8.Timer.AddTimer(this, 1f, 0f, 3, () => { LogF8.Log("tick"); }, () => { LogF8.Log("完成"); });

            // FrameTimer,传入自身this，每1帧执行一次，延迟0帧后开始，循环执行(-1表示循环)
            timeid = FF8.Timer.AddTimerFrame(this, 1f, 0f, -1, () => { LogF8.Log("tick"); },
                () => { LogF8.Log("完成"); });

            FF8.Timer.RemoveTimer(timeid); // 停止名为timeid的Timer

            // 监听游戏程序获得或失去焦点，重新开始或暂停所有Timer
            FF8.Timer.AddListenerApplicationFocus();

            // 手动重新开始或暂停所有Timer
            FF8.Timer.Pause();
            FF8.Timer.Restart();

            FF8.Timer.SetServerTime(1702573904000); // 网络游戏，与服务器对表，单位ms
            FF8.Timer.GetServerTime();

            FF8.Timer.GetTime(); // 获取游戏中的总时长
        }
    }
}
