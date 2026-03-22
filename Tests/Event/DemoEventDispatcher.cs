using System;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoEventDispatcher : MonoBehaviour
    {
        private object[] data = new object[] { 123123, "asdasd" };

        private void Awake()
        {
            AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
            AddEventListener(10001, OnPlayerSpawned2);
            AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC);
        }

        private void Start()
        {
            DispatchEvent(MessageEvent.ApplicationFocus);
            DispatchEvent(10001, data);
            DispatchEvent(10002, 123123, "asdasd");
            //可不执行，OnDestroy时会清理此脚本所有监听
            RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
            RemoveEventListener(10001, OnPlayerSpawned2);
            RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC);
        }

        private void OnPlayerSpawned()
        {
            LogF8.Log("OnPlayerSpawned");
        }

        private void OnPlayerSpawned2(params object[] obj)
        {
            LogF8.Log("OnPlayerSpawned2");
            if (obj is { Length: > 0 })
            {
                LogF8.Log(obj[0]);
                LogF8.Log(obj[1]);
            }
        }

        private void OnPlayerSpawnedNoGC(int id, string name)
        {
            LogF8.Log("OnPlayerSpawnedNoGC");
            LogF8.Log(id);
            LogF8.Log(name);
        }

        private EventDispatcher _eventDispatcher = null;

        public EventDispatcher EventDispatcher
        {
            get
            {
                if (_eventDispatcher == null)
                {
                    _eventDispatcher = new EventDispatcher();
                }

                return _eventDispatcher;
            }
        }

        public void AddEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }

        public void AddEventListener<T>(T eventName, Action<object[]> listener) where T : struct, Enum, IConvertible
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

        public void RemoveEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }

        public void RemoveEventListener<T>(T eventName, Action<object[]> listener) where T : struct, Enum, IConvertible
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

        public void DispatchEvent<T>(T eventName) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName);
        }

        public void DispatchEvent<T>(T eventName, params object[] arg1) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1);
        }

        public void DispatchEvent<T, T1>(T eventName, T1 arg1) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1);
        }

        public void DispatchEvent<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2);
        }


        public void AddEventListener(int eventId, Action listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }

        public void AddEventListener(int eventId, Action<object[]> listener)
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

        public void RemoveEventListener(int eventId, Action listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }

        public void RemoveEventListener(int eventId, Action<object[]> listener)
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

        public void DispatchEvent(int eventId)
        {
            EventDispatcher.DispatchEvent(eventId);
        }

        public void DispatchEvent(int eventId, params object[] arg1)
        {
            EventDispatcher.DispatchEvent(eventId, arg1);
        }

        public void DispatchEvent<T1>(int eventId, T1 arg1)
        {
            EventDispatcher.DispatchEvent(eventId, arg1);
        }

        public void DispatchEvent<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2);
        }

        void OnDestroy()
        {
            if (_eventDispatcher != null)
            {
                _eventDispatcher.Clear();
                _eventDispatcher = null;
            }
        }
    }
}
