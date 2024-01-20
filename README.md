# <strong>F8Framework

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) [![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) [![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

> F8 框架初衷：希望自己点击 F8，就能开始制作游戏，不想多余的事。
> 
> F8 Framework original intention: Just click F8 and start making the game, don't want to be redundant.

## 简介
#### F8Framework是一个优雅的，符合直觉的Unity框架全平台解决方案，可根据自己需要选择组件，组件围绕F8一键启动。  
系统支持：Win/Android/iOS/Mac/Linux  

## 文档快速预览 - 3分钟
### ----------可选功能----------  
* [1.配置表：](https://github.com/TippingGame/F8ExcelTool.git)配置表模块文档。https://github.com/TippingGame/F8ExcelTool.git  
* [2.热更新版本号管理：]待添加  
### ----------核心功能----------
* [1.资源加载（内置）：](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README.md)资源模块文档。[README.md](https://github.com/TippingGame/F8Framework/blob/main/Tests/AssetManager/README.md)  
* [2.UI界面管理（内置）：](https://github.com/TippingGame/F8Framework/blob/main/Tests/UI/README.md)UI界面模块文档。[README.md](https://github.com/TippingGame/F8Framework/blob/main/Tests/UI/README.md)  
* [3.模块中心管理（内置）：](https://github.com/TippingGame/F8Framework/blob/main/Tests/Module/README.md)模块中心文档。[README.md](https://github.com/TippingGame/F8Framework/blob/main/Tests/Module/README.md)  
* [4.日志管理（内置）：](https://github.com/TippingGame/F8Framework/blob/main/Tests/Log/README.md)日志模块文档。[README.md](https://github.com/TippingGame/F8Framework/blob/main/Tests/Log/README.md)  
* [5.声音管理（内置）：](https://github.com/TippingGame/F8Framework/blob/main/Tests/Audio/README.md)音频模块文档。[README.md](https://github.com/TippingGame/F8Framework/blob/main/Tests/Audio/README.md)  
* [6.事件管理（内置）：](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/README.md)事件模块文档。[README.md](https://github.com/TippingGame/F8Framework/blob/main/Tests/Event/README.md)  
* [7.时间管理（内置）：](https://github.com/TippingGame/F8Framework/blob/main/Tests/Timer/README.md)时间模块文档。[README.md](https://github.com/TippingGame/F8Framework/blob/main/Tests/Timer/README.md)  
* [8.实体组件（内置）：]待添加  
* [9.网络连接与通信（内置）：]待添加  
* [10.SDK接入（内置）：]待添加  

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

* [游戏启动器：](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/GameLauncher.cs)游戏启动器。[GameLauncher.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/GameLauncher.cs)  
* [模块自定义改名：](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/FF8.cs)模块自定义改名。[FF8.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Launcher/FF8.cs)  

