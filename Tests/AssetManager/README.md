# F8 AssetManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）

**Unity F8 AssetManager资产加载组件**
1. 编辑器下:
   * 点击F8自动生成资产索引/AB名称
   * 自动区分不同平台
   * 清理多余AB和文件夹
   * 编辑器模式下减少开发周期
2. 运行时:
   * 同步/异步加载单个资产
   * 展开文件夹或同一AB下所有资产
   * 自动判断是 Resources / AssetBundle 资产
   * 加载Remote远程资产
   * 获取加载进度
   * 同步打断异步加载
3. AssetBundle可以这样加载:
   * 单个资产单个AB
   * 指定文件夹名称（文件夹第一层的AB）
   * 设置多个资产为同一AB名（指定任意资产名）
4. 注意AB资产地址(大小写不敏感)

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 视频教程：[【Unity框架】（2）资源加载](https://www.bilibili.com/video/BV1WZ421x7TP)

### 初始化

1. 点击F8，自动获取 Resources 下的所有资产，生成索引  
   自动获取 Assets / AssetBundles 下的所有资产，生成索引  
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
6. **注意**：如要使用同名资源，可启用完整资源路径加载，点击F5打包工具勾选对应功能  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_1761979618779.png)
7. 假如没有报错，就可以愉快地使用了

### 代码使用方法
```C#
IEnumerator Start()
{
    /*========== 基础配置 ==========*/
    // 启用编辑器模式（无需每次修改资源都按F8），也可在菜单栏勾选
    FF8.Asset.IsEditorMode = true;
    

    /*========== 1. 同步加载 ==========*/

    // 基础加载 - 自动识别 Resources 或 AssetBundle
    GameObject cube = FF8.Asset.Load<GameObject>("Cube");

    // 完整路径加载（需在 F5 打包工具中启用对应功能）
    GameObject prefab1 = FF8.Asset.Load<GameObject>("AssetBundles/Prefabs/Cube");
    GameObject prefab2 = FF8.Asset.Load<GameObject>("Resources/Prefabs/Cube.prefab");

    // 加载子资源（如 使用 Multiple 模式的 Sprite 图片）
    Sprite sprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12");

    // 强制远程加载模式，需在 F5 打包工具配置远程资源地址
    Sprite remoteSprite = FF8.Asset.Load<Sprite>("PackForest01", "PackForest01_12",
        AssetManager.AssetAccessMode.REMOTE_ASSET_BUNDLE);


    /*========== 2. 异步加载 ==========*/

    // 回调方式
    FF8.Asset.LoadAsync<GameObject>("Cube", (go) => { Instantiate(go); });

    // 协程方式
    yield return FF8.Asset.LoadAsync<GameObject>("Cube");

    // 或获取加载器控制
    BaseLoader loader = FF8.Asset.LoadAsync<GameObject>("Cube");
    yield return loader;
    GameObject result = loader.GetAssetObject<GameObject>();

    // async/await 方式（WebGL 兼容）
    await FF8.Asset.LoadAsync<GameObject>("Cube");


    /*========== 3. 批量加载 ==========*/

    // 同步加载文件夹
    FF8.Asset.LoadDir("UI/Prefabs");

    // 异步加载文件夹 - 回调方式
    FF8.Asset.LoadDirAsync("UI/Prefabs", () => { LogF8.Log("所有UI资源加载完成"); });

    // 异步加载文件夹 - 协程方式
    BaseDirLoader dirLoader = FF8.Asset.LoadDirAsync("UI/Prefabs");
    yield return dirLoader;

    // 遍历加载进度
    foreach (var progress in FF8.Asset.LoadDirAsyncCoroutine("UI/Prefabs"))
    {
        LogF8.Log($"加载进度: {progress}");
        yield return progress;
    }

    // async/await 方式（WebGL 兼容）
    await FF8.Asset.LoadDirAsync("UI/Prefabs");
    
    // 加载此资源的全部资产
    FF8.Asset.LoadAll("Cube");
    BaseLoader loaderAll = FF8.Asset.LoadAllAsync("Cube");
    
    // 加载此资源的全部子资产
    FF8.Asset.LoadSub("Atlas");
    BaseLoader loaderSub = FF8.Asset.LoadSubAsync("Atlas");

    
    /*========== 4. 场景加载 ==========*/

    // 同步加载场景
    FF8.Asset.LoadScene("MainScene");

    // 异步加载场景
    SceneLoader sceneLoader = FF8.Asset.LoadSceneAsync("MainScene", LoadSceneMode.Single);
    yield return sceneLoader;

    // 手动控制场景激活
    SceneLoader sceneLoader2 = FF8.Asset.LoadSceneAsync("MainScene", new LoadSceneParameters(LoadSceneMode.Single),
        allowSceneActivation: false);
    yield return new WaitForSeconds(2);
    sceneLoader2.AllowSceneActivation();


    /*========== 5. 资源管理 ==========*/

    // 获取已加载资源
    GameObject cachedCube = FF8.Asset.GetAssetObject<GameObject>("Cube");

    // 获取所有子资源
    Dictionary<string, Object> allAssets = FF8.Asset.GetAllAssetObject("Atlas");
    Dictionary<string, Sprite> allSprites = FF8.Asset.GetAllAssetObject<Sprite>("Atlas");

    // 获取加载进度
    float assetProgress = FF8.Asset.GetLoadProgress("Cube"); // 单个资源
    float totalProgress = FF8.Asset.GetLoadProgress(); // 所有资源

    // 资源卸载
    FF8.Asset.Unload("Cube", false); // 保留依赖
    FF8.Asset.Unload("Cube", true); // 完全卸载

    // 异步卸载
    FF8.Asset.UnloadAsync("Cube", false, () => { LogF8.Log("资源卸载完成"); });
    
    
    /*========== 6. 注意：常见问题 ==========*/
    
    // 编辑器下加载不同平台的AB包（Android平台，iOS平台，WebGL平台），Shader会变紫色，Scene会加载失败，音频加载失败等（解决方案：启用编辑器模式）
    
    // 加载场景，别忘了加载天空盒材质，不然会变紫色，并且不能加载Resources目录中的场景（需要手动放入Build Setting处）
    
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
#### [如何使用多进程加速构建AB包](https://docs.unity3d.com/6000.1/Documentation/Manual/Build-MultiProcess.html)
升级到 Unity6000 版本  
Project Settings -> Editor -> Build Pipeline -> Multi-Process AssetBundle Building（勾选）  
#### 资产状态检查器
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20250523001_2.png)
#### 你应该注意每次移出AssetBundles目录的文件，AB名都需要手动清空
1. 编辑器功能  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20240216212631_2.png)
* 1. 寻找资源是否被引用（将搜索全工程文件，输出Log信息）  
     ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_1761979131320.png)
* 2. 清空所有选中的资产AB名（可选择多个文件，也可以选中文件夹）  
* 3. 设置选中的所有资产为相同Ab名（可选择多个文件，也可以选中文件夹）  
* 4. 全局空引用查找（打开窗口，显示引用丢失的资产）  
     ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_1761979410848.png)

#### 你可以在资源栏 **鼠标指着** 文件/文件夹并按下 **键盘空格键** ，跳转到系统资源管理器：如windows文件夹
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/AssetManager/ui_20241112212631.png)  
