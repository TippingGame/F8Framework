using System;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoStorage : MonoBehaviour
    {
        public class ClassInfo
        {
            public string Initial = "initial";
        }

        public ClassInfo Info = new ClassInfo();

        void Start()
        {
            // 先配置存储模式，再读写数据
            FF8.Storage.Configure(new StorageManager.Settings
            {
                location = StorageManager.Location.File,
                directory = StorageManager.Directory.PersistentDataPath,
                defaultFilePath = "Save/PlayerData.json",
                compressionType = StorageManager.CompressionType.Gzip,
                encryption = new Util.OptimizedAES(key: "AES_Key", iv: null)
            });
            
            // 设置UserId，用户私有的Key
            FF8.Storage.SetUser("12345");
            
            // 使用基础类型
            FF8.Storage.SetString("Key1", "value", user: true);
            FF8.Storage.GetString("Key1", "", user: true);
            
            FF8.Storage.SetInt("Key2", 1);
            FF8.Storage.GetInt("Key2");
            
            FF8.Storage.SetBool("Key3", true);
            FF8.Storage.GetBool("Key3");
            
            FF8.Storage.SetFloat("Key4", 1.1f);
            FF8.Storage.GetFloat("Key4");

            // 使用数据类
            FF8.Storage.SetObject("Key5", Info);
            ClassInfo info2 = FF8.Storage.GetObject<ClassInfo>("Key5");
            LogF8.Log(info2.Initial);
            
            // 使用泛型接口与集合
            FF8.Storage.Set("Key6", new[] { 1, 2, 3, 4 });
            int[] arrayValue = FF8.Storage.Get<int[]>("Key6");
            LogF8.Log($"Array Length: {arrayValue.Length}");
            
            FF8.Storage.SetList("Key7", new List<string> { "A", "B", "C" });
            List<string> listValue = FF8.Storage.GetList<string>("Key7");
            LogF8.Log($"List Count: {listValue.Count}");
            
            FF8.Storage.SetDictionary("Key8", new Dictionary<int, string>
            {
                { 1, "One" },
                { 2, "Two" }
            });
            Dictionary<int, string> dictValue = FF8.Storage.GetDictionary<int, string>("Key8");
            LogF8.Log(dictValue[1]);
            
            FF8.Storage.SetQueue("Key9", new Queue<int>(new[] { 10, 20, 30 }));
            Queue<int> queueValue = FF8.Storage.GetQueue<int>("Key9");
            LogF8.Log($"Queue Peek: {queueValue.Peek()}");
            
            FF8.Storage.SetHashSet("Key10", new HashSet<int> { 100, 200, 300 });
            HashSet<int> hashSetValue = FF8.Storage.GetHashSet<int>("Key10");
            LogF8.Log($"HashSet Count: {hashSetValue.Count}");
            
            FF8.Storage.SetStack("Key11", new Stack<int>(new[] { 7, 8, 9 }));
            Stack<int> stackValue = FF8.Storage.GetStack<int>("Key11");
            LogF8.Log($"Stack Peek: {stackValue.Peek()}");
            
            // 使用多维数组和交错数组
            FF8.Storage.SetRectangularArray("Key12", new int[,]
            {
                { 1, 2 },
                { 3, 4 }
            });
            int[,] grid = FF8.Storage.GetRectangularArray<int>("Key12");
            LogF8.Log($"Grid[1,1]: {grid[1, 1]}");
            
            FF8.Storage.SetJaggedArray("Key13", new int[][]
            {
                new[] { 1, 2 },
                new[] { 3, 4, 5 }
            });
            int[][] jagged = FF8.Storage.GetJaggedArray<int>("Key13");
            LogF8.Log($"Jagged[1][2]: {jagged[1][2]}");

            // 保存到硬盘
            FF8.Storage.Save();
            
            // 也可以临时指定其他文件路径
            FF8.Storage.Save("Save/BackupPlayerData.json");
            FF8.Storage.Remove("Key2", filePath: "Save/BackupPlayerData.json");
            FF8.Storage.Clear("Save/TempPlayerData.json");
        }
    }
}
