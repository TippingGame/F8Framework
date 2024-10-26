using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    // 消息管理器类，实现了单例模式和消息管理接口
    public class MessageManager : ModuleSingletonMono<MessageManager>, IMessageManager, IModule
    {
        // 存储事件ID与事件处理器列表的字典
        private Dictionary<int, List<IEventDataBase>> events = new Dictionary<int, List<IEventDataBase>>();
        // 存储待删除的事件处理器列表
        private List<IEventDataBase> delects = new List<IEventDataBase>();
        // 用于检测死循环调用的调用栈
        private HashSet<IEventDataBase> callStack = new HashSet<IEventDataBase>();
        // 存储待触发的事件处理器列表
        private Dictionary<int, Queue<IEventDataBase>> dispatchInvokes = new Dictionary<int, Queue<IEventDataBase>>();

        // 输出消息死循环的函数
        private void MessageLoop(string debugInfo)
        {
            LogF8.LogError("消息死循环：{0}", debugInfo);
        }

        // 输出不存在事件处理函数的警告
        private void NotActionLog(string eventId, string actionName)
        {
            LogF8.LogEvent("函数不存在：【{0}】【{1}】", eventId, actionName);
        }

        // 输出不存在监听者的警告
        private void NotListenerLog(string debugInfo)
        {
            LogF8.LogEvent("监听者不存在：{0}", debugInfo);
        }

        // 输出不存在事件的警告
        private void NotEventLogDispatch(string eventId)
        {
            LogF8.LogEvent("没有创建监听，发送事件：【{0}】", eventId);
        }
        
        // 输出不存在事件的警告
        private void NotEventLogRemove(string eventId)
        {
            LogF8.LogEvent("没有创建监听，移除监听：【{0}】", eventId);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) // 应用程序获得焦点
            {
                DispatchEvent(MessageEvent.ApplicationFocus);
            }
            else // 应用程序失去焦点
            {
                DispatchEvent(MessageEvent.NotApplicationFocus);
            }
        }
    
        private void OnApplicationQuit()
        {
            DispatchEvent(MessageEvent.ApplicationQuit);
        }
        
        // 清空调用栈
        private void ClearCallStack()
        {
            callStack.Clear();
        }

        // 判断事件是否在调用栈中
        private bool IsInCallStack(IEventDataBase eventData)
        {
            return callStack.Contains(eventData);
        }

        // 添加事件监听器（不带参数）
        public void AddEventListener<T>(T eventName, Action listener, object handle) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            AddEventListener(tempName, listener, handle);
        }

        // 添加事件监听器（不带参数）
        public void AddEventListener(int eventId, Action listener, object handle)
        {
            // 创建事件数据对象
            IEventDataBase eventData = new EventData(eventId, listener, handle);

            // 检查是否存在相同的事件数据
            if (!events.ContainsKey(eventId))
            {
                events[eventId] = new List<IEventDataBase>(); // 如果不存在，则创建一个新列表
            }
            else
            {
                if (events[eventId].Contains(eventData))
                {
                    LogF8.LogEvent("不能允许存在重复的事件处理函数。");
                    return;
                }
            }

            events[eventId].Add(eventData);
        }

        // 添加事件监听器（带参数）
        public void AddEventListener<T>(T eventName, Action<object[]> listener, object handle) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            AddEventListener(tempName, listener, handle);
        }

        // 添加事件监听器（带参数）
        public void AddEventListener(int eventId, Action<object[]> listener, object handle)
        {
            IEventDataBase eventData = new EventData<object[]>(eventId, listener, handle);
            if (!events.ContainsKey(eventId))
            {
                events[eventId] = new List<IEventDataBase>();
            }
            else
            {
                if (events[eventId].Contains(eventData))
                {
                    LogF8.LogEvent("不能允许存在重复的事件处理函数。");
                    return;
                }
            }

            events[eventId].Add(eventData);
        }

        // 移除事件监听器（不带参数）
        public void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName, listener, handle);
        }

        // 移除事件监听器（不带参数）
        public void RemoveEventListener(int eventId, Action listener, object handle = null)
        {
            if (!events.ContainsKey(eventId))
            {
                NotEventLogRemove(eventId.ToString());
                return;
            }

            var eventList = events[eventId];
            if (eventList.Count == 0)
            {
                NotActionLog(eventId.ToString(), listener.Method.Name);
                return;
            }

            delects.Clear();

            foreach (var itemObj in eventList)
            {
                if (itemObj is EventData eventData && eventData.Listener == listener && eventData.Handle == handle)
                {
                    eventData.Handle = null;
                    delects.Add(eventData);
                }
            }

            foreach (var deletion in delects)
            {
                eventList.Remove(deletion);
            }

            delects.Clear();
        }

        // 移除事件监听器（带参数）
        public void RemoveEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName, listener, handle);
        }

        // 移除事件监听器（带参数）
        public void RemoveEventListener(int eventId, Action<object[]> listener, object handle = null)
        {
            if (!events.ContainsKey(eventId))
            {
                NotEventLogRemove(eventId.ToString());
                return;
            }

            var eventList = events[eventId];
            if (eventList.Count == 0)
            {
                NotActionLog(eventId.ToString(), listener.Method.Name);
                return;
            }

            delects.Clear();

            foreach (var itemObj in eventList)
            {
                if (itemObj is EventData<object[]> eventData && eventData.Listener == listener && eventData.Handle == handle)
                {
                    eventData.Handle = null;
                    delects.Add(itemObj);
                }
            }

            foreach (var deletion in delects)
            {
                eventList.Remove(deletion);
            }

            delects.Clear();
        }

        // 删除此事件所有监听
        public void RemoveEventListener<T>(T eventName)
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName);
        }
        
        public void RemoveEventListener(int eventId)
        {
            if (events.ContainsKey(eventId))
            {
                if (events[eventId].Count > 0)
                {
                    events.Remove(eventId);
                }
            }
        }
        
        // 触发事件（不带参数）
        public void DispatchEvent<T>(T eventName) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            DispatchEvent(tempName);
        }

        // 触发事件（不带参数）
        public void DispatchEvent(int eventId)
        {
            if (!events.TryGetValue(eventId, out List<IEventDataBase> eventDatas))
            {
                NotEventLogDispatch(eventId.ToString());
                return;
            }

            foreach (IEventDataBase obj in eventDatas)
            {
                if (IsInCallStack(obj))
                {
                    MessageLoop(obj.LogDebugInfo());
                    continue;
                }

                if (obj.EventDataShouldBeInvoked())
                {
                    if (!dispatchInvokes.ContainsKey(eventId))
                    {
                        dispatchInvokes[eventId] = new Queue<IEventDataBase>();
                    }
                    dispatchInvokes[eventId].Enqueue(obj);
                    callStack.Add(obj);
                }
                else
                {
                    NotListenerLog(obj.LogDebugInfo());
                    continue;
                }
            }
            
            while (dispatchInvokes.ContainsKey(eventId) && dispatchInvokes[eventId].Count > 0)
            {
                var obj = dispatchInvokes[eventId].Dequeue();
                if (obj is EventData eventData)
                {
                    eventData.Listener.Invoke();
                }
                else if (obj is EventData<object[]> eventData1)
                {
                    eventData1.Listener.Invoke(null);
                }
            }

            ClearCallStack(); // 清除调用栈
        }

        // 触发事件（带参数）
        public void DispatchEvent<T>(T eventName, params object[] arg1) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            DispatchEvent(tempName, arg1);
        }

        // 触发事件（带参数）
        public void DispatchEvent(int eventId, params object[] arg1)
        {
            if (!events.TryGetValue(eventId, out List<IEventDataBase> eventDatas))
            {
                NotEventLogDispatch(eventId.ToString());
                return;
            }

            foreach (IEventDataBase obj in eventDatas)
            {
                if (IsInCallStack(obj))
                {
                    MessageLoop(obj.LogDebugInfo());
                    continue;
                }
                
                if (obj.EventDataShouldBeInvoked())
                {
                    if (!dispatchInvokes.ContainsKey(eventId))
                    {
                        dispatchInvokes[eventId] = new Queue<IEventDataBase>();
                    }
                    dispatchInvokes[eventId].Enqueue(obj);
                    callStack.Add(obj);
                }
                else
                {
                    NotListenerLog(obj.LogDebugInfo());
                    continue;
                }
            }
            
            while (dispatchInvokes.ContainsKey(eventId) && dispatchInvokes[eventId].Count > 0)
            {
                var obj = dispatchInvokes[eventId].Dequeue();
                if (obj is EventData<object[]> eventData1)
                {
                    eventData1.Listener.Invoke(arg1);
                }
                else if (obj is EventData eventData)
                {
                    eventData.Listener.Invoke();
                }
            }

            ClearCallStack(); // 清除调用栈
        }

        // 清空事件管理器
        public void Clear()
        {
            events.Clear();
            delects.Clear();
            callStack.Clear();
            dispatchInvokes.Clear();
        }

        // 模块初始化
        public void OnInit(object createParam)
        {

        }

        // 更新逻辑
        public void OnUpdate()
        {

        }

        // 更新逻辑（后执行）
        public void OnLateUpdate()
        {

        }

        // 固定更新逻辑
        public void OnFixedUpdate()
        {

        }

        // 终止模块
        public void OnTermination()
        {
            Clear();
            Destroy(gameObject);
        }
    }
}