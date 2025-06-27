# F8 FSM

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 FSM Component**  
1. By inheriting from the finite state machine state **FSMState** and state transition **FSMSwitch**, you can control the state machine to add/switch/poll/destroy states.

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
public Transform Target;
public Transform objectA;

private void Start()
{
    /*-------------------------------------Basic Functions-------------------------------------*/
    // Create two states
    var enterState = new EnterRangeState();
    var exitState = new ExitRangeState();

    // Create two state transition conditions (optional)
    var enterSwitch = new EnterSwitch();
    var exitSwitch = new ExitSwitch();
    exitState.AddSwitch(exitSwitch, typeof(EnterRangeState));
    enterState.AddSwitch(enterSwitch, typeof(ExitRangeState));

    // Create finite state machine
    IFSM<Transform> fsmA = FF8.FSM.CreateFSM<Transform>("FSMTesterA", objectA, "FSMGroupName", exitState, enterState);
    fsmA.DefaultState = exitState;
    fsmA.ChangeToDefaultState();

    // Switch state
    fsmA.ChangeState<ExitRangeState>();

    
    /*-------------------------------------Additional Functions-------------------------------------*/
    // Get FSM
    FF8.FSM.GetFSM<Transform>("FSMTesterA");
    
    // Check if FSM exists
    FF8.FSM.HasFSM<Transform>("FSMTesterA");
    
    // Set FSM group
    FF8.FSM.SetFSMGroup<Transform>("FSMTesterA", "FSMGroupName");
    
    // Get all FSMs
    IList<FSMBase> fsms = FF8.FSM.GetAllFSMs();
    
    // Check if FSM group exists
    bool hasFSMGroup1 = FF8.FSM.HasFSMGroup("FSMGroupName");
    
    // Get FSM group
    bool hasFSMGroup2 = FF8.FSM.PeekFSMGroup("FSMGroupName", out var fsmGroup);
    
    // Remove FSM group
    FF8.FSM.RemoveFSMGroup("FSMGroupName");
    
    // Destroy FSM
    FF8.FSM.DestoryFSM<Transform>("FSMTesterA");
    
    // Destroy all FSMs
    FF8.FSM.DestoryAllFSM();
}

/*------------------------------Using Inherited FSMState/FSMSwitch------------------------------*/

// Inherit from FSM state
public class EnterRangeState : FSMState<Transform>
{
    public override void OnInitialization(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateEnter(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateUpdate(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateLateUpdate(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateFixedUpdate(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateExit(IFSM<Transform> fsm)
    {
    }
    
    public override void OnTermination(IFSM<Transform> fsm)
    {
    }
}
// Inherit from state transition
public class EnterSwitch : FSMSwitch<Transform>
{
    public override bool SwitchFunction(IFSM<Transform> fsm)
    {
        float distance = Vector3.Distance(fsm.Owner.transform.position, DemoFSM.Instance.Target.position);
        if (distance <= 10)
            return true;
        else
            return false;
    }
}

// Inherit from FSM state
public class ExitRangeState : FSMState<Transform>
{
    public override void OnInitialization(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateEnter(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateUpdate(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateLateUpdate(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateFixedUpdate(IFSM<Transform> fsm)
    {
    }
    
    public override void OnStateExit(IFSM<Transform> fsm)
    {
    }
    
    public override void OnTermination(IFSM<Transform> fsm)
    {
    }
}
// Inherit from state transition
public class ExitSwitch : FSMSwitch<Transform>
{
    public override bool SwitchFunction(IFSM<Transform> fsm)
    {
        float distance = Vector3.Distance(fsm.Owner.transform.position, DemoFSM.Instance.Target.position);
        if (distance > 10)
            return true;
        else
            return false;
    }
}
```


