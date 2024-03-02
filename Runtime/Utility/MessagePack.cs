using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;

namespace F8Framework.Core
{
    public static partial class Util
    {
        public static class MessagePack
        {
            /// <summary>
            /// 序列化对象到二进制；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="obj">mp对象</param>
            /// <returns>序列化后的对象</returns>
            public static byte[] Serialize<T>(T obj)
            {
                return MessagePackSerializer.Serialize(obj);
            }

            /// <summary>
            /// 使用 MessagePack 序列化将类型为 T 的对象转换为字节数组，不执行安全检查。
            /// </summary>
            /// <typeparam name="T">要序列化对象的类型。</typeparam>
            /// <param name="obj">要序列化的对象。</param>
            /// <returns>表示序列化对象的字节数组。</returns>
            public static byte[] SerializeUnsafe<T>(T obj)
            {
                return MessagePackSerializer.SerializeUnsafe<T>(obj).Array;
            }

            /// <summary>
            /// 反序列化二进制到对象；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="bytes">需要反序列化的数组</param>
            /// <returns>反序列化后的对象</returns>
            public static T Deserialize<T>(byte[] bytes)
            {
                return MessagePackSerializer.Deserialize<T>(bytes);
            }

            /// <summary>
            /// 将类型为 T 的对象转换为 JSON 格式的字符串。
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型。</typeparam>
            /// <param name="obj">要转换的对象。</param>
            /// <returns>表示对象的 JSON 格式的字符串。</returns>
            public static string ToJson<T>(T obj)
            {
                return MessagePackSerializer.Json.ToJson<T>(obj);
            }

            /// <summary>
            /// byte[]转json字符串；
            /// </summary>
            /// <param name="jsonBytes">需要被转换成json的byte数组</param>
            /// <returns>转换后的json</returns>
            public static string BytesToJson(byte[] jsonBytes)
            {
                return MessagePackSerializer.Json.ToJson(jsonBytes);
            }

            /// <summary>
            /// json字符串反序列化成对象；
            /// </summary>
            /// <typeparam name="T">mp标记的对象类型</typeparam>
            /// <param name="json">需要被转换的json</param>
            /// <returns>反序列化后的对象</returns>
            public static T JsonToObject<T>(string json)
            {
                return MessagePackSerializer.Json.FromJson<T>(json);
            }

            /// <summary>
            /// json字符串转byte[]；
            /// </summary>
            /// <param name="json">需要被转换成bytes的json</param>
            /// <returns>转换后的bytes</returns>
            public static byte[] JsonToBytes(string json)
            {
                return MessagePackSerializer.Json.FromJson(json);
            }

            /// <summary>
            /// 初始化 MessagePack 序列化器，可以自定义添加多个解析器。
            /// </summary>
            /// <param name="customResolvers">自定义解析器数组。</param>
            public static void Initialize(params IFormatterResolver[] customResolvers)
            {
                // 创建解析器列表
                List<IFormatterResolver> resolverList = new List<IFormatterResolver>();

                // 将自定义解析器添加到列表中
                foreach (var resolver in customResolvers)
                {
                    resolverList.Add(resolver);
                }

                // 添加内置解析器、属性解析器、基本对象解析器和 Unity 解析器到列表中
                resolverList.Add(BuiltinResolver.Instance);
                resolverList.Add(AttributeFormatterResolver.Instance);
                resolverList.Add(PrimitiveObjectResolver.Instance);
                resolverList.Add(UnityResolver.Instance);

                // 注册并将组合解析器设置为默认解析器
                CompositeResolver.RegisterAndSetAsDefault(resolverList.ToArray());
            }
        }
    }
}