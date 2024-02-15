namespace F8Framework.Core
{
    /// <summary>
    /// 状态机组； 
    /// </summary>
    public interface IFSMGroup
    {
        /// <summary>
        /// 状态机组名；
        /// </summary>
        string GroupName { get; }
        
        /// <summary>
        /// 当前组状态机的数量；
        /// </summary>
        int FSMCount { get; }
        
        /// <summary>
        /// 根据条件查找是否存在符合条件的状态机
        /// </summary>
        /// <returns>是否存在</returns>
        bool HasFSM(TypeStringPair fsmKey);
        
        /// <summary>
        /// 条件查找状态机；
        /// </summary>
        /// <returns>符合条件的状态机</returns>
        FSMBase GetFSM(TypeStringPair fsmKey);
        
    }
}