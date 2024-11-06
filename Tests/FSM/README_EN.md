# F8 FSM

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 FSM有限状态机组件。
1. 通过继承有限状态机状态 FSMState 和状态切换 FSMSwitch，控制状态机，添加/切换/轮询/销毁。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 代码使用方法
```C#
/*------------------------------FSM使用------------------------------*/
    public Transform Target;
    public Transform objectA;

    private void Start()
    {
        // 创建两个状态
        var enterState = new EnterRangeState();
        var exitState = new ExitRangeState();
        
        // 创建两个状态切换的时机
        var enterSwitch = new EnterSwitch();
        var exitSwitch = new ExitSwitch();
        exitState.AddSwitch(exitSwitch, typeof(EnterRangeState));
        enterState.AddSwitch(enterSwitch, typeof(ExitRangeState));

        // 创建有限状态机
        IFSM<Transform> fsmA = FF8.FSM.CreateFSM("FSMTesterA", objectA, exitState, enterState);
        fsmA.DefaultState = exitState;
        fsmA.ChangeToDefaultState();

        // 切换状态
        fsmA.ChangeState<ExitRangeState>();
    }
}

/*------------------------------继承FSMState/FSMSwitch使用------------------------------*/

// 继承有限状态机状态
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
    
    public override void OnStateExit(IFSM<Transform> fsm)
    {
    }
    
    public override void OnTermination(IFSM<Transform> fsm)
    {
    }
}
// 继承状态切换
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

// 继承有限状态机状态
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
    
    public override void OnStateExit(IFSM<Transform> fsm)
    {
    }
    
    public override void OnTermination(IFSM<Transform> fsm)
    {
    }
}
// 继承状态切换
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


