---
name: f8-features-ui-workflow
description: Use when creating, modifying, opening, closing, animating, binding, loading, debugging, or validating F8Framework UI views and items, including UIManager layers, BaseView/BaseItem templates, ComponentBind generation, canvas setup, Notify/Dialog behavior, and UI resource cleanup.
---

# F8 UI Feature Workflow

## Mission and operating rules

Use this skill as the runtime source of truth for the F8Framework UI feature flow. Make the workflow executable by another agent: identify the correct source files, preserve generator-owned code, implement the smallest change, and verify the complete open/close path.

When receiving a UI task, follow this order:

1. Classify the request as runtime behavior, editor authoring, generated code, or a combination.
2. Read the relevant source files listed below before editing. Treat current source as authoritative over README examples.
3. Check the bootstrap gate and the asset key before diagnosing a UI symptom.
4. Decide the layer and lifecycle (`BaseView` or `BaseItem`) before writing code.
5. Keep generated regions intact and regenerate them from the editor when the prefab hierarchy changes.
6. Validate both the first activation and a cached re-open; test destruction separately when `isDestroy` is involved.
7. Report the UIID, asset key, layer, lifecycle hooks, binding mode, cleanup behavior, tests run, and remaining risks.

Do not silently invent a new UI manager, layer, prefab-loading path, or binding convention. If the requested behavior conflicts with the implementation, show the conflict and route the change to the owning source file.

## Scope, paths, and routing

Prefer `Assets/F8Framework`. If the project is package-based, apply the same relative paths below under `Packages/F8Framework`.

| Area | Source of truth |
| --- | --- |
| UI manager and public API | `Assets/F8Framework/Runtime/UI/UIManager.cs` |
| View lifecycle and tracked resources | `Assets/F8Framework/Runtime/UI/Base/BaseView.cs` |
| Item lifecycle and tracked resources | `Assets/F8Framework/Runtime/UI/Base/BaseItem.cs` |
| Async handle | `Assets/F8Framework/Runtime/UI/Base/UILoader.cs` |
| Callback/state records | `Assets/F8Framework/Runtime/UI/ViewParams.cs` and `DelegateComponent.cs` |
| Layer behavior | `Assets/F8Framework/Runtime/UI/Layer/` |
| Binding generator | `Assets/F8Framework/Runtime/ComponentBind/ComponentBind.cs` |
| Binding name map | `Assets/F8Framework/Runtime/ComponentBind/DefaultCodeBindNameTypeConfig.cs` |
| Binding inspector | `Assets/F8Framework/Editor/ComponentBind/ComponentBindEditor.cs` |
| Script templates and creation menu | `Assets/F8Framework/Editor/UI/BaseViewTemplate.cs.txt`, `BaseItemTemplate.cs.txt`, `CreateUIScript.cs` |
| UI editor utilities | `Assets/F8Framework/Editor/UI/` |
| Usage examples | `Assets/F8Framework/Tests/UI/README.md`, `README_EN.md`, and `DemoUIManager.cs` |

Route related work as follows:

- Bootstrap or module-order questions: `f8-foundation-bootstrap-workflow`.
- Prefab/resource keys, Resources/AssetBundle mode, or unloading details: `f8-features-assetmanager-workflow`.
- Open/close tweens and sequences: `f8-features-tween-workflow`.
- Event ownership and global messages: `f8-features-event-workflow`.
- Component naming or Inspector generation: `f8-editor-componentbind-workflow`.
- Template creation, atlas/slicing, image tools, and UI editor menus: `f8-editor-ui-workflow`.
- Localization components or the Chinese-text collector: `f8-features-localization-workflow`.

## Bootstrap gate

Do not debug `FF8.UI` before confirming that F8 has been initialized. The minimum valid order is:

```csharp
IEnumerator Start()
{
    ModuleCenter.Initialize(this);                 // required first
    FF8.Asset = ModuleCenter.CreateModule<AssetManager>();
    // The standard launcher loads AssetBundleManifest here when AB mode is used.
    FF8.Tween = ModuleCenter.CreateModule<Tween>(); // required before UI tween code
    FF8.UI = ModuleCenter.CreateModule<UIManager>();
    FF8.UI.Initialize(configs);                    // creates the six UI layers
}
```

The project launcher creates the other dependency modules and awaits `AssetBundleManager.Instance.LoadAssetBundleManifest()` after `AssetManager` and before later modules. Preserve that order; use the bootstrap skill for the complete launcher sequence. `MessageManager`, `TimerManager`, `InputManager`, and `Tween` must exist before code that uses their APIs.

`ModuleCenter.CreateModule<UIManager>()` invokes `UIManager.OnInit`. If no `EventSystem.current` exists, `OnInit` creates an `EventSystem` and the input module selected by the Unity compile symbols. Still verify that the scene has the expected input setup.

`UIManager.Initialize(...)` builds `LayerGame`, `LayerUI`, `LayerPopUp`, `LayerDialog`, `LayerNotify`, and `LayerGuide`. Call it once after creating a manager and before any `Open` call. If the manager is terminated and recreated, initialize the new instance again; do not repeatedly initialize the same live manager just to change one config, because initialization creates new layer objects.

Press F8 before relying on AssetBundle indexes or generated config data. In development, follow the AssetManager skill when `IsEditorMode` is required.

## Configuration and layer selection

`UIConfig` contains only a `LayerType` and an AssetManager `AssetName`. The key can be an enum or an `int`; enum overloads are converted to `int` internally.

```csharp
public enum UIID
{
    Main = 1,
    Notify = 2,
}

private readonly Dictionary<UIID, UIConfig> configs = new Dictionary<UIID, UIConfig>
{
    { UIID.Main,   new UIConfig(LayerType.UI,     "Main") },
    { UIID.Notify, new UIConfig(LayerType.Notify, "Notify") },
};
```

Use the exact asset key recognized by AssetManager. Do not add a `.prefab` suffix or a new path convention unless that key is how the project indexes the asset.

| Layer | Default sort order | Select it for | Important behavior |
| --- | ---: | --- | --- |
| `Game` | 100 | Game-world or game HUD bucket | Standard `LayerUI` behavior. |
| `UI` | 200 | Normal screens, menus, HUD panels | One active/loading view per prefab path. |
| `PopUp` | 300 | A separate standard popup bucket | No special queue logic in the current implementation. |
| `Dialog` | 400 | Modal dialogs that must serialize | One queue per `uiId`; only the oldest is shown. |
| `Notify` | 500 | Toasts/notifications that may coexist | Multiple instances are allowed; close by `Guid` for one instance. |
| `Guide` | 600 | Tutorial/guide bucket | Standard `LayerUI` behavior. |

For normal layers, opening an already active or loading prefab is a duplicate: synchronous `Open` usually returns `string.Empty`, while asynchronous `OpenAsync` returns the existing loader. `isDestroy = false` hides and caches the instance; `isDestroy = true` destroys it and releases the prefab load. A cached instance is not active, so `Has` only reports currently active views.

Normal-layer uniqueness is keyed by `UIConfig.AssetName` (the prefab path), not only by `uiId`. Do not register the same prefab under multiple normal-layer IDs unless sharing one instance is intentional. A direct `Close` while a view is still pending its first async load is ignored by the current layer implementation; `Clear(...)` is the API that cancels pending loads. A view in `Closing` state is also treated as a duplicate, so wait for the close tween/removal to finish before reopening the same prefab.

`Dialog` requests with the same `uiId` are queued. The oldest request is displayed; after it closes, the next request is shown after one frame. Do not implement a second queue in the view script. For a queued asynchronous request, wait for its own `UILoader` and then verify that its view is actually active.

For a queued synchronous `Dialog` request, the returned guid identifies the currently shown or cached view rather than a newly instantiated queued object. Use the callbacks or an async loader when the caller must track an individual queued request.

`Notify` can show the same prefab more than once. Each active instance has a unique `Guid`; call `FF8.UI.Close(uiId, isDestroy, guid)` or `BaseView.Close(...)` when closing one notification. Omitting the guid closes instances for that prefab according to the layer implementation.

## End-to-end workflow for a new view

Execute these steps in order and stop to fix the first failed gate:

1. Prepare a prefab under a loadable location (`AssetBundles` for the normal production path or `Resources` for prototyping). Confirm its AssetManager key and press F8 when indexes are stale.
2. Select the project folder in Unity and use `Assets/（F8UI界面管理功能）/（BaseView.cs）` to create a view script, or `（BaseItem.cs）` for a reusable list/card item. Attach the generated script to the prefab root (not to a nested visual child).
3. Keep the generated class name, namespace, and file name compilable. A `BaseView` root represents one UIManager-managed prefab; a `BaseItem` root represents a reusable child item.
4. Add a stable `UIID` and a matching `UIConfig`. Choose the layer from the table above; keep the asset key identical to the prefab key used by AssetManager.
5. Build the hierarchy and name bindable children using the ComponentBind contract below. Put a `BaseItem` subtree behind its own binding boundary.
6. On the root component, click the convention or all-components binding button. The Inspector labels are `按照约定名称UI组件绑定（需要点击两次）` and `搜索全部UI组件绑定（需要点击两次）`; click again if Unity has not refreshed the generated script.
7. Wait for compilation and inspect the generated fields, listener override, and `SetComponents` assignments. If a field is null, fix the hierarchy/name and regenerate; do not hand-patch the generated block.
8. Initialize UI once, then configure Canvas and CanvasScaler if the defaults are not suitable. Configure them before opening views.
9. Use `Open` for an immediate synchronous path or `OpenAsync` when loading may take time. Pass view data through `object[]` and use callbacks only for external orchestration.
10. Implement lifecycle logic in the appropriate hook. Test first activation, close, cached reopen, and destroy/recreate before calling the feature complete.

For a change to an existing view, first identify whether the change belongs in the prefab, the hand-written region of the view script, the template, or `ComponentBind.cs`. Regenerate after hierarchy changes and keep the generated region reproducible.

## AI code-generation protocol

Use this protocol whenever an AI agent is asked to create or extend a UI script. Separate authored behavior from Unity-generated bindings:

| Responsibility | Owner | Required action |
| --- | --- | --- |
| Script skeleton and lifecycle hooks | Unity template + AI | Create from the BaseView/BaseItem template when possible; then fill only the hand-written hooks. |
| Prefab hierarchy and component placement | Unity Editor/user | Create or inspect the prefab and attach the root script. Do not fabricate hierarchy facts from a screenshot-free prompt. |
| Serialized component fields, paths, arrays, and control listeners | `ComponentBind` | Name the objects, click the binding button, and let the generator write the delimited region. |
| UI behavior and data flow | AI | Implement `OnAdded`, `Refresh`, `ButtonClick`, `ValueChange`, tween hooks, callbacks, and cleanup outside the generated region. |
| Compile and prefab-reference verification | Unity Editor | Refresh/compile, inspect the generated fields, and run the first-open/reopen test. |

Follow these AI steps:

1. Inspect the target prefab, existing script, UIID/config, asset key, and selected layer. If the prefab hierarchy is unavailable, state that binding cannot be verified instead of guessing field names.
2. Choose `BaseView` for a UIManager-managed panel or `BaseItem` for a reusable child. Use the exact template menu/path or mirror the current template hook surface when editor automation is unavailable.
3. Preserve the marker pair in the new script. Use `// Auto-generated component bindings below` for new code; accept `// 自动获取组件（自动生成，不能删除）` in existing code. Never place hand-written logic between the markers.
4. Build a binding plan from actual child names and components. Record expected fields, including TMP legacy/type choices and array indices, before asking Unity to bind.
5. Invoke the Inspector binding action (`Bind` or `BindAllUIComponents`) and wait for Unity refresh/compile. Click twice when the Inspector label requires it. Read the generated fields and listener override back from the script; do not infer them from the plan.
6. Generate behavior against the fields that actually exist. Prefer generated `ButtonClick`/`ValueChange` callbacks; make `OnAdded` and `Refresh` idempotent for cached reuse; reset visual state in `OnViewTweenInit` when close tweens are used.
7. Keep all custom state, API calls, event ownership, and teardown outside the generated region. If a generator limitation is found, fix the template/config/generator source and regenerate rather than patching one view's generated code.
8. Validate the script in Unity: compile, open synchronously or asynchronously, verify `GetByGuid`/`Has`, exercise close/reopen/destroy, and inspect the Console. If Unity is unavailable, report editor validation as pending.

When handing off an AI-generated UI script, report: script path/class/base type, UIID and asset key, layer, prefab binding mode, actual generated field names, lifecycle hooks implemented, close/cache policy, explicit cleanup points, and editor/runtime checks performed.

Do not hand-write `[SerializeField]` references that `ComponentBind` can generate, do not invent `transform.Find` paths, and do not edit a generated listener or `SetComponents` method to make a single prefab compile. The generated output must be reproducible from the prefab hierarchy.

## ComponentBind contract

### Modes and hierarchy boundaries

- `Bind()` is convention mode. It scans descendants, but if a descendant has `BaseItem` (or a subclass), it does not traverse that item's children. Bind the item with its own script.
- `BindAllUIComponents()` scans configured component types on every scanned child, excluding `GameObject` and `Transform` from all-components mode.
- The component must be attached to the same GameObject as the name match; the generator does not search up or down for the component.
- Both modes generate serialized fields, reference assignments in `SetComponents()`, and listener code for supported interactive controls.

### Naming rules

The current map is `DefaultCodeBindNameTypeConfig.BindNameTypeDict`. In convention mode, split the GameObject name on `_`; each segment is compared with mapping keys using case-sensitive `Contains`. For example, `Title_Text`, `Confirm_btn`, and `Btn_Button_Image` are meaningful conventions only when the corresponding components are on that same object.

The generated field is the normalized child name plus `_` plus the matched mapping key. Normalization keeps ASCII letters, digits, underscores, and Chinese characters; it removes spaces, brackets, parentheses, and other punctuation; a leading digit receives `_`. Collisions receive `_2`, `_3`, and so on. Renaming a child changes the field name and requires regeneration plus compile.

In all-components mode, the suffix is derived from the component type name (for example, `TMP_Text` is normalized to `TMPText`) rather than from the GameObject's alias. In convention mode, the suffix is the matched map key.

The underscore in an alias is significant: the generator splits names on `_` before matching. Therefore a natural name such as `Search_TMP_InputField` becomes the segments `TMP` and `InputField` and does not match the configured `TMP_InputField` key. For a TMP input in convention mode, use one unsplit key such as `Search_InputField (TMP)` (with the component on that object), add a project alias without `_`, or use all-components mode. Apply the same check to any custom alias containing `_`.

Common current keys include:

| Component family | Keys/aliases |
| --- | --- |
| Object and transform | `GameObject`, `go`, `Transform`, `tf`, `RectTransform` |
| Basic UI | `Button`, `btn`, `Image`, `img`, `RawImage`, `rimg`, `Text`, `Text (Legacy)`, `txt` |
| Input/value controls | `InputField`, `InputField (Legacy)`, `Slider`, `Toggle`, `ToggleGroup`, `Scrollbar`, `ScrollRect` |
| TextMeshPro | `Text (TMP)`, `tmp`, `TextMeshProUGUI` |
| TMP controls | `Dropdown`, `TMP_Dropdown`, `InputField (TMP)`, `TMP_InputField` |
| Layout and rendering | `VerticalLayoutGroup`, `HorizontalLayoutGroup`, `GridLayoutGroup`, `Canvas`, `CanvasGroup`, `Animator`, `Animation`, `SpriteRenderer`, `Mask`, `RectMask2D` |

The map distinguishes legacy and TMP controls by the exact key. In particular, `Dropdown`/`TMP_Dropdown` bind `TMPro.TMP_Dropdown`, while `Dropdown (Legacy)` binds `UnityEngine.UI.Dropdown`; `InputField`/`InputField (Legacy)` are legacy and `InputField (TMP)`/`TMP_InputField` are TMP.

Add project-specific aliases to `DefaultCodeBindNameTypeConfig.cs`; do not duplicate the map inside a view script. Keep aliases unambiguous because matching is substring-based and case-sensitive.

### Arrays and generated listeners

Use a trailing numeric index such as `Item_Image[0]`, `Item_Image[1]`. The generator groups the indexed paths and allocates an array of `maxIndex + 1`; use contiguous indices to avoid null slots. The current generator also emits its normal per-child field before the array aggregation, so inspect the generated block instead of assuming that only one array field exists. Regenerate after adding, removing, or reordering indexed children and inspect the resulting field names.

The generator creates listener fields and an `OnAddUIComponentListener()` override for `Button`, `Slider`, `Scrollbar`, legacy/TMP `Dropdown`, `Toggle`, legacy/TMP `InputField`, and `ScrollRect`. Buttons call `ButtonClick(UIBehaviour)`; value controls call `ValueChange<T>(UIBehaviour, T)`. Do not add a second override with the same name. Put custom behavior in `ButtonClick`/`ValueChange` or outside the generated region.

Prefer the generated callback contract for a TMP input instead of adding another `onValueChanged` listener:

```csharp
protected override void ValueChange<T>(UIBehaviour ui, T value)
{
    if (ui is TMPro.TMP_InputField && value is string text)
    {
        // React to the generated listener once; do not subscribe again in OnAdded.
    }
}
```

### Generated markers and compatibility

The canonical delimiter in new templates is exactly:

```csharp
// Auto-generated component bindings below
// ... generated fields, listeners, and SetComponents ...
// Auto-generated component bindings below
```

The generator also accepts the legacy delimiter exactly as written below:

```csharp
// 自动获取组件（自动生成，不能删除）
// ... old generated region ...
// 自动获取组件（自动生成，不能删除）
```

`ComponentBind.cs` matches either pair and writes the English marker back when regenerating. The two identical marker comments in the templates are intentional start/end delimiters; do not remove one, translate it, or “deduplicate” it. If neither pair exists, generation logs that the insertion marker cannot be found.

Treat everything between the markers as generator-owned, including `[SerializeField]` fields, the generated listener override, and the editor-only `SetComponents()` method. Keep hand-written fields and overrides outside the delimiters. After a prefab rename or hierarchy change, regenerate instead of editing serialized references by hand.

## BaseView lifecycle and template rules

Use `BaseView` for a UIManager-opened view. The effective order is:

| Phase | Hook/order | Frequency and responsibility |
| --- | --- | --- |
| Unity activation | `Awake` -> `OnAwake` -> generated `OnAddUIComponentListener` | Once per instantiated object. Generated fields must already be serialized. |
| First Unity activation | `Start` -> `OnStart` (Unity may defer this until the first frame) | Once per instance, not once per open; it is not a prerequisite for the first `OnAdded`. |
| Every open | assign `Args`, `UIid`, `Guid` -> `OnAdded` -> `OnViewTweenInit` -> `OnPlayViewTween` | Runs on every open, including cached re-open. Read `Args` in `OnAdded`. |
| Open completion | `OnViewOpen` | Default `OnPlayViewTween` calls it immediately; a real open sequence calls it on sequence completion. |
| Every close | `OnPlayViewCloseTween` | A close sequence delays removal; no sequence continues immediately. |
| Before removal | internal tracked event/timer cleanup -> `OnBeforeRemove`; then external `UICallbacks.OnBeforeRemove` | Runs on every close. Unsubscribe direct global/input/UnityEvent listeners here. |
| After removal | external `UICallbacks.OnRemoved` -> `OnRemoved` -> asset tracker release | Runs after close; `isDestroy` controls full unload/destruction. |

`OnAdded` is the place to refresh from arguments, but do not accumulate global subscriptions there without first removing them. Cached objects run `OnAdded` again while `OnStart` does not run again.

`LayerUI.CreateNode` calls `BaseView.Added` immediately after instantiation, while Unity can defer `Start`; therefore the first `OnAdded`/tween hooks may run before `OnStart`. Put required reference/setup initialization in `Awake` or `OnAdded`, not only in `OnStart`.

Override the protected template hooks; do not add derived `Awake`/`Start` methods that hide BaseView's private Unity message methods. Use `OnAwake` and `OnStart` instead.

`UICallbacks.OnAdded` is invoked after `BaseView.Added` starts the view; it does not mean that an open tween has finished. Use `OnViewOpen` for visual-open completion. During removal, external `UICallbacks.OnRemoved` runs before `BaseView.OnRemoved`.

Treat `Args` as nullable. `ShowNotify`/`ShowNotifyAsync` wrap the content in a one-element array (`Args[0]`), while an ordinary `Open` call with no payload leaves `Args` null.

### BaseView reserved interfaces: choose by intent

Use the template's reserved interfaces instead of inventing parallel methods:

| Intent | Interface | How the AI should use it |
| --- | --- | --- |
| One-time component/reference setup | `OnAwake()` | Read serialized bindings and initialize instance-only state. Do not put per-open arguments here. |
| Receive data on every open | `OnAdded(int uiId, object[] args)` | Refresh from `Args`, `UIid`, and `Guid`; make the code idempotent because cached views call it again. |
| One-time first activation | `OnStart()` | Use for work that truly runs once per instance. Do not use it as the per-open refresh hook. |
| Reset animation baseline | `OnViewTweenInit()` | Restore scale, alpha, interactability, and other visual defaults on every open. |
| Start open animation | `OnPlayViewTween()` | Append to `ViewOpenSequence`, or call the base behavior when no animation is needed. |
| Visual open completion | `OnViewOpen()` | Start work that must wait until the open sequence finishes. |
| Start close animation | `OnPlayViewCloseTween()` | Build `ViewCloseSequence` and disable interaction/guard callbacks while closing. |
| Cleanup before removal | `OnBeforeRemove()` | Unsubscribe direct global/input/custom UnityEvent listeners. BaseView's tracked event/timer cleanup has already run. |
| Post-close bookkeeping | `OnRemoved()` | Finish view-owned post-close state; do not assume the object will be destroyed because it may be cached. |
| Button control behavior | `ButtonClick(UIBehaviour ui)` | Handle generated Button listeners; do not add a second `onClick` subscription for the same control. |
| Value control behavior | `ValueChange<T>(UIBehaviour ui, T value)` | Handle generated Slider/Toggle/InputField/Dropdown/ScrollRect listeners. |
| Generated binding plumbing | `OnAddUIComponentListener()` and `SetComponents()` | Treat as generator-owned; never hand-edit or define a competing override. |

Do not use `OnViewOpen` as a substitute for `OnAdded`: it has no guarantee that new arguments have been applied for the next open. Do not use `OnRemoved` as a substitute for `OnBeforeRemove`: direct subscriptions must be detached before the view is hidden or cached.

For open tweens, use the protected sequence property as in the template:

```csharp
protected override void OnPlayViewTween()
{
    ViewOpenSequence?.Append(transform.ScaleTween(Vector3.one, 0.7f));
    // With a sequence, let BaseView invoke OnViewOpen on completion.
}
```

If an override creates no sequence, call `OnViewOpen()` explicitly (or call `base.OnPlayViewTween()`). Do not both append a sequence and call `OnViewOpen()` manually, or the completion hook fires twice. Ensure `FF8.Tween` exists before using `ViewOpenSequence`/`ViewCloseSequence`.

`BaseView` cancels an in-flight open sequence before a new open and cancels the open/previous close sequence when a close starts. Keep rapid open/close behavior idempotent and do not retain stale sequence references.

Reset the initial visual state in `OnViewTweenInit` on every open when using a close tween. For example, a cached view scaled to zero or faded out by `OnPlayViewCloseTween` must restore `localScale`, `CanvasGroup.alpha`, `interactable`, and `blocksRaycasts` before the next open; `isDestroy = false` preserves the instance state.

`OnBeforeRemove` runs only after a close tween completes. Generated control listeners therefore remain active during the close animation. If closing UI must not accept input, disable the relevant `CanvasGroup`/raycast targets or guard the handlers as soon as closing begins; do not remove the generated listeners unless you also rebind them on the next enable.

Use `Close(bool isDestroy = false)` on the view when possible; it forwards `UIid` and `Guid` to `UIManager`. The guid is meaningful for `Notify` instances and harmless for other layers.

## BaseItem lifecycle and template rules

Use `BaseItem` for reusable list/grid/tab children, not for a UIManager panel. It has no `Added`, `BeforeRemove`, `Removed`, or `Close` lifecycle from `UIManager`.

The template lifecycle is:

1. `Awake` -> `OnAwake` -> `OnViewTweenInit` -> generated `OnAddUIComponentListener`.
2. `Start` -> `OnStart` -> `OnPlayViewTween`; the default play hook calls `OnViewOpen`.
3. `Refresh()` is the explicit data-refresh entry point generated by the template.
4. `OnEnable`/`OnDisable` handle pooled or cached visibility changes.

Keep the template's `OnDisable` cleanup unless there is a documented replacement:

```csharp
base.ClearEventDispatcher();
base.ClearTimerTracker();
base.ClearAssetLoadTracker(false);
```

Keep one `OnEnable` and one `OnDisable` Unity message on an item; edit the template hook bodies rather than declaring duplicate methods. Do not hide BaseItem's private `Awake`/`Start`; use its protected hooks.

If an item needs to reset data on reuse, do it in `Refresh` or `OnEnable`; do not assume `Start` runs again. The ComponentBind traversal boundary means an item owns its own child bindings.

### BaseItem reserved interfaces: choose by intent

| Intent | Interface | Frequency/rule |
| --- | --- | --- |
| One-time instance setup | `OnAwake()` | Runs during `Awake`; use serialized bindings and instance-only initialization. |
| Initial item animation setup | `OnViewTweenInit()` | Runs once during `Awake`, unlike BaseView's per-open call. Reset pooled visuals in `OnEnable` as well when needed. |
| One-time first activation | `OnStart()` | Runs once; never use it as the item refresh path. |
| Start the item animation | `OnPlayViewTween()` | Runs from `Start`; use the template's tween pattern and complete with `OnViewOpen()`. |
| Animation completion | `OnViewOpen()` | Runs after the item open tween, or immediately in the default implementation. |
| Apply/reapply data | `Refresh()` | Call explicitly when a list/grid/tab item receives new data. |
| Each activation | `OnEnable()` | Reset visibility-dependent state and make idempotent for pooling. |
| Each deactivation | `OnDisable()` | Remove custom listeners and retain the template's tracker cleanup. |
| Control behavior | `ButtonClick(...)` / `ValueChange<T>(...)` | Consume generated listeners; do not duplicate UnityEvent subscriptions. |

BaseItem has no `OnAdded`, `OnBeforeRemove`, `OnRemoved`, or UIManager close callback. If an AI proposes one of those hooks for an item, redirect the logic to `Refresh`, `OnEnable`, or `OnDisable`.

## Events, assets, timers, and cleanup

Prefer the wrappers supplied by `BaseView`/`BaseItem`:

- `AddEventListener` / `RemoveEventListener` / `DispatchEvent` for the per-view `EventDispatcher`.
- `LoadAsset`, `LoadAssetAsync`, `LoadAllAsset*`, `LoadSubAsset*`, `LoadDirAsset*`, `UnloadAsset`, and `ClearAssetLoadTracker` for tracked resources.
- `AddTimer`, `AddTimerFrame`, `RemoveTimer`, and `ClearTimerTracker` for tracked timers.

For `BaseView`, the internal `EventDispatcher` and `TimerTracker` are cleared by `BeforeRemove`; tracked assets are released by `Removed`. Do not recreate those trackers in `OnBeforeRemove` just to remove listeners. For `BaseItem`, preserve the template's `OnDisable` cleanup because it has no `BeforeRemove`/`Removed` call.

Direct subscriptions to `FF8.Message`, `FF8.Input`, UnityEvents, third-party controls, or static callbacks are outside those automatic hooks. Store the delegate and explicitly unsubscribe in the matching teardown hook (`OnBeforeRemove` for a view, `OnDisable` for an item). This is especially important when `isDestroy` is false and the object is cached.

Distinguish custom listeners from generated control wiring: `OnAddUIComponentListener` runs once in `Awake`, and its generated `ButtonClick`/`ValueChange` listeners are meant to stay attached while a cached view is hidden and reopened. Do not remove those generated listeners in `OnBeforeRemove` unless the script also has an explicit, idempotent rebind path. If custom code adds `onValueChanged` or another UnityEvent listener in `OnAdded`, remove the stored delegate before adding it again or move the subscription to a one-time hook.

Choose the close policy deliberately:

- `isDestroy = false`: hide/detach and cache the instance; expect `OnAdded` on the next open and preserved serialized/runtime state unless `Refresh` resets it.
- `isDestroy = true`: destroy the instance and release the prefab with full unload semantics.
- `FF8.UI.Clear(true)` defaults to destroying all layers.
- `FF8.UI.Clear(layerType)` defaults to `isDestroy = false`; pass the flag explicitly when memory behavior matters.

The current `UIManager.Clear(bool)` implementation also clears `Notify`; the older README wording that says “except Notify” is stale. Verify behavior against `UIManager.cs` when documentation and source disagree.

## Public API quick reference

UIID-based open, query, notify, and close APIs accept either an enum key or an `int` key. Missing configuration logs an error and returns `null`/`false` as appropriate; fix the config instead of masking the error.

```csharp
// Synchronous open. The return value is a guid; null means config failure.
string guid = FF8.UI.Open(UIID.Main, new object[] { playerId }, new UICallbacks(
    (parameters, id) => { /* external on-added */ },
    (parameters, id) => { /* external on-removed */ },
    () => { /* external before-remove */ }));

// Async: all three forms are supported.
UILoader loader = FF8.UI.OpenAsync(UIID.Main, new object[] { playerId });
loader.SetOnCompleted(() =>
{
    // LoaderSuccess means the request ended; verify the view separately.
    bool active = FF8.UI.GetByGuid(loader.Guid) != null;
});
// yield return loader;   // coroutine
// await loader;          // async/await (including WebGL, no worker thread)

// Notify content is passed to BaseView as Args[0].
string notifyGuid = FF8.UI.ShowNotify(UIID.Notify, "Saved");
UILoader notifyLoader = FF8.UI.ShowNotifyAsync(UIID.Notify, "Saved");

bool active = FF8.UI.Has(UIID.Main);
List<GameObject> views = FF8.UI.GetByUIid(UIID.Main); // null when none is active
GameObject view = FF8.UI.GetByGuid(guid);
FF8.UI.Close(UIID.Main, isDestroy: false);
FF8.UI.Close(UIID.Notify, isDestroy: true, guid: notifyGuid);
FF8.UI.Clear(isDestroy: true);
FF8.UI.Clear(LayerType.UI, isDestroy: false);
```

Use this end-to-end pattern when a panel must be ready before showing a toast. It deliberately verifies both loaders and closes the Notify instance by guid; Notify has no built-in auto-dismiss timer.

```csharp
IEnumerator OpenPanelThenNotify()
{
    UILoader panelLoader = FF8.UI.OpenAsync(UIID.Main);
    if (panelLoader == null) yield break;
    yield return panelLoader;
    if (FF8.UI.GetByGuid(panelLoader.Guid) == null) yield break;

    UILoader toastLoader = FF8.UI.ShowNotifyAsync(UIID.Notify, "Saved");
    if (toastLoader == null) yield break;
    yield return toastLoader;
    if (FF8.UI.GetByGuid(toastLoader.Guid) == null) yield break;

    yield return new WaitForSeconds(2f);
    FF8.UI.Close(UIID.Notify, true, toastLoader.Guid);
}
```

For a cached panel, refresh its state in `OnAdded` and reset its open visual state in `OnViewTweenInit`; do not assume the async loader or `OnStart` will perform either job.

`UILoader.LoaderSuccess`/`SetOnCompleted` means that the load request has finished, including a missing prefab, cancellation, or a queued request being released. It does not prove that a `GameObject` was instantiated. After an async completion, check `GetByGuid(loader.Guid)`, `Has`, or the relevant callback state. A null loader itself means the UIID was not configured.

`GetByUIid` can return multiple objects for `Notify`; `GetByGuid` identifies one active instance. Hidden cached objects are not returned by active-view queries.

## Canvas and CanvasScaler setup

Call these methods after `FF8.UI.Initialize` and before opening views. Passing `layer: null` applies the change to all six layers; passing a `LayerType` changes one layer.

```csharp
FF8.UI.SetCanvas(
    layer: null,
    sortOrder: 1,
    sortingLayerName: "Default",
    renderMode: RenderMode.ScreenSpaceCamera,
    pixelPerfect: false,
    camera: Camera.main);

FF8.UI.SetCanvasScaler(
    LayerType.UI,
    CanvasScaler.ScaleMode.ScaleWithScreenSize,
    referenceResolution: new Vector2(1920, 1080),
    screenMatchMode: CanvasScaler.ScreenMatchMode.MatchWidthOrHeight,
    matchWidthOrHeight: 0f,
    referencePixelsPerUnit: 100f);
```

There are three scaler overloads: Constant Pixel Size, Scale With Screen Size, and Constant Physical Size. The Scale With Screen Size overload uses `800 x 600` when no reference resolution is supplied. A camera is required for `ScreenSpaceCamera`; a wrong render mode or sort order can look like a loading failure. `SetCanvas(null, sortOrder, ...)` assigns the same sort order to every layer, so configure layers individually when the six default ordering values must remain distinct.

## Editor utilities and common UI components

Use `f8-editor-ui-workflow` for detailed editor operations. The current UI editor area provides:

- BaseView/BaseItem template creation.
- `（图片自动切割九宫格）`, atlas slicing, and `（图片尺寸设为4的倍数）` tools.
- `（收集UI所有的中文放入本地化表，并添加组件）` for localization collection.
- Runtime components such as `SimpleRoundedImage`, `SafeAreaAdapter`, `UIParticleSystem`, `UIRedDot`, and `SpriteSequenceFrame`.
- Built-in examples for nested layout, infinite scrolling, drag/drop, and tab control under `Assets/F8Framework/Tests/UI/Example`.

Do not mix a utility component's own lifecycle with `BaseView` teardown without reading its source. Route localization, AssetManager, and Tween-specific behavior to their companion skills.

## Debugging decision table

| Symptom | Check in this order |
| --- | --- |
| `Open` returns `null` | Confirm `UIManager.Initialize(configs)` ran, the UIID exists, and the dictionary key maps to the intended asset key. |
| No object after a successful async wait | Inspect `loader.Guid`, `GetByGuid`, and AssetManager logs; completion also covers missing/cancelled loads. |
| Prefab cannot load | Press F8, verify Resources/AssetBundle placement and AssetManager mode, and use the exact indexed key. |
| View is behind another UI | Confirm `LayerType`, default sort order, `SetCanvas`, sorting layer, render mode, and camera. |
| Normal UI opens twice or returns an empty guid | This is duplicate protection for the same active/loading prefab; query or close the existing view first. |
| Close appears ineffective during async loading | `LayerUI.Close` currently leaves a pending first load alone; use `Clear(...)` to cancel it or handle the loader completion explicitly. |
| Reopen during a close tween returns the old view/loader | `Closing` is still a duplicate state; wait for removal completion or make the transition logic explicitly coordinate close-then-open. |
| Dialog appears out of order | Confirm the same `uiId` is being queued and allow the one-frame delay after the previous close. |
| Wrong Notify instance closes | Pass the returned `Guid` to `Close`; do not identify a Notify only by prefab path. |
| Auto-bound field is null | Check exact same-object component, case-sensitive name segment, marker pair, generated script compile, and rerun the binding button twice. |
| Binding misses a nested item | The scan stops below a `BaseItem`; attach/bind the item script independently. |
| Generated listener does not fire | Confirm the control type is supported, the generated `OnAddUIComponentListener` remains intact, and the object reached `Awake`. |
| Listener/timer/resource leak on reopen | Use BaseView/BaseItem tracker wrappers and manually remove direct global/UnityEvent subscriptions in teardown. |
| `OnStart` runs only once | This is expected for a cached instance; put per-open refresh in `OnAdded`, `Refresh`, or `OnEnable`. |
| `OnViewOpen` fires twice or never | With a custom sequence, do not call it manually; without a sequence, call it explicitly or call the base implementation. |
| Close waits unexpectedly | Inspect `OnPlayViewCloseTween`; a non-empty close sequence intentionally delays removal. |
| UI input does nothing | Confirm `EventSystem.current`, the generated input module, active Canvas raycaster, and blocked raycast targets. |

## Validation and handoff checklist

Before declaring a UI task complete, perform as many checks as the environment allows:

- Confirm bootstrap order, AssetManager availability, Tween availability when animations are used, and exactly one `UIManager.Initialize` for the live manager.
- Confirm the UIID/config/asset key and selected layer are documented.
- Confirm the prefab root owns the correct `BaseView` or `BaseItem` script and compiles.
- Confirm the binding mode, naming convention, generated fields, listener override, and `SetComponents` assignments.
- Confirm both marker variants remain supported: `// Auto-generated component bindings below` and `// 自动获取组件（自动生成，不能删除）`.
- Exercise sync open, async open, callback/await/coroutine completion, query, close, cached reopen, and destroy/recreate as applicable.
- Exercise Dialog queueing and Notify guid-based close when those layers are used.
- Exercise Canvas/CanvasScaler at the target resolution and input path.
- Check the Unity Console for missing config, asset, component, EventSystem, or duplicate-load errors.
- Run the skill validator when editing this skill itself:

```powershell
python -X utf8 C:\Users\41269\.codex\skills\.system\skill-creator\scripts\quick_validate.py `
  G:\Work\unity2021315f1pj\F8Framework\Assets\F8Framework\Tests\AISkills\f8framework-skills\features\f8-features-ui-workflow
```

In the final handoff, list changed files, the chosen layer and asset key, lifecycle/cleanup decisions, binding regeneration status, validation commands/results, and any Unity-only checks that could not be run outside the Editor.
