# F8Localization

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8Localization本地化组件。
1. 通过继承模块 ModuleSingleton / ModuleSingletonMono，控制所有模块的，获取/初始化/轮询顺序/销毁。
2. 使用自定义模块 CustomModule

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 如何使用

1. 在 StreamingAssets/config 目录创建 LocalizedStrings.xlsx 作为本地化配置

### 代码使用方法
```C#
        /*----------------------------模块中心功能----------------------------*/
        
        //初始化模块中心
        ModuleCenter.Initialize(this);
        
        // 创建模块，（参数可选，优先级越小越早轮询）
        int priority = 100;
        ModuleCenter.CreateModule<TimerManager>(priority);
        
        // 通过ModuleCenter调用模块方法
        ModuleCenter.GetModule<TimerManager>().GetServerTime();
        
        // 通过获取实例调用模块方法
        TimerManager.Instance.GetServerTime();
        
        // 继承ModuleSingletonMono创建模块，按需添加Update特性
        [UpdateRefresh]
        [LateUpdateRefresh]
        [FixedUpdateRefresh]
        public class DemoModuleCenterMonoClass : ModuleSingletonMono<DemoModuleCenterMonoClass>, IModule
        {
            public void OnInit(object createParam)
            {
                // 模块创建初始化
            }
        
            public void OnUpdate()
            {
                // 模块Update
            }
        
            public void OnLateUpdate()
            {
                // 模块LateUpdate
            }
        
            public void OnFixedUpdate()
            {
                // 模块FixedUpdate
            }
        
            public void OnTermination()
            {
                // 模块销毁
                Destroy(gameObject);
            }
        }
        
        /*----------------------------自定义模块功能----------------------------*/
        
        // 获取所有模块，并调用进入游戏
        foreach (var center in CustomModule.GetSubCenter())
        {
            center.Value.OnEnterGame();
        }
        
        // 获取指定模块
        CustomModule demo = CustomModule.GetCenterByType(typeof(CustomModuleClass));
        
        // 使用模块
        CustomModuleClass.Instance.OnEnterGame();
        
        // 继承CustomModule的自定义模块
        public class CustomModuleClass : CustomModule
        {
            public static CustomModuleClass Instance => GetInstance<CustomModuleClass>();
            
            protected override void Init()
            {
                // 初始化CustomModule
            }
                
            public override void OnEnterGame()
            {
                // 进入游戏
            }
        
            public override void OnQuitGame()
            {
                // 退出游戏
            }
        }
```


