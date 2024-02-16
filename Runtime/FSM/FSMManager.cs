using System.Collections.Generic;
using System;
using System.Linq;

namespace F8Framework.Core
{
    // 有限状态机管理器类
    [UpdateRefresh]
    public sealed class FSMManager : ModuleSingleton<FSMManager>, IModule
    {
        // 单个状态机字典
        Dictionary<TypeStringPair, FSMBase> fsmDict;
        
        // 状态机群组字典
        Dictionary<string, FSMGroup> fsmGroupDict;
        
        // FSM 缓存列表
        List<FSMBase> fsmCache = new List<FSMBase>();
        
        // 获取 FSM 数量
        public int FSMCount { get { return fsmDict.Count; } }
        
        // 获取 FSM 群组数量
        public int FSMGroupCount { get { return fsmGroupDict.Count; } }
        
        // 根据类型获取 FSM
        public FSMBase GetFSM<T>() where T : class
        {
            Type type = typeof(T).GetType();
            return GetFSM(type);
        }
        
        // 根据类型获取 FSM
        public FSMBase GetFSM(Type type)
        {
            fsmDict.TryGetValue(new TypeStringPair(type), out var fsm);
            return fsm;
        }
        
        // 获取所有 FSM
        public IList<FSMBase> GetAllFSMs()
        {
            return fsmDict.Values.ToArray();
        }
        
        // 检查是否存在指定名称的 FSM
        public bool HasFSM<T>(string name) where T : class
        {
            return HasFSM(typeof(T), name);
        }
        
        // 检查是否存在指定名称的 FSM
        public bool HasFSM(Type type, string name)
        {
            return fsmDict.ContainsKey(new TypeStringPair(type, name));
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(T owner, IList<FSMState<T>> states) where T : class
        {
            return CreateFSM(string.Empty, owner, string.Empty, states);
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states) where T : class
        {
            return CreateFSM(string.Empty, owner, string.Empty, states);
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, IList<FSMState<T>> states) where T : class
        {
            return CreateFSM(string.Empty, owner, fsmGroupName, states);
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, params FSMState<T>[] states) where T : class
        {
            return CreateFSM(string.Empty, owner, fsmGroupName, states);
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(string name, T owner, IList<FSMState<T>> states) where T : class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(string name, T owner, params FSMState<T>[] states) where T : class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(string name, T owner, string fsmGroupName, IList<FSMState<T>> states) where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            var fsmKey = new TypeStringPair(type, name);
            if (string.IsNullOrEmpty(fsmGroupName))
            {
                if (fsmDict.ContainsKey(fsmKey))
                    throw new ArgumentException($"FSMManager : FSM {type} 已存在");
                fsm = FSM<T>.Create(name, owner, states);
                fsmDict.Add(fsmKey, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                fsm.GroupName = fsmGroupName;
                if (HasFSMGroup(fsmGroupName))
                {
                    if (fsmGroupDict[fsmGroupName].HasFSM(fsmKey))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[fsmGroupName].AddFSM(fsmKey, fsm);
                }
                else
                {
                    var fsmGroup = ReferencePool.Acquire<FSMGroup>();
                    fsmGroup.GroupName = fsmGroupName;
                    fsmGroup.AddFSM(fsmKey, fsm);
                    fsmGroupDict.Add(fsmGroupName, fsmGroup);
                }
            }
            return fsm;
        }
        
        // 创建 FSM
        public IFSM<T> CreateFSM<T>(string name, T owner, string fsmGroupName, params FSMState<T>[] states) where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            var fsmKey = new TypeStringPair(type, name);
            if (string.IsNullOrEmpty(fsmGroupName))
            {
                if (fsmDict.ContainsKey(fsmKey))
                    throw new ArgumentException($"FSMManager : FSM {type} 已存在");
                fsm = FSM<T>.Create(name, owner, states);
                fsmDict.Add(fsmKey, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                fsm.GroupName = fsmGroupName;
                if (HasFSMGroup(fsmGroupName))
                {
                    if (fsmGroupDict[fsmGroupName].HasFSM(fsmKey))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[fsmGroupName].AddFSM(fsmKey, fsm);
                }
                else
                {
                    var fsmGroup = ReferencePool.Acquire<FSMGroup>();
                    fsmGroup.GroupName = fsmGroupName;
                    fsmGroup.AddFSM(fsmKey, fsm);
                    fsmGroupDict.Add(fsmGroupName, fsmGroup);
                }
            }
            return fsm;
        }
        
        // 销毁 FSM
        public void DestoryFSM<T>() where T : class
        {
            DestoryFSM(typeof(T));
        }
        
        // 销毁 FSM
        public void DestoryFSM(Type type)
        {
            FSMBase fsm = null;
            var fsmKey = new TypeStringPair(type);
            if (fsmDict.TryGetValue(fsmKey, out fsm))
            {
                fsm.Shutdown();
                fsmDict.Remove(fsmKey);
                var fsmGroupName = fsm.GroupName;

                if (!string.IsNullOrEmpty(fsmGroupName))
                {
                    fsmGroupDict.TryGetValue(fsmGroupName, out var group);
                    group.RemoveFSM(fsmKey);
                }
                fsm.GroupName = null;
            }
        }
        
        // 查找 FSM 群组
        public bool PeekFSMGroup(string fsmGroupName, out IFSMGroup fsmGroup)
        {
            var rst = fsmGroupDict.TryGetValue(fsmGroupName, out var group);
            fsmGroup = group;
            return rst;
        }
        
        // 移除 FSM 群组
        public void RemoveFSMGroup(string fsmGroupName)
        {
            if (!fsmGroupDict.TryRemove(fsmGroupName, out var fsmGroup))
                return;
            var dict = fsmGroup.FSMDict;
            foreach (var fsm in dict)
            {
                fsm.Value.GroupName = string.Empty;
            }
            ReferencePool.Release(fsmGroup);
        }
        
        // 检查是否存在 FSM 群组
        public bool HasFSMGroup(string fsmGroupName)
        {
            return fsmGroupDict.ContainsKey(fsmGroupName);
        }
        
        // 设置 FSM 群组
        public void SetFSMGroup<T>(string name, string fsmGroupName) where T : class
        {
            var fsmKey = new TypeStringPair(typeof(T), name);
            fsmDict.TryGetValue(fsmKey, out var fsm);
            if (!string.IsNullOrEmpty(fsm.GroupName))
            {
                fsmGroupDict.TryGetValue(fsm.GroupName, out var group);
                group?.RemoveFSM(fsmKey);
            }
            fsmGroupDict.TryGetValue(fsmGroupName, out var newGroup);
            newGroup?.AddFSM(fsmKey, fsm);
        }
        
        // 设置 FSM 群组
        public void SetFSMGroup<T>(string fsmGroupName) where T : class
        {
            SetFSMGroup<T>(string.Empty, fsmGroupName);
        }
        
        // 销毁所有 FSM
        public void DestoryAllFSM()
        {
            if (fsmDict.Count > 0)
            {
                foreach (var fsm in fsmDict)
                {
                    fsm.Value.Shutdown();
                    fsm.Value.GroupName = string.Empty;
                }
            }
            fsmCache.Clear();
            if (fsmGroupDict.Count > 0)
            {
                foreach (var fsmGroup in fsmGroupDict.Values)
                {
                    ReferencePool.Release(fsmGroup);
                }
            }
            fsmCache.Clear();
            fsmDict.Clear();
            fsmGroupDict.Clear();
            ReferencePool.RemoveAll<FSMGroup>();
        }
        
        // 在初始化时调用
        public void OnInit(object createParam)
        {
            fsmGroupDict = new Dictionary<string, FSMGroup>();
            fsmDict = new Dictionary<TypeStringPair, FSMBase>();
        }

        // 在更新时调用
        public void OnUpdate()
        {
            if (fsmDict.Count > 0)
                foreach (var fsm in fsmDict)
                {
                    fsm.Value.OnRefresh();
                }
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            base.Destroy();
        }
    }
}