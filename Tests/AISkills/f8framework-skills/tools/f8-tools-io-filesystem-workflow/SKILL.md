---
name: f8-tools-io-filesystem-workflow
description: Use when working with IO/FileSystem tools — file read/write, directory operations, and path utilities in F8Framework.
---

# IO/FileSystem Tools Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves file I/O, directory management, or path manipulation.
- The user needs to read/write files, copy/move/delete, or manage directories.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/IO.cs (~35KB)

## Sources of truth

- Source file: Assets/F8Framework/Runtime/Utility/IO.cs

## Key capabilities

- File read/write (text, bytes, streams)
- Directory creation, deletion, enumeration
- File copy, move, rename
- Path manipulation and normalization
- File existence checking
- File size and metadata queries
- Platform-specific path handling
- Recursive directory operations

## Workflow

1. Use `Util.IO` or `FileTools` class (check actual class name in source).
2. All methods are static — no initialization needed.
3. Handle platform differences: use `Application.persistentDataPath` for writable paths.
4. Be careful with StreamingAssets on Android (read-only, in APK).

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| File write fails | Path not writable | Use `Application.persistentDataPath` |
| StreamingAssets read fails on Android | Files inside APK | Use `SyncStreamingAssetsLoader` |
| Path separator issues | Windows vs Unix paths | Use `Path.Combine()` and normalize |

## Output checklist

- File operation type identified.
- Platform-appropriate paths used.
- Error handling for I/O exceptions.
