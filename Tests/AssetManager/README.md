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
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205230012_2.png)
---------------------------------
2. Assets / AssetBundles目录下的文件，自动赋予资产AB名称（**注意**：已有AB名不会覆盖，不想资源继续打包AB，注意手动清空AB名）  
   打包 AssetBundle 后，在目录 StreamingAssets / AssetBundles / Windows 生成AB包（不同平台例如 Windows 或 iOS ，**注意**：如果没有加载任何AB包时，请手动清空此目录）  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240205225815.png)
---------------------------------
3. **注意**：编辑器下加载不同平台的AB包（Android平台，iOS平台，WebGL平台），Shader会变紫色，Scene会加载失败，音频加载失败等（解决方案：启用编辑器模式）
---------------------------------
4. **注意**：如何启用编辑器模式，代码中调用：FF8.Asset.IsEditorMode = true  
   或者在编辑器中勾选编辑器模式：  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20251736474182.png)
---------------------------------
5. **注意**：Unity的WebGL平台不能使用同步加载AB包，可以使用Resources进行同步加载
---------------------------------
6. 假如没有报错，就可以愉快地使用了

### 代码使用方法
```C#
IEnumerator Start()
{
    /*----------所有加载均会自动判断是Resources资产还是AssetBundle资产----------*/
    
    // 编辑器模式，无需每次修改资源都按F8
    FF8.Asset.IsEditorMode = true;
    
    
    /*-------------------------------------同步加载-------------------------------------*/
    GameObject go = FF8.Asset.Load<GameObject>("Cube");

    // assetName：资产名
    // subAssetName：子资产名，使用Multiple模式的Sprite图片则可使用
    // 指定加载模式REMOTE_ASSET_BUNDLE，加载远程AssetBundle资产，需要配置AssetRemoteAddress = "http://127.0.0.1:6789/remote"
    Sprite sprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12", AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);
    
    
    /*-------------------------------------异步加载-------------------------------------*/
    FF8.Asset.LoadAsync<GameObject>("Cube", (go) =>
    {
        GameObject goo = Instantiate(go);
    });

    // async/await方式（无多线程，WebGL也可使用）
    // BaseLoader load = FF8.Asset.LoadAsync<GameObject>("Cube");
    // await load;
    
    // 协程方式
    BaseLoader load2 = FF8.Asset.LoadAsync<GameObject>("Cube");
    yield return load2;
    GameObject go2 = load2.GetAssetObject<GameObject>();
    
    
    /*-------------------------------------加载文件夹内首层资产-------------------------------------*/
    // 加载文件夹内首层资产（不遍历所有文件夹）
    FF8.Asset.LoadDir("NewFolder");
    
    // async/await方式（无多线程，WebGL也可使用）
    // BaseDirLoader loadDir = FF8.Asset.LoadDirAsync("NewFolder", () => { });
    // await loadDir;
    
    // 加载文件夹内资产
    BaseDirLoader loadDir2 = FF8.Asset.LoadDirAsync("NewFolder", () => { });
    yield return loadDir2;
    
    // 你可以查看所有资产的BaseLoader
    List<BaseLoader> loaders = loadDir2.Loaders;
    
    // 也可以这样设置查看加载进度
    foreach (var item in FF8.Asset.LoadDirAsyncCoroutine("NewFolder"))
    {
        yield return item;
    }

    // 也可以这样
    var loadDir3 = FF8.Asset.LoadDirAsyncCoroutine("NewFolder").GetEnumerator();
    while (loadDir3.MoveNext())
    {
        yield return loadDir3.Current;
    }
    
    
    /*-------------------------------------其他功能-------------------------------------*/
    // 获取资产
    GameObject go3 = FF8.Asset.GetAssetObject<GameObject>("Cube");
    
    // 获取加载进度
    float loadProgress = FF8.Asset.GetLoadProgress("Cube");

    // 获取所有加载器的进度
    float loadProgress2 = FF8.Asset.GetLoadProgress();

    // 同步卸载资产
    FF8.Asset.Unload("Cube", false); //根据AbPath卸载资产，如果设置为 true，完全卸载。

    // 异步卸载资产
    FF8.Asset.UnloadAsync("Cube", false, () =>
    {
        // 卸载资产完成
    });
    
    
    /*-------------------------------------其他类型加载示例-------------------------------------*/
    // 加载场景，别忘了加载天空盒材质，不然会变紫色
    FF8.Asset.Load("Scene");
    SceneManager.LoadScene("Scene");
    
    // 使用图集首先需要，加载图集
    FF8.Asset.Load("SpriteAtlas");
    
    // 假如将图集与图片改为同一AB名，则无需预先加载图集
    FF8.Asset.LoadAsync<Sprite>("PackForest_2", sprite =>
    {
        LogF8.Log(sprite);
    });
    
    // 图片加载需要小心区分Texture2D和Sprite，当资源被当成Texture2D加载后，则加载不出Sprite类型
    FF8.Asset.Load<Texture2D>("PackForest_2");
}
```

### 编辑器拓展功能
#### 你应该注意每次移出AssetBundles目录的文件，AB名都需要手动清空
1. 编辑器功能
* 1. 寻找资源是否被引用（将搜索全工程文件，输出Log信息）
* 2. 清空所有选中的资产AB名（可选择多个文件，也可以选中文件夹）
* 3. 设置选中的所有资产为相同Ab名（可选择多个文件，也可以选中文件夹）
* 4. 全局空引用查找（打开窗口，显示引用丢失的资产）
     ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240216212631_2.png)

#### 你可以在资源栏 **鼠标指着** 文件/文件夹并按下 **键盘空格键** ，跳转到系统资源管理器：如windows文件夹
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20241112212631.png)  
