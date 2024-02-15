using System.Collections.Generic;
using System;

namespace F8Framework.Core
{
    // 有限状态机（FSM）类，泛型化以便适用于不同类型的状态机
    internal sealed class FSM<T> : FSMBase, IFSM<T>, IReference
         where T : class
    {
        // FSM 的状态基类的类型
        Type stateBaseType = typeof(FSMState<T>);
        // FSM 的所有者
        T owner;
        // 当前状态
        FSMState<T> currentState;
        // 是否已销毁
        bool isDestoryed;
        // 默认状态
        FSMState<T> defaultState;
        // 状态改变事件委托
        Action<FSMState<T>, FSMState<T>> onStateChange;
        
        // 获取所有者
        public T Owner
        {
            get { return owner; }
            private set { owner = value; }
        }
        
        // 获取当前状态
        public FSMState<T> CurrentState { get { return currentState; } }
        
        // 获取是否已销毁
        public bool IsDestoryed
        {
            get
            {
                return isDestoryed;
            }
            private set
            {
                isDestoryed = value;
            }
        }
        
        // 获取或设置默认状态
        public FSMState<T> DefaultState
        {
            get { return defaultState; }
            set { defaultState = value; }
        }
        
        // FSM 的状态字典
        readonly Dictionary<Type, FSMState<T>> fsmStateDict = new Dictionary<Type, FSMState<T>>();
        
        // 获取 FSM 状态数
        public override int FSMStateCount
        {
            get { return fsmStateDict.Count; }
        }
        
        // 获取 FSM 是否正在运行
        public override bool IsRunning
        {
            get { return currentState != null; }
        }
        
        // 获取 FSM 所有者类型
        public override Type OwnerType
        {
            get { return typeof(T); }
        }
        
        // 获取当前状态名称
        public override string CurrentStateName
        {
            get
            {
                return currentState != null ? currentState.GetType().FullName : Constants.NULL;
            }
        }
        
        // 状态改变事件
        public event Action<FSMState<T>, FSMState<T>> OnStateChange
        {
            add { onStateChange += value; }
            remove { onStateChange -= value; }
        }
        
        // 释放 FSM
        public void Clear()
        {
            if (currentState != null)
            {
                currentState.OnStateExit(this);
            }
            foreach (var state in fsmStateDict)
            {
                state.Value.OnTermination(this);
            }
            Name = null;
            IsDestoryed = true;
            fsmStateDict.Clear();
            currentState = null;
        }
        
        // 切换到默认状态
        public void ChangeToDefaultState()
        {
            if (defaultState == null)
                return;
            Type type = defaultState.GetType();
            if (!stateBaseType.IsAssignableFrom(type))
                throw new ArgumentException($"状态类型 {type.FullName} 是无效的！");
            var hasState = PeekState(type, out var state);
            if (!hasState)
                return;
            currentState = state;
            currentState.OnStateEnter(this);
            ChangeStateHandler(null, currentState);
        }
        
        // FSM 轮询
        public override void OnRefresh()
        {
            if (Pause)
                return;
            currentState?.RefreshSwitch(this);
            currentState?.OnStateUpdate(this);
        }
        
        // 关闭 FSM
        public override void Shutdown()
        {
            ReferencePool.Release(this);
        }
        
        // 检查是否存在指定类型的状态
        public bool HasState(Type stateType)
        {
            if (stateType == null)
                throw new ArgumentNullException("状态类型无效！");
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentException($"状态类型 {stateType.FullName} 是无效的！");
            return fsmStateDict.ContainsKey(stateType);
        }
        
        // 检查是否存在指定类型的状态（泛型版本）
        public bool HasState<TState>()
            where TState : FSMState<T>
        {
            return HasState(typeof(TState));
        }
        
        // 切换到指定类型的状态
        public void ChangeState<TState>()
            where TState : FSMState<T>
        {
            ChangeState(typeof(TState));
        }
        
        // 切换到指定类型的状态
        public void ChangeState(Type stateType)
        {
            FSMState<T> state = null;
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentException($"状态类型 {stateType.FullName} 是无效的！");
            var hasState = PeekState(stateType, out state);
            if (!hasState)
                return;
            var previouseState = currentState;
            var nextState = state;

            currentState?.OnStateExit(this);
            currentState = state;
            currentState?.OnStateEnter(this);

            ChangeStateHandler(previouseState, nextState);
        }
        
        // 获取所有状态
        public void GetAllState(out List<FSMState<T>> result)
        {
            result = new List<FSMState<T>>();
            foreach (var state in fsmStateDict)
            {
                result.Add(state.Value);
            }
        }
        
        // 检查是否存在指定类型的状态，并返回该状态（泛型版本）
        public bool PeekState(Type stateType, out FSMState<T> state)
        {
            if (stateType == null)
                throw new ArgumentNullException("状态类型是无效的！");
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentNullException($"状态类型 {stateType.FullName} is invaild !");
            state = null;
            if (fsmStateDict.TryGetValue(stateType, out state))
                return true;
            return false;
        }
        
        // 检查是否存在指定类型的状态，并返回该状态（泛型版本）
        public bool PeekState<TState>(out TState state) where TState : FSMState<T>
        {
            state = null;
            var type = typeof(TState);
            if (fsmStateDict.TryGetValue(type, out var srcSate))
            {
                state = (TState)srcSate;
                return true;
            }
            return false;
        }
        
        // 获取所有状态，并返回数组
        public FSMState<T>[] GetAllState()
        {
            List<FSMState<T>> states = new List<FSMState<T>>();
            foreach (var state in fsmStateDict)
            {
                states.Add(state.Value);
            }
            return states.ToArray();
        }

        // 创建 FSM 实例
        internal static FSM<T> Create(string name, T owner, params FSMState<T>[] states)
        {
            if (states == null || states.Length < 1)
                throw new ArgumentNullException("FSM owner 是无效的！");
            // 从引用池获得同类
            FSM<T> fsm = ReferencePool.Acquire<FSM<T>>();
            fsm.Name = name;
            fsm.Owner = owner;
            fsm.IsDestoryed = false;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] == null)
                    throw new ArgumentNullException("FSM owner 是无效的！");
                Type type = states[i].GetType();
                if (fsm.HasState(type))
                    throw new ArgumentException($"FSM状态 {states[i].GetType().FullName} 已存在");
                else
                {
                    states[i].OnInitialization(fsm);
                    fsm.fsmStateDict.Add(type, states[i]);
                }
            }
            return fsm;
        }
        
        // 创建 FSM 实例
        internal static FSM<T> Create(string name, T owner, IList<FSMState<T>> states)
        {
            if (states == null || states.Count < 1)
                throw new ArgumentNullException("FSM owner 是无效的！");
            // 从引用池获得同类
            FSM<T> fsm = ReferencePool.Acquire<FSM<T>>();
            fsm.Name = name;
            fsm.Owner = owner;
            fsm.IsDestoryed = false;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i] == null)
                    throw new ArgumentNullException("FSM owner 是无效的！");
                Type type = states[i].GetType();
                if (fsm.HasState(type))
                    throw new ArgumentException($"FSM状态 {states[i].GetType()} 已存在");
                else
                {
                    states[i].OnInitialization(fsm);
                    fsm.fsmStateDict.Add(type, states[i]);
                }
            }
            return fsm;
        }

        // 处理状态改变
        void ChangeStateHandler(FSMState<T> previouseState, FSMState<T> currentState)
        {
            onStateChange?.Invoke(previouseState, currentState);
        }
    }
}