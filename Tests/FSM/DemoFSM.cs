using System.Collections.Generic;
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
            /*-------------------------------------基础功能-------------------------------------*/
            // 创建两个状态
            var enterState = new EnterRangeState();
            var exitState = new ExitRangeState();

            // 创建两个状态切换的时机（可选）
            var enterSwitch = new EnterSwitch();
            var exitSwitch = new ExitSwitch();
            exitState.AddSwitch(exitSwitch, typeof(EnterRangeState));
            enterState.AddSwitch(enterSwitch, typeof(ExitRangeState));

            // 创建有限状态机
            IFSM<Transform> fsmA = FF8.FSM.CreateFSM<Transform>("FSMTesterA", objectA, "FSMGroupName", exitState, enterState);
            fsmA.DefaultState = exitState;
            fsmA.ChangeToDefaultState();

            // 切换状态
            fsmA.ChangeState<ExitRangeState>();

            
            /*-------------------------------------其他功能-------------------------------------*/
            // 获取 FSM
            FF8.FSM.GetFSM<Transform>("FSMTesterA");
            
            // 是否存在指定名称的 FSM
            FF8.FSM.HasFSM<Transform>("FSMTesterA");
            
            // 设置 FSM 群组
            FF8.FSM.SetFSMGroup<Transform>("FSMTesterA", "FSMGroupName");
            
            // 获取所有 FSM
            IList<FSMBase> fsms = FF8.FSM.GetAllFSMs();
            
            // 是否存在 FSM 群组
            bool hasFSMGroup1 = FF8.FSM.HasFSMGroup("FSMGroupName");
            
            // 获取 FSM 群组
            bool hasFSMGroup2 = FF8.FSM.PeekFSMGroup("FSMGroupName", out var fsmGroup);
            
            // 移除 FSM 群组
            FF8.FSM.RemoveFSMGroup("FSMGroupName");
            
            // 销毁 FSM
            FF8.FSM.DestoryFSM<Transform>("FSMTesterA");
            
            // 销毁所有 FSM
            FF8.FSM.DestoryAllFSM();
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