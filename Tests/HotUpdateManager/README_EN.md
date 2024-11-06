# F8 HotUpdate

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 HotUpdate 热更新版本管理，负责打包，分包，热更新资源。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 编辑器界面使用

* 如何设置分包资源  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/HotUpdateManager/ui_20240323173756.png)
--------------------------
* 选择打包平台，输出路径，版本号，远程资产加载地址，启用热更新，全量打包，分包，空包。  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/HotUpdateManager/ui_20240317214323.png)
--------------------------

### 如构建失败：请尝试使用Unity自带的Build一次后再尝试

--------------------------
* 构建后将文件放入CDN服务器  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/HotUpdateManager/ui_20240323173827.png)
--------------------------
### 代码使用方法
```C#
        IEnumerator Start()
        {
            // 初始化本地版本
            FF8.HotUpdate.InitLocalVersion();

            // 初始化远程版本
            yield return FF8.HotUpdate.InitRemoteVersion();
            
            // 初始化资源版本
            yield return FF8.HotUpdate.InitAssetVersion();
            
            // 检查需要热更的资源，总大小
            Tuple<List<string>, long> result  = FF8.HotUpdate.CheckHotUpdate();
            var hotUpdateAssetUrl = result.Item1;
            var allSize = result.Item2;
            
            // 资源热更新
            FF8.HotUpdate.StartHotUpdate(hotUpdateAssetUrl, () =>
            {
                LogF8.Log("完成");
            }, () =>
            {
                LogF8.Log("失败");
            }, progress =>
            {
                LogF8.Log("进度：" + progress);
            });

            // 检查未加载的分包
            List<string> subPackage = FF8.HotUpdate.CheckPackageUpdate(GameConfig.LocalGameVersion.SubPackage);
            
            // 分包加载
            FF8.HotUpdate.StartPackageUpdate(subPackage, () =>
            {
                LogF8.Log("完成");
            }, () =>
            {
                LogF8.Log("失败");
            }, progress =>
            {
                LogF8.Log("进度：" + progress);
            });
        }
```