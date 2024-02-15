using System;

namespace F8Framework.Core
{
    public class ProcedureNodeChangedEventArgs : IReference
    {
        public Type ExitedProcedureNodeType { get; private set; }
        public string ExitedProcedureNodeName { get; private set; }
        public Type EnteredProcedureNodeType { get; private set; }
        public string EnteredProcedureNodeName { get; private set; }
        public void Clear()
        {
            ExitedProcedureNodeType = null;
            ExitedProcedureNodeName = string.Empty;
            EnteredProcedureNodeType = null;
            EnteredProcedureNodeName = string.Empty;
        }
        public static ProcedureNodeChangedEventArgs Create(Type exitedType, Type enteredType)
        {
            var eventArgs = ReferencePool.Acquire<ProcedureNodeChangedEventArgs>();
            eventArgs.ExitedProcedureNodeType = exitedType;
            if (exitedType != null)
                eventArgs.ExitedProcedureNodeName = exitedType.Name;
            eventArgs.EnteredProcedureNodeType = enteredType;
            if (enteredType != null)
                eventArgs.EnteredProcedureNodeName = enteredType.Name;
            return eventArgs;
        }
        public static void Release(ProcedureNodeChangedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
