# F8 Module

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## Introduction (The goal is to start making games by pressing F8, without dealing with extra overhead)
The Unity F8 Module is the central module management component. It provides three types of modules:
1. Game modules: `Module` / `ModuleMono`, with delayed loading, controllable execution order, and centralized management for getting, initializing, polling, and destroying modules.
2. Static modules: `StaticModule`, initialized together when used, with no execution order, and providing `OnEnterGame` / `OnQuitGame` methods.
3. Activity modules: `ActivityModule`, initialized together when used, with no execution order. In addition to the static module methods, they also provide methods for refreshing state, reacting to state changes, checking whether unlocked, visible, open, and adding conditions.

## Import the Plugin (the core package must be imported first)
Note: This functionality is built into the F8Framework core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download the files directly and place them into Unity.  
Method 2: In Unity, click the menu bar -> `Window` -> `Package Manager` -> click the `+` button -> `Add Package from git URL` -> enter: `https://github.com/TippingGame/F8Framework.git`

### Video Tutorial: [【Unity Framework】(3) Module Management](https://www.bilibili.com/video/BV1Sr421F7Vw)

### Create Templates

1. Right-click the Assets folder, find `F8 Module Center Features`, and create a template: `Module` / `ModuleMono` / `StaticModule` / `ActivityModule`  
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/Module/ui_20240302154204_2.png)

### How to Use in Code
```C#
/*----------------------------Module Center Features----------------------------*/

// Initialize the module center
ModuleCenter.Initialize(this);

// Create a module (parameter optional; smaller priority values are updated earlier)
int priority = 100;
ModuleCenter.CreateModule<TimerManager>(priority);

// Call module methods through ModuleCenter
ModuleCenter.GetModule<TimerManager>().GetServerTime();

// Call module methods through the instance
TimerManager.Instance.GetServerTime();

// Inherit from ModuleSingletonMono to create a module, and add Update attributes as needed
[UpdateRefresh]
[LateUpdateRefresh]
[FixedUpdateRefresh]
public class DemoModuleCenterClass : ModuleSingleton<DemoModuleCenterClass>, IModule
{
    public void OnInit(object createParam)
    {
        // Module creation initialization
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
        // Module destruction
        Destroy(gameObject);
    }
}

/*----------------------------Custom Static Module Features----------------------------*/

// Get all static modules and call enter game
StaticModule.EnterGameAllModules();

// Get a specific static module
StaticModule demo = StaticModule.GetStaticModuleByType(typeof(StaticModuleClass));

// Use a static module
StaticModuleClass.Instance.OnEnterGame();

// A custom module that inherits from StaticModule
public class StaticModuleClass : StaticModule
{
    public static StaticModuleClass Instance => GetInstance<StaticModuleClass>();
    
    protected override void Init()
    {
        // Initialize StaticModule
    }
        
    public override void OnEnterGame()
    {
        // Enter game
    }

    public override void OnQuitGame()
    {
        // Quit game
    }
}

/*----------------------------Custom Activity Module Features----------------------------*/

// APIs for controlling all activity modules
ActivityModule.EnterGameAllModules();
ActivityModule.RefreshAllModules();
ActivityModule.QuitGameAllModules();
ActivityModule.ReleaseAllModules();

public class ActivityModuleClass : ActivityModule
{
    public static ActivityModuleClass Instance => GetInstance<ActivityModuleClass>();

    // Initialize the activity module
    protected override void Init()
    {
    }

    // Enter game
    public override void OnEnterGame()
    {
    }

    // Quit game
    public override void OnQuitGame()
    {
    }

    // Release the module
    protected override void OnDispose()
    {
    }

    // State changed
    protected override void OnStateChanged(ActivityModuleState previousState, ActivityModuleState currentState)
    {
    }

    // Unlock state changed
    protected override void OnUnlockStateChanged(bool previousValue, bool currentValue)
    {
    }

    // Visible state changed
    protected override void OnVisibleStateChanged(bool previousValue, bool currentValue)
    {
    }

    // Open state changed
    protected override void OnOpenStateChanged(bool previousValue, bool currentValue)
    {
    }

    // Unlock condition (custom logic required)
    protected override bool EvaluateUnlockedCore()
    {
        return false;
    }

    // Visibility condition (custom logic required)
    protected override bool EvaluateVisibleCore(bool isUnlocked)
    {
        return false;
    }

    // Open condition (custom logic required)
    protected override bool EvaluateOpenCore(bool isUnlocked)
    {
        return false;
    }
}
```
