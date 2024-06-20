using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoProcedure : MonoBehaviour
    {
        void Start()
        {
            // 添加流程节点
            FF8.Procedure.AddProcedureNodes(new DemoInitState());

            // 运行指定类型的流程节点
            FF8.Procedure.RunProcedureNode<DemoInitState>();

            // 移除指定类型的流程节点
            FF8.Procedure.RemoveProcedureNode<DemoInitState>();

            // 检查是否存在指定类型的流程节点
            FF8.Procedure.HasProcedureNode<DemoInitState>();

            // 获取指定类型的流程节点
            FF8.Procedure.PeekProcedureNode(out DemoInitState initState);

            // 获取当前流程节点
            ProcedureNode procedureNode = FF8.Procedure.CurrentProcedureNode;

            // 获取流程节点的数量
            int procedureNodeCount = FF8.Procedure.ProcedureNodeCount;
        }
    }

    public class DemoInitState : ProcedureNode
    {
        public override void OnInit(ProcedureProcessor processor)
        {

        }

        public override void OnEnter(ProcedureProcessor processor)
        {

        }

        public override void OnExit(ProcedureProcessor processor)
        {

        }

        public override void OnUpdate(ProcedureProcessor processor)
        {

        }

        public override void OnDestroy(ProcedureProcessor processor)
        {

        }
    }
}
