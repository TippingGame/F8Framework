# F8 SDKManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 SDKManager Component**  
Cross-platform SDK integration system that provides unified interfaces for native platform interactions across multiple channels.

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
    // Launch SDK, platform ID, channel ID
    FF8.SDK.SDKStart("1", "1");
    
    // Login
    FF8.SDK.SDKLogin();
    
    // Logout
    FF8.SDK.SDKLogout();
    
    // Switch accounts
    FF8.SDK.SDKSwitchAccount();
    
    // Load video advertisement
    FF8.SDK.SDKLoadVideoAd("1", "1");
    
    // Play video advertisements
    FF8.SDK.SDKShowVideoAd("1", "1");
    
    // payment
    FF8.SDK.SDKPay("serverNum", "serverName", "playerId", "playerName", "amount", "extra", "orderId",
        "productName", "productContent", "playerLevel", "sign", "guid");
    
    // Update user information
    FF8.SDK.SDKUpdateRole("scenes", "serverId", "serverName", "roleId", "roleName", "roleLeve", "roleCTime", "rolePower", "guid");
    
    // SDK exits the game
    FF8.SDK.SDKExitGame();
    
    // Native Tips
    FF8.SDK.SDKToast("Native Toast");
```

## Android Project Usage Guide
### If you need to integrate privacy policies or interact with Android SDKs, follow the manual instructions below

* (Note: First determine which Gradle version your Unity uses) [Unity Documentation on Gradle Versions](https://docs.unity3d.com/2021.3/Documentation/Manual/android-gradle-overview.html)
* The framework only supports the following versions. You can also check Unity's Gradle version in the installation directory（`C:\Program Files\Unity\Hub\Editor\2021.3.15f1\Editor\Data\PlaybackEngines\AndroidPlayer\Tools\gradle\lib`）
* Unity 2021 versions（2021.2 / 2021.1 starting from 2021.1.16f1：Gradle6.1.1）  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241120134318.png)
* Unity 2022 versions（2022.1：Gradle6.1.1）  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241120134325.png)
* Unity 2023 versions（2023.1：Gradle7.6）  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241121004145.png)
* Unity 6000 versions（Suspected typo in 8.7.2; currently, 8.4.2 is the highest supported version）  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241120134329_2.png)
------------------------------
* Based on your version, select the correct directory and copy the following two files to your project's `Assets/Plugins/Android` folder.
* Add the `.xml` extension to the file [AndroidManifest.xml](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/Gradle6.1.1/AndroidManifest)
* Add the `.aar` extension to the file [UnityAndroidDemo-release.aar](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/Gradle6.1.1/UnityAndroidDemo-release)   
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241120213148.png)  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241120213210.png)
------------------------------
* Note: Each Unity version has slight differences. After switching Unity versions, manually delete these two files.
------------------------------
* (Optional) Build an AAR using an Android project: 
* Download [UnityAndroidDemo2021.3.15f1.zip](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/UnityAndroidDemo2021.3.15f1.zip) to export [UnityAndroidDemo-release.aar](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/Gradle6.1.1/UnityAndroidDemo-release) and [AndroidManifest.xml](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/Gradle6.1.1/AndroidManifest)
    1. Download [Android Studio](https://developer.android.google.cn/studio/archive/) (change the language to English in the top-right corner).
    2. Select the version: 
       * Unity 2022 / 2023: `android-studio-2022.2.1.20-windows`
       * Unity 2023 / 6000: `2023.3.1.20-windows`
    3. After unzipping and opening the project, go to **Settings → Build → Build Tools → Gradle → Gradle JDK** and set the JDK to the one included with Unity (`C:\Program Files\Unity\Hub\Editor\6000.1.5f1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK`)
    4. In **Project Structure → Project / SDK Location**, modify the Gradle version and set the SDK to Unity's built-in SDK (`C:\Program Files\Unity\Hub\Editor\6000.1.5f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK`).
    5. From the top menu, select **Build → Rebuild Project**. After exporting, an AAR file will be generated.
    6. Use a ZIP-capable compression tool to delete the `libs/classes.jar` file inside the AAR.
    7. Open the root directory's `classes.jar` and delete the `UnityPlayerActivity.java` file (not required for Unity 2023 / 6000).
------------------------------

* Prebuilt Android projects for four Unity versions:
    1. Unity 2021.3.15f1: [UnityAndroidDemo2021.3.15f1.zip](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/UnityAndroidDemo2021.3.15f1.zip)
    2. Unity 2022.3.52f1: [UnityAndroidDemo2022.3.52f1.zip](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2022/UnityAndroidDemo2022.3.52f1.zip)
    3. Unity 2023.2.20f1 (deprecated by Unity): [UnityAndroidDemo2023.2.20f1.zip](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2023/UnityAndroidDemo2023.2.20f1.zip)
    4. Unity 6000.0.24f1: [UnityAndroidDemo6000.0.24f1.zip](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ6000/UnityAndroidDemo6000.0.24f1.zip)
------------------------------
* After a successful build, you will see this interface:  
  1. If you don’t want to display this interface,  
  2. Open [AndroidManifest.xml](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/Gradle6.1.1/AndroidManifest) and swap `MoeNativeActivity` with `MainActivity`.  
  3. Also modify the `AndroidManifest.xml` inside [UnityAndroidDemo-release.aar](https://github.com/TippingGame/F8Framework/blob/main/Runtime/SDKManager/Plugins_Android/AndroidPJ2021/Gradle6.1.1/UnityAndroidDemo-release)  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241119233017.png)
---

## iOS Project Usage Guide
* Modify these two files for SDK integration: [F8SDKInterfaceUnity.h](https://github.com/TippingGame/F8Framework/blob/main/Plugins/iOS/SDKManager/F8SDKInterfaceUnity.h) and [F8SDKInterfaceUnity.mm](https://github.com/TippingGame/F8Framework/blob/main/Plugins/iOS/SDKManager/F8SDKInterfaceUnity.mm)

---
## WebGL Games
* Note: WebGL cannot use synchronous AssetBundle (AB) loading but can load Resources synchronously.
---
## WeChat Mini Game Integration
* Review the usage guide for the [WebGL to WeChat Mini Game Plugin](https://github.com/wechat-miniprogram/minigame-unity-webgl-transform) and download the[ UnityPackage ](https://game.weixin.qq.com/cgi-bin/gamewxagwasmsplitwap/getunityplugininfo?download=1)to import into your project.
---
* Delete `LitJson.dll` from the WX-WASM-SDK-V2 plugin (Note: The Unity Engine also includes this; keep the one in F8Framework).  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20240524000853.png)
* Add F8Framework’s `LitJson` reference to the `.asmdef` files in the WX-WASM-SDK-V2’s `Editor` and `Runtime` directories.  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20240524001621.png)
---
* Set the following three variables to true:
1. [AssetManager.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/AssetManager/AssetManager.cs)
```C#
// Force asset loading mode to remote (for WeChat Mini Games)  
public static bool ForceRemoteAssetBundle = false;
```
2. [ABBuildTool.cs](https://github.com/TippingGame/F8Framework/blob/main/Editor/AssetManager/ABBuildTool.cs)
```C#
// Append MD5 to AB names after building (for WeChat Mini Games)  
private static bool appendHashToAssetBundleName = false;
```
3. [DownloadRequest.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/AssetManager/DownloadRequest/DownloadRequest.cs)
```C#
// Disable Unity's cache system on WebGL (for WeChat Mini Games)  
public static bool DisableUnityCacheOnWebGL = false;
```
---

* (Note) Since WeChat Mini Games only support remote AB loading, press F5, configure the remote asset address, and build the game once.  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241203214539_2.png)
* Alternatively, directly modify the `AssetRemoteAddress` parameter in [GameVersion.json](https://github.com/TippingGame/F8Framework/blob/main/AssetMap/Resources/GameVersion.json)  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20241203214624.png)
* Build settings:  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_20240329230924.png)

### If the build fails: Try building with Unity’s default settings first.

---

## Remote Building with Jenkins

1. [Download Java SDK (Demo uses v21.0.7)](https://www.oracle.com/cn/java/technologies/downloads/)
2. [Download Jenkins (Demo uses v2.504.2 LTS)](https://www.jenkins.io/download/), Install using a local administrator account (otherwise, builds may fail).  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749999206518.png)
3. After installation, start Jenkins.
4. Create a job with the following configuration:  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749788881032.png)  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749788919208.png)
5. Install the **Unity3d Plugin** from the Plugins interface.  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749787027911.png)
6. Add Unity versions in the **Tools** interface.  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749787076031.png)
7. Copy [config.xml](https://github.com/TippingGame/F8Framework/blob/main/Editor/Build/Jenkins/config.xml) to the corresponding job directory in Jenkins’ data folder.  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749789384733.png)
8. Restart the Jenkins service.  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749790107926.png)
9. (If needed) Modify the Unity version in the **Build Steps** configuration (name must match the Tools interface).  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749789502754.png)
10. Finally, adjust the parameters (matching the editor’s build settings) and start building.  
    ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/SDKManager/ui_1749789318664.png)  