# F8 UIManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 UIManager界面管理组件，处理界面加载，打开，关闭，查询，层级控制，自定义动画，自动获取组件索引。  
UI界面分为三大类：  
1.普通UI  
2.模态弹窗（只显示最老的窗口，关闭后自动显示下一个新窗口）  
3.非模态弹窗（老窗口和新窗口共存，新的显示在前，自己管理窗口关闭）

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 初始化，创建UI，创建代码模板

1. 制作UI预制体，放到AssetBundles或者Resources文件夹下任意目录
2. 右键资源文件夹，看到（F8UI界面管理功能），创建BaseView模板，挂载到UI根层级上  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240302154254.png)
--------------------------
3. 生成UI组件的索引，可以在 [DefaultCodeBindNameTypeConfig.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/ComponentBind/DefaultCodeBindNameTypeConfig.cs) 中添加名称索引（可能需要点击两次）  
   ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240205223438.png)
### 代码使用方法
```C#
// UI的定义，枚举
public enum UIID
{
    UIMain = 1, // 游戏主界面
}
private Dictionary<UIID, UIConfig> configs = new Dictionary<UIID, UIConfig>
{
    // 兼容int和枚举作为Key
    { UIID.UIMain, new UIConfig(LayerType.UI, "UIMain") } // 手动添加UI配置
};

private object[] data = new object[] { 123123, "asdasd" };


IEnumerator Start()
{
    /*--------------------------UI管理功能--------------------------*/
    // 初始化（必须执行，兼容int和枚举作为Key的configs）
    FF8.UI.Initialize(configs);

    // 设置UI Canvas属性（如果不懂属性有什么用，可自建Canvas进行试验）
    // null代表设置所有Layer
    // sortOrder层级
    // sortingLayerName层级名称
    // RenderMode渲染模式
    // pixelPerfect像素模式
    // camera设置主相机
    FF8.UI.SetCanvas(null, 1, "Default", RenderMode.ScreenSpaceCamera, false, Camera.main);

    // 设置UI CanvasScaler属性（如果不懂属性有什么用，可自建Canvas进行试验）
    FF8.UI.SetCanvasScaler(null, CanvasScaler.ScaleMode.ConstantPixelSize, scaleFactor: 1f, referencePixelsPerUnit: 100f);
    FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ScaleWithScreenSize, referenceResolution: new Vector2(1920, 1080),
        CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, matchWidthOrHeight: 0f, referencePixelsPerUnit: 100f);
    FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ConstantPhysicalSize, CanvasScaler.Unit.Points,
        fallbackScreenDPI: 96f, defaultSpriteDPI: 100f, referencePixelsPerUnit: 100f);

    
    /*-------------------------------------同步加载-------------------------------------*/
    // 打开UI，兼容int和枚举，可选参数：data，new UICallbacks()
    FF8.UI.Open(UIID.UIMain, data, new UICallbacks(
        (parameters, id) => // onAdded
        {
            
        }, (parameters, id) => // OnRemoved
        {
            
        }, () => // OnBeforeRemove
        {
            
        }));
    // 也可以这样，guid是唯一ID
    string guid = FF8.UI.Open(1);
    
    
    /*-------------------------------------异步加载-------------------------------------*/
    // async/await方式（无多线程，WebGL也可使用）
    // await FF8.UI.OpenAsync(UIID.UIMain);
    // 或者
    // UILoader load = FF8.UI.OpenAsync(UIID.UIMain);
    // await load;
    // string guid2 = load.Guid;
    
    // 协程方式
    yield return FF8.UI.OpenAsync(UIID.UIMain);
    // 或者
    UILoader load2 = FF8.UI.OpenAsync(UIID.UIMain);
    yield return load2;
    string guid2 = load2.Guid;
    
    /*-------------------------------------其他功能-------------------------------------*/
    // 打开提示类Notify
    FF8.UI.ShowNotify(UIID.UIMain, "tip");
    FF8.UI.ShowNotify(1, "tip");
    // 异步加载
    // await FF8.UI.ShowNotifyAsync(UIID.UIMain, "tip");
    // yield return FF8.UI.ShowNotifyAsync(UIID.UIMain, "tip");
    
    // UI是否存在
    FF8.UI.Has(UIID.UIMain);
    FF8.UI.Has(1);
    
    // 根据UIid获取UI物体列表
    FF8.UI.GetByUIid(UIID.UIMain);
    FF8.UI.GetByUIid(1);
    
    // 根据guid获取UI物体
    FF8.UI.GetByGuid(guid);
    
    // 关闭UI，可选参数：isDestroy
    FF8.UI.Close(UIID.UIMain, true);
    FF8.UI.Close(1, true);
    
    // 关闭所有UI（除了Notify类），可选参数：isDestroy
    FF8.UI.Clear(true);
}


/*----------------------------如何使用模板----------------------------*/

public class UIMain : BaseView
{
    // Awake
    protected override void OnAwake()
    {
    }

    // 参数传入，每次打开UI都会执行
    protected override void OnAdded(int uiId, object[] args = null)
    {
    }

    // Start
    protected override void OnStart()
    {
    }

    protected override void OnViewTweenInit()
    {
        //transform.localScale = Vector3.one * 0.7f;
    }

    // 自定义打开界面动画
    protected override void OnPlayViewTween()
    {
        //transform.ScaleTween(Vector3.one, 0.1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen);
    }

    // 打开界面动画完成后
    protected override void OnViewOpen()
    {
    }

    // 删除之前，每次UI关闭前调用
    protected override void OnBeforeRemove()
    {
    }

    // 删除，每次UI关闭后调用
    protected override void OnRemoved()
    {
    }

    // 自动获取组件（自动生成，不能删除）

    // 自动获取组件（自动生成，不能删除）
}
```
## 拓展功能
1. 编辑器功能
* 1. 图片自动切割九宫格（将图片中间部分相同颜色切除，减少图片体积）
* 2. 图集切割（需要预先点击 Sprite Editor 的 Slice 切分图片，图片 Read/Write 勾选，压缩等级设置为None）
* 3. 图片尺寸设为4的倍数（更适合图片压缩优化）
* 4. 收集UI所有的中文放入本地化表
     ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240315025120.png)
----------------------------------
2. 常用功能
* 1.图片圆角遮罩 [SimpleRoundedImage.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/Mask/SimpleRoundedImage.cs)
* 2.UI安全区刘海防遮挡 [SafeAreaAdapter.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/UIAdapter/SafeAreaAdapter.cs)
* 3.粒子特效在UI上显示 [UIParticleSystem.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/UIParticleSystem/UIParticleSystem.cs)
* 4.红点系统 [UIRedDot.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/UIRedDot/UIRedDot.cs)
* 5.Sprite序列帧动画 [SpriteSequenceFrame.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/UI/SequenceFrame/SpriteSequenceFrame.cs)
----------------------------------
3. UI常用组件：[https://github.com/nhn/gpm.unity.git](https://github.com/nhn/gpm.unity.git)（已内置，参考目录：F8Framework/Tests/UI/Example）如要使用Tests目录下的示例，请在 Project Setting -> Player -> Script Compilation 处添加宏定义 BUILD_F8FRAMEWORK_TEST
* 嵌套布局  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240302173446.png)
----------------------------------
* 无限列表  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240302173458.png)
----------------------------------
* 拖拽  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240302173503.png)
----------------------------------
* Tab标签页  
  ![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240302173507.png)
----------------------------------

