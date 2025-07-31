<p align="center">
    <img src="https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Logo.png" width="256" height="256" alt="F8Framework" style="display: block; margin: 20px auto -90px;">
</p>

# F8 Framework

[![license](https://img.shields.io/github/stars/TippingGame/F8Framework.svg)](https://github.com/TippingGame/F8Framework/stargazers) 
[![license](https://img.shields.io/github/forks/TippingGame/F8Framework?color=eb6ea5)](https://github.com/TippingGame/F8Framework/fork) 
[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

> F8 框架初衷：希望自己点击 F8，就能开始制作游戏，不想多余的事。
> 
> F8 Framework original intention: Just click F8 and start making the game, don't want to be redundant.

## 简介
F8 Framework是一个**优雅，轻量，符合直觉的**基于Unity引擎的游戏框架，组件围绕F8一键启动，**不用繁琐的启动配置**，**最低的心智负担**，框架整体遵循以**极少的使用成本**开发游戏。  

## 支持版本
Unity 2021、2022、2023、6000  
构建可支持：Win / Android / iOS / Mac / Linux / WebGL / 微信小游戏（[构建文档，Jenkins集成](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README.md)）

## 文档快速预览 - 1分钟
### ----------可选功能----------
* [1. 热更新版本管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/HotUpdateManager/README.md) - 选择打包平台，输出路径，版本号，远程资产加载地址，启用热更新，全量打包，分包，空包。
* [2. 代码热更新（接入HybridCLR）](https://github.com/TippingGame/F8Framework/blob/main/Tests/HybridCLR/README.md) - [HybridCLR](https://github.com/focus-creative-games/hybridclr) 是一个特性完整、零成本、高性能、低内存的近乎完美的Unity全平台原生c#热更方案。
### ----------核心功能----------
* [1. 配置表（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/ExcelTool/README.md) - 使用Excel作为配置表，兼顾**高性能、高适应性**。字段类型分为：**基础类型、容器类型**，可**自由组合**类型。点击F8生成的 Excel 二进制文件和C#类，点击F7实时读取 Excel 并替换数据，无需频繁导表，可多端运行时读写Excel。
* [2. 资源加载（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README.md) - **编辑器下**：点击F8自动生成资产索引/AB名称，自动区分不同平台，清理多余AB和文件夹，Editor模式下减少开发周期。**运行时**：同步/异步加载单个资产，展开文件夹或同一AB下所有资产，自动判断是 Resources / AssetBundle 资产，加载Remote远程资产，获取加载进度，同步打断异步加载。你可以这样加载AssetBundle：**单个资产单个AB、指定文件夹名称（文件夹第一层的AB）、设置多个资产为同一AB名（指定任意资产名）**
* [3. 模块中心（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Module/README.md) - 模块中心可以获取所有模块的实例，延迟加载策略，自由控制生命周期。
* [4. 日志管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Log/README.md) - 打印日志，写入文件，上报错误。
* [5. 声音管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Audio/README.md) - 声音的播放/暂停/停止/进度控制，音量控制/保存，全局暂停/恢复。Audio分为三大类：**背景音乐、人声、特效声**。
* [6. 事件管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/README.md) - 发送消息事件，事件监听，防止**消息死循环**，自动释放事件。
* [7. 时间管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Timer/README.md) - 提供Timer、FrameTimer两种计时器，暂停/恢复，自动释放Timer。
* [8. 补间动画（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Tween/README.md) - 播放/终止动画，**自由组合**动画，有旋转/位移/缩放/渐变/填充/震动动画，可根据UI的**相对布局**位移动画。
* [9. 引用池管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/ReferencePool/README.md) - 引用池管理，C# 对象，入池/取出/回收/清空。
* [10. 游戏对象池（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/GameObjectPool/README.md) - 游戏对象池管理，GameObject 预加载池化，生成/销毁/延迟销毁，生命周期事件监听。
* [11. 本地化管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Localization/README.md) - 本地化 Text / TextMeshPro / Image / RawImage / SpriteRenderer / Renderer / Audio / Timeline 等组件，使用 **Excel** 作为多语言翻译表。
* [12. 有限状态机（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/FSM/README.md) - 自定义有限状态机 FSMState / FSMSwitch，创建/切换状态/轮询/销毁。
* [13. 下载管理器（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Download/README.md) - 支持localhost与http地址文件的下载，可**本地写入、监听下载进度、断点续传**，支持动态添加、移除、暂停、恢复下载。
* [14. UI界面管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/UI/README.md) - 处理界面加载、打开、关闭、查询、层级控制、自定义动画、自动获取组件索引。UI界面分为三大类：**普通UI、模态弹窗、非模态弹窗**，内置各种常用组件。
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
* [LitJson](https://github.com/LitJSON/litjson)：序列化/反序列化 JSON（已修改字典Key支持byte，short，int，long，float，double，string 类型，增加Unity常用类型：Type，Vector2，Vector3，Vector4，Quaternion，GameObject，Transform，Color，Color32，Bounds，Rect，RectOffset，LayerMask，Vector2Int，Vector3Int，RangeInt，BoundsInt，修复DateTime精度丢失的问题）

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

更新版本号说明，如：1.0.0，第一位代表大版本，第二位代表框架的使用有修改，第三位代表修订版本。（注意：确保更新框架前工程没有报错，更新后点击F8即可）

## 新手指南

* 首次导入插件后，需要点击一次 F8
* [游戏启动器：](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)游戏启动器示例（注意：使用框架前必须先启动框架）。[GameLauncher.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)  
* [模块自定义改名：](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)模块自定义改名。[FF8.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)  

## 视频教程

* 视频教程：[【Unity框架】开源 F8Framework 游戏框架介绍](https://www.bilibili.com/video/BV16i42117nx/?share_source=copy_web&vd_source=2fde88c46cd96d06f86859724813e355)  
* 游戏项目Demo：[https://github.com/TippingGame/F8FrameworkDemo](https://github.com/TippingGame/F8FrameworkDemo)  

## 社区
* qq开发交流群：[722647431](https://qm.qq.com/q/uTxdVIJykE)  
* Discord聊天组：[https://discord.gg/AHRb7Hwy9R](https://discord.gg/dwWQ8EHz)  

## 上架游戏
| 脑光（[TapTap](https://www.taptap.cn/app/725455) / [WebGL](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/h5game/naoguang/index.html)）       | 消除异世界（[微信小游戏](#小程序://消除异界/dZ7HgPPybAOkD2E) / [4399](http://www.4399.com/flash/250713.htm)）                     | 待定                                              | 待定                                             | 
|-------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------|-------------------------------------------------|------------------------------------------------|
| <img src='https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/naoguang_icon_256.png' width="256"/> | <img src='https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/xcysj_icon_256.png' width="256"/> | <img src='' width="256"/> | <img src='' width="256"/> |


## 赞助
###### (可备注留言你的主页链接)
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Sponsorship_icon_356.png)
###### 感谢赞助

| <img src='https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Sponsor/yemaozi.png' width="38"/> 夜猫子 ￥8.88 | <img src='https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Sponsor/fumeng.png' width="38"/> 浮梦 ￥6.6 | <img src='https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Sponsor/WXanonymous.png' width="38"/> C*r ￥20 | <img src='https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Sponsor/WXanonymous.png' width="38"/> .*. ￥20 | <img src='https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Sponsor/WXanonymous.png' width="38"/> N*o ￥18.8 |
| --- | --- | --- | --- |------------------------------------------------------------------------------------------------------------------------------------|

---

## Introduction
The **F8 Framework** is an **elegant, lightweight, and intuitive** game framework based on the Unity engine. Centered around the F8 one-click startup, it eliminates **tedious configuration** and **minimizes cognitive load**, adhering to the principle of **minimal development overhead** for game creation.  

## Supported Versions
Unity 2021, 2022, 2023, 6000  
Build targets: Win / Android / iOS / Mac / Linux / WebGL / WeChat Mini Games（[Build Documentation, Jenkins Integration](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README_EN.md)）

## Quick Overview – 1 Minute
### ----------Optional Features----------
* [1. Hot update version manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/HotUpdateManager/README_EN.md) - Select build platform, output path, version number, remote asset URL, enable hot updates, full build, subpackage, or empty build.
* [2. Code Hot Update (Import HybridCLR)](https://github.com/TippingGame/F8Framework/blob/main/Tests/HybridCLR/README_EN.md) - [HybridCLR](https://github.com/focus-creative-games/hybridclr) is a feature-complete, zero-cost, high-performance, low-memory near-perfect Unity cross-platform native C# hot update solution.
### ----------Core Features----------
* [1. Config table (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/ExcelTool/README_EN.md) - Uses Excel for configuration tables, balancing **high performance and adaptability**. Field types include **basic types and container types**, with **free combination** support. Press F8 to generate Excel binary files and C# classes, or F7 to hot-reload Excel data without frequent exports. Supports runtime read/write across platforms.
* [2. Asset Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README_EN.md) - **Editor Mode:** Press F8 to auto-generate asset indices/AB names, auto-detect platforms, and clean redundant ABs/folders, reducing iteration time. **Runtime:** Sync/async loading for single assets, folder contents, or shared AB assets. Supports Resources/AssetBundle detection, remote asset loading, progress tracking, and async interruption. AB strategies: **per-asset AB, folder-based AB, or custom AB grouping**.
* [3. Module Center (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Module/README_EN.md) - Centralized module access with lazy-loading and lifecycle control.
* [4. Log Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Log/README_EN.md) - Logging, file writing, and error reporting.
* [5. Sound Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Audio/README_EN.md) - Play/pause/stop/seek controls, volume settings/saving, global pause/resume. Three audio types: **BGM, Voice, SFX**.
* [6. Event Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/README_EN.md) - Event dispatch/listening, **dead-loop prevention**, and auto-cleanup.
* [7. Time Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Timer/README_EN.md) - Offers Timer and FrameTimer with pause/resume and auto-release.
* [8. Tween Animation (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Tween/README_EN.md) - Play/stop tweens, **composite animations**, including rotation/translation/scale/fade/fill/shake, and **UI-relative** motion.
* [9. Reference Pool Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/ReferencePool/README_EN.md) - C# object pooling: get/release/clear.
* [10. GameObject Pool (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/GameObjectPool/README_EN.md) - GameObject pooling with preloading, spawn/destroy/delayed-destroy, and lifecycle events.
* [11. Localization Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Localization/README_EN.md) - Localizes Text/TextMeshPro/Image/RawImage/SpriteRenderer/Renderer/Audio/Timeline using **Excel** for translations.
* [12. Finite state machine (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/FSM/README_EN.md) - Custom FSMState/FSMSwitch for state creation/switching/polling/destruction.
* [13. Download Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Download/README_EN.md) - Supports localhost/HTTP downloads with **local write, progress tracking, and resumable transfers**. Dynamic add/remove/pause/resume.
* [14. UI interface manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/UI/README_EN.md) - Handles loading/opening/closing/querying/layering/custom animations/auto-component indexing. UI types: **Standard UI, Modal Popup, Non-Modal Popup**, with built-in common components.
* [15. Input System Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Input/README_EN.md) - Unified multi-platform input via customizable devices, with hot-swapping or multi-device support.
* [16. Game Procedure Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Procedure/README_EN.md) - Custom ProcedureNode for flow control: add/run/poll/remove.
* [17. Local data storage (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Storage/README_EN.md) - Local data save/load.
* [18. SDK Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README_EN.md) - **Native platform interoperability**: Supports integration with multiple platform/channel SDKs for login/logout/account switching/payments/video ads/game exit/native toast notifications.
* [19. Network Manager (built-in)](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/README_EN.md) - Long connections via KCP/TCP/WebSocket for Client/Server.
### ----------Third-Party Libraries (Watch for Conflicts)----------
* Excel.dll：Read/write Excel (cache path modified to **Application.persistentDataPath**).
* I18N.CJK.dll，I18N.dll，I18N.MidEast.dll，I18N.Other.dll，I18N.Rare.dll，I18N.West.dll：Only for Read/Write Excel
* [ICSharpCode.SharpZipLib](https://github.com/icsharpcode/SharpZipLib)：Compression/Decompression
* [Mirror(built in):KCP](https://github.com/MirrorNetworking/kcp2k)：Reliable UDP
* [Mirror(built in):Telepathy](https://github.com/MirrorNetworking/Telepathy)：TCP
* [Mirror(built in):Websockets](https://github.com/MirrorNetworking/SimpleWebTransport)：Websockets
* [LitJson](https://github.com/LitJSON/litjson)：JSON serialization/deserialization (modified to support dictionary keys of byte, short, int, long, float, double, and string types; added support for commonly used Unity types: Type, Vector2, Vector3, Vector4, Quaternion, GameObject, Transform, Color, Color32, Bounds, Rect, RectOffset, LayerMask, Vector2Int, Vector3Int, RangeInt, BoundsInt; fixed the DateTime precision loss issue)

## Setup
### Recommended (Editable Source/Updates)
[Install Git](https://git-scm.com/), then clone:
```text
git clone https://github.com/TippingGame/F8Framework.git
```
Or [download the package](https://codeload.github.com/TippingGame/F8Framework/zip/refs/heads/main) manually.

### Alternative

Add via Unity Package Manager:

1. Open **Unity Editor** → **Window** → **Package Manager**

2. Click **+** → **Add Package from git URL**

3. Enter: <https://github.com/TippingGame/F8Framework.git>

Versioning: MAJOR.MINOR.PATCH (e.g., 1.0.0). Ensure no errors before updating, then press F8.

## Beginner’s Guide

* Press F8 after first import.
* [GameLauncher: ](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)Game Launcher Example (Note: The framework must be initialized before any usage).[GameLauncher.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/GameLauncher.cs)
* [Module renaming: ](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)Customize module names.[FF8.cs](https://github.com/TippingGame/F8Framework/blob/main/Launcher/FF8.cs)  

## Tutorials

* Video: [【Unity Framework】F8Framework Introduction.](https://www.bilibili.com/video/BV16i42117nx/?share_source=copy_web&vd_source=2fde88c46cd96d06f86859724813e355)
* Demo Project: [https://github.com/TippingGame/F8FrameworkDemo.](https://github.com/TippingGame/F8FrameworkDemo)

## Community
* QQ Group: [722647431](https://qm.qq.com/q/uTxdVIJykE)
* Discord: [https://discord.gg/AHRb7Hwy9R](https://discord.gg/dwWQ8EHz)  