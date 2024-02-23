using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public class TimerManager : ModuleSingleton<TimerManager>, IModule
    {
        private Dictionary<string, Timer> times = new Dictionary<string, Timer>(); // 存储计时器的字典
        private HashSet<string> deleteTimes = new HashSet<string>(); // 存储要删除的计时器ID的哈希集合
        private Dictionary<string, Timer> addTimes = new Dictionary<string, Timer>(); // 存储要添加的计时器
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
                string id = pair.Value.ID;

                if (pair.Value.IsFrameTimer ? pair.Value.Update(frameTime) : pair.Value.Update(dt)) // 根据计时器类型更新计时器
                {
                    if (pair.Value.IsFinish || pair.Value.Handle == null || pair.Value.Handle.Equals(null)) // 若计时器已经完成，若对象为空，标记计时器为完成，并将其ID添加到待删除列表
                    {
                        deleteTimes.Add(pair.Key);
                        continue;
                    }
                    int field = pair.Value.Field; // 获取计时器剩余字段值
                    field = field > 0 ? field - 1 : field; // 减少计时器字段值
                    if (field == 0) // 若字段值为0，触发onSecond事件，并执行OnTimerComplete
                    {
                        pair.Value.Field = field; // 更新计时器剩余字段值
                        if (pair.Value.OnSecond is { } onSecond)
                        {
                            onSecond.Invoke();
                        }
                        OnTimerComplete(id);
                    }
                    else
                    {
                        pair.Value.Field = field; // 更新计时器剩余字段值
                        if (pair.Value.OnSecond is { } onSecond)
                        {
                            onSecond.Invoke();
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

       private void OnTimerComplete(string id)
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
        public string AddTimer(object handle, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null)
        {
            string id = Guid.NewGuid().ToString(); // 生成一个唯一的ID
            Timer timer = new Timer(handle, id, step, delay, field, onSecond, onComplete, false); // 创建一个计时器对象
            addTimes.Add(id, timer);
            return id;
        }

        // 注册一个以帧为单位的计时器并返回其ID
        public string AddTimerFrame(object handle, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null)
        {
            string id = Guid.NewGuid().ToString(); // 生成一个唯一的ID
            Timer timer = new Timer(handle, id, step, delay, field, onSecond, onComplete, true); // 创建一个以帧为单位的计时器对象
            addTimes.Add(id, timer);
            return id;
        }

        // 根据ID注销计时器
        public void RemoveTimer(string id)
        {
            if (id == null)
            {
                return;
            }
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

        // 当应用程序获得或失去焦点时调用
        // void OnApplicationFocus(bool hasFocus)
        // {
        //     if (hasFocus) // 如果应用程序获得焦点，重新开始所有计时器
        //     {
        //         Restart();
        //         isFocus = true;
        //     }
        //     else // 如果应用程序失去焦点，暂停所有计时器
        //     {
        //         isFocus = false;
        //         Pause();
        //     }
        // }

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
