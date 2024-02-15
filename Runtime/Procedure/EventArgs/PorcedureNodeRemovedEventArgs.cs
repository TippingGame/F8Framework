using System;

namespace F8Framework.Core
{
    public class PorcedureNodeRemovedEventArgs : IReference
    {
        public Type RemovedProcedureNodeType { get; private set; }
        public string RemovedProcedureNodeName { get; private set; }
        public void Clear()
        {
            RemovedProcedureNodeType = null;
            RemovedProcedureNodeName = string.Empty;
        }
        public static PorcedureNodeRemovedEventArgs Create(Type type)
        {
            var eventArgs = ReferencePool.Acquire<PorcedureNodeRemovedEventArgs>();
            eventArgs.RemovedProcedureNodeType = type;
            if (type != null)
                eventArgs.RemovedProcedureNodeName = type.Name;
            return eventArgs;
        }
        public static void Release(PorcedureNodeRemovedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
