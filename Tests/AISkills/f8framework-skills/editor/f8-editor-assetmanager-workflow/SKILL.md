---
name: f8-editor-assetmanager-workflow
description: Use when working with AssetManager editor tools — F8 shortcut, AB name management, asset index generation, and asset status inspector in F8Framework.
---

# AssetManager Editor Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves F8 shortcut actions (asset index, AB name generation).
- The user asks about AB name management, asset reference searching, or null reference checking.
- Troubleshooting missing AB names or stale asset index.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/AssetManager
2. Asset index output: Assets/F8Framework/AssetMap

## Sources of truth

- Editor module: Assets/F8Framework/Editor/AssetManager
- Asset index: Assets/F8Framework/AssetMap
- Test docs: Assets/F8Framework/Tests/AssetManager/README.md

## Key editor features

| Feature | Description |
|---------|-------------|
| **F8 Shortcut** | Generates Resources index + AssetBundle names + config classes |
| **AB Name Auto-assign** | Files in `Assets/AssetBundles` get AB names automatically |
| **Asset Status Inspector** | Editor window showing loaded/unloaded asset states |
| **Reference Search** | Right-click → Find if asset is referenced anywhere in project |
| **Clear AB Names** | Right-click → Clear selected files/folders AB names |
| **Set Same AB Name** | Right-click → Set all selected assets to same AB name |
| **Null Reference Finder** | Window showing assets with missing references |
| **Space → Explorer** | Hover file + press Space to open in system file manager |
| **Multi-process AB Build** | Unity 6000+ supports parallel AB building |

## Workflow

1. Press **F8** to regenerate all asset indices and AB names.
2. Assets in `AssetBundles/` auto-get AB names — existing names are NOT overwritten.
3. AB output goes to `StreamingAssets/AssetBundles/<Platform>/`.
4. Use right-click context menu for:
   - Find references (searches entire project, outputs to log)
   - Clear AB names on selected files
   - Set same AB name for multiple files
5. Use Null Reference Finder window to locate broken references.
6. For Unity 6000+: Enable multi-process AB build in Project Settings → Editor.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Stale AB names after file move | AB names don't auto-clear | Manually clear AB names on moved files |
| Empty AB directory | No assets loaded | Verify AB names assigned, rebuild with F8 |
| Index generation fails | Compilation errors | Fix code errors first, then press F8 |

## Output checklist

- F8 generation completed without errors.
- AB names verified on target assets.
- Asset index files generated in AssetMap.
