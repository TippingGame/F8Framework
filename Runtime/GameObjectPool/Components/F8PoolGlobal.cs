using System;
using UnityEngine;

namespace F8Framework.Core
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
    [AddComponentMenu(Constants.F8PoolComponentPath + "F8 Pool Global")]
#endif
    public sealed class F8PoolGlobal : SingletonMono<F8PoolGlobal>
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
        [SerializeField] internal bool _dontDestroyOnLoad;

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
        [SerializeField] private bool _checkForPrefab = true;
        
        [Tooltip(Constants.Tooltips.ClearEventsOnDestroy)]
        [SerializeField] private bool _clearEventsOnDestroy;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                FF8.GameObjectPool.s_f8PoolMode = _f8PoolMode;
                FF8.GameObjectPool.s_checkForPrefab = _checkForPrefab;
                FF8.GameObjectPool.s_checkClonesForNull = _checkClonesForNull;
                FF8.GameObjectPool.s_despawnPersistentClonesOnDestroy = _despawnPersistentClonesOnDestroy;
            }
        }
#endif
        protected override void Init()
        {
            Initialize();
            PreloadPools(PreloadType.OnAwake);
        }

        private void Start()
        {
            PreloadPools(PreloadType.OnStart);
        }

        private void Update()
        {
            if (_updateType == UpdateType.Update)
            {
                HandleDespawnRequests(Time.deltaTime);
            }
        }
        
        private void FixedUpdate()
        {
            if (_updateType == UpdateType.FixedUpdate)
            {
                HandleDespawnRequests(Time.fixedDeltaTime);
            }
        }
        
        private void LateUpdate()
        {
            if (_updateType == UpdateType.LateUpdate)
            {
                HandleDespawnRequests(Time.deltaTime);
            }
        }

        private void OnApplicationQuit()
        {
            FF8.GameObjectPool.s_isApplicationQuitting = true;
        }
        
        public override void OnQuitGame()
        {
            FF8.GameObjectPool.ResetPool();

            if (_clearEventsOnDestroy || FF8.GameObjectPool.s_isApplicationQuitting)
            {
                FF8.GameObjectPool.GameObjectInstantiated.Clear();
            }
        }

        private void Initialize()
        {
#if DEBUG
            if (FF8.GameObjectPool.s_instance != null && FF8.GameObjectPool.s_instance != this)
                throw new Exception($"场景中的 {nameof(FF8.GameObjectPool)} 实例数量大于一个！");

            if (enabled == false)
                LogF8.LogEntity($"<{nameof(F8PoolGlobal)}> 实例已禁用！" +
                                "因此，某些功能可能无法正常工作！", this);
#endif
            FF8.GameObjectPool.s_isApplicationQuitting = false;
            FF8.GameObjectPool.s_instance = this;
            FF8.GameObjectPool.s_hasTheF8PoolInitialized = true;
            FF8.GameObjectPool.s_f8PoolMode = _f8PoolMode;
            FF8.GameObjectPool.s_checkForPrefab = _checkForPrefab;
            FF8.GameObjectPool.s_checkClonesForNull = _checkClonesForNull;
            FF8.GameObjectPool.s_despawnPersistentClonesOnDestroy = _despawnPersistentClonesOnDestroy;
        }

        private void PreloadPools(PreloadType requiredType)
        {
            if (requiredType != preloadPoolsType)
                return;
            
            FF8.GameObjectPool.InstallPools(poolsPreset);
        }

        private void HandleDespawnRequests(float deltaTime)
        {
            for (int i = 0; i < FF8.GameObjectPool.DespawnRequests._count; i++)
            {
                ref DespawnRequest request = ref FF8.GameObjectPool.DespawnRequests._components[i];

                if (request.Poolable._status == PoolableStatus.Despawned)
                {
                    FF8.GameObjectPool.DespawnRequests.RemoveUnorderedAt(i);
                    continue;
                }
                
                request.TimeToDespawn -= deltaTime;
                
                if (request.TimeToDespawn <= 0f)
                {
                    FF8.GameObjectPool.DespawnImmediate(request.Poolable);
                    FF8.GameObjectPool.DespawnRequests.RemoveUnorderedAt(i);
                }
            }
        }
    }
}