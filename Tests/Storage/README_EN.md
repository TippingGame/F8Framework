# F8 StorageManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction
`StorageManager` is used for local save/load workflows. It supports `PlayerPrefs`, file storage, and `Resources`, plus whole-file compression, field-level AES encryption, generic APIs, and common Unity types.

## Installation
Built into F8Framework Core:  
https://github.com/TippingGame/F8Framework.git

Import options:
- Download the project and import it into Unity
- `Unity -> Window -> Package Manager -> + -> Add Package from git URL`

## Features
- Supports `PlayerPrefs`, `File`, and `Resources`
- Supports `Location`, `Directory`, `DefaultFilePath`, and `Compression`
- Supports `None` and `Gzip`
- Supports field-level AES encryption
- Supports primitive types, `Enum`, and `ValueTuple`
- Supports `Array`, `List`, `Dictionary`, `Queue`, `HashSet`, and `Stack`
- Supports rectangular arrays and jagged arrays
- Supports common Unity types:
  - `Vector2`, `Vector3`, `Vector4`
  - `Vector2Int`, `Vector3Int`
  - `Quaternion`
  - `Color`, `Color32`
  - `Matrix4x4`
  - `Bounds`, `Rect`, `RectOffset`
  - `LayerMask`
- `Remove`, `Save`, and `Clear` can use a temporary file path

## Quick Start
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

FF8.Storage.SetInt("coins", 2560);

FF8.Storage.Save();
```

## Code Example
```csharp
using System;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

public class DemoStorage : MonoBehaviour
{
    [Serializable]
    public class SaveProfile
    {
        public string Name;
        public int Level;
        public List<int> OwnedItems;
    }

    public enum SaveMode
    {
        Casual = 1,
        Hardcore = 2
    }

    void Start()
    {
        FF8.Storage.Configure(new StorageManager.Settings
        {
            location = StorageManager.Location.File,
            directory = StorageManager.Directory.PersistentDataPath,
            defaultFilePath = "Save/PlayerData.json",
            compressionType = StorageManager.CompressionType.Gzip,
            encryption = new Util.OptimizedAES(key: "AES_Key", iv: null)
        });

        FF8.Storage.SetString("nickname", "F8Player", user: true);
        FF8.Storage.SetInt("coins", 2560);
        FF8.Storage.SetBool("guide_finished", true);
        FF8.Storage.SetEnum("save_mode", SaveMode.Hardcore);

        FF8.Storage.SetObject("profile", new SaveProfile
        {
            Name = "Knight",
            Level = 18,
            OwnedItems = new List<int> { 1001, 1002, 1003 }
        });

        FF8.Storage.Set("int_array", new[] { 1, 2, 3, 4 });
        FF8.Storage.SetDictionary("map", new Dictionary<int, string> { { 1, "one" }, { 2, "two" } });
        FF8.Storage.SetQueue("queue", new Queue<int>(new[] { 10, 20, 30 }));
        FF8.Storage.SetValueTuple("tuple7", (1, 2, 3, 4, 5, 6, 7));

        FF8.Storage.SetVector3("player_pos", new Vector3(3f, 4f, 5f));
        FF8.Storage.SetQuaternion("player_rot", Quaternion.Euler(15f, 30f, 45f));
        FF8.Storage.SetBounds("spawn_bounds", new Bounds(Vector3.zero, Vector3.one * 2));
        FF8.Storage.SetRect("ui_rect", new Rect(5f, 10f, 100f, 50f));
        FF8.Storage.SetLayerMask("enemy_layer", (LayerMask)(1 << 8));

        string nickname = FF8.Storage.GetString("nickname", user: true);
        SaveProfile profile = FF8.Storage.GetObject<SaveProfile>("profile");
        Vector3 playerPos = FF8.Storage.GetVector3("player_pos");
        Bounds spawnBounds = FF8.Storage.GetBounds("spawn_bounds");

        FF8.Storage.Save();
    }
}
```

Full sample source:
- `Assets/F8Framework/Tests/Storage/DemoStorage.cs`

## Common APIs
- Primitive types: `SetString/GetString`, `SetInt/GetInt`, `SetFloat/GetFloat`, `SetBool/GetBool`
- Extended numeric types: `SetChar`, `SetByte`, `SetShort`, `SetLong`, `SetDouble`, `SetDecimal`
- Enum: `SetEnum<TEnum>`, `GetEnum<TEnum>`
- Tuple: `SetValueTuple`, `GetValueTuple`
- Collections: `SetList/GetList`, `SetDictionary/GetDictionary`, `SetQueue/GetQueue`, `SetStack/GetStack`
- Unity types: `SetVector3/GetVector3`, `SetBounds/GetBounds`, `SetRect/GetRect`
- Generic APIs: `Set<T>`, `Get<T>`

## Path Formats
Supported file path formats:
- `"PlayerData.json"`
- `"Save/PlayerData.json"`
- `"C:/Users/User/Save/PlayerData.json"`

When using `Location.File`:
- `Directory.PersistentDataPath` uses `Application.persistentDataPath`
- `Directory.DataPath` uses `Application.dataPath`

## Notes
- `Resources` mode is read-only at runtime
- When encryption is enabled, storage keys are MD5-hashed
- In file mode, data is cached in memory first and written to disk on `Save()`
- `OnTermination()` automatically calls `Save()`
