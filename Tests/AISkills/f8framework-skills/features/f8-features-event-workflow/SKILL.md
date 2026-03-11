---
name: f8-features-event-workflow
description: Use when implementing or troubleshooting Event feature workflows — message dispatching, event listening, EventDispatcher auto-cleanup in F8Framework.
---

# Event Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


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
| `EventManager` | Core module. Access via `FF8.Message`. Global event bus. |
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
FF8.Message.AddEventListener(10001, OnEventWithArgs, this);

// Dispatch event (without/with parameters)
FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus);
FF8.Message.DispatchEvent(10001, new object[] { 123, "data" });

// Remove listener
FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnEvent, this);
FF8.Message.RemoveEventListener(10001, OnEventWithArgs, this);

// Callback signatures
void OnEvent() { }
void OnEventWithArgs(params object[] args) { }
```

### EventDispatcher pattern (auto-cleanup)
```csharp
// In classes inheriting EventDispatcher (e.g., BaseView):
AddEventListener(MessageEvent.ApplicationFocus, OnEvent);
DispatchEvent(MessageEvent.ApplicationFocus);
RemoveEventListener(MessageEvent.ApplicationFocus, OnEvent);
// All listeners auto-cleaned on Clear()
```

## Workflow

1. Define event IDs as enum (start from 10000 to avoid framework conflicts).
2. Choose pattern: global `FF8.Message` or `EventDispatcher` mixin.
3. For UI/entity classes, prefer `EventDispatcher` for automatic cleanup.
4. Always pass `this` as the last parameter to `AddEventListener` for lifecycle binding.
5. The framework has built-in dead-loop prevention.
6. Use the Event System Monitor editor window to debug active listeners.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Event not received | Listener added after dispatch | Ensure listener registration before dispatch |
| Dead loop warning | Event A dispatches Event B which dispatches Event A | Break the cycle, use intermediate state |
| Memory leak | Listeners not removed on destroy | Use EventDispatcher or pass `this` for auto-cleanup |
| Wrong callback signature | Params mismatch | Use `void()` for no-args or `void(params object[])` for args |

## Cross-module dependencies

- **UI**: BaseView extends EventDispatcher for auto-cleanup.
- **Timer**: Application focus events used by Timer module.

## Output checklist

- Event ID enum defined with proper range.
- Listener pattern selected (global / EventDispatcher).
- Files changed and why.
- Validation status and remaining risks.
