using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class MessageManager : Singleton<MessageManager>,MessageInterface 
    {
        private Dictionary<int, HashSet<IEventDataBase>> events = new Dictionary<int, HashSet<IEventDataBase>>();
        private HashSet<IEventDataBase> delects = new HashSet<IEventDataBase>();
        private HashSet<IEventDataBase> callStack = new HashSet<IEventDataBase>(); // 检测死循环调用
        
        private void MessageLoop(string debugInfo)
        {
            LogF8.LogEvent("消息死循环:{0}",debugInfo);
        }
        private void NotActionLog(string eventName, string actionName)
        {
            LogF8.LogEvent("函数不存在:【{0}】【{1}】",eventName,actionName);
        }
        private void NotListenerLog(string debugInfo)
        {
            LogF8.LogEvent("监听者不存在:【{0}】【{1}】",debugInfo);
        }
        private void NotEventLog(string eventName)
        {
            LogF8.LogEvent("事件不存在:【{0}】",eventName);
        }
        private void ClearCallStack()
        {
            callStack.Clear();
        }

        private bool IsInCallStack(IEventDataBase eventData)
        {
            return callStack.Contains(eventData);
        }
        public void AddEventListener<T>(T eventName,  Action listener, object handle) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (!events.ContainsKey(tempName))
            {
                events[tempName] = new HashSet<IEventDataBase>();
            }
            IEventDataBase eventData = new EventData<T>(eventName, listener, handle);
            events[tempName].Add(eventData);
        }

        public void AddEventListener<T>(T eventName, Action<object[]> listener, object handle) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (!events.ContainsKey(tempName))
            {
                events[tempName] = new HashSet<IEventDataBase>();
            }
            IEventDataBase eventData = new EventData<T,object[]>(eventName, listener, handle);
            events[tempName].Add(eventData);
        }
        
        public void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (events.ContainsKey(tempName))
            {
                if (events[tempName].Count == 0)
                {
                    NotActionLog(eventName.ToString(), listener.Method.Name);
                }
                else
                {
                    foreach (IEventDataBase itemObj in events[tempName])
                    {
                        if (itemObj is EventData<T> eventData)
                        {
                            if (eventData.Listener == listener && eventData.Handle == handle)
                            {
                                eventData.Handle = null;
                                delects.Add(eventData);
                            }
                        }
                    }
                    
                    foreach (var delect in delects)
                    {
                        events[tempName].Remove(delect);
                    }
                }
            }
            else
            {
                NotEventLog(eventName.ToString());
            }
        }
        
        public void RemoveEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (events.ContainsKey(tempName))
            {
                if (events[tempName].Count == 0)
                {
                    NotActionLog(eventName.ToString(), listener.Method.Name);
                }
                else
                {
                    foreach (IEventDataBase itemObj in events[tempName])
                    {
                        if (itemObj is EventData<T,object[]> eventData)
                        {
                            if (eventData.Listener == listener && eventData.Handle == handle)
                            {
                                eventData.Handle = null;
                                delects.Add(itemObj);
                            }
                        }
                    }
                    
                    foreach (var delect in delects)
                    {
                        events[tempName].Remove(delect);
                    }
                }
            }
            else
            {
                NotEventLog(eventName.ToString());
            }
        }

        public void DispatchEvent<T>(T eventName) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (events.TryGetValue(tempName, out HashSet<IEventDataBase> eventDatas))
            {
                foreach (IEventDataBase obj in eventDatas)
                {
                    if (IsInCallStack(obj))
                    {
                        MessageLoop(obj.LogDebugInfo());
                        continue;
                    }
                    callStack.Add(obj);
                    if (obj is EventData<T> eventData)
                    {
                        if (eventData.Handle == null || eventData.Handle.Equals(null))
                        {
                            NotListenerLog(obj.LogDebugInfo());
                            continue;
                        }
                        eventData.Listener.Invoke();
                    }
                    else if (obj is EventData<T,object[]> eventData1)
                    {
                        if (eventData1.Handle == null || eventData1.Handle.Equals(null))
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
        }

        public void DispatchEvent<T>(T eventName, object[] arg1) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (events.TryGetValue(tempName, out HashSet<IEventDataBase> eventDatas))
            {
                foreach (IEventDataBase obj in eventDatas)
                {
                    if (IsInCallStack(obj))
                    {
                        MessageLoop(obj.LogDebugInfo());
                        continue;
                    }
                    callStack.Add(obj);
                    if (obj is EventData<T,object[]> eventData)
                    {
                        if (eventData.Handle == null || eventData.Handle.Equals(null))
                        {
                            NotListenerLog(obj.LogDebugInfo());
                            continue;
                        }
                        eventData.Listener.Invoke(arg1);
                    }
                    else if (obj is EventData<T> eventData1)
                    {
                        if (eventData1.Handle == null || eventData1.Handle.Equals(null))
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
        }

        public void Clear()
        {
            events.Clear();
            delects.Clear();
            callStack.Clear();
        }
    }
}

