using System;

namespace F8Framework.Core
{
    //================================================
    /*
    * 1、流程节点的生命周期按照顺序依次为:OnInit>OnEnter>OnUpdate>OnExit>OnDestroy;
    * 
    * 2、OnInit函数在ProcedureNode被添加到ProcedureManager时触发。
    * 
    * 3、OnEnter函数在进入ProcedureNode状态时触发。
    * 
    * 4、OnUpdate函数在ProcedureNodeW状态中轮询触发。
    * 
    * 5、OnExit函数在离开ProcedureNode状态时触发。
    * 
    * 6、OnDestroy函数在ProcedureNode被从ProcedureManager移除时触发。
    * 
    */
    //================================================
    public abstract class ProcedureNode
    {
        /// <summary>
        /// 当状态被添加时触发；
        /// </summary>
        public abstract void OnInit(ProcedureProcessor  processor);
        
        /// <summary>
        /// 进入状态时触发；
        /// </summary>
        /// <param name="processor">所属的状态机</param>
        public abstract void OnEnter(ProcedureProcessor processor);
        
        /// <summary>
        /// 当状态被执行时，轮询触发；
        /// </summary>
        /// <param name="processor">所属的状态机</param>
        public abstract void OnUpdate(ProcedureProcessor processor);
        
        /// <summary>
        /// 当离开状态时触发；
        /// </summary>
        /// <param name="processor">所属的状态机</param>
        public abstract void OnExit(ProcedureProcessor processor);
        
        /// <summary>
        /// 当状态被移除时触发；
        /// </summary>
        /// <param name="processor">所属的状态机</param>
        public abstract void OnDestroy(ProcedureProcessor processor);
        
        /// <summary>
        /// 切换所属状态机的状态；
        /// </summary>
        /// <typeparam name="K">切换的状态类型</typeparam>
        /// <param name="processor">所属的状态机</param>
        protected void ChangeState<K>(ProcedureProcessor processor)
            where K : ProcedureNode
        {
            processor.ChangeNode(typeof(K));
        }
        
        /// <summary>
        /// 切换所属状态机的状态；
        /// </summary>
        /// <param name="processor">所属的状态机</param>
        /// <param name="stateType">切换的状态类型</param>
        protected void ChangeState(ProcedureProcessor processor, Type stateType)
        {
            processor.ChangeNode(stateType);
        }
    }
}
