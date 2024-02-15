using System;

namespace F8Framework.Core
{
    public class ProcedureNodeAddedEventArgs : IReference
    {
        public Type AddedProcedureNodeType { get; private set; }
        public string AddedProcedureNodeName { get; private set; }
        public void Clear()
        {
            AddedProcedureNodeType = null;
            AddedProcedureNodeName = string.Empty;
        }
        public static ProcedureNodeAddedEventArgs Create(Type type)
        {
            var eventArgs = ReferencePool.Acquire<ProcedureNodeAddedEventArgs>();
            eventArgs.AddedProcedureNodeType= type;
            if (type != null)
                eventArgs.AddedProcedureNodeName= type.Name;
            return eventArgs;
        }
        public static void Release(ProcedureNodeAddedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
