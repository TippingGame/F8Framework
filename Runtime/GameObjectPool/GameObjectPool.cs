using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace F8Framework.Core
{
    public class GameObjectPool : ModuleSingleton<GameObjectPool>, IModule
    {
        internal readonly Dictionary<GameObject, Poolable> ClonesMap =
            new Dictionary<GameObject, Poolable>(Constants.DefaultClonesCapacity);

        internal readonly F8PoolList<DespawnRequest> DespawnRequests =
            new F8PoolList<DespawnRequest>(Constants.DefaultDespawnRequestsCapacity);

        internal F8PoolMode s_f8PoolMode = Constants.DefaultF8PoolMode;
        internal bool s_hasTheF8PoolInitialized = false;
        internal bool s_isApplicationQuitting = false;
        internal bool s_despawnPersistentClonesOnDestroy = true;
        internal bool s_checkClonesForNull = true;
        internal bool s_checkForPrefab = false;
        internal F8PoolGlobal s_instance = null;

        private readonly Dictionary<GameObject, F8GameObjectPool> AllPoolsMap =
            new Dictionary<GameObject, F8GameObjectPool>(Constants.DefaultPoolsMapCapacity);

        private readonly Dictionary<GameObject, F8GameObjectPool> PersistentPoolsMap =
            new Dictionary<GameObject, F8GameObjectPool>(Constants.DefaultPersistentPoolsCapacity);

        private readonly List<ISpawnable> SpawnableItemComponents =
            new List<ISpawnable>(Constants.DefaultPoolableInterfacesCapacity);

        private readonly List<IDespawnable> DespawnableItemComponents =
            new List<IDespawnable>(Constants.DefaultPoolableInterfacesCapacity);

        private readonly object SecurityLock = new object();

        private BehaviourOnCapacityReached BehaviourOnCapacityReached => s_hasTheF8PoolInitialized
            ? s_instance._behaviourOnCapacityReached
            : Constants.DefaultBehaviourOnCapacityReached;

        private DespawnType DespawnType => s_hasTheF8PoolInitialized
            ? s_instance._despawnType
            : Constants.DefaultDespawnType;

        private CallbacksType CallbacksType => s_hasTheF8PoolInitialized
            ? s_instance._callbacksType
            : Constants.DefaultCallbacksType;

        private ReactionOnRepeatedDelayedDespawn ReactionOnRepeatedDelayedDespawn =>
            s_hasTheF8PoolInitialized
                ? s_instance._reactionOnRepeatedDelayedDespawn
                : Constants.DefaultDelayedDespawnHandleType;

        private int Capacity => s_hasTheF8PoolInitialized
            ? s_instance._capacity
            : Constants.DefaultPoolCapacity;

        private bool Persistent => s_hasTheF8PoolInitialized
            ? s_instance._dontDestroyOnLoad
            : Constants.DefaultPoolPersistenceStatus;

        private bool Warnings => s_hasTheF8PoolInitialized
            ? s_instance._sendWarnings
            : Constants.DefaultSendWarningsStatus;

        /// <summary>
        /// The actions will be performed on a game object created in any pool.
        /// </summary>
        public readonly F8PoolEvent<GameObject> GameObjectInstantiated = new F8PoolEvent<GameObject>();

        /// <summary>
        /// Installs a pools by PoolPreset.
        /// </summary>
        public void InstallPools(PoolsPreset poolsPreset)
        {
#if DEBUG
            if (poolsPreset == null)
                throw new ArgumentNullException(nameof(poolsPreset));
#endif
            int count = poolsPreset.Presets.Count;

            for (int i = 0; i < count; i++)
            {
                PoolPreset preset = poolsPreset.Presets[i];

                if (preset.Enabled == false)
                    continue;

                GameObject prefab = preset.Prefab;
#if DEBUG
                if (prefab == null)
                {
                    LogF8.LogError($"名称为{nameof(PoolsPreset)}的'{poolsPreset}'预设中有一个或多个空的预制体!",
                        poolsPreset);

                    continue;
                }
#endif
                int preloadSize = Mathf.Clamp(preset.PreloadSize, 0, preset.Capacity);

                if (TryGetPoolByPrefab(prefab, out F8GameObjectPool pool) == false)
                {
                    pool = CreateNewGameObjectPool(prefab);

                    SetupNewPool(
                        pool,
                        prefab,
                        preset.BehaviourOnCapacityReached,
                        preset.DespawnType,
                        preset.CallbacksType,
                        preset.Capacity,
                        preloadSize,
                        preset.Persistent,
                        preset.Warnings);
                }
                else
                {
                    if (preset.Persistent && pool.HasRegisteredAsPersistent)
                    {
                        continue;
                    }
#if DEBUG
                    LogF8.LogError($"您正在尝试通过{nameof(PoolsPreset)} '{poolsPreset}'安装的池 '{pool}' 已经存在!",
                        pool);
#endif
                }
            }
        }
        
        /// <summary>
        /// Spawns a GameObject.
        /// </summary>
        /// <param name="prefabName">GameObject prefab name to spawn.</param>
        /// <returns>Spawned GameObject.</returns>
        public GameObject Spawn(string prefabName)
        {
            F8GameObjectPool prefab = GetPoolByPrefabName(prefabName);
            if (!prefab)
            {
                LogF8.LogError("对象池未创建，通过名称生成对象失败。");
                return null;
            }
            Transform prefabTransform = prefab.AttachedPrefab.transform;

            return DefaultSpawn(
                prefab.AttachedPrefab, prefabTransform.localPosition, prefabTransform.localRotation, null, false, out _);
        }
        
        /// <summary>
        /// Spawns a GameObject.
        /// </summary>
        /// <param name="prefab">GameObject prefab to spawn.</param>
        /// <returns>Spawned GameObject.</returns>
        public GameObject Spawn(GameObject prefab)
        {
            Transform prefabTransform = prefab.transform;

            return DefaultSpawn(
                prefab, prefabTransform.localPosition, prefabTransform.localRotation, null, false, out _);
        }

        /// <summary>
        /// Spawns a GameObject.
        /// </summary>
        /// <param name="prefab">GameObject prefab to spawn.</param>
        /// <param name="position">Spawned GameObject position.</param>
        /// <param name="rotation">Spawned GameObject rotation.</param>
        /// <returns>Spawned GameObject.</returns>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return DefaultSpawn(prefab, position, rotation, null, false, out _);
        }

        /// <summary>
        /// Spawns a GameObject.
        /// </summary>
        /// <param name="prefab">GameObject prefab to spawn.</param>
        /// <param name="position">Spawned GameObject position.</param>
        /// <param name="rotation">Spawned GameObject rotation.</param>
        /// <param name="parent">The parent of the spawned GameObject.</param>
        /// <returns>Spawned GameObject.</returns>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (parent != null)
            {
                position = parent.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(parent.rotation) * rotation;
            }

            return DefaultSpawn(prefab, position, rotation, parent, false, out _);
        }

        /// <summary>
        /// Spawns a GameObject.
        /// </summary>
        /// <param name="prefab">GameObject prefab to spawn.</param>
        /// <param name="parent">The parent of the spawned GameObject.</param>
        /// <param name="worldPositionStays">World position stays.</param>
        /// <returns>Spawned GameObject.</returns>
        public GameObject Spawn(GameObject prefab, Transform parent, bool worldPositionStays = false)
        {
            GetPositionAndRotationByParent(prefab, parent, out Vector3 position, out Quaternion rotation);

            return DefaultSpawn(prefab, position, rotation, parent, worldPositionStays, out _);
        }

        /// <summary>
        /// Spawns a GameObject as T component.
        /// </summary>
        /// <param name="prefab">Component prefab to spawn.</param>
        /// <typeparam name="T">Component.</typeparam>
        /// <returns>Spawned GameObject as T component.</returns>
        public T Spawn<T>(T prefab) where T : Component
        {
            Transform prefabTransform = prefab.transform;

            GameObject spawnedGameObject = DefaultSpawn(prefab.gameObject, prefabTransform.localPosition,
                prefabTransform.localRotation, null, false, out bool haveToGetComponent);

            return haveToGetComponent
                ? spawnedGameObject.GetComponent<T>()
                : null;
        }

        /// <summary>
        /// Spawns a GameObject as T component.
        /// </summary>
        /// <param name="prefab">Component prefab to spawn.</param>
        /// <param name="position">Spawned GameObject position.</param>
        /// <param name="rotation">Spawned GameObject rotation.</param>
        /// <typeparam name="T">Component type.</typeparam>
        /// <returns>Spawned GameObject as T component.</returns>
        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation)
            where T : Component
        {
            GameObject spawnedGameObject = DefaultSpawn(prefab.gameObject, position, rotation, null, false,
                out bool haveToGetComponent);

            return haveToGetComponent
                ? spawnedGameObject.GetComponent<T>()
                : null;
        }

        /// <summary>
        /// Spawns a GameObject as T component.
        /// </summary>
        /// <param name="prefab">Component prefab to spawn.</param>
        /// <param name="parent">The parent of the spawned GameObject.</param>
        /// <param name="position">Spawned GameObject position.</param>
        /// <param name="rotation">Spawned GameObject rotation.</param>
        /// <typeparam name="T">Component type.</typeparam>
        /// <returns>Spawned GameObject as T component.</returns>
        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
            where T : Component
        {
            if (parent != null)
            {
                position = parent.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(parent.rotation) * rotation;
            }

            GameObject spawnedGameObject = DefaultSpawn(prefab.gameObject, position, rotation, parent, false,
                out bool haveToGetComponent);

            return haveToGetComponent
                ? spawnedGameObject.GetComponent<T>()
                : null;
        }

        /// <summary>
        /// Spawns a GameObject as T component.
        /// </summary>
        /// <param name="prefab">Component prefab to spawn.</param>
        /// <param name="parent">The parent of the spawned GameObject.</param>
        /// <param name="worldPositionStays">World position stays.</param>
        /// <typeparam name="T">Component type.</typeparam>
        /// <returns>Spawned GameObject as T component.</returns>
        public T Spawn<T>(T prefab, Transform parent, bool worldPositionStays = false) where T : Component
        {
            GameObject prefabGameObject = prefab.gameObject;

            GetPositionAndRotationByParent(prefabGameObject, parent, out Vector3 position, out Quaternion rotation);

            GameObject spawnedGameObject = DefaultSpawn(prefabGameObject, position, rotation, parent,
                worldPositionStays, out bool haveToGetComponent);

            return haveToGetComponent
                ? spawnedGameObject.GetComponent<T>()
                : null;
        }

        /// <summary>
        /// Despawns the clone.
        /// </summary>
        /// <param name="clone">Clone to despawn.</param>
        /// <param name="delay">Despawn delay.</param>
        public void Despawn(Component clone, float delay = 0f)
        {
            DefaultDespawn(clone.gameObject, delay);
        }

        /// <summary>
        /// Despawns the clone.
        /// </summary>
        /// <param name="clone">Clone to despawn.</param>
        /// <param name="delay">Despawn delay.</param>
        public void Despawn(GameObject clone, float delay = 0f)
        {
            DefaultDespawn(clone, delay);
        }

        /// <summary>
        /// Performs an action for each pool.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        /// <exception cref="ArgumentNullException">Throws if action is null.</exception>
        public void ForEachPool(Action<F8GameObjectPool> action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            foreach (F8GameObjectPool pool in AllPoolsMap.Values)
            {
                action.Invoke(pool);
            }
        }

        /// <summary>
        /// Performs an action for each clone.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        /// <exception cref="ArgumentNullException">Throws if action is null.</exception>
        public void ForEachClone(Action<GameObject> action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            foreach (Poolable poolable in ClonesMap.Values)
            {
                action.Invoke(poolable._gameObject);
            }
        }

        /// <summary>
        /// Tries to get pool by spawned gameObject.
        /// </summary>
        /// <param name="clone">Component which spawned via F8Pool</param>
        /// <param name="pool">Found pool.</param>
        /// <returns>Returns true if pool found, otherwise false.</returns>
        public bool TryGetPoolByClone(Component clone, out F8GameObjectPool pool)
        {
            return TryGetPoolByClone(clone.gameObject, out pool);
        }

        /// <summary>
        /// Tries to get pool by spawned gameObject.
        /// </summary>
        /// <param name="clone">GameObject which spawned via F8Pool</param>
        /// <param name="pool">Found pool.</param>
        /// <returns>Returns true if pool found, otherwise false.</returns>
        public bool TryGetPoolByClone(GameObject clone, out F8GameObjectPool pool)
        {
            if (ClonesMap.TryGetValue(clone, out Poolable poolable) && poolable._isSetup)
            {
                pool = poolable._pool;
                return true;
            }

            pool = null;
            return false;
        }

        /// <summary>
        /// Tries to get pool by gameObject prefab.
        /// </summary>
        /// <param name="prefab">Component prefab.</param>
        /// <param name="pool">Found pool.</param>
        /// <returns>Returns true if pool found, otherwise false.</returns>
        public bool TryGetPoolByPrefab(Component prefab, out F8GameObjectPool pool)
        {
            return TryGetPoolByPrefab(prefab.gameObject, out pool);
        }

        /// <summary>
        /// Tries to get pool by gameObject prefab.
        /// </summary>
        /// <param name="prefab">GameObject prefab.</param>
        /// <param name="pool">Found pool.</param>
        /// <returns>Returns true if pool found, otherwise false.</returns>
        public bool TryGetPoolByPrefab(GameObject prefab, out F8GameObjectPool pool)
        {
            return AllPoolsMap.TryGetValue(prefab, out pool);
        }

        /// <summary>
        /// Returns the pool by clone.
        /// </summary>
        /// <param name="clone">Component which spawned via F8Pool</param>
        /// <returns>Found pool.</returns>
        public F8GameObjectPool GetPoolByClone(Component clone)
        {
            return GetPoolByClone(clone.gameObject);
        }

        /// <summary>
        /// Returns the pool by clone.
        /// </summary>
        /// <param name="clone">GameObject which spawned via F8Pool</param>
        /// <returns>Found pool.</returns>
        public F8GameObjectPool GetPoolByClone(GameObject clone)
        {
            var hasPool = TryGetPoolByClone(clone, out F8GameObjectPool pool);
#if DEBUG
            if (hasPool == false)
                LogF8.LogError($"克隆 '{clone}' 未找到对应的池!",
                    clone);
#endif
            return pool;
        }

        /// <summary>
        /// Returns the pool by prefab.
        /// </summary>
        /// <param name="prefab">Component's prefab.</param>
        /// <returns>Found pool.</returns>
        public F8GameObjectPool GetPoolByPrefab(Component prefab)
        {
            return GetPoolByPrefab(prefab.gameObject);
        }

        /// <summary>
        /// Returns the pool by prefab.
        /// </summary>
        /// <param name="prefab">GameObject's prefab.</param>
        /// <returns>Found pool.</returns>
        public F8GameObjectPool GetPoolByPrefab(GameObject prefab)
        {
            var hasPool = TryGetPoolByPrefab(prefab, out F8GameObjectPool pool);
#if DEBUG
            if (hasPool == false)
                LogF8.LogError($"未通过预制体 '{prefab}' 找到池!",
                    prefab);
#endif
            return pool;
        }

        /// <summary>
        /// Returns the pool by prefab name.
        /// </summary>
        /// <param name="prefabName">GameObject's prefab name.</param>
        /// <returns>Found pool.</returns>
        public F8GameObjectPool GetPoolByPrefabName(string prefabName)
        {
            foreach (var poolKey in AllPoolsMap.Keys)
            {
                if (poolKey.name == prefabName)
                {
                    return AllPoolsMap[poolKey];
                }
            }
#if DEBUG
            LogF8.LogError($"未通过预制体名称 '{prefabName}' 找到池!", prefabName);
#endif
            return null;
        }
        
        /// <summary>
        /// Is the component a clone (spawned using F8Pool)?
        /// </summary>
        /// <param name="clone">Component to check.</param>
        /// <returns>True if component is a clone of the prefab, otherwise false.</returns>
        public bool IsClone(Component clone)
        {
            return IsClone(clone.gameObject);
        }

        /// <summary>
        /// Is the game object a clone (spawned using F8Pool)?
        /// </summary>
        /// <param name="clone">GameObject to check.</param>
        /// <returns>True if game object is a clone of the prefab, otherwise false.</returns>
        public bool IsClone(GameObject clone)
        {
            return ClonesMap.ContainsKey(clone);
        }

        /// <summary>
        /// Returns the status of the clone.
        /// </summary>
        /// <param name="clone">Component which spawned via F8Pool</param>
        /// <returns>Status of the clone.</returns>
        public PoolableStatus GetCloneStatus(Component clone)
        {
            return GetCloneStatus(clone.gameObject);
        }

        /// <summary>
        /// Returns the status of the clone.
        /// </summary>
        /// <param name="clone">GameObject which spawned via F8Pool</param>
        /// <returns>Status of the clone.</returns>
        public PoolableStatus GetCloneStatus(GameObject clone)
        {
            if (ClonesMap.TryGetValue(clone.gameObject, out Poolable poolable))
            {
                return poolable._status;
            }
#if DEBUG
            LogF8.LogError($"克隆 '{clone}' 不是可池化的!",
                clone);
#endif
            return default;
        }

        /// <summary>
        /// Destroys a clone.
        /// </summary>
        /// <param name="clone">Component which spawned via F8Pool</param>
        public void DestroyClone(Component clone)
        {
            DestroyPoolableWithGameObject(clone.gameObject, false);
        }

        /// <summary>
        /// Destroys a clone.
        /// </summary>
        /// <param name="clone">GameObject which spawned via F8Pool</param>
        public void DestroyClone(GameObject clone)
        {
            DestroyPoolableWithGameObject(clone, false);
        }

        /// <summary>
        /// Destroys a clone immediately.
        /// </summary>
        /// <param name="clone">GameObject which spawned via F8Pool</param>
        public void DestroyCloneImmediate(Component clone)
        {
            DestroyPoolableWithGameObject(clone.gameObject, true);
        }

        /// <summary>
        /// Destroys a clone immediately.
        /// </summary>
        /// <param name="clone">GameObject which spawned via F8Pool</param>
        public void DestroyCloneImmediate(GameObject clone)
        {
            DestroyPoolableWithGameObject(clone, true);
        }

        /// <summary>
        /// Destroys all pools.
        /// </summary>
        /// <param name="immediately">Should all pools be destroyed immediately?</param>
        public void DestroyAllPools(bool immediately = false)
        {
            if (CanPerformPoolAction() == false)
            {
#if DEBUG
                LogF8.LogError("在应用程序退出时，您正在尝试销毁所有池！");
#endif
                return;
            }

            if (immediately)
                ForEachPool(pool => pool.DestroyPoolImmediate());
            else
                ForEachPool(pool => pool.DestroyPool());
        }

        internal void RegisterPool(F8GameObjectPool pool)
        {
            if (AllPoolsMap.ContainsKey(pool._prefab) == false)
            {
                AllPoolsMap.Add(pool._prefab, pool);
            }
#if DEBUG
            else
            {
                LogF8.LogError($"您正在尝试注册另一个使用相同预制体 '{pool._prefab}' 的池 '{pool.name}'!",
                    pool);
            }
#endif
        }

        internal void UnregisterPool(F8GameObjectPool pool)
        {
            if (pool._isSetup == false)
                return;

            if (pool._dontDestroyOnLoad)
                PersistentPoolsMap.Remove(pool._prefab);

            AllPoolsMap.Remove(pool._prefab);
        }

        internal void RegisterPersistentPool(F8GameObjectPool pool)
        {
            if (pool._dontDestroyOnLoad)
            {
                if (PersistentPoolsMap.ContainsKey(pool._prefab) == false)
                {
                    PersistentPoolsMap.Add(pool._prefab, pool);
                }
#if DEBUG
                else
                {
                    if (pool._sendWarnings)
                    {
                        LogF8.LogEntity($"您正在尝试注册持久池 '{pool.name}' 两次！",
                            pool);
                    }
                }
#endif
            }
        }

        internal bool HasPoolRegisteredAsPersistent(F8GameObjectPool pool)
        {
            return PersistentPoolsMap.ContainsKey(pool._prefab);
        }

        internal void DespawnImmediate(Poolable poolable)
        {
            if (poolable._isSetup)
            {
                if (poolable._status == PoolableStatus.SpawnedOverCapacity)
                {
                    if (poolable._pool._behaviourOnCapacityReached ==
                        BehaviourOnCapacityReached.InstantiateWithCallbacks)
                    {
                        RaiseCallbacksOnDespawn(poolable);
                    }

                    poolable.Dispose(true);
                    return;
                }

                RaiseCallbacksOnDespawn(poolable);

                poolable._pool.Release(poolable);
                poolable._pool.RaiseGameObjectDespawnedCallback(poolable._gameObject);
                poolable._status = PoolableStatus.Despawned;
            }
            else
            {
#if DEBUG
                if (Warnings)
                {
                    LogF8.LogEntity($"可池化对象 '{poolable._gameObject}' 尚未设置并将被销毁！",
                        poolable._gameObject);
                }
#endif
                poolable.Dispose(true);
            }
        }

        internal void ResetPool()
        {
            ResetLists();
            ResetClonesDictionary();
            HandlePersistentPoolsOnDestroy();
            s_hasTheF8PoolInitialized = false;
        }

        private void RaiseCallbacksOnSpawn(Poolable poolable)
        {
            if (poolable._pool._callbacksType == CallbacksType.None)
                return;

            InvokeCallbacks(
                poolable._gameObject,
                poolable._pool._callbacksType,
                spawnable => spawnable.OnSpawn(),
                SpawnableItemComponents,
                Constants.OnSpawnMessageName);
        }

        private void RaiseCallbacksOnDespawn(Poolable poolable)
        {
            if (poolable._pool._callbacksType == CallbacksType.None)
                return;

            InvokeCallbacks(
                poolable._gameObject,
                poolable._pool._callbacksType,
                despawnable => despawnable.OnDespawn(),
                DespawnableItemComponents,
                Constants.OnDespawnMessageName);
        }

        private void InitializeTheF8Pool()
        {
            lock (SecurityLock)
            {
                if (s_instance == null)
                {
                    if (TryFindF8PoolInstanceAsSingle(out s_instance) == false)
                    {
                        CreateF8PoolInstance();
#if DEBUG
                        LogF8.LogEntity($"<{nameof(F8PoolGlobal)}> 实例已自动创建。也可以手动添加以修改默认参数。");
#endif
                    }
                }

                s_hasTheF8PoolInitialized = true;
            }
        }

        private bool TryFindF8PoolInstanceAsSingle(out F8PoolGlobal f8Pool)
        {
            var instances = Object.FindObjectsOfType<F8PoolGlobal>();
            var length = instances.Length;

            if (length > 0)
            {
#if DEBUG
                if (length > 1)
                {
                    for (var i = 1; i < length; i++)
                    {
                        Object.Destroy(instances[i]);
                    }

                    LogF8.LogError($"场景中 {nameof(F8PoolGlobal)} 实例的数量大于一个！");
                }
#endif
                f8Pool = instances[0];
                return true;
            }

            f8Pool = null;
            return false;
        }

        private F8GameObjectPool GetPoolByPrefabOrCreate(GameObject prefab)
        {
            if (TryGetPoolByPrefab(prefab, out F8GameObjectPool pool) == false)
            {
                pool = CreateNewGameObjectPool(prefab);

                SetupNewPool(
                    pool,
                    prefab,
                    BehaviourOnCapacityReached,
                    DespawnType,
                    CallbacksType,
                    Capacity,
                    Constants.NewPoolPreloadSize,
                    Persistent,
                    Warnings);
            }

            return pool;
        }

        private void CreateF8PoolInstance()
        {
            s_instance = F8PoolGlobal.Instance;
        }

        private F8GameObjectPool CreateNewGameObjectPool(GameObject prefab)
        {
            return new GameObject($"[{nameof(GameObjectPool)}] {prefab.name}").AddComponent<F8GameObjectPool>();
        }

        private void SetupNewPool(
            F8GameObjectPool pool,
            GameObject prefab,
            BehaviourOnCapacityReached behaviourOnCapacityReached,
            DespawnType despawnType,
            CallbacksType callbacksType,
            int capacity,
            int preloadSize,
            bool persistent,
            bool warnings)
        {
            pool._dontDestroyOnLoad = persistent;
            pool.SetWarningsActive(warnings);
            pool.SetCapacity(capacity);
            pool.SetCallbacksType(callbacksType);
            pool.SetDespawnType(despawnType);
            pool.SetBehaviourOnCapacityReached(behaviourOnCapacityReached);
            pool.TrySetup(prefab);
            pool.PopulatePool(preloadSize);
        }

        private GameObject DefaultSpawn(GameObject prefab, Vector3 position, Quaternion rotation,
            Transform parent, bool worldPositionStays, out bool haveToGetComponent)
        {
            if (CanPerformPoolAction() == false)
            {
#if DEBUG
                LogF8.LogError($"在应用程序退出时，您正在尝试生成预制体 '{prefab}'！", prefab);
#endif
                haveToGetComponent = false;
                return null;
            }

            F8GameObjectPool pool = GetPoolByPrefabOrCreate(prefab);
            pool.Get(out GettingPoolableArguments arguments);

            if (arguments.IsResultNullable)
            {
                haveToGetComponent = false;
                return null;
            }
#if DEBUG
            if (s_checkClonesForNull)
            {
                if (arguments.Poolable._gameObject == null)
                {
                    LogF8.LogError("您正在尝试生成一个已经在没有 {nameof(GameObjectPool)} 的情况下被销毁的克隆！预制体: '{prefab}'", pool);
                }
            }
#endif
            if (arguments.Poolable._status == PoolableStatus.Despawned)
            {
                arguments.Poolable._gameObject.SetActive(true);
            }

            SetupTransform(arguments.Poolable, pool, position, rotation, parent, worldPositionStays);
            pool.RaiseGameObjectSpawnedCallback(arguments.Poolable._gameObject);

            if (arguments.Poolable._status == PoolableStatus.SpawnedOverCapacity)
            {
                if (pool._behaviourOnCapacityReached == BehaviourOnCapacityReached.InstantiateWithCallbacks)
                {
                    RaiseCallbacksOnSpawn(arguments.Poolable);
                }
            }
            else
            {
                arguments.Poolable._status = PoolableStatus.Spawned;
                RaiseCallbacksOnSpawn(arguments.Poolable);
            }

            haveToGetComponent = true;
            return arguments.Poolable._gameObject;
        }

        private void DefaultDespawn(GameObject gameObject, float delay = 0f)
        {
            if (CanPerformPoolAction() == false)
            {
#if DEBUG
                LogF8.LogError($"在应用程序退出时，您正在尝试取消生成 '{gameObject}'！", gameObject);
#endif
                return;
            }

            if (ClonesMap.TryGetValue(gameObject, out Poolable poolable))
            {
                if (poolable._status == PoolableStatus.Despawned)
                {
#if DEBUG
                    if (poolable._pool._sendWarnings)
                    {
                        LogF8.LogEntity("您要取消生成的游戏对象已经被取消生成！", gameObject);
                    }
#endif
                    return;
                }

                if (delay > 0f)
                {
                    DespawnWithDelay(poolable, delay);
                }
                else
                {
                    DespawnImmediate(poolable);
                }
            }
            else
            {
#if DEBUG
                if (Warnings)
                {
                    LogF8.LogEntity($"'{gameObject}' 未使用 {nameof(GameObjectPool)}（或池已销毁）生成，并将被销毁！", gameObject);
                }
#endif
                Object.Destroy(gameObject, delay);
            }
        }

        private void DespawnWithDelay(Poolable poolable, float delay)
        {
            ReactionOnRepeatedDelayedDespawn reaction = ReactionOnRepeatedDelayedDespawn;

            if (reaction == ReactionOnRepeatedDelayedDespawn.Ignore)
            {
                CreateDespawnRequest(poolable, delay);
            }
            else
            {
                if (HasDespawnRequest(poolable, out int index))
                {
                    ref DespawnRequest request = ref DespawnRequests._components[index];

                    switch (reaction)
                    {
                        case ReactionOnRepeatedDelayedDespawn.ResetDelay:
                            ResetDespawnDelay(ref request, delay);
                            break;
                        case ReactionOnRepeatedDelayedDespawn.ResetDelayIfNewTimeIsLess:
                            ResetDespawnDelayIfNewTimeIsLess(ref request, delay);
                            break;
                        case ReactionOnRepeatedDelayedDespawn.ResetDelayIfNewTimeIsGreater:
                            ResetDespawnDelayIfNewTimeIsGreater(ref request, delay);
                            break;
                        case ReactionOnRepeatedDelayedDespawn.ThrowException:
#if DEBUG
                            if (HasDespawnRequest(poolable, out _))
                            {
                                LogF8.LogError("延迟取消生成请求已经存在于该克隆！", poolable._gameObject);
                            }
#endif
                            break;
                    }
                }
                else
                {
                    CreateDespawnRequest(poolable, delay);
                }
            }
        }

        private bool HasDespawnRequest(Poolable poolable, out int id)
        {
            for (int i = 0; i < DespawnRequests._count; i++)
            {
                if (DespawnRequests._components[i].Poolable == poolable)
                {
                    id = i;
                    return true;
                }
            }

            id = default;
            return false;
        }

        private void CreateDespawnRequest(Poolable poolable, float delay)
        {
            DespawnRequests.Add(new DespawnRequest
            {
                Poolable = poolable,
                TimeToDespawn = delay
            });
        }

        private void ResetDespawnDelay(ref DespawnRequest request, float delay)
        {
            request.TimeToDespawn = delay;
        }

        private void ResetDespawnDelayIfNewTimeIsLess(ref DespawnRequest request, float delay)
        {
            if (delay < request.TimeToDespawn)
            {
                request.TimeToDespawn = delay;
            }
        }

        private void ResetDespawnDelayIfNewTimeIsGreater(ref DespawnRequest request, float delay)
        {
            if (delay > request.TimeToDespawn)
            {
                request.TimeToDespawn = delay;
            }
        }

        private bool CanPerformPoolAction()
        {
            if (s_isApplicationQuitting)
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorSettings.enterPlayModeOptionsEnabled && s_instance == null)
                {
                    LogF8.LogError($"<{nameof(F8PoolGlobal)}> 实例为空！");
                }
#endif
                return false;
            }

            if (s_hasTheF8PoolInitialized == false)
            {
#if DEBUG
                if (Application.isPlaying == false)
                {
                    LogF8.LogError("在应用程序未运行时，您正在尝试执行生成或取消生成操作！");
                }
#endif
                InitializeTheF8Pool();
            }

            return true;
        }

        private void GetPositionAndRotationByParent(GameObject prefab, Transform parent,
            out Vector3 position, out Quaternion rotation)
        {
            if (parent != null)
            {
                Transform prefabTransform = prefab.transform;

                position = prefabTransform.position;
                rotation = prefabTransform.rotation;
            }
            else
            {
                position = Constants.DefaultPosition;
                rotation = Constants.DefaultRotation;
            }
        }

        private void SetupTransform(Poolable poolable, F8GameObjectPool pool, Vector3 position,
            Quaternion rotation, Transform parent = null, bool worldPositionStays = false)
        {
            if (s_f8PoolMode == F8PoolMode.Safety)
            {
                SetPoolableNullParent(poolable);
            }
            else
            {
                CheckPoolableForLightweightTransformSetup(pool, poolable);
            }

            poolable._transform.localScale = pool._regularPrefabScale;
            poolable._transform.SetPositionAndRotation(position, rotation);
            poolable._transform.SetParent(parent, worldPositionStays);
        }

        private void CheckPoolableForLightweightTransformSetup(F8GameObjectPool pool, Poolable poolable)
        {
            if (pool._behaviourOnCapacityReached == BehaviourOnCapacityReached.Recycle)
            {
                SetPoolableNullParent(poolable);
                return;
            }

            if (pool._despawnType == DespawnType.OnlyDeactivate)
            {
                SetPoolableNullParent(poolable);
                return;
            }
#if DEBUG
            if (poolable._pool._cachedTransform.lossyScale != Constants.Vector3One)
            {
                LogF8.LogError($"池及其父物体在 F8 池 '{nameof(F8PoolMode.Performance)}' 模式下必须具有相同的缩放，即 'Vector3.one'！",
                    poolable._pool);

                SetPoolableNullParent(poolable);
            }
#endif
        }

        private void SetPoolableNullParent(Poolable poolable)
        {
            poolable._transform.SetParent(null, false);
        }

        private void InvokeCallbacks<T>(GameObject gameObject, CallbacksType callbacksType,
            Action<T> poolableCallback, List<T> listForComponentsCaching, string messageKey)
        {
            switch (callbacksType)
            {
                case CallbacksType.Interfaces:
                    InvokeGameObjectPoolEvents(gameObject, listForComponentsCaching,
                        poolableCallback, inChildren: false);
                    break;
                case CallbacksType.InterfacesInChildren:
                    InvokeGameObjectPoolEvents(gameObject, listForComponentsCaching,
                        poolableCallback, inChildren: true);
                    break;
                case CallbacksType.SendMessage:
                    gameObject.SendMessage(messageKey, SendMessageOptions.DontRequireReceiver);
                    break;
                case CallbacksType.BroadcastMessage:
                    gameObject.BroadcastMessage(messageKey, SendMessageOptions.DontRequireReceiver);
                    break;
                case CallbacksType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(callbacksType));
            }
        }

        private void InvokeGameObjectPoolEvents<T>(GameObject gameObject, List<T> listForComponentCaching,
            Action<T> callback, bool inChildren)
        {
            if (inChildren)
                gameObject.GetComponentsInChildren(listForComponentCaching);
            else
                gameObject.GetComponents(listForComponentCaching);

            int count = listForComponentCaching.Count;

            for (int i = 0; i < count; i++)
            {
                callback.Invoke(listForComponentCaching[i]);
            }
        }

        private void DestroyPoolableWithGameObject(GameObject clone, bool immediately)
        {
            if (ClonesMap.TryGetValue(clone, out Poolable poolable))
            {
                if (poolable._isSetup)
                {
                    poolable._pool.UnregisterPoolable(poolable);
                    poolable.Dispose(immediately);
                }
#if DEBUG
                else
                {
                    LogF8.LogError($"克隆 '{clone}' 尚未设置！", clone);
                }
#endif
            }
            else
            {
#if DEBUG
                LogF8.LogEntity($"克隆 '{clone}' 并非由 {nameof(GameObjectPool)} 生成！", clone);
#endif
                Object.Destroy(clone);
            }
        }

        private void ResetLists()
        {
            ClearListAndSetCapacity(SpawnableItemComponents, Constants.DefaultPoolableInterfacesCapacity);
            ClearListAndSetCapacity(DespawnableItemComponents, Constants.DefaultPoolableInterfacesCapacity);
            ClearListAndSetCapacity(DespawnRequests, Constants.DefaultDespawnRequestsCapacity);
        }

        private void HandlePersistentPoolsOnDestroy()
        {
            if (s_isApplicationQuitting)
                return;

            if (s_despawnPersistentClonesOnDestroy == false)
                return;

            if (PersistentPoolsMap.Count == 0)
                return;

            foreach (F8GameObjectPool persistentPool in PersistentPoolsMap.Values)
            {
                persistentPool.DespawnAllClones();
            }
        }

        private void ResetClonesDictionary()
        {
            if (s_isApplicationQuitting)
            {
                ClonesMap.Clear();
            }
        }

        private void ClearListAndSetCapacity<T>(List<T> list, int capacity)
        {
            list.Clear();
            list.Capacity = capacity;
        }

        private void ClearListAndSetCapacity(F8PoolList<DespawnRequest> list, int capacity)
        {
            list.Clear();
            list.SetCapacity(capacity);
        }

        public void OnInit(object createParam)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnLateUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnTermination()
        {
            base.Destroy();
        }
    }
}


#if ENABLE_IL2CPP
namespace Unity.IL2CPP.CompilerServices
{
    internal enum Option
    {
        NullChecks = 1,
        ArrayBoundsChecks = 2,
        DivideByZeroChecks = 3,
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Delegate, Inherited
 = false, AllowMultiple = true)]
    internal class Il2CppSetOptionAttribute : Attribute
    {
        public Option Option { get; private set; }
        public object Value { get; private set; }

        public Il2CppSetOptionAttribute(Option option, object value)
        {
            Option = option;
            Value = value;
        }
    }
}
#endif