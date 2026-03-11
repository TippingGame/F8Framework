---
name: f8-editor-componentbind-workflow
description: Use when working with ComponentBind editor tools — auto-generating component reference code from UI hierarchies in F8Framework.
---

# ComponentBind Editor Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task involves auto-binding UI component references in code.
- The user asks about component naming conventions for auto-generation.
- Troubleshooting null component references or binding code issues.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/ComponentBind
2. Runtime config: Assets/F8Framework/Runtime/ComponentBind/DefaultCodeBindNameTypeConfig.cs

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/ComponentBind
- Editor module: Assets/F8Framework/Editor/ComponentBind

## Naming conventions

Component references are auto-generated based on child GameObject naming:

| Format | Example | Generated Field |
|--------|---------|----------------|
| `SimpleName_ComponentType` | `Title_Text` | `Text Title_Text` |
| `Name_ComboType` | `Btn_Button_Image` | `Button Btn_Button; Image Btn_Image` |
| `Name_Type[index]` | `Item_Image[0]` | Array of Image components |

### Rules
- Only allows: letters, digits, underscores, and Chinese characters
- First character cannot be a digit
- Final format: `SimplifiedName_ComponentType`
- Combo types separated by underscore: `Button_Image`
- Arrays supported: `Image[0]`, `Image[1]`
- May need to click generate button twice

## Workflow

1. Create UI hierarchy with properly named child GameObjects.
2. Attach BaseView script to root GameObject.
3. Click the component bind generation button in Inspector.
4. Generated code appears between auto-generated markers in BaseView.
5. Add new type mappings in `DefaultCodeBindNameTypeConfig.cs` if needed.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Field not generated | Naming convention wrong | Check name format: `Name_Type` |
| Null reference at runtime | Hierarchy changed after generation | Regenerate bindings |
| Custom type not recognized | Not in DefaultCodeBindNameTypeConfig | Add type mapping |

## Cross-module dependencies

- **UI**: Works with BaseView UI panel system.

## Output checklist

- Child objects named with correct convention.
- Binding code generated.
- References validated at runtime.
