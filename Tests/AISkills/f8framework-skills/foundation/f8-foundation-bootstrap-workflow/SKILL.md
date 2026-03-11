---
name: f8-foundation-bootstrap-workflow
description: Use when onboarding F8Framework, initializing first-run setup, or explaining launcher and directory responsibilities before feature work.
---

# F8 Foundation Bootstrap Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The project is new to F8Framework.
- The user asks how to start, what to click first, or where modules live.
- A downstream feature task is blocked by missing bootstrap context.
- The user needs to understand GameLauncher or module initialization order.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. Keep all follow-up routing consistent with the same root (Assets or Packages).

## Sources of truth

- Assets/F8Framework/README.md or Packages/F8Framework/README.md
- Assets/F8Framework/Launcher/GameLauncher.cs or Packages/F8Framework/Launcher/GameLauncher.cs
- Assets/F8Framework/Launcher/FF8.cs or Packages/F8Framework/Launcher/FF8.cs
- Assets/F8Framework/Tests or Packages/F8Framework/Tests

## Key classes and interfaces

| Class | Role |
|-------|------|
| `GameLauncher` | Framework startup entry point (MonoBehaviour). Uses coroutine `IEnumerator Start()` for async initialization. Drives `ModuleCenter.Update/LateUpdate/FixedUpdate` in its own Update loops. |
| `FF8` | Static module alias hub with lazy initialization. Each property auto-creates its module via `ModuleCenter.CreateModule<T>()` if not yet assigned. |
| `ModuleCenter` | Core module lifecycle manager. Handles Initialize, CreateModule, Update, LateUpdate, FixedUpdate for all modules. |
| `MessageManager` | Global message/event dispatcher. Accessed via `FF8.Message`. |
| `InputManager` | Input device management. Requires `DefaultInputHelper` as creation parameter. |
| `StorageManager` | Local data persistence (PlayerPrefs-based with optional AES encryption). |
| `TimerManager` | Timer and FrameTimer management. Depends on `MessageManager`. |
| `ProcedureManager` | Game flow/procedure node management. |
| `NetworkManager` | TCP/KCP/WebSocket networking. |
| `FSMManager` | Finite state machine management. |
| `GameObjectPool` | GameObject pooling. Also creates `F8PoolGlobal` module automatically. |
| `AssetManager` | Asset loading (Resources / AssetBundle / Remote). |
| `AssetBundleManager` | AssetBundle manifest loading. Must call `LoadAssetBundleManifest()` after `AssetManager` creation. |
| `F8DataManager` | Excel config data manager. Accessed via `FF8.Config`. |
| `AudioManager` | BGM, voice, and SFX management. Depends on Asset, GameObjectPool, Tween, Timer. |
| `Tween` | Tween animation engine. Accessed via `FF8.Tween`. |
| `UIManager` | UI panel lifecycle management. Depends on Asset. |
| `Localization` | Multi-language support. Requires `F8DataManager.Instance.GetLocalizedStrings()` as creation parameter. |
| `SDKManager` | Native platform SDK integration. Depends on Message. |
| `DownloadManager` | HTTP download management. |
| `F8LogWriter` | File-based log writer. Accessed via `FF8.LogWriter`. |
| `HotUpdateManager` | Hot update version management. Depends on Asset and Download. |

## GameLauncher.cs — full reference

```csharp
using System.Collections;
using F8Framework.Core;
using F8Framework.F8ExcelDataClass;
using UnityEngine;

namespace F8Framework.Launcher
{
    public class GameLauncher : MonoBehaviour
    {
        // 使用协程 IEnumerator Start()，支持异步加载
        IEnumerator Start()
        {
            // 1. 初始化模块中心（必须首先调用）
            ModuleCenter.Initialize(this);

            // 2. 初始化热更新版本管理（可选模块，建议最先创建）
            FF8.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();

            // 3. 按顺序创建核心模块（顺序影响依赖关系）
            FF8.Message = ModuleCenter.CreateModule<MessageManager>();
            FF8.Input = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
            FF8.Storage = ModuleCenter.CreateModule<StorageManager>();
            FF8.Timer = ModuleCenter.CreateModule<TimerManager>();
            FF8.Procedure = ModuleCenter.CreateModule<ProcedureManager>();
            FF8.Network = ModuleCenter.CreateModule<NetworkManager>();
            FF8.FSM = ModuleCenter.CreateModule<FSMManager>();
            FF8.GameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
            FF8.Asset = ModuleCenter.CreateModule<AssetManager>();

            // 4. 加载 AssetBundleManifest（必须在 AssetManager 模块创建之后）
            yield return AssetBundleManager.Instance.LoadAssetBundleManifest();

            FF8.Config = ModuleCenter.CreateModule<F8DataManager>();
            FF8.Audio = ModuleCenter.CreateModule<AudioManager>();
            FF8.Tween = ModuleCenter.CreateModule<Tween>();
            FF8.UI = ModuleCenter.CreateModule<UIManager>();

            // 5. 加载 LocalizedStrings 配置表（必须在 Localization 模块创建之前）
            yield return F8DataManager.Instance.LoadLocalizedStringsIEnumerator();

            FF8.Local = ModuleCenter.CreateModule<Localization>(
                F8DataManager.Instance.GetLocalizedStrings());
            FF8.SDK = ModuleCenter.CreateModule<SDKManager>();
            FF8.Download = ModuleCenter.CreateModule<DownloadManager>();
            FF8.LogWriter = ModuleCenter.CreateModule<F8LogWriter>();

            yield return new WaitForEndOfFrame();

            // 6. 所有模块就绪，开始游戏逻辑
            StartGame();
            yield break;
        }

        // 开始游戏（在此添加游戏入口逻辑）
        public void StartGame()
        {
            // 例如：FF8.Procedure.RunProcedureNode<InitProcedure>();
        }

        void Update()
        {
            // 驱动所有模块的 Update，切勿在其他地方重复调用
            ModuleCenter.Update();
        }

        void LateUpdate()
        {
            // 驱动所有模块的 LateUpdate，切勿在其他地方重复调用
            ModuleCenter.LateUpdate();
        }

        void FixedUpdate()
        {
            // 驱动所有模块的 FixedUpdate，切勿在其他地方重复调用
            ModuleCenter.FixedUpdate();
        }
    }
}
```

## FF8.cs — lazy initialization pattern

FF8 是一个静态类，每个模块属性都有懒加载机制：

```csharp
public static class FF8
{
    // 每个属性同时支持 get（懒创建）和 set（手动赋值）
    public static MessageManager Message
    {
        get
        {
            if (_message == null)
                _message = ModuleCenter.CreateModule<MessageManager>();
            return _message;
        }
        set
        {
            if (_message == null)
                _message = value;
        }
    }
    // ... 其他模块同此模式
}
```

**重要特性**：
- 如果 `GameLauncher` 中已通过 `set` 赋值，后续 `get` 直接返回已创建的实例
- 如果未通过 `GameLauncher` 初始化，首次 `get` 会自动创建模块（懒加载）
- `GameObjectPool` 的 `set` 和 `get` 会额外创建 `F8PoolGlobal` 模块
- `Localization` 的 `get` 会自动调用 `F8DataManager.Instance.GetLocalizedStrings()` 获取翻译数据
- `InputManager` 的 `get` 会自动传入 `new DefaultInputHelper()` 作为参数

## Module dependency chain (creation order matters)

```
ModuleCenter.Initialize(this)  ← 必须最先
    ↓
HotUpdateManager               ← 可选，建议最先
    ↓
MessageManager                  ← 全局消息（被 Input, Timer, SDK 依赖）
InputManager(DefaultInputHelper)← 依赖 Message
StorageManager
TimerManager                    ← 依赖 Message
ProcedureManager
NetworkManager
FSMManager
GameObjectPool + F8PoolGlobal
AssetManager
    ↓
yield return LoadAssetBundleManifest()  ← 异步！必须在 AssetManager 之后
    ↓
F8DataManager(Config)           ← 依赖 Asset
AudioManager                    ← 依赖 Asset, GameObjectPool, Tween, Timer
Tween
UIManager                       ← 依赖 Asset
    ↓
yield return LoadLocalizedStringsIEnumerator()  ← 异步！必须在 Localization 之前
    ↓
Localization(GetLocalizedStrings()) ← 依赖 Config, Asset
SDKManager                      ← 依赖 Message
DownloadManager
F8LogWriter
```

## Directory structure map

| Directory | Purpose | Layer |
|-----------|---------|-------|
| `Launcher/` | Framework startup: GameLauncher.cs, FF8.cs | foundation |
| `Runtime/` | All runtime modules and systems | features / tools |
| `Runtime/Utility/` | Shared low-level helpers (Algorithm, IO, Encryption, etc.) | tools |
| `Editor/` | Editor windows, menus, context tools, authoring helpers | editor |
| `Tests/` | Usage docs (README.md) and demo scripts for each module | reference |
| `ConfigData/` | Auto-generated C# data classes from Excel | features (ExcelTool) |
| `AssetMap/` | Auto-generated asset index and AB name mapping files | features (AssetManager) |

## First-run checklist

1. **Import framework**: git clone 或 Package Manager 导入
2. **Press F8**: 自动生成资源索引、AB名和配置数据类
3. **挂接 GameLauncher**: 确保场景中有 GameObject 挂载 `GameLauncher` 组件
4. **检查初始化顺序**: 在 `GameLauncher.Start()` 中按依赖链创建模块
5. **编写 StartGame()**: 在所有模块就绪后启动游戏逻辑
6. **跳转目标层技能**: 按任务类型路由到 features / editor / tools / build

## Operation steps

1. Read README from Assets/F8Framework/README.md to extract startup intent, supported platforms, and first-run notes.
2. Confirm launcher entry in Launcher/GameLauncher.cs and module-name customization in Launcher/FF8.cs.
3. Map directories for the user with strict boundaries as shown above.
4. Provide a minimal start path in order:
   - Import or sync framework (git clone or Package Manager)
   - Press F8 to generate asset index and config classes
   - Verify GameLauncher is attached to a scene GameObject
   - Check module initialization order in GameLauncher.Start()
   - Move to target layer skill
5. Route task by target intent:
   - features: runtime module implementation and behavior
   - editor: editor window/menu/inspector tooling
   - tools: Runtime/Utility helper internals
   - build: packaging and artifact verification

## Common error handling

| Error | Handling |
|-------|----------|
| README or Launcher path not found under Assets | Switch to Packages/F8Framework and continue. |
| User mixes editor setup and runtime implementation | Split into two phases: editor first, then features/tools. |
| Startup is skipped and feature APIs fail at runtime | Require bootstrap checklist completion before feature debugging. |
| User asks for build execution during onboarding | Finish foundation mapping first, then hand off to build skill. |
| Press F8 shows compilation errors | Ensure no naming conflicts in ConfigData/ auto-generated classes. Clean and regenerate. |
| ModuleCenter.Initialize not called | Framework modules will throw NullReferenceException. Ensure GameLauncher runs first. |
| AssetBundleManifest load fails | 必须在 `AssetManager` 创建之后才能调用 `LoadAssetBundleManifest()`。 |
| Localization 初始化失败 | 必须先 `yield return LoadLocalizedStringsIEnumerator()`，再创建 `Localization` 模块。 |
| ModuleCenter.Update 被多处调用 | 只在 GameLauncher 的 Update/LateUpdate/FixedUpdate 中调用，切勿在其他地方重复调用。 |
| FF8.GameObjectPool 创建异常 | 注意 `GameObjectPool` 的 get/set 会自动额外创建 `F8PoolGlobal` 模块。 |

## Output checklist

- Explain startup sequence in concrete steps.
- Name the exact folders the user should inspect next.
- State which follow-up layer skill should be used for the actual task.
