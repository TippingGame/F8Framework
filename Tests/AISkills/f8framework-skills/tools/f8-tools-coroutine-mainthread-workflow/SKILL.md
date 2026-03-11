---
name: f8-tools-coroutine-mainthread-workflow
description: Use when working with Coroutine/MainThread tools — coroutine utilities and main thread dispatch in F8Framework.
---

# Coroutine/MainThread Tools Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves coroutine management or dispatching work to the main thread.
- The user needs to run code on the main thread from a background thread.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/Coroutine/

## Sources of truth

- Source directory: Assets/F8Framework/Runtime/Utility/Coroutine

## Key capabilities

- Coroutine start/stop utilities
- Main thread action dispatch from background threads
- Thread-safe Unity API access patterns
- Coroutine chaining helpers

## Workflow

1. Use coroutine utilities for managed coroutine lifecycle.
2. Dispatch to main thread when accessing Unity APIs from background threads.
3. Combine with `LogF8.LogToMainThread()` for thread-safe logging.
4. WebGL does not support multi-threading — use coroutines only.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Unity API from background thread | Thread-unsafe access | Dispatch to main thread |
| Coroutine not running | MonoBehaviour inactive | Ensure host is active |

## Output checklist

- Threading model identified (main thread only vs multi-thread).
- Main thread dispatch configured if needed.
- Coroutine host MonoBehaviour verified.
