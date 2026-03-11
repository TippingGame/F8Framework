---
name: f8-editor-playerprefseditor-workflow
description: Use when working with PlayerPrefsEditor tools — viewing, editing, and managing PlayerPrefs data in the Unity Editor in F8Framework.
---

# PlayerPrefsEditor Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves viewing or editing PlayerPrefs data in Editor.
- The user asks about inspecting or clearing stored preferences.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/PlayerPrefsEditor

## Sources of truth

- Editor module: Assets/F8Framework/Editor/PlayerPrefsEditor

## Key editor features

| Feature | Description |
|---------|-------------|
| **PlayerPrefs Viewer** | Editor window showing all stored PlayerPrefs key-value pairs |
| **Edit Values** | Modify PlayerPrefs values directly in Editor |
| **Delete Keys** | Remove specific or all PlayerPrefs entries |
| **Type Display** | Shows data type (int, float, string) for each entry |

## Workflow

1. Open PlayerPrefs Editor from the F8Framework menu.
2. View all stored key-value pairs with types.
3. Edit values directly in the editor window.
4. Delete unwanted keys.
5. Use to debug Audio volume/switch settings, Storage data, etc.

## Output checklist

- PlayerPrefs inspected.
- Values verified or modified.
- Stale data cleaned if needed.
