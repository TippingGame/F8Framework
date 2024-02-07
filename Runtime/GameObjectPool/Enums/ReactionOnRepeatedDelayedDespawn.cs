namespace F8Framework.Core
{
    internal enum ReactionOnRepeatedDelayedDespawn
    {
        Ignore,
        ResetDelay,
        ResetDelayIfNewTimeIsLess,
        ResetDelayIfNewTimeIsGreater,
        ThrowException
    }
}