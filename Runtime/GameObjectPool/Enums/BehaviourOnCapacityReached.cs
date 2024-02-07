namespace F8Framework.Core
{
    public enum BehaviourOnCapacityReached
    {
        ReturnNullableClone,
        Instantiate,
        InstantiateWithCallbacks,
        Recycle,
        ThrowException
    }
}