namespace F8Framework.Core
{
    public abstract  class FSMSwitch<T>
        where T:class
    {
        protected string switchName;
        
        public string SwitchName { get { return switchName; } }
        
        public abstract bool SwitchFunction(IFSM<T> fsm);
    }
}