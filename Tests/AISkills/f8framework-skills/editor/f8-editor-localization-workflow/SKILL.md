---
name: f8-editor-localization-workflow
description: Use when working with Localization editor tools — F6 language switching, localization component Inspector, and Excel-based translation management in F8Framework.
---

# Localization Editor Workflow



## Use this skill when

- The task involves editing or testing localization in the Unity Editor.
- The user asks about F6 shortcut, language preview, or localization component setup.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/Localization

## Sources of truth

- Editor module: Assets/F8Framework/Editor/Localization
- Test docs: Assets/F8Framework/Tests/Localization/README.md

## Key editor features

| Feature | Shortcut | Description |
|---------|----------|-------------|
| **Language Switching** | F6 | Quick-switch between 42 supported languages in Editor |
| **Text ID Preview** | Inspector | Shows ID index directly on Text/TextMeshPro components |
| **Localization Component Inspector** | Inspector | Configure localization targets per component type |

## Supported editor component localization

- Text / TextMeshPro — with real-time ID display
- Font — per-language font selection
- Image / RawImage — language-specific images
- SpriteRenderer — language-specific sprites
- Material Renderer — language-specific materials
- Audio Clips — language-specific audio
- Timeline Tracks — language-specific timeline

## Workflow

1. Create `Localization.xlsx` in `StreamingAssets/config/`.
2. Add localization components to UI objects in Inspector.
3. Set ID references to match Excel row IDs.
4. Press **F6** to switch languages and preview in Editor.
5. For Text/TextMeshPro, the ID is shown in the Inspector.
6. For non-text components, set resource paths per language.

## Output checklist

- Localization components attached and configured.
- F6 preview tested for target languages.
- Excel translation table populated.
