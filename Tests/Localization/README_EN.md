# F8 Localization

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Localization Component**
Multi-language Localization System with Excel Integration
1. Multi-Component Localization Support:
   * Text
   * TextMeshPro
   * Font
   * Image/RawImage
   * SpriteRenderer
   * Material Renderer
   * Audio Clips
   * Timeline Tracks
2. Excel-Based Translation Tables:
   * Manage all translations in Excel sheets
   * Support for unlimited languages
   * Easy export/import functionality
3. High Extensibility:
   * Custom localization targets via interfaces
   * Runtime language switching
   * Dynamic content updating

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### How to Use

Tip: For lightweight usage, you can directly use the [variant type](https://github.com/TippingGame/F8Framework/blob/main/Tests/ExcelTool/README.md#excel-%E7%A4%BA%E4%BE%8B) `variant<name,variantName>` in the configuration table module.  

1. Create an Excel file named **Localization.xlsx** in the `StreamingAssets/config` directory (rename the sheet to **LocalizedStrings**) as your localization configuration  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219212643.png)
----------------------------
2. Supports real-time switching between 42 system languages in both editor and runtime (Shortcut key F6)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219212707.png)
----------------------------
3. Text/TextMeshPro Components (Displays ID reference in real-time)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213728.png)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213734.png)
----------------------------
4. Usage with Other Components (Can also use ID references)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213738_2.png)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213741_2.png)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20241109113409_2.png)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20241109113656_2.png)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213745.png)

### Code Examples
```C#
/*----------------------------Localization Features----------------------------*/

    // Change language
    FF8.Local.ChangeLanguage("English");
    
    // Get translated text
    string text = FF8.Local.GetTextFromId("test", "Support", "Format");
    string text1 = FF8.Local.GetTextFromIdLanguage("test", "English");
    
    // Available languages list
    FF8.Local.LanguageList;
    
    // Current language
    FF8.Local.CurrentLanguageName;
    
    // Reload translation table
    FF8.Local.Load();
    
    // Refresh all localized components
    FF8.Local.InjectAll();
```

