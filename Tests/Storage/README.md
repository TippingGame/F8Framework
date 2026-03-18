# F8 StorageManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 StorageManager 组件，本地数据存储、读取、字段级 AES 加密、整文件 Gzip 压缩。  

## 导入插件（需要首先导入核心）
注意！内置在 -> F8Framework 核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入 Unity  
方式二：Unity -> 点击菜单栏 -> Window -> Package Manager -> 点击 + 号 -> Add Package from git URL -> 输入：https://github.com/TippingGame/F8Framework.git  

### 视频教程：[【Unity框架】（17）本地数据存储](https://www.bilibili.com/video/BV1EQRwYEE9x)

## 特性
- 支持 `PlayerPrefs`、`File`、`Resources`
- 支持 `Location`、`Directory`、`Default File Path`、`Compression`
- 支持字段级 AES 加密
- 支持泛型 `Get<T>/Set<T>`
- 支持 `Array`、`List`、`Dictionary`、`Queue`、`HashSet`、`Stack`
- 支持 `Rectangular Array` 和 `Jagged Array`
- `Remove`、`Save`、`Clear` 支持临时指定文件路径

## 代码使用方法
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

## 路径说明
文件路径支持三种写法：
- `"PlayerData.json"`
- `"Save/PlayerData.json"`
- `"C:/Users/User/Save/PlayerData.json"`
