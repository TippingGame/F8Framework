using System;
using F8Framework.Core;
using UnityEngine;

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
        StorageManager.Instance.SetUser("12345");
        StorageManager.Instance.SetString("Key1", "value", user: true);
        
        // 使用
        StorageManager.Instance.SetInt("Key2", 1);
        StorageManager.Instance.SetBool("Key3", true);
        StorageManager.Instance.SetFloat("Key4", 1.1f);
        
        StorageManager.Instance.SetObject("Key5", Info);
        
        //取出数据
        StructInfo info2 = StorageManager.Instance.GetObject<StructInfo>("Key5");
        LogF8.Log(info2.Initial);
        
        StorageManager.Instance.Save();
        StorageManager.Instance.Clear();
    }
}
