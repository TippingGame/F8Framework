using System;
using UnityEngine;
using Object = UnityEngine.Object;
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
    internal sealed class Poolable
    {
        internal F8GameObjectPool _pool;
        internal Transform _transform;
        internal GameObject _gameObject;
        internal PoolableStatus _status;
        internal bool _isSetup;
        
        internal void SetupAsDefault()
        {
#if DEBUG
            if (_isSetup)
                LogF8.LogError("池对象已经设置！");
#endif
            GameObjectPool.Instance.ClonesMap.Add(_gameObject, this);
            _status = PoolableStatus.Despawned;
            _isSetup = true;
        }
        
        internal void SetupAsSpawnedOverCapacity()
        {
#if DEBUG
            if (_isSetup)
                LogF8.LogError("池对象已经设置！");
#endif
            GameObjectPool.Instance.ClonesMap.Add(_gameObject, this);
            _status = PoolableStatus.SpawnedOverCapacity;
            _isSetup = true;
        }

        internal void Dispose(bool immediately)
        {
            GameObjectPool.Instance.ClonesMap.Remove(_gameObject);
            
            if (immediately)
                Object.DestroyImmediate(_gameObject);
            else
                Object.Destroy(_gameObject);
        }
    }
}