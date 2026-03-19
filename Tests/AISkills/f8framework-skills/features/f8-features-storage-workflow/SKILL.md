---
name: f8-features-storage-workflow
description: Use when implementing, documenting, or troubleshooting Storage workflows in F8Framework, including local persistence, user-scoped keys, AES encryption, Gzip compression, generic collections, and common Unity value types.
---

# Storage Feature Workflow

> **Important**: Before using any F8Framework feature, initialize the framework first. Make sure `ModuleCenter.Initialize(this);` has been called and the required modules are created in the launcher flow.

## Use this skill when

- The task is about local save/load, persistence, storage paths, or per-user save data.
- The user asks about `FF8.Storage`, `StorageManager`, AES encryption, compression, or file-based save operations.
- The task needs storage examples, troubleshooting guidance, or docs updates.

## Path resolution

1. Prefer runtime source under `Assets/F8Framework/Runtime/Storage`.
2. Prefer examples under `Assets/F8Framework/Tests/Storage`.
3. If docs need updates, keep these files aligned:
   - `Assets/F8Framework/Tests/Storage/DemoStorage.cs`
   - `Assets/F8Framework/Tests/Storage/README.md`
   - `Assets/F8Framework/Tests/Storage/README_EN.md`

## Sources of truth

- Runtime module: `Assets/F8Framework/Runtime/Storage/StorageManager.cs`
- Storage tests and docs: `Assets/F8Framework/Tests/Storage`

## Key classes

| Class | Role |
|-------|------|
| `StorageManager` | Core storage module, accessed via `FF8.Storage`. |
| `StorageManager.Settings` | Runtime storage configuration. |
| `Util.OptimizedAES` | AES configuration used by `StorageManager`. |

## Initialization checklist

1. Initialize module center.
2. Create `StorageManager` before first storage access.
3. Configure storage before any dependent read/write.
4. If user-isolated keys are required, call `SetUser()` first.
5. Call `Save()` after writes in `File` mode.

## API quick reference

```csharp
FF8.Storage.Configure(new StorageManager.Settings
{
    location = StorageManager.Location.File,
    directory = StorageManager.Directory.PersistentDataPath,
    defaultFilePath = "Save/PlayerData.json",
    compressionType = StorageManager.CompressionType.Gzip,
    encryption = new Util.OptimizedAES(key: "AES_Key", iv: null)
});

FF8.Storage.SetUser("User_10001");

// Primitive types
FF8.Storage.SetString("nickname", "F8Player", user: true);
FF8.Storage.SetInt("coins", 2560);
FF8.Storage.SetFloat("music_volume", 0.75f);
FF8.Storage.SetBool("guide_finished", true);

// Extended numeric types
FF8.Storage.SetChar("grade", 'S');
FF8.Storage.SetByte("byte_value", 128);
FF8.Storage.SetShort("short_value", 1024);
FF8.Storage.SetLong("long_value", 9876543210L);
FF8.Storage.SetDouble("double_value", 12.3456789d);
FF8.Storage.SetDecimal("decimal_value", 99.99m);
FF8.Storage.SetSByte("sbyte_value", -12);
FF8.Storage.SetUShort("ushort_value", 65530);
FF8.Storage.SetUInt("uint_value", 1234567890U);
FF8.Storage.SetULong("ulong_value", 1234567890123456789UL);

// Enum and tuple
FF8.Storage.SetEnum("save_mode", SaveMode.Hardcore);
FF8.Storage.SetValueTuple("tuple2", (1, "tuple"));
FF8.Storage.SetValueTuple("tuple7", (1, 2, 3, 4, 5, 6, 7));

// Object and collections
FF8.Storage.SetObject("profile", profile);
FF8.Storage.Set("int_array", new[] { 1, 2, 3, 4 });
FF8.Storage.SetList("string_list", new List<string> { "A", "B", "C" });
FF8.Storage.SetDictionary("map", new Dictionary<int, string> { { 1, "one" } });
FF8.Storage.SetQueue("queue", new Queue<int>(new[] { 10, 20, 30 }));
FF8.Storage.SetHashSet("hashset", new HashSet<string> { "fire", "ice" });
FF8.Storage.SetStack("stack", new Stack<int>(new[] { 7, 8, 9 }));
FF8.Storage.SetRectangularArray("grid", new int[,] { { 1, 2 }, { 3, 4 } });
FF8.Storage.SetJaggedArray("jagged", new int[][] { new[] { 1, 2 }, new[] { 3, 4, 5 } });

// Unity types currently exposed by StorageManager.cs
FF8.Storage.SetVector2("vector2", new Vector2(1.5f, 2.5f));
FF8.Storage.SetVector3("vector3", new Vector3(3f, 4f, 5f));
FF8.Storage.SetVector4("vector4", new Vector4(6f, 7f, 8f, 9f));
FF8.Storage.SetVector2Int("vector2int", new Vector2Int(10, 11));
FF8.Storage.SetVector3Int("vector3int", new Vector3Int(12, 13, 14));
FF8.Storage.SetQuaternion("rotation", Quaternion.Euler(15f, 30f, 45f));
FF8.Storage.SetColor("color", new Color(0.1f, 0.2f, 0.3f, 1f));
FF8.Storage.SetColor32("color32", new Color32(10, 20, 30, 255));
FF8.Storage.SetMatrix4x4("matrix", Matrix4x4.identity);
FF8.Storage.SetBounds("bounds", new Bounds(Vector3.zero, Vector3.one * 2));
FF8.Storage.SetRect("rect", new Rect(5f, 10f, 100f, 50f));
FF8.Storage.SetRectOffset("rect_offset", new RectOffset(1, 2, 3, 4));
FF8.Storage.SetLayerMask("layer_mask", (LayerMask)(1 << 8));

// Save / remove / clear
FF8.Storage.Save();
FF8.Storage.Save("Save/PlayerData.backup.json");
FF8.Storage.Remove("coins", filePath: "Save/PlayerData.backup.json");
FF8.Storage.Clear("Save/TempPlayerData.json");
```

## Supported categories in current StorageManager.cs

- Storage backends:
  - `PlayerPrefs`
  - `File`
  - `Resources` (read-only at runtime)
- Compression:
  - `None`
  - `Gzip`
- Primitive APIs:
  - `string`, `int`, `float`, `bool`
  - `char`, `byte`, `short`, `long`, `double`, `decimal`
  - `sbyte`, `ushort`, `uint`, `ulong`
- Generic helpers:
  - `Get<T>/Set<T>`
  - `GetObject<T>/SetObject<T>`
  - `GetEnum<TEnum>/SetEnum<TEnum>`
  - `GetValueTuple/SetValueTuple` for 1 to 7 items
- Collections:
  - arrays
  - `List`
  - `Dictionary`
  - `Queue`
  - `HashSet`
  - `Stack`
  - rectangular arrays
  - jagged arrays
- Unity types currently surfaced by `StorageManager`:
  - `Vector2`
  - `Vector3`
  - `Vector4`
  - `Vector2Int`
  - `Vector3Int`
  - `Quaternion`
  - `Color`
  - `Color32`
  - `Matrix4x4`
  - `Bounds`
  - `Rect`
  - `RectOffset`
  - `LayerMask`

## Workflow

1. Confirm framework initialization and module creation order.
2. Confirm storage backend and file path strategy.
3. Configure `StorageManager.Settings`.
4. Set user scope if required.
5. Prefer typed APIs for primitive and Unity value types.
6. Use `SetObject/GetObject` for plain data classes.
7. Use collection helpers for lists, maps, queues, stacks, and arrays.
8. Call `Save()` after writes when using file storage.
9. If behavior differs in build vs editor, inspect whether the type relies on LitJson reflection for object deserialization.

## Build-time caution

- Unity built-in structs serialized through LitJson `SetObject/GetObject` may behave differently in player builds if deserialization falls back to reflection-based property assignment.
- If a Unity type works in editor but fails in build, prefer one of these:
  - store it via a dedicated typed API already exposed by `StorageManager`
  - convert it to a plain DTO with primitive fields before `SetObject`
  - store it as explicit primitive components instead of relying on LitJson object reflection

## Common troubleshooting

| Problem | Likely cause | Recommended fix |
|--------|--------------|-----------------|
| Data not persisted | `Save()` was not called in file mode | Call `Save()` after writes |
| User data overlaps | `SetUser()` missing before user-scoped keys | Set user id first |
| Read/write mismatch | Storage configured after first access | Configure before any storage usage |
| Decryption fails | AES key changed between sessions | Keep encryption config stable |
| Queue or Stack shape is wrong | LitJson collection support mismatch | Check `JsonMapper.g.cs` collection handling |
| Unity type works in editor but fails in build | Reflection-based value-type deserialization | Use DTO or primitive-component storage |

## Output checklist

- Storage backend chosen.
- Path strategy defined.
- Compression choice defined.
- Encryption choice defined.
- User scope handled if needed.
- Example code updated if docs changed.
- Build-time risks called out when Unity value types rely on LitJson reflection.
