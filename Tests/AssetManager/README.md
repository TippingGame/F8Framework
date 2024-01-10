# F8AssetManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8AssetManager资产加载组件，自动区分不同平台，同步/异步加载资产，自动判断是Resources资产还是AssetBundle资产，可加载Remote远程资产（可热修复指定资产）

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 初始化

1. 点击F8，自动获取Resources下的所有资产，生成索引（注意：资产名称唯一）  
          自动获取Assets/AssetBundles下的所有资产，生成索引（注意：资产名称唯一） 
          自动生成索引文件Assets/F8Framework/AssetMap目录下面  
          
2. 生成AssetBundles目录，自动赋予资产AB名称，打包AssetBundle，目录StreamingAssets/AssetBundles/Windows（不同平台例如iOS）  
          
3. 假如没有报错，就可以愉快地使用了  

### 代码使用方法
```C#
        //同步加载资产，自动判断是Resources资产还是AssetBundle资产
        GameObject go = AssetManager.Instance.Load<GameObject>("Cube");
        GameObject go2 = AssetManager.Instance.Load("Cube")as GameObject;
        GameObject go3 = AssetManager.Instance.Load("Cube", typeof(GameObject))as GameObject;
        GameObject go4 = Instantiate(go);
        
        //异步加载资产，自动判断是Resources资产还是AssetBundle资产
        AssetManager.Instance.LoadAsync<GameObject>("Cube", (go) =>
        {
            GameObject goo = Instantiate(go);
        });
        
        //同步加载远程AssetBundle资产，需要配置REMOTE_ADDRESS = "http://127.0.0.1:6789/remote"
        GameObject go5 = AssetManager.Instance.Load<GameObject>("Cube", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
        GameObject goo5 = Instantiate(go5);
        
        //异步加载远程AssetBundle资产
        AssetManager.Instance.LoadAsync<GameObject>("Cube", (go) =>
        {
            GameObject goo = Instantiate(go);
        }, AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
        
        
        //根据AssetName获取Ab映射属性
        AssetBundleMap.Mappings.TryGetValue("Cube", out AssetBundleMap.AssetMapping assetMapping);
        //获取加载进度
        float loadProgress = AssetManager.Instance.GetLoadProgress("Cube");
        //获取所有加载器的进度
        float loadProgress2 = AssetManager.Instance.GetLoadProgress();
        
        
        //同步卸载Resources或者AssetBundle资产
        AssetManager.Instance.Unload("Cube", false);//根据AbPath卸载资产，如果设置为 true，将卸载目标依赖的所有资源，
        //异步卸载AssetBundle资产
        AssetManager.Instance.UnloadAsync("Cube", false, () =>
        {
            //卸载资产完成
        });
```


