# F8 Log

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Log组件，打印日志，写入文件，上报错误

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 代码使用方法
```C#
        /*----------Log基础功能----------*/
        LogF8.Log(this);
        LogF8.Log("测试{0}",1);
        LogF8.Log("3123测试", this);
        LogF8.LogNet("1123{0}", "测试");
        LogF8.LogEvent(this);
        LogF8.LogConfig(this);
        LogF8.LogView(this);
        LogF8.LogEntity(this);
        
        
        /*----------Log其他功能----------*/
        // 开启写入log文件
         FF8.LogWriter.OnEnterGame();
        // 开启捕获错误日志
        LogF8.GetCrashErrorMessage();
        // 开始监视代码使用情况
        LogF8.Watch();
        LogF8.Log(LogF8.UseMemory);
        LogF8.Log(LogF8.UseTime);
```

## 拓展功能
1. LogViewer日志显示，内置指令系统、系统状态检测（https://github.com/nhn/gpm.unity.git）
* 直接拖动到场景，或制作成资产直接加载，显示按键（键盘输入：波浪号~ / 移动端：五指长按1秒）  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Log/ui_20240302152501.png)  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Log/ui_20240302152840.png)
