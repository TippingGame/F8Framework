using System;

namespace F8Framework.Core
{
    //================================================
    /*
     * 1、流程管理模块。
     * 
     * 2、流程节点的生命周期按照顺序依次为:OnInit>OnEnter>OnUpdate
     * >OnExit>OnDestroy。
     * 
     * 3、OnInit函数在ProcedureNode被添加到ProcedureManager时触发。
     * 
     * 4、OnEnter函数在进入ProcedureNode状态时触发。
     * 
     * 5、OnUpdate函数在ProcedureNode状态中轮询触发。
     * 
     * 6、OnExit函数在离开ProcedureNode状态时触发。
     * 
     * 7、OnDestroy函数在ProcedureNode被从ProcedureManager移除时触发。
     */
    //================================================

    [UpdateRefresh]
    public class ProcedureManager : ModuleSingleton<ProcedureManager>, IModule
    {
        ProcedureProcessor procedureProcessor;
        Type procedureNodeType = typeof(ProcedureNode);
        Action<ProcedureNodeAddedEventArgs> onProcedureNodeAdd;
        Action<PorcedureNodeRemovedEventArgs> onProcedureNodeRemove;
        Action<ProcedureNodeChangedEventArgs> onProcedureNodeChange;
        
        /// <summary>
        /// 获取流程节点的数量。
        /// </summary>
        public int ProcedureNodeCount { get { return procedureProcessor.NodeCount; } }

        /// <summary>
        /// 获取当前流程节点。
        /// </summary>
        public ProcedureNode CurrentProcedureNode
        {
            get { return procedureProcessor.CurrentNode; }
        }

        /// <summary>
        /// 流程节点添加事件。
        /// </summary>
        public event Action<ProcedureNodeAddedEventArgs> OnProcedureNodeAdd
        {
            add { onProcedureNodeAdd += value; }
            remove { onProcedureNodeAdd -= value; }
        }

        /// <summary>
        /// 流程节点移除事件。
        /// </summary>
        public event Action<PorcedureNodeRemovedEventArgs> OnProcedureNodeRemove
        {
            add { onProcedureNodeRemove += value; }
            remove { onProcedureNodeRemove -= value; }
        }

        /// <summary>
        /// 流程节点变化事件。
        /// </summary>
        public event Action<ProcedureNodeChangedEventArgs> OnProcedureNodeChange
        {
            add { onProcedureNodeChange += value; }
            remove { onProcedureNodeChange -= value; }
        }

        /// <summary>
        /// 添加流程节点。
        /// </summary>
        /// <param name="nodes">要添加的流程节点。</param>
        public void AddProcedureNodes(params ProcedureNode[] nodes)
        {
            procedureProcessor.AddNodes(nodes);
        }

        /// <summary>
        /// 运行指定类型的流程节点。
        /// </summary>
        /// <typeparam name="T">要运行的流程节点类型。</typeparam>
        public void RunProcedureNode<T>() where T : ProcedureNode
        {
            RunProcedureNode(typeof(T));
        }

        /// <summary>
        /// 运行指定类型的流程节点。
        /// </summary>
        /// <param name="type">要运行的流程节点类型。</param>
        public void RunProcedureNode(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("类型无效！");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"类型:{type} 不是从ProcedureState继承的");
            procedureProcessor.ChangeNode(type);
        }

        /// <summary>
        /// 检查是否存在指定类型的流程节点。
        /// </summary>
        /// <typeparam name="T">要检查的流程节点类型。</typeparam>
        /// <returns>如果存在指定类型的流程节点，则为 true；否则为 false。</returns>
        public bool HasProcedureNode<T>() where T : ProcedureNode
        {
            return HasProcedureNode(typeof(T));
        }

        /// <summary>
        /// 检查是否存在指定类型的流程节点。
        /// </summary>
        /// <param name="type">要检查的流程节点类型。</param>
        /// <returns>如果存在指定类型的流程节点，则为 true；否则为 false。</returns>
        public bool HasProcedureNode(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("类型无效！");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"类型:{type} 不是从ProcedureState继承的");
            return procedureProcessor.HasNode(type);
        }

        /// <summary>
        /// 获取指定类型的流程节点。
        /// </summary>
        /// <param name="type">要获取的流程节点类型。</param>
        /// <param name="node">返回的流程节点。</param>
        /// <returns>如果存在指定类型的流程节点，则为 true；否则为 false。</returns>
        public bool PeekProcedureNode(Type type, out ProcedureNode node)
        {
            if (type == null)
                throw new ArgumentNullException("类型无效！");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"类型:{type} 不是从ProcedureState继承的");
            node = null;
            var rst = procedureProcessor.PeekNode(type, out node);
            return rst;
        }

        /// <summary>
        /// 获取指定类型的流程节点。
        /// </summary>
        /// <typeparam name="T">要获取的流程节点类型。</typeparam>
        /// <param name="node">返回的流程节点。</param>
        /// <returns>如果存在指定类型的流程节点，则为 true；否则为 false。</returns>
        public bool PeekProcedureNode<T>(out T node) where T : ProcedureNode
        {
            node = default;
            var type = typeof(T);
            var hasNode = PeekProcedureNode(type, out var procedureNode);
            if (hasNode)
                node = (T)procedureNode;
            return hasNode;
        }

        /// <summary>
        /// 移除指定类型的流程节点。
        /// </summary>
        /// <param name="types">要移除的流程节点类型。</param>
        public void RemoveProcedureNodes(params Type[] types)
        {
            var length = types.Length;
            for (int i = 0; i < length; i++)
            {
                RemoveProcedureNode(types[i]);
            }
        }

        /// <summary>
        /// 移除指定类型的流程节点。
        /// </summary>
        /// <typeparam name="T">要移除的流程节点类型。</typeparam>
        /// <returns>如果成功移除流程节点，则为 true；否则为 false。</returns>
        public bool RemoveProcedureNode<T>() where T : ProcedureNode
        {
            return RemoveProcedureNode(typeof(T));
        }
        
        /// <summary>
        /// 移除指定类型的流程节点。
        /// </summary>
        /// <param name="type">要移除的流程节点类型。</param>
        /// <returns>如果成功移除流程节点，则为 true；否则为 false。</returns>
        public bool RemoveProcedureNode(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("类型无效！");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"类型:{type} 不是从ProcedureState继承的");
            return procedureProcessor.RemoveNode(type);
        }
        
        public void OnInit(object createParam)
        {
            procedureProcessor = new ProcedureProcessor(this);
            procedureProcessor.OnProcedureNodeAdd += ProcedureNodeAddCallback;
            procedureProcessor.OnProcedureNodeRemove += ProcedureNodeRemoveCallback;
            procedureProcessor.OnProcedureNodeChange += ProcedureNodeChangedCallback;
        }

        public void OnUpdate()
        {
            procedureProcessor.Refresh();
        }
        
        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }
        public void OnTermination()
        {
            procedureProcessor.ClearAllNode();
            procedureProcessor.OnProcedureNodeAdd -= ProcedureNodeAddCallback;
            procedureProcessor.OnProcedureNodeRemove -= ProcedureNodeRemoveCallback;
            procedureProcessor.OnProcedureNodeChange -= ProcedureNodeChangedCallback;
            base.Destroy();
        }
        void ProcedureNodeAddCallback(Type type)
        {
            var eventArgs = ProcedureNodeAddedEventArgs.Create(type);
            onProcedureNodeAdd?.Invoke(eventArgs);
            ProcedureNodeAddedEventArgs.Release(eventArgs);
        }
        void ProcedureNodeRemoveCallback(Type type)
        {
            var eventArgs = PorcedureNodeRemovedEventArgs.Create(type);
            onProcedureNodeRemove?.Invoke(eventArgs);
            PorcedureNodeRemovedEventArgs.Release(eventArgs);
        }
        void ProcedureNodeChangedCallback(Type exitedNodeType, Type enteredNodeType)
        {
            var eventArgs = ProcedureNodeChangedEventArgs.Create(exitedNodeType, enteredNodeType);
            onProcedureNodeChange?.Invoke(eventArgs);
            ProcedureNodeChangedEventArgs.Release(eventArgs);
        }
    }
}
