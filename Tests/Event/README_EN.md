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

private void Start()
{
    // Global listener (supports both int and enum parameters)
    FF8.Message.AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
    FF8.Message.AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);
    FF8.Message.AddEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnPlayerSpawnedNoGC, this);
    FF8.Message.AddEventListener<int, string, bool, float, long, byte, char>(10004, OnPlayerSpawnedT7, this);
    
    // Dispatch global message (with/without parameters)
    FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus);
    FF8.Message.DispatchEvent(10002, 123123, "asdasd");
    FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus, 123123, "asdasd");
    FF8.Message.DispatchEvent(10004, 123123, "asdasd", true, 1.5f, 999L, (byte)7, 'F');

    // Async frame-sliced dispatch (executes 1 listener per frame)
    FF8.Message.DispatchEventAsync(MessageEvent.ApplicationFocus);
    FF8.Message.DispatchEventAsync(10002, 123123, "asdasd");
    FF8.Message.DispatchEventAsync(MessageEvent.ApplicationFocus, 123123, "asdasd");
    FF8.Message.DispatchEventAsync(10004, 123123, "asdasd", true, 1.5f, 999L, (byte)7, 'F');
    
    // Remove listeners
    FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
    FF8.Message.RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);
    FF8.Message.RemoveEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnPlayerSpawnedNoGC, this);
    FF8.Message.RemoveEventListener<int, string, bool, float, long, byte, char>(10004, OnPlayerSpawnedT7, this);
}

private void OnPlayerSpawned()
{
    LogF8.Log("OnPlayerSpawned");
}

private void OnPlayerSpawnedNoGC(int id, string name)
{
    LogF8.Log("OnPlayerSpawnedNoGC");
    LogF8.Log(id);
    LogF8.Log(name);
}

private void OnPlayerSpawnedT7(int id, string name, bool active, float speed, long score, byte level, char rank)
{
}

/*--------------------------EventDispatcher Usage--------------------------*/
// When used on entities/UI, simplifies code with auto-cleanup
AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC);

// Dispatch global message (with/without parameters)
DispatchEvent(MessageEvent.ApplicationFocus);
DispatchEvent(10002, 123123, "asdasd");

// Async frame-sliced dispatch (executes 1 listener per frame)
DispatchEventAsync(MessageEvent.ApplicationFocus);
DispatchEventAsync(10002, 123123, "asdasd");

// Optional: Clear() will remove all listeners from this script
RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned);
RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC);
```

### Zero-GC parameter events
The event API now uses strongly typed overloads only. Use the generic overload matching the fixed parameter count directly.

```C#
FF8.Message.AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);
FF8.Message.DispatchEvent(10002, 123123, "asdasd");
FF8.Message.DispatchEventAsync(10002, 123123, "asdasd");
FF8.Message.RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);

AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC);
DispatchEvent(10002, 123123, "asdasd");
DispatchEventAsync(10002, 123123, "asdasd");
RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC);

void OnPlayerSpawnedNoGC(int id, string name)
{
}
```

Currently supports 0-7 fixed parameters.

## EventDispatcher Usage Guide[（Refer to BaseView.cs）](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/Base/BaseView.cs)
For Demo: Simply drag and attach [(DemoEventDispatcher.cs)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/DemoEventDispatcher.cs) to a GameObject  

### Editor extension function
#### Event System Monitor
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Event/ui_1759047457821.png)
