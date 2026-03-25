using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class MessageManager : ModuleSingletonMono<MessageManager>, IMessageManager, IModule
    {
        internal Dictionary<int, List<IEventDataBase>> events = new Dictionary<int, List<IEventDataBase>>();
        private readonly List<IEventDataBase> delects = new List<IEventDataBase>();
        internal HashSet<IEventDataBase> callStack = new HashSet<IEventDataBase>();
        internal Dictionary<int, Queue<IEventDataBase>> dispatchInvokes = new Dictionary<int, Queue<IEventDataBase>>();

        private void MessageLoop(string debugInfo)
        {
            LogF8.LogError("消息死循环：{0}", debugInfo);
        }

        private void NotActionLog(string eventId, string actionName)
        {
            LogF8.LogEvent("函数不存在：【{0}】【{1}】", eventId, actionName);
        }

        private void NotListenerLog(string debugInfo)
        {
            LogF8.LogEvent("监听者不存在：{0}", debugInfo);
        }

        private void NotEventLogDispatch(string eventId)
        {
            LogF8.LogEvent("没有创建监听，发送事件：【{0}】", eventId);
        }

        private void NotEventLogRemove(string eventId)
        {
            LogF8.LogEvent("没有创建监听，移除监听：【{0}】", eventId);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                DispatchEvent(MessageEvent.ApplicationFocus);
            }
            else
            {
                DispatchEvent(MessageEvent.NotApplicationFocus);
            }
        }

        private void OnApplicationQuit()
        {
            DispatchEvent(MessageEvent.ApplicationQuit);
        }

        private void ClearCallStack()
        {
            callStack.Clear();
        }

        private bool IsInCallStack(IEventDataBase eventData)
        {
            return callStack.Contains(eventData);
        }

        private List<IEventDataBase> GetOrCreateEventList(int eventId)
        {
            if (!events.TryGetValue(eventId, out var eventList))
            {
                eventList = new List<IEventDataBase>();
                events[eventId] = eventList;
            }

            return eventList;
        }

        private void AddEventListenerInternal(int eventId, IEventDataBase eventData)
        {
            var eventList = GetOrCreateEventList(eventId);
            if (eventList.Contains(eventData))
            {
                LogF8.LogEvent("【{0}】不能允许存在重复的事件处理函数。", eventId);
                return;
            }

            eventList.Add(eventData);
        }

        private void RemoveEventListenerInternal<TEventData>(int eventId, Func<TEventData, bool> predicate, string actionName = null)
            where TEventData : class, IEventDataBase
        {
            if (!events.TryGetValue(eventId, out var eventList))
            {
                NotEventLogRemove(eventId.ToString());
                return;
            }

            if (eventList.Count == 0)
            {
                if (!string.IsNullOrEmpty(actionName))
                {
                    NotActionLog(eventId.ToString(), actionName);
                }
                return;
            }

            delects.Clear();

            foreach (var itemObj in eventList)
            {
                if (itemObj is TEventData eventData && predicate(eventData))
                {
                    if (eventData is EventDataBase eventDataBase)
                    {
                        eventDataBase.Handle = null;
                    }
                    delects.Add(itemObj);
                }
            }

            foreach (var deletion in delects)
            {
                eventList.Remove(deletion);
            }

            delects.Clear();
        }

        private Queue<IEventDataBase> GetOrCreateDispatchQueue(int eventId)
        {
            if (!dispatchInvokes.TryGetValue(eventId, out var queue))
            {
                queue = new Queue<IEventDataBase>();
                dispatchInvokes[eventId] = queue;
            }

            return queue;
        }

        private Queue<IEventDataBase> CollectDispatchTargets(int eventId)
        {
            if (!events.TryGetValue(eventId, out var eventDatas))
            {
                NotEventLogDispatch(eventId.ToString());
                return null;
            }

            var queue = GetOrCreateDispatchQueue(eventId);

            foreach (var obj in eventDatas)
            {
                if (IsInCallStack(obj))
                {
                    MessageLoop(obj.LogDebugInfo());
                    continue;
                }

                if (obj.EventDataShouldBeInvoked())
                {
                    queue.Enqueue(obj);
                    callStack.Add(obj);
                }
                else
                {
                    NotListenerLog(obj.LogDebugInfo());
                }
            }

            return queue;
        }

        private void FinishDispatch()
        {
            ClearCallStack();
        }

        public void AddEventListener<T>(T eventName, Action listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener(int eventId, Action listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData(eventId, listener, handle));
        }

        public void AddEventListener<T, T1>(T eventName, Action<T1> listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1>(int eventId, Action<T1> listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData<T1>(eventId, listener, handle));
        }

        public void AddEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2>(int eventId, Action<T1, T2> listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2>(eventId, listener, handle));
        }

        public void AddEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3>(eventId, listener, handle));
        }

        public void AddEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4>(eventId, listener, handle));
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4, T5>(eventId, listener, handle));
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4, T5, T6>(eventId, listener, handle));
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle) where T : Enum, IConvertible
        {
            AddEventListener((int)(object)eventName, listener, handle);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle)
        {
            AddEventListenerInternal(eventId, new EventData<T1, T2, T3, T4, T5, T6, T7>(eventId, listener, handle));
        }

        public void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener(int eventId, Action listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
        }

        public void RemoveEventListener<T, T1>(T eventName, Action<T1> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1>(int eventId, Action<T1> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1>>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
        }

        public void RemoveEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2>(int eventId, Action<T1, T2> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2>>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
        }

        public void RemoveEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3>>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4>>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4, T5>>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4, T5, T6>>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null) where T : Enum, IConvertible
        {
            RemoveEventListener((int)(object)eventName, listener, handle);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null)
        {
            RemoveEventListenerInternal<EventData<T1, T2, T3, T4, T5, T6, T7>>(eventId, eventData => eventData.Listener == listener && eventData.Handle == handle, listener?.Method.Name);
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
            }
        }

        public void DispatchEvent<T>(T eventName) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName);
        }

        public void DispatchEvent(int eventId)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData eventData)
                {
                    eventData.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void DispatchEvent<T, T1>(T eventName, T1 arg1) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName, arg1);
        }

        public void DispatchEvent<T1>(int eventId, T1 arg1)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData<T1> eventData)
                {
                    eventData.Listener?.Invoke(arg1);
                }
                else if (obj is EventData eventData0)
                {
                    eventData0.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void DispatchEvent<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName, arg1, arg2);
        }

        public void DispatchEvent<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData<T1, T2> eventData)
                {
                    eventData.Listener?.Invoke(arg1, arg2);
                }
                else if (obj is EventData eventData0)
                {
                    eventData0.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void DispatchEvent<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName, arg1, arg2, arg3);
        }

        public void DispatchEvent<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData<T1, T2, T3> eventData)
                {
                    eventData.Listener?.Invoke(arg1, arg2, arg3);
                }
                else if (obj is EventData eventData0)
                {
                    eventData0.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void DispatchEvent<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData<T1, T2, T3, T4> eventData)
                {
                    eventData.Listener?.Invoke(arg1, arg2, arg3, arg4);
                }
                else if (obj is EventData eventData0)
                {
                    eventData0.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData<T1, T2, T3, T4, T5> eventData)
                {
                    eventData.Listener?.Invoke(arg1, arg2, arg3, arg4, arg5);
                }
                else if (obj is EventData eventData0)
                {
                    eventData0.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData<T1, T2, T3, T4, T5, T6> eventData)
                {
                    eventData.Listener?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
                }
                else if (obj is EventData eventData0)
                {
                    eventData0.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : Enum, IConvertible
        {
            DispatchEvent((int)(object)eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var queue = CollectDispatchTargets(eventId);
            if (queue == null)
            {
                return;
            }

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj is EventData<T1, T2, T3, T4, T5, T6, T7> eventData)
                {
                    eventData.Listener?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                }
                else if (obj is EventData eventData0)
                {
                    eventData0.Listener?.Invoke();
                }
            }

            FinishDispatch();
        }

        public void Clear()
        {
            events.Clear();
            delects.Clear();
            callStack.Clear();
            dispatchInvokes.Clear();
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
            Destroy(gameObject);
        }
    }
}
