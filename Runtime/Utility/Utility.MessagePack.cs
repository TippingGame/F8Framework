using System;
namespace F8Framework.Core
{
    public static partial class Utility
    {
        public static class MessagePack
        {
            public interface IMessagePackHelper
            {
                /// <summary>
                /// 序列化对象到二进制；
                /// </summary>
                /// <typeparam name="T">mp标记的对象类型</typeparam>
                /// <param name="obj">mp对象</param>
                /// <returns>序列化后的对象</returns>
                byte[] Serialize<T>(T obj);
                /// <summary>
                /// 反序列化二进制到对象；
                /// </summary>
                /// <typeparam name="T">mp标记的对象类型</typeparam>
                /// <param name="bytes">需要反序列化的数组</param>
                /// <returns>反序列化后的对象</returns>
                T Deserialize<T>(byte[] bytes);
                /// <summary>
                /// 反序列化二进制到对象；
                /// </summary>
                /// <param name="bytes">需要反序列化的数组</param>
                /// <param name="type">mp标记的对象类型</param>
                /// <returns>反序列化后的对象</returns>
                object Deserialize(byte[] bytes, Type type);
                /// <summary>
                /// byte[]转json字符串；
                /// </summary>
                /// <param name="jsonBytes">需要被转换成json的byte数组</param>
                /// <returns>转换后的json</returns>
                string BytesToJson(byte[] jsonBytes);
                /// <summary>
                /// json字符串转byte[]；
                /// </summary>
                /// <param name="json">需要被转换成bytes的json</param>
                /// <returns>转换后的bytes</returns>
                byte[] JsonToBytes(string json);
                /// <summary>
                /// json字符串反序列化成对象；
                /// </summary>
                /// <typeparam name="T">mp标记的对象类型</typeparam>
                /// <param name="json">需要被转换的json</param>
                /// <returns>反序列化后的对象</returns>
                T DeserializeJson<T>(string json);
                /// <summary>
                /// json字符串反序列化成对象；
                /// </summary>
                /// <param name="json">需要被转换的json</param>
                /// <param name="type">mp标记的对象类型</param>
                /// <returns>反序列化后的对象</returns>
                object DeserializeJson(string json, Type type);
                /// <summary>
                /// 对象序列化成json字符串；
                /// </summary>
                /// <typeparam name="T">mp标记的对象类型</typeparam>
                /// <param name="obj">mp对象</param>
                /// <returns>转换后的json<</returns>
                string SerializeToJson<T>(T obj);
                /// <summary>
                /// 对象序列化成json的bytes；
                /// </summary>
                /// <typeparam name="T">mp标记的对象类型</typeparam>
                /// <param name="obj">mp对象</param>
                /// <returns>转换后的bytes</returns>
                byte[] SerializeToJsonBytes<T>(T obj);
            }
            static IMessagePackHelper messagePackHelper = null;
            public static void SetHelper(IMessagePackHelper helper)
            {
                messagePackHelper = helper;
            }

            /// <summary>
            /// 序列化对象到二进制；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="obj">mp对象</param>
            /// <returns>序列化后的对象</returns>
            public static byte[] Serialize<T>(T obj)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.Serialize(obj);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// 反序列化二进制到对象；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="bytes">需要反序列化的数组</param>
            /// <returns>反序列化后的对象</returns>
            public static T Deserialize<T>(byte[] bytes)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.Deserialize<T>(bytes);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// 反序列化二进制到对象；
            /// </summary>
            /// <param name="bytes">需要反序列化的数组</param>
            /// <param name="type">mp标记的对象类型</param>
            /// <returns>反序列化后的对象</returns>
            public static object Deserialize(byte[] bytes, Type type)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.Deserialize(bytes, type);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// byte[]转json字符串；
            /// </summary>
            /// <param name="jsonBytes">需要被转换成json的byte数组</param>
            /// <returns>转换后的json</returns>
            public static string BytesToJson(byte[] jsonBytes)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.BytesToJson(jsonBytes);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// json字符串转byte[]；
            /// </summary>
            /// <param name="json">需要被转换成bytes的json</param>
            /// <returns>转换后的bytes</returns>
            public static byte[] JsonToBytes(string json)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.JsonToBytes(json);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// json字符串反序列化成对象；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="json">需要被转换的json</param>
            /// <returns>反序列化后的对象</returns>
            public static T DeserializeJson<T>(string json)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.DeserializeJson<T>(json);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// json字符串反序列化成对象；
            /// </summary>
            /// <param name="json">需要被转换的json</param>
            /// <param name="type">mp标记的对象类型</param>
            /// <returns>反序列化后的对象</returns>
            public static object DeserializeJson(string json, Type type)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.DeserializeJson(json, type);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// 对象序列化成json字符串；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="obj">mp对象</param>
            /// <returns>转换后的json<</returns>
            public static string SerializeToJson<T>(T obj)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.SerializeToJson<T>(obj);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            /// <summary>
            /// 对象序列化成json的bytes；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="obj">mp对象</param>
            /// <returns>转换后的bytes</returns>
            public static byte[] SerializeToJsonBytes<T>(T obj)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("MessagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.SerializeToJsonBytes<T>(obj);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }
    }
}
