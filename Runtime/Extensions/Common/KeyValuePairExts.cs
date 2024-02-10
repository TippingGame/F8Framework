using System.Collections.Generic;
namespace F8Framework.Core
{
    public static class KeyValuePairExts
    {
        public static KeyValuePair<TValue, TKey> Reverse<TKey, TValue>(this KeyValuePair<TKey, TValue> @this)
        {
            return new KeyValuePair<TValue, TKey>(@this.Value, @this.Key);
        }
    }
}
