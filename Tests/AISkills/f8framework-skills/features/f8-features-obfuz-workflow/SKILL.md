---
name: f8-features-obfuz-workflow
description: Use when implementing or troubleshooting Obfuz feature workflows — code obfuscation and protection in F8Framework.
---

# Obfuz Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about code obfuscation or protection using Obfuz.
- The user asks about protecting compiled assemblies from reverse engineering.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Obfuz/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Obfuz
- Editor module: Assets/F8Framework/Editor/Obfuz
- Test docs: Assets/F8Framework/Tests/Obfuz

## Key concepts

| Concept | Description |
|---------|-------------|
| Obfuz | Code obfuscation tool integrated with F8Framework. |
| Name obfuscation | Renames classes, methods, fields to unreadable names. |
| Control flow obfuscation | Makes code logic harder to follow. |
| String encryption | Encrypts string literals in compiled code. |

## Workflow

1. Configure Obfuz settings in Editor (which assemblies to obfuscate).
2. Define exclusion rules for public APIs that should not be renamed.
3. Build with obfuscation enabled.
4. Verify obfuscated builds work correctly.
5. **HybridCLR Integration**:
    - Use `Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll()` in `F8Helper.cs`.
    - Move `EncryptionService` initialization to `LoadDll.cs`.
    - Set `Api Compatibility Level` to `.NET Framework`.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Reflection fails | Obfuscated names break reflection calls | Add exclusion rules for reflected types |
| Serialization breaks | Field names changed | Exclude serialized classes from obfuscation |
| Unity callbacks missing | Method names obfuscated | Exclude MonoBehaviour methods |
| LogViewer broken | Method lookup fails | LogViewer does not support obfuscation of UI-called methods |

## Cross-module dependencies

- **HybridCLR**: Obfuscated code can be hot-updated.
- **Build**: Obfuscation applies during the build process.

## Output checklist

- Obfuscation target assemblies defined.
- Exclusion rules configured.
- Build test passed with obfuscation.
- Validation status and remaining risks.
