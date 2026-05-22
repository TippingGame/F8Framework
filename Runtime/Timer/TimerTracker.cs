using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public sealed class TimerTracker
    {
        private readonly HashSet<int> _timerIds = new HashSet<int>();
        private bool _released;

        public int AddTimer(object handle, float step = 1f, float delay = 0f, int field = 0,
            Action onSecond = null, Action onComplete = null, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimer(handle, step, delay, field, onSecond, onComplete, ignoreTimeScale));
        }

        public int AddTimer(float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimer(duration, onComplete, ignoreTimeScale));
        }

        public int AddTimer(float duration, bool isLoop, Action onComplete = null, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimer(duration, isLoop, onComplete, ignoreTimeScale));
        }

        public int AddTimer(float step = 1f, int field = 0, Action onSecond = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimer(step, field, onSecond, onComplete, ignoreTimeScale));
        }

        public int AddTimer(object handle, float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimer(handle, duration, onComplete, ignoreTimeScale));
        }

        public int AddTimer(object handle, float duration, bool isLoop, Action onComplete = null,
            bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimer(handle, duration, isLoop, onComplete, ignoreTimeScale));
        }

        public int AddTimer(object handle, float step = 1f, int field = 0, Action onSecond = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimer(handle, step, field, onSecond, onComplete, ignoreTimeScale));
        }

        public int AddTimerFrame(object handle, float stepFrame = 1f, float delayFrame = 0f, int field = 0,
            Action onFrame = null, Action onComplete = null, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimerFrame(handle, stepFrame, delayFrame, field, onFrame, onComplete, ignoreTimeScale));
        }

        public int AddTimerFrame(float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimerFrame(duration, onComplete, ignoreTimeScale));
        }

        public int AddTimerFrame(float duration, bool isLoop, Action onComplete, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimerFrame(duration, isLoop, onComplete, ignoreTimeScale));
        }

        public int AddTimerFrame(float step = 1f, int field = 0, Action onSecond = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimerFrame(step, field, onSecond, onComplete, ignoreTimeScale));
        }

        public int AddTimerFrame(object handle, float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimerFrame(handle, duration, onComplete, ignoreTimeScale));
        }

        public int AddTimerFrame(object handle, float duration, bool isLoop, Action onComplete, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimerFrame(handle, duration, isLoop, onComplete, ignoreTimeScale));
        }

        public int AddTimerFrame(object handle, float step = 1f, int field = 0, Action onSecond = null,
            Action onComplete = null, bool ignoreTimeScale = false)
        {
            return Track(TimerManager.Instance.AddTimerFrame(handle, step, field, onSecond, onComplete, ignoreTimeScale));
        }

        public void RemoveTimer(int id)
        {
            if (!_timerIds.Remove(id))
                return;

            if (ModuleCenter.Contains<TimerManager>())
            {
                TimerManager.Instance.RemoveTimer(id);
            }
        }

        public void Pause(int id)
        {
            if (!_timerIds.Contains(id))
                return;

            if (ModuleCenter.Contains<TimerManager>())
            {
                TimerManager.Instance.Pause(id);
            }
        }

        public void Resume(int id)
        {
            if (!_timerIds.Contains(id))
                return;

            if (ModuleCenter.Contains<TimerManager>())
            {
                TimerManager.Instance.Resume(id);
            }
        }

        public void Clear()
        {
            _released = true;

            if (ModuleCenter.Contains<TimerManager>())
            {
                var timerManager = TimerManager.Instance;
                foreach (int timerId in _timerIds)
                {
                    timerManager.RemoveTimer(timerId);
                }
            }

            _timerIds.Clear();
        }

        private int Track(int id)
        {
            if (id <= 0)
                return id;

            if (_released)
            {
                if (ModuleCenter.Contains<TimerManager>())
                {
                    TimerManager.Instance.RemoveTimer(id);
                }
                return id;
            }

            _timerIds.Add(id);
            return id;
        }
    }
}
