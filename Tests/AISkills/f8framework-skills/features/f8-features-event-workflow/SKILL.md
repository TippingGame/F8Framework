---
name: f8-features-event-workflow
description: Use when implementing or troubleshooting Event feature workflows — message dispatching, event listening, EventDispatcher auto-cleanup, and strongly typed zero-GC event parameters (0-7 args, no object[] compatibility path) in F8Framework.
---

# Event Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`.


## Use this skill when

- The task is about event/message dispatching and listening.
- The user asks about decoupled communication between components.
- The user needs EventDispatcher auto-cleanup on UI or entities.
- Troubleshooting event dead loops or missing callbacks.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. For usage docs, read: Assets/F8Framework/Tests/Event/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Event
- Editor module: Assets/F8Framework/Editor/Event
- Test docs: Assets/F8Framework/Tests/Event

## Key classes and interfaces

| Class | Role |
|-------|------|
| `MessageManager` | Core module. Access via `FF8.Message`. Global event bus. |
| `EventDispatcher` | Mixin base class for auto-cleanup event listeners. Used by BaseView. |

## API quick reference

### Define events
```csharp
public enum MessageEvent
{
    Empty = 10000,
    ApplicationFocus = 10001,
    NotApplicationFocus = 10002,
    ApplicationQuit = 10003,
}
```

### Global event listening (via FF8.Message)
```csharp
// Add listener (supports int and enum, pass 'this' for auto-cleanup)
FF8.Message.AddEventListener(MessageEvent.ApplicationFocus, OnEvent, this);
FF8.Message.AddEventListener<int, string>(10002, OnEventNoGC, this);
FF8.Message.AddEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnEventNoGC, this);
FF8.Message.AddEventListener<int, string, bool, float, long, byte, char>(10004, OnEventT7, this);

// Dispatch event (without/with parameters)
FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus);
FF8.Message.DispatchEvent(10002, 123, "data");
FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus, 123, "data");
FF8.Message.DispatchEvent(10004, 123, "data", true, 1.5f, 999L, (byte)7, 'F');

// Async frame-sliced dispatch (executes 1 listener per frame)
FF8.Message.DispatchEventAsync(MessageEvent.ApplicationFocus);
FF8.Message.DispatchEventAsync(10002, 123, "data");
FF8.Message.DispatchEventAsync(MessageEvent.ApplicationFocus, 123, "data");
FF8.Message.DispatchEventAsync(10004, 123, "data", true, 1.5f, 999L, (byte)7, 'F');

// Remove listener
FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnEvent, this);
FF8.Message.RemoveEventListener<int, string>(10002, OnEventNoGC, this);
FF8.Message.RemoveEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnEventNoGC, this);
FF8.Message.RemoveEventListener<int, string, bool, float, long, byte, char>(10004, OnEventT7, this);

// Callback signatures
void OnEvent() { }
void OnEventNoGC(int id, string name) { }
void OnEventT7(int id, string name, bool active, float speed, long score, byte level, char rank) { }
```

### EventDispatcher pattern (auto-cleanup)
```csharp
// In classes inheriting EventDispatcher (e.g., BaseView):
AddEventListener(MessageEvent.ApplicationFocus, OnEvent);
AddEventListener<int, string>(10002, OnEventNoGC);
AddEventListener<int, string, bool, float, long, byte, char>(10004, OnEventT7);
DispatchEvent(MessageEvent.ApplicationFocus);
DispatchEvent(10002, 123, "data");
DispatchEvent(10004, 123, "data", true, 1.5f, 999L, (byte)7, 'F');
DispatchEventAsync(MessageEvent.ApplicationFocus);
DispatchEventAsync(10002, 123, "data");
DispatchEventAsync(10004, 123, "data", true, 1.5f, 999L, (byte)7, 'F');
RemoveEventListener(MessageEvent.ApplicationFocus, OnEvent);
RemoveEventListener<int, string>(10002, OnEventNoGC);
RemoveEventListener<int, string, bool, float, long, byte, char>(10004, OnEventT7);
// All listeners auto-cleaned on Clear()
```

### Zero-GC recommendation
```csharp
// Use the fixed-parameter overload matching the event payload size.
FF8.Message.AddEventListener<int>(10010, OnHpChanged, this);
FF8.Message.DispatchEvent(10010, 99);
FF8.Message.DispatchEventAsync(10010, 99);

FF8.Message.AddEventListener<int, int>(10011, OnDamage, this);
FF8.Message.DispatchEvent(10011, 12, 3);
FF8.Message.DispatchEventAsync(10011, 12, 3);

FF8.Message.AddEventListener<int, int, int, int, int, int, int>(10012, OnCombo, this);
FF8.Message.DispatchEvent(10012, 1, 2, 3, 4, 5, 6, 7);
FF8.Message.DispatchEventAsync(10012, 1, 2, 3, 4, 5, 6, 7);

void OnHpChanged(int hp) { }
void OnDamage(int damage, int criticalType) { }
void OnCombo(int a, int b, int c, int d, int e, int f, int g) { }
```

## Workflow

1. Define event IDs as enum (start from 10000 to avoid framework conflicts).
2. Choose pattern: global `FF8.Message` or `EventDispatcher` mixin.
3. For UI/entity classes, prefer `EventDispatcher` for automatic cleanup.
4. Use the strongly typed overload matching the event payload size; the event module now supports 0~7 fixed parameters.
5. If you need to spread callback cost across frames, use `DispatchEventAsync(...)`; the current implementation executes 1 listener per frame.
6. Always pass `this` as the last parameter to `AddEventListener` for lifecycle binding.
7. The framework has built-in dead-loop prevention.
8. Use the Event System Monitor editor window to debug active listeners.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Event not received | Listener added after dispatch | Ensure listener registration before dispatch |
| Dead loop warning | Event A dispatches Event B which dispatches Event A | Break the cycle, use intermediate state |
| Memory leak | Listeners not removed on destroy | Use EventDispatcher or pass `this` for auto-cleanup |
| Wrong callback signature | Params mismatch | Match the overload exactly: `void()`, `void(T1)`, `void(T1,T2)` ... `void(T1,T2,T3,T4,T5,T6,T7)` |

## Cross-module dependencies

- **UI**: BaseView extends EventDispatcher for auto-cleanup.
- **Timer**: Application focus events used by Timer module.

## Output checklist

- Event ID enum defined with proper range.
- Listener pattern selected (global / EventDispatcher).
- Files changed and why.
- Validation status and remaining risks.
