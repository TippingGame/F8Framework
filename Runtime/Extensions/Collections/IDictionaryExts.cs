using System;
using System.Collections.Generic;
using System.Linq;

namespace F8Framework.Core
{
    public static class IDictionaryExts
    {
        public static IDictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> @this)
        {
            return @this.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (@this.ContainsKey(key))
                return false;
            else
            {
                @this.Add(key, value);
                return true;
            }
        }
        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, out TValue value)
        {
            value = default;
            if (@this.ContainsKey(key))
            {
                value = @this[key];
                @this.Remove(key);
                return true;
            }
            return false;
        }
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, out TValue value)
        {
            if (@this.ContainsKey(key))
            {
                value = @this[key];
                @this.Remove(key);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this[key] = addValueFactory(key);
            }
            else
            {
                var oldValue = @this[key];
                @this[key] = updateValueFactory(key, oldValue);
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : defaultValue;
        }
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TValue> getDefaultValue)
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : getDefaultValue();
        }

        public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue? defaultValue = null)
            where TValue : struct
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : defaultValue;
        }
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TValue> getDefaultValue)
        {
            TValue value;
            if (!@this.TryGetValue(key, out value))
                @this[key] = value = getDefaultValue();
            return value;
        }
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
            where TValue : new()
        {
            TValue value;
            if (!@this.TryGetValue(key, out value))
                @this[key] = value = new TValue();
            return value;
        }
        public static int GetAndIncrement<TKey>(this IDictionary<TKey, int> @this, TKey key, int startValue = 0)
        {
            int value;
            if (@this.TryGetValue(key, out value))
                @this[key] = ++value;
            else
                @this[key] = value = startValue;
            return value;
        }
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            TValue value = default(TValue);
            bool isSuccess = @this.TryGetValue(key, out value);
            if (isSuccess)
                return value;
            return value;
        }
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> @this, Action<TKey, TValue> callback)
        {
            foreach (var item in @this)
            {
                callback(item.Key, item.Value);
            }
        }
    }
}
