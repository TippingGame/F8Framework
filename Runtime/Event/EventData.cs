using System;

namespace F8Framework.Core
{ 
    public interface IEventData
    {
        Enum GetEvent();
        Action Listener { get; }
        object Object { get; }
    }
    
    public interface IEventData<in T1>
    {
        Enum GetEvent();
        Action<T1> Listener { get; }
        object Object { get; }
    }
    
    public class EventData<TEnum> : IEventData where TEnum : Enum, IConvertible
    {
        public TEnum Event;
        public Action Listener;
        public object Object;

        public EventData(TEnum eventName, Action listener, object obj)
        {
            Event = eventName;
            Listener = listener;
            Object = obj;
        }
        public Enum GetEvent()
        {
            return Event as Enum;
        }
        Action IEventData.Listener => Listener;
        object IEventData.Object => Object;
    }
    
    public class EventData<TEnum, T2> : IEventData<T2> where TEnum : Enum, IConvertible
    {
        public TEnum Event;
        public Action<T2> Listener;
        public object Object;

        public EventData(TEnum eventName, Action<T2> listener, object obj)
        {
            Event = eventName;
            Listener = listener;
            Object = obj;
        }
        public Enum GetEvent()
        {
            return Event as Enum;
        }
        Action<T2> IEventData<T2>.Listener => Listener;
        object IEventData<T2>.Object => Object;
    }
}
