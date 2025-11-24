using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace F8Framework.Core
{
    internal class NullableHandler : TypeHandler
    {
        private readonly Type _underlyingType;

        public NullableHandler(Type underlyingType)
        {
            _underlyingType = underlyingType;
        }

        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write((byte)0); // null 标记
            }
            else
            {
                writer.Write((byte)1); // 非 null 标记
                var handler = TypeHandlerFactory.GetHandler(_underlyingType);
                handler.Serialize(writer, value);
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var isNull = reader.ReadByte();
            if (isNull == 0)
                return null;

            var handler = TypeHandlerFactory.GetHandler(_underlyingType);
            return handler.Deserialize(reader, _underlyingType);
        }
    }

    internal class SByteHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((sbyte)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadSByte();
    }

    internal class UShortHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((ushort)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadUInt16();
    }

    internal class UIntHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((uint)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadUInt32();
    }

    internal class ULongHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((ulong)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadUInt64();
    }

    internal class DecimalHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var decimalValue = (decimal)value;
            var bits = decimal.GetBits(decimalValue);
            writer.Write(bits[0]);
            writer.Write(bits[1]);
            writer.Write(bits[2]);
            writer.Write(bits[3]);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var bits = new int[4];
            bits[0] = reader.ReadInt32();
            bits[1] = reader.ReadInt32();
            bits[2] = reader.ReadInt32();
            bits[3] = reader.ReadInt32();
            return new decimal(bits);
        }
    }

    internal class IntHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((int)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadInt32();
    }

    internal class FloatHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((float)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadSingle();
    }

    internal class DoubleHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((double)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadDouble();
    }

    internal class BoolHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((bool)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadBoolean();
    }

    internal class StringHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var str = (string)value;
            if (string.IsNullOrEmpty(str))
            {
                writer.Write((ushort)0);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                writer.Write((ushort)bytes.Length);
                writer.Write(bytes);
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var length = reader.ReadUInt16();
            if (length == 0) return string.Empty;

            var bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }
    }

    internal class ByteHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((byte)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadByte();
    }

    internal class ShortHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((short)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadInt16();
    }

    internal class LongHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value) => writer.Write((long)value);
        public override object Deserialize(BinaryReader reader, Type type) => reader.ReadInt64();
    }

    internal class DateTimeHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var dateTime = (DateTime)value;
            // 使用 Ticks 进行精确序列化，也可以使用 ToBinary() 方法
            writer.Write(dateTime.Ticks);
            writer.Write((int)dateTime.Kind); // 保存 DateTimeKind
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var ticks = reader.ReadInt64();
            var kind = (DateTimeKind)reader.ReadInt32();
            return new DateTime(ticks, kind);
        }
    }

    internal class DateTimeOffsetHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var dateTimeOffset = (DateTimeOffset)value;
            writer.Write(dateTimeOffset.Ticks);
            writer.Write(dateTimeOffset.Offset.Ticks);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var ticks = reader.ReadInt64();
            var offsetTicks = reader.ReadInt64();
            return new DateTimeOffset(ticks, new TimeSpan(offsetTicks));
        }
    }

    internal class TimeSpanHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var timeSpan = (TimeSpan)value;
            writer.Write(timeSpan.Ticks);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var ticks = reader.ReadInt64();
            return TimeSpan.FromTicks(ticks);
        }
    }

    internal class ValueTupleHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var type = value.GetType();

            if (!IsValueTuple(type))
            {
                throw new ArgumentException("Value must be a ValueTuple");
            }

            var genericArgs = type.GetGenericArguments();
            writer.Write((byte)genericArgs.Length);

            for (int i = 0; i < genericArgs.Length; i++)
            {
                var fieldName = $"Item{i + 1}";
                var field = type.GetField(fieldName);

                if (field == null)
                {
                    throw new InvalidOperationException($"Field {fieldName} not found in ValueTuple");
                }

                var fieldValue = field.GetValue(value);
                var fieldType = field.FieldType;

                // 处理 null 值
                if (fieldValue == null)
                {
                    writer.Write((byte)0); // null 标记
                }
                else
                {
                    writer.Write((byte)1); // 非 null 标记
                    var handler = TypeHandlerFactory.GetHandler(fieldType);
                    handler.Serialize(writer, fieldValue);
                }
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            if (!IsValueTuple(type))
            {
                throw new ArgumentException("Type must be a ValueTuple");
            }

            var elementCount = reader.ReadByte();
            var genericArgs = type.GetGenericArguments();

            if (elementCount != genericArgs.Length)
            {
                throw new InvalidDataException(
                    $"Expected {genericArgs.Length} tuple elements, but found {elementCount}");
            }

            var values = new object[elementCount];

            for (int i = 0; i < elementCount; i++)
            {
                var isNull = reader.ReadByte();
                if (isNull == 0)
                {
                    values[i] = null;
                }
                else
                {
                    var handler = TypeHandlerFactory.GetHandler(genericArgs[i]);
                    values[i] = handler.Deserialize(reader, genericArgs[i]);
                }
            }

            return Activator.CreateInstance(type, values);
        }

        private static bool IsValueTuple(Type type)
        {
            return type.IsGenericType && type.FullName != null && type.FullName.StartsWith("System.ValueTuple");
        }
    }

    internal class SingleValueTupleHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var type = value.GetType();
            var fieldValue = type.GetField("Item1")?.GetValue(value);
            var handler = TypeHandlerFactory.GetHandler(type.GetGenericArguments()[0]);
            handler.Serialize(writer, fieldValue);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var handler = TypeHandlerFactory.GetHandler(elementType);
            var value = handler.Deserialize(reader, elementType);
            return Activator.CreateInstance(type, value);
        }
    }

    internal class Vector2Handler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var vec = (Vector2)value;
            writer.Write(vec.x);
            writer.Write(vec.y);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
    }

    internal class Vector3Handler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var vec = (Vector3)value;
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    internal class Vector4Handler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var vec = (Vector4)value;
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
            writer.Write(vec.w);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    internal class Vector2IntHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var vec = (Vector2Int)value;
            writer.Write(vec.x);
            writer.Write(vec.y);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
        }
    }

    internal class Vector3IntHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var vec = (Vector3Int)value;
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }
    }

    internal class QuaternionHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var quat = (Quaternion)value;
            writer.Write(quat.x);
            writer.Write(quat.y);
            writer.Write(quat.z);
            writer.Write(quat.w);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    internal class ColorHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var color = (Color)value;
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    internal class Color32Handler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var color = (Color32)value;
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }
    }

    internal class RectHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var rect = (Rect)value;
            writer.Write(rect.x);
            writer.Write(rect.y);
            writer.Write(rect.width);
            writer.Write(rect.height);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    internal class RectIntHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var rect = (RectInt)value;
            writer.Write(rect.x);
            writer.Write(rect.y);
            writer.Write(rect.width);
            writer.Write(rect.height);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return new RectInt(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }
    }

    internal class BoundsHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var bounds = (Bounds)value;
            SerializeVector3(writer, bounds.center);
            SerializeVector3(writer, bounds.size);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var center = DeserializeVector3(reader);
            var size = DeserializeVector3(reader);
            return new Bounds(center, size);
        }

        private void SerializeVector3(BinaryWriter writer, Vector3 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        private Vector3 DeserializeVector3(BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    internal class BoundsIntHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            var bounds = (BoundsInt)value;
            SerializeVector3Int(writer, bounds.position);
            SerializeVector3Int(writer, bounds.size);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var position = DeserializeVector3Int(reader);
            var size = DeserializeVector3Int(reader);
            return new BoundsInt(position, size);
        }

        private void SerializeVector3Int(BinaryWriter writer, Vector3Int vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        private Vector3Int DeserializeVector3Int(BinaryReader reader)
        {
            return new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }
    }

    internal class EnumHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null)
            {
                // 对于可空枚举，写入默认值
                writer.Write(0);
                return;
            }

            var enumType = value.GetType();
            var underlyingType = Enum.GetUnderlyingType(enumType);

            // 根据底层类型进行序列化
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.Boolean:
                    writer.Write((bool)value);
                    break;
                case TypeCode.Byte:
                    writer.Write((byte)value);
                    break;
                case TypeCode.SByte:
                    writer.Write((sbyte)value);
                    break;
                case TypeCode.Int16:
                    writer.Write((short)value);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)value);
                    break;
                case TypeCode.Int32:
                    writer.Write((int)value);
                    break;
                case TypeCode.UInt32:
                    writer.Write((uint)value);
                    break;
                case TypeCode.Int64:
                    writer.Write((long)value);
                    break;
                case TypeCode.UInt64:
                    writer.Write((ulong)value);
                    break;
                case TypeCode.Single:
                    writer.Write((float)value);
                    break;
                case TypeCode.Double:
                    writer.Write((double)value);
                    break;
                default:
                    // 默认使用 int
                    writer.Write((int)value);
                    break;
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);

            object enumValue;
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.Boolean:
                    enumValue = reader.ReadBoolean();
                    break;
                case TypeCode.Byte:
                    enumValue = reader.ReadByte();
                    break;
                case TypeCode.SByte:
                    enumValue = reader.ReadSByte();
                    break;
                case TypeCode.Int16:
                    enumValue = reader.ReadInt16();
                    break;
                case TypeCode.UInt16:
                    enumValue = reader.ReadUInt16();
                    break;
                case TypeCode.Int32:
                    enumValue = reader.ReadInt32();
                    break;
                case TypeCode.UInt32:
                    enumValue = reader.ReadUInt32();
                    break;
                case TypeCode.Int64:
                    enumValue = reader.ReadInt64();
                    break;
                case TypeCode.UInt64:
                    enumValue = reader.ReadUInt64();
                    break;
                case TypeCode.Single:
                    enumValue = reader.ReadSingle();
                    break;
                case TypeCode.Double:
                    enumValue = reader.ReadDouble();
                    break;
                default:
                    enumValue = reader.ReadInt32();
                    break;
            }

            return Enum.ToObject(type, enumValue);
        }
    }

    internal class ArrayHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write(-1); // 使用 -1 表示 null 数组
                return;
            }

            var array = (Array)value;
            
            if (array.Length == 0)
            {
                writer.Write(0); // 写入 0 表示空数组
                return;
            }
            
            writer.Write(array.Length);

            var elementType = value.GetType().GetElementType();
            var handler = TypeHandlerFactory.GetHandler(elementType);

            for (int i = 0; i < array.Length; i++)
            {
                var elementValue = array.GetValue(i);
                
                if (elementValue == null)
                {
                    writer.Write((byte)0); // null 标记
                }
                else
                {
                    writer.Write((byte)1); // 非 null 标记
                    handler.Serialize(writer, elementValue);
                }
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var length = reader.ReadInt32();
            
            if (length == -1)
            {
                return null;
            }
            
            if (length == 0)
            {
                return Array.CreateInstance(type.GetElementType(), 0);
            }

            var elementType = type.GetElementType();
            var array = Array.CreateInstance(elementType, length);
            var handler = TypeHandlerFactory.GetHandler(elementType);

            for (int i = 0; i < length; i++)
            {
                // 读取 null 标记
                var isNotNull = reader.ReadByte();
            
                if (isNotNull == 0)
                {
                    // null 元素
                    array.SetValue(null, i);
                }
                else
                {
                    // 非 null 元素
                    array.SetValue(handler.Deserialize(reader, elementType), i);
                }
            }

            return array;
        }
    }

    internal class ListHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write(-1); // 使用 -1 表示 null 列表
                return;
            }

            var list = (System.Collections.IList)value;
            
            if (list.Count == 0)
            {
                writer.Write(0);
                return;
            }

            writer.Write(list.Count);

            var elementType = value.GetType().GetGenericArguments()[0];
            var handler = TypeHandlerFactory.GetHandler(elementType);

            foreach (var item in list)
            {
                if (item == null)
                {
                    writer.Write((byte)0); // null 标记
                }
                else
                {
                    writer.Write((byte)1); // 非 null 标记
                    handler.Serialize(writer, item);
                }
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var count = reader.ReadInt32();

            if (count == -1)
            {
                return null;
            }

            if (count == 0)
            {
                return Activator.CreateInstance(type);
            }

            var elementType = type.GetGenericArguments()[0];
            var list = (System.Collections.IList)Activator.CreateInstance(type);
            var handler = TypeHandlerFactory.GetHandler(elementType);

            for (int i = 0; i < count; i++)
            {
                var isNotNull = reader.ReadByte();

                if (isNotNull == 0)
                {
                    // null 元素
                    list.Add(null);
                }
                else
                {
                    // 非 null 元素
                    list.Add(handler.Deserialize(reader, elementType));
                }
            }

            return list;
        }
    }

    internal class DictionaryHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write(-1); // 使用 -1 表示 null 字典
                return;
            }

            var dict = (System.Collections.IDictionary)value;

            if (dict.Count == 0)
            {
                writer.Write(0);
                return;
            }

            writer.Write(dict.Count);

            var keyType = value.GetType().GetGenericArguments()[0];
            var valueType = value.GetType().GetGenericArguments()[1];
            var keyHandler = TypeHandlerFactory.GetHandler(keyType);
            var valueHandler = TypeHandlerFactory.GetHandler(valueType);

            foreach (System.Collections.DictionaryEntry entry in dict)
            {
                if (entry.Key == null)
                {
                    writer.Write((byte)0); // null 键标记
                }
                else
                {
                    writer.Write((byte)1); // 非 null 键标记
                    keyHandler.Serialize(writer, entry.Key);
                }

                if (entry.Value == null)
                {
                    writer.Write((byte)0); // null 值标记
                }
                else
                {
                    writer.Write((byte)1); // 非 null 值标记
                    valueHandler.Serialize(writer, entry.Value);
                }
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var count = reader.ReadInt32();

            if (count == -1)
            {
                return null;
            }

            if (count == 0)
            {
                return Activator.CreateInstance(type);
            }

            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];
            var dict = (System.Collections.IDictionary)Activator.CreateInstance(type);

            var keyHandler = TypeHandlerFactory.GetHandler(keyType);
            var valueHandler = TypeHandlerFactory.GetHandler(valueType);

            for (int i = 0; i < count; i++)
            {
                var keyNotNull = reader.ReadByte();
                object key;

                if (keyNotNull == 0)
                {
                    key = null;
                }
                else
                {
                    key = keyHandler.Deserialize(reader, keyType);
                }

                var valueNotNull = reader.ReadByte();
                object value;

                if (valueNotNull == 0)
                {
                    value = null;
                }
                else
                {
                    value = valueHandler.Deserialize(reader, valueType);
                }

                dict.Add(key, value);
            }

            return dict;
        }
    }

    internal class ObjectHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write((byte)0); // null 对象标记
                return;
            }

            writer.Write((byte)1); // 非 null 对象标记

            var type = value.GetType();
            
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                // 检查是否标记了 BinaryIgnore 特性
                if (property.IsDefined(typeof(BinaryIgnore), true))
                {
                    continue;
                }

                var fieldValue = property.GetValue(value);
                var handler = TypeHandlerFactory.GetHandler(property.PropertyType);

                if (fieldValue == null)
                {
                    writer.Write((byte)0); // null 字段标记
                }
                else
                {
                    writer.Write((byte)1); // 非 null 字段标记
                    handler.Serialize(writer, fieldValue);
                }
            }
            
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                // 检查是否标记了 BinaryIgnore 特性
                if (field.IsDefined(typeof(BinaryIgnore), true))
                {
                    continue;
                }

                var fieldValue = field.GetValue(value);
                var handler = TypeHandlerFactory.GetHandler(field.FieldType);

                if (fieldValue == null)
                {
                    writer.Write((byte)0); // null 字段标记
                }
                else
                {
                    writer.Write((byte)1); // 非 null 字段标记
                    handler.Serialize(writer, fieldValue);
                }
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var isNotNull = reader.ReadByte();

            if (isNotNull == 0)
            {
                return null;
            }

            var instance = Activator.CreateInstance(type);
            
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                // 检查是否标记了 BinaryIgnore 特性
                if (property.IsDefined(typeof(BinaryIgnore), true))
                {
                    continue;
                }

                var fieldNotNull = reader.ReadByte();

                if (fieldNotNull == 0)
                {
                    // null 字段
                    property.SetValue(instance, null);
                }
                else
                {
                    // 非 null 字段
                    var handler = TypeHandlerFactory.GetHandler(property.PropertyType);
                    var value = handler.Deserialize(reader, property.PropertyType);
                    property.SetValue(instance, value);
                }
            }
            
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                // 检查是否标记了 BinaryIgnore 特性
                if (field.IsDefined(typeof(BinaryIgnore), true))
                {
                    continue;
                }

                var fieldNotNull = reader.ReadByte();

                if (fieldNotNull == 0)
                {
                    // null 字段
                    field.SetValue(instance, null);
                }
                else
                {
                    // 非 null 字段
                    var handler = TypeHandlerFactory.GetHandler(field.FieldType);
                    var value = handler.Deserialize(reader, field.FieldType);
                    field.SetValue(instance, value);
                }
            }

            return instance;
        }
    }
}