using System;
using F8Framework.Core;
using F8Framework.Launcher;
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
            // 设置密钥，自动加密所有数据（编辑器下不加密）
            FF8.Storage.SetEncrypt(new Util.OptimizedAES("AES_Key", "AES_IV"));
            
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

            FF8.Storage.Save();
            FF8.Storage.Clear();
        }
    }
}
