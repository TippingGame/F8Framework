# F8 ExcelTool

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
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

### 视频教程：[【Unity框架】（1）配置表使用](https://www.bilibili.com/video/BV1Jw4m1X7kZ)

## 初始化

1. 在 `Assets` 下，创建 `StreamingAssets/config` 目录，按照下面 "Excel 示例" 创建你的 Excel[（Excel例子）](https://github.com/TippingGame/F8Framework/blob/main/Runtime/ExcelTool/StreamingAssets_config/DemoWorkSheet.xlsx)（首次F8后自动创建Excel）


2. 点击菜单的**开发工具**项 -> **导入配置表**\_F8（快捷键），在 `Assets/AssetBundles/Config/BinConfigData` 下生成 **json** 文件（也可选择 **binary** 文件）  


3. **注意**：如果你不想生成在`AssetBundles`目录下，在代码 [ExcelDataTool.cs](https://github.com/TippingGame/F8Framework/blob/main/Editor/ExcelTool/ExcelDataTool.cs) 中修改 "BinDataFolder" 的值
    ```C#
    // 序列化的数据文件都会放在此文件夹内,此文件夹位于AssetBundles或Resources文件夹下用于读取数据
    public const string BinDataFolder = "/AssetBundles/Config/BinConfigData";
    ```


4. 如无意外，根目录下会生成 `Assets/F8Framework/ConfigData/` 目录和相关文件，（注意：F8后会清除框架自带的，并重新生成，一切报错均来自这些代码的冲突）  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/ExcelTool/ui_20241112212632.png)

5. （可选项）更改Excel存放目录，**开发工具**项 -> **设置Excel存放目录**  


6. （可选项）通过 Editor，可在运行时读取 Excel 数据：点击菜单的**开发工具**项 -> **运行时读取 Excel**\_F7（快捷键）


## Excel 示例

#### 类型可分为 1. 基础类型 2. 容器类型 3. 特殊类型
* 1.[C# 基础类型支持](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types)（bool，byte，short，int，long，float，double，decimal，str / string，obj / object，[datetime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime.-ctor?view=net-9.0)，sbyte，ushort，uint，ulong）  
Unity基础类型支持（[vec2 / vector2](https://docs.unity3d.com/ScriptReference/Vector2-ctor.html)，[vec3 / vector3](https://docs.unity3d.com/ScriptReference/Vector3-ctor.html)，[vec4 / vector4](https://docs.unity3d.com/ScriptReference/Vector4-ctor.html)，[vec2int / vector2int](https://docs.unity3d.com/ScriptReference/Vector2Int-ctor.html)，[vec3int / vector3int](https://docs.unity3d.com/ScriptReference/Vector3Int.html)，[quat / quaternion](https://docs.unity3d.com/ScriptReference/Quaternion-ctor.html)，[color](https://docs.unity3d.com/ScriptReference/Color.html)）  

Excel 示例：（id 是唯一索引，必须添加！）  

| int | long       | bool  | float    | double      | str         | vector3           | color              | datetime                          |
| --- | ---------- |-------|----------|-------------|-------------|-------------------|--------------------|-----------------------------------|
| id  | name1      | name2 | name3    | name4       | name5       | name6             | name7              | name8                             |
| 1   | 9935434343 | true  | 2.725412 | 1.346655321 | 读取 Excel 工具 | 1.23,1.35,1.45    | 122,135,145,255    | 1750316265001                     |
| 2   | 9935434343 | 1     | 2.725412 | 1.346655321 | 读取 Excel 工具 | \[1.23,1.35,1.45] | \[122,135,145,255] | 2025-06-19T14:30:00.1234567+08:00 |

* 2.容器类型支持
  * 数组，交错数组（[[]](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays) / [[][]](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays#jagged-arrays) / [[][][]](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays#jagged-arrays)）
  * 列表（[list<>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.-ctor?view=net-9.0)）
  * 字典（[dict<,> / dictionary<,>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.-ctor?view=net-9.0)，注意：key只能为byte，short，int，long，float，double，str / string 类型）
  * 值元组（[valuetuple<,>](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple?view=net-9.0)，最高支持7个类型）  
  * 容器内可以填写任意的类型  

Excel 示例：  

| int\[] | string\[]   | vec2\[]            | obj\[]\[]              | list\<obj\>       | dict\<int,string\> | dict\<int,list\<float\>\> |
| ------ |-------------|--------------------|------------------------|-------------------|--------------------|---------------------------|
| name1  | name2       | name3              | name4                  | name5             | name6              | name7                     |
| \[1,5] | \[test,str] | \[[12,66],[12,66]] | \[\[22,"str"],\[33,"obj"]] | 123,1.888,"列表"    | 1,"字典",2,"字典2"     | 1,\[1.11,2.22],2,\[3.33]  |
| \[1,5] | \[test,str] | \[[12,66],[12,66]] | \[\[22,"str"],\[33,"obj"]] | \[123,1.888,"列表"] | \[1,"字典",2,"字典2"]  | 1,\[1.11,2.22],2,\[3.33]  |

* 3.特殊类型支持
  * 枚举（[enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum?view=net-9.0)<name,int,Flags>{}）
    * 默认在当前表生成枚举类
    * 可跨表访问枚举，支持自定义名称，类型，Flags特性  

Excel 示例：  
（可选参数：int类型(默认)，Flags特性，标志枚举：Value1, Value2，跨表访问：Sheet1.name）  

| enum<name,int,Flags>{Value1 = 1,Value2 = 2,Value3 = 4,Value4 = 8,} | enum<Sheet1.name> | enum<Status,long>{OK = 200,Success = 200,Created = 201,Accepted = 202,} |
|--------------------------------------------------------------------|-------------------|-------------------------------------------------------------------------|
| name1                                                              | name2             | name3                                                                   |
| Value1                                                             | Value1            | 200                                                                     |
| Value2                                                             | Value2            | Success                                                                 |
| Value1, Value2                                                     | Value3            | 201                                                                     |
| Value4                                                             | Value4            | 202                                                                     |

（你还可以拓展其他类型：[ReadExcel.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/ExcelTool/ReadExcel.cs)）
## 使用范例

**在使用 Excel 数据前，需要执行**：

加载二进制或者json配置方式：

```C#
// 指定Sheet名字加载
Sheet1 sheet1 = FF8.Config.Load<Sheet1>("Sheet1");

// 同步加载全部配置
FF8.Config.LoadAll();

// 异步加载全部配置
yield return FF8.Config.LoadAllAsyncIEnumerator();
// 也可以这样
foreach(var item in FF8.Config.LoadAllAsync())
{
    yield return item;
}
// async/await方式（无多线程，WebGL也可使用）
await FF8.Config.LoadAllAsyncTask();
```

运行时读取Excel的方式（如没有需求请谨慎使用）：

```C#
ReadExcel.Instance.LoadAllExcelData(); // 运行时加载 Excel 最新文件
```

**打印数据**：

基础类型，譬如 `int/float/string`，请参考[C# 类型系统 - Microsoft Document](https://learn.microsoft.com/zh-cn/dotnet/csharp/fundamentals/types/#value-types)：

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
```

## 使用到的库

Excel.dll（已修改缓存地址为Application.persistentDataPath，新增使用byte[]读取Excel方法）  
I18N.CJK.dll\
I18N.dll\
I18N.MidEast.dll\
I18N.Other.dll\
I18N.Rare.dll\
I18N.West.dll\
[ICSharpCode.SharpZipLib.dll](https://github.com/icsharpcode/SharpZipLib)  
[LitJson.dll](https://github.com/LitJSON/litjson)（已修改字典Key支持byte，short，int，long，float，double，string 类型，增加Unity常用类型：Type，Vector2，Vector3，Vector4，Quaternion，GameObject，Transform，Color，Color32，Bounds，Rect，RectOffset，LayerMask，Vector2Int，Vector3Int，RangeInt，BoundsInt，修复DateTime精度丢失的问题）

## 你可能需要写入Excel
使用 [EPPlus.dll（已内置）](https://github.com/TippingGame/F8Framework/blob/main/Plugins/EPPlus.dll)但未启用，请手动选择编译的平台  
```C#
    public static void WriteExcel(string str, int row, int col, string value)
    {
        string filePath = Application.streamingAssetsPath + "/"+ str + ".xlsx";

        FileInfo excelName = new FileInfo(filePath);

        using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(excelName))
        {
            // 获取第1个sheet
            OfficeOpenXml.ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            // 修改某一行，列的数据
            worksheet.Cells[row, col].Value = value;
            // 保存excel
            package.Save();
        }
    }
```
    
## 注意

**由于 Android 资源都在包内，在 Android 上使用实时读取Excel功能，需要先复制到可读写文件夹中再进行读取**  
**框架内使用ICSharpCode.SharpZipLib.Zip库ZipFile类直接读取，可直接读取StreamingAssets文件，具体请看：[SyncStreamingAssetsLoader.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Utility/StreamingAssetsHelper/SyncStreamingAssetsLoader.cs)**

```C#
    // 同步读取config文件夹下的文件
    string[] files = SyncStreamingAssetsLoader.Instance.ReadAllLines("config/fileindex.txt");
    
    // 使用后释放资源
    SyncStreamingAssetsLoader.Instance.Close();
```
