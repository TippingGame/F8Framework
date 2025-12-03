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
   * Encrypt AssetBundle
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
6. **Important**: If you need to use resources with the same name, you can enable full resource path loading. Click F5 in the build tool and check the corresponding feature.  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_1761979618779.png)
7. If no errors occur, you're ready to go!

### Code Examples
```C#
IEnumerator Start()
{
    /*========== Basic Configuration ==========*/
    // Enable editor mode (no need to press F8 for every resource change), can also be toggled in the menu bar
    FF8.Asset.IsEditorMode = true;
    

    /*========== 1. Synchronous Loading ==========*/

    // Basic loading - automatically recognizes Resources or AssetBundle
    GameObject cube = FF8.Asset.Load<GameObject>("Cube");

    // Full path loading (requires enabling corresponding feature in F5 packaging tool)
    GameObject prefab1 = FF8.Asset.Load<GameObject>("AssetBundles/Prefabs/Cube");
    GameObject prefab2 = FF8.Asset.Load<GameObject>("Resources/Prefabs/Cube.prefab");

    // Load sub-assets (e.g., Sprite images using Multiple mode)
    Sprite sprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12");

    // Force remote loading mode, requires configuring remote resource address in F5 packaging tool
    Sprite remoteSprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12",
        AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);


    /*========== 2. Asynchronous Loading ==========*/

    // Callback approach
    FF8.Asset.LoadAsync<GameObject>("Cube", (go) => { Instantiate(go); });

    // Coroutine approach
    yield return FF8.Asset.LoadAsync<GameObject>("Cube");

    // Or get loader for control
    BaseLoader loader = FF8.Asset.LoadAsync<GameObject>("Cube");
    yield return loader;
    GameObject result = loader.GetAssetObject<GameObject>();

    // async/await approach (WebGL compatible)
    await FF8.Asset.LoadAsync<GameObject>("Cube");


    /*========== 3. Batch Loading ==========*/

    // Synchronous folder loading
    FF8.Asset.LoadDir("UI/Prefabs");

    // Asynchronous folder loading - callback approach
    FF8.Asset.LoadDirAsync("UI/Prefabs", () => { LogF8.Log("All UI resources loaded"); });

    // Asynchronous folder loading - coroutine approach
    BaseDirLoader dirLoader = FF8.Asset.LoadDirAsync("UI/Prefabs");
    yield return dirLoader;

    // Iterate loading progress
    foreach (var progress in FF8.Asset.LoadDirAsyncCoroutine("UI/Prefabs"))
    {
        LogF8.Log($"Loading progress: {progress}");
        yield return progress;
    }

    // async/await approach (WebGL compatible)
    await FF8.Asset.LoadDirAsync("UI/Prefabs");
    
    // Load all assets of this resource
    FF8.Asset.LoadAll("Cube");
    BaseLoader loaderAll = FF8.Asset.LoadAllAsync("Cube");
    
    // Load all sub-assets of this resource
    FF8.Asset.LoadSub("Atlas");
    BaseLoader loaderSub = FF8.Asset.LoadSubAsync("Atlas");

    
    /*========== 4. Scene Loading ==========*/

    // Synchronous scene loading
    FF8.Asset.LoadScene("MainScene");

    // Asynchronous scene loading
    SceneLoader sceneLoader = FF8.Asset.LoadSceneAsync("MainScene", LoadSceneMode.Single);
    yield return sceneLoader;

    // Manual scene activation control
    SceneLoader sceneLoader2 = FF8.Asset.LoadSceneAsync("MainScene", new LoadSceneParameters(LoadSceneMode.Single),
        allowSceneActivation: false);
    yield return new WaitForSeconds(2);
    sceneLoader2.AllowSceneActivation();


    /*========== 5. Asset Management ==========*/

    // Get loaded asset
    GameObject cachedCube = FF8.Asset.GetAssetObject<GameObject>("Cube");

    // Get all sub-assets
    Dictionary<string, Object> allAssets = FF8.Asset.GetAllAssetObject("Atlas");
    Dictionary<string, Sprite> allSprites = FF8.Asset.GetAllAssetObject<Sprite>("Atlas");

    // Get loading progress
    float assetProgress = FF8.Asset.GetLoadProgress("Cube"); // Single asset
    float totalProgress = FF8.Asset.GetLoadProgress(); // All assets

    // Asset unloading
    FF8.Asset.Unload("Cube", false); // Keep dependencies
    FF8.Asset.Unload("Cube", true); // Complete unload

    // Asynchronous unloading
    FF8.Asset.UnloadAsync("Cube", false, () => { LogF8.Log("Asset unloaded"); });
    
    
    /*========== 6. Note: Common Issues ==========*/
    
    // 1. When loading Asset Bundles for different platforms (Android, iOS, WebGL) in the Editor, 
    //    Shaders may turn magenta, Scenes may fail to load, audio may fail to load, etc.
    //    (Solution: Enable Editor Mode)
    
    // 2. When loading a Scene, remember to load the Skybox material; otherwise, it may turn magenta.
    //    Also, Scenes cannot be loaded from the Resources directory (they must be manually added to Build Settings).
    
    // 3. When using images from a Sprite Atlas, you need to load the Sprite Atlas.
    //    If the Sprite Atlas and the images are set to the same Asset Bundle name, the Sprite Atlas does not need to be loaded separately.
    FF8.Asset.Load("SpriteAtlas");
    
    // 4. Sprite Atlases can also be loaded using SpriteAtlasManager callback listeners.
    SpriteAtlasManager.atlasRequested += (tag, callback) =>
    {
        FF8.Asset.LoadAsync<SpriteAtlas>(tag, (atlas) =>
        {
            callback(atlas);
        });
    };
    
    // 5. Be careful to distinguish between Texture2D and Sprite when loading images.
    //    If a resource has been loaded as a Texture2D, it cannot be loaded as a Sprite type afterward.
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
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240216212631_2.png)
* 1. Find asset references (full project scan)
     ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_1761979131320.png)
* 2. Clear AB names (supports multi-select)
* 3. Set custom AB names (supports multi-select)
* 4. Global missing reference detector
     ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_1761979410848.png)

![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240216212631_2.png)  

#### Quick Access: Hover over files/folders and press Spacebar to open in system explorer
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20241112212631.png)  
