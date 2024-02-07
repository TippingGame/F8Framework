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
    internal sealed class F8PoolList<T>
    {
        internal T[] _components;
        internal int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal F8PoolList(int capacity = 32)
        {
#if DEBUG
            if (capacity <= 0) 
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero!");
#endif
            _components = new T[capacity];
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(in T component)
        {
            if (_count >= _components.Length)
                Array.Resize(ref _components, _components.Length << 1);

            _components[_count++] = component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveUnorderedAt(int id)
        {
            var lastComponentId = _count - 1;
            _components[id] = _components[lastComponentId];
            _components[lastComponentId] = default;
            _count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveAt(int id)
        {
#if DEBUG
            CheckForRemove(id);
#endif
            for (int i = id; i < _count; i++)
            {
                _components[i] = i + 1 < _count ? _components[i + 1] : default;
            }
            
            _count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear()
        {
            Array.Clear(_components, 0, _count);
            _count = 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetCapacity(int capacity)
        {
#if DEBUG
            if (capacity <= 0) 
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero!");
#endif
            if (_components.Length == capacity)
                return;
            
            Array.Resize(ref _components, capacity);

            if (_count > capacity)
                _count = capacity;
        }

#if DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckForRemove(int id)
        {
            if (_count <= id)
                throw new ArgumentOutOfRangeException(nameof(id), "Index is greater than count!");
            
            if (_count <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "List is empty, nothing to remove!");
        }
#endif
    }
}