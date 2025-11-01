using System;
using UnityEngine;

namespace F8Framework.Core
{
    public static class F8TimerExts
    {
        public static int AttachTimerF8(this MonoBehaviour behaviour, float duration, Action onComplete,
            Action onUpdate = null, bool isLooped = false, bool useRealTime = false)
        {
            int times = isLooped ? -1 : 1;
            return TimerManager.Instance.AddTimer(behaviour, duration, duration, times, onUpdate, onComplete, useRealTime);
        }
    }
}