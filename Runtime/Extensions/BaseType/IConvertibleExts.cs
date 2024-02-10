using System;
using System.Globalization;

namespace F8Framework.Core
{
    public static class IConvertibleExts
    {
        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this IConvertible @this) where T : IConvertible
        {
            return (T)ConvertTo(@this, typeof(T));
        }
        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns></returns>
        public static T TryConvertTo<T>(this IConvertible @this, T defaultValue = default) where T : IConvertible
        {
            try
            {
                return (T)ConvertTo(@this, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="result">转换失败的默认值</param>
        /// <returns></returns>
        public static bool TryConvertTo<T>(this IConvertible @this, out T result) where T : IConvertible
        {
            try
            {
                result = (T)ConvertTo(@this, typeof(T));
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
        /// <summary>
        /// 类型直转
        /// </summary>
        /// <param name="this"></param>
        /// <param name="type">目标类型</param>
        /// <param name="result">转换失败的默认值</param>
        /// <returns></returns>
        public static bool TryConvertTo(this IConvertible @this, Type type, out object result)
        {
            try
            {
                result = ConvertTo(@this, type);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
        /// <summary>
        /// 类型直转
        /// </summary>
        /// <param name="this"></param>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        public static object ConvertTo(this IConvertible @this, Type type)
        {
            if (null == @this)
            {
                return default;
            }
            if (type.IsEnum)
            {
                return Enum.Parse(type, @this.ToString(CultureInfo.InvariantCulture));
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null)
                {
                    return underlyingType.IsEnum ? Enum.Parse(underlyingType, @this.ToString(CultureInfo.CurrentCulture)) : Convert.ChangeType(@this, underlyingType);
                }
            }
            return Convert.ChangeType(@this, type);
        }
    }
}