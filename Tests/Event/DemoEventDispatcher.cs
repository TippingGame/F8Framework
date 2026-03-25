using System;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoEventDispatcher : MonoBehaviour
    {
        private void Awake()
        {
            AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
            AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC);
            AddEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnPlayerSpawnedNoGC);
            AddEventListener<int, string, bool, float, long, byte, char>(10004, OnPlayerSpawnedT7);
        }

        private void Start()
        {
            DispatchEvent(MessageEvent.ApplicationFocus);
            DispatchEvent(10002, 123123, "asdasd");
            DispatchEvent(MessageEvent.ApplicationFocus, 123123, "asdasd");
            DispatchEvent(10004, 123123, "asdasd", true, 1.5f, 999L, (byte)7, 'F');
            //可不执行，OnDestroy时会清理此脚本所有监听
            RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
            RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC);
            RemoveEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnPlayerSpawnedNoGC);
            RemoveEventListener<int, string, bool, float, long, byte, char>(10004, OnPlayerSpawnedT7);
        }

        private void OnPlayerSpawned()
        {
            LogF8.Log("OnPlayerSpawned");
        }

        private void OnPlayerSpawnedNoGC(int id, string name)
        {
            LogF8.Log("OnPlayerSpawnedNoGC");
            LogF8.Log(id);
            LogF8.Log(name);
        }

        private void OnPlayerSpawnedT7(int id, string name, bool active, float speed, long score, byte level, char rank)
        {
            LogF8.Log("OnPlayerSpawnedT7");
            LogF8.Log(id);
            LogF8.Log(name);
            LogF8.Log(active);
            LogF8.Log(speed);
            LogF8.Log(score);
            LogF8.Log(level);
            LogF8.Log(rank);
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

        public void DispatchEvent<T, T1>(T eventName, T1 arg1) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1);
        }

        public void DispatchEvent<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2);
        }

        public void DispatchEvent<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3);
        }

        public void DispatchEvent<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
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

        public void DispatchEvent<T1>(int eventId, T1 arg1)
        {
            EventDispatcher.DispatchEvent(eventId, arg1);
        }

        public void DispatchEvent<T1, T2>(int eventId, T1 arg1, T2 arg2)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2);
        }

        public void DispatchEvent<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3);
        }

        public void DispatchEvent<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void DispatchEvent<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            EventDispatcher.DispatchEvent(eventId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
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
