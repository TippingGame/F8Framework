---
name: f8-features-hotupdatemanager-workflow
description: Use when implementing or troubleshooting HotUpdate feature workflows — version management, hot update, sub-package loading in F8Framework.
---

# HotUpdateManager Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about hot update version management, resource patching, or sub-packages.
- The user asks about CDN deployment, version checking, or asset hot update flow.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/HotUpdateManager/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/HotUpdateManager
- Editor module: Assets/F8Framework/Editor/Build
- Test docs: Assets/F8Framework/Tests/HotUpdateManager

## Key classes and interfaces

| Class | Role |
|-------|------|
| `HotUpdateManager` | Core module. Access via `FF8.HotUpdate`. |
| `GameConfig` | Holds version info including `LocalGameVersion`. |

## API quick reference

```csharp
// Required module initialization order
FF8.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();
FF8.Asset = ModuleCenter.CreateModule<AssetManager>();
FF8.Download = ModuleCenter.CreateModule<DownloadManager>();
yield return AssetBundleManager.Instance.LoadAssetBundleManifest();

// Initialize versions
FF8.HotUpdate.InitLocalVersion();
yield return FF8.HotUpdate.InitRemoteVersion();
yield return FF8.HotUpdate.InitAssetVersion();

// Check for updates
Tuple<Dictionary<string, string>, long> result = FF8.HotUpdate.CheckHotUpdate();
var hotUpdateAssetUrl = result.Item1;
var allSize = result.Item2;

// Start hot update
FF8.HotUpdate.StartHotUpdate(hotUpdateAssetUrl,
    () => { LogF8.Log("Success"); },
    () => { LogF8.Log("Failed"); },
    (eventArgs) =>
    {
        ulong downloaded = eventArgs.DownloadInfo.DownloadedLength;
        double speed = downloaded / eventArgs.DownloadInfo.DownloadTimeSpan.TotalSeconds;
        LogF8.Log($"Progress: {downloaded/(1024.0*1024.0):F2}MB/{allSize/(1024.0*1024.0):F2}MB, Speed: {speed/(1024.0*1024.0):F2}MB/s");
    });

// Sub-package support
List<string> subPackage = FF8.HotUpdate.CheckPackageUpdate(GameConfig.LocalGameVersion.SubPackage);
FF8.HotUpdate.StartPackageUpdate(subPackage,
    () => { LogF8.Log("Success"); },
    () => { LogF8.Log("Failed"); },
    (eventArgs) => { /* same progress tracking */ });
```

## Workflow

1. Initialize modules in strict order: HotUpdate → Asset → Download → LoadManifest.
2. Initialize local version, then remote version, then asset version.
3. Call `CheckHotUpdate()` to get the update manifest and total size.
4. Show update dialog with size info to user.
5. Call `StartHotUpdate()` with success/failure/progress callbacks.
6. For sub-packages (DLC), use `CheckPackageUpdate()` and `StartPackageUpdate()`.
7. After update, reload affected assets.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Remote version check fails | CDN unreachable | Check remote URL config in F5 build tool |
| Module init order crash | Wrong initialization order | Must be HotUpdate → Asset → Download → Manifest |
| Sub-package not found | Package naming wrong | Use `Package_ + identifier` folder naming |
| Local sandbox stale files | Previous test data | Clear sandbox directory before testing |

## Cross-module dependencies

- **AssetManager**: Hot update downloads and reloads asset bundles.
- **Download**: File download infrastructure.

## Output checklist

- Module initialization order verified.
- Hot update flow configured with callbacks.
- CDN deployment path documented.
- Validation status and remaining risks.
