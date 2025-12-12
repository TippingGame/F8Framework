# F8 GameObjectPool

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 GameObjectPool Component**  
1. Preload Pooling - Initialize objects in advance for instant access
2. Core Operations:
   * Spawn - Get objects from pool
   * Despawn - Return objects to pool
   * DespawnDelay - Return objects after delay
3. Lifecycle Event Listening - Monitor object state changes

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
// Prefab or component
private GameObject _gameObjectPrefab;
private DemoGameObjectPool _componentPrefab;
// Particle effect
private ParticleSystem _particleSystemPrefab;
// Pool preset ScriptableObject
private PoolsPreset _poolsPreset;

void Start()
{
    /*------------------------------Using GameObjectPool------------------------------*/
    
    // Create objects using name, GameObject or Component
    GameObject spawnedClone = FF8.GameObjectPool.Spawn("name");
    GameObject spawnedClone = FF8.GameObjectPool.Spawn(_gameObjectPrefab);
    DemoGameObjectPool component = FF8.GameObjectPool.Spawn(_componentPrefab, Vector3.zero, Quaternion.identity, this.transform);
    
    // Destroy with delay
    FF8.GameObjectPool.Despawn(gameObject, delay: 0.5f);
    
    // Auto-despawn particle system after complete
    FF8.GameObjectPool
        .Spawn(_particleSystemPrefab)
        .DespawnOnComplete();
    
    // Generate preset GameObjectPools (_poolsPreset can be loaded using the resource module)
    FF8.GameObjectPool.InstallPools(_poolsPreset);
    
    // Get pool reference
    F8GameObjectPool _pool = FF8.GameObjectPool.GetPoolByPrefab(_gameObjectPrefab);
    F8GameObjectPool _pool = FF8.GameObjectPool.GetPoolByPrefabName(_gameObjectPrefab.name);
    
    // Execute action for each pool
    FF8.GameObjectPool.ForEachPool((obj) => { });

    // Execute action for each clone
    FF8.GameObjectPool.ForEachClone((obj) => { });

    // Get clone status (Spawned/Despawned/OverCapacity)
    PoolableStatus cloneStatus = FF8.GameObjectPool.GetCloneStatus(spawnedClone);

    // Check if object is pool clone
    bool isClone = FF8.GameObjectPool.IsClone(spawnedClone);
    
    // Destroy clone without recycling (use to avoid errors)
    FF8.GameObjectPool.DestroyClone(spawnedClone);
    
    // Destroy all pools
    FF8.GameObjectPool.DestroyAllPools(immediately: false);
    
    
    
    /*------------------------------Pool Management Functions------------------------------*/
    
    // Manual pool initialization (needed if accessing pool before Awake)
    _pool.Init();
    _pool.Init(_gameObjectPrefab);

    // Prepopulate pool
    _pool.PopulatePool(16);
    
    // Set pool capacity
    _pool.SetCapacity(32);
    
    // Set overflow behavior
    _pool.SetBehaviourOnCapacityReached(BehaviourOnCapacityReached.Recycle);
    
    // Set despawn behavior
    _pool.SetDespawnType(DespawnType.DeactivateAndHide);
    
    // Set callback type
    _pool.SetCallbacksType(CallbacksType.Interfaces);
    
    // Toggle warnings
    _pool.SetWarningsActive(true);
    
    // Execute action for each clone
    _pool.ForEachClone((obj) => { });
    
    // Execute action for spawned clones
    _pool.ForEachSpawnedClone((obj) => { });
    
    // Execute action for despawned clones
    _pool.ForEachDespawnedClone((obj) => { });
    
    // Destroy spawned clones
    _pool.DestroySpawnedClones();
    _pool.DestroyDespawnedClones();
    _pool.DestroyAllClones();

    // Immediate destruction
    _pool.DestroySpawnedClonesImmediate();
    _pool.DestroyDespawnedClonesImmediate();
    _pool.DestroyAllClonesImmediate();
    
    // Pool destruction
    _pool.DestroyPool();
    _pool.DestroyPoolImmediate();
    
    // Despawn all clones
    _pool.DespawnAllClones();

    // Clear pool
    _pool.Clear();
    
    // Pool events
    void DoSomething(GameObject go)
    {
        
    }
    // Event listeners
    _pool.GameObjectInstantiated.AddListener(DoSomething);
    _pool.GameObjectSpawned.AddListener(DoSomething);
    _pool.GameObjectDespawned.AddListener(DoSomething);
    // Remove listeners
    _pool.GameObjectInstantiated.RemoveListener(DoSomething);
    _pool.GameObjectSpawned.RemoveListener(DoSomething);
    _pool.GameObjectDespawned.RemoveListener(DoSomething);
}

/*------------------------------IPoolable Callbacks------------------------------*/
public class DemoPoolCallBack : MonoBehaviour, IPoolable
{
    public void OnSpawn()
    {
        // Called when object is spawned
    }

    public void OnDespawn()
    {
        // Called when object is despawned
    }
}
```
## Extended Features
1. Using Preloaded Pools
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/GameObjectPool/ui_20240302154233.png)
