# F8 Integrated HybridCLR

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Plugin Installation (Requires HybridCLR First)
Note! Built into → HybridCLR：https://github.com/focus-creative-games/hybridclr_unity.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/focus-creative-games/hybridclr_unity.git

## Introduction (Simply press F8 to start game development without distractions)
**Integrated HybridCLR Hot Update Code Component**
1. First, follow this[ Official Quickstart Guide ](https://www.hybridclr.cn/docs/beginner/quickstart)to create a HotUpdate assembly
2. Locate the[ F8Helper.cs ](https://github.com/TippingGame/F8Framework/blob/main/Editor/F8Helper/F8Helper.cs)code file:
   * Uncomment the following code section
3. Add metadata - see[ Official Generic Types Tutorial ](https://www.hybridclr.cn/docs/beginner/generic)

```C#
// Metadata supplementation - DLLs here won't be hot updated. Typically found in {project}/HybridCLRData/AssembliesPostIl2CppStrip/{target}
public static List<string> AOTDllList = new List<string>
{
    "mscorlib.dll",
    "System.dll",
    "System.Core.dll", // Required if using Linq
    // "Newtonsoft.Json.dll", 
    // "protobuf-net.dll",
    "F8Framework.Core.dll", // For framework generic support
};

[MenuItem("开发工具/3: 生成并复制热更新Dll-F8", false, 210)]
public static void GenerateCopyHotUpdateDll()
{
    // SessionState.SetBool("compilationFinishedHotUpdateDll", false);
    //
    // // Only execute commands using HybridCLR (choose one)
    // HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
    // // Execute commands using both HybridCLR and Obfuz (choose one)
    // // Obfuz4HybridCLR.PrebuildCommandExt.GenerateAll();
    //
    // string outpath = Application.dataPath + "/AssetBundles/Code/";
    //
    // FileTools.SafeClearDir(outpath);
    // FileTools.CheckDirAndCreateWhenNeeded(outpath);
    // foreach (var dll in HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesIncludePreserved) // 获取HybridCLR设置面板的dll名称
    // {
    //     var path = HybridCLR.Editor.SettingsUtil.GetHotUpdateDllsOutputDirByTarget(
    //         EditorUserBuildSettings.activeBuildTarget) + "/" + dll + ".dll";
    //
    //     // Uncomment to use both HybridCLR and Obfuz
    //     // if (Obfuz.Settings.ObfuzSettings.Instance.assemblySettings.GetObfuscationRelativeAssemblyNames().Contains(dll))
    //     // {
    //     //     path = Obfuz4HybridCLR.PrebuildCommandExt.GetObfuscatedHotUpdateAssemblyOutputPath(
    //     //         EditorUserBuildSettings.activeBuildTarget) + "/" + dll + ".dll";
    //     // }
    //     
    //     FileTools.SafeCopyFile(path, outpath + dll + ".bytes");
    //     LogF8.LogAsset("Generate and copy hot update dll: " + dll);
    // }
    //
    // foreach (var aotDllName in F8Helper.AOTDllList)
    // {
    //     var mscorlibsouPath = HybridCLR.Editor.SettingsUtil.GetAssembliesPostIl2CppStripDir(
    //         EditorUserBuildSettings.activeBuildTarget) + "/" + aotDllName;
    //     
    //     FileTools.SafeCopyFile(
    //         mscorlibsouPath,
    //         outpath + aotDllName + "by.bytes");
    //     LogF8.LogAsset("Generate and copy supplementary metadata dll: " + aotDllName);
    // }
    //
    // AssetDatabase.Refresh();
}
```
3. Code has been split into assemblies (Note: AOT code cannot reference hot update code)
   * AOT Assemblies: (`F8Framework.Core`) (Note: Other scattered project code will also be packaged as AOT)
   * Hot Update Assemblies: (`F8Framework.F8ExcelDataClass`), (`F8Framework.Launcher`)
4. Drag these two hot update assemblies into HybridCLR settings panel  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/HybridCLR/ui_20241128235509.png)
5. Assembly-CSharp is the default global assembly in Unity, and it can be treated as a hot update assembly just like a regular DLL [(Official Configuration)](https://www.hybridclr.cn/docs/basic/projectsettings). Simply add `Assembly-CSharp` to the `Hot Update Assemblies` list below, while the `LoadDll.cs` script needs to be placed in a separate assembly as AOT code.
6. Important: Main project cannot directly reference hot update code - must use reflection:
   * Attach a DLL loading script to startup scene (handles metadata supplementation and assembly loading)
```C#
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F8Framework.Core;
using HybridCLR;
using UnityEngine;

public class LoadDll : MonoBehaviour
{
    public IEnumerator Start()
    {
        ModuleCenter.Initialize(this);
        // You can start a hot update here
        HotUpdateManager HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();
        DownloadManager DownloadManager = ModuleCenter.CreateModule<DownloadManager>();
        AssetManager AssetManager = ModuleCenter.CreateModule<AssetManager>();
        yield return AssetBundleManager.Instance.LoadAssetBundleManifest();
        
        // Initialize local version
        HotUpdate.InitLocalVersion();

        // Initialize remote version
        yield return HotUpdate.InitRemoteVersion();
            
        // Initialize asset version
        yield return HotUpdate.InitAssetVersion();
            
        // Check resources needing updates
        var (downloadInfos, allSize) = HotUpdate.CheckHotUpdate();
        
        // Execute hot update
        HotUpdate.StartHotUpdate(downloadInfos, () =>
        {
            LogF8.Log("Complete");
            // Optional metadata supplementation
            List<string> aotDllList = new List<string>
            {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll", // Required for Linq
                // "Newtonsoft.Json.dll", 
                // "protobuf-net.dll",
                "F8Framework.Core.dll", // For framework generics
            };

            foreach (var aotDllName in aotDllList)
            {
                byte[] dllBytes = AssetManager.Instance.Load<TextAsset>(aotDllName + "by").bytes;
                LoadImageErrorCode err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                LogF8.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
            
#if !UNITY_EDITOR
            // Runtime DLL loading
            TextAsset asset1 = AssetManager.Instance.Load<TextAsset>("F8Framework.F8ExcelDataClass");
            Assembly hotUpdateAss1 = Assembly.Load(asset1.bytes);
            TextAsset asset2 = AssetManager.Instance.Load<TextAsset>("F8Framework.Launcher");
            Assembly hotUpdateAss2 = Assembly.Load(asset2.bytes);
#else
            // Editor assembly access
            Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "F8Framework.F8ExcelDataClass");
            Assembly hotUpdateAss2 = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "F8Framework.Launcher");
#endif
            Type type = hotUpdateAss2.GetType("F8Framework.Launcher.GameLauncher");
            // AddComponent
            gameObject.AddComponent(type);
        }, () =>
        {
            LogF8.Log("Failed");
        }, eventArgs =>
        {
            long downloadedBytes = eventArgs.TotalDownloadedLength;
            double speedBytesPerSecond = eventArgs.DownloadInfo.DownloadedLength / eventArgs.DownloadInfo.DownloadTimeSpan.TotalSeconds;
            LogF8.Log($"Progress: {downloadedBytes}B/{allSize}B, Speed: {speedBytesPerSecond}B/s");
        });
    }
    void Update()
    {
        // Update module, do not call from multiple places.
        ModuleCenter.Update();
    }

    void LateUpdate()
    {
        // Update module, do not call from multiple places.
        ModuleCenter.LateUpdate();
    }

    void FixedUpdate()
    {
        // Update module, do not call from multiple places.
        ModuleCenter.FixedUpdate();
    }
}
```
### Frequently Asked Questions:
1. Packaging HybridCLR takes a long time. After the first execution, you can switch to this API: `HybridCLR.Editor.Commands.CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget);`
   * If Obfuz is enabled, you can use this API instead: `Obfuz4HybridCLR.PrebuildCommandExt.CompileAndObfuscateDll();`
   * [Reference Documentation (Working with HybridCLR)](https://www.obfuz.com/docs/manual/hybridclr/work-with-hybridclr)
