namespace F8Framework.Core
{
    public enum BehaviourOnCapacityReached : byte
    {
        ReturnNullableClone,
        Instantiate,
        InstantiateWithCallbacks,
        Recycle,
        ThrowException
    }
}