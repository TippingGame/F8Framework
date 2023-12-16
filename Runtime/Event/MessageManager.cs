using System;
using System.Collections.Generic;
using System.Reflection;

namespace F8Framework.Core
{
    public class MessageManager : Singleton<MessageManager>,MessageInterface 
    {
        private Dictionary<Int16, HashSet<object>> events = new Dictionary<Int16, HashSet<object>>();
        private HashSet<object> delects = new HashSet<object>();
        private void NotActionLog(string actionName)
        {
            LogF8.LogEvent("函数:【{0}】不存在",actionName);
        }
        private void NotListenerLog(string actionName)
        {
            LogF8.LogEvent("监听者:【{0}】不存在",actionName);
        }
        private void NotEventLog(string eventName)
        {
            LogF8.LogEvent("名为【{0}】的事件不存在",eventName);
        }
        public void AddEventListener<T>(T eventName,  Action listener, object obj = null) where T : Enum, IConvertible
        {
            Int16 tempName = Convert.ToInt16(eventName);
            if (!events.ContainsKey(tempName))
            {
                events[tempName] = new HashSet<object>();
            }
            EventData<T> eventData = new EventData<T>(eventName, listener, obj);
            events[tempName].Add(eventData);
        }

        public void AddEventListener<T>(T eventName, Action<object[]> listener, object obj = null) where T : Enum, IConvertible
        {
            Int16 tempName = Convert.ToInt16(eventName);
            if (!events.ContainsKey(tempName))
            {
                events[tempName] = new HashSet<object>();
            }
            EventData<T,object[]> eventData = new EventData<T,object[]>(eventName, listener, obj);
            events[tempName].Add(eventData);
        }
        
        public void RemoveEventListener<T>(T eventName, Action listener, object obj = null) where T : Enum, IConvertible
        {
            Int16 tempName = Convert.ToInt16(eventName);
            if (events.ContainsKey(tempName))
            {
                if (events[tempName].Count == 0)
                {
                    NotActionLog(eventName.ToString());
                }
                else
                {
                    foreach (object itemObj in events[tempName])
                    {
                        if (itemObj is EventData<T> eventData)
                        {
                            if (eventData.Listener == listener && eventData.Object == obj)
                            {
                                eventData.Object = null;
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
        
        public void RemoveEventListener<T>(T eventName, Action<object[]> listener, object obj = null) where T : Enum, IConvertible
        {
            Int16 tempName = Convert.ToInt16(eventName);
            if (events.ContainsKey(tempName))
            {
                if (events[tempName].Count == 0)
                {
                    NotActionLog(eventName.ToString());
                }
                else
                {
                    foreach (object itemObj in events[tempName])
                    {
                        if (itemObj is EventData<T,object[]> eventData)
                        {
                            if (eventData.Listener == listener && eventData.Object == obj)
                            {
                                eventData.Object = null;
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

        public void DispatchEvent<T>(T eventName) where T : Enum, IConvertible
        {
            Int16 tempName = Convert.ToInt16(eventName);
            if (events.TryGetValue(tempName, out HashSet<object> eventDatas))
            {
                foreach (object obj in eventDatas)
                {
                    if (obj is EventData<T> eventData)
                    {
                        if (eventData.Object == null || eventData.Object.Equals(null))
                        {
                            NotListenerLog(eventData.Listener.GetMethodInfo().ToString());
                            continue;
                        }
                        eventData.Listener.Invoke();
                    }
                    else if (obj is EventData<T,object[]> eventData1)
                    {
                        if (eventData1.Object == null || eventData1.Object.Equals(null))
                        {
                            NotListenerLog(eventData1.Listener.GetMethodInfo().ToString());
                            continue;
                        }
                        eventData1.Listener.Invoke(null);
                    }
                }
            }
        }

        public void DispatchEvent<T>(T eventName, object[] arg1) where T : Enum, IConvertible
        {
            Int16 tempName = Convert.ToInt16(eventName);
            if (events.TryGetValue(tempName, out HashSet<object> eventDatas))
            {
                foreach (object obj in eventDatas)
                {
                    if (obj is EventData<T,object[]> eventData)
                    {
                        if (eventData.Object == null || eventData.Object.Equals(null))
                        {
                            NotListenerLog(eventData.Listener.GetMethodInfo().ToString());
                            continue;
                        }
                        eventData.Listener.Invoke(arg1);
                    }
                    else if (obj is EventData<T> eventData1)
                    {
                        if (eventData1.Object == null || eventData1.Object.Equals(null))
                        {
                            NotListenerLog(eventData1.Listener.GetMethodInfo().ToString());
                            continue;
                        }
                        eventData1.Listener.Invoke();
                    }
                }
            }
        }

        public void Clear()
        {
            events.Clear();
            delects.Clear();
        }
    }
}

