# F8 Event

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Event Component**  
* An elegant message dispatch and event listening system
* Dead-loop prevention - Safeguards against infinite message cycles
* Auto-cleanup - EventDispatcher automatically releases events

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
// Message definition (enum)
public enum MessageEvent
{
    // Framework events (starting from 10000)
    Empty = 10000,
    ApplicationFocus = 10001,       // Game window focused
    NotApplicationFocus = 10002,    // Game window lost focus
    ApplicationQuit = 10003,        // Game quitting
}

private object[] data = new object[] { 123123, "asdasd" };

private void Start()
{
    // Global listener (supports both int and enum parameters)
    FF8.Message.AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
    FF8.Message.AddEventListener(10001, OnPlayerSpawned2, this);
    
    // Dispatch global message (with/without parameters)
    FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus);
    FF8.Message.DispatchEvent(10001, data);
    
    // Remove listeners
    FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
    FF8.Message.RemoveEventListener(10001, OnPlayerSpawned2, this);
}

private void OnPlayerSpawned()
{
    LogF8.Log("OnPlayerSpawned");
}

private void OnPlayerSpawned2(params object[] obj)
{
    LogF8.Log("OnPlayerSpawned2");
    if (obj is { Length: > 0 })
    {
        LogF8.Log(obj[0]);
        LogF8.Log(obj[1]);
    }
}

/*--------------------------EventDispatcher Usage--------------------------*/
// When used on entities/UI, simplifies code with auto-cleanup
AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);

// Dispatch global message (with/without parameters)
DispatchEvent(MessageEvent.ApplicationFocus);

// Optional: Clear() will remove all listeners from this script
RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
```

## EventDispatcher Usage Guide[（Refer to BaseView.cs）](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/Base/BaseView.cs)
For Demo: Simply drag and attach DemoEventDispatcher.cs to a GameObject  
