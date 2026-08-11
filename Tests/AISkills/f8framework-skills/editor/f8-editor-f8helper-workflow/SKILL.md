---
name: f8-editor-f8helper-workflow
description: Use when working with F8Helper editor tools — the F8Run pipeline, main development menu items, shortcuts, and one-click actions in F8Framework.
---

# F8Helper Editor Workflow



## Use this skill when

- The user asks about the F8 main menu, shortcuts, or one-click framework actions.
- The task involves understanding what F8Run executes or how it resumes after script compilation.

## Path resolution

1. Editor code: Assets/F8Framework/Editor

## Sources of truth

- Editor module: Assets/F8Framework/Editor

## Key editor features

| Shortcut | Action |
|----------|--------|
| **F8** | Starts the shared F8Run pipeline: optional contributors, hot-update DLL generation, and AssetBundle build |
| **F7** | Runtime Excel reload (development) |
| **F6** | Language switching (Localization editor) |
| **F5** | Build tool window |

## F8 One-click action includes

1. Create an `F8BuildRequest.CreateF8Run()` request.
2. Discover optional `IF8BuildPipelineContributor` implementations.
3. Run Excel code generation and serialization only when the Excel contributor is installed and enabled.
4. Request compilation and resume through `F8EditorPipeline` when generated scripts change.
5. Generate/copy hot-update DLLs.
6. Build AssetBundles and related indices.
7. Do not build a Player or update package; use the F5 build tool for those outputs.

## Workflow

1. Press **F8** after resource, hot-update code, or enabled extension data changes.
2. Configure optional contributors such as Excel in the F5 build tool before running F8.
3. For Excel-only generation, use menu **开发工具 → 2: Excel导表-F8**; it is not the F8 shortcut.
4. For targeted actions, use the specific manager's menu items.
5. Check console output and the shared pipeline state after F8.

## Output checklist

- F8 executed without errors.
- Enabled contributors completed or resumed successfully after compilation.
- Hot-update DLLs, AssetBundle indices, and bundles are up to date.
- No failed or pending pipeline remains unexpectedly.
