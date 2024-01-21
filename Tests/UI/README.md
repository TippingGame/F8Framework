# F8UIManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8UIManager界面管理组件，处理界面加载，打开，关闭，层级

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 初始化，创建模板，创建UI

1. 找到UIRoot预制体，放入场景中（需要自行添加Camera）  
2. 右键资源文件夹，看到（F8UI界面管理功能），创建模板  
3. 脚本挂载到UI根层级上  

### 代码使用方法
```C#
        /*----------UI管理功能----------*/
        
        // 初始化
        UIManager.Instance.Initialize(configs);
        // 打开UI，可选参数：data，new UICallbacks()
        UIManager.Instance.Open(1, data, new UICallbacks());
        UIManager.Instance.OpenAsync(1);
        // 打开提示类Notify
        UIManager.Instance.ShowNotify(1, "tip");
        // UI是否存在
        UIManager.Instance.Has(1);
        // 关闭UI，可选参数：isDestroy
        UIManager.Instance.Close(1, true);
        // 关闭所有UI（除了Notify类），可选参数：isDestroy
        UIManager.Instance.Clear(true);
        
        
        /*----------如何使用模板----------*/
        
public class UIMain : BaseView
{
    protected override void OnAwake()
    {
        // Awake
    }
        
    protected override void OnAdded(object[] args, string uuid)
    {
        // 参数传入
    }
    
    protected override void OnStart()
    {
        // Start
        object[] args = Args;
        string uuid = Uuid;
    }
    
    protected override void OnViewOpen()
    {
        // 打开界面动画完成后
    }
    
    protected override void OnBeforeRemove(object[] args, string uuid){
        // 删除之前
    }
    
    protected override void OnRemoved(object[] args, string uuid)
    {
        // 删除
    }
}
```


