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
                var writer = BinaryIOPool.GetWriter();
                if (obj == null)
                    writer.Write((byte)0);
                else
                {
                    writer.Write((byte)1);
                    var type = obj.GetType();
                    var handler = TypeHandlerFactory.GetHandler(type);
                    handler.Serialize(writer, obj);
                }
                var ms = (MemoryStream)writer.BaseStream;
                return ms.ToArray();
            }

            public static void SerializeToFile(object obj, string filePath)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var data = Serialize(obj);
                    stream.Write(data, 0, data.Length);
                }
            }

            public static T Deserialize<T>(byte[] data)
            {
                var reader = BinaryIOPool.GetReader(data);
                var isNull = reader.ReadByte();
                if (isNull == 0) return default(T);
                var handler = TypeHandlerFactory.GetHandler(typeof(T));
                return (T)handler.Deserialize(reader, typeof(T));
            }

            public static T Deserialize<T>(string filePath)
            {
                byte[] data = File.ReadAllBytes(filePath);
                return Deserialize<T>(data);
            }

            public static object Deserialize(byte[] data, Type type)
            {
                var reader = BinaryIOPool.GetReader(data);
                var isNull = reader.ReadByte();
                if (isNull == 0) return null;
                var handler = TypeHandlerFactory.GetHandler(type);
                return handler.Deserialize(reader, type);
            }
        }
        internal static class BinaryIOPool
        {
            // 单线程环境：直接使用静态实例
            private static readonly MemoryStream _stream = new MemoryStream();
            private static readonly BinaryWriter _writer = new BinaryWriter(_stream, Encoding.UTF8, true);
            private static readonly BinaryReader _reader = new BinaryReader(_stream, Encoding.UTF8, true);

            /// <summary>
            /// 获取重置后的 BinaryWriter（关联的 MemoryStream 已清空）
            /// </summary>
            internal static BinaryWriter GetWriter()
            {
                _stream.SetLength(0); // 重置长度，相当于清空
                _stream.Position = 0;
                return _writer;
            }

            /// <summary>
            /// 获取重置后的 BinaryReader，并将指定数据写入底层流
            /// </summary>
            internal static BinaryReader GetReader(byte[] data)
            {
                _stream.SetLength(0);
                _stream.Position = 0;
                _stream.Write(data, 0, data.Length);
                _stream.Position = 0;
                return _reader;
            }
        }
    }
}