# F8StorageManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8StorageManager组件，本地数据存储/读取  

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
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
```


