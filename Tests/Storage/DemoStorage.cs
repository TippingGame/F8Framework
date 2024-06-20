using System;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoStorage : MonoBehaviour
    {
        [Serializable]
        public struct StructInfo
        {
            public string Initial;

            // 构造函数来初始化结构体的字段
            public StructInfo(string initial)
            {
                Initial = initial;
            }
        }

        public StructInfo Info = new StructInfo("initial");

        void Start()
        {
            // 设置UserId，用户私有的Key
            FF8.Storage.SetUser("12345");
            FF8.Storage.SetString("Key1", "value", user: true);

            // 使用
            FF8.Storage.SetInt("Key2", 1);
            FF8.Storage.SetBool("Key3", true);
            FF8.Storage.SetFloat("Key4", 1.1f);

            FF8.Storage.SetObject("Key5", Info);

            // 取出数据
            StructInfo info2 = FF8.Storage.GetObject<StructInfo>("Key5");
            LogF8.Log(info2.Initial);

            FF8.Storage.Save();
            FF8.Storage.Clear();
        }
    }
}
