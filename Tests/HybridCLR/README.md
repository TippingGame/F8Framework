# F8 接入HybridCLR

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
接入HybridCLR热更新代码组件。
1. 使用这个[官方教程](https://hybridclr.doc.code-philosophy.com/docs/beginner/quickstart)创建HotUpdate程序集后。  
2. 找到代码[F8Helper.cs](https://github.com/TippingGame/F8Framework/blob/main/Editor/F8Helper/F8Helper.cs)  
	1.解除注释状态
	* [MenuItem("开发工具/生成并复制热更新Dll-F8")]
	  public static void GenerateCopyHotUpdateDll()  
	  {  
	  // HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();  
	  // FileTools.SafeClearDir(Application.dataPath + "/AssetBundles/Code");  
	  // FileTools.CheckDirAndCreateWhenNeeded(Application.dataPath + "/AssetBundles/Code");  
	  // List<string> hotUpdateDll = new List<string>()  
	  // {  
	  //     "HotUpdate", // 自行添加需要热更的程序集  
	  // };  
	  // foreach (var dll in hotUpdateDll)  
	  // {  
	  //     FileTools.SafeCopyFile(  
	  //         HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget) + "/" + dll + ".dll",  
	  //         Application.dataPath + "/AssetBundles/Code/" + dll + ".bytes");  
	  //     LogF8.LogAsset("生成并复制热更新dll：" + dll);  
	  // }  
	  // AssetDatabase.Refresh();  
	  }  

## 导入插件（需要首先导入HybridCLR）
注意！HybridCLR：https://github.com/focus-creative-games/hybridclr_unity.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/focus-creative-games/hybridclr_unity.git  
