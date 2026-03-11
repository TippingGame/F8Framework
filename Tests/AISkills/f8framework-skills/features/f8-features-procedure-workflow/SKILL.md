---
name: f8-features-procedure-workflow
description: Use when implementing or troubleshooting Procedure feature workflows — game flow control, ProcedureNode management in F8Framework.
---

# Procedure Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about game flow control, procedure/state management.
- The user asks about ProcedureNode, game phases, or scene-level flow transitions.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Procedure/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Procedure
- Test docs: Assets/F8Framework/Tests/Procedure

## Key classes and interfaces

| Class | Role |
|-------|------|
| `ProcedureManager` | Core module. Access via `FF8.Procedure`. |
| `ProcedureNode` | Abstract base class for game flow nodes. |
| `ProcedureProcessor` | Runtime context passed to procedure lifecycle methods. |

## API quick reference

```csharp
// Add procedure node (optional — auto-search for ProcedureNode subclasses on init)
FF8.Procedure.AddProcedureNodes(new DemoInitState());

// Run a specific procedure
FF8.Procedure.RunProcedureNode<DemoInitState>();

// Remove procedure
FF8.Procedure.RemoveProcedureNode<DemoInitState>();

// Check existence
FF8.Procedure.HasProcedureNode<DemoInitState>();

// Get procedure node
FF8.Procedure.PeekProcedureNode(out DemoInitState initState);

// Current procedure
ProcedureNode current = FF8.Procedure.CurrentProcedureNode;

// Procedure count
int count = FF8.Procedure.ProcedureNodeCount;
```

### ProcedureNode template
```csharp
public class DemoInitState : ProcedureNode
{
    public override void OnInit(ProcedureProcessor processor) { }
    public override void OnEnter(ProcedureProcessor processor) { }
    public override void OnUpdate(ProcedureProcessor processor) { }
    public override void OnExit(ProcedureProcessor processor) { }
    public override void OnDestroy(ProcedureProcessor processor) { }
}
```

## Workflow

1. Define game phases as `ProcedureNode` subclasses (Init, Login, Lobby, Battle, etc.).
2. Register nodes via `AddProcedureNodes()` or let auto-search find them.
3. Start flow with `RunProcedureNode<InitState>()`.
4. Transition between nodes by calling `RunProcedureNode<NextState>()` from within a node (automatically exits current).
5. Use `OnEnter`/`OnExit` for setup/cleanup, `OnUpdate` for per-frame logic.
6. Query current state with `CurrentProcedureNode`.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Procedure not found | Not registered | Add via `AddProcedureNodes()` or check auto-search |
| Multiple procedures running | Calling Run without exiting current | Each Run auto-exits current node |

## Cross-module dependencies

- **Module**: ProcedureManager is created via ModuleCenter.
- **FSM**: For finer-grained state control within a procedure, use FSM.

## Output checklist

- Procedure nodes defined for all game phases.
- Transition flow documented.
- Files changed and why.
- Validation status and remaining risks.
