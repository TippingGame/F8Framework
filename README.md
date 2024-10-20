<p align="center">
    <img src="https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Logo.png" width="256" height="256" alt="F8Framework" style="display: block; margin: 20px auto -90px;">
</p>

# F8 Framework

[![license](https://img.shields.io/github/stars/TippingGame/F8Framework.svg)](https://github.com/TippingGame/F8Framework/stargazers) 
[![license](https://img.shields.io/github/forks/TippingGame/F8Framework?color=eb6ea5)](https://github.com/TippingGame/F8Framework/fork) 
[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

> F8 框架初衷：希望自己点击 F8，就能开始制作游戏，不想多余的事。
> 
> F8 Framework original intention: Just click F8 and start making the game, don't want to be redundant.

## 简介
F8 Framework是一个**优雅，轻量，符合直觉的**基于Unity引擎的游戏框架，组件围绕F8一键启动，**不用繁琐的启动配置**，**最低的心智负担**，框架整体遵循以**极少的使用成本**开发游戏。  

## 支持版本
Unity 2021.3.15f1+  
构建可支持：Win / Android / iOS / Mac / Linux / WebGL / 微信小游戏（[构建文档](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README.md)）

## 文档快速预览 - 1分钟
### ----------可选功能----------
* [1. 热更新版本管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/HotUpdateManager/README.md) - 选择打包平台，输出路径，版本号，远程资产加载地址，启用热更新，全量打包，分包，空包。
* [2. 高速本地缓存（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/CacheStorage/README.md) - 高速本地缓存，下载并缓存byte、json、text、Texture等可序列化资产，设置缓存数量、大小、存活时间，清理缓存。
* [3. 代码热更新（接入HybridCLR）](https://github.com/TippingGame/F8Framework/blob/main/Tests/HybridCLR/README.md) - [HybridCLR](https://github.com/focus-creative-games/hybridclr) 是一个特性完整、零成本、高性能、低内存的近乎完美的Unity全平台原生c#热更方案。
### ----------核心功能----------
* [1. 配置表（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/ExcelTool/README.md) - 使用Excel作为配置表，兼顾**高性能、高适应性**，点击F8加载手动生成的 Excel 二进制缓存，运行时自动读取最新 Excel，无需频繁导表。
* [2. 资源加载（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README.md) - **编辑器下**：点击F8自动生成资产索引/AB名称，自动区分不同平台，清理多余AB和文件夹，Editor模式下减少开发周期。**运行时**：同步/异步加载单个资产，展开文件夹或同一AB下所有资产，自动判断是 Resources / AssetBundle 资产，加载Remote远程资产，获取加载进度，同步打断异步加载。你可以这样加载AssetBundle：**单个资产单个AB、指定文件夹名称（文件夹第一层的AB）、设置多个资产为同一AB名（指定任意资产名）**
* [3. 模块中心（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Module/README.md) - 模块中心可以获取所有模块的实例，延迟加载策略，自由控制生命周期。
* [4. 日志管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Log/README.md) - 打印日志，写入文件，上报错误。
* [5. 声音管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Audio/README.md) - 声音的播放/暂停/停止/进度控制，音量控制/保存，全局暂停/恢复。Audio分为三大类：**背景音乐、人声、特效声**。
* [6. 事件管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/README.md) - 发送消息事件，事件监听，防止**消息死循环**，自动释放事件。
* [7. 时间管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Timer/README.md) - 提供Timer、FrameTimer两种计时器，暂停/恢复，自动释放Timer。
* [8. 补间动画（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Tween/README.md) - 播放/终止动画，有旋转/位移/缩放/渐变/填充动画，可根据UI的**相对布局**位移动画。
* [9. 引用池管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/ReferencePool/README.md) - 引用池管理，C# 对象，入池/取出/回收/清空。
* [10. 游戏对象池（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/GameObjectPool/README.md) - 游戏对象池管理，GameObject 预加载池化，生成/销毁/延迟销毁，生命周期事件监听。
* [11. 本地化管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Localization/README.md) - 本地化 Text / TextMeshPro / Image / RawImage / SpriteRenderer / Renderer / Audio / Timeline 等组件，使用 **Excel** 作为多语言翻译表。
* [12. 有限状态机（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/FSM/README.md) - 自定义有限状态机 FSMState / FSMSwitch，创建/切换状态/轮询/销毁。
* [13. 下载管理器（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Download/README.md) - 支持localhost与http地址文件的下载，可**本地写入、监听下载进度、断点续传**，支持动态添加、移除、暂停、恢复下载。
* [14. UI界面管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/UI/README.md) - 处理界面加载、打开、关闭、查询、层级控制、自定义动画、自动获取组件索引。UI界面分为三大类：**普通UI、模态弹窗、非模态弹窗**。
* [15. 输入系统管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Input/README.md) - 使用同一套代码，通过自定义输入设备，适配多平台，可热切换输入设备，或同时启用多套输入设备。
* [16. 游戏流程管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Procedure/README.md) - 自定义流程节点 ProcedureNode，控制游戏流程的，添加/运行/轮询/移除。
* [17. 本地数据存储（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Storage/README.md) - 本地数据存储/读取。
* [18. SDK接入管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README.md) - 与**原生平台交互**，接入多个平台或者渠道SDK，登录/退出/切换/支付/视频广告/退出游戏/原生Toast。
* [19. 网络连接与通信（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/README.md) - 使用 KCP / TCP / WebSocket 网络通讯协议建立长连接通道，支持Client端和Server端。
### ----------第三方库（注意冲突）----------
* Excel.dll：读/写 Excel（已修改缓存地址为Application.persistentDataPath）
* I18N.CJK.dll，I18N.dll，I18N.MidEast.dll，I18N.Other.dll，I18N.Rare.dll，I18N.West.dll：只为读/写 Excel
* [ICSharpCode.SharpZipLib](https://github.com/icsharpcode/SharpZipLib)：压缩/解压缩
* [Mirror(内置):KCP](https://github.com/MirrorNetworking/kcp2k)：Reliable UDP
* [Mirror(内置):Telepathy](https://github.com/MirrorNetworking/Telepathy)：TCP
* [Mirror(内置):Websockets](https://github.com/MirrorNetworking/SimpleWebTransport)：Websockets
* [LitJson](https://github.com/LitJSON/litjson)：序列化/反序列化 JSON（已修改字典Key可以使用int类型，增加Unity常用类型：Type，Vector2，Vector3，Vector4，Quaternion，GameObject，Transform，Color，Color32，Bounds，Rect，RectOffset，LayerMask，Vector2Int，Vector3Int，RangeInt，BoundsInt）
* [MessagePack-CSharp](https://github.com/MessagePack-CSharp/MessagePack-CSharp)：序列化/反序列化数据

## 使用步骤

### 推荐导入方式（可修改源码或更新）
[安装git](https://git-scm.com/)，使用git命令拉取：
```text
git clone https://github.com/TippingGame/F8Framework.git
```
或者直接[下载完整包](https://codeload.github.com/TippingGame/F8Framework/zip/refs/heads/main)，放入工程里。

### 也可以

在 Unity 包管理器中，使用 Git URL 添加 F8 核心包。

1. 打开 Unity Editor

2. 点击菜单的 **Window** 项，再点击 **Package Manager** 子项

3. 点击左上角 **+** 号，选择 **Add Package from git URL**

4. 输入 <https://github.com/TippingGame/F8Framework.git>，请确认导入成功

## 新手指南

* [游戏启动器：](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)游戏启动器示例。[GameLauncher.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)  
* [模块自定义改名：](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)模块自定义改名。[FF8.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)  

## 视频教程

* 视频教程：[【Unity框架】开源 F8Framework 游戏框架介绍](https://www.bilibili.com/video/BV16i42117nx/?share_source=copy_web&vd_source=2fde88c46cd96d06f86859724813e355)  
* 游戏项目Demo：[https://github.com/TippingGame/F8FrameworkDemo](https://github.com/TippingGame/F8FrameworkDemo)  

## 社区
* qq开发交流群：722647431

---

## INTRODUCTION
F8 Framework is an **elegant, lightweight, and intuitive** Game Framework based on the Unity engine. The components revolve around F8 one click startup, **without the need for cumbersome startup configurations**, with minimal mental burden. The overall framework follows the principle of developing games with **minimal usage costs**.  

## SUPPORTED
Unity 2021.3.15f1+  
Support for building：Win / Android / iOS / Mac / Linux / WebGL / WeChat mini game（[Building Documents](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README_EN.md)）

## Quick preview of document - 1 minute
### ----------Optional Features----------
* [1. Hot update version manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/HotUpdateManager/README_EN.md) - Select packaging platform, output path, version number, remote asset loading address, enable hot update, full packaging, subcontracting, and empty packaging.
* [2. High speed local cache (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/CacheStorage/README_EN.md) - High speed local caching, downloading and caching bytes Serializable assets such as JSON, text, and texture, set cache quantity, size, and lifetime, and clear cache.
* [3. Code Hot Update (Import HybridCLR)](https://github.com/TippingGame/F8Framework/blob/main/Tests/HybridCLR/README_EN.md) - [HybridCLR](https://github.com/focus-creative-games/hybridclr) HybridCLR is a full-platform native c# hot update solution for Unity with complete features, zero cost, high performance, and low memory.
### ----------Core Features----------
* [1. Config table (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/ExcelTool/README_EN.md) - Using Excel as the configuration table, **balancing high performance and adaptability**, click F8 to load the manually generated Excel binary cache, and automatically read the latest Excel at runtime without the need for frequent table navigation.
* [2. Asset Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README_EN.md) - **At editor**: Click F8 to automatically generate asset index/AB name, automatically distinguish different platforms, and clean up excess AB and folders, Reduce development cycle in Editor mode. **At runtime**: load a single asset synchronously/asynchronously, expand a folder or all assets under the same AB, automatically determine whether it is a Resources/AssetBundle asset, load remote assets, obtain loading progress, and synchronously interrupt asynchronous loading. You can load AssetBundle in this way: **a single asset with a single AB, specify a folder name (AB on the first layer of the folder), set multiple assets to the same AB name (specify any asset name)**.
* [3. Module Center (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Module/README_EN.md) - The module center can obtain instances of all modules, implement delayed loading strategies, and freely control the lifecycle.
* [4. Log Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Log/README_EN.md) - Print logs, write files, and report errors.
* [5. Sound Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Audio/README_EN.md) - Play/pause/stop/progress control of sound, volume control/save, global pause/resume. Audio is divided into three categories: **background music, vocals, and special effects sound**.
* [6. Event Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/README_EN.md) - Send message events, monitor events, **prevent message loops**, and automatically release events.
* [7. Time Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Timer/README_EN.md) - Provide Timer/FrameTimer has two types of timers, pause/resume, and automatically release the timer.
* [8. Tween Animation (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Tween/README_EN.md) - Play/Stop animations, including rotation/displacement/scaling/gradient/fill animations, which can be shifted based on the **relative layout** of the UI.
* [9. Reference Pool Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/ReferencePool/README_EN.md) - Reference Pool Manager, C # object, pooling/retrieving/recycling/emptying.
* [10. GameObject Pool (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/GameObjectPool/README_EN.md) - GameObject Pool Manager, GameObject preload pooling, generation/destruction/delayed destruction, lifecycle event monitoring.
* [11. Localization Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Localization/README_EN.md) - Localize components such as Text/TextMeshPro/Image/RawImage/SpriteRenderer/Renderer/Audio/Timeline, and use **Excel** as a multilingual translation table.
* [12. Finite state machine (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/FSM/README_EN.md) - Customize finite state machine FSMState/FSMSwitch, create/switch states/poll/destroy.
* [13. Download Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Download/README_EN.md) - Supports downloading of files from both local host and HTTP addresses, **allowing for local writing, monitoring of download progress, and recovery of downloads**. It also supports dynamic addition, removal, pause, and recovery of downloads.
* [14. UI interface manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/UI/README_EN.md) - Handle interface loading, opening, closing, querying, hierarchical control, custom animation, and automatic retrieval of component indexes. UI interfaces are divided into three categories: **regular UI, modal pop ups, and non modal pop ups**.
* [15. Input System Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Input/README_EN.md) - Using the same set of code, customize input devices, adapt to multiple platforms, hot switch input devices, or enable multiple sets of input devices simultaneously.
* [16. Game Procedure Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Procedure/README_EN.md) - Customize the process node ProcedureNode to control the game process, add/run/poll/remove.
* [17. Local data storage (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Storage/README_EN.md) - Local data storage/reading.
* [18. SDK Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README_EN.md) - Interacting with **native platforms**, accessing multiple platform or channel SDKs, logging in/out/switching/payment/video advertising/exiting games/native Toast.
* [19. Network Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/README_EN.md) - Establish long connection channels using KCP/TCP/WebSocket network communication protocols, supporting both client and server ends.
### ----------Third Party Libraries (note conflicts)----------
* Excel.dll：Read/Write Excel (cache address has been changed to Application. persistentDataPath)
* I18N.CJK.dll，I18N.dll，I18N.MidEast.dll，I18N.Other.dll，I18N.Rare.dll，I18N.West.dll：Only for Read/Write Excel
* [ICSharpCode.SharpZipLib](https://github.com/icsharpcode/SharpZipLib)：Compression/Decompression
* [Mirror(built in):KCP](https://github.com/MirrorNetworking/kcp2k)：Reliable UDP
* [Mirror(built in):Telepathy](https://github.com/MirrorNetworking/Telepathy)：TCP
* [Mirror(built in):Websockets](https://github.com/MirrorNetworking/SimpleWebTransport)：Websockets
* [LitJson](https://github.com/LitJSON/litjson)：Serialize/Deserialize JSON (modified dictionary key can use int type, added Unity common types: Type，Vector2，Vector3，Vector4，Quaternion，GameObject，Transform，Color，Color32，Bounds，Rect，RectOffset，LayerMask，Vector2Int，Vector3Int，RangeInt，BoundsInt)
* [MessagePack-CSharp](https://github.com/MessagePack-CSharp/MessagePack-CSharp)：Serialize/Deserialize data

## IMPORT
### Recommend (source code can be modified or updated)
[Install Git](https://git-scm.com/) use git command to pull：
```text
git clone https://github.com/TippingGame/F8Framework.git
```
Or [download the complete package](https://codeload.github.com/TippingGame/F8Framework/zip/refs/heads/main), Put it into the project。

### also

In the Unity Package Manager, add the F8Framework package using Git URL.

1. Open Unity Editor

2. Click on the **Window** item in the menu, then click on the **Package Manager** sub item

3. Click on the **+** sign in the upper left corner and select **Add Package from git URL**

4. Input <https://github.com/TippingGame/F8Framework.git>, Please confirm successful import

## GUIDE

* [GameLauncher：](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)Game Launcher Example.[GameLauncher.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)
* [Module renaming：](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)Module customization renaming.[FF8.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)  

