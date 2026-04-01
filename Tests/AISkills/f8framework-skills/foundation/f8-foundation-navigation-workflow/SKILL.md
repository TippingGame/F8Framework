---
name: f8-foundation-navigation-workflow
description: Use as the routing skill for selecting the correct F8Framework layer skill chain across foundation, features, editor, tools, and build.
---

# F8 Foundation Navigation Workflow



## Use this skill when

- The request is broad and does not yet map to one module.
- The user asks which skill should be used.
- The task crosses multiple layers.
- You need to decide skill execution order for a complex task.

## Routing source

- Assets/f8framework-skills/SKILL.md

## Layer → Skill mapping

### foundation (2)
| Skill | When to use |
|-------|------------|
| `f8-foundation-bootstrap-workflow` | First-time setup, startup sequence, directory walkthrough |
| `f8-foundation-navigation-workflow` | Cross-layer routing, skill selection guidance |

### features (22)
| Skill | Module | When to use |
|-------|--------|------------|
| `f8-features-assetmanager-workflow` | AssetManager | Asset loading (sync/async), AB management, resource loading |
| `f8-features-audio-workflow` | Audio | BGM, voice, SFX playback and volume control |
| `f8-features-download-workflow` | Download | HTTP downloads, resume, progress tracking |
| `f8-features-event-workflow` | Event | Message dispatch, event listening, EventDispatcher |
| `f8-features-exceltool-workflow` | ExcelTool | Config tables, Excel read/write, F8/F7 generation |
| `f8-features-fsm-workflow` | FSM | Finite state machine, states, transitions, blackboard |
| `f8-features-gameobjectpool-workflow` | GameObjectPool | GameObject pooling, spawn/despawn, lifecycle events |
| `f8-features-hotupdatemanager-workflow` | HotUpdateManager | Hot update version management, sub-packages |
| `f8-features-hybridclr-workflow` | HybridCLR | C# code hot update via HybridCLR |
| `f8-features-input-workflow` | Input | Multi-platform input, virtual buttons, device switching |
| `f8-features-localization-workflow` | Localization | Multi-language, component localization, Excel-based |
| `f8-features-log-workflow` | Log | Logging, file writing, LogViewer, error reporting |
| `f8-features-module-workflow` | Module | ModuleCenter lifecycle, StaticModule, module creation |
| `f8-features-network-workflow` | Network | TCP/KCP/WebSocket client and server |
| `f8-features-obfuz-workflow` | Obfuz | Code obfuscation and protection via Obfuz |
| `f8-features-procedure-workflow` | Procedure | Game flow control, ProcedureNode management |
| `f8-features-referencepool-workflow` | ReferencePool | C# object pooling (IReference) |
| `f8-features-sdkmanager-workflow` | SDKManager | Native platform SDK integration |
| `f8-features-storage-workflow` | Storage | Local data storage, encryption |
| `f8-features-timer-workflow` | Timer | Timer/FrameTimer, server time sync |
| `f8-features-tween-workflow` | Tween | Tween animations, sequences, chains |
| `f8-features-ui-workflow` | UI | UI panel management, BaseView, layer control |

### editor (12)
| Skill | When to use |
|-------|------------|
| `f8-editor-assetmanager-workflow` | Asset editor tools: F8 generation, AB name management, asset inspector |
| `f8-editor-componentbind-workflow` | Component binding code generation, inspector binding |
| `f8-editor-event-workflow` | Event system monitor editor window |
| `f8-editor-exceltool-workflow` | F8/F7 keybinds, Excel directory config, export settings |
| `f8-editor-f8helper-workflow` | F8 main menu items and shortcuts |
| `f8-editor-gameobjectpool-workflow` | Preload pool Inspector, PoolsPreset asset creation |
| `f8-editor-localization-workflow` | F6 language switching, localization editor |
| `f8-editor-log-workflow` | Log editor settings, LogViewer |
| `f8-editor-module-workflow` | Right-click menu template creation for modules |
| `f8-editor-playerprefseditor-workflow` | PlayerPrefs editor window |
| `f8-editor-playmodeplus-workflow` | Play mode enhancement tools |
| `f8-editor-ui-workflow` | UI sprite slicing, atlas tools, safe area, BaseView template |

### tools (11)
| Skill | Source Files | When to use |
|-------|-------------|------------|
| `f8-tools-runtime-foundation-workflow` | Algorithm/Assert/Program/Time/Unity.cs | Core utility methods |
| `f8-tools-assembly-reflection-workflow` | Assembly.cs | Reflection and type discovery |
| `f8-tools-converter-workflow` | Converter.cs | Type conversion helpers |
| `f8-tools-binaryserializer-workflow` | BinarySerializer.cs | Binary serialization |
| `f8-tools-text-json-xml-workflow` | Text/LitJson/Xml.cs | JSON/XML/text processing |
| `f8-tools-io-filesystem-workflow` | IO.cs | File I/O operations |
| `f8-tools-net-workflow` | Net.cs | HTTP request helpers |
| `f8-tools-encryption-compression-zip-workflow` | Encryption/Compression/ZipHelper.cs | AES/ZIP/compression |
| `f8-tools-coroutine-mainthread-workflow` | Coroutine/ | Coroutine and main-thread utilities |
| `f8-tools-streamingassets-loader-workflow` | StreamingAssetsHelper/ | StreamingAssets file reading |
| `f8-tools-threadsync-workflow` | UnityThread/ | Thread synchronization |

### build (1)
| Skill | When to use |
|-------|------------|
| `f8-build-buildpkg-workflow` | F5 packaging, release build, hot update build, Jenkins CI |

## Workflow

1. Parse the request into intent type:
   - startup and structure → `foundation`
   - runtime feature behavior → `features`
   - editor tooling and UI operations → `editor`
   - low-level utility helper behavior → `tools`
   - packaging and release output → `build`
2. For mixed tasks, define sequence explicitly:
   - `foundation` → `editor` → `features` / `build`
   - `tools` as cross-cutting support when utility internals are involved
3. Return selected skills and reason in one concise mapping list.

## Routing examples

| User request | Selected skills | Order |
|-------------|----------------|-------|
| "How do I start using F8?" | bootstrap | 1 |
| "Add BGM to my game" | audio + assetmanager | assetmanager → audio |
| "Create a new UI panel" | editor-ui + features-ui | editor-ui → features-ui |
| "Set up hot update" | hotupdatemanager + buildpkg | hotupdatemanager → buildpkg |
| "Encrypt local save data" | storage + encryption-compression-zip | tools → features |
| "Debug network connection" | network + log | features (parallel) |

## Output checklist

- Selected layer and skill names.
- Short reason for each selection.
- Execution order when task spans multiple skills.
