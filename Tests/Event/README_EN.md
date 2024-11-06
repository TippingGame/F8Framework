# F8 Event

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Event组件，优雅的发送消息、事件监听系统，防止消息死循环，EventDispatcher可自动释放Event

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 代码使用方法
```C#
        // 消息的定义，枚举
        public enum MessageEvent
        {
            // 框架事件，10000起步
            Empty = 10000,
            ApplicationFocus = 10001, // 游戏对焦
            NotApplicationFocus = 10002, // 游戏失焦
            ApplicationQuit = 10003, // 游戏退出
        }
        
        private object[] data = new object[] { 123123, "asdasd" };
        
        // 全局监听
        FF8.Message.AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
        FF8.Message.AddEventListener(10001, OnPlayerSpawned, this);
        
        // 发送全局消息（不带参数/带参数）
        FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus, data);
        FF8.Message.DispatchEvent(10001, data);
        
        // 移除监听
        FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
        FF8.Message.RemoveEventListener(10001, OnPlayerSpawned, this);
        
        
        // EventDispatcher用法，用作在实体或UI上，简化代码，监听自动释放
        AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
        
        // 发送全局消息（不带参数/带参数）
        DispatchEvent(MessageEvent.ApplicationFocus);
        
        // 可不执行，Clear()时会清理此脚本所有监听
        RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
```

## EventDispatcher使用方法[（参考）](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/Base/BaseView.cs)
Demo直接拖拽DemoEventDispatcher.cs，挂载到GameObject  
