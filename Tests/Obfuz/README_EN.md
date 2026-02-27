# F8 Integration with Obfuz

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## Import Plugin (Obfuz must be imported first)
Note! Obfuz：https://github.com/focus-creative-games/obfuz.git  
Method 1: Directly download the file and place it into Unity  
Method 2: Unity -> Click Menu Bar -> Window -> Package Manager -> Click + -> Add Package from git URL -> Enter: https://github.com/focus-creative-games/obfuz.git

## Introduction (Simply press F8 to start game development without distractions)
Integrate Obfuz code obfuscation and protection solution.
1. Use this[ Official Tutorial (Quick Start) ](https://www.obfuz.com/docs/beginner/quick-start)to generate a key, after mounting the initialization code.  
2. Open the ObfuzSettings window
	* Add `Assembly-CSharp`、`F8Framework.Core`、`F8Framework.Launcher`、`F8Framework.F8ExcelDataClass` to the `AssemblySettings.AssembliesToObfuscate` list
    * Add `F8Framework.Core.Editor`、`F8Framework.Tests` to the `AssemblySettings.NonObfuscatedButReferenceingObfuscatedAssemblies` list
      ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Obfuz/ui_1772175480597.png)  

#### Common Errors:
* Switch Api Compatibility Level to .Net Framework
* Try setting all F8Framework assemblies to non-obfuscated
* Mounting scripts through the interface for method invocation will cause loss of references, so LogViewer cannot work properly after obfuscation