---
name: f8-editor-exceltool-workflow
description: Use when working with ExcelTool editor tools — manual Excel import, F7 runtime reload, Excel directory and binary/JSON export settings, or optional Excel participation in F8/F5 build pipelines in F8Framework.
---

# ExcelTool Editor Workflow



## Use this skill when

- The task involves manual Excel generation or F7 runtime reload.
- The user asks about Excel directory, output format, or pre-build generation settings.
- Troubleshooting config generation, script reload, or optional pipeline integration.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/ExcelTool
2. Pipeline code: Assets/F8Framework/Editor/Pipeline

## Sources of truth

- Editor module: Assets/F8Framework/Editor/ExcelTool
- Pipeline integration: Assets/F8Framework/Editor/Pipeline/F8BuildPipeline.cs and F8EditorPipeline.cs
- Test docs: Assets/F8Framework/Tests/ExcelTool/README.md

## Key editor features

| Feature | Shortcut | Description |
|---------|----------|-------------|
| **Import Config Tables** | Menu | Excel-only import via `开发工具/2: Excel导表-F8`; this menu item has no F8 shortcut |
| **F8Run** | F8 | Runs the shared F8 pipeline; Excel participates only when pre-build generation is enabled |
| **Runtime Excel Reload** | F7 | Reloads Excel data at runtime without regeneration |
| **Pre-build Excel Generation** | F5 build tool | Enabled by default; enable/disable Excel steps for F8Run, Player, and update builds |
| **Set Excel Directory** | Menu | Change where Excel files are stored |
| **Export Format** | F5 build tool | Choose binary or JSON export format |
| **Output Directory** | F5 build tool | Configure where generated files go |

## Workflow

1. Place Excel files in `Assets/StreamingAssets/config/` (default).
   - Subdirectories are supported; `.xls`/`.xlsx` matching is case-insensitive.
   - `fileindex.txt` stores sorted paths relative to the Excel root with `/` separators for Android.
2. Configure **Pre-build Excel Generation**, source path, output path, and export format in the F5 build tool.
3. Choose the required entry:
   - Excel only: menu **开发工具 → 2: Excel导表-F8**.
   - Full F8Run: press **F8**; this also generates hot-update DLLs and AssetBundles.
   - Player/update build: use the F5 build tool; Excel runs automatically when enabled.
4. After an Excel import:
   - Generates C# data classes in `Assets/F8Framework/ConfigData/`
   - Generates binary/JSON in `Assets/AssetBundles/Config/BinConfigData/`
   - Require the exact case-sensitive Sheet name `LocalizedStrings`, exactly one `id` and one `TextID`; generate `ILocalizationItem` from every remaining language column in sheet order.
   - If generated C# changes, the shared editor pipeline requests compilation and resumes serialization after Domain Reload.
   - Serializes into a sibling staging directory and replaces the old output only after every table succeeds.
5. Press **F7** for runtime reload (development only).
6. Do not add Excel-specific `DidReloadScripts` or `AllScriptsReloaded` callbacks; return `RequestScriptReload` and let `F8EditorPipeline` resume the task.

## Optional pipeline integration

1. Keep Excel integration behind `ExcelBuildPipelineContributor`.
2. Return without adding steps when `ExcelDataSettings.Enabled` is false.
3. Keep dependencies one-way: Excel Editor references Core Editor; Core Editor must not reference Excel Editor.
4. Before removing the Excel module, cancel any pending pipeline that already contains `f8.excel.*` step IDs, then start a new build.
5. Keep the export directory dedicated to generated `.bytes`/`.json` files. The exporter rejects project roots, Excel source ancestors, subdirectories, and unrelated files instead of deleting them.
6. Keep the localization dependency one-way: generated configuration types implement Core's `ILocalizationItem`; Core must not reference `F8DataManager` or generated item types.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Import shows compilation errors | Generated code conflicts | Fix the Excel schema or generated naming conflict, then import again; do not delete the entire `ConfigData/` directory |
| Localization Sheet or reserved field validation fails | The Sheet name has different casing, or `id`/`TextID` appears more than once with casing variants | Use the exact Sheet name `LocalizedStrings` and keep exactly one `id` and one `TextID` column |
| Excel is skipped during F8/F5 | Pre-build Excel generation is disabled | Enable **Pre-build Excel Generation** if the build should refresh configs |
| Pipeline reports a missing `f8.excel.*` handler | Excel module was removed while an old pipeline was pending | Cancel the pending pipeline and start a new build |
| Existing config remains after an import failure | Parsing, reflection, or serialization failed before atomic commit | Fix the first reported error and retry; the previous config is intentionally preserved |
| Export directory is rejected | The path is unsafe or contains non-generated files/subdirectories | Select a dedicated config output directory; do not manually delete unrelated files through the exporter |
| Excel not found | Wrong directory | Check/set Excel directory via menu |
| F7 cannot find nested Excel on Android | `fileindex.txt` is stale | Run Excel import once to regenerate the relative-path index before building Android |

## Output checklist

- Manual import or F8/F5 pipeline entry selected intentionally.
- Pre-build Excel generation enabled/disabled intentionally.
- Generated data classes compile.
- `LocalizedStringsItem` implements `ILocalizationItem`; do not hand-edit the generated implementation.
- Export format and directory configured.
- Previous config remains usable if generation fails; no `.f8-staging-*` or `.f8-backup-*` directory remains after success.
- Pipeline resumed after compilation and cleared pending state.
