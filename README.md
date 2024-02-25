# F8 Framework

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) [![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) [![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

> F8 框架初衷：希望自己点击 F8，就能开始制作游戏，不想多余的事。
> 
> F8 Framework original intention: Just click F8 and start making the game, don't want to be redundant.

## 简介
F8 Framework是一个**优雅，轻量，符合直觉的**基于Unity引擎的Game Framework，组件围绕F8一键启动，延迟加载策略减少不必要的资源消耗，框架整体遵循以**极少的使用成本**开发游戏。  

## 支持版本
Unity 2021.3.15f1+  
系统支持：Win/Android/iOS/Mac/Linux

## 文档快速预览 - 1分钟
### ----------可选功能----------
* [1. 下载管理器](https://github.com/TippingGame/F8ExcelTool.git) - 待添加
* [2. 热更新版本管理](https://github.com/TippingGame/F8ExcelTool.git) - 待添加
### ----------核心功能----------
* [1. 配置表（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/ExcelTool/README.md) - 使用Excel作为配置表，兼顾**高性能、高适应性**，点击F8加载手动生成的 Excel 二进制缓存，运行时自动读取最新 Excel，无需频繁导表。
* [2. 资源加载（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README.md) - **编辑器下**：点击F8自动生成资产索引，AB名称，自动区分不同平台，清理多余AB和文件夹，Editor模式下减少开发周期。**运行时**：同步/异步加载单个资产，指定文件夹加载多个资产，自动判断是Resources/AssetBundle资产，加载Remote远程资产，获取加载进度，同步打断异步加载，相同资源同时加载。
* [3. 模块中心（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Module/README.md) - 模块中心可以获取所有模块的实例，延迟加载策略，自由控制生命周期。
* [4. 日志管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Log/README.md) - 打印日志，写入文件，上报错误。
* [5. 声音管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Audio/README.md) - 声音的播放，暂停，停止，进度控制，音量控制/保存，全局暂停/恢复。Audio分为三大类：**背景音乐、人声、特效声**。
* [6. 事件管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/README.md) - 发送消息事件，事件监听，防止消息死循环，自动释放事件。
* [7. 时间管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Timer/README.md) - 提供Timer、FrameTimer两种计时器，暂停/恢复，自动释放Timer。
* [8. 补间动画（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Tween/README.md) - 播放/终止动画，有旋转/位移/缩放/渐变/填充动画，为UI设计的相对布局位移动画。
* [9. 引用池管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/ReferencePool/README.md) - 引用池管理，C# 对象，入池/取出/回收/清空。
* [10. 游戏对象池（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/GameObjectPool/README.md) - 游戏对象池管理，Unity GameObject 预加载，生成/销毁/延迟销毁，生命周期事件监听。
* [11. 本地化管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Localization/README.md) - 本地化 Text / TextMeshPro / Image / RawImage / SpriteRenderer / Renderer / Audio / Timeline 等组件，使用Excel作为多语言翻译表
* [12. 有限状态机（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/FSM/README.md) - 通过继承有限状态机状态 FSMState 和状态切换 FSMSwitch，控制状态机，添加/切换/轮询/销毁。
* [13. UI界面管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/UI/README.md) - 处理界面加载，打开，关闭，查询，层级控制，自定义动画，自动获取组件索引。UI界面分为三大类：**普通UI、模态弹窗、非模态弹窗**。
* [14. 输入系统管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Input/README.md) - 使用同一套代码，通过自定义输入设备，适配多平台，可热切换输入设备。
* [15. 游戏流程管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Procedure/README.md) - 通过继承流程节点 ProcedureNode，控制游戏流程的，添加/运行/轮询/移除。
* [16. 本地数据存储（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/Storage/README.md) - 本地数据存储/读取。
* [17. SDK接入管理（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/SDKManager/README.md) - 与原生平台交互，接入多个平台或者渠道SDK，登录/退出/切换/支付/视频广告/退出游戏/Toast
* [18. 网络连接与通信（内置）](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README.md) - 待添加

## 使用步骤

### 导入

在 Unity 包管理器中，使用 Git URL 添加 F8 核心包。

1. 打开 Unity Editor

2. 点击菜单的 **Window** 项，再点击 **Package Manager** 子项

3. 点击左上角 **+** 号，选择 **Add Package from git URL**

4. 输入 <https://github.com/TippingGame/F8Framework.git>，请确认导入成功

#### 网络问题

若您的网络不佳，也可以下载核心包文件，直接放入工程里，或者修改 package.json 并放入 Packages 文件夹。

## 新手指南

* [游戏启动器：](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/GameLauncher.cs)游戏启动器示例。[GameLauncher.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/GameLauncher.cs)  
* [模块自定义改名：](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/FF8.cs)模块自定义改名。[FF8.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/FF8.cs)  

