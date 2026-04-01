---
name: f8-editor-module-workflow
description: Use when working with Module editor tools — right-click context menu for creating Module, ModuleMono, and StaticModule templates in F8Framework.
---

# Module Editor Workflow



## Use this skill when

- The task involves creating new module template files via Editor context menu.
- The user asks about the right-click create module feature.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/Module

## Sources of truth

- Editor module: Assets/F8Framework/Editor/Module
- Test docs: Assets/F8Framework/Tests/Module/README.md

## Key editor features

| Feature | Description |
|---------|-------------|
| **Create Module** | Right-click → F8 Module Center → Create Module template (ModuleSingleton) |
| **Create ModuleMono** | Right-click → F8 Module Center → Create ModuleMono template (ModuleSingletonMono) |
| **Create StaticModule** | Right-click → F8 Module Center → Create StaticModule template |

## Workflow

1. Right-click on a project folder.
2. Select **F8 Module Center** → desired module type.
3. Name the generated script file.
4. Implement the generated interface methods.
5. Register with ModuleCenter in GameLauncher.

## Output checklist

- Template created with correct base class.
- Interface methods populated.
- Module registered in startup code.
