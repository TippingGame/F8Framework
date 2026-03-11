---
name: f8-features-timer-workflow
description: Use when implementing or troubleshooting Timer feature workflows — Timer, FrameTimer, server time sync, extensions, pause/resume in F8Framework.
---

# Timer Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about timers, frame-based timers, delayed actions, or server time sync.
- The user asks about pause/resume, repeat actions, or application focus handling.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Timer/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Timer
- Test docs: Assets/F8Framework/Tests/Timer

## Key classes and interfaces

| Class | Role |
|-------|------|
| `TimerManager` | Core module. Access via `FF8.Timer`. |

## API quick reference

### Standard Timer
```csharp
// Full API: owner, interval, delay, repeatCount (-1 = loop), tick, complete, ignoreTimeScale
int id = FF8.Timer.AddTimer(this, 1f, 0f, 3,
    () => { LogF8.Log("tick"); },
    () => { LogF8.Log("done"); },
    ignoreTimeScale: false);
```

### Extension methods (simplified)
```csharp
FF8.Timer.AddTimer(1f, () => { });                   // One-shot after 1s
FF8.Timer.AddTimer(1f, false, () => { });             // With ignoreTimeScale
FF8.Timer.AddTimer(1f, 1, () => { }, () => { });      // With repeat and callbacks

// MonoBehaviour extensions
this.AttachTimerF8(1f, onTick, onComplete, ignoreTimeScale);
this.DelayTimerF8(1f, () => { });            // Delay then execute
this.IntervalTimerF8(1f, () => { });         // Repeat forever
this.RepeatTimerF8(1f, 5, () => { });        // Repeat N times
this.UntilTimerF8(1f, () => true, () => { }); // Repeat until condition
```

### Frame Timer
```csharp
// Frame-based timer: every N frames
int id = FF8.Timer.AddTimerFrame(this, 1f, 0f, -1,
    () => { LogF8.Log("tick"); },
    () => { LogF8.Log("done"); },
    ignoreTimeScale: false);

FF8.Timer.AddTimerFrame(1f, () => { });
FF8.Timer.AddTimerFrame(1f, false, () => { });
FF8.Timer.AddTimerFrame(1f, 1, () => { }, () => { });
```

### Timer control
```csharp
FF8.Timer.RemoveTimer(id);     // Stop specific timer
FF8.Timer.Pause();              // Pause all (or by id)
FF8.Timer.Resume();             // Resume all (or by id)
FF8.Timer.Restart();            // Restart all (or by id)
```

### Application focus
```csharp
FF8.Timer.AddListenerApplicationFocus(); // Auto pause/resume on focus change
```

### Server time sync
```csharp
FF8.Timer.SetServerTime(1702573904000);  // Sync with server (ms)
long serverTime = FF8.Timer.GetServerTime();
float gameTime = FF8.Timer.GetTime();    // Total game time
```

## Workflow

1. Choose timer type: time-based (`AddTimer`) or frame-based (`AddTimerFrame`).
2. Use extension methods for common patterns (delay, interval, repeat, until).
3. Pass `this` as owner for auto-cleanup when MonoBehaviour is destroyed.
4. Use `ignoreTimeScale: true` for UI timers that should work during pause.
5. Set up `AddListenerApplicationFocus()` for mobile pause handling.
6. For online games, sync with `SetServerTime()`.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Timer continues after object destroyed | Missing owner reference | Pass `this` as owner |
| Timer not ticking during pause | `ignoreTimeScale` is false | Set `ignoreTimeScale: true` |
| Server time drift | Network latency | Re-sync periodically |

## Cross-module dependencies

- **Event**: Uses ApplicationFocus event for auto-pause.

## Output checklist

- Timer type and pattern selected.
- Owner set for auto-cleanup.
- TimeScale handling configured.
- Validation status and remaining risks.
