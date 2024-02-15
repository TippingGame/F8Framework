using System.Collections.Generic;
using System;
namespace F8Framework.Core
{
    /// <summary>
    /// 有限状态机接口
    /// </summary>
    /// <typeparam name="T">持有者类型</typeparam>
    public interface IFSM<T>
        where T : class
    {
        /// <summary>
        /// 状态机持有者
        /// </summary>
        T Owner { get; }
        
        /// <summary>
        /// 当前的状态
        /// </summary>
        FSMState<T> CurrentState { get; }
        
        /// <summary>
        /// 状态机中的状态数量
        /// </summary>
        int FSMStateCount { get; }
        
        /// <summary>
        /// 是否运行中
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// 是否背销毁
        /// </summary>
        bool IsDestoryed { get; }
        
        /// <summary>
        /// 默认状态
        /// </summary>
        FSMState<T> DefaultState { get; set; }
        
        /// <summary>
        /// 更换状态切换事件，previousState===currentState
        /// </summary>
        event Action<FSMState<T>, FSMState<T>> OnStateChange;
        
        /// <summary>
        /// 切换到默认状态
        /// </summary>
        void ChangeToDefaultState();
        
        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns>存在结果</returns>
        bool HasState(Type stateType);
        
        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <returns>存在结果</returns>
        bool HasState<TState>() where TState : FSMState<T>;
        
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        void ChangeState<TState>() where TState : FSMState<T>;
        
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        void ChangeState(Type stateType);
        
        /// <summary>
        /// 获取所有的状态
        /// </summary>
        /// <param name="result">状态集合</param>
        void GetAllState(out List<FSMState<T>> result);
        
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="state">获取到的状态</param>
        /// <returns>存在结果</returns>
        bool PeekState(Type stateType, out FSMState<T> state);
        
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <param name="state">获取到的状态</param>
        /// <returns>存在结果</returns>
        bool PeekState<TState>(out TState state) where TState : FSMState<T>;
        
        /// <summary>
        /// 获取所有的状态
        /// </summary>
        /// <returns>状态数组</returns>
        FSMState<T>[] GetAllState();
    }
}