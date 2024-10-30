# F8 ExcelTool

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

> F8 框架初衷：希望自己点击 F8，就能开始制作游戏，不想多余的事。
>
> F8 Framework original intention: Just click F8 and start making the game, don't want to be redundant.

## 简介

Unity 读取 Excel 的工具

- **加载缓存**：高性能，加载手动生成的 Excel 二进制缓存

- **加载文件**：高适应性，运行时自动读取最新 Excel，无需人工干预

  - **必须**先完成本指南的[初始化](#初始化)部分（因：对于结构、类型等变动，运行时无法刷新 C# 代码）。

系统支持：Win/Android/iOS/Mac/Linux/WebGL

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

## 初始化

1. 在 Assets 下，创建 StreamingAssets/config 目录，按照下面 "Excel 示例" 创建你的 Excel[（Excel例子）](https://github.com/TippingGame/F8Framework/blob/main/Runtime/ExcelTool/StreamingAssets_config/DemoWorkSheet.xlsx)


2. 点击菜单的**开发工具**项 -> **导入配置表**\_F8（快捷键），在 Assets/AssetBundles/Config/BinConfigData 下生成 **bytes** 文件（WebGL下生成 **Json** 文件）


3. 如无意外，根目录下会生成 **Assets/F8Framework/ConfigData/** 目录和相关文件


4. （可选项）更改Excel存放目录，**开发工具**项 -> **设置Excel存放目录**


5. （可选项）通过 Editor，可在运行时读取 Excel 数据：点击菜单的**开发工具**项 -> **运行时读取 Excel**\_F7（快捷键）


## Excel 示例

值类型 Excel 示例：
（id 是唯一索引）

| int | long       | float    | double      | str             |
| --- | ---------- | -------- | ----------- | --------------- |
| id  | name1      | name2    | name3       | name4           |
| 1   | 9935434343 | 2.725412 | 1.346655321 | 读取 Excel 工具 |
| 2   | 9935434343 | 2.725412 | 1.346655321 | 读取 Excel 工具 |

支持的引用类型 Excel 示例：
（更多引用类型譬如 `List/Dict` 等，不确定是否支持）

| int\[] | float\[]   | str\[]      | obj\[]             | long\[]\[]      | double\[]\[]                  | str\[]\[]                       |
| ------ | ---------- |-------------|--------------------|-----------------|-------------------------------| ------------------------------- |
| name1  | name2      | name3       | name4              | name5           | name6                         | name7                           |
| \[1,5] | \[1.5,5.8] | \[test,str] | \["test",65,1.239] | {\[1,6],\[2,8]} | {\[6.215,6.12],\[2.5,14.556]} | {\[自动,格式],\[tipping,excel]} |
| \[1,5] | \[1.5,5.8] | \[test,str] | \["test",65,1.235] | {\[1,6],\[2,8]} | {\[6.215,6.12],\[2.5,14.556]} | {\[自动,格式],\[tipping,excel]} |

## 使用范例

**在使用 Excel 数据前，需要执行**：

加载二进制或者json配置方式：

```C#
FF8.Config.LoadAll(); // 同步加载全部配置

foreach(var item in FF8.Config.LoadAllAsync()) // 异步加载全部配置
{
    yield return item;
}
```

加载Excel的方式：

```C#
ReadExcel.Instance.LoadAllExcelData(); // 运行时加载 Excel 最新文件
```

**打印数据**：

值类型，譬如 `int/float/string`，请参考[C# 类型系统 - Microsoft Document](https://learn.microsoft.com/zh-cn/dotnet/csharp/fundamentals/types/#value-types)：

```C#
        // 注意：GetSheet1ByID 方法为自动生成的。
        // 注意：Sheet1 需替换为实际 Sheet 名
        // 注意：name 需替换为实际表头
        // 注意：2 代表您设置的 ID 2 的行
        // 单个表单个数据
        LogF8.Log(FF8.Config.GetSheet1ByID(2).name);
        
        // 单个表全部数据
        foreach (var item in FF8.Config.GetSheet1())
        {
            LogF8.Log(item.Key);
            LogF8.Log(item.Value.name);
        }
        
        // 指定名字加载
        Sheet1 sheet1 = new Sheet1();
        sheet1 = FF8.Config.Load<Sheet1>("Sheet1");
        LogF8.Log(sheet1.Dict[2].name);
```

引用类型：

```C#
        foreach (var VARIABLE in FF8.Config.GetSheet1ByID(2).price)
        {
            foreach (var item in VARIABLE)
            {
                LogF8.Log(item);
            }
        }
```

## 使用到的库

Excel.dll（已修改缓存地址为Application.persistentDataPath）  
I18N.CJK.dll\
I18N.dll\
I18N.MidEast.dll\
I18N.Other.dll\
I18N.Rare.dll\
I18N.West.dll\
[ICSharpCode.SharpZipLib.dll](https://github.com/icsharpcode/SharpZipLib)  
[LitJson.dll](https://github.com/LitJSON/litjson)（已修改字典Key可以使用int类型，增加UnityEngine基础类型）

## 注意

### Android

加载文件模式在热加载插件(譬如 [HybridCLR](https://github.com/focus-creative-games/hybridclr))时，可能需要额外操作：

**由于 Android 资源都在包内，在 Android 上使用，需要先复制到可读写文件夹中再进行读取**

```C#
    // 方式二：运行时读取Excel
    IEnumerator Start()
    {
        // 由于安卓资源都在包内，需要先复制到可读写文件夹1
        string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
        string[] paths = null;
        WWW www = new WWW(assetPath + "/fileindex.txt");
        yield return www;
        if (www.error != null)
        {
            LogF8.Log(www.error);
            yield return null;
        }
        else
        {
            string ss = www.text;
            // 去除夹杂的空行
            string[] lines = ss.Split('\n');
            List<string> nonEmptyLines = new List<string>();
    
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
    
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    nonEmptyLines.Add(trimmedLine);
                }
            }
    
            paths = nonEmptyLines.ToArray();
        }
    
        for (int i = 0; i < paths.Length; i++)
        {
            yield return CopyAssets(paths[i].Replace("\r", ""));
        }
        // 读取Excel文件
        ReadExcel.Instance.LoadAllExcelData();
        LogF8.Log(FF8.Config.GetSheet1ByID(1).name);
        LogF8.Log(FF8.Config.GetSheet1());
    }
    
    // 由于安卓资源都在包内，需要先复制到可读写文件夹2
    IEnumerator CopyAssets(string paths)
    {
        string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
        string sdCardPath = Application.persistentDataPath + "/config";
        WWW www = new WWW(assetPath + "/" + paths);
        yield return www;
        if(www.error != null)
        {
            LogF8.Log(www.error);
            yield return null;
        }
        else
        {
            FileTools.SafeWriteAllBytes(sdCardPath + "/" + paths, www.bytes);
        }
    }
```
