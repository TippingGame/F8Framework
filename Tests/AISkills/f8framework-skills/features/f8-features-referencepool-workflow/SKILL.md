---
name: f8-features-referencepool-workflow
description: Use when implementing or troubleshooting ReferencePool feature workflows — C# object pooling via IReference interface in F8Framework.
---

# ReferencePool Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first.


## Use this skill when

- The task is about C# object pooling (not GameObject pooling).
- The user asks about reducing GC allocation for frequently created/destroyed objects.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/ReferencePool/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/ReferencePool
- Test docs: Assets/F8Framework/Tests/ReferencePool

## Key classes and interfaces

| Class | Role |
|-------|------|
| `ReferencePool` | Static pool manager. Add, Acquire, Release, Remove. |
| `IReference` | Interface for poolable objects. Must implement `Clear()`. |

## API quick reference

```csharp
// Implement IReference
public class AssetInfo : IReference
{
    public string Path;
    public void Clear()
    {
        Path = null; // Reset state for reuse
    }
}

// Pre-populate pool
ReferencePool.Add<AssetInfo>(50);

// Acquire from pool (creates if empty)
AssetInfo info = ReferencePool.Acquire<AssetInfo>();

// Release back to pool (calls Clear() automatically)
ReferencePool.Release(info);

// Remove all pooled instances of type
ReferencePool.RemoveAll(typeof(AssetInfo));
```

## Workflow

1. Implement `IReference` on classes that are frequently allocated.
2. Implement `Clear()` to reset all fields to default values.
3. Optionally pre-populate with `Add<T>(count)`.
4. Use `Acquire<T>()` instead of `new T()`.
5. Use `Release(obj)` instead of letting GC collect.
6. Clean up with `RemoveAll()` when pool is no longer needed.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Stale data | `Clear()` doesn't reset all fields | Reset all fields in `Clear()` |
| Double release | Releasing same object twice | Track released state or null reference after release |

## Cross-module dependencies

- Used internally by various F8 modules for object reuse.

## Output checklist

- `IReference` implemented with proper `Clear()`.
- Pool size appropriate for use case.
- Validation status and remaining risks.
