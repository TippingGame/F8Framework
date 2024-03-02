using System;
using MessagePack;
using MessagePack.Formatters;

namespace F8Framework.Core
{
    public class StringToValueIntFormatter : IMessagePackFormatter<StringToValue<int>>
    {
        public int Serialize(ref byte[] bytes, int offset, StringToValue<int> value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt32(ref bytes, offset, value);
        }

        public StringToValue<int> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return MessagePackBinary.ReadInt32(bytes, offset, out readSize);
        }
    }
    public class StringToValueHttpTimeFormatter : IMessagePackFormatter<StringToValueHttpTime>
    {
        public int Serialize(ref byte[] bytes, int offset, StringToValueHttpTime value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt64(ref bytes, offset, value);
        }

        public StringToValueHttpTime Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return MessagePackBinary.ReadInt64(bytes, offset, out readSize);
        }
    }
}