using System;
using UnityEngine;

namespace F8Framework.Core
{
    public static class F8TimerExts
    {
        public static int AttachTimerF8(this MonoBehaviour behaviour, float duration, Action onComplete,
            Action onUpdate = null, bool isLoop = false, bool ignoreTimeScale = false)
        {
            int times = isLoop ? -1 : 1;
            return TimerManager.Instance.AddTimer(behaviour, duration, duration, times, onUpdate, onComplete, ignoreTimeScale);
        }
        
        // 简单的一次性延迟
        public static int DelayTimerF8(this MonoBehaviour mono, float seconds, Action onComplete, bool ignoreTimeScale = false)
        {
            return TimerManager.Instance.AddTimer(mono, seconds, seconds, 1, null, onComplete, ignoreTimeScale);
        }

        // 简单的间隔执行
        public static int IntervalTimerF8(this MonoBehaviour mono, float seconds, Action onSecond, bool ignoreTimeScale = false)
        {
            return TimerManager.Instance.AddTimer(mono, seconds, seconds, -1, onSecond, null, ignoreTimeScale);
        }

        // 执行指定次数
        public static int RepeatTimerF8(this MonoBehaviour mono, float interval, int times, Action onSecond, bool ignoreTimeScale = false)
        {
            return TimerManager.Instance.AddTimer(mono, interval, interval, times, onSecond, null, ignoreTimeScale);
        }

        // 条件计时器（当条件满足时停止）
        public static int UntilTimerF8(this MonoBehaviour mono, float interval, Func<bool> condition, Action onSecond, Action onComplete = null, bool ignoreTimeScale = false)
        {
            int timerId = -1;
        
            timerId = TimerManager.Instance.AddTimer(mono, interval, interval, -1, () => 
            {
                if (condition())
                {
                    TimerManager.Instance.RemoveTimer(timerId);
                    return;
                }
                onSecond?.Invoke();
            }, onComplete, ignoreTimeScale);

            return timerId;
        }
    }
}