using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class EventDispatcher : IMessageManager
    {
        private readonly Dictionary<int, HashSet<IEventDataBase>> events = new Dictionary<int, HashSet<IEventDataBase>>();
        private readonly List<IEventDataBase> delects = new List<IEventDataBase>();

        private HashSet<IEventDataBase> GetOrCreateEventSet(int eventId)
        {
            if (!events.TryGetValue(eventId, out var eventSet))
            {
                eventSet = new HashSet<IEventDataBase>();
                events[eventId] = eventSet;
            }

            return eventSet;
        }

        private void AddEventListenerInternal(int eventId, IEventDataBase eventData)
        {
            GetOrCreateEventSet(eventId).Add(eventData);
        }

        private void RemoveEventListenerInternal<TEventData>(int eventId, Func<TEventData, bool> predicate)
            where TEventData : class, IEventDataBase
        {
            if (!events.TryGetValue(eventId, out var eventSet) || eventSet.Count <= 0)
            {
                return;
            }

            delects.Clear();

            foreach (var item in eventSet)
            {
                if (item is TEventData eventData && predicate(eventData))
                {
                    RemoveFromMessageManager(item);
                    delects.Add(item);
                }
            }

            foreach (var deletion in delects)
            {
                eventSet.Remove(deletion);
            }

            delects.Clear();
        }

        private void RemoveFromMessageManager(IEventDataBase eventData)
        {
            if (eventData == null)
            {
                return;
            }
            eventData.RemoveFrom(MessageManager.Instance);
        }

        public void AddEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener(int eventId, Action listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T, T1>(T eventName, Action<T1> listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1>(int eventId, Action<T1> listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData<T1>(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2>(int eventId, Action<T1, T2> listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2>(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3>(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4>(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4, T5>(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4, T5, T6>(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4, T5, T6, T7>(eventId, listener, handle));
            MessageManager.Instance.AddEventListener(eventId, listener, handle);
        }

        public void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener(int eventId, Action listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T, T1>(T eventName, Action<T1> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1>(int eventId, Action<T1> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1>>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2>(int eventId, Action<T1, T2> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2>>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3>>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4>>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4, T5>>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4, T5, T6>>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4, T5, T6, T7>>(eventId, eb => eb.Listener == listener && eb.Handle == handle);
        }

        public void RemoveEventListener<T>(T eventName)
        {
            RemoveEventListener((int)(object)eventName);
        }

        public void RemoveEventListener(int eventId)
        {
            if (events.ContainsKey(eventId) && events[eventId].Count > 0)
            {
                events.Remove(eventId);
                MessageManager.Instance.RemoveEventListener(eventId);
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

        public void DispatchEvent<T, T1>(T eventName, T1 arg1) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1);
        }

        public void DispatchEvent<T1>(int eventId, T1 arg1)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1);
        }

        public void DispatchEvent<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1, arg2);
        }

        public void DispatchEvent<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1, arg2);
        }

        public void DispatchEvent<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1, arg2, arg3);
        }

        public void DispatchEvent<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1, arg2, arg3);
        }

        public void DispatchEvent<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : Enum, IConvertible
        {
            MessageManager.Instance.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            MessageManager.Instance.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void Clear()
        {
            foreach (var eventSet in events.Values)
            {
                foreach (var eventData in eventSet)
                {
                    RemoveFromMessageManager(eventData);
                }
            }

            events.Clear();
            delects.Clear();
        }
    }
}
