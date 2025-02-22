# F8 StorageManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 StorageManager组件，本地数据存储/读取

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 代码使用方法
```C#
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
```


