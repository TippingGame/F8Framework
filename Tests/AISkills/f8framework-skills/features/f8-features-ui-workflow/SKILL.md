---
name: f8-features-ui-workflow
description: Use when implementing or troubleshooting UI feature workflows — panel management, layer control, BaseView template, async loading, and UI utilities in F8Framework.
---

# UI Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about UI panel management, open/close, layer control, or custom animations.
- The user asks about BaseView, UIConfig, Notify popups, or component auto-binding.
- Troubleshooting UI loading, layer ordering, or modal/non-modal behavior.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/UI/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/UI
- Editor module: Assets/F8Framework/Editor/UI
- Test docs: Assets/F8Framework/Tests/UI

## Key classes and interfaces

| Class | Role |
|-------|------|
| `UIManager` | Core module. Access via `FF8.UI`. |
| `BaseView` | Base class for all UI panels. Provides lifecycle and auto-binding. |
| `UIConfig` | Maps UIID to layer type and prefab asset name. |
| `UICallbacks` | Callbacks for onAdded, onRemoved, onBeforeRemove. |
| `UILoader` | Async load handle with guid. |
| `LayerType` | UI layer enum (UI, Popup, Notify, etc.). |

## UI types

1. **Normal UI** — Standard panels (menus, HUD)
2. **Modal Popup** — Shows oldest first, auto-shows next on close
3. **Non-modal Popup** — Multiple visible, newer on top, self-managed close

## API quick reference

### Initialization
```csharp
public enum UIID { UIMain = 1 }
Dictionary<UIID, UIConfig> configs = new Dictionary<UIID, UIConfig>
{
    //'UIMain' is the asset name in f8 features assetmanager workflow
    { UIID.UIMain, new UIConfig(LayerType.UI, "UIMain") }
};
FF8.UI.Initialize(configs);
```

### Canvas configuration
```csharp
FF8.UI.SetCanvas(null, sortOrder: 1, "Default",
    RenderMode.ScreenSpaceCamera, pixelPerfect: false, Camera.main);
FF8.UI.SetCanvasScaler(LayerType.UI,
    CanvasScaler.ScaleMode.ScaleWithScreenSize,
    referenceResolution: new Vector2(1920, 1080),
    CanvasScaler.ScreenMatchMode.MatchWidthOrHeight,
    matchWidthOrHeight: 0f);
```

### Open UI (sync)
```csharp
string guid = FF8.UI.Open(UIID.UIMain, data, new UICallbacks(
    (parameters, id) => { /* onAdded */ },
    (parameters, id) => { /* onRemoved */ },
    () => { /* onBeforeRemove */ }));
```

### Open UI (async)
```csharp
// async/await
await FF8.UI.OpenAsync(UIID.UIMain);
// Coroutine
yield return FF8.UI.OpenAsync(UIID.UIMain);
// With guid access
UILoader loader = FF8.UI.OpenAsync(UIID.UIMain);
yield return loader;
string guid = loader.Guid;
```

### Notify
```csharp
FF8.UI.ShowNotify(UIID.UIMain, "tip");
await FF8.UI.ShowNotifyAsync(UIID.UIMain, "tip");
```

### Query and close
```csharp
FF8.UI.Has(UIID.UIMain);              // Check existence
FF8.UI.GetByUIid(UIID.UIMain);        // Get by UIID
FF8.UI.GetByGuid(guid);               // Get by unique guid
FF8.UI.Close(UIID.UIMain, true);      // Close and destroy
FF8.UI.Clear(true);                    // Clear all (except Notify)
FF8.UI.Clear(LayerType.Notify, true);  // Clear specific layer
```

### BaseView template
```csharp
public class UIMain : BaseView
{
    protected override void OnAwake() { }
    protected override void OnAdded(int uiId, object[] args = null) { }
    protected override void OnStart() { }
    protected override void OnViewTweenInit()
    {
        // transform.localScale = Vector3.one * 0.7f;
    }
    protected override void OnPlayViewTween()
    {
        // ViewOpenSequence?.Append(transform.ScaleTween(Vector3.one, 0.7f));
    }
    protected override void OnViewOpen() { }
    protected override void OnPlayViewCloseTween()
    {
        // ViewCloseSequence?.Append(transform.ScaleTween(Vector3.zero, 0.7f));
    }
    protected override void OnBeforeRemove() { }
    protected override void OnRemoved() { }
    // Auto-generated component bindings below
}
```

## Additional UI utilities

| Utility | Description |
|---------|-------------|
| `SimpleRoundedImage` | Rounded corner image mask |
| `SafeAreaAdapter` | Notch/safe area adaptation |
| `UIParticleSystem` | Particle effects rendered on UI Canvas |
| `UIRedDot` | Red dot notification system |
| `SpriteSequenceFrame` | Sprite sequence frame animation |
| Nested Layout | Nested UI layout support |
| Infinite List | Virtual scrolling list |
| Drag & Drop | Drag support |
| Tab Pages | Tab navigation component |

## Workflow

1. Create UI prefab, place in `AssetBundles` or `Resources`.
2. Right-click → F8 UI → Create BaseView template, attach to prefab root.
3. Define UIID enum and register in configs dictionary.
4. Call `FF8.UI.Initialize(configs)` in startup.
5. Name child objects with type convention for auto-binding (e.g., `Button_Image`).
6. Click component bind button to generate code.
7. Open with `FF8.UI.Open()` or `OpenAsync()`.
8. Override `OnPlayViewTween()` for custom open animations.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| UI not showing | Missing from configs dictionary | Add UIConfig entry |
| Wrong layer order | Incorrect LayerType | Adjust LayerType in UIConfig |
| Auto-bind fields null | Naming convention wrong | Follow: `SimpleName_ComponentType` |
| Async load fails | Asset not found | Ensure prefab in loadable directory, press F8 |

## Cross-module dependencies

- **AssetManager**: UI prefabs loaded via asset system.
- **Tween**: Custom panel animations.
- **Event**: BaseView extends EventDispatcher.
- **Localization**: UI text localization.
- **ComponentBind**: Auto-generates component references.

## Output checklist

- UI prefab created with BaseView attached.
- UIConfig registered.
- Open/close methods implemented.
- Custom animations configured.
- Validation status and remaining risks.
