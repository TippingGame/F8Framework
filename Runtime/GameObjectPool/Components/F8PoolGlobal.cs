using System;
using UnityEngine;

namespace F8Framework.Core
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    [UpdateRefresh]
    [FixedUpdateRefresh]
    [LateUpdateRefresh]
    public sealed class F8PoolGlobal : ModuleSingletonMono<F8PoolGlobal>, IModule
    {
        [Header("Main")] 
        [Tooltip(Constants.Tooltips.GlobalUpdateType)]
        [SerializeField] private UpdateType _updateType = UpdateType.Update;
        
        [Header("Preload Pools")]
        [Tooltip(Constants.Tooltips.GlobalPreloadType)]
        [SerializeField] private PreloadType preloadPoolsType = PreloadType.Disabled;
        
        [Tooltip(Constants.Tooltips.PoolsToPreload)]
        [SerializeField] private PoolsPreset poolsPreset;

        [Header("Global Pool Settings")] 
        [Tooltip(Constants.Tooltips.OverflowBehaviour)]
        [SerializeField] internal BehaviourOnCapacityReached _behaviourOnCapacityReached = Constants.DefaultBehaviourOnCapacityReached;
        
        [Tooltip(Constants.Tooltips.DespawnType)]
        [SerializeField] internal DespawnType _despawnType = Constants.DefaultDespawnType;
        
        [Tooltip(Constants.Tooltips.CallbacksType)]
        [SerializeField] internal CallbacksType _callbacksType = Constants.DefaultCallbacksType;
        
        [Tooltip(Constants.Tooltips.Capacity)]
        [SerializeField, Min(0)] internal int _capacity = 64;
        
        [Tooltip(Constants.Tooltips.Persistent)]
        [SerializeField] internal bool _dontDestroyOnLoad = true;

        [Tooltip(Constants.Tooltips.Warnings)]
        [SerializeField] internal bool _sendWarnings = true;
        
        [Header("Safety")] 
        [Tooltip(Constants.Tooltips.F8PoolMode)]
        [SerializeField] internal F8PoolMode _f8PoolMode = Constants.DefaultF8PoolMode;
        
        [Tooltip(Constants.Tooltips.DelayedDespawnReaction)]
        [SerializeField] internal ReactionOnRepeatedDelayedDespawn _reactionOnRepeatedDelayedDespawn = Constants.DefaultDelayedDespawnHandleType;
        
        [Tooltip(Constants.Tooltips.DespawnPersistentClonesOnDestroy)]
        [SerializeField] private bool _despawnPersistentClonesOnDestroy = true;
        
        [Tooltip(Constants.Tooltips.CheckClonesForNull)]
        [SerializeField] private bool _checkClonesForNull = true;
        
        [Tooltip(Constants.Tooltips.CheckForPrefab)]
        [SerializeField] private bool _checkForPrefab = false;
        
        [Tooltip(Constants.Tooltips.ClearEventsOnDestroy)]
        [SerializeField] private bool _clearEventsOnDestroy;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                GameObjectPool.Instance.s_f8PoolMode = _f8PoolMode;
                GameObjectPool.Instance.s_checkForPrefab = _checkForPrefab;
                GameObjectPool.Instance.s_checkClonesForNull = _checkClonesForNull;
                GameObjectPool.Instance.s_despawnPersistentClonesOnDestroy = _despawnPersistentClonesOnDestroy;
            }
        }
#endif
        
        public void OnInit(object createParam)
        {
            Initialize();
            PreloadPools(PreloadType.OnAwake);
        }

        public void OnUpdate()
        {
            if (_updateType == UpdateType.Update)
            {
                HandleDespawnRequests(Time.deltaTime);
            }
        }

        public void OnLateUpdate()
        {
            if (_updateType == UpdateType.LateUpdate)
            {
                HandleDespawnRequests(Time.deltaTime);
            }
        }

        public void OnFixedUpdate()
        {
            if (_updateType == UpdateType.FixedUpdate)
            {
                HandleDespawnRequests(Time.fixedDeltaTime);
            }
        }

        public void OnTermination()
        {
            GameObjectPool.Instance.ResetPool();

            if (_clearEventsOnDestroy || GameObjectPool.Instance.s_isApplicationQuitting)
            {
                GameObjectPool.Instance.GameObjectInstantiated.Clear();
            }
            
            Destroy(gameObject);
        }

        private void Start()
        {
            PreloadPools(PreloadType.OnStart);
        }

        private void OnApplicationQuit()
        {
            GameObjectPool.Instance.s_isApplicationQuitting = true;
        }

        private void Initialize()
        {
#if DEBUG
            if (GameObjectPool.Instance.s_instance != null && GameObjectPool.Instance.s_instance != this)
                LogF8.LogError($"场景中的 {nameof(GameObjectPool)} 实例数量大于一个！");

            if (enabled == false)
                LogF8.LogEntity($"<{nameof(F8PoolGlobal)}> 实例已禁用！" +
                                "因此，某些功能可能无法正常工作！", this);
#endif
            GameObjectPool.Instance.s_isApplicationQuitting = false;
            GameObjectPool.Instance.s_instance = this;
            GameObjectPool.Instance.s_hasTheF8PoolInitialized = true;
            GameObjectPool.Instance.s_f8PoolMode = _f8PoolMode;
            GameObjectPool.Instance.s_checkForPrefab = _checkForPrefab;
            GameObjectPool.Instance.s_checkClonesForNull = _checkClonesForNull;
            GameObjectPool.Instance.s_despawnPersistentClonesOnDestroy = _despawnPersistentClonesOnDestroy;
        }

        private void PreloadPools(PreloadType requiredType)
        {
            if (requiredType != preloadPoolsType)
                return;
            
            GameObjectPool.Instance.InstallPools(poolsPreset);
        }

        private void HandleDespawnRequests(float deltaTime)
        {
            for (int i = 0; i < GameObjectPool.Instance.DespawnRequests._count; i++)
            {
                ref DespawnRequest request = ref GameObjectPool.Instance.DespawnRequests._components[i];

                if (request.Poolable._status == PoolableStatus.Despawned)
                {
                    GameObjectPool.Instance.DespawnRequests.RemoveUnorderedAt(i);
                    continue;
                }
                
                request.TimeToDespawn -= deltaTime;
                
                if (request.TimeToDespawn <= 0f)
                {
                    GameObjectPool.Instance.DespawnImmediate(request.Poolable);
                    GameObjectPool.Instance.DespawnRequests.RemoveUnorderedAt(i);
                }
            }
        }
    }
}