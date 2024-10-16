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
        private Dictionary<int, UIConfig> configs = new Dictionary<int, UIConfig>
        {
            { 1, new UIConfig(LayerType.UI, "UITip") },
            // 手动添加UI配置
        };
        private object[] data = new object[] { 123123, "asdasd" };
        
        /*----------------------------UI管理功能----------------------------*/
        
        // 初始化
        FF8.UI.Initialize(configs);
        // 设置UI Canvas属性
        FF8.UI.SetCanvas(null, 1, "Default", RenderMode.ScreenSpaceCamera, false, Camera.main);
        // 设置UI CanvasScaler属性
        FF8.UI.SetCanvasScaler(null, CanvasScaler.ScaleMode.ConstantPixelSize, 1f, 100f);
        FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ScaleWithScreenSize, new Vector2(1920, 1080), CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, 0f, 100f);
        FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ConstantPhysicalSize, CanvasScaler.Unit.Points, 96f, 100f, 100f);
        // 打开UI，可选参数：data，new UICallbacks()
        FF8.UI.Open(1, data, new UICallbacks());
        FF8.UI.OpenAsync(1);
        // 打开提示类Notify
        FF8.UI.ShowNotify(1, "tip");
        // UI是否存在
        FF8.UI.Has(1);
        // 关闭UI，可选参数：isDestroy
        FF8.UI.Close(1, true);
        // 关闭所有UI（除了Notify类），可选参数：isDestroy
        FF8.UI.Clear(true);
        
        
        /*----------------------------如何使用模板----------------------------*/
        
        public class UIMain : BaseView
        {
            protected override void OnAwake()
            {
                // Awake
            }
                
            protected override void OnAdded(object[] args, int uiId)
            {
                // 参数传入
            }
            
            protected override void OnStart()
            {
                // Start
                object[] args = Args;
                int uiId = UIid;
            }
            
            protected override void OnViewTweenInit()
            {
                //transform.localScale = Vector3.one * 0.7f;
            }
            
            protected override void OnPlayViewTween()
            {
                // 打开界面动画，可自行添加关闭界面动画
                //transform.ScaleTween(Vector3.one, 0.1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen);
            }
            
            protected override void OnViewOpen()
            {
                // 打开界面动画完成后
            }
            
            protected override void OnBeforeRemove(){
                // 删除之前
            }
            
            protected override void OnRemoved()
            {
                // 删除
            }
            
            // 自动获取组件（自动生成，不能删除）
    
            // 自动获取组件（自动生成，不能删除）
        }
```
## 拓展功能
1. 编辑器功能
* 1. 图片自动切割九宫格
* 2. 图集切割
* 3. 图片尺寸设为4的倍数
* 4. 收集UI所有的中文放入本地化表
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/UI/ui_20240315025120.png)
----------------------------------
2. UI常用组件：[https://github.com/nhn/gpm.unity.git](https://github.com/nhn/gpm.unity.git)（已内置，参考目录：F8Framework/Tests/UI/Example）  
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

