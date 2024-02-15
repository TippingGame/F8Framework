using System;
namespace F8Framework.Core
{
    public abstract class FSMBase 
    {
        string name;
        
        public string Name
        {
            get { return name; }
            protected set { name = value ?? string.Empty; }
        }
        
        public string GroupName { get; internal set; }
        
        public abstract Type OwnerType { get; }
        
        public abstract int FSMStateCount { get; }
        
        public abstract bool IsRunning { get; }
        
        public abstract string CurrentStateName { get; }
        
        public bool Pause { get; set; }
        
        public abstract void OnRefresh();
        
        public abstract void Shutdown();
    }
}