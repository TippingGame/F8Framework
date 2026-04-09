using System;
using UnityEngine;

namespace F8Framework.Core
{
    public interface IMessageManager
    {
        void AddEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible;
        void AddEventListener(int eventId, Action listener, object handle = null);

        void AddEventListener<T, T1>(T eventName, Action<T1> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener<T1>(int eventId, Action<T1> listener, object handle = null);

        void AddEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener<T1, T2>(int eventId, Action<T1, T2> listener, object handle = null);

        void AddEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener, object handle = null);

        void AddEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener, object handle = null);

        void AddEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener, object handle = null);

        void AddEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null);

        void AddEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null);

        void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible;
        void RemoveEventListener(int eventId, Action listener, object handle = null);

        void RemoveEventListener<T, T1>(T eventName, Action<T1> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener<T1>(int eventId, Action<T1> listener, object handle = null);

        void RemoveEventListener<T, T1, T2>(T eventName, Action<T1, T2> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener<T1, T2>(int eventId, Action<T1, T2> listener, object handle = null);

        void RemoveEventListener<T, T1, T2, T3>(T eventName, Action<T1, T2, T3> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> listener, object handle = null);

        void RemoveEventListener<T, T1, T2, T3, T4>(T eventName, Action<T1, T2, T3, T4> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener<T1, T2, T3, T4>(int eventId, Action<T1, T2, T3, T4> listener, object handle = null);

        void RemoveEventListener<T, T1, T2, T3, T4, T5>(T eventName, Action<T1, T2, T3, T4, T5> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener<T1, T2, T3, T4, T5>(int eventId, Action<T1, T2, T3, T4, T5> listener, object handle = null);

        void RemoveEventListener<T, T1, T2, T3, T4, T5, T6>(T eventName, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener<T1, T2, T3, T4, T5, T6>(int eventId, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null);

        void RemoveEventListener<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7>(int eventId, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null);

        void DispatchEvent<T>(T eventName) where T : Enum, IConvertible;
        void DispatchEvent(int eventId);
        Coroutine DispatchEventAsync<T>(T eventName) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync(int eventId);
        void DispatchEvent<T, T1>(T eventName, T1 arg1) where T : Enum, IConvertible;
        void DispatchEvent<T1>(int eventId, T1 arg1);
        Coroutine DispatchEventAsync<T, T1>(T eventName, T1 arg1) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync<T1>(int eventId, T1 arg1);
        void DispatchEvent<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : Enum, IConvertible;
        void DispatchEvent<T1, T2>(int eventId, T1 arg1, T2 arg2);
        Coroutine DispatchEventAsync<T, T1, T2>(T eventName, T1 arg1, T2 arg2) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync<T1, T2>(int eventId, T1 arg1, T2 arg2);
        void DispatchEvent<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : Enum, IConvertible;
        void DispatchEvent<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3);
        Coroutine DispatchEventAsync<T, T1, T2, T3>(T eventName, T1 arg1, T2 arg2, T3 arg3) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync<T1, T2, T3>(int eventId, T1 arg1, T2 arg2, T3 arg3);
        void DispatchEvent<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : Enum, IConvertible;
        void DispatchEvent<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        Coroutine DispatchEventAsync<T, T1, T2, T3, T4>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync<T1, T2, T3, T4>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        void DispatchEvent<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : Enum, IConvertible;
        void DispatchEvent<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync<T1, T2, T3, T4, T5>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        void DispatchEvent<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : Enum, IConvertible;
        void DispatchEvent<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5, T6>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync<T1, T2, T3, T4, T5, T6>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        void DispatchEvent<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : Enum, IConvertible;
        void DispatchEvent<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        Coroutine DispatchEventAsync<T, T1, T2, T3, T4, T5, T6, T7>(T eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) where T : Enum, IConvertible;
        Coroutine DispatchEventAsync<T1, T2, T3, T4, T5, T6, T7>(int eventId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        void Clear();
    }
}
