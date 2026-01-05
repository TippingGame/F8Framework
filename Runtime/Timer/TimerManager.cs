using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public class TimerManager : ModuleSingleton<TimerManager>, IModule
    {
        private List<Timer> times = new List<Timer>(); // 存储计时器的列表
        private Stack<Timer> timerPool = new Stack<Timer>();
        private long initTime; // 初始化时间
        private long serverTime; // 服务器时间
        private long tempTime; // 临时时间
        private bool isFocus = true; // 是否处于焦点状态
        private int frameTime = 1; // 帧时间，默认为1

        public void OnInit(object createParam)
        {
            initTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            serverTime = 0;
            tempTime = 0;
        }

        public void OnLateUpdate()
        {

        }

        public void OnFixedUpdate()
        {

        }

        public void OnTermination()
        {
            MessageManager.Instance.RemoveEventListener(MessageEvent.ApplicationFocus, OnApplicationFocus, this);
            MessageManager.Instance.RemoveEventListener(MessageEvent.NotApplicationFocus, NotApplicationFocus, this);
            times.Clear();
            base.Destroy();
        }

        public void OnUpdate()
        {
            if (isFocus == false || times.Count <= 0) // 如果失去焦点或者计时器数量为0，则返回
            {
                return;
            }

            for (int i = 0; i < times.Count; i++)
            {
                Timer timer = times[i];

                if (timer.IsFinish)
                {
                    times.RemoveAt(i);
                    ReturnTimer(timer);
                    i--;
                    continue;
                }

                float dt = timer.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                
                // 调用计时器
                int triggerCount = timer.IsFrameTimer ? timer.Update(dt > 0 ? frameTime : 0) : timer.Update(dt);

                if (triggerCount > 0) // 如果本帧触发次数大于0，执行相关逻辑
                {
                    if (timer.IsFinish || timer.Handle == null || timer.Handle.Equals(null))
                    {
                        timer.IsFinish = true;
                        continue;
                    }

                    int field = timer.Field; // 获取计时器剩余字段值

                    for (int j = 0; j < triggerCount; j++)
                    {
                        field = field > 0 ? field - 1 : field; // 每次减少计时器字段值

                        if (field == 0) // 若字段值为0，触发onSecond事件，并执行OnTimerComplete
                        {
                            timer.Field = field; // 更新计时器剩余字段值
                            timer.OnSecond?.Invoke();
                            OnTimerComplete(timer);
                            break;
                        }
                        else
                        {
                            timer.Field = field; // 更新计时器剩余字段值
                            timer.OnSecond?.Invoke();
                        }
                    }
                }
            }
        }

        private void OnTimerComplete(Timer timer)
        {
            timer.IsFinish = true;
            if (timer.OnComplete is { } onComplete) // 若OnComplete事件存在，触发事件
            {
                onComplete.Invoke();
            }
        }

        private Timer GetTimer(object handle, int id, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null, bool ignoreTimeScale = false, bool isFrameTimer = false) {
            Timer timer;
            if (timerPool.Count > 0) {
                timer = timerPool.Pop();
                timer.Reset();
            } else {
                timer = new Timer();
            }

            timer.Init(handle, id, step, delay, field, onSecond, onComplete, ignoreTimeScale, isFrameTimer);
            return timer;
        }

        private void ReturnTimer(Timer timer) {
            timer.Handle = null;
            timer.OnSecond = null;
            timer.OnComplete = null;
            timerPool.Push(timer);
        }
        
        // 注册一个计时器并返回其ID
        public int AddTimer(object handle, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null, bool ignoreTimeScale = false)
        {
            int id = Guid.NewGuid().GetHashCode(); // 生成一个唯一的ID
            Timer timer = GetTimer(handle, id, step, delay, field, onSecond, onComplete, ignoreTimeScale, false); // 创建一个计时器对象
            times.Add(timer);
            return id;
        }
        
        public int AddTimer(float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return AddTimer(this, duration, duration, 1, null, onComplete, ignoreTimeScale);
        }
        
        public int AddTimer(float duration, bool isLooped, Action onComplete = null, bool ignoreTimeScale = false)
        {
            if (isLooped)
            {
                return AddTimer(this, duration, duration, -1, onComplete, null, ignoreTimeScale);
            }

            return AddTimer(this, duration, duration, 1, null, onComplete, ignoreTimeScale);
        }
        
        public int AddTimer(float step = 1f, int field = 0, Action onSecond = null, Action onComplete = null, bool ignoreTimeScale = false)
        {
            return AddTimer(this, step, step, field, onSecond, onComplete, ignoreTimeScale);
        }

        // 注册一个以帧为单位的计时器并返回其ID
        public int AddTimerFrame(object handle, float stepFrame = 1f, float delayFrame = 0f, int field = 0, Action onFrame = null, Action onComplete = null, bool ignoreTimeScale = false)
        {
            int id = Guid.NewGuid().GetHashCode(); // 生成一个唯一的ID
            Timer timer = GetTimer(handle, id, stepFrame, delayFrame, field, onFrame, onComplete, ignoreTimeScale, true); // 创建一个以帧为单位的计时器对象
            times.Add(timer);
            return id;
        }
        
        public int AddTimerFrame(float duration, Action onComplete, bool ignoreTimeScale = false)
        {
            return AddTimerFrame(this, duration, duration, 1, null, onComplete, ignoreTimeScale);
        }
        
        public int AddTimerFrame(float duration, bool isLooped, Action onComplete, bool ignoreTimeScale = false)
        {
            if (isLooped)
            {
                return AddTimerFrame(this, duration, duration, -1, onComplete, null, ignoreTimeScale);
            }

            return AddTimerFrame(this, duration, duration, 1, null, onComplete, ignoreTimeScale);
        }
        
        public int AddTimerFrame(float step = 1f, int field = 0, Action onSecond = null, Action onComplete = null, bool ignoreTimeScale = false)
        {
            return AddTimerFrame(this, step, step, field, onSecond, onComplete, ignoreTimeScale);
        }

        // 根据ID注销计时器
        public void RemoveTimer(int id)
        {
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i].ID == id)
                {
                    times[i].IsFinish = true;
                    break;
                }
            }
        }

        // 设置服务器时间
        public void SetServerTime(long val)
        {
            if (val != 0) // 如果传入的值不为0，则更新服务器时间和临时时间
            {
                serverTime = val;
                tempTime = GetTime();
            }
        }

        // 获取服务器时间
        public long GetServerTime()
        {
            return (serverTime + (GetTime() - tempTime)); // 返回服务器时间加上当前时间与临时时间之间的差值
        }

        // 获取游戏中的总时长
        public long GetTime()
        {
            //可改为Unity启动的总时长
            // float floatValue = Time.time;
            // long longValue = (long)(floatValue * 1000000);
            // return longValue;
            return GetLocalTime() - initTime; // 返回当前时间与初始化时间的差值
        }

        // 获取本地时间
        public long GetLocalTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 返回当前时间的毫秒数
        }

        // 暂停所有计时器，或指定计时器
        public void Pause(int id = 0)
        {
            if (id == 0)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    times[i].IsPaused = true;
                }
            }
            else
            {
                for (int i = 0; i < times.Count; i++)
                {
                    if (times[i].ID == id)
                    {
                        times[i].IsPaused = true;
                        break;
                    }
                }
            }
        }

        // 恢复所有计时器，或指定计时器
        public void Resume(int id = 0)
        {
            if (id == 0)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    times[i].IsPaused = false;
                }
            }
            else
            {
                for (int i = 0; i < times.Count; i++)
                {
                    if (times[i].ID == id)
                    {
                        times[i].IsPaused = false;
                        break;
                    }
                }
            }
        }
        
        public void AddListenerApplicationFocus()
        {
            MessageManager.Instance.AddEventListener(MessageEvent.ApplicationFocus, OnApplicationFocus, this);
            MessageManager.Instance.AddEventListener(MessageEvent.NotApplicationFocus, NotApplicationFocus, this);
        }

        // 当应用程序获得焦点时调用
        void OnApplicationFocus()
        {
            isFocus = true;
        }

        // 当应用程序失去焦点时调用
        void NotApplicationFocus()
        {
            isFocus = false;
        }

        // 重新启动所有计时器，或指定计时器
        public void Restart(int id = 0)
        {
            if (id == 0)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    times[i].Reset();
                }
            }
            else
            {
                for (int i = 0; i < times.Count; i++)
                {
                    if (times[i].ID == id)
                    {
                        times[i].Reset();
                        break;
                    }
                }
            }
        }
    }
}