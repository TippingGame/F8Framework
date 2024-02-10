using System;
using System.Text;

namespace F8Framework.Core
{
    public static partial class Utility
    {
        public static class Json
        {
            /// <summary>
            /// Json辅助类接口，用于适配各类JSON解决方案
            /// </summary>
            public interface IJsonHelper
            {
                /// <summary>
                /// 将对象序列化为JSON字段
                /// </summary>
                /// <param name="obj">需要被序列化的对象</param>
                /// <returns>序列化后的JSON字符串</returns>
                string ToJson(object obj, bool prettyPrint);
                /// <summary>
                /// 将JSON字符串反序列化为对象
                /// </summary>
                /// <typeparam name="T">对象的类型</typeparam>
                /// <param name="json">需要被反序列化的JSON字符串</param>
                /// <returns>反序列化后的对象</returns>
                T ToObject<T>(string json);
                /// <summary>
                /// 将JSON字符串反序列化为对象
                /// </summary>
                /// <param name="objectType">对象类型</param>
                /// <param name="json">需要被反序列化的JSON字符串</param>
                /// <returns>反序列化后的对象</returns>
                object ToObject(string json, Type objectType);
            }

            static IJsonHelper jsonHelper = null;
            public static void SetHelper(IJsonHelper helper)
            {
                jsonHelper = helper;
            }
            public static void ClearHelper()
            {
                jsonHelper = null;
            }
            /// <summary>
            /// 将对象序列化为JSON字段
            /// </summary>
            /// <param name="obj">需要被序列化的对象</param>
            /// <returns>序列化后的JSON字符串</returns>xxxx
            public static string ToJson(object obj, bool prettyPrint = false)
            {
                if (jsonHelper == null)
                {
                    throw new ArgumentNullException("Json  helper is invalid");
                }
                try
                {
                    return jsonHelper.ToJson(obj, prettyPrint);
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"Can not convert to Json with exception {exception}");
                }
            }
            /// <summary>
            /// 将对象序列化为JSON流
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <returns>序列化后的JSON流</returns>
            public static byte[] ToJsonBytes(object obj)
            {
                return Encoding.UTF8.GetBytes(ToJson(obj));
            }
            /// <summary>
            /// 将JSON反序列化为泛型对象
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="json">需要反序列化的JSON字符串</param>
            /// <returns>反序列化后的泛型对象</returns>
            public static T ToObject<T>(string json)
            {
                if (jsonHelper == null)
                    throw new ArgumentNullException("Json helper is invalid");
                try
                {
                    return jsonHelper.ToObject<T>(json);
                }
                catch (System.Exception exception)
                {
                    throw new ArgumentException($"Can not convert to Json with exception {exception}");
                }
            }
            /// <summary>
            /// 将JSON字符串反序列化对象
            /// </summary>
            /// <param name="objectType">对象类型</param>
            /// <param name="json">需要反序列化的JSON字符串</param>
            /// <returns>反序列化后的对象</returns>
            public static object ToObject(string json, Type objectType)
            {
                if (jsonHelper == null)
                    throw new ArgumentNullException("Json helper is invalid");
                if (objectType == null)
                    throw new ArgumentNullException("Object type is invalid");
                try
                {
                    return jsonHelper.ToObject(json, objectType);
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"Can not convert to Json with exception {exception}");
                }
            }
            /// <summary>
            /// 将JSON Bytes流转换为对象
            /// </summary>
            /// <typeparam name="T">目标类型</typeparam>
            /// <param name="jsonData">JSON流</param>
            /// <returns>反序列化后的对象</returns>
            public static T BytesToObject<T>(byte[] jsonData)
            {
                return ToObject<T>(Encoding.UTF8.GetString(jsonData));
            }
            /// <summary>
            /// 将JSON Bytes流转换为对象
            /// </summary>
            /// <param name="jsonData">JSON流</param>
            /// <param name="type">目标类型</param>
            /// <returns>反序列化后的对象</returns>
            public static object BytesToObject(byte[] jsonData,Type type)
            {
                return ToObject(Encoding.UTF8.GetString(jsonData),type);
            }
        }
    }
}