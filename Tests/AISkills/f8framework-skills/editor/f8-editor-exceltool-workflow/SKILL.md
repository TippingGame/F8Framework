---
name: f8-editor-exceltool-workflow
description: Use when working with ExcelTool editor tools — F8/F7 shortcuts, Excel directory configuration, binary/JSON export settings in F8Framework.
---

# ExcelTool Editor Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves F8 (generate configs) or F7 (runtime reload) shortcuts.
- The user asks about Excel directory configuration or export format settings.
- Troubleshooting config table generation errors.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/ExcelTool

## Sources of truth

- Editor module: Assets/F8Framework/Editor/ExcelTool
- Test docs: Assets/F8Framework/Tests/ExcelTool/README.md

## Key editor features

| Feature | Shortcut | Description |
|---------|----------|-------------|
| **Import Config Tables** | F8 | Generates binary/JSON files and C# data classes from Excel |
| **Runtime Excel Reload** | F7 | Reloads Excel data at runtime without regeneration |
| **Set Excel Directory** | Menu | Change where Excel files are stored |
| **Export Format** | F5 build tool | Choose binary or JSON export format |
| **Output Directory** | F5 build tool | Configure where generated files go |

## Workflow

1. Place Excel files in `Assets/StreamingAssets/config/` (default).
2. Press **F8** → menu **Dev Tools → Import Config Tables**:
   - Generates C# data classes in `Assets/F8Framework/ConfigData/`
   - Generates binary/JSON in `Assets/AssetBundles/Config/BinConfigData/`
3. Press **F7** for runtime reload (development only).
4. Change Excel directory: menu **Dev Tools → Set Excel Directory**.
5. Change output directory: F5 build tool settings.
6. Change export format: F5 build tool (binary default, JSON optional).

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| F8 shows compilation errors | Generated code conflicts | Delete ConfigData/, fix Excel, re-press F8 |
| Excel not found | Wrong directory | Check/set Excel directory via menu |
| F7 fails on Android | StreamingAssets not directly readable | Use binary cache mode instead |

## Output checklist

- F8 generation completed.
- Generated data classes compile.
- Export format and directory configured.
