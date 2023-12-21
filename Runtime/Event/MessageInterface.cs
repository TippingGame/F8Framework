using System;

public interface MessageInterface
{
    void AddEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible;
    void AddEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible;
    void RemoveEventListener<T>(T eventName, Action listener, object handle = null) where T : Enum, IConvertible;
    void RemoveEventListener<T>(T eventName, Action<object[]> listener, object handle = null) where T : Enum, IConvertible;
    void DispatchEvent<T>(T eventName) where T : Enum, IConvertible;
    void DispatchEvent<T>(T eventName, params object[] arg1) where T : Enum, IConvertible;
    void Clear();
}