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

    public interface IInvokableEventData
    {
        void Invoke();
    }

    public interface IInvokableEventData<T1>
    {
        void Invoke(T1 arg1);
    }

    public interface IInvokableEventData<T1, T2>
    {
        void Invoke(T1 arg1, T2 arg2);
    }

    public interface IInvokableEventData<T1, T2, T3>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInvokableEventData<T1, T2, T3, T4>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IInvokableEventData<T1, T2, T3, T4, T5>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IInvokableEventData<T1, T2, T3, T4, T5, T6>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IInvokableEventData<T1, T2, T3, T4, T5, T6, T7>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
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

    public class EventData : EventDataBase, IInvokableEventData
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

        public void Invoke()
        {
            Listener?.Invoke();
        }
    }

    public class EventData<T1> : EventDataBase, IInvokableEventData, IInvokableEventData<T1>
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

        public void Invoke()
        {
            Listener?.Invoke(default);
        }

        public void Invoke(T1 arg1)
        {
            Listener?.Invoke(arg1);
        }
    }

    public class EventData<T1, T2> : EventDataBase, IInvokableEventData, IInvokableEventData<T1>, IInvokableEventData<T1, T2>
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

        public void Invoke()
        {
            Listener?.Invoke(default, default);
        }

        public void Invoke(T1 arg1)
        {
            Listener?.Invoke(arg1, default);
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Listener?.Invoke(arg1, arg2);
        }
    }

    public class EventData<T1, T2, T3> : EventDataBase, IInvokableEventData, IInvokableEventData<T1>, IInvokableEventData<T1, T2>, IInvokableEventData<T1, T2, T3>
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

        public void Invoke()
        {
            Listener?.Invoke(default, default, default);
        }

        public void Invoke(T1 arg1)
        {
            Listener?.Invoke(arg1, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Listener?.Invoke(arg1, arg2, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            Listener?.Invoke(arg1, arg2, arg3);
        }
    }

    public class EventData<T1, T2, T3, T4> : EventDataBase, IInvokableEventData, IInvokableEventData<T1>, IInvokableEventData<T1, T2>, IInvokableEventData<T1, T2, T3>, IInvokableEventData<T1, T2, T3, T4>
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

        public void Invoke()
        {
            Listener?.Invoke(default, default, default, default);
        }

        public void Invoke(T1 arg1)
        {
            Listener?.Invoke(arg1, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Listener?.Invoke(arg1, arg2, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            Listener?.Invoke(arg1, arg2, arg3, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4);
        }
    }

    public class EventData<T1, T2, T3, T4, T5> : EventDataBase, IInvokableEventData, IInvokableEventData<T1>, IInvokableEventData<T1, T2>, IInvokableEventData<T1, T2, T3>, IInvokableEventData<T1, T2, T3, T4>, IInvokableEventData<T1, T2, T3, T4, T5>
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

        public void Invoke()
        {
            Listener?.Invoke(default, default, default, default, default);
        }

        public void Invoke(T1 arg1)
        {
            Listener?.Invoke(arg1, default, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Listener?.Invoke(arg1, arg2, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            Listener?.Invoke(arg1, arg2, arg3, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
    }

    public class EventData<T1, T2, T3, T4, T5, T6> : EventDataBase, IInvokableEventData, IInvokableEventData<T1>, IInvokableEventData<T1, T2>, IInvokableEventData<T1, T2, T3>, IInvokableEventData<T1, T2, T3, T4>, IInvokableEventData<T1, T2, T3, T4, T5>, IInvokableEventData<T1, T2, T3, T4, T5, T6>
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

        public void Invoke()
        {
            Listener?.Invoke(default, default, default, default, default, default);
        }

        public void Invoke(T1 arg1)
        {
            Listener?.Invoke(arg1, default, default, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Listener?.Invoke(arg1, arg2, default, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            Listener?.Invoke(arg1, arg2, arg3, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, arg5, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }

    public class EventData<T1, T2, T3, T4, T5, T6, T7> : EventDataBase, IInvokableEventData, IInvokableEventData<T1>, IInvokableEventData<T1, T2>, IInvokableEventData<T1, T2, T3>, IInvokableEventData<T1, T2, T3, T4>, IInvokableEventData<T1, T2, T3, T4, T5>, IInvokableEventData<T1, T2, T3, T4, T5, T6>, IInvokableEventData<T1, T2, T3, T4, T5, T6, T7>
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

        public void Invoke()
        {
            Listener?.Invoke(default, default, default, default, default, default, default);
        }

        public void Invoke(T1 arg1)
        {
            Listener?.Invoke(arg1, default, default, default, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Listener?.Invoke(arg1, arg2, default, default, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            Listener?.Invoke(arg1, arg2, arg3, default, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, default, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, arg5, default, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, default);
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Listener?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }
}
