# F8 StorageManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 StorageManager Component**  
Local Data Storage & Loading

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
public class ClassInfo
{
    public string Initial = "initial";
}

public ClassInfo Info = new ClassInfo();

void Start()
{
    // Set the key to automatically encrypt all data (not encrypted in the editor)
    FF8.Storage.SetEncrypt(new Util.OptimizedAES("AES_Key", "AES_IV"));
    
    // Set UserId, user's private key
    FF8.Storage.SetUser("12345");
    
    // Use basic types
    FF8.Storage.SetString("Key1", "value", user: true);
    FF8.Storage.GetString("Key1", "", user: true);
    
    FF8.Storage.SetInt("Key2", 1);
    FF8.Storage.GetInt("Key2");
    
    FF8.Storage.SetBool("Key3", true);
    FF8.Storage.GetBool("Key3");
    
    FF8.Storage.SetFloat("Key4", 1.1f);
    FF8.Storage.GetFloat("Key4");

    // Using data classes
    FF8.Storage.SetObject("Key5", Info);
    ClassInfo info2 = FF8.Storage.GetObject<ClassInfo>("Key5");
    LogF8.Log(info2.Initial);

    FF8.Storage.Save();
    FF8.Storage.Clear();
}
```


