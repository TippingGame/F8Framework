# F8 StorageManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介
`StorageManager` 用于本地数据保存与读取，支持 `PlayerPrefs`、文件、`Resources` 三种后端，并支持整文件压缩、字段级 AES 加密、泛型读写和常用 Unity 类型存储。

## 导入插件
内置于 F8Framework 核心：  
https://github.com/TippingGame/F8Framework.git

导入方式：
- 直接下载项目并导入 Unity
- `Unity -> Window -> Package Manager -> + -> Add Package from git URL`

## 特性
- 支持 `PlayerPrefs`、`File`、`Resources`
- 支持 `Location`、`Directory`、`DefaultFilePath`、`Compression`
- 支持 `None`、`Gzip`
- 支持字段级 AES 加密
- 支持基础类型、`Enum`、`ValueTuple`
- 支持 `Array`、`List`、`Dictionary`、`Queue`、`HashSet`、`Stack`
- 支持二维数组、交错数组
- 支持常用 Unity 类型：
  - `Vector2`、`Vector3`、`Vector4`
  - `Vector2Int`、`Vector3Int`
  - `Quaternion`
  - `Color`、`Color32`
  - `Matrix4x4`
  - `Bounds`、`Rect`、`RectOffset`
  - `LayerMask`
- `Remove`、`Save`、`Clear` 支持临时指定文件路径

## 快速开始
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

## 代码示例
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

完整示例代码见：
- `Assets/F8Framework/Tests/Storage/DemoStorage.cs`

## 常用接口
- 基础类型：`SetString/GetString`、`SetInt/GetInt`、`SetFloat/GetFloat`、`SetBool/GetBool`
- 扩展数值类型：`SetChar`、`SetByte`、`SetShort`、`SetLong`、`SetDouble`、`SetDecimal`
- 枚举：`SetEnum<TEnum>`、`GetEnum<TEnum>`
- 元组：`SetValueTuple`、`GetValueTuple`
- 集合：`SetList/GetList`、`SetDictionary/GetDictionary`、`SetQueue/GetQueue`、`SetStack/GetStack`
- Unity 类型：`SetVector3/GetVector3`、`SetBounds/GetBounds`、`SetRect/GetRect`
- 通用泛型：`Set<T>`、`Get<T>`

## 路径说明
文件路径支持三种写法：
- `"PlayerData.json"`
- `"Save/PlayerData.json"`
- `"C:/Users/User/Save/PlayerData.json"`

`Location.File` 时：
- `Directory.PersistentDataPath` 使用 `Application.persistentDataPath`
- `Directory.DataPath` 使用 `Application.dataPath`

## 注意事项
- `Resources` 模式运行时只读，不能写入和保存
- 开启加密后，存储 key 会使用 MD5 处理
- 文件模式下数据会先写入内存缓存，调用 `Save()` 才会真正落盘
- `OnTermination()` 会自动调用一次 `Save()`
