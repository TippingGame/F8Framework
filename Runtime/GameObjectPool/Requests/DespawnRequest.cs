#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace F8Framework.Core
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal struct DespawnRequest
    {
        internal Poolable Poolable;
        internal float TimeToDespawn;
    }
}