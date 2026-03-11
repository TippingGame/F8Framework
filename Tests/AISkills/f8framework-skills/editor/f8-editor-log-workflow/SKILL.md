---
name: f8-editor-log-workflow
description: Use when working with Log editor tools — LogViewer setup, debug command registration, and log editor settings in F8Framework.
---

# Log Editor Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves setting up LogViewer in a scene or configuring log display.
- The user asks about runtime debug console or cheat codes.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/Log

## Sources of truth

- Editor module: Assets/F8Framework/Editor/Log
- Test docs: Assets/F8Framework/Tests/Log/README.md

## Key editor features

| Feature | Description |
|---------|-------------|
| **LogViewer** | Runtime debug console with command system and system stats |
| **Activation** | PC: tilde key `~` / Mobile: five-finger long press 1 second |
| **Command System** | Register custom debug commands at runtime |
| **System Stats** | Shows device info, FPS, memory usage |

## Workflow

1. Add LogViewer component to a scene GameObject, or load as asset.
2. Register debug commands: `Function.Instance.AddCommand(this, "MethodName", params)`.
3. Register cheat codes: `Function.Instance.AddCheatKeyCallback(callback)`.
4. LogViewer appears at runtime via keyboard/touch activation.
5. Use LogViewer to execute commands, view logs, and check system status.

## Output checklist

- LogViewer added to scene or loadable as asset.
- Debug commands registered.
- Activation method documented for target platform.
