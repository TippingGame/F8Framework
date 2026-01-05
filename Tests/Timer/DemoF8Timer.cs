using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoF8Timer : MonoBehaviour
    {
        void Start()
        {
            // 普通Timer,传入自身this，每1秒执行一次，延迟0秒后开始，执行3次(-1表示循环)
            int timeid = FF8.Timer.AddTimer(this, 1f, 0f, 3,
                () => { LogF8.Log("tick"); },
                () => { LogF8.Log("完成"); },
                ignoreTimeScale: false);

            // 拓展方法
            FF8.Timer.AddTimer(1f, () => { });
            FF8.Timer.AddTimer(1f, false, () => { });
            FF8.Timer.AddTimer(1f, 1, () => { }, () => { });
            // 更多拓展方法
            this.AttachTimerF8(1f, () => { }, () => { }, false);
            this.DelayTimerF8(1f, () => { });
            this.IntervalTimerF8(1f, () => { });
            this.RepeatTimerF8(1f, 5, () => { });
            this.UntilTimerF8(1f,  () => true, () => { });

            // FrameTimer,传入自身this，每1帧执行一次，延迟0帧后开始，循环执行(-1表示循环)
            timeid = FF8.Timer.AddTimerFrame(this, 1f, 0f, -1,
                () => { LogF8.Log("tick"); },
                () => { LogF8.Log("完成"); },
                ignoreTimeScale: false);
            
            // 拓展方法
            FF8.Timer.AddTimerFrame(1f, () => { });
            FF8.Timer.AddTimerFrame(1f, false, () => { });
            FF8.Timer.AddTimerFrame(1f, 1, () => { }, () => { });

            // 停止id为timeid的Timer
            FF8.Timer.RemoveTimer(timeid);

            // 监听游戏程序获得或失去焦点，重新开始或暂停所有Timer
            FF8.Timer.AddListenerApplicationFocus();

            // 手动重新开始或暂停所有Timer，或指定id
            FF8.Timer.Pause();
            FF8.Timer.Resume();
            // 重新启动所有Timer，或指定id
            FF8.Timer.Restart();

            // 网络游戏，与服务器对表，单位ms
            FF8.Timer.SetServerTime(1702573904000);
            FF8.Timer.GetServerTime();

            // 获取游戏中的总时长
            FF8.Timer.GetTime();
        }
    }
}
