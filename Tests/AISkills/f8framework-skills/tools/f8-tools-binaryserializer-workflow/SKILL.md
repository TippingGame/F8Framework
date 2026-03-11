---
name: f8-tools-binaryserializer-workflow
description: Use when working with BinarySerializer tools — binary serialization and deserialization utilities in F8Framework.
---

# BinarySerializer Tools Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves binary serialization/deserialization of data.
- The user needs to save/load binary data formats efficiently.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility/BinarySerializer.cs (~3KB)
2. Extended: Assets/F8Framework/Runtime/Utility/BinarySerializer/

## Sources of truth

- Source files: Assets/F8Framework/Runtime/Utility/BinarySerializer.cs and BinarySerializer/

## Key capabilities

- Serialize C# objects to binary byte arrays
- Deserialize binary data back to C# objects
- High-performance binary format for config data
- Used by ExcelTool for binary config cache

## Workflow

1. Use `BinarySerializer` class for serialization operations.
2. `Serialize(object)` → `byte[]`
3. `Deserialize<T>(byte[])` → T
4. Primarily used internally by ExcelTool binary export.

## Output checklist

- Serialization format selected (binary vs JSON).
- Serializable types validated.
- Binary data file path configured.
