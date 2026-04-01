# F8 Integration with Obfuz

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## Import Plugin (Obfuz must be imported first)
Note! Obfuz：https://github.com/focus-creative-games/obfuz.git  
Method 1: Directly download the file and place it into Unity  
Method 2: Unity -> Click Menu Bar -> Window -> Package Manager -> Click + -> Add Package from git URL -> Enter: https://github.com/focus-creative-games/obfuz.git

## Introduction (Simply press F8 to start game development without distractions)
### Integrate Obfuz code obfuscation and protection solution.
1. Use this[ Official Tutorial (Quick Start) ](https://www.obfuz.com/docs/beginner/quick-start)to generate a key, after mounting the initialization code.  
2. Open the ObfuzSettings window
	* Add `Assembly-CSharp`、`F8Framework.Core`、`F8Framework.Launcher`、`F8Framework.F8ExcelDataClass` to the `AssemblySettings.AssembliesToObfuscate` list
    * Add `F8Framework.Core.Editor`、`F8Framework.Tests` to the `AssemblySettings.NonObfuscatedButReferenceingObfuscatedAssemblies` list
      ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Obfuz/ui_1772175480597.png)  

### Working with HybridCLR:
1. Use this[ official tutorial (Working with HybridCLR) ](https://www.obfuz.com/docs/manual/hybridclr/work-with-hybridclr)to resolve dnlib plugin conflicts after installing the obfuz4hybridclr extension package.
2. Locate the `GenerateCopyHotUpdateDll` method in the code[ F8Helper.cs ](https://github.com/TippingGame/F8Framework/blob/main/Editor/F8Helper/F8Helper.cs)
    * Replace the command with Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll
    ```C#
    // Only execute commands using HybridCLR (choose one)
    // HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
    // Execute commands using both HybridCLR and Obfuz (choose one)
    Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll();
    ```
    * Uncomment the following
    ```C#
   // Uncomment to use both HybridCLR and Obfuz
   if (Obfuz.Settings.ObfuzSettings.Instance.assemblySettings.GetAssembliesToObfuscate().Contains(dll))
   {
       path = Obfuz4HybridCLR.PrebuildCommandExt.GetObfuscatedHotUpdateAssemblyOutputPath(
           EditorUserBuildSettings.activeBuildTarget) + "/" + dll + ".dll";
   }
    ```
    * Move the [EncryptionService initialization code](https://www.obfuz.com/en/docs/beginner/quick-start#add-code) to the hot update entry point LoadDll.cs for use
    * If everything is fine, you can run F8

### Common Errors:
* Switch Api Compatibility Level to .Net Framework
* Try setting all F8Framework assemblies to non-obfuscated
* To read Excel in real-time, you need to use FF8.Config.RuntimeLoadAll instead
    ```C#
  // Read Excel in real-time
  FF8.Config.RuntimeLoadAll();
    ```
* GeneratedEncryptionVirtualMachine.cs requires initialization. A path error will throw an error, which can be set in the ObfuzSettings window.
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Obfuz/ui_1772552238826.png)  