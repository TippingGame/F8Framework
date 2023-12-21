using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class EventDispatcher : MessageInterface
    {
        private Dictionary<int, HashSet<IEventDataBase>> events = new Dictionary<int, HashSet<IEventDataBase>>();
        public void AddEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (!events.ContainsKey(tempName))
            {
                events[tempName] = new HashSet<IEventDataBase>();
            }
            EventData<T> eventData = new EventData<T>(eventName, listener, handle);
            events[tempName].Add(eventData);
            
            MessageManager.Instance.AddEventListener(eventName, listener, handle);
        }

        public void AddEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (!events.ContainsKey(tempName))
            {
                events[tempName] = new HashSet<IEventDataBase>();
            }
            EventData<T,object[]> eventData = new EventData<T,object[]>(eventName, listener, handle);
            events[tempName].Add(eventData);
            
            MessageManager.Instance.AddEventListener(eventName, listener, handle);
        }

        public void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (events.ContainsKey(tempName))
            {
                if (events[tempName].Count > 0)
                {
                    HashSet<IEventDataBase> ebs = events[tempName];
                    if (ebs.Count < 0) {
                        return;
                    }

                    foreach (var item in ebs)
                    {
                        if (item is EventData<T> eb)
                        {
                            MessageManager.Instance.RemoveEventListener(eventName, eb.Listener, eb.Handle);
                        }
                    }
                    events.Remove(tempName);
                }
            }
        }

        public void RemoveEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible
        {
            int tempName = (int)(object)eventName;
            if (events.ContainsKey(tempName))
            {
                if (events[tempName].Count > 0)
                {
                    HashSet<IEventDataBase> ebs = events[tempName];
                    if (ebs.Count < 0) {
                        return;
                    }
                    
                    foreach (var item in ebs)
                    {
                        if (item is EventData<T,object[]> eb)
                        {
                            MessageManager.Instance.RemoveEventListener(eventName, eb.Listener, eb.Handle);
                        }
                    }
                    events.Remove(tempName);
                }
            }
        }
        
        public void DispatchEvent<T>(T eventName) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName);
        }

        public void DispatchEvent<T>(T eventName, object[] arg1) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1);
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
                            MessageManager.Instance.RemoveEventListener(typedEventData.GetEvent(), typedEventData.Listener,typedEventData.Handle);
                        }
                        else if (eventData is IEventData<object[]> typedEventData1)
                        {
                            MessageManager.Instance.RemoveEventListener(typedEventData1.GetEvent(), typedEventData1.Listener,typedEventData1.Handle);
                        }
                    }
                }
            }
            events.Clear();
            events = null;
        }
    }
}