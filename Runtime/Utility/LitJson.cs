using System;
using System.IO;
using LitJson; // 使用 LitJson 库进行 JSON 操作

namespace F8Framework.Core
{
    public static partial class Util
    {
        /// <summary>
        /// JSON 操作类
        /// </summary>
        public static class LitJson
        {
            /// <summary>
            /// 将对象转换为 JSON 字符串
            /// </summary>
            public static string ToJson(object obj)
            {
                return JsonMapper.ToJson(obj);
            }

            /// <summary>
            /// 将对象写入 JSON 格式到指定的 JsonWriter
            /// </summary>
            public static void ToJson(object obj, JsonWriter writer)
            {
                JsonMapper.ToJson(obj, writer);
            }

            /// <summary>
            /// 从 JsonReader 中读取 JSON 数据并转换为 JsonData 对象
            /// </summary>
            public static JsonData ToObject(JsonReader reader)
            {
                return JsonMapper.ToObject(reader);
            }

            /// <summary>
            /// 从 TextReader 中读取 JSON 数据并转换为 JsonData 对象
            /// </summary>
            public static JsonData ToObject(TextReader reader)
            {
                return JsonMapper.ToObject(reader);
            }

            /// <summary>
            /// 将 JSON 字符串转换为 JsonData 对象
            /// </summary>
            public static JsonData ToObject(string json)
            {
                return JsonMapper.ToObject(json);
            }

            /// <summary>
            /// 从 JsonReader 中读取 JSON 数据并转换为指定类型的对象
            /// </summary>
            public static T ToObject<T>(JsonReader reader)
            {
                return JsonMapper.ToObject<T>(reader);
            }

            /// <summary>
            /// 从 TextReader 中读取 JSON 数据并转换为指定类型的对象
            /// </summary>
            public static T ToObject<T>(TextReader reader)
            {
                return JsonMapper.ToObject<T>(reader);
            }

            /// <summary>
            /// 将 JSON 字符串转换为指定类型的对象
            /// </summary>
            public static T ToObject<T>(string json)
            {
                return JsonMapper.ToObject<T>(json);
            }

            /// <summary>
            /// 将 JSON 字符串转换为指定类型的对象
            /// </summary>
            public static object ToObject(string json, Type convertType)
            {
                return JsonMapper.ToObject(json, convertType);
            }

            /// <summary>
            /// 从 JsonReader 中读取 JSON 数据并转换为指定类型的包装器对象
            /// </summary>
            public static IJsonWrapper ToWrapper(WrapperFactory factory, JsonReader reader)
            {
                return JsonMapper.ToWrapper(factory, reader);
            }

            /// <summary>
            /// 将 JSON 字符串转换为指定类型的包装器对象
            /// </summary>
            public static IJsonWrapper ToWrapper(WrapperFactory factory, string json)
            {
                return JsonMapper.ToWrapper(factory, json);
            }

            /// <summary>
            /// 注册指定类型的导出器
            /// </summary>
            public static void RegisterExporter<T>(ExporterFunc<T> exporter)
            {
                JsonMapper.RegisterExporter<T>(exporter);
            }

            /// <summary>
            /// 注册指定类型的导入器
            /// </summary>
            public static void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
            {
                JsonMapper.RegisterImporter(importer);
            }

            /// <summary>
            /// 注销所有导出器
            /// </summary>
            public static void UnregisterExporters()
            {
                JsonMapper.UnregisterExporters();
            }

            /// <summary>
            /// 注销所有导入器
            /// </summary>
            public static void UnregisterImporters()
            {
                JsonMapper.UnregisterImporters();
            }
        }
    }
}