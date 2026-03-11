---
name: f8-editor-playmodeplus-workflow
description: Use when working with PlayModePlus editor tools — editor play mode enhancements and development convenience features in F8Framework.
---

# PlayModePlus Editor Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves editor play mode enhancements or development convenience.
- The user asks about editor-only quality-of-life features during play testing.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/PlayModePlus

## Sources of truth

- Editor module: Assets/F8Framework/Editor/PlayModePlus

## Key editor features

| Feature | Description |
|---------|-------------|
| **Play Mode Enhancement** | Additional editor features active during Play Mode |
| **Quick Restart** | Convenience tools for rapid iteration |
| **State Inspection** | Enhanced state visualization during play |

## Workflow

1. PlayModePlus features are automatically active in Editor.
2. Leverage enhanced play mode features during development.
3. Features only affect Editor — no runtime impact on builds.

## Output checklist

- PlayModePlus features acknowledged.
- Editor environment optimized for development workflow.
