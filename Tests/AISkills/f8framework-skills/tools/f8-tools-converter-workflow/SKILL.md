---
name: f8-tools-converter-workflow
description: Use when working with Converter tools — type conversion helpers and data format conversion utilities in F8Framework.
---

# Converter Tools Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves type conversions between common data formats.
- The user needs numeric, string, or Unity type conversion helpers.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/Converter.cs (~5KB)

## Sources of truth

- Source file: Assets/F8Framework/Runtime/Utility/Converter.cs

## Key capabilities

- Numeric type conversions (int ↔ float ↔ string)
- Unity type conversions (Vector ↔ string, Color ↔ string)
- Safe conversion with fallback defaults
- Batch conversion utilities

## Workflow

1. Use `Converter` class static methods.
2. No initialization needed — pure utility methods.
3. Safe methods return default values on conversion failure.

## Output checklist

- Conversion method identified.
- Input/output types verified.
- Fallback handling documented.
