using System;

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
        
        private void Awake()
        {
            OnAwake();
            OnViewTweenInit();
        }

        protected virtual void OnAwake()
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
        
        public void AddEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void AddEventListener<T>(T eventName, Action<object[]> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T>(T eventName, Action<object[]> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void DispatchEvent<T>(T eventName) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName);
        }
            
        public void DispatchEvent<T>(T eventName, params object[] arg1) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1);
        }
        
        public void AddEventListener(int eventId, Action listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void AddEventListener(int eventId, Action<object[]> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener(int eventId, Action listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener(int eventId, Action<object[]> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void DispatchEvent(int eventId)
        {
            EventDispatcher.DispatchEvent(eventId);
        }
            
        public void DispatchEvent(int eventId, params object[] arg1)
        {
            EventDispatcher.DispatchEvent(eventId, arg1);
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
