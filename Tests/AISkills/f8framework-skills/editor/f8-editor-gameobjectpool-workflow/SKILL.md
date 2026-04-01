---
name: f8-editor-gameobjectpool-workflow
description: Use when working with GameObjectPool editor tools — preload pool Inspector, PoolsPreset ScriptableObject creation in F8Framework.
---

# GameObjectPool Editor Workflow



## Use this skill when

- The task involves configuring preload pools in the Inspector.
- The user asks about PoolsPreset ScriptableObject or pool setup in Editor.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/GameObjectPool

## Sources of truth

- Editor module: Assets/F8Framework/Editor/GameObjectPool
- Test docs: Assets/F8Framework/Tests/GameObjectPool/README.md

## Key editor features

| Feature | Description |
|---------|-------------|
| **Preload Pool Inspector** | Configure pool size, capacity, and prefab references visually |
| **PoolsPreset Asset** | ScriptableObject defining a set of pools to install at startup |

## Workflow

1. Create a `PoolsPreset` ScriptableObject via right-click → Create.
2. Add prefab references and configure pool size/capacity per prefab.
3. Install at runtime: `FF8.GameObjectPool.InstallPools(poolsPreset)`.
4. Alternatively, configure pools via code with `PopulatePool()` and `SetCapacity()`.
5. Use the Inspector to adjust pool settings visually during development.

## Output checklist

- PoolsPreset configured with appropriate prefabs.
- Pool sizes tuned for target scene.
- Inspector validation passed.
