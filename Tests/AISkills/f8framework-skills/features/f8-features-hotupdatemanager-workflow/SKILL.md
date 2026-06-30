---
name: f8-features-hotupdatemanager-workflow
description: Use when implementing or troubleshooting HotUpdate feature workflows — version management, hot update, sub-package loading in F8Framework.
---

# HotUpdateManager Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();`.


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
| `GameConfig.SetAssetRemoteBaseAddressGetter` | Optional runtime CDN base URL provider. Appends `Remote/platform`. |
| `GameConfig.SetAssetRemoteFinalAddressGetter` | Optional runtime final remote asset root provider. Does not append suffix. |

## API quick reference

```csharp
// Optional remote URL provider.
// Use this when test/prod/channel URLs should be selected by code.
static string GetHotfixUrl()
{
#if TEST_VERSION
    return "https://cdn-test.xxx.com/hotfix/";
#else
    return "https://cdn-prod.xxx.com/hotfix/";
#endif
}

// Required module initialization order
FF8.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();
FF8.Asset = ModuleCenter.CreateModule<AssetManager>();
FF8.Download = ModuleCenter.CreateModule<DownloadManager>();
yield return AssetBundleManager.Instance.LoadAssetBundleManifest();

// Initialize versions
FF8.HotUpdate.InitLocalVersion();
GameConfig.SetAssetRemoteBaseAddressGetter(GetHotfixUrl);
yield return FF8.HotUpdate.InitRemoteVersion();
yield return FF8.HotUpdate.InitAssetVersion();

// Check for updates
var (downloadInfos, allSize) = FF8.HotUpdate.CheckHotUpdate();
// `allSize` is the remaining download size after local file checks / resume offsets.
// Both hot update and package checks return `List<HotUpdateManager.DownloadTaskInfo>`.

// Start hot update
FF8.HotUpdate.StartHotUpdate(downloadInfos,
    () => { LogF8.Log("Success"); },
    () => { LogF8.Log("Failed"); },
    (eventArgs) =>
    {
        long downloaded = eventArgs.TotalDownloadedLength;
        double speed = eventArgs.DownloadInfo.DownloadedLength / eventArgs.DownloadInfo.DownloadTimeSpan.TotalSeconds;
        LogF8.Log($"Progress: {downloaded/(1024.0*1024.0):F2}MB/{allSize/(1024.0*1024.0):F2}MB, Speed: {speed/(1024.0*1024.0):F2}MB/s");
    });

// Sub-package support
var (packageDownloadTasks, packageAllSize) = FF8.HotUpdate.CheckPackageUpdate(GameConfig.LocalGameVersion.SubPackage);
FF8.HotUpdate.StartPackageUpdate(packageDownloadTasks,
    () => { LogF8.Log("Success"); },
    () => { LogF8.Log("Failed"); },
    (eventArgs) => { /* same progress tracking */ });
```

## Workflow

1. Initialize modules in strict order: HotUpdate → Asset → Download → LoadManifest.
2. Initialize local version. If the remote URL should be selected by code, call `GameConfig.SetAssetRemoteBaseAddressGetter(...)` before remote version initialization.
3. Initialize remote version, then asset version.
4. Call `CheckHotUpdate()` to get prepared download infos and the actual remaining download size.
5. Show update dialog with size info to user.
6. Call `StartHotUpdate()` directly with the returned `downloadInfos` and success/failure/progress callbacks.
7. Read overall downloaded bytes from `eventArgs.TotalDownloadedLength`; `eventArgs.DownloadInfo.DownloadedLength` is only for the current file.
8. For sub-packages (DLC), use `CheckPackageUpdate(GameConfig.LocalGameVersion.SubPackage)` to prepare `DownloadTaskInfo` items and pass them directly to `StartPackageUpdate()`.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Remote version check fails | CDN unreachable or remote URL provider not registered early enough | Check remote URL config in F5 build tool, or call `GameConfig.SetAssetRemoteBaseAddressGetter(...)` before `InitRemoteVersion()` |
| Module init order crash | Wrong initialization order | Must be HotUpdate → Asset → Download → Manifest |
| Sub-package not found | Package naming wrong | Use `Package_ + identifier` folder naming |
| Progress only shows current file | Read wrong field | Use `eventArgs.TotalDownloadedLength` for overall progress |
| Local sandbox stale files | Previous test data | Clear sandbox directory before testing |

## Cross-module dependencies

- **AssetManager**: Hot update downloads and reloads asset bundles.
- **Download**: File download infrastructure.

## Output checklist

- Module initialization order verified.
- Hot update flow configured with callbacks.
- CDN deployment path documented.
- Validation status and remaining risks.
