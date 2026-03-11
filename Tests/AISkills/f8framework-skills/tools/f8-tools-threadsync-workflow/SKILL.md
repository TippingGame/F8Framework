---
name: f8-tools-threadsync-workflow
description: Use when working with ThreadSync tools — Unity main thread synchronization and thread-safe operations in F8Framework.
---

# ThreadSync Tools Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves thread synchronization or main thread dispatch.
- The user needs to safely access Unity APIs from worker threads.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/UnityThread/

## Sources of truth

- Source directory: Assets/F8Framework/Runtime/Utility/UnityThread

## Key capabilities

- Main thread action queue
- Thread-safe Unity API access
- Synchronization context management
- Background-to-main thread dispatch

## Workflow

1. Use UnityThread utilities to queue actions for main thread execution.
2. Background thread work that needs Unity API access should dispatch to main thread.
3. Combine with Network module for thread-safe message handling.
4. WebGL: no multi-threading available — this module not needed.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Crash from background thread | Direct Unity API access | Dispatch to main thread first |
| Action not executing | UnityThread component not in scene | Ensure initialization |
| WebGL thread crash | Multi-threading not supported | Avoid threads on WebGL |

## Output checklist

- Thread model determined.
- Main thread dispatch properly configured.
- Unity API access only on main thread.
