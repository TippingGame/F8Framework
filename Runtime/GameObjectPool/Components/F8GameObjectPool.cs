using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace F8Framework.Core
{
    /// <summary>
    /// Gives you control over the spawning and despawning clones.
    /// </summary>
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class F8GameObjectPool : MonoBehaviour
    {
        [Header("Main")]
        [Tooltip("此池的预设物体。")]
        [SerializeField] internal GameObject _prefab;
        [Tooltip(Constants.Tooltips.OverflowBehaviour)]
        [SerializeField] internal BehaviourOnCapacityReached _behaviourOnCapacityReached = Constants.DefaultBehaviourOnCapacityReached;
        [Tooltip(Constants.Tooltips.DespawnType)]
        [SerializeField] internal DespawnType _despawnType = Constants.DefaultDespawnType;
        [Tooltip("此池的容量。")]
        [SerializeField, Delayed, Min(0)] private int _capacity = 32;

        [Header("Preload")]
        [Tooltip("此池的克隆预加载类型。")]
        [SerializeField] private PreloadType _preloadType = PreloadType.Disabled;
        [Tooltip("此池的预加载大小。")]
        [SerializeField, Delayed, Min(0)] private int _preloadSize = 16;

        [Header("Callbacks")]
        [Tooltip(Constants.Tooltips.CallbacksType)]
        [SerializeField] internal CallbacksType _callbacksType = Constants.DefaultCallbacksType;

        [Header("Persistent")]
        [Tooltip("此池是否应该是持久的？")]
        [SerializeField] internal bool _dontDestroyOnLoad = true;

        [Header("Debug")]
        [Tooltip("此池是否应该查找问题并记录警告？")]
        [SerializeField] internal bool _sendWarnings = true;

#if UNITY_EDITOR
        [Space, ReadOnlyInspectorField]
#endif
        [SerializeField] private int _allClonesCount;

#if UNITY_EDITOR
        [ReadOnlyInspectorField]
#endif
        [SerializeField] private int _spawnedClonesCount;
        
#if UNITY_EDITOR
        [ReadOnlyInspectorField]
#endif
        [SerializeField] private int _despawnedClonesCount;
        
#if UNITY_EDITOR
        [Space, ReadOnlyInspectorField]
#endif
        [SerializeField] private int _spawnsCount;
        
#if UNITY_EDITOR
        [ReadOnlyInspectorField]
#endif
        [SerializeField] private int _despawnsCount;
        
#if UNITY_EDITOR
        [ReadOnlyInspectorField]
#endif
        [SerializeField] private int _total;
        
#if UNITY_EDITOR
        [Space, ReadOnlyInspectorField]
#endif
        [SerializeField] private int _instantiated;

        [SerializeField, HideInInspector] private List<GameObject> _gameObjectsToPreload;
        [SerializeField, HideInInspector] private bool _hasPreloadedGameObjects;

        internal Transform _cachedTransform;
        internal Vector3 _regularPrefabScale;
        internal bool _isSetup;
        
        private readonly F8PoolList<Poolable> _spawnedPoolables 
            = new F8PoolList<Poolable>(Constants.DefaultPoolablesListCapacity);
        
        private readonly F8PoolList<Poolable> _despawnedPoolables 
            = new F8PoolList<Poolable>(Constants.DefaultPoolablesListCapacity);
        
        private F8PoolList<Poolable> _poolablesTemp;
        private Transform _prefabTransform;
#if UNITY_EDITOR
        private GameObject _cachedPrefab;
#endif

        /// <summary>
        /// The prefab attached to this pool.
        /// </summary>
        public GameObject AttachedPrefab => _prefab;
        
        /// <summary>
        /// Pool overflow behaviour.
        /// </summary>
        public BehaviourOnCapacityReached BehaviourOnCapacityReached => _behaviourOnCapacityReached;

        /// <summary>
        /// Clone despawn type.
        /// </summary>
        public DespawnType DespawnType => _despawnType;
        
        /// <summary>
        /// Callbacks on clone spawn and despawn.
        /// </summary>
        public CallbacksType CallbacksType => _callbacksType;
        
        /// <summary>
        /// Pool capacity.
        /// </summary>
        public int Capacity => _capacity;
        
        /// <summary>
        /// Number of spawned clones.
        /// </summary>
        public int SpawnedClonesCount => _spawnedClonesCount;
        
        /// <summary>
        /// Number of despawned clones.
        /// </summary>
        public int DespawnedClonesCount => _despawnedClonesCount;
        
        /// <summary>
        /// Number of all clones.
        /// </summary>
        public int AllClonesCount => _allClonesCount;
        
        /// <summary>
        /// Number of spawns.
        /// </summary>
        public int SpawnsCount => _spawnsCount;
        
        /// <summary>
        /// Number of despawns.
        /// </summary>
        public int DespawnsCount => _despawnsCount;

        /// <summary>
        /// Number of instantiates.
        /// </summary>
        public int InstantiatesCount => _instantiated;
        
        /// <summary>
        /// Total number of spawns and despawns.
        /// </summary>
        public int TotalActionsCount => _total;
        
        /// <summary>
        /// Has this pool registered as persistent?
        /// </summary>
        public bool HasRegisteredAsPersistent => GameObjectPool.Instance.HasPoolRegisteredAsPersistent(this);

        /// <summary>
        /// The actions will be performed on a game object spawned by this pool.
        /// </summary>
        public readonly F8PoolEvent<GameObject> GameObjectSpawned = new F8PoolEvent<GameObject>();
        
        /// <summary>
        /// The actions will be performed on a game object despawned by this pool.
        /// </summary>
        public readonly F8PoolEvent<GameObject> GameObjectDespawned = new F8PoolEvent<GameObject>();
        
        /// <summary>
        /// The actions will be performed on a game object instantiated by this pool.
        /// </summary>
        public readonly F8PoolEvent<GameObject> GameObjectInstantiated = new F8PoolEvent<GameObject>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            ClampCapacity();
            ClampPreloadSize();
            CheckPreloadedClonesForErrors();
            CheckForPrefabMatchOnPlay();
            CheckForPrefab(_prefab);
        }
#endif
        private void Awake()
        {
            if (_prefab == null)
                return;
            
            if (_dontDestroyOnLoad && HasRegisteredAsPersistent)
            {
                DestroyPool();
                return;
            }
            
            if (TrySetup(_prefab))
            {
                PreloadElements(PreloadType.OnAwake);
            }
        }

        private void Start()
        {
            if (_isSetup)
            {
                PreloadElements(PreloadType.OnStart);
                RaiseEventForPreloadedClonesAndClear();
            }
        }

        /// <summary>
        /// You can initialize the pool manually using this method.
        /// </summary>
        public void Init()
        {
            Init(_prefab);
        }

        /// <summary>
        /// You can initialize the pool manually using this method.
        /// </summary>
        /// <param name="prefab">Pool's prefab.</param>
        public void Init(GameObject prefab)
        {
#if DEBUG
            if (_isSetup)
            {
                if (_sendWarnings)
                {
                    LogF8.LogEntity("池已经初始化完毕！", this);
                }
                
                return;
            }
            
            if (prefab == null)
            {
                LogF8.LogError("您正在尝试使用空预制体初始化此池！", this);
                return;
            }

            if (_hasPreloadedGameObjects && _prefab != prefab)
            {
                LogF8.LogError("此池已预加载游戏对象，而您正在尝试使用另一个预制体初始化此池！" +
                               "清除此池或使用正确的预制体进行初始化。", this);
                return;
            }
#endif
            if (TrySetup(prefab))
            {
                RaiseEventForPreloadedClonesAndClear();
            }
        }
        
        /// <summary>
        /// Populates this pool.
        /// </summary>
        /// <param name="count">Populate count.</param>
        /// <exception cref="Exception">Throws if pool is not setup.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throws if populate count is smaller than zero.</exception>
        public void PopulatePool(int count)
        {
#if DEBUG
            if (_isSetup == false)
            {
                LogF8.LogError($"池 '{name}' 未设置！", this);
                return;
            }

            if (Application.isPlaying == false)
            {
                LogF8.LogError($"在应用程序未运行时，您正在尝试填充池 '{name}'！", this);
                return;
            }

            if (count < 0)
            {
                LogF8.LogError("填充数量不能小于零！", this);
                return;
            }
#endif
            for (var i = 0; i < count; i++)
            {
                if (_allClonesCount >= _capacity)
                {
#if DEBUG
                    if (_sendWarnings)
                    {
                        LogF8.LogEntity($"池 {name} 达到最大容量！");
                    }
#endif
                    return;
                }
                _preloadSize = Mathf.Clamp(count, 0, _capacity);
                AddPoolableToList(_despawnedPoolables, InstantiateAndSetupPoolable(true), 
                    ref _despawnedClonesCount);
            }
        }
        
        /// <summary>
        /// Sets the capacity of this pool.
        /// </summary>
        /// <param name="capacity">New pool capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if capacity is smaller than zero or smaller than all clones count.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCapacity(int capacity)
        {
#if DEBUG
            if (capacity < 0)
            {
                LogF8.LogError($"池 '{name}' 的容量不能小于零！", this);
                return;
            }

            if (capacity < _allClonesCount)
            {
                LogF8.LogError($"池 '{name}' 的容量不能小于所有克隆的数量！", this);
                return;
            }

            if (_hasPreloadedGameObjects && _capacity < _gameObjectsToPreload.Count)
            {
                LogF8.LogError($"池 '{name}' 的容量不能小于预加载克隆的数量！", this);
                return;
            }

            if (_sendWarnings && capacity == 0)
            {
                LogF8.LogEntity($"池 '{name}' 的容量为零。");
            }
#endif
            _capacity = capacity;
            _preloadSize = Mathf.Clamp(_preloadSize, 0, _capacity);
        }

        /// <summary>
        /// Sets the behaviour on capacity reached of this pool.
        /// </summary>
        /// <param name="behaviourOnCapacityReached">New behaviour.</param>
        public void SetBehaviourOnCapacityReached(BehaviourOnCapacityReached behaviourOnCapacityReached)
        {
            _behaviourOnCapacityReached = behaviourOnCapacityReached;
        }

        /// <summary>
        /// Sets the despawn type of this pool.
        /// </summary>
        /// <param name="despawnType">New despawn type.</param>
        public void SetDespawnType(DespawnType despawnType)
        {
            _despawnType = despawnType;
        }
        
        /// <summary>
        /// Sets the callbacks type of this pool.
        /// </summary>
        /// <param name="callbacksType">New callbacks type.</param>
        public void SetCallbacksType(CallbacksType callbacksType)
        {
            _callbacksType = callbacksType;
        }

        /// <summary>
        /// Sets the warnings active of this pool.
        /// </summary>
        /// <param name="active">New warnings active status.</param>
        public void SetWarningsActive(bool active)
        {
            _sendWarnings = active;
        }

        /// <summary>
        /// Performs an action for each clone.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        public void ForEachClone(Action<GameObject> action)
        {
            ForEach(_spawnedPoolables, action);
            ForEach(_despawnedPoolables, action);
        }
        
        /// <summary>
        /// Performs an action for each spawned clone.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        public void ForEachSpawnedClone(Action<GameObject> action)
        {
            ForEach(_spawnedPoolables, action);
        }

        /// <summary>
        /// Performs an action for each despawned clone.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        public void ForEachDespawnedClone(Action<GameObject> action)
        {
            ForEach(_despawnedPoolables, action);
        }

        /// <summary>
        /// Destroys this pool with clones.
        /// </summary>
        public void DestroyPool()
        {
            Clear();
            GameObjectPool.Instance.UnregisterPool(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Destroys this pool with clones immediate.
        /// </summary>
        public void DestroyPoolImmediate()
        {
            Clear();
            GameObjectPool.Instance.UnregisterPool(this);
            DestroyImmediate(gameObject);
        }
        
        /// <summary>
        /// Destroys all clones in this pool (also destroys preloaded clones).
        /// </summary>
#if UNITY_EDITOR
        [ContextMenu("Clear")]
#endif
        public void Clear()
        {
            ClearEvents();
            ClearGameObjectsToPreload();
            DestroyAllClonesImmediate();
            ResetCounts();
        }
        
        /// <summary>
        /// Destroys all clones in this pool.
        /// </summary>
        public void DestroyAllClones()
        {
            DestroySpawnedClones();
            DestroyDespawnedClones();
        }

        /// <summary>
        /// Destroys spawned clones in this pool.
        /// </summary>
        public void DestroySpawnedClones()
        {
            DisposePoolablesInList(_spawnedPoolables, ref _spawnedClonesCount, false);
        }

        /// <summary>
        /// Destroys despawned clones in this pool.
        /// </summary>
        public void DestroyDespawnedClones()
        {
            DisposePoolablesInList(_despawnedPoolables, ref _despawnedClonesCount, false);
        }

        /// <summary>
        /// Destroys all clones in this pool immediately.
        /// </summary>
        public void DestroyAllClonesImmediate()
        {
            DestroySpawnedClonesImmediate();
            DestroyDespawnedClonesImmediate();
        }

        /// <summary>
        /// Destroys spawned clones in this pool immediately.
        /// </summary>
        public void DestroySpawnedClonesImmediate()
        {
            DisposePoolablesInList(_spawnedPoolables, ref _spawnedClonesCount, true);
        }

        /// <summary>
        /// Destroys despawned clones in this pool immediately.
        /// </summary>
        public void DestroyDespawnedClonesImmediate()
        {
            DisposePoolablesInList(_despawnedPoolables, ref _despawnedClonesCount, true);
        }

        /// <summary>
        /// Despawns all spawned clones.
        /// </summary>
        public void DespawnAllClones()
        {
            _poolablesTemp ??= new F8PoolList<Poolable>(Constants.DefaultPoolablesListCapacity);

            for (int i = 0; i < _spawnedPoolables._count; i++)
            {
                _poolablesTemp.Add(_spawnedPoolables._components[i]);
            }

            for (int i = 0; i < _poolablesTemp._count; i++)
            {
                GameObjectPool.Instance.DespawnImmediate(_poolablesTemp._components[i]);
            }

            if (_poolablesTemp._count > 0)
            {
                _poolablesTemp.Clear();
                _poolablesTemp.SetCapacity(Constants.DefaultPoolablesListCapacity);
            }
        }

        internal bool TrySetup(GameObject prefab)
        {
            if (_isSetup)
                return false;
            
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                LogF8.LogError("应用程序未运行时，无法设置池！", this);
                return false;
            }
            
            if (GameObjectPool.Instance.s_checkForPrefab)
            {
                if (CheckForPrefab(prefab) == false)
                {
                    return false;
                }
            }

            _cachedPrefab = prefab;
#endif
            _prefab = prefab;
            _cachedTransform = transform;
            _prefabTransform = prefab.transform;
            _regularPrefabScale = _prefabTransform.localScale;
            
            if (_dontDestroyOnLoad)
            {
                if (TryRegisterPoolAsPersistent() == false)
                {
                    return false;
                }
            }

            if (_hasPreloadedGameObjects)
            {
                SetupPreloadedClones();
            }
            
            GameObjectPool.Instance.RegisterPool(this);
            
            _isSetup = true;
            return true;
        }

        internal void UnregisterPoolable(Poolable poolable)
        {
            RemovePoolableUnorderedFromList(_spawnedPoolables, poolable, ref _spawnedClonesCount);
            RemovePoolableUnorderedFromList(_despawnedPoolables, poolable, ref _despawnedClonesCount);
        }

        internal void Get(out GettingPoolableArguments arguments)
        {
            if (_despawnedPoolables._count <= 0)
            {
                if (_allClonesCount >= _capacity)
                {
                    if (_behaviourOnCapacityReached == BehaviourOnCapacityReached.Recycle)
                    {
                        Poolable poolable = _spawnedPoolables._components[0];
                        _spawnedPoolables.RemoveAt(0);
                        _spawnedPoolables.Add(poolable);
                        arguments = new GettingPoolableArguments(poolable, false);
                        return;
                    }
                    
                    if (_behaviourOnCapacityReached == BehaviourOnCapacityReached.InstantiateWithCallbacks)
                    {
                        InstantiatePoolableOverCapacity(out arguments);
                        return;
                    }
                    
                    if (_behaviourOnCapacityReached == BehaviourOnCapacityReached.Instantiate)
                    {
                        InstantiatePoolableOverCapacity(out arguments);
                        return;
                    }

                    if (_behaviourOnCapacityReached == BehaviourOnCapacityReached.ReturnNullableClone)
                    {
                        arguments = new GettingPoolableArguments(null, true);
                        return;
                    }

                    if (_behaviourOnCapacityReached == BehaviourOnCapacityReached.ThrowException)
                    {
#if DEBUG
                        LogF8.LogError("已达到容量上限！无法生成新的克隆！", this);
#endif
                        arguments = new GettingPoolableArguments(null, true);
                        return;
                    }
                }

                arguments = new GettingPoolableArguments(InstantiateAndSetupPoolable(false), false);
                AddPoolableToList(_spawnedPoolables, arguments.Poolable, ref _spawnedClonesCount);
                return;
            }

            arguments = new GettingPoolableArguments(_despawnedPoolables._components[0], false);
            AddPoolableToList(_spawnedPoolables, _despawnedPoolables._components[0], ref _spawnedClonesCount);
            RemoveFirstPoolableUnordered(_despawnedPoolables, ref _despawnedClonesCount);
        }

        internal void Release(Poolable poolable)
        {
            if (poolable._status == PoolableStatus.Despawned)
            {
#if DEBUG
                if (_sendWarnings)
                {
                    LogF8.LogEntity($"池对象 '{poolable._gameObject}' 已经取消生成！", poolable._gameObject);
                }
#endif
                return;
            }
            
            poolable._gameObject.SetActive(false);

            switch (_despawnType)
            {
                case DespawnType.DeactivateAndHide: HidePoolable(poolable); break;
                case DespawnType.DeactivateAndSetNullParent: SetPoolableParentAsNull(poolable); break;
                case DespawnType.OnlyDeactivate: break;
                default: throw new ArgumentOutOfRangeException(nameof(_despawnType));
            }

            AddPoolableToList(_despawnedPoolables, poolable, ref _despawnedClonesCount);
            RemovePoolableUnorderedFromList(_spawnedPoolables, poolable, ref _spawnedClonesCount);
        }

        internal void RaiseGameObjectSpawnedCallback(GameObject spawnedGameObject)
        {
            RaisePoolActionCallback(spawnedGameObject, ref _spawnsCount, GameObjectSpawned);
        }

        internal void RaiseGameObjectDespawnedCallback(GameObject despawnedGameObject)
        {
            RaisePoolActionCallback(despawnedGameObject, ref _despawnsCount, GameObjectDespawned);
        }

        private void InstantiatePoolableOverCapacity(out GettingPoolableArguments arguments)
        {
            GameObject newGameObject = Instantiate(_prefab);
            SetupPoolableAsSpawnedOverCapacity(newGameObject, out Poolable poolable);
            arguments = new GettingPoolableArguments(poolable, false);
            RaiseGameObjectInstantiatedCallback(poolable._gameObject);
        }

        private void RaisePoolActionCallback(GameObject clone, ref int actionCount, 
            F8PoolEvent<GameObject> poolEvent)
        {
            _total++;
            actionCount++;
            poolEvent.RaiseEvent(clone);
        }

        private void RaiseEventForPreloadedClonesAndClear()
        {
            if (_hasPreloadedGameObjects)
            {
                for (int i = 0; i < _gameObjectsToPreload.Count; i++)
                {
                    GameObjectInstantiated.RaiseEvent(_gameObjectsToPreload[i]);
                }

                _hasPreloadedGameObjects = false;
                _gameObjectsToPreload = null;
            }
        }
        
        private void HidePoolable(Poolable poolable)
        {
            poolable._transform.SetParent(_cachedTransform, true);
        }

        private void SetPoolableParentAsNull(Poolable poolable)
        {
            poolable._transform.SetParent(null, false);
        }
        
        private void RaiseGameObjectInstantiatedCallback(GameObject instantiatedGameObject)
        {
            _instantiated++;
            GameObjectInstantiated.RaiseEvent(instantiatedGameObject);
        }

#if UNITY_EDITOR
        private bool CheckForPrefab(GameObject gameObjectToCheck)
        {
            if (gameObjectToCheck == null)
                return false;

            if (gameObjectToCheck.scene.isLoaded)
            {
                LogF8.LogError("您不能将场景中的游戏对象设置为预制体！", this);
                _prefab = null;
                return false;
            }

            if (PrefabUtility.IsPartOfAnyPrefab(gameObjectToCheck) == false)
            {
                LogF8.LogError($"'{gameObjectToCheck}' 不是一个预制体！", this);
                _prefab = null;
                return false;
            }

            return true;
        }

        private void ClampCapacity()
        {
            if (_hasPreloadedGameObjects && _capacity < _gameObjectsToPreload.Count)
            {
                LogF8.LogError("容量不能小于预加载克隆的数量！", this);
                _capacity = _gameObjectsToPreload.Count;
            }
            
            if (_despawnedPoolables != null)
            {
                if (_capacity < _allClonesCount)
                {
                    LogF8.LogError("容量不能小于所有克隆的数量！", this);
                    _capacity = _allClonesCount;
                }
            }
        }
        
        private void ClampPreloadSize()
        {
            if (_preloadSize > _capacity)
            {
                _preloadSize = _capacity;
            }
        }
        
        private void CheckPreloadedClonesForErrors()
        {
            if (_hasPreloadedGameObjects)
            {
                bool isApplicationPlaying = Application.isPlaying;
                
                if (_prefab == null)
                {
                    LogF8.LogError("此池中已预加载游戏对象，但现在预制体为空！" +
                                   "设置正确的预制体以解决此问题或清除此池。", this);
                }

                for (int i = 0; i < _gameObjectsToPreload.Count; i++)
                {
                    GameObject clone = _gameObjectsToPreload[i];
                    
                    if (clone == null)
                    {
                        LogF8.LogError("此池的预加载游戏对象之一为空！" +
                                       "清除此池以解决此问题。", this);
                        return;
                    }
                    
                    if (isApplicationPlaying == false && 
                        PrefabUtility.GetCorrespondingObjectFromSource(clone) != _prefab)
                    {
                        LogF8.LogError("您预加载的游戏对象与预制体不匹配。" +
                                       "清除此池或设置正确的预制体。", this);
                        return;
                    }
                }
            }
        }

        private void CheckForPrefabMatchOnPlay()
        {
            if (_isSetup && Application.isPlaying)
            {
                if (_cachedPrefab != null && _prefab != _cachedPrefab)
                {
                    _prefab = _cachedPrefab;
                }
            }
        }
#endif
        
        private static void ForEach(F8PoolList<Poolable> list, Action<GameObject> action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            for (int i = 0; i < list._count; i++)
            {
                action.Invoke(list._components[i]._gameObject);
            }
        }
        
        private void DisposePoolablesInList(F8PoolList<Poolable> f8PoolList, ref int count, bool immediately)
        {
            for (int i = 0; i < f8PoolList._count; i++)
            {
                f8PoolList._components[i].Dispose(immediately);
                _allClonesCount--;
            }
            
            f8PoolList.Clear();
            count = 0;
        }
        
        private bool TryRegisterPoolAsPersistent()
        {
            if (GameObjectPool.Instance.HasPoolRegisteredAsPersistent(this) == false)
            {
#if DEBUG
                if (_cachedTransform.parent != null)
                {
                    LogF8.LogError("池不能是持久的！" +
                                   "因为此 GameObject 有父 Transform，" +
                                   "而 DontDestroyOnLoad 只对根 GameObject 或根 GameObject 上的组件有效。", this);
                    return false;
                }
#endif
                _dontDestroyOnLoad = true;
                DontDestroyOnLoad(gameObject);
                GameObjectPool.Instance.RegisterPersistentPool(this);
                return true;
            }
            
            DestroyPool();
            return false;
        }
        
        private void AddPoolableToList(F8PoolList<Poolable> f8PoolList,
            Poolable poolable, ref int count)
        {
            f8PoolList.Add(poolable);
            count++;
            _allClonesCount++;
        }
        
        private void RemovePoolableUnorderedFromList(F8PoolList<Poolable> f8PoolList,
            Poolable poolable, ref int count)
        {
            for (int i = 0; i < f8PoolList._count; i++)
            {
                if (f8PoolList._components[i] == poolable)
                {
                    f8PoolList.RemoveUnorderedAt(i);
                    count--;
                    _allClonesCount--;
                    return;
                }
            }
        }

        private void RemoveFirstPoolableUnordered(F8PoolList<Poolable> f8PoolList, ref int count)
        {
            f8PoolList.RemoveUnorderedAt(0);
            count--;
            _allClonesCount--;
        }

#if UNITY_EDITOR
        [ContextMenu("Preload")]
        private void Preload()
        {
            for (int i = 0; i < _preloadSize; i++)
            {
                if (TryPreloadGameObject() == false)
                {
                    return;
                }
            }
        }

        [ContextMenu("Preload One")]
        private void PreloadOne()
        {
            TryPreloadGameObject();
        }

        private bool TryPreloadGameObject()
        {
            if (CanPreloadGameObject())
            {
                PreloadGameObject();
                return true;
            }

            return false;
        }

        private bool CanPreloadGameObject()
        {
            if (_prefab == null)
            {
                LogF8.LogError($"池 '{name}' 的预制体为空！", this);
                return false;
            }
            
            if (CheckForPrefab(_prefab) == false)
            {
                return false;
            }
            
            if (_gameObjectsToPreload.Count >= _capacity || _allClonesCount >= _capacity)
            {
                if (_sendWarnings)
                {
                    LogF8.LogEntity("已达到容量上限！无法预加载更多游戏对象！", this);
                }
                
                return false;
            }

            return true;
        }
        
        private void PreloadGameObject()
        {
            GameObject gameObjectToPreload = PrefabUtility.InstantiatePrefab(_prefab, transform) as GameObject;

            if (gameObjectToPreload == null) 
                return;
            
            gameObjectToPreload.SetActive(false);

            _instantiated++;

            if (Application.isPlaying)
            {
                SetupPoolableAsDefault(gameObjectToPreload, out Poolable poolable);
                AddPoolableToList(_despawnedPoolables, poolable, ref _despawnedClonesCount);
            }
            else
            {
                _hasPreloadedGameObjects = true;
                _gameObjectsToPreload.Add(gameObjectToPreload);
            }
        }
#endif
        
        private void PreloadElements(PreloadType requiredType)
        {
            if (_preloadType != requiredType)
                return;

            if (_allClonesCount >= _capacity)
                return;
            
            PopulatePool(_preloadSize);
        }

        private void SetupPreloadedClones()
        {
            for (var i = 0; i < _gameObjectsToPreload.Count; i++)
            {
                GameObject clone = _gameObjectsToPreload[i];
#if DEBUG
                if (clone == null)
                {
                    LogF8.LogError($"其中一个预加载的游戏对象已被销毁！在应用程序未运行时，从组件上下文菜单清除 '{name}' 池 " +
                                   "以解决此问题！", this);
                    continue;
                }
#endif
                SetupPoolableAsDefault(clone, out Poolable poolable);
                AddPoolableToList(_despawnedPoolables, poolable, ref _despawnedClonesCount);
            }
        }
        
        private Poolable InstantiateAndSetupPoolable(bool isPopulatingPool)
        {
            GameObject newGameObject = Instantiate(_prefab);
            SetupPoolableAsDefault(newGameObject, out Poolable poolable);
            
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(newGameObject);
            }
            
            if (isPopulatingPool)
            {
                poolable._gameObject.SetActive(false);
                poolable._transform.SetParent(_isSetup ? _cachedTransform : transform, false);
            }

            GameObjectPool.Instance.GameObjectInstantiated.RaiseEvent(newGameObject);
            RaiseGameObjectInstantiatedCallback(newGameObject);
            return poolable;
        }

        private void SetupPoolableAsDefault(GameObject clone, out Poolable poolable)
        {
            poolable = CreatePoolable(clone);
            poolable.SetupAsDefault();
        }

        private void SetupPoolableAsSpawnedOverCapacity(GameObject clone, out Poolable poolable)
        {
            poolable = CreatePoolable(clone);
            poolable.SetupAsSpawnedOverCapacity();
        }

        private Poolable CreatePoolable(GameObject clone)
        {
            return new Poolable
            {
                _pool = this,
                _gameObject = clone,
                _transform = clone.transform   
            };
        }

        private void ClearGameObjectsToPreload()
        {
            if (_gameObjectsToPreload != null)
            {
                for (int i = 0; i < _gameObjectsToPreload.Count; i++)
                {
                    DestroyImmediate(_gameObjectsToPreload[i]);
                }
                
                _gameObjectsToPreload.Clear();
                _hasPreloadedGameObjects = false;
            }
        }

        private void ResetCounts()
        {
            _allClonesCount = 0;
            _instantiated = 0;
            _spawnsCount = 0;
            _despawnsCount = 0;
            _total = 0;
        }

        private void ClearEvents()
        {
            GameObjectSpawned.Clear();
            GameObjectDespawned.Clear();
            GameObjectInstantiated.Clear();
        }
    }
}

#if UNITY_EDITOR
namespace F8Framework.Core
{
    [CustomPropertyDrawer(typeof(ReadOnlyInspectorFieldAttribute))]
    public sealed class ReadOnlyInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReadOnlyInspectorFieldAttribute : PropertyAttribute { }
}
#endif