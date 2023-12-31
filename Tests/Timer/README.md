# F8Timer

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity Timer组件，提供普通Timer、FrameTimer，暂停恢复（时间正常流逝）

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
         //普通Timer,传入自身this，每1秒执行一次，延迟0秒后开始，执行3次(-1表示循环)
        timeid = TimerManager.Instance.Register(this,1f,0,3, () =>
        {
            LogF8.Log("tick");
        }, () =>
        {
            LogF8.Log("完成");
        });
        
        //FrameTimer,传入自身this，每1帧执行一次，延迟0帧后开始，循环执行(-1表示循环)
        timeid = TimerManager.Instance.RegisterFrame(this,1f,0,-1, () =>
        {
            LogF8.Log("tick");
        }, () =>
        {
            LogF8.Log("完成");
        });
        
        TimerManager.Instance.UnRegister(timeid);//停止名为timeid的Timer
        
        //可以使用OnApplicationFocus监听游戏焦点，暂停所有Timer
        TimerManager.Instance.Pause();
        TimerManager.Instance.Restart();
        
        TimerManager.Instance.SetServerTime(1702573904000);//网络游戏，与服务器对表
        TimerManager.Instance.GetServerTime();
        
        TimerManager.Instance.GetTime(); //获取游戏中的总时长
```

## 自动OnApplicationFocus监听窗口焦点，暂停所有Timer
