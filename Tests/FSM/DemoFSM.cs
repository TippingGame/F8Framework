using UnityEngine;
using F8Framework.Core;
using F8Framework.Launcher;

namespace F8Framework.Tests
{
    public class DemoFSM : SingletonMono<DemoFSM>
    {
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
}