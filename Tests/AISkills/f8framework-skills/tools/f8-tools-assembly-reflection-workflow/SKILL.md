---
name: f8-tools-assembly-reflection-workflow
description: Use when working with Assembly Reflection tools — type discovery, reflection utilities, and assembly scanning in F8Framework.
---

# Assembly Reflection Tools Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves runtime type discovery, reflection, or assembly scanning.
- The user needs to find types by attribute, interface, or name pattern.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/Assembly.cs (~60KB)

## Sources of truth

- Source file: Assets/F8Framework/Runtime/Utility/Assembly.cs

## Key capabilities

- Scan all loaded assemblies for types
- Find types implementing specific interfaces
- Find types with specific attributes
- Type name and namespace queries
- Cached reflection for performance

## Workflow

1. Use Assembly.cs static methods for type discovery.
2. Results are typically cached internally for performance.
3. Used internally by ModuleCenter for auto-discovering StaticModules.
4. Used by ProcedureManager for auto-discovering ProcedureNodes.

## Output checklist

- Reflection query defined.
- Assembly scanning scope appropriate.
- Cache behavior understood.
