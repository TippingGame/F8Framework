---
name: f8-tools-streamingassets-loader-workflow
description: Use when working with StreamingAssets Loader tools — sync reading of StreamingAssets files, especially on Android in F8Framework.
---

# StreamingAssets Loader Tools Workflow



## Use this skill when

- The task involves reading files from StreamingAssets directory.
- The user needs to access StreamingAssets on Android (where files are inside APK).
- Troubleshooting unable to read StreamingAssets files on mobile.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/StreamingAssetsHelper/

## Sources of truth

- Source directory: Assets/F8Framework/Runtime/Utility/StreamingAssetsHelper
- Key class: SyncStreamingAssetsLoader

## Key capabilities

- Synchronous file reading from StreamingAssets
- Android APK StreamingAssets access (via ZipFile reading)
- Cross-platform consistent file access
- File content reading (text, lines, bytes)

## API quick reference

```csharp
// Sync read all lines from StreamingAssets/config/
string[] files = SyncStreamingAssetsLoader.Instance.ReadAllLines("config/fileindex.txt");

// Release resources after use
SyncStreamingAssetsLoader.Instance.Close();
```

## Workflow

1. Use `SyncStreamingAssetsLoader` for reading StreamingAssets files.
2. On Android, uses ICSharpCode.SharpZipLib.Zip to read from APK directly.
3. Call `Close()` after reading to release resources.
4. On other platforms, reads directly from file system.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| File not found on Android | Path case sensitivity | Ensure exact case match |
| Resource leak | Not calling Close() | Always call Close() after reading |
| Read fail on WebGL | Different file access model | Use UnityWebRequest for WebGL |

## Output checklist

- StreamingAssets path verified.
- Platform-specific access pattern used.
- Resources properly released.
