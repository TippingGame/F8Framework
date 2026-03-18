---
name: f8-features-storage-workflow
description: Use when implementing or troubleshooting Storage feature workflows — local data storage, reading, field-level AES encryption, whole-file Gzip compression, and file-path based save operations in F8Framework.
---

# Storage Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about local data persistence, key-value storage, or data encryption.
- The user asks about save/load, user-scoped data, file paths, collections/arrays, AES encryption, or Gzip compression.

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
// Configure first, then read/write
FF8.Storage.Configure(new StorageManager.Settings
{
    location = StorageManager.Location.File,
    directory = StorageManager.Directory.PersistentDataPath,
    defaultFilePath = "Save/PlayerData.json",
    compressionType = StorageManager.CompressionType.Gzip,
    encryption = new Util.OptimizedAES(key: "AES_Key", iv: null)
});

// Optional: still supported
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

// Generic Set/Get
FF8.Storage.Set("Key6", new[] { 1, 2, 3 });
int[] arr = FF8.Storage.Get<int[]>("Key6");

FF8.Storage.SetList("Key7", new List<string> { "A", "B" });
List<string> list = FF8.Storage.GetList<string>("Key7");

FF8.Storage.SetDictionary("Key8", new Dictionary<int, string> { { 1, "One" } });
Dictionary<int, string> dict = FF8.Storage.GetDictionary<int, string>("Key8");

FF8.Storage.SetQueue("Key9", new Queue<int>(new[] { 1, 2, 3 }));
Queue<int> queue = FF8.Storage.GetQueue<int>("Key9");

FF8.Storage.SetHashSet("Key10", new HashSet<int> { 1, 2, 3 });
HashSet<int> hashSet = FF8.Storage.GetHashSet<int>("Key10");

FF8.Storage.SetStack("Key11", new Stack<int>(new[] { 1, 2, 3 }));
Stack<int> stack = FF8.Storage.GetStack<int>("Key11");

FF8.Storage.SetRectangularArray("Key12", new int[,] { { 1, 2 }, { 3, 4 } });
int[,] grid = FF8.Storage.GetRectangularArray<int>("Key12");

FF8.Storage.SetJaggedArray("Key13", new int[][] { new[] { 1, 2 }, new[] { 3 } });
int[][] jagged = FF8.Storage.GetJaggedArray<int>("Key13");

// Save / remove / clear
FF8.Storage.Save();    // Flush to disk
FF8.Storage.Save("Save/BackupPlayerData.json");
FF8.Storage.Remove("Key2", filePath: "Save/BackupPlayerData.json");
FF8.Storage.Clear();   // Delete current storage
FF8.Storage.Clear("Save/TempPlayerData.json");
```

## Workflow

1. Initialize F8Framework before using `FF8.Storage`.
2. Configure `StorageManager.Settings` before the first read/write.
3. If needed, set `SetUser()` for per-user data isolation.
4. Use typed Set/Get methods for basic types.
5. Use `Get<T>/Set<T>` or collection helpers for arrays and generic collections.
6. Use `SetObject`/`GetObject` for complex data classes.
7. Call `Save()` to persist changes to disk.
8. Use `Save(filePath)` / `Remove(..., filePath)` / `Clear(filePath)` for explicit file operations.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Data not persisted | Forgot to call `Save()` | Always call `Save()` after writes |
| Decryption mismatch | Key changed between sessions | Use consistent encryption key |
| Read/write mismatch | Read happened before storage configuration | Configure storage before any dependent module loads data |
| Object deserialization fails | Class structure changed | Handle migration or catch exceptions |
| User data crossover | Missing `SetUser()` | Always set user before accessing user-scoped data |
| File not shrinking with Gzip | Compressing per-field instead of per-file | In current implementation, Gzip is whole-file in `File` mode |

## Cross-module dependencies

- **Tools (Encryption)**: Uses `Util.OptimizedAES` for encryption.

## Output checklist

- Storage strategy selected (`PlayerPrefs` / `File` / `Resources`).
- Settings configured before first read/write.
- Encryption configured if needed.
- Compression configured if needed.
- User scope set.
- Validation status and remaining risks.
