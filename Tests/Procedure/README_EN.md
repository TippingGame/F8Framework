# F8 Procedure

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Procedure Component**  
A structured game state management system that enables modular control of game flow through inheritable procedure nodes.
* Procedure Node System:
  * Inherit from ProcedureNode to create custom game states
  * Full lifecycle control: Add/Run/Update/Remove
  * Hierarchical state management

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
/*----------------------------Game Process Management----------------------------*/
void Start()
{
    // Add procedure node
    // (Optional - automatically searches and adds ProcedureNode subclasses during initialization)
    FF8.Procedure.AddProcedureNodes(new DemoInitState());

    // Run procedure node of specified type
    FF8.Procedure.RunProcedureNode<DemoInitState>();

    // Remove procedure node of specified type
    FF8.Procedure.RemoveProcedureNode<DemoInitState>();

    // Check if procedure node of specified type exists
    FF8.Procedure.HasProcedureNode<DemoInitState>();

    // Get procedure node of specified type
    FF8.Procedure.PeekProcedureNode(out DemoInitState initState);

    // Get current procedure node
    ProcedureNode procedureNode = FF8.Procedure.CurrentProcedureNode;

    // Get total count of procedure nodes
    int procedureNodeCount = FF8.Procedure.ProcedureNodeCount;
}

// Create a procedure node by inheriting ProcedureNode
public class DemoInitState : ProcedureNode
{
    public override void OnInit(ProcedureProcessor processor)
    {
        // Initialization logic
    }
    
    public override void OnEnter(ProcedureProcessor processor)
    {
        // Called when entering this state
    }

    public override void OnExit(ProcedureProcessor processor)
    {
        // Called when exiting this state
    }

    public override void OnUpdate(ProcedureProcessor processor)
    {
        // Called every frame while active
    }
    
    public override void OnDestroy(ProcedureProcessor processor)
    {
        // Cleanup logic
    }
}
```


