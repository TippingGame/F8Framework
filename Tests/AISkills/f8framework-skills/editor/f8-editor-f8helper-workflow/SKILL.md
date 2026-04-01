---
name: f8-editor-f8helper-workflow
description: Use when working with F8Helper editor tools — the main F8 menu items, shortcuts, and one-click actions in F8Framework.
---

# F8Helper Editor Workflow



## Use this skill when

- The user asks about the F8 main menu, shortcuts, or one-click framework actions.
- The task involves understanding what F8 key does at a system level.

## Path resolution

1. Editor code: Assets/F8Framework/Editor

## Sources of truth

- Editor module: Assets/F8Framework/Editor

## Key editor features

| Shortcut | Action |
|----------|--------|
| **F8** | One-click: generates asset index, AB names, and config classes |
| **F7** | Runtime Excel reload (development) |
| **F6** | Language switching (Localization editor) |
| **F5** | Build tool window |

## F8 One-click action includes

1. Scan `Resources/` → generate resource index
2. Scan `AssetBundles/` → assign AB names and generate AB index
3. Scan `StreamingAssets/config/` → generate config data classes
4. Clean stale files and rebuild indices
5. Output to `Assets/F8Framework/AssetMap/` and `Assets/F8Framework/ConfigData/`

## Workflow

1. Press **F8** after any resource, asset, or config changes.
2. F8 is the primary "make everything work" action.
3. For targeted actions, use the specific manager's menu items.
4. Check console output for any generation errors post-F8.

## Output checklist

- F8 executed without errors.
- Asset index, AB names, and config classes up to date.
