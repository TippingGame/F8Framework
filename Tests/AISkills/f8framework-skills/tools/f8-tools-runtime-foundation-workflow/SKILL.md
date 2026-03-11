---
name: f8-tools-runtime-foundation-workflow
description: Use when working with Runtime Foundation utility helpers — Algorithm, Assert, Program, Time, Unity utility classes in F8Framework.
---

# Runtime Foundation Tools Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves core utility methods shared across modules.
- The user asks about algorithms, assertions, time helpers, or Unity utility extensions.

## Path resolution

1. Source: Assets/F8Framework/Runtime/Utility

## Sources of truth

| File | Size | Purpose |
|------|------|---------|
| `Algorithm.cs` | ~22KB | Sorting, searching, math algorithms |
| `Assert.cs` | ~9KB | Runtime assertions and validation |
| `Program.cs` | ~2KB | Program/process utilities |
| `Time.cs` | ~15KB | Time formatting, date conversion, timestamps |
| `Unity.cs` | ~39KB | Unity-specific extension methods and helpers |

## Key utility areas

### Algorithm (Algorithm.cs)
- Sorting algorithms and helpers
- Math utilities
- Collection manipulation

### Assert (Assert.cs)
- Runtime assertion checks
- Parameter validation
- Type checking

### Program (Program.cs)
- Process startup utilities
- Platform-specific program helpers

### Time (Time.cs)
- Time format conversion
- Date/timestamp utilities
- Duration formatting

### Unity (Unity.cs)
- Transform, GameObject, Component extensions
- Color, Vector conversion helpers
- Rendering and layout utilities
- Platform detection

## Workflow

1. Identify the utility category needed (Algorithm/Assert/Time/Unity/Program).
2. All utilities are in the `Util` namespace or extensions.
3. Reference directly — no module initialization needed.
4. These are static/extension methods, not managed modules.

## Output checklist

- Utility category identified.
- Correct static/extension method called.
- No module initialization needed.
