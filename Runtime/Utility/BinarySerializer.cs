using System;
using System.IO;
using System.Text;

namespace F8Framework.Core
{
    public static partial class Util
    {
        public static class BinarySerializer
        {
            public static byte[] Serialize(object obj)
            {
                using (var stream = new MemoryStream())
                {
                    SerializeToStream(obj, stream);
                    return stream.ToArray();
                }
            }

            public static void SerializeToFile(object obj, string filePath)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    SerializeToStream(obj, stream);
                }
            }

            public static T Deserialize<T>(byte[] data)
            {
                using (var stream = new MemoryStream(data))
                {
                    return (T)DeserializeFromStream(typeof(T), stream);
                }
            }

            public static T Deserialize<T>(string filePath)
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return (T)DeserializeFromStream(typeof(T), stream);
                }
            }

            public static object Deserialize(byte[] data, Type type)
            {
                using (var stream = new MemoryStream(data))
                {
                    return DeserializeFromStream(type, stream);
                }
            }

            private static void SerializeToStream(object obj, Stream stream)
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    if (obj == null)
                    {
                        writer.Write((byte)0); // null 标记
                        return;
                    }

                    writer.Write((byte)1); // 非 null 标记

                    var type = obj.GetType();
                    var handler = TypeHandlerFactory.GetHandler(type);
                    handler.Serialize(writer, obj);
                }
            }

            private static object DeserializeFromStream(Type type, Stream stream)
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var isNull = reader.ReadByte();
                    if (isNull == 0) return null;

                    var handler = TypeHandlerFactory.GetHandler(type);
                    return handler.Deserialize(reader, type);
                }
            }
        }
    }
}