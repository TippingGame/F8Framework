# F8Event

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8Event组件，优雅的发送消息、事件监听系统，防止消息死循环

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
        //消息的定义，枚举
    public enum MessageEvent
    {
        //框架事件，预留10000为起始
        Empty = 10000,
        ApplicationFocus,
        NotApplicationFocus,
        ApplicationQuit,
    }
        private object[] data = new object[] { 123123, "asdasd" };
        //全局监听
        MessageManager.Instance.AddEventListener(MessageEvent.ApplicationFocus,OnPlayerSpawned,this);
        //发送全局消息（不带参数/带参数）
        MessageManager.Instance.DispatchEvent(MessageEvent.ApplicationFocus,data);
        //移除监听
        MessageManager.Instance.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned,this);
        
        
        //EventDispatcher用法，用作在实体或UI上，简化代码，监听自动释放
        AddEventListener(MessageEvent.ApplicationFocus,OnPlayerSpawned);
        DispatchEvent(MessageEvent.ApplicationFocus);
        //可不执行，Clear()时会清理此脚本所有监听
        RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
```

## EventDispatcher使用方法
直接拖拽DemoEventDispatcher.cs，挂载到GameObject  
