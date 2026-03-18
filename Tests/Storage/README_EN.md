# F8 StorageManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction
Unity F8 StorageManager for local data storage, loading, field-level AES encryption, and whole-file Gzip compression.

## Plugin Installation
Built into F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import them into Unity  
Method 2: Unity -> Window -> Package Manager -> "+" -> Add Package from git URL -> Enter: https://github.com/TippingGame/F8Framework.git

## Features
- Supports `PlayerPrefs`, `File`, and `Resources`
- Supports `Location`, `Directory`, `Default File Path`, and `Compression`
- Supports field-level AES encryption
- Supports generic `Get<T>/Set<T>`
- Supports `Array`, `List`, `Dictionary`, `Queue`, `HashSet`, `Stack`
- Supports `Rectangular Array` and `Jagged Array`
- `Remove`, `Save`, and `Clear` support temporary custom file paths

## Code Example
```csharp
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

public class DemoStorage : MonoBehaviour
{
    public class ClassInfo
    {
        public string Initial = "initial";
    }

    public ClassInfo Info = new ClassInfo();

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

        FF8.Storage.SetUser("12345");

        FF8.Storage.SetString("Key1", "value", user: true);
        string stringValue = FF8.Storage.GetString("Key1", "", user: true);

        FF8.Storage.SetInt("Key2", 1);
        int intValue = FF8.Storage.GetInt("Key2");

        FF8.Storage.SetBool("Key3", true);
        bool boolValue = FF8.Storage.GetBool("Key3");

        FF8.Storage.SetFloat("Key4", 1.1f);
        float floatValue = FF8.Storage.GetFloat("Key4");

        FF8.Storage.SetObject("Key5", Info);
        ClassInfo info2 = FF8.Storage.GetObject<ClassInfo>("Key5");

        FF8.Storage.Set("Key6", new[] { 1, 2, 3, 4 });
        int[] arrayValue = FF8.Storage.Get<int[]>("Key6");

        FF8.Storage.SetList("Key7", new List<string> { "A", "B", "C" });
        List<string> listValue = FF8.Storage.GetList<string>("Key7");

        FF8.Storage.SetDictionary("Key8", new Dictionary<int, string>
        {
            { 1, "One" },
            { 2, "Two" }
        });
        Dictionary<int, string> dictValue = FF8.Storage.GetDictionary<int, string>("Key8");

        FF8.Storage.SetQueue("Key9", new Queue<int>(new[] { 10, 20, 30 }));
        Queue<int> queueValue = FF8.Storage.GetQueue<int>("Key9");

        FF8.Storage.SetHashSet("Key10", new HashSet<int> { 100, 200, 300 });
        HashSet<int> hashSetValue = FF8.Storage.GetHashSet<int>("Key10");

        FF8.Storage.SetStack("Key11", new Stack<int>(new[] { 7, 8, 9 }));
        Stack<int> stackValue = FF8.Storage.GetStack<int>("Key11");

        FF8.Storage.SetRectangularArray("Key12", new int[,]
        {
            { 1, 2 },
            { 3, 4 }
        });
        int[,] grid = FF8.Storage.GetRectangularArray<int>("Key12");

        FF8.Storage.SetJaggedArray("Key13", new int[][]
        {
            new[] { 1, 2 },
            new[] { 3, 4, 5 }
        });
        int[][] jagged = FF8.Storage.GetJaggedArray<int>("Key13");

        FF8.Storage.Save();
        FF8.Storage.Save("Save/BackupPlayerData.json");
        FF8.Storage.Remove("Key2", filePath: "Save/BackupPlayerData.json");
        FF8.Storage.Clear("Save/TempPlayerData.json");
    }
}
```

## Path Formats
File paths support:
- `"PlayerData.json"`
- `"Save/PlayerData.json"`
- `"C:/Users/User/Save/PlayerData.json"`
