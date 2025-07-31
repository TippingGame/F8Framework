# F8 AssetManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 AssetManager Component**
1. Editor Mode:
   * Press F8 to auto-generate asset indices/AB names
   * Auto-detect platform differences
   * Clean redundant ABs and folders
   * Reduce development cycles in Editor mode
2. Runtime:
   * Sync/async loading of individual assets
   * Load all assets in a folder or shared AB
   * Auto-detect Resources/AssetBundle assets
   * Load remote assets
   * Get loading progress
   * Interrupt async loading synchronously
3. AssetBundle Loading Methods:
   * Single asset per AB
   * Folder-based AB (first-level only)
   * Custom AB grouping (multiple assets under same AB name)
4. Important: AB asset paths are case-insensitive

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Initialization

1. Press F8 to:
   * Auto-scan all assets in Resources
   * Auto-scan all assets in Assets/AssetBundles
   * Generate index files in Assets/F8Framework/AssetMap  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205225637.png)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205230012_2.png)
---------------------------------
2. Files in Assets/AssetBundles:
   * Auto-assign AB names (Note: Existing AB names won't be overwritten. Clear AB names manually if needed)
   * Built ABs will generate in StreamingAssets/AssetBundles/Windows (Note: Clear this directory manually if no ABs are loaded)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205225815.png)
---------------------------------
3. **Important**: Loading cross-platform ABs (Android/iOS/WebGL) in Editor may cause:
   * Purple shaders
   * Scene loading failures
   * Audio loading failures
   * **Solution**: Enable Editor Mode
---------------------------------
4. **Important**: How to Enable Editor Mode:
   * Enable in code: FF8.Asset.IsEditorMode = true;
   * Or toggle in Editor:  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20251736474182.png)
---------------------------------
5. **Important**: WebGL doesn't support synchronous AB loading. Use Resources for sync loading instead.
---------------------------------
6. If no errors occur, you're ready to go!

### Code Examples
```C#
IEnumerator Start()
{
    /*----------All loading methods auto-detect Resources/AssetBundle assets----------*/
    
    // [Important] Editor mode - no need to press F8 after every asset modification
    FF8.Asset.IsEditorMode = true;
    
    
    /*-------------------------------------Sync Loading-------------------------------------*/
    // Load single asset
    GameObject go = FF8.Asset.Load<GameObject>("Cube");
    // 如要使用完整路径加载，需点击F5打包工具勾选对应功能
    go = FF8.Asset.Load<GameObject>("AssetBundles/Prefabs/Cube");
    go = FF8.Asset.Load<GameObject>("Resources/Prefabs/Cube.prefab");

    // assetName: Asset name
    // subAssetName: Sub-asset name (for Sprite in Multiple mode)
    // [Warning] REMOTE_ASSET_BUNDLE requires AssetRemoteAddress = "http://127.0.0.1:6789/remote"
    Sprite sprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
    
    // Load all assets
    FF8.Asset.LoadAll("Cube");
    // Load all sub-assets
    FF8.Asset.LoadSub("Cube");
    
    
    /*-------------------------------------Async Loading-------------------------------------*/
    FF8.Asset.LoadAsync<GameObject>("Cube", (go) =>
    {
        GameObject goo = Instantiate(go);
    });

    // [Note] async/await (no multithreading, works on WebGL)
    // await FF8.Asset.LoadAsync<GameObject>("Cube");
    // or
    // BaseLoader load = FF8.Asset.LoadAsync<GameObject>("Cube");
    // await load;
    
    // Coroutine method
    yield return FF8.Asset.LoadAsync<GameObject>("Cube");
    // or
    BaseLoader load2 = FF8.Asset.LoadAsync<GameObject>("Cube");
    yield return load2;
    GameObject go2 = load2.GetAssetObject<GameObject>();
    
    // Load all assets
    BaseLoader loaderAll = FF8.Asset.LoadAllAsync("Cube");
    yield return loaderAll;
    Dictionary<string, Object> allAsset = loaderAll.GetAllAssetObject();
    
    // Load all sub-assets
    BaseLoader loaderSub = FF8.Asset.LoadSubAsync("Atlas");
    yield return loaderSub;
    Dictionary<string, Sprite> allAsset2 = loaderSub.GetAllAssetObject<Sprite>();
    
    
    /*-------------------------------------Folder Loading-------------------------------------*/
    // [Note] Only loads first-level assets (non-recursive)
    FF8.Asset.LoadDir("NewFolder");
    
    // async/await (no multithreading, works on WebGL)
    // BaseDirLoader loadDir = FF8.Asset.LoadDirAsync("NewFolder", () => { });
    // await loadDir;
    
    // Async folder loading
    BaseDirLoader loadDir2 = FF8.Asset.LoadDirAsync("NewFolder", () => { });
    yield return loadDir2;
    
    // Access all BaseLoaders
    List<BaseLoader> loaders = loadDir2.Loaders;
    
    // Progress tracking
    foreach (var item in FF8.Asset.LoadDirAsyncCoroutine("NewFolder"))
    {
        yield return item;
    }

    // Alternative method
    var loadDir3 = FF8.Asset.LoadDirAsyncCoroutine("NewFolder").GetEnumerator();
    while (loadDir3.MoveNext())
    {
        yield return loadDir3.Current;
    }
    
    
    /*-------------------------------------Utilities-------------------------------------*/
    // Get all assets
    Dictionary<string, Object> allAsset3 = FF8.Asset.GetAllAssetObject("Cube");
    
    // Get specific type only
    Dictionary<string, Sprite> allAsset4 = FF8.Asset.GetAllAssetObject<Sprite>("Atlas");
    
    // Get single asset
    GameObject go3 = FF8.Asset.GetAssetObject<GameObject>("Cube");
    
    // Get loading progress
    float loadProgress = FF8.Asset.GetLoadProgress("Cube");

    // Get total progress
    float loadProgress2 = FF8.Asset.GetLoadProgress();

    // [Important] Sync unload
    // Set true to completely unload AB
    FF8.Asset.Unload("Cube", false); 

    // Async unload
    FF8.Asset.UnloadAsync("Cube", false, () =>
    {
        // Callback when unload completes
    });
    
    
    /*-------------------------------------Examples-------------------------------------*/
    // [Warning] Must load skybox material or will turn purple
    // [Limitation] Cannot load scenes from Resources directory
    FF8.Asset.Load("Scene");
    SceneManager.LoadScene("Scene");
    
    // [Prerequisite] Must load atlas first
    FF8.Asset.Load("SpriteAtlas");
    
    // [Optimization] Skip preload if atlas/images share same AB name
    FF8.Asset.LoadAsync<Sprite>("PackForest_2", sprite =>
    {
        LogF8.Log(sprite);
    });
    
    // [Critical] Texture2D/Sprite conflict: 
    // If loaded as Texture2D first, cannot load as Sprite later
    FF8.Asset.Load<Texture2D>("PackForest_2");
}
```

### Editor Features
#### [Multi-Process AB Building](https://docs.unity3d.com/6000.1/Documentation/Manual/Build-MultiProcess.html)
Requires Unity 6000+  
Project Settings → Editor → Build Pipeline → Enable "Multi-Process AssetBundle Building"
#### Asset Inspector
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20250523001_2.png)
#### Important: Clear AB names manually when moving files out of AssetBundles directory
1. Editor Tools:
* 1. Find asset references (full project scan)
* 2. Clear AB names (supports multi-select)
* 3. Set custom AB names (supports multi-select)
* 4. Global missing reference detector

![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240216212631_2.png)  

#### Quick Access: Hover over files/folders and press Spacebar to open in system explorer
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20241112212631.png)  
