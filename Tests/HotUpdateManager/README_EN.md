# F8 HotUpdate

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 HotUpdate**  
Version Management System for Hot Updates
* Asset Bundling - Package game resources efficiently
* Subpackage Management - Split content into downloadable chunks
* Hot Update Resources - Deploy patches without requiring full app updates

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Editor Interface Usage

* How to Set Up Subpackage Resources  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/HotUpdateManager/ui_20240323173756.png)
--------------------------
* Configuration Options:
  * Select build platform
  * Set output path
  * Specify version number
  * Configure remote asset loading URL
  * Enable hot updates
  * Full build / Subpackage build / Empty build  

![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/HotUpdateManager/ui_20240317214323_2.png)  

--------------------------
* For Local Hot Update Testing: Clear temporary files in the sandbox directory before testing.
--------------------------
### If Build Fails: Try running Unity's native Build once before retrying.

--------------------------
* After Building: Upload files to CDN server  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/HotUpdateManager/ui_20240323173827_2.png)
--------------------------
### Code Examples
```C#
IEnumerator Start()
{
    // Initialize local version
    FF8.HotUpdate.InitLocalVersion();

    // Initialize remote version 
    yield return FF8.HotUpdate.InitRemoteVersion();
    
    // Initialize asset version
    yield return FF8.HotUpdate.InitAssetVersion();
    
    // Check resources needing hot updates (returns file list + total size)
    Tuple<Dictionary<string, string>, long> result = FF8.HotUpdate.CheckHotUpdate();
    var hotUpdateAssetUrl = result.Item1;
    var allSize = result.Item2;
    
    // Execute hot update
    FF8.HotUpdate.StartHotUpdate(hotUpdateAssetUrl, () =>
    {
        LogF8.Log("Completed");
    }, () =>
    {
        LogF8.Log("Failed"); 
    }, progress =>
    {
        LogF8.Log("Progress: " + progress);
    });

    // Check unloaded subpackages  
    List<string> subPackage = FF8.HotUpdate.CheckPackageUpdate(GameConfig.LocalGameVersion.SubPackage);
    
    // Load subpackages
    FF8.HotUpdate.StartPackageUpdate(subPackage, () =>
    {
        LogF8.Log("Completed");
    }, () =>
    {
        LogF8.Log("Failed");
    }, progress =>
    {
        LogF8.Log("Progress: " + progress);
    });
}
```