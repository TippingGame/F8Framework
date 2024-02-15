using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    // 有限状态机状态基类
    public abstract class FSMState<T> where T : class
    {
        // 状态转换列表
        List<FSMSwitch<T>> switchList = new List<FSMSwitch<T>>();
        
        // 状态转换字典，键为转换条件，值为目标状态类型
        Dictionary<FSMSwitch<T>, Type> switchStateDict = new Dictionary<FSMSwitch<T>, Type>();
        
        // 添加状态转换
        public void AddSwitch(FSMSwitch<T> @switch, Type stateType)
        {
            if (switchStateDict.ContainsKey(@switch))
                return;
            // 检查状态类型是否有效
            if (!typeof(FSMState<T>).IsAssignableFrom(stateType))
                throw new ArgumentException($"状态类型 {stateType.FullName} 是无效的！");
            switchStateDict.Add(@switch, stateType);
            switchList.Add(@switch);
        }
        
        // 移除状态转换
        public void RemoveSwitch(FSMSwitch<T> @switch)
        {
            if (!switchStateDict.ContainsKey(@switch))
                return;
            switchStateDict.Remove(@switch);
            switchList.Remove(@switch);
        }
        
        // 获取状态转换的目标状态类型
        public Type GetSwitchState(FSMSwitch<T> @switch)
        {
            if (switchStateDict.ContainsKey(@switch))
                return switchStateDict[@switch];
            return null;
        }
        
        // 初始化方法，在进入状态时调用
        public virtual void OnInitialization(IFSM<T> fsm) { }
        
        // 进入状态时的操作，由子类实现具体逻辑
        public abstract void OnStateEnter(IFSM<T> fsm);
        
        // 状态更新方法，由子类实现具体逻辑
        public abstract void OnStateUpdate(IFSM<T> fsm);
        
        // 刷新状态转换，检查是否需要进行状态转换
        public virtual void RefreshSwitch(IFSM<T> fsm)
        {
            for (int i = 0; i < switchList.Count; i++)
            {
                if (switchList[i].SwitchFunction(fsm))
                {
                    fsm.ChangeState(GetSwitchState(switchList[i]));
                    return;
                }
            }
        }
        
        // 退出状态时的操作，由子类实现具体逻辑
        public abstract void OnStateExit(IFSM<T> fsm);
        
        // 终止状态时的操作，由子类实现具体逻辑
        public virtual void OnTermination(IFSM<T> fsm) { }
        
        // 释放资源，清空状态转换列表和字典
        public virtual void Release()
        {
            switchList.Clear();
            switchStateDict.Clear();
        }
    }
}