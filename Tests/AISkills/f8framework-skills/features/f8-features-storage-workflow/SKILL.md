---
name: f8-features-storage-workflow
description: Use when implementing or troubleshooting Storage feature workflows — local data storage, reading, and encryption in F8Framework.
---

# Storage Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about local data persistence, key-value storage, or data encryption.
- The user asks about save/load, user-scoped data, or AES encryption.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Storage/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Storage
- Test docs: Assets/F8Framework/Tests/Storage

## Key classes and interfaces

| Class | Role |
|-------|------|
| `StorageManager` | Core module. Access via `FF8.Storage`. |
| `Util.OptimizedAES` | AES encryption utility for data protection. |

## API quick reference

```csharp
// Optional: enable encryption (auto-encrypts all data, Editor excluded)
FF8.Storage.SetEncrypt(new Util.OptimizedAES(key: "AES_Key", iv: null)); // null = random IV

// Set user scope (user-private keys)
FF8.Storage.SetUser("12345");

// Basic types
FF8.Storage.SetString("Key1", "value", user: true);  // user-scoped
FF8.Storage.GetString("Key1", "", user: true);

FF8.Storage.SetInt("Key2", 1);
FF8.Storage.GetInt("Key2");

FF8.Storage.SetBool("Key3", true);
FF8.Storage.GetBool("Key3");

FF8.Storage.SetFloat("Key4", 1.1f);
FF8.Storage.GetFloat("Key4");

// Complex objects (serialized as JSON)
FF8.Storage.SetObject("Key5", myObject);
MyClass obj = FF8.Storage.GetObject<MyClass>("Key5");

// Save and clear
FF8.Storage.Save();    // Flush to disk
FF8.Storage.Clear();   // Delete all data
```

## Workflow

1. Optionally configure encryption with `SetEncrypt()` early in startup.
2. Set user ID with `SetUser()` for per-user data isolation.
3. Use typed Set/Get methods for basic types.
4. Use `SetObject`/`GetObject` for complex data classes.
5. Call `Save()` to persist changes to disk.
6. Use `Clear()` for data reset (e.g., logout, account switch).

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Data not persisted | Forgot to call `Save()` | Always call `Save()` after writes |
| Decryption mismatch | Key changed between sessions | Use consistent encryption key |
| Object deserialization fails | Class structure changed | Handle migration or catch exceptions |
| User data crossover | Missing `SetUser()` | Always set user before accessing user-scoped data |

## Cross-module dependencies

- **Tools (Encryption)**: Uses `Util.OptimizedAES` for encryption.

## Output checklist

- Storage strategy selected (basic types / objects).
- Encryption configured if needed.
- User scope set.
- Validation status and remaining risks.
