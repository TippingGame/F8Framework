using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace F8Framework.Core
{
    public static class ListExts
    {
        private static readonly Random random = new Random();
        public static TValue RemoveLast<TValue>(this List<TValue> @this)
        {
            if (@this == null || @this.Count == 0)
                return default(TValue);
            var index = @this.Count - 1;
            var result = @this[index];
            @this.RemoveAt(index);
            return result;
        }
        public static TValue RemoveFirst<TValue>(this List<TValue> @this)
        {
            if (@this == null || @this.Count == 0)
                return default(TValue);
            var result = @this[0];
            @this.RemoveAt(0);
            return result;
        }
        public static TValue First<TValue>(this List<TValue> @this)
        {
            if (@this == null || @this.Count == 0)
                return default(TValue);
            var result = @this[0];
            return result;
        }
        public static TValue Last<TValue>(this List<TValue> @this)
        {
            if (@this == null || @this.Count == 0)
                return default(TValue);
            var result = @this[@this.Count - 1];
            return result;
        }
        public static T RandomItem<T>(this IList<T> @this, Random rnd = null)
        {
            return @this[(rnd ?? random).Next(@this.Count)];
        }
        public static void InsertRange<T>(this IList<T> @this, int index, IEnumerable<T> items)
        {
            foreach (T item in items)
                @this.Insert(index++, item);
        }
        public static T AtWrapped<T>(this IList<T> @this, int index)
        {
            return @this[WrapIndex(index, @this.Count)];
        }
        public static T AtWrappedOrDefault<T>(this IList<T> @this, int index, T defaultValue = default(T))
        {
            return @this.Count > 0 ? @this[WrapIndex(index, @this.Count)] : defaultValue;
        }
        public static void SetAtWrapped<T>(this IList<T> @this, int index, T value)
        {
            @this[WrapIndex(index, @this.Count)] = value;
        }
        public static int IndexOfOrDefault<T>(this IList<T> @this, T value, int defaultIndex)
        {
            int index = @this.IndexOf(value);
            return index != -1 ? index : defaultIndex;
        }
        public static void ClearAndDispose<T>(this IList<T> @this) where T : IDisposable
        {
            foreach (T item in @this)
                item.Dispose();
            @this.Clear();
        }
        public static void AddRangeUntyped(this IList @this, IEnumerable items)
        {
            foreach (object item in items)
                @this.Add(item);
        }
        public static void RemoveRangeUntyped(this IList @this, IEnumerable items)
        {
            foreach (object item in items)
                @this.Remove(item);
        }
        public static void ReplaceUntyped(this IList @this, IEnumerable items)
        {
            @this.Clear();
            @this.AddRangeUntyped(items);
        }
        /// <summary>
        /// 改变元素的索引位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">集合</param>
        /// <param name="item">元素</param>
        /// <param name="index">索引值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<T> ChangeIndex<T>(this IList<T> @this, T item, int index)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            ChangeIndexInternal(@this, item, index);
            return @this;
        }

        /// <summary>
        /// 改变元素的索引位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">集合</param>
        /// <param name="condition">元素定位条件</param>
        /// <param name="index">索引值</param>
        public static IList<T> ChangeIndex<T>(this IList<T> @this, Func<T, bool> condition, int index)
        {
            var item = @this.FirstOrDefault(condition);
            if (item != null)
            {
                ChangeIndexInternal(@this, item, index);
            }
            return @this;
        }

        private static void ChangeIndexInternal<T>(IList<T> list, T item, int index)
        {
            index = Math.Max(0, index);
            index = Math.Min(list.Count - 1, index);
            list.Remove(item);
            list.Insert(index, item);
        }
        private static int WrapIndex(int index, int count)
        {
            if (count == 0)
                throw new IndexOutOfRangeException();
            else if (index >= count)
                index = index % count;
            else if (index < 0)
            {
                index = index % count;
                if (index != 0)
                    index += count;
            }
            return index;
        }
    }
}
