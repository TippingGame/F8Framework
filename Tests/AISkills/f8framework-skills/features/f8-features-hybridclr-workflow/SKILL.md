---
name: f8-features-hybridclr-workflow
description: Use when implementing or troubleshooting HybridCLR feature workflows — C# code hot update integration in F8Framework.
---

# HybridCLR Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first.


## Use this skill when

- The task is about C# code hot update via HybridCLR.
- The user asks about assembly hot loading, AOT metadata, or hot update code deployment.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/HybridCLR/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/HybridCLR
- Editor module: Assets/F8Framework/Editor/HybridCLR
- Test docs: Assets/F8Framework/Tests/HybridCLR

## Key concepts

| Concept | Description |
|---------|-------------|
| HybridCLR | Open-source C# hot update solution for Unity IL2CPP. |
| Hot Update Assembly | User C# assemblies that can be updated without rebuilding the app. |
| AOT Metadata | Ahead-of-Time compiled metadata that must be supplemented for hot update types. |
| Interpreter | Runtime C# interpreter integrated into IL2CPP for executing hot update code. |

## Workflow

1. Install HybridCLR via Package Manager following official docs.
2. Configure hot update assemblies in HybridCLR settings (which assemblies to hot-update).
3. Generate AOT metadata bridge via HybridCLR menu tools.
4. Build hot update DLLs separately from the main package (e.g., F8Helper menu tools).
5. Deploy DLLs alongside AssetBundles to CDN (package as `.bytes` or `.dll.bytes`).
6. At runtime, supplement AOT metadata first:
```csharp
foreach (var aotDllName in aotDllList)
{
    byte[] dllBytes = FF8.Asset.Load<TextAsset>(aotDllName + "by").bytes;
    HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
}
```
7. Load DLLs via `System.Reflection.Assembly.Load(textAsset.bytes)`.
8. Use with HotUpdateManager for version management and download.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| TypeLoadException | Missing AOT metadata | Regenerate AOT metadata and include in build |
| Assembly load fails | DLL not found or corrupt | Verify DLL path and re-download |
| IL2CPP build fails | HybridCLR not properly installed | Reinstall HybridCLR, check Unity version compatibility |

## Cross-module dependencies

- **HotUpdateManager**: Downloads and version-manages hot update DLLs.
- **AssetManager**: Hot update DLLs can be packaged as AssetBundle resources.

## Output checklist

- HybridCLR installed and configured.
- Hot update assemblies defined.
- AOT metadata generated.
- Validation status and remaining risks.
