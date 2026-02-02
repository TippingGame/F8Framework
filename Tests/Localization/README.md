# F8 Localization

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
**Unity F8 Localization 本地化组件**
1. 多组件本地化支持：
    * Text
    * TextMeshPro
    * Font
    * Image / RawImage
    * SpriteRenderer
    * Material Renderer
    * Audio Clips
    * Timeline Tracks
2. 使用Excel作为多语言翻译表：
    * 支持42种语言
    * 轻松导出/导入功能
3. 高扩展性：
    * 通过界面自定义本地化目标
    * 运行时语言切换
    * 动态内容更新

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 视频教程：[【Unity框架】（11）本地化](https://www.bilibili.com/video/BV1LVDzYNE3E)

### 如何使用

提示：如轻量使用，可直接使用配置表模块的[变体类型](https://github.com/TippingGame/F8Framework/blob/main/Tests/ExcelTool/README.md#excel-%E7%A4%BA%E4%BE%8B)`variant<name,variantName>`  

1. 在 StreamingAssets/config 目录创建一个名为：**Localization.xlsx** 的 Excel（Sheet改名为 **LocalizedStrings** ） 作为本地化配置  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219212643.png)  
----------------------------
2. 编辑器和运行时，支持42种系统语言实时切换。（快捷键F6）  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219212707.png)  
----------------------------
3. Text / TextTextMeshPro （实时显示ID索引）  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213728.png)  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213734.png)  
----------------------------
4. 其他组件使用（也可以使用ID索引）  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213738_2.png)  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213741_2.png)  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20241109113409_2.png)  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20241109113656_2.png)  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Localization/ui_20240219213745.png)  

### 代码使用方法
```C#
/*----------------------------本地化功能----------------------------*/

    // 切换语言
    FF8.Local.ChangeLanguage("English");
    
    // 获取翻译文本
    string text = FF8.Local.GetTextFromId("test", "Support", "Format");
    string text1 = FF8.Local.GetTextFromIdLanguage("test", "English");
    
    // 语言列表
    FF8.Local.LanguageList;
    
    // 当前语言
    FF8.Local.CurrentLanguageName;
    
    // 重新加载翻译表
    FF8.Local.Load();
    
    // 刷新所有本地化组件
    FF8.Local.InjectAll();
```

