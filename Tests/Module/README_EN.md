# F8 Module

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Module Center Component**  
A modular architecture system that provides three distinct module types for game development:
* Lazy Loading: Modules load only when needed
* Execution Order Control: Precise initialization sequence management
* Lifecycle Management: Full control over:
  * Acquisition
  * Initialization
  * Update cycles
  * Destruction

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Module Creation Templates

1. Right-click in the Assets folder and select (F8 Module Center Functions) to create templates for:  
    * Module
    * ModuleMono
    * StaticModule

![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Module/ui_20240302154204.png)  

### Code Examples
```C#
/*----------------------------Module Center Functions----------------------------*/

// Initialize module center
ModuleCenter.Initialize(this);

// Create module (priority parameter optional - lower values update earlier)
int priority = 100;
ModuleCenter.CreateModule<TimerManager>(priority);

// Access module methods through ModuleCenter
ModuleCenter.GetModule<TimerManager>().GetServerTime();

// Alternative access via singleton instance
TimerManager.Instance.GetServerTime();

// Creating a module by inheriting ModuleSingletonMono
[UpdateRefresh]
[LateUpdateRefresh]
[FixedUpdateRefresh]
public class DemoModuleCenterClass : ModuleSingleton<DemoModuleCenterClass>, IModule
{
    public void OnInit(object createParam)
    {
        // Module initialization
    }

    public void OnUpdate()
    {
        // Module Update
    }

    public void OnLateUpdate()
    {
        // Module LateUpdate
    }

    public void OnFixedUpdate()
    {
        // Module FixedUpdate
    }

    public void OnTermination()
    {
        // Module cleanup
        Destroy(gameObject);
    }
}

/*----------------------------Custom Static Module Functions----------------------------*/

// Get all static modules and trigger game entry
foreach (var center in StaticModule.GetStaticModule())
{
    center.Value.OnEnterGame();
}

// Get specific static module
StaticModule demo = StaticModule.GetStaticModuleByType(typeof(StaticModuleClass));

// Using static module
StaticModuleClass.Instance.OnEnterGame();

// Custom static module implementation
public class StaticModuleClass : StaticModule
{
    public static StaticModuleClass Instance => GetInstance<StaticModuleClass>();
    
    protected override void Init()
    {
        // StaticModule initialization
    }
        
    public override void OnEnterGame()
    {
        // Game entry logic
    }

    public override void OnQuitGame()
    {
        // Game exit logic
    }
}
```


