---
name: f8-features-fsm-workflow
description: Use when implementing or troubleshooting FSM feature workflows — finite state machines, state transitions, blackboard data, and FSM groups in F8Framework.
---

# FSM Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.FSM = ModuleCenter.CreateModule<FSMManager>();`.


## Use this skill when

- The task is about state machine construction, transition flow, and update loop.
- The user asks about FSMState, FSMSwitch, blackboard, or FSM groups.
- Troubleshooting state transition failures or update loop issues.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. For usage docs, read: Assets/F8Framework/Tests/FSM/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/FSM
- Test docs: Assets/F8Framework/Tests/FSM

## Key classes and interfaces

| Class | Role |
|-------|------|
| `FSMManager` | Core module. Access via `FF8.FSM`. Creates, finds, destroys FSMs. |
| `IFSM<T>` | FSM interface. Owner type `T` is the owner object type. |
| `FSMState<T>` | Abstract state base. Override lifecycle methods. |
| `FSMSwitch<T>` | Abstract transition condition. Override `SwitchFunction()`. |
| `Blackboard` | Key-value data store shared across states in a FSM. |
| `FSMBase` | Non-generic base class for FSM collections. |

## API quick reference

### Create FSM
```csharp
var enterState = new EnterRangeState();
var exitState = new ExitRangeState();
var enterSwitch = new EnterSwitch();
var exitSwitch = new ExitSwitch();
exitState.AddSwitch(exitSwitch, typeof(EnterRangeState));
enterState.AddSwitch(enterSwitch, typeof(ExitRangeState));

IFSM<Transform> fsm = FF8.FSM.CreateFSM<Transform>(
    "FSMName", owner, "GroupName", new Blackboard(), exitState, enterState);
fsm.DefaultState = exitState;
fsm.ChangeToDefaultState();
```

### Change state
```csharp
fsm.ChangeState<ExitRangeState>();
```

### Blackboard
```csharp
fsm.Blackboard.SetValue<float>("health", 100f);
fsm.Blackboard.GetValue<float>("health");
fsm.Blackboard.HasValue("health");
fsm.Blackboard.RemoveValue("health");
fsm.Blackboard.Clear();
// Subscribe to value changes
fsm.Blackboard.RegisterValueChanged<Vector2>(OnValueChanged);
fsm.Blackboard.RegisterValueRemoved(OnValueRemoved);
fsm.Blackboard.UnregisterValueChanged<Vector2>(OnValueChanged);
fsm.Blackboard.UnregisterValueRemoved(OnValueRemoved);
```

### FSM management
```csharp
FF8.FSM.GetFSM<Transform>("FSMName");
FF8.FSM.HasFSM<Transform>("FSMName");
FF8.FSM.SetFSMGroup<Transform>("FSMName", "GroupName");
FF8.FSM.GetAllFSMs();
FF8.FSM.HasFSMGroup("GroupName");
FF8.FSM.PeekFSMGroup("GroupName", out var group);
FF8.FSM.RemoveFSMGroup("GroupName");
FF8.FSM.DestoryFSM<Transform>("FSMName");
FF8.FSM.DestoryAllFSM();
```

### State class template
```csharp
public class MyState : FSMState<Transform>
{
    public override void OnInitialization(IFSM<Transform> fsm) { }
    public override void OnStateEnter(IFSM<Transform> fsm) { }
    public override void OnStateUpdate(IFSM<Transform> fsm) { }
    public override void OnStateLateUpdate(IFSM<Transform> fsm) { }
    public override void OnStateFixedUpdate(IFSM<Transform> fsm) { }
    public override void OnStateExit(IFSM<Transform> fsm) { }
    public override void OnTermination(IFSM<Transform> fsm) { }
}
```

### Switch class template
```csharp
public class MySwitch : FSMSwitch<Transform>
{
    public override bool SwitchFunction(IFSM<Transform> fsm)
    {
        // Return true to trigger transition
        return someCondition;
    }
}
```

## Workflow

1. Define states by inheriting `FSMState<T>` with desired owner type.
2. Define transition conditions by inheriting `FSMSwitch<T>`.
3. Wire switches to states with `state.AddSwitch(switch, targetStateType)`.
4. Create FSM via `FF8.FSM.CreateFSM<T>()` passing all states.
5. Set default state and call `ChangeToDefaultState()`.
6. Use Blackboard for shared data across states.
7. Use FSM groups for managing related FSMs together.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| State not changing | Switch condition never returns true | Debug `SwitchFunction` logic |
| Null owner | Owner destroyed before FSM | Destroy FSM when owner is destroyed |
| Duplicate FSM name | Same name used for multiple FSMs | Use unique names per FSM |

## Cross-module dependencies

- None — FSM is self-contained. Can reference any module from within states.

## Output checklist

- States and switches defined.
- FSM created with correct owner type.
- Blackboard data flow documented.
- Validation status and remaining risks.
