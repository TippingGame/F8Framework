using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class EventDispatcher : IMessageManager
    {
        private Dictionary<int, HashSet<IEventDataBase>> events = new Dictionary<int, HashSet<IEventDataBase>>();
        // 存储待删除的事件处理器列表
        private List<IEventDataBase> delects = new List<IEventDataBase>();
        public void AddEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            AddEventListener(tempName, listener, handle);
        }

        public void AddEventListener(int eventId, Action listener, object handle = null)
        {
            if (!events.ContainsKey(eventId))
            {
                events[eventId] = new HashSet<IEventDataBase>();
            }
            EventData eventData = new EventData(eventId, listener, handle);
            events[eventId].Add(eventData);
            
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            AddEventListener(tempName, listener, handle);
        }

        public void AddEventListener(int eventId, Action<object[]> listener, object handle = null)
        {
            if (!events.ContainsKey(eventId))
            {
                events[eventId] = new HashSet<IEventDataBase>();
            }
            EventData<object[]> eventData = new EventData<object[]>(eventId, listener, handle);
            events[eventId].Add(eventData);
            
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName, listener, handle);
        }

        public void RemoveEventListener(int eventId, Action listener, object handle = null)
        {
            if (events.ContainsKey(eventId))
            {
                if (events[eventId].Count > 0)
                {
                    HashSet<IEventDataBase> ebs = events[eventId];
                    if (ebs.Count < 0) {
                        return;
                    }

                    delects.Clear();

                    foreach (var item in ebs)
                    {
                        if (item is EventData eb && eb.Listener == listener && eb.Handle == handle)
                        {
                            MessageManager.Instance.RemoveEventListener(eventId, eb.Listener, eb.Handle);
                            delects.Add(eb);
                        }
                    }
                    
                    foreach (var deletion in delects)
                    {
                        ebs.Remove(deletion);
                    }

                    delects.Clear();
                    
                    
                }
            }
        }

        public void RemoveEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName, listener, handle);
        }

        public void RemoveEventListener(int eventId, Action<object[]> listener, object handle = null)
        {
            if (events.ContainsKey(eventId))
            {
                if (events[eventId].Count > 0)
                {
                    HashSet<IEventDataBase> ebs = events[eventId];
                    if (ebs.Count < 0) {
                        LogF8.Log("不可能为零");
                        return;
                    }
                    
                    delects.Clear();

                    foreach (var item in ebs)
                    {
                        if (item is EventData<object[]> eb && eb.Listener == listener && eb.Handle == handle)
                        {
                            MessageManager.Instance.RemoveEventListener(eventId, eb.Listener, eb.Handle);
                            delects.Add(eb);
                        }
                    }
                    
                    foreach (var deletion in delects)
                    {
                        ebs.Remove(deletion);
                    }

                    delects.Clear();
                }
            }
        }

        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener<T>(T eventName)
        {
            int tempName = (int)(object)eventName;
            RemoveEventListener(tempName);
        }
        
        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener(int eventId)
        {
            if (events.ContainsKey(eventId))
            {
                if (events[eventId].Count > 0)
                {
                    events.Remove(eventId);
                    MessageManager.Instance.RemoveEventListener(eventId);
                }
            }
        }
        
        public void DispatchEvent<T>(T eventName) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName);
        }

        public void DispatchEvent(int eventId)
        {
            MessageManager.Instance.DispatchEvent(eventId);
        }

        public void DispatchEvent<T>(T eventName, params object[] arg1) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1);
        }

        public void DispatchEvent(int eventId, params object[] arg1)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1);
        }

        public void Clear()
        {
            foreach (var eventSet in events.Values)
            {
                foreach (var eventData in eventSet)
                {
                    if (eventData is IEventData or IEventData<object[]>)
                    {
                        if (eventData is IEventData typedEventData)
                        {
                            MessageManager.Instance.RemoveEventListener(typedEventData.GetEvent(), typedEventData.Listener, typedEventData.Handle);
                        }
                        else if (eventData is IEventData<object[]> typedEventData1)
                        {
                            MessageManager.Instance.RemoveEventListener(typedEventData1.GetEvent(), typedEventData1.Listener, typedEventData1.Handle);
                        }
                    }
                }
            }
            events.Clear();
            delects.Clear();
        }
    }
}