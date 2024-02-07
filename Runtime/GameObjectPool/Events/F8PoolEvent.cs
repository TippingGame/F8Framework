using System;
using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace F8Framework.Core
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public class F8PoolEvent<T>
    {
        private Action<T> _action;
        private bool _hasAction;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddListener(Action<T> action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            _action += action;
            _hasAction = _action != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveListener(Action<T> action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            _action -= action;
            _hasAction = _action != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _action = null;
            _hasAction = false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseEvent(T objectToRaise)
        {
            if (_hasAction)
            {
                _action.Invoke(objectToRaise);
            }
        }
    }
}