# F8 ExcelTool

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)

Unity Excel Tool

- **Load Cached Data**: High performance, loads manually generated Excel binary cache.

- **Load Files**: High adaptability, automatically reads the latest Excel files at runtime without manual intervention.

    - **Prerequisite**: Complete the [Initialization](#Initialization) section of this guide first (reason: runtime cannot refresh C# code for structural or type changes).

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

## Initialization

1. Under `Assets`, create the `StreamingAssets/config` directory. Follow the Excel Example below to create your Excel file [(Excel Example)](https://github.com/TippingGame/F8Framework/blob/main/Runtime/ExcelTool/StreamingAssets_config/DemoWorkSheet.xlsx) (Excel will be auto-generated after the first F8 execution).


2. Click the **Development Tools** menu → **Import Config Tables**_F8 (shortcut). This generates **.bytes** files under `Assets/AssetBundles/Config/BinConfigData` (**binary** or **json** files can be selected).


3. **Note**: If you don’t want to generate files in the `AssetBundles` directory, You can set the export table directory in the F5 packaging tool.


4. If successful, the `Assets/F8Framework/ConfigData/` directory and related files will be generated in the root directory. (**Note**: F8 execution will clear the framework's default files and regenerate them. Any errors will stem from conflicts in this code.)
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/ExcelTool/ui_20241112212632.png)


5. **(Optional)** Change the Excel storage directory: Development Tools → Set Excel Storage Directory.


6. **(Optional)** Use the Editor to read Excel data at runtime: Development Tools → Read Excel at Runtime_F7 (shortcut).


## Excel Example

#### Types are divided into: 
1. Basic Types
2. Container Types
3. Special Types

* 1.[C# Basic Types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types)（char，bool，byte，short，int，long，float，double，decimal，str / string，obj / object，[datetime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime.-ctor?view=net-9.0)，sbyte，ushort，uint，ulong）  
  UnityBasic Types（[vec2 / vector2](https://docs.unity3d.com/ScriptReference/Vector2-ctor.html)，[vec3 / vector3](https://docs.unity3d.com/ScriptReference/Vector3-ctor.html)，[vec4 / vector4](https://docs.unity3d.com/ScriptReference/Vector4-ctor.html)，[vec2int / vector2int](https://docs.unity3d.com/ScriptReference/Vector2Int-ctor.html)，[vec3int / vector3int](https://docs.unity3d.com/ScriptReference/Vector3Int.html)，[quat / quaternion](https://docs.unity3d.com/ScriptReference/Quaternion-ctor.html)，[color](https://docs.unity3d.com/ScriptReference/Color.html)，[color32](https://docs.unity3d.com/ScriptReference/Color32.html)）  

Excel Example (**id** is a unique index, value type, must be added!):  

| int | long       | bool  | float    | double      | str        | vector3           | color              | datetime                          |
| --- | ---------- |-------|----------|-------------|------------|-------------------|--------------------|-----------------------------------|
| id  | name1      | name2 | name3    | name4       | name5      | name6             | name7              | name8                             |
| 1   | 9935434343 | true  | 2.725412 | 1.346655321 | Excel Tool | 1.23,1.35,1.45    | 122,135,145,255    | 1750316265001                     |
| 2   | 9935434343 | 1     | 2.725412 | 1.346655321 | Excel Tool | \[1.23,1.35,1.45] | \[122,135,145,255] | 2025-06-19T14:30:00.1234567+08:00 |

* 2.Container Types
  * Supported:
    * Arrays, Jagged Arrays ([[]](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays), [[][]](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays#jagged-arrays), [[][][]](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays#jagged-arrays))
    * List ([list<>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.-ctor?view=net-9.0))
    * Dictionary ([dict<,> / dictionary<,>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.-ctor?view=net-9.0), Note: Export 'json' format table key can only be a base type and enumerations. If you need to support containers, you can use 'binary'`)
    * ValueTuple ([valuetuple<,>](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple?view=net-9.0), Supports up to 7 types).
    * Containers can accept any type (excluding variant types).

Excel Example:  

| int\[] | string\[]   | vec2\[]            | obj\[]\[]              | list\<obj\>       | dict\<int,list\<string\>\> | valuetuple\<int,string\> |
| ------ |-------------|--------------------|------------------------|-------------------|----------------------------|--------------------------|
| name1  | name2       | name3              | name4                  | name5             | name6                      | name7                    |
| \[1,5] | \[test,str] | \[[12,66],[12,66]] | \[\[22,"str"],\[33,"obj"]] | 123,1.888,"列表"    | 1,\[di,ct],2,\["di,ct"]        | 1,valuetuple                    |
| \[1,5] | \[test,str] | \[[12,66],[12,66]] | \[\[22,"str"],\[33,"obj"]] | \[123,1.888,"列表"] | [1,\["di","ct"],2,\["di,ct"]]  | [1,"value,tuple"]                |

* 3.Special Type Support
  * Enum ([enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum?view=net-9.0)<name,int,Flags>{})
    * `name` is the enumeration name, `int` is the enumeration type, `Flags` is the enumeration attribute
    * By default, generates the enumeration class in the current table
    * `enum<Sheet1.name>` can access enumerations across tables, where `Sheet1` is the table name and `name` is the enumeration name
  * Variant (variant<name,variantName>)
    * `name` is the name of other variables within the current table, `variantName` is the variant name
    * Enables one-click switching of configuration table variants, thereby supporting multi-language/multi-version configurations

Excel Example:  
(Optional parameters: int type (default), Flags attribute, can cross-sheet access: Sheet1.name)

| enum<name,int,Flags>{Value1 = 1,Value2 = 2,Value3 = 4,Value4 = 8,} | enum<Sheet1.name> | enum<Status,long>{OK = 200,Success = 200,Created = 201,Accepted = 202,} |
|--------------------------------------------------------------------|-------------------|-------------------------------------------------------------------------|
| name1                                                              | name2             | name3                                                                   |
| Value1                                                             | Value1            | 200                                                                     |
| Value2                                                             | Value2            | Success                                                                 |
| Value1, Value2                                                     | Value3            | 201                                                                     |
| Value4                                                             | Value4            | 202                                                                     |

```C#
// Set variant name
FF8.Config.VariantName = "English";
```
| string | variant<desc,English> | variant<desc,Korean> | int    | variant<attack,English> | variant<attack,Korean> |
|--------|-----------------------|----------------------|--------|-------------------------|------------------------|
| desc    | desc                  | desc                 | attack |                         |                        |
| 中文1    | Chinese 1             | 중국어 1                | 1000   | 800                     | 300                    |
| 中文2    | Chinese 2             | 중국어 2                | 1000    | 800                     | 300                    |
| 中文3    | Chinese 3             | 중국어 3                | 1000    | 800                     | 300                    |
| 中文4    | Chinese 4             | 중국어 4                | 1000    | 800                     | 300                    |

(You can extend other types: [ReadExcel.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/ExcelTool/ReadExcel.cs))  

* Additional Notes:
  * To skip an entire row: Leave `id` blank
  * To skip an entire column: Leave `type` or `name` blank

## Usage Examples

**Before using Excel data, execute the following:**：

Load Binary or JSON Configurations:

```C#
// Load by sheet name  
Sheet1 sheet1 = FF8.Config.Load<Sheet1>("Sheet1");  

// Synchronously load all configurations  
FF8.Config.LoadAll();  

// Asynchronously load all configurations  
yield return FF8.Config.LoadAllAsyncIEnumerator();  
// Or:  
foreach(var item in FF8.Config.LoadAllAsync())  
{  
    yield return item;  
}  
// async/await method (no multithreading, WebGL can also be used)
await FF8.Config.LoadAllAsyncTask();
```

Read Excel at Runtime (Use with Caution):

```C#
ReadExcel.Instance.LoadAllExcelData(); // Load the latest Excel files at runtime  
```

**Print Data:**：

For basic types like `int, float, string`, refer to [C# Type System - Microsoft Document](https://learn.microsoft.com/zh-cn/dotnet/csharp/fundamentals/types/#value-types)：

```C#
// Note: GetSheet1ByID is auto-generated.  
// Replace Sheet1 with the actual sheet name.  
// Replace name with the actual header.  
// Replace 2 with the row ID you set.  

// Single cell data  
LogF8.Log(FF8.Config.GetSheet1ByID(2).name);  

// Entire sheet data  
foreach (var item in FF8.Config.GetSheet1())  
{  
    LogF8.Log(item.Key);  
    LogF8.Log(item.Value.name);  
}  
```

## Libraries Used

Excel.dll (cache path modified to **Application.persistentDataPath**, Add the method of using byte [] to read Excel)  
I18N.CJK.dll\
I18N.dll\
I18N.MidEast.dll\
I18N.Other.dll\
I18N.Rare.dll\
I18N.West.dll\
[ICSharpCode.SharpZipLib.dll](https://github.com/icsharpcode/SharpZipLib)  
[LitJson.dll](https://github.com/LitJSON/litjson) (The dictionary key has been modified to support all basic and enumeration types; added support for commonly used Unity types: Type, Vector2, Vector3, Vector4, Quaternion, GameObject, Transform, Color, Color32, Bounds, Rect, RectOffset, LayerMask, Vector2Int, Vector3Int, RangeInt, BoundsInt; fixed the DateTime precision loss issue; Fix the issue of long error)

## Writing to Excel (Optional)
Use [EPPlus.dll (built-in) ](https://github.com/TippingGame/F8Framework/blob/main/Plugins/EPPlus.dll)(disabled by default; manually select the compilation platform):
```C#
public static void WriteExcel(string str, int row, int col, string value)  
{  
    string filePath = Application.streamingAssetsPath + "/" + str + ".xlsx";  
    FileInfo excelName = new FileInfo(filePath);  

    using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(excelName))  
    {  
        // Get the first sheet  
        OfficeOpenXml.ExcelWorksheet worksheet = package.Workbook.Worksheets[1];  
        // Modify a cell  
        worksheet.Cells[row, col].Value = value;  
        // Save  
        package.Save();  
    }  
}  
```

## Important Notes

**On Android, resources are inside the APK. To use runtime Excel reading, copy files to a writable folder first:**  
**Using the ICSharpCode.SharpZipLib.Zip library ZipFile class within the framework can directly read StreamingAssets files. Please refer to the details for more information:[SyncStreamingAssetsLoader.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Utility/StreamingAssetsHelper/SyncStreamingAssetsLoader.cs)**

```C#
    // Sync reading files from the StreamingAssets/config folder
    string[] files = SyncStreamingAssetsLoader.Instance.ReadAllLines("config/fileindex.txt");
    
    // Release resources after use
    SyncStreamingAssetsLoader.Instance.Close();
```
