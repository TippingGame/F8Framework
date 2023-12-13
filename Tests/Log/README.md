# F8Log

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介
Unity F8Log组件，打印日志，写入文件，上报错误

## 导入插件（需要首先导入核心）
F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Log.git  

### 代码使用方法
```C#
        LogF8.GetCrashErrorMessage();//开启错误上报，需要配置url
        LogF8.Log(LogF8.UseMemory);//打印当前使用堆内存
        LogF8.Watch();//开始耗时计时
        LogF8.Log(111);//常规打印
        LogF8.LogColor(Color.green, "打印颜色");
        LogF8.LogError("打印错误");
        LogF8.Log(LogF8.UseTime);//打印耗时
        
        LogF8.LogNet("222{0}", "测试");//打印网络日志，带时间和标识
        LogF8.LogEvent(333);//打印事件日志，带时间和标识
        LogF8.LogConfig(this);//打印配置日志，带时间和标识
        LogF8.LogView(this);//打印视图日志，带时间和标识
        LogF8.LogEntity(this);//打印视图日志，带时间和标识
        
        F8LogHelper.Instance.OnEnterGame();//开启日志写入
        F8LogHelper.Instance.OnQuitGame();//关闭日志写入
```

## F8DebugConsole使用方法（游戏界面显示log）
直接拖拽F8DebugConsole.cs，挂载到GameObject  
