# F8 Timer

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Timer Component**  
Timing system providing both time-based and frame-based counters with full lifecycle management.
* Timer Types:
    * Timer: Real-time seconds counter
    * FrameTimer: Frame-based counter

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
void Start()
{
    // Standard Timer - Attached to this object
    // Triggers every 1 second, starts immediately (0 delay), executes 3 times (-1 would loop infinitely)
    int timeid = FF8.Timer.AddTimer(this, 1f, 0f, 3, () =>
    {
        LogF8.Log("tick"); // Tick callback
    }, () =>
    {
        LogF8.Log("Timer completed"); // Completion callback
    }, ignoreTimeScale: false);
    
    // FrameTimer - Attached to this object
    // Triggers every frame, starts immediately (0 frame delay), loops infinitely (-1)
    timeid = FF8.Timer.AddTimerFrame(this, 1f, 0f, -1, () =>
    {
        LogF8.Log("frame tick"); // Per-frame callback
    }, () =>
    {
        LogF8.Log("Timer completed"); // Completion callback
    }, ignoreTimeScale: false);
    
    // Stop a specific timer by its ID
    FF8.Timer.RemoveTimer(timeid);
    
    // Automatically pause/resume timers when application loses/gains focus
    FF8.Timer.AddListenerApplicationFocus();
    
    // Manual timer control
    FF8.Timer.Pause();    // Pause all timers
    FF8.Timer.Resume();   // Resume all paused timers
    FF8.Timer.Restart();  // Restart all timers
    
    // Server time synchronization (for online games)
    FF8.Timer.SetServerTime(1702573904000); // Synchronize with server time (milliseconds)
    long serverTime = FF8.Timer.GetServerTime(); // Get synchronized server time
    
    // Get total elapsed game time
    float totalGameTime = FF8.Timer.GetTime();
}
```

## Automatic Window Focus Handling with OnApplicationFocus, Pause all timers
