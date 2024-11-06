# F8 Network

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Network(互联)网络组件。
1. 使用 KCP / TCP / WebSocket 网络通讯协议建立长连接通道，支持Client端和Server端。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 使用方法
* Client使用
  1. kcp 或 tcp 示例 [MultiNetworkClient.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/MultiNetworkChannel/MultiNetworkClient.cs)
  2. websocket 示例 [ExampleWebClient.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/SimpleWebTransport/ExampleWebClient.cs)
------------------------------------------
* Server使用
  1. kcp 或 tcp 示例 [MultiNetworkServer.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/MultiNetworkChannel/MultiNetworkServer.cs)
  2. websocket 示例 [ExampleWebServer.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/SimpleWebTransport/ExampleWebServer.cs)  

