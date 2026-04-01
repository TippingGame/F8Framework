---
name: f8-features-obfuz-workflow
description: Use when implementing or troubleshooting Obfuz feature workflows — code obfuscation and protection in F8Framework.
---

# Obfuz Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first.


## Use this skill when

- The task is about code obfuscation or protection using Obfuz.
- The user asks about protecting compiled assemblies from reverse engineering.
- The task includes Obfuz + HybridCLR integration or hot-update DLL obfuscation.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read both:
   - `Assets/F8Framework/Tests/Obfuz/README.md`
   - `Assets/F8Framework/Tests/Obfuz/README_EN.md`
3. If docs need updates, keep the Chinese and English README files aligned.

## Sources of truth

- Test docs: `Assets/F8Framework/Tests/Obfuz`
- Launcher/build integration points:
  - `Assets/F8Framework/Editor/F8Helper/F8Helper.cs`
  - hot-update entry `LoadDll.cs` in the target project
- Obfuz package / extension behavior as referenced by the test docs:
  - `Assets/Packages/com.code-philosophy.obfuz4hybridclr/README.md`

## Key settings and integration points

| Item | Guidance |
|------|----------|
| Obfuz import | Import Obfuz first from `https://github.com/focus-creative-games/obfuz.git`. |
| `AssembliesToObfuscate` | Add `Assembly-CSharp`, `F8Framework.Core`, `F8Framework.Launcher`, `F8Framework.F8ExcelDataClass`. |
| `NonObfuscatedButReferenceingObfuscatedAssemblies` | Add `F8Framework.Core.Editor`, `F8Framework.Tests`. |
| HybridCLR prebuild | Replace the HybridCLR-only prebuild command with `Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll();`. |
| Encryption init | Move Obfuz `EncryptionService` initialization code to the hot-update entry `LoadDll.cs`. |
| API compatibility | Set `Api Compatibility Level` to `.NET Framework`. |

## Workflow

1. Import Obfuz into Unity first.
2. Generate the Obfuz key by following the official quick-start guide, and mount the generated initialization code.
3. Open the `ObfuzSettings` window and populate the two assembly lists exactly as documented in `Tests/Obfuz/README.md` and `README_EN.md`.
4. If the project uses HybridCLR, update `F8Helper.cs`:
   - in `GenerateCopyHotUpdateDll`, replace the prebuild command with `Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll();`
   - uncomment the branch that redirects DLL output to `GetObfuscatedHotUpdateAssemblyOutputPath(...)`
5. Move the Obfuz `EncryptionService` initialization code into the hot-update entry `LoadDll.cs`.
6. Set `Api Compatibility Level` to `.NET Framework`.
7. Run F8 / build the project and verify the obfuscated output still works.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| API compatibility errors | Compatibility level is too low | Switch `Api Compatibility Level` to `.NET Framework` |
| F8Framework code breaks after obfuscation | Too many framework assemblies are obfuscated | Try setting all F8Framework assemblies to non-obfuscated first, then narrow scope |
| Real-time Excel loading fails | Obfuz affects runtime config loading path | Use `FF8.Config.RuntimeLoadAll()` for runtime Excel reload |
| `GeneratedEncryptionVirtualMachine.cs` init error | Generated file path is wrong | Fix the path in the `ObfuzSettings` window |
| HybridCLR hot-update DLL not found | F8Helper prebuild/output path not switched to Obfuz | Use `Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll()` and the obfuscated output-path branch |

## Cross-module dependencies

- **HybridCLR**: Required when hot-update DLLs also need Obfuz processing.
- **Build/F8Helper**: Obfuscation is wired into the prebuild/build pipeline.
- **ExcelTool / Config**: Real-time Excel reload may need `FF8.Config.RuntimeLoadAll()` after integration.

## Output checklist

- Obfuz import path confirmed.
- Obfuscation target assemblies defined.
- Non-obfuscated referencing assemblies defined.
- HybridCLR integration points updated when applicable.
- Encryption initialization location confirmed.
- Build test passed with obfuscation.
- Validation status and remaining risks.
