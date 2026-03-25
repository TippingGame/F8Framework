using System;

namespace F8Framework.Core
{
    public interface IEventDataBase
    {
        string LogDebugInfo();
        bool EventDataShouldBeInvoked();
        void RemoveFrom(MessageManager manager);
    }

    public interface IEventData : IEventDataBase
    {
        int GetEvent();
        Delegate GetListener();
        object Handle { get; }
    }

    public abstract class EventDataBase : IEventData
    {
        public int Event;
        public object Handle;

        protected EventDataBase(int eventName, object handle = null)
        {
            Event = eventName;
            Handle = handle;
        }

        public int GetEvent()
        {
            return Event;
        }

        object IEventData.Handle => Handle;

        public abstract Delegate GetListener();

        public string LogDebugInfo()
        {
            var listener = GetListener();
            return "【" + Event + "】【" + listener?.Target + "】【" + listener?.Method.Name + "】";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (EventDataBase)obj;
            return Event == other.Event && Equals(GetListener(), other.GetListener());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Event.GetHashCode();
                hash = hash * 23 + (GetListener()?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public bool EventDataShouldBeInvoked()
        {
            if (Handle == null || Handle.Equals(null))
            {
                return false;
            }

            return true;
        }

        public abstract void RemoveFrom(MessageManager manager);
    }

    public class EventData : EventDataBase
    {
        public Action Listener;

        public EventData(int eventName, Action listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }

    public class EventData<T1> : EventDataBase
    {
        public Action<T1> Listener;

        public EventData(int eventName, Action<T1> listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }

    public class EventData<T1, T2> : EventDataBase
    {
        public Action<T1, T2> Listener;

        public EventData(int eventName, Action<T1, T2> listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }

    public class EventData<T1, T2, T3> : EventDataBase
    {
        public Action<T1, T2, T3> Listener;

        public EventData(int eventName, Action<T1, T2, T3> listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }

    public class EventData<T1, T2, T3, T4> : EventDataBase
    {
        public Action<T1, T2, T3, T4> Listener;

        public EventData(int eventName, Action<T1, T2, T3, T4> listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }

    public class EventData<T1, T2, T3, T4, T5> : EventDataBase
    {
        public Action<T1, T2, T3, T4, T5> Listener;

        public EventData(int eventName, Action<T1, T2, T3, T4, T5> listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }

    public class EventData<T1, T2, T3, T4, T5, T6> : EventDataBase
    {
        public Action<T1, T2, T3, T4, T5, T6> Listener;

        public EventData(int eventName, Action<T1, T2, T3, T4, T5, T6> listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }

    public class EventData<T1, T2, T3, T4, T5, T6, T7> : EventDataBase
    {
        public Action<T1, T2, T3, T4, T5, T6, T7> Listener;

        public EventData(int eventName, Action<T1, T2, T3, T4, T5, T6, T7> listener, object handle = null) : base(eventName, handle)
        {
            Listener = listener;
        }

        public override Delegate GetListener()
        {
            return Listener;
        }

        public override void RemoveFrom(MessageManager manager)
        {
            manager.RemoveEventListener(Event, Listener, Handle);
        }
    }
}
