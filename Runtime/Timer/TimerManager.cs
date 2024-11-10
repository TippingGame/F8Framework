using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public class TimerManager : ModuleSingleton<TimerManager>, IModule
    {
        private Dictionary<int, Timer> times = new Dictionary<int, Timer>(); // 存储计时器的字典
        private HashSet<int> deleteTimes = new HashSet<int>(); // 存储要删除的计时器ID的哈希集合
        private Dictionary<int, Timer> addTimes = new Dictionary<int, Timer>(); // 存储要添加的计时器
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
            base.Destroy();
        }
        
        public void OnUpdate()
        {
            foreach (var add in addTimes) //待添加字典
            {
                times.Add(add.Key, add.Value);
            }
            
            addTimes.Clear();
            
            if (isFocus == false || times.Count <= 0) // 如果失去焦点或者计时器数量为0，则返回
            {
                return;
            }
            float dt = Time.deltaTime;

            foreach (var pair in times)
            {
                Timer timer = pair.Value;
                int id = timer.ID;

                // 调用计时器
                int triggerCount = timer.IsFrameTimer ? timer.Update(frameTime) : timer.Update(dt);

                if (triggerCount > 0) // 如果本帧触发次数大于0，执行相关逻辑
                {
                    if (timer.IsFinish || timer.Handle == null || timer.Handle.Equals(null)) 
                    {
                        deleteTimes.Add(pair.Key);
                        continue;
                    }

                    int field = timer.Field; // 获取计时器剩余字段值

                    for (int i = 0; i < triggerCount; i++)
                    {
                        field = field > 0 ? field - 1 : field; // 每次减少计时器字段值

                        if (field == 0) // 若字段值为0，触发onSecond事件，并执行OnTimerComplete
                        {
                            timer.Field = field; // 更新计时器剩余字段值
                            timer.OnSecond?.Invoke();
                            OnTimerComplete(id);
                        }
                        else
                        {
                            timer.Field = field; // 更新计时器剩余字段值
                            timer.OnSecond?.Invoke();
                        }
                    }
                }
            }

            foreach (var delete in deleteTimes) // 删除已完成的计时器
            {
                times.Remove(delete);
            }
            
            deleteTimes.Clear();
        }

       private void OnTimerComplete(int id)
        {
            if (times.TryGetValue(id, out Timer timer)) // 根据ID获取计时器
            {
                if (timer.OnComplete is { } onComplete) // 若OnComplete事件存在，触发事件
                {
                    onComplete.Invoke();
                }
                timer.IsFinish = true;
            }
            if (addTimes.TryGetValue(id, out Timer addtimer)) //有可能在待添加里
            {
                addtimer.IsFinish = true;
            }
        }

        // 注册一个计时器并返回其ID
        public int AddTimer(object handle, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null)
        {
            int id = Guid.NewGuid().GetHashCode(); // 生成一个唯一的ID
            Timer timer = new Timer(handle, id, step, delay, field, onSecond, onComplete, false); // 创建一个计时器对象
            addTimes.Add(id, timer);
            return id;
        }

        // 注册一个以帧为单位的计时器并返回其ID
        public int AddTimerFrame(object handle, float stepFrame = 1f, float delayFrame = 0f, int field = 0, Action onFrame = null, Action onComplete = null)
        {
            int id = Guid.NewGuid().GetHashCode(); // 生成一个唯一的ID
            Timer timer = new Timer(handle, id, stepFrame, delayFrame, field, onFrame, onComplete, true); // 创建一个以帧为单位的计时器对象
            addTimes.Add(id, timer);
            return id;
        }

        // 根据ID注销计时器
        public void RemoveTimer(int id)
        {
            if (times.TryGetValue(id, out Timer timer)) // 根据ID获取计时器并标记为完成，将其ID添加到待删除列表
            {
                timer.IsFinish = true;
            }
            if (addTimes.TryGetValue(id, out Timer addtimer)) //有可能在待添加里
            {
                addtimer.IsFinish = true;
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

        // 暂停所有计时器
        public void Pause()
        {
            foreach (var pair in times)
            {
               pair.Value.StartTime = GetTime();
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
            Restart();
            isFocus = true;
        }
        
        // 当应用程序失去焦点时调用
        void NotApplicationFocus()
        {
            isFocus = false;
            Pause();
        }

        // 重新启动所有计时器
        public void Restart()
        {
            long currentTime = GetTime();

            foreach (var pair in times)
            {
                if (pair.Value.StartTime != 0) // 如果计时器的开始时间不为0
                {
                    long startTime = pair.Value.StartTime; // 获取计时器的开始时间
                    int interval = (int)((currentTime - startTime) / 1000); // 计算时间间隔（秒数）
                    int field = pair.Value.Field; // 获取计时器字段值
                    pair.Value.StartTime = 0; // 重置计时器的开始时间为0

                    if (field < 0)
                    {
                        // 处理循环计时器（若有需要）
                    }
                    else
                    {
                        field -= interval; // 减去时间间隔
                        if (field < 0) // 如果字段值小于0，将其置为0，并执行OnTimerComplete
                        {
                            field = 0;
                            pair.Value.Field = field; // 更新计时器字段值
                            OnTimerComplete(pair.Value.ID);
                        }
                        else
                        {
                            pair.Value.Field = field; // 更新计时器字段值
                        }
                    }
                }
            }
        }
    }
}
