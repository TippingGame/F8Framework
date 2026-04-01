---
name: f8-editor-ui-workflow
description: Use when working with UI editor tools — sprite slicing, atlas tools, safe area setup, BaseView template creation, and UI utility components in F8Framework.
---

# UI Editor Workflow



## Use this skill when

- The task involves UI editor tools like sprite slicing, atlas management, or template creation.
- The user asks about nine-patch auto-slicing, atlas cutting, or image size optimization.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/UI

## Sources of truth

- Editor module: Assets/F8Framework/Editor/UI
- Test docs: Assets/F8Framework/Tests/UI/README.md

## Key editor features

| Feature | Description |
|---------|-------------|
| **Nine-Patch Auto-Slice** | Automatically cuts middle uniform color area to reduce image size |
| **Atlas Slice** | Cuts sprite atlas (requires Read/Write enabled, compression None, pre-sliced in Sprite Editor) |
| **Image Size to 4x** | Sets image dimensions to multiples of 4 (optimal for compression) |
| **Collect Chinese Text** | Scans all UI Chinese text into localization table |
| **Create BaseView Template** | Right-click → F8 UI → Create BaseView code template |

## Workflow

1. Create UI prefab in `AssetBundles` or `Resources`.
2. Right-click → **F8 UI** → **Create BaseView** to generate template.
3. Attach generated script to prefab root.
4. Use Inspector component binding to auto-generate code.
5. Use editor tools for image optimization:
   - Nine-patch auto-slice for large uniform images
   - Size-to-4x for compression optimization
   - Atlas slice for pre-sliced sprite sheets
6. Use Collect Chinese Text to populate localization table.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Atlas slice fails | Read/Write not enabled or not pre-sliced | Enable Read/Write, set compression to None, slice in Sprite Editor |
| BaseView already exists | Duplicate script generation | Delete old script or rename |

## Output checklist

- BaseView template created and attached.
- Image optimization tools applied.
- Localization text collected if applicable.
