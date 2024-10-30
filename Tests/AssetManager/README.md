# F8 AssetManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 AssetManager资产加载组件。  
1. 编辑器下：点击F8自动生成资产索引/AB名称，自动区分不同平台，清理多余AB和文件夹，Editor模式下减少开发周期。  
2. 运行时：同步/异步加载单个资产，展开文件夹或同一AB下所有资产，自动判断是 Resources / AssetBundle 资产，加载Remote远程资产，获取加载进度，同步打断异步加载。
3. AssetBundle可以这样加载：1. 单个资产单个AB 2. 指定文件夹名称（文件夹第一层的AB） 3. 设置多个资产为同一AB名（指定任意资产名）
4. 注意AB资产地址(大小写不敏感)，文件和目录名需要保证唯一。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 初始化

1. 点击F8，自动获取 Resources 下的所有资产，生成索引（注意：资产名称唯一）  
          自动获取 Assets / AssetBundles 下的所有资产，生成索引（注意：资产名称唯一）  
          自动生成索引文件 Assets / F8Framework / AssetMap 目录下面  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205225637.png)
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205230012.png)
---------------------------------
2. 生成AssetBundles目录，自动赋予资产AB名称（已有AB名不会覆盖），打包 AssetBundle，目录 StreamingAssets / AssetBundles / Windows（不同平台例如 Windows / iOS ）  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205225815.png)
---------------------------------
3. 假如没有报错，就可以愉快地使用了（注意：编辑器下加载不同平台的AB，Shader会变紫色，Scene也会加载失败（解决方案：FF8.Asset.IsEditorMode = true））  

### 代码使用方法
```C#
    IEnumerator Start()
    {
        /*----------所有加载均会自动判断是Resources资产还是AssetBundle资产----------*/
        
        /*-------------------------------------同步加载-------------------------------------*/
        GameObject go = FF8.Asset.Load<GameObject>("Cube");
        
        // 指定加载模式REMOTE_ASSET_BUNDLE，加载远程AssetBundle资产，需要配置AssetRemoteAddress = "http://127.0.0.1:6789/remote"
        GameObject go5 = FF8.Asset.Load<GameObject>("Cube", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
        
        // 加载文件夹内资产（不遍历所有文件夹）
        FF8.Asset.LoadDir("NewFolder");
        
        
        /*-------------------------------------异步加载-------------------------------------*/
        
        FF8.Asset.LoadAsync<GameObject>("Cube", (go) =>
        {
            GameObject goo = Instantiate(go);
        });
        
        // 协程
        var load = FF8.Asset.LoadAsyncCoroutine<GameObject>("Cube");
        yield return load;
        GameObject go2 = FF8.Asset.GetAssetObject<GameObject>("Cube");
        
        // 加载文件夹内资产
        FF8.Asset.LoadDirAsync("NewFolder", () =>
        {
            
        });
        
        // 协程，迭代文件夹内资产（不遍历所有文件夹）
        foreach(var item in FF8.Asset.LoadDirAsyncCoroutine("NewFolder"))
        {
            yield return item;
        }
        // 也可以这样
        var loadDir = FF8.Asset.LoadDirAsyncCoroutine("NewFolder").GetEnumerator();
        while (loadDir.MoveNext())
        {
            yield return loadDir.Current;
        }
        
        
        /*-------------------------------------其他功能-------------------------------------*/
        
        // 获取加载进度
        float loadProgress = FF8.Asset.GetLoadProgress("Cube");
        
        // 获取所有加载器的进度
        float loadProgress2 = FF8.Asset.GetLoadProgress();
        
        // 同步卸载资产
        FF8.Asset.Unload("Cube", false);//根据AbPath卸载资产，如果设置为 true，完全卸载。
        
        // 步卸载资产
        FF8.Asset.UnloadAsync("Cube", false, () =>
        {
            // 卸载资产完成
        });
        
        // 编辑器模式，无需打包AB
        FF8.Asset.IsEditorMode = true;
        
        // 加载场景，别忘了加载天空盒材质，不然会变紫色
        FF8.Asset.Load("Scene");
        SceneManager.LoadScene("Scene");
        
        // 使用图集首先需要，加载图集
        FF8.Asset.Load("SpriteAtlas");
    }
```

### 编辑器拓展功能
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240216212631.png)
