# F8 接入Obfuz

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 导入插件（需要首先导入Obfuz）
注意！Obfuz：https://github.com/focus-creative-games/obfuz.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/focus-creative-games/obfuz.git

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
### 接入Obfuz代码混淆和加固解决方案。
1. 使用这个[ 官方教程（快速上手） ](https://www.obfuz.com/docs/beginner/quick-start)生成密钥，挂载初始化代码后。  
2. 打开ObfuzSettings设置窗口
	* 将 `Assembly-CSharp`、`F8Framework.Core`、`F8Framework.Launcher`、`F8Framework.F8ExcelDataClass`加入到`AssemblySettings.AssembliesToObfuscate`列表
    * 将 `F8Framework.Core.Editor`、`F8Framework.Tests`加入到`AssemblySettings.NonObfuscatedButReferenceingObfuscatedAssemblies`列表
      ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Obfuz/ui_1772175480597.png)  

### 与HybridCLR协同工作：
1. 使用这个[ 官方教程（与HybridCLR协同工作） ](https://www.obfuz.com/docs/manual/hybridclr/work-with-hybridclr)解决dnlib插件冲突，安装obfuz4hybridclr扩展包后。
2. 找到代码[ F8Helper.cs ](https://github.com/TippingGame/F8Framework/blob/main/Editor/F8Helper/F8Helper.cs) 的`GenerateCopyHotUpdateDll`方法
   * 替换命令为 Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll
    ```C#
    // 只使用HybridCLR执行的命令（二选一）
    // HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
    // 使用HybridCLR的同时也使用Obfuz执行的命令（二选一）
    Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll();
    ```
    * 解除下面注释
    ```C#
   // 使用HybridCLR的同时也使用Obfuz解除注释
   if (Obfuz.Settings.ObfuzSettings.Instance.assemblySettings.GetAssembliesToObfuscate().Contains(dll))
   {
       path = Obfuz4HybridCLR.PrebuildCommandExt.GetObfuscatedHotUpdateAssemblyOutputPath(
           EditorUserBuildSettings.activeBuildTarget) + "/" + dll + ".dll";
   }
    ```
   * 将[初始化EncryptionService代码](https://www.obfuz.com/docs/beginner/quick-start#%E6%B7%BB%E5%8A%A0%E4%BB%A3%E7%A0%81)移动到热更新入口 LoadDll.cs 中使用
   * 没问题的话，就可以运行 F8

### 常见报错：
* Api Compatability Level 切换为 .Net Framework
* 尝试将 F8Framework 所有程序集设为不混淆
* 实时读取 Excel 需要改用 FF8.Config.RuntimeLoadAll
    ```C#
  // 实时读取Excel
  FF8.Config.RuntimeLoadAll();
    ```
* 生成的 GeneratedEncryptionVirtualMachine.cs 初始化时需要用到，路径错误时会报错，ObfuzSettings 窗口中可设置
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Obfuz/ui_1772552238826.png)  