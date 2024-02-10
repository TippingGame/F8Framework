namespace F8Framework.Core
{
    /// <summary>
    /// 模块的抽象基类，外部可扩展
    /// </summary>
    public abstract class ModuleManager
    {
        public bool Pause { get; protected set; }

        internal void Update()
        {
            if (Pause)
                return;
            OnUpdate();
        }
        
        internal void LateUpdate()
        {
            if (Pause)
                return;
            OnLateUpdate();
        }
        
        internal void FixedUpdate()
        {
            if (Pause)
                return;
            OnFixedUpdate();
        }
        
        protected virtual void Init() { }
        
        protected virtual void OnEnterGame() { }
        
        protected virtual void OnFixedUpdate() { }
        
        protected virtual void OnUpdate() { }
        
        protected virtual void OnLateUpdate() { }
        
        protected virtual void OnQuitGame() { }
        
        protected virtual void OnTermination() { }
    }
}