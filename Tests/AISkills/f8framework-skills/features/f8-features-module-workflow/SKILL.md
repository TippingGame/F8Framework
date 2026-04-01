---
name: f8-features-module-workflow
description: Use when implementing or troubleshooting Module feature workflows — ModuleCenter lifecycle, module creation, StaticModule, update attributes in F8Framework.
---

# Module Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first before any `ModuleCenter.CreateModule<T>()` call.


## Use this skill when

- The task is about creating custom modules, managing module lifecycle, or using StaticModule.
- The user asks about ModuleCenter, module initialization order, or update loops.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Module/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Module
- Editor module: Assets/F8Framework/Editor/Module
- Test docs: Assets/F8Framework/Tests/Module

## Key classes and interfaces

| Class | Role |
|-------|------|
| `ModuleCenter` | Core lifecycle management. Initialize, Create, Get, Destroy modules. |
| `ModuleSingleton<T>` | Non-MonoBehaviour module base with singleton. |
| `ModuleSingletonMono<T>` | MonoBehaviour module base with singleton. |
| `IModule` | Module interface: `OnInit`, `OnUpdate`, `OnLateUpdate`, `OnFixedUpdate`, `OnTermination`. |
| `StaticModule` | Auto-initialized static modules with `OnEnterGame`/`OnQuitGame`. |

## API quick reference

### ModuleCenter usage
```csharp
// Initialize with MonoBehaviour driver
ModuleCenter.Initialize(this);

// Create module with optional priority (lower = earlier update)
ModuleCenter.CreateModule<TimerManager>(100);

// Access module
ModuleCenter.GetModule<TimerManager>().GetServerTime();
// Or via singleton
TimerManager.Instance.GetServerTime();
```

### Custom module template
```csharp
[UpdateRefresh]
[LateUpdateRefresh]
[FixedUpdateRefresh]
public class MyModule : ModuleSingleton<MyModule>, IModule
{
    public void OnInit(object createParam)
    {
        // Module created
    }
    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void OnTermination()
    {
        Destroy(gameObject);
    }
}
```

### StaticModule template
```csharp
public class MyStaticModule : StaticModule
{
    public static MyStaticModule Instance => GetInstance<MyStaticModule>();

    protected override void Init() { }
    public override void OnEnterGame() { }
    public override void OnQuitGame() { }
}

// Usage
StaticModule.GetStaticModule();                      // Get all
StaticModule.GetStaticModuleByType(typeof(MyStaticModule)); // Get one
MyStaticModule.Instance.OnEnterGame();

// Iterate all static modules
foreach (var center in StaticModule.GetStaticModule())
{
    center.Value.OnEnterGame();
}
```

### Update attributes
- `[UpdateRefresh]` — Enable OnUpdate loop
- `[LateUpdateRefresh]` — Enable OnLateUpdate loop
- `[FixedUpdateRefresh]` — Enable OnFixedUpdate loop

### Module creation (right-click menu)
Right-click in Project → F8 Module Center → Create Module / ModuleMono / StaticModule template.

## Workflow

1. Decide module type: `ModuleSingleton<T>` (pure C#) or `ModuleSingletonMono<T>` (MonoBehaviour).
2. Implement `IModule` interface.
3. Add update attributes as needed.
4. Create module via `ModuleCenter.CreateModule<T>()` with priority.
5. For auto-initialized modules, use `StaticModule` pattern.
6. Use right-click context menu to generate template files.

## Module types comparison

| Type | Base Class | MonoBehaviour | Init | Update | Usage |
|------|-----------|--------------|------|--------|-------|
| Game Module | `ModuleSingleton<T>` | No | Lazy via CreateModule | Via attributes | Most modules |
| Game Module Mono | `ModuleSingletonMono<T>` | Yes | Lazy via CreateModule | Via attributes | Needs coroutines |
| Static Module | `StaticModule` | No | Auto on game start | Manual | Global utilities |

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Module is null | Not created via `CreateModule` | Create before accessing |
| Update not running | Missing `[UpdateRefresh]` attribute | Add appropriate attribute |
| Module init order issue | Dependency module not yet created | Adjust priority or creation order |

## Cross-module dependencies

- **Foundation**: GameLauncher uses ModuleCenter to create all framework modules.

## Output checklist

- Module type selected (Singleton / SingletonMono / StaticModule).
- Lifecycle methods implemented.
- Update attributes applied.
- Validation status and remaining risks.
