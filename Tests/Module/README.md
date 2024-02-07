# F8Module

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8Module模块中心组件，通过继承模块，模块中心可以获取所有模块的实例，自由控制生命周期

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 创建模板

1. 右键资源文件夹，看到（F8模块中心功能），创建模板  

### 代码使用方法
```C#
        /*----------------------------模块中心功能----------------------------*/
        
        // 获取所有模块，并调用进入游戏
        foreach (var center in ModuleCenter.GetSubCenter())
        {
            center.Value.OnEnterGame();
        }
        
        // 获取指定模块
        ModuleCenter demoCenter = ModuleCenter.GetCenterByType(typeof(DemoCenter));
        
        // 使用模块
        DemoCenter.Instance.OnEnterGame();
        
        
        /*----------------------------如何继承模块中心----------------------------*/
        
public class DemoCenter : ModuleCenter
{
	public static DemoCenter Instance => GetInstance<DemoCenter>();
	
	protected override void Init()
	{
		// 初始化Center
	}
		
	public override void OnEnterGame()
	{
		// 进入游戏
	}

	public override void OnQuitGame()
	{
		// 退出游戏
	}
}
```


