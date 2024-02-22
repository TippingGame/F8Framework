using System;

namespace F8Framework.Core
{
    public interface IMessageManager
    {
        void AddEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible;
        void AddEventListener(int eventId, Action listener, object handle = null);

        void AddEventListener<T>(T eventName, Action<object[]> listener, object handle = null)
            where T : Enum, IConvertible;
        void AddEventListener(int eventId, Action<object[]> listener, object handle = null);

        void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible;
        void RemoveEventListener(int eventId, Action listener, object handle = null);

        void RemoveEventListener<T>(T eventName, Action<object[]> listener, object handle = null)
            where T : Enum, IConvertible;
        void RemoveEventListener(int eventId, Action<object[]> listener, object handle = null);

        void DispatchEvent<T>(T eventName) where T : Enum, IConvertible;
        void DispatchEvent(int eventId);
        void DispatchEvent<T>(T eventName, params object[] arg1) where T : Enum, IConvertible;
        void DispatchEvent(int eventId, params object[] arg1);
        void Clear();
    }
}