# F8 Procedure

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Procedure游戏流程管理组件。
1. 通过继承流程节点 ProcedureNode，控制游戏流程的，添加/运行/轮询/移除。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 代码使用方法
```C#
    /*----------------------------游戏流程管理----------------------------*/
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
    
    // 继承ProcedureNode创建一个流程节点
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
```


