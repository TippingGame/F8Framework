---
name: f8-features-log-workflow
description: Use when implementing or troubleshooting Log feature workflows — logging, file writing, error reporting, LogViewer, and debug commands in F8Framework.
---

# Log Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.LogWriter = ModuleCenter.CreateModule<F8LogWriter>();`.


## Use this skill when

- The task is about logging, debug output, file-based log writing, or error reporting.
- The user asks about LogViewer, cheat codes, or debug commands.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Log/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Log
- Test docs: Assets/F8Framework/Tests/Log

## Key classes and interfaces

| Class | Role |
|-------|------|
| `LogF8` | Static logging class. Multiple log channels and utility methods. |
| `LogWriter` | Module for file-based log writing. Access via `FF8.LogWriter`. |
| `Function` | LogViewer command system. |

## API quick reference

### Basic logging
```csharp
LogF8.Log(message);                    // General log
LogF8.Log("Format {0}", 1);           // Formatted log
LogF8.Log("Message", this);            // With object context (clickable in Console)
LogF8.LogNet("Network {0}", "data");   // Network channel
LogF8.LogEvent(data);                  // Event channel
LogF8.LogConfig(data);                 // Config channel
LogF8.LogView(data);                   // View channel
LogF8.LogEntity(data);                // Entity channel
LogF8.LogStackTrace("Stack trace");    // With stack trace
```

### Thread-safe logging
```csharp
LogF8.LogToMainThread("From background thread");
LogF8.LogErrorToMainThread("Error from background thread");
```

### Log control
```csharp
LogF8.EnabledLog();    // Enable logging
LogF8.DisableLog();    // Disable logging
```

### File writing
```csharp
FF8.LogWriter.OnEnterGame(); // Start writing logs to file
```

### Error capture
```csharp
LogF8.GetCrashErrorMessage(); // Start capturing error logs
```

### Performance monitoring
```csharp
LogF8.Watch();                // Start monitoring
LogF8.Log(LogF8.UseMemory);   // Memory usage
LogF8.Log(LogF8.UseTime);     // Time elapsed
```

### LogViewer commands
```csharp
// Add debug command to LogViewer
Function.Instance.AddCommand(this, "MethodName", new object[] { param });

// Add cheat code callback
Function.Instance.AddCheatKeyCallback((cheatKey) =>
{
    LogF8.Log("Cheat: " + cheatKey);
});
```

### LogViewer activation
- Keyboard: press tilde key `~`
- Mobile: five-finger long press for 1 second

## Workflow

1. Use `LogF8.Log()` for general-purpose debug output.
2. Use channel-specific methods for organized logging (Net, Event, Config, View, Entity).
3. Enable file writing with `FF8.LogWriter.OnEnterGame()` for production logging.
4. Add `GetCrashErrorMessage()` for crash error capture.
5. Add LogViewer to scene for runtime debug console.
6. Register debug commands via `Function.Instance.AddCommand()`.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Logs not appearing | `DisableLog()` was called | Call `EnabledLog()` |
| File write permission | No write access to log directory | Ensure `persistentDataPath` is writable |
| Background thread log crash | Using Unity API from background thread | Use `LogToMainThread()` instead |
| LogViewer not showing | Component not in scene | Add LogViewer GameObject or load as asset |

## Cross-module dependencies

- None — Log is a foundational utility used by all modules.

## Output checklist

- Logging pattern selected (channel-based, file-based, error capture).
- LogViewer configuration documented.
- Files changed and why.
- Validation status and remaining risks.
