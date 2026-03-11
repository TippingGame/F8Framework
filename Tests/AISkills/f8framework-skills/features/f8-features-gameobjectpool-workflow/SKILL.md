---
name: f8-features-gameobjectpool-workflow
description: Use when implementing or troubleshooting GameObjectPool feature workflows — object pooling, preloading, spawn/despawn, and lifecycle events in F8Framework.
---

# GameObjectPool Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about GameObject pooling, preloading, and spawn/despawn.
- The user needs to manage pooled object lifecycle events.
- Troubleshooting pool exhaustion, capacity overflow, or despawn issues.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. For usage docs, read: Assets/F8Framework/Tests/GameObjectPool/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/GameObjectPool
- Editor module: Assets/F8Framework/Editor/GameObjectPool
- Test docs: Assets/F8Framework/Tests/GameObjectPool

## Key classes and interfaces

| Class | Role |
|-------|------|
| `GameObjectPoolManager` | Core module. Access via `FF8.GameObjectPool`. |
| `F8GameObjectPool` | Individual pool instance with capacity, callbacks, and settings. |
| `PoolsPreset` | ScriptableObject for pre-configured pool definitions. |
| `IPoolable` | Interface for spawn/despawn callbacks on pooled objects. |
| `PoolableStatus` | Enum: Spawned / Despawned / SpawnedOverCapacity. |

## API quick reference

### Spawn and despawn
```csharp
// Spawn by name, prefab, or component
GameObject clone = FF8.GameObjectPool.Spawn("prefabName");
GameObject clone = FF8.GameObjectPool.Spawn(prefab);
MyComp comp = FF8.GameObjectPool.Spawn(compPrefab, Vector3.zero, Quaternion.identity, parent);

// Despawn (with optional delay)
FF8.GameObjectPool.Despawn(clone, delay: 0.5f);

// Particle auto-despawn on complete
FF8.GameObjectPool.Spawn(particlePrefab).DespawnOnComplete();
```

### Pool management
```csharp
// Install preset pools (from ScriptableObject)
FF8.GameObjectPool.InstallPools(poolsPreset);

// Get pool reference
F8GameObjectPool pool = FF8.GameObjectPool.GetPoolByPrefab(prefab);
F8GameObjectPool pool = FF8.GameObjectPool.GetPoolByPrefabName("PrefabName");

// Clone status
PoolableStatus status = FF8.GameObjectPool.GetCloneStatus(clone);
bool isClone = FF8.GameObjectPool.IsClone(clone);

// For each clone/pool
FF8.GameObjectPool.ForEachPool((pool) => { });
FF8.GameObjectPool.ForEachClone((clone) => { });

// Safe destroy (avoids pool errors)
FF8.GameObjectPool.DestroyClone(clone);
FF8.GameObjectPool.DestroyAllPools(immediately: false);
```

### Pool configuration
```csharp
pool.Init();                            // Init with cached prefab
pool.Init(prefab);
pool.PopulatePool(16);                  // Pre-instantiate
pool.SetCapacity(32);                    // Max capacity
pool.SetBehaviourOnCapacityReached(BehaviourOnCapacityReached.Recycle);
pool.SetDespawnType(DespawnType.DeactivateAndHide);
pool.SetCallbacksType(CallbacksType.Interfaces);
pool.SetWarningsActive(true);
```

### Pool operations
```csharp
pool.ForEachClone((obj) => { });
pool.ForEachSpawnedClone((obj) => { });
pool.ForEachDespawnedClone((obj) => { });
pool.DestroySpawnedClones();
pool.DestroyDespawnedClones();
pool.DestroyAllClones();
pool.DestroySpawnedClonesImmediate();
pool.DestroyDespawnedClonesImmediate();
pool.DestroyAllClonesImmediate();
pool.DespawnAllClones();
pool.DestroyPool();
pool.DestroyPoolImmediate();
pool.Clear();
```

### Lifecycle events
```csharp
pool.GameObjectInstantiated.AddListener(OnInstantiated);
pool.GameObjectSpawned.AddListener(OnSpawned);
pool.GameObjectDespawned.AddListener(OnDespawned);
```

### IPoolable interface
```csharp
public class MyPooledObject : MonoBehaviour, IPoolable
{
    public void OnSpawn() { /* Reset state */ }
    public void OnDespawn() { /* Cleanup */ }
}
```

## Workflow

1. Create prefabs to pool or define PoolsPreset ScriptableObjects.
2. Optionally pre-populate pools with `PopulatePool()`.
3. Spawn objects via `FF8.GameObjectPool.Spawn()`.
4. Despawn to return objects to pool instead of Destroy.
5. Implement `IPoolable` on pooled objects for reset/cleanup callbacks.
6. Set capacity and overflow behavior for production use.
7. Use `DespawnOnComplete()` for particles.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Pool exhausted | All clones in use, no recycle policy | Set `BehaviourOnCapacityReached.Recycle` or increase capacity |
| Destroy/Despawn mismatch | Using `Destroy()` on pooled object | Always use `Despawn()` or `DestroyClone()` |
| OnSpawn not called | IPoolable not implemented or wrong CallbacksType | Implement `IPoolable` and set `CallbacksType.Interfaces` |

## Cross-module dependencies

- **AssetManager**: Pool prefabs can be loaded via AssetManager.

## Output checklist

- Pool strategy defined (preload count, capacity, overflow behavior).
- Spawn/Despawn lifecycle verified.
- Files changed and why.
- Validation status and remaining risks.
