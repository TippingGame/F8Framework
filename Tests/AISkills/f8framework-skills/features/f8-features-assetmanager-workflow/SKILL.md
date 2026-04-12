---
name: f8-features-assetmanager-workflow
description: Use when implementing or troubleshooting AssetManager feature workflows — asset loading, AB mapping, runtime resource retrieval, and scene loading in F8Framework.
---

# AssetManager Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.Asset = ModuleCenter.CreateModule<AssetManager>();`.


## Use this skill when

- The task is about asset loading (sync/async), AB mapping, and runtime resource retrieval.
- The user asks about Resources vs AssetBundle loading strategies.
- The user needs to load scenes, sprites, prefabs, or remote assets at runtime.
- Troubleshooting asset loading failures, purple shaders, or missing assets.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. For usage docs, read: Assets/F8Framework/Tests/AssetManager/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/AssetManager
- Editor module: Assets/F8Framework/Editor/AssetManager
- Asset index files: Assets/F8Framework/AssetMap
- Test docs: Assets/F8Framework/Tests/AssetManager

## Key classes and interfaces

| Class | Role |
|-------|------|
| `AssetManager` | Core module. Access via `FF8.Asset`. Handles all loading/unloading. |
| `AssetBundleManager` | Manages AB manifest, AB loading/caching, and dependencies. |
| `BaseLoader` | Async load handle. Supports yield / await / callback patterns. |
| `BaseDirLoader` | Async folder load handle. |
| `SceneLoader` | Async scene load handle with `AllowSceneActivation()` support. |

## API quick reference

### Configuration
```csharp
FF8.Asset.IsEditorMode = true;  // Skip AB, load from AssetDatabase (Editor only)
```

### Sync loading
```csharp
GameObject cube = FF8.Asset.Load<GameObject>("Cube");
// Full path (needs F5 setting enabled)
GameObject prefab = FF8.Asset.Load<GameObject>("AssetBundles/Prefabs/Cube");
// Sub-asset (e.g., Multiple-mode Sprite)
Sprite sp = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12");
// Force remote
Sprite remote = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12",
    AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
```

### Async loading
```csharp
// Callback
FF8.Asset.LoadAsync<GameObject>("Cube", (go) => { Instantiate(go); });
// Coroutine
yield return FF8.Asset.LoadAsync<GameObject>("Cube");
// async/await (WebGL compatible)
await FF8.Asset.LoadAsync<GameObject>("Cube");
// Loader handle
BaseLoader loader = FF8.Asset.LoadAsync<GameObject>("Cube");
yield return loader;
GameObject result = loader.GetAssetObject<GameObject>();
```

### Batch loading
```csharp
FF8.Asset.LoadDir("UI/Prefabs");                         // Sync folder
FF8.Asset.LoadDirAsync("UI/Prefabs", () => { });          // Async folder
BaseDirLoader dirLoader = FF8.Asset.LoadDirAsync("UI/Prefabs"); // Loader handle
// Interate progress
foreach (var progress in FF8.Asset.LoadDirAsyncCoroutine("UI/Prefabs"))
{
    yield return progress;
}
FF8.Asset.LoadAll("Cube");                                // All assets in same AB
FF8.Asset.LoadSub("Atlas");                               // All sub-assets
```

### Scene loading
```csharp
FF8.Asset.LoadScene("MainScene");                         // Sync
SceneLoader sl = FF8.Asset.LoadSceneAsync("MainScene");   // Async
// Manual activation
SceneLoader sl2 = FF8.Asset.LoadSceneAsync("MainScene",
    new LoadSceneParameters(LoadSceneMode.Single), allowSceneActivation: false);
sl2.AllowSceneActivation();
```

### Resource management
```csharp
float p = FF8.Asset.GetLoadProgress("Cube");   // Single asset progress
float t = FF8.Asset.GetLoadProgress();          // Total progress
GameObject cachedCube = FF8.Asset.GetAssetObject<GameObject>("Cube"); // Get cached
FF8.Asset.Unload("Cube", false);               // Keep dependencies
FF8.Asset.Unload("Cube", true);                // Full unload
FF8.Asset.UnloadAsync("Cube", false, () => {}); // Async unload
FF8.Asset.UnloadScene("Scene");
FF8.Asset.UnloadUnused(true);
FF8.Asset.UnloadSceneAsync("Scene");
FF8.Asset.UnloadUnusedAsync(true);
```

## Workflow

1. Ensure F8 has been pressed at least once to generate asset index and AB names.
2. Configure Editor mode if in development: `FF8.Asset.IsEditorMode = true`.
3. Choose loading strategy: sync for gameplay-critical, async for large resources.
4. For SpriteAtlas: load atlas first, or set atlas and sprites to same AB name.
5. For remote assets: configure remote address in F5 build tool first.
6. Track loading progress via `GetLoadProgress()` for loading screens.
7. Unload unused assets to manage memory.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Purple shaders on Android/iOS AB in Editor | Loading cross-platform AB | Enable Editor mode: `FF8.Asset.IsEditorMode = true` |
| Scene load fails from AB | Cross-platform AB issue | Enable Editor mode or build AB for current platform |
| Sprite loads as null but Texture2D works | Asset was first loaded as Texture2D | Load as correct type first, or use `Load<Sprite>` |
| WebGL sync AB load fails | WebGL cannot sync-load AB | Use Resources for sync or switch to async AB loading |
| Same-name asset conflict | Two assets share the same name | Enable full-path loading in F5 build tool |
| AB not found after moving files | Stale AB names on moved files | Manually clear AB names on moved files |
| Skybox purple after scene load | Missing skybox material in build | Include skybox material in a loadable directory |
| Resources scene load fails | Scene in Resources folder | Move scene out of Resources or use Build Settings |

## Cross-module dependencies

- **ExcelTool**: Config binary/json files are loaded via AssetManager.
- **HotUpdateManager**: Uses AssetManager for hot update asset downloads.
- **Audio**: Audio clips loaded through AssetManager.
- **UI**: UI prefabs loaded through AssetManager.
- **Localization**: Localization resources loaded through AssetManager.

## Output checklist

- Effective loading method selected (sync/async/batch).
- Editor mode configuration documented.
- Files changed and why.
- Validation status and remaining risks.
