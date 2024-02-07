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
    internal readonly struct GettingPoolableArguments
    {
        public readonly Poolable Poolable;
        public readonly bool IsResultNullable;

        public GettingPoolableArguments(
            Poolable poolable, 
            bool isResultNullable)
        {
            Poolable = poolable;
            IsResultNullable = isResultNullable;
        }
    }
}