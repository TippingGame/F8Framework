using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class BaseItem : ComponentBind
    {
        // 消息事件
        private EventDispatcher _eventDispatcher = null;
                
        public EventDispatcher EventDispatcher {
            get
            {
                if (_eventDispatcher == null)
                {
                    _eventDispatcher = new EventDispatcher();
                }
        
                return _eventDispatcher;
            }
        }
        
        public void ClearEventDispatcher()
        {
            if (_eventDispatcher != null) {
                _eventDispatcher.Clear();
                _eventDispatcher = null;
            }
        }
        
        private void Awake()
        {
            OnAwake();
            OnViewTweenInit();
            OnAddUIComponentListener();
        }

        protected virtual void OnAwake()
        {
        }
        
        protected virtual void OnAddUIComponentListener()
        {
        }
        
        private void Start()
        {
            OnStart();
            OnPlayViewTween();
        }
        
        protected virtual void OnStart()
        {
        }
        
        protected virtual void OnViewTweenInit()
        {
        }
        
        protected virtual void OnPlayViewTween()
        {
            OnViewOpen();
        }
        
        protected virtual void OnViewOpen()
        {
        }
        
        protected virtual void ButtonClick(UIBehaviour ui)
        {
        }
        
        protected virtual void ValueChange<T>(UIBehaviour ui, T value)
        {
        }
        
        public void AddEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void AddEventListener<T, T1>(T eventName, Action<T1> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T, T1>(T eventName, Action<T1> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void DispatchEvent<T>(T eventName) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName);
        }

        public Coroutine DispatchEventAsync<T>(T eventName) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName);
        }
            
        public void DispatchEvent<T, T1>(T eventName, T1 arg1) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1);
        }

        public Coroutine DispatchEventAsync<T, T1>(T eventName, T1 arg1) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1);
        }

        public void DispatchEvent<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2);
        }

        public Coroutine DispatchEventAsync<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2);
        }

        public void DispatchEvent<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3);
        }

        public void DispatchEvent<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : struct, Enum, IConvertible
        {
            return EventDispatcher.DispatchEventAsync(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        
        public void AddEventListener(int eventId, Action listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void AddEventListener<T1>(int eventId, Action<T1> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2>(int eventId, Action<T1, T2> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener(int eventId, Action listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener<T1>(int eventId, Action<T1> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2>(int eventId, Action<T1, T2> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void DispatchEvent(int eventId)
        {
            EventDispatcher.DispatchEvent(eventId);
        }

        public Coroutine DispatchEventAsync(int eventId)
        {
            return EventDispatcher.DispatchEventAsync(eventId);
        }
            
        public void DispatchEvent<T1>(int eventId, T1 arg1)
        {
            EventDispatcher.DispatchEvent(eventId, arg1);
        }

        public Coroutine DispatchEventAsync<T1>(int eventId, T1 arg1)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1);
        }

        public void DispatchEvent<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2);
        }

        public Coroutine DispatchEventAsync<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2);
        }

        public void DispatchEvent<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3);
        }

        public void DispatchEvent<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public Coroutine DispatchEventAsync<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return EventDispatcher.DispatchEventAsync(eventId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        
        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener<T>(T eventName)
        {
            int tempName = (int)(object)eventName;
            EventDispatcher.RemoveEventListener(tempName);
        }
        
        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener(int eventId)
        {
            EventDispatcher.RemoveEventListener(eventId);
        }
    }
}
