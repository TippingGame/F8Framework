using System.Collections.Generic;

namespace F8Framework.Core
{
    /// <summary>
    /// 状态机容器
    /// </summary>
    public class FSMGroup : IFSMGroup, IReference
    {
        string groupName;
            
            
        readonly Dictionary<TypeStringPair, FSMBase> fsmDict
            = new Dictionary<TypeStringPair, FSMBase>();
            
        public Dictionary<TypeStringPair, FSMBase> FSMDict { get { return fsmDict; } }
            
        public string GroupName { get { return groupName; } set { groupName = value; } }
            
            
        public int FSMCount { get { return fsmDict.Count; } }
            
            
        public FSMBase GetFSM(TypeStringPair fsmKey)
        {
            fsmDict.TryGetValue(fsmKey, out var fsm);
            return fsm;
        }
            
        public bool HasFSM(TypeStringPair fsmKey)
        {
            return fsmDict.ContainsKey(fsmKey);
        }
            
        internal void AddFSM(TypeStringPair fsmKey, FSMBase fsm)
        {
            fsmDict.Add(fsmKey, fsm);
            fsm.GroupName = groupName;
        }
            
        internal void RemoveFSM(TypeStringPair fsmKey)
        {
            fsmDict.Remove(fsmKey, out var fsm);
            fsm.GroupName = string.Empty;
        }
            
        public void Clear()
        {
            fsmDict.Clear();
            groupName = string.Empty;
        }
    }
}