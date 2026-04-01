---
name: f8-features-localization-workflow
description: Use when implementing or troubleshooting Localization feature workflows — multi-language support, component localization, Excel-based translation tables in F8Framework.
---

# Localization Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.Local = ModuleCenter.CreateModule<Localization>(F8DataManager.Instance.GetLocalizedStrings());`.


## Use this skill when

- The task is about multi-language support, language switching, or localized assets.
- The user asks about Text/Image/Audio localization or Excel translation tables.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Localization/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Localization
- Editor module: Assets/F8Framework/Editor/Localization
- Test docs: Assets/F8Framework/Tests/Localization
- Translation table: Assets/StreamingAssets/config/Localization.xlsx

## Key classes and interfaces

| Class | Role |
|-------|------|
| `LocalizationManager` | Core module. Access via `FF8.Local`. |
| Localization components | Text, TextMeshPro, Font, Image, RawImage, SpriteRenderer, Material, Audio, Timeline |

## Supported component types

1. Text / TextMeshPro — with real-time ID display in editor
2. Font — font switching per language
3. Image / RawImage — localized images
4. SpriteRenderer — localized sprites
5. Material Renderer — localized materials
6. Audio Clips — localized audio
7. Timeline Tracks — localized timeline

## API quick reference

```csharp
// Change language
FF8.Local.ChangeLanguage("English");

// Get translated text (with format support)
string text = FF8.Local.GetTextFromId("test", "Support", "Format");
string text1 = FF8.Local.GetTextFromIdLanguage("test", "English");

// Language list and current language
var languages = FF8.Local.LanguageList;
string current = FF8.Local.CurrentLanguageName;

// Reload translation table
FF8.Local.Load();

// Refresh all localization components
FF8.Local.InjectAll();
```

## Excel setup

1. Create `Localization.xlsx` in `StreamingAssets/config/`.
2. Rename the Sheet to `LocalizedStrings`.
3. Column layout: ID | Language1 | Language2 | ...
4. Supports 42 languages.
5. Alternative: use ExcelTool `variant<name,variantName>` for lightweight localization.

## Workflow

1. Create `Localization.xlsx` with `LocalizedStrings` sheet.
2. Add localization components to UI elements (Text, Image, etc.).
3. Set ID references on components to match Excel IDs.
4. Call `FF8.Local.ChangeLanguage()` to switch languages.
5. Use **F6** shortcut for quick switching in Editor.
6. For runtime content, use `GetTextFromId()` for text retrieval.
7. Call `InjectAll()` to refresh all components after language change (if not using components).
8. Use **F6** in Editor for real-time language preview.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Text shows ID instead of translation | ID not found in Excel | Check ID spelling in Excel table |
| Language not available | Missing column in Excel | Add language column to Localization.xlsx |
| Font rendering wrong | Font doesn't support target language | Use font that covers target language character set |
| Android Excel read fails | StreamingAssets access on Android | Use binary cache or `SyncStreamingAssetsLoader` |

## Cross-module dependencies

- **ExcelTool**: Translation tables use Excel infrastructure. Alternative: use ExcelTool variants.
- **AssetManager**: Localized assets (images, audio) loaded via AssetManager.
- **UI**: Localization components attach to UI elements.

## Output checklist

- Translation Excel created with proper structure.
- Localization components attached to UI elements.
- Language switching tested.
- Validation status and remaining risks.
