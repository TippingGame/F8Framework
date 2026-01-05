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
    // Normal Timer: pass 'this' as context, executes every 1 second, 
    // starts after 0 seconds delay, repeats 3 times (-1 means infinite loop)
    int timeid = FF8.Timer.AddTimer(this, 1f, 0f, 3,
        () => { LogF8.Log("tick"); },
        () => { LogF8.Log("completed"); },
        ignoreTimeScale: false);

    // Extension methods
    FF8.Timer.AddTimer(1f, () => { });
    FF8.Timer.AddTimer(1f, false, () => { });
    FF8.Timer.AddTimer(1f, 1, () => { }, () => { });

    // Frame Timer: pass 'this' as context, executes every 1 frame,
    // starts after 0 frames delay, loops infinitely (-1 means infinite loop)
    timeid = FF8.Timer.AddTimerFrame(this, 1f, 0f, -1,
        () => { LogF8.Log("tick"); },
        () => { LogF8.Log("completed"); },
        ignoreTimeScale: false);
    
    // Extension methods
    FF8.Timer.AddTimerFrame(1f, () => { });
    FF8.Timer.AddTimerFrame(1f, false, () => { });
    FF8.Timer.AddTimerFrame(1f, 1, () => { }, () => { });

    // Stop the timer with the specified timeid
    FF8.Timer.RemoveTimer(timeid);

    // Listen for application focus events to automatically pause/resume all timers
    FF8.Timer.AddListenerApplicationFocus();

    // Manually pause or resume all timers (or specific timer by id)
    FF8.Timer.Pause();
    FF8.Timer.Resume();
    // Restart all timers (or specific timer by id)
    FF8.Timer.Restart();

    // For online games: sync with server time (unit: milliseconds)
    FF8.Timer.SetServerTime(1702573904000);
    FF8.Timer.GetServerTime();

    // Get total elapsed time in the game
    FF8.Timer.GetTime();
}
```

## Automatic Window Focus Handling with OnApplicationFocus, Pause all timers
