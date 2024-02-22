using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class MessageManager : ModuleSingleton<MessageManager>, IMessageManager ,IModule
    {
        private Dictionary<int, List<IEventDataBase>> events = new Dictionary<int, List<IEventDataBase>>();
        private HashSet<IEventDataBase> delects = new HashSet<IEventDataBase>();
        private HashSet<IEventDataBase> callStack = new HashSet<IEventDataBase>(); // 检测死循环调用
        
        private void MessageLoop(string debugInfo)
        {
            LogF8.LogError("消息死循环:{0}", debugInfo);
        }
        private void NotActionLog(string eventName, string actionName)
        {
            LogF8.LogEvent("函数不存在:【{0}】【{1}】", eventName, actionName);
        }
        private void NotListenerLog(string debugInfo)
        {
            LogF8.LogEvent("监听者不存在:【{0}】【{1}】", debugInfo);
        }
        private void NotEventLog(string eventName)
        {
            LogF8.LogEvent("事件不存在:【{0}】", eventName);
        }
        private void ClearCallStack()
        {
            callStack.Clear();
        }

        private bool IsInCallStack(IEventDataBase eventData)
        {
            return callStack.Contains(eventData);
        }
        public void AddEventListener<T>(T eventName, Action listener, object handle) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            AddEventListener(tempName, listener, handle);
        }

        public void AddEventListener(int eventId, Action listener, object handle = null)
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

        public void AddEventListener<T>(T eventName, Action<object[]> listener, object handle) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            AddEventListener(tempName, listener, handle);
        }

        public void AddEventListener(int eventId, Action<object[]> listener, object handle = null)
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

        public void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName, listener, handle);
        }

        public void RemoveEventListener(int eventId, Action listener, object handle = null)
        {
            if (!events.ContainsKey(eventId))
            {
                NotEventLog(eventId.ToString());
                return;
            }

            var eventList = events[eventId];
            if (eventList.Count == 0)
            {
                NotActionLog(eventId.ToString(), listener.Method.Name);
                return;
            }

            var deletions = new List<EventData>();
            foreach (var itemObj in eventList)
            {
                if (itemObj is EventData eventData && eventData.Listener == listener && eventData.Handle == handle)
                {
                    eventData.Handle = null;
                    deletions.Add(eventData);
                }
            }

            foreach (var deletion in deletions)
            {
                eventList.Remove(deletion);
            }
        }

        public void RemoveEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName, listener, handle);
        }

        public void RemoveEventListener(int eventId, Action<object[]> listener, object handle = null)
        {
            if (!events.ContainsKey(eventId))
            {
                NotEventLog(eventId.ToString());
                return;
            }

            var eventList = events[eventId];
            if (eventList.Count == 0)
            {
                NotActionLog(eventId.ToString(), listener.Method.Name);
                return;
            }

            var deletions = new List<IEventDataBase>();
            foreach (var itemObj in eventList)
            {
                if (itemObj is EventData<object[]> eventData && eventData.Listener == listener && eventData.Handle == handle)
                {
                    eventData.Handle = null;
                    deletions.Add(itemObj);
                }
            }

            foreach (var deletion in deletions)
            {
                eventList.Remove(deletion);
            }
        }

        public void DispatchEvent<T>(T eventName) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            DispatchEvent(tempName);
        }

        public void DispatchEvent(int eventId)
        {
            if (!events.TryGetValue(eventId, out List<IEventDataBase> eventDatas))
            {
                NotEventLog(eventId.ToString());
                return;
            }

            for (int i = eventDatas.Count - 1; i >= 0; i--)
            {
                IEventDataBase obj = eventDatas[i];
                if (IsInCallStack(obj))
                {
                    MessageLoop(obj.LogDebugInfo());
                    continue;
                }

                callStack.Add(obj);

                if (obj is EventData eventData)
                {
                    if (!eventData.EventDataShouldBeInvoked())
                    {
                        NotListenerLog(obj.LogDebugInfo());
                        continue;
                    }
                    eventData.Listener.Invoke();
                }
                else if (obj is EventData<object[]> eventData1)
                {
                    if (!eventData1.EventDataShouldBeInvoked())
                    {
                        NotListenerLog(obj.LogDebugInfo());
                        continue;
                    }
                    eventData1.Listener.Invoke(null);
                }

                callStack.Remove(obj);
            }

            ClearCallStack(); // 清除调用栈
        }

        public void DispatchEvent<T>(T eventName, params object[] arg1) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            DispatchEvent(tempName, arg1);
        }

        public void DispatchEvent(int eventId, params object[] arg1)
        {
            if (!events.TryGetValue(eventId, out List<IEventDataBase> eventDatas))
            {
                NotEventLog(eventId.ToString());
                return;
            }
            
            for (int i = eventDatas.Count - 1; i >= 0; i--)
            {
                IEventDataBase obj = eventDatas[i];
                if (IsInCallStack(obj))
                {
                    MessageLoop(obj.LogDebugInfo());
                    continue;
                }

                callStack.Add(obj);
                
                if (obj is EventData<object[]> eventData)
                {
                    if (!eventData.EventDataShouldBeInvoked())
                    {
                        NotListenerLog(obj.LogDebugInfo());
                        continue;
                    }
                    eventData.Listener.Invoke(arg1);
                }
                else if (obj is EventData eventData1)
                {
                    if (!eventData1.EventDataShouldBeInvoked())
                    {
                        NotListenerLog(obj.LogDebugInfo());
                        continue;
                    }
                    eventData1.Listener.Invoke();
                }
                
                callStack.Remove(obj);
            }

            ClearCallStack(); // 清除调用栈
        }

        public void Clear()
        {
            events.Clear();
            delects.Clear();
            callStack.Clear();
        }

        public void OnInit(object createParam)
        {
            
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            Clear();
            base.Destroy();
        }
    }
}

