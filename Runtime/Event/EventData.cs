using System;

namespace F8Framework.Core
{ 
    public interface IEventDataBase
    {
        public string LogDebugInfo();
    }
    
    public interface IEventData : IEventDataBase
    {
        Enum GetEvent();
        Action Listener { get; }
        object Handle { get; }
    }
    
    public interface IEventData<in T1> : IEventDataBase
    {
        Enum GetEvent();
        Action<T1> Listener { get; }
        object Handle { get; }
    }
    
    public class EventData<TEnum> : IEventData where TEnum : Enum, IConvertible
    {
        public TEnum Event;
        public Action Listener;
        public object Handle;

        public EventData(TEnum eventName, Action listener, object handle = null)
        {
            Event = eventName;
            Listener = listener;
            Handle = handle;
        }
        public Enum GetEvent()
        {
            return Event as Enum;
        }
        Action IEventData.Listener => Listener;
        object IEventData.Handle => Handle;
        
        public string LogDebugInfo()
        {
            return "【" + Event + "】【" + Listener.Method.Name + "】";
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            EventData<TEnum> other = (EventData<TEnum>)obj;
            return Event.Equals(other.Event) && Listener.Equals(other.Listener);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Event.GetHashCode();
                hash = hash * 23 + Listener.GetHashCode();
                return hash;
            }
        }
    }
    
    public class EventData<TEnum, T2> : IEventData<T2> where TEnum : Enum, IConvertible
    {
        public TEnum Event;
        public Action<T2> Listener;
        public object Handle;

        public EventData(TEnum eventName, Action<T2> listener, object handle = null)
        {
            Event = eventName;
            Listener = listener;
            Handle = handle;
        }
        public Enum GetEvent()
        {
            return Event as Enum;
        }
        Action<T2> IEventData<T2>.Listener => Listener;
        object IEventData<T2>.Handle => Handle;
        
        public string LogDebugInfo()
        {
              return "【" + Event + "】【" + Listener.Method.Name + "】";
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            EventData<TEnum, T2> other = (EventData<TEnum, T2>)obj;
            return Event.Equals(other.Event) && Listener.Equals(other.Listener);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Event.GetHashCode();
                hash = hash * 23 + Listener.GetHashCode();
                return hash;
            }
        }
    }
}
