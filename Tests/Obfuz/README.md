# F8 接入Obfuz

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 导入插件（需要首先导入Obfuz）
注意！Obfuz：https://github.com/focus-creative-games/obfuz.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/focus-creative-games/obfuz.git

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
接入Obfuz代码混淆和加固解决方案。
1. 使用这个[ 官方教程（快速上手） ](https://www.obfuz.com/docs/beginner/quick-start)生成密钥，挂载初始化代码后。  
2. 打开ObfuzSettings设置窗口
	* 将 `Assembly-CSharp`、`F8Framework.Core`、`F8Framework.Launcher`、`F8Framework.F8ExcelDataClass`加入到`AssemblySettings.AssembliesToObfuscate`列表
    * 将 `F8Framework.Core.Editor`、`F8Framework.Tests`加入到`AssemblySettings.NonObfuscatedButReferenceingObfuscatedAssemblies`列表
      ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Obfuz/ui_1772175480597.png)  

#### 常见报错：
* Api Compatability Level 切换为 .Net Framework
* 尝试将 F8Framework 所有程序集设为不混淆
* 通过界面挂载脚本方法调用的方式，会导致丢失引用，所以 LogViewer 在混淆后，无法正常工作