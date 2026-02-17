using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace F8Framework.Core
{
    // 非泛型基类，用于工厂统一管理
    public abstract class TypeHandler
    {
        public abstract void Serialize(BinaryWriter writer, object value);
        public abstract object Deserialize(BinaryReader reader, Type type);
    }

    // 泛型基类，所有具体 Handler 继承此类
    public abstract class TypeHandler<T> : TypeHandler
    {
        public abstract void Serialize(BinaryWriter writer, T value);
        public abstract T Deserialize(BinaryReader reader);

        // 实现非泛型接口，通过装箱/拆箱调用泛型方法
        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null && default(T) == null)
            {
                // 处理可空类型
                Serialize(writer, default(T));
            }
            else
            {
                Serialize(writer, (T)value);
            }
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            return Deserialize(reader);
        }
    }

    // 以下为具体 Handler，均继承 TypeHandler<T>
    internal class CharHandler : TypeHandler<char>
    {
        public override void Serialize(BinaryWriter writer, char value) => writer.Write(value);
        public override char Deserialize(BinaryReader reader) => reader.ReadChar();
    }

    public class NullableHandler<T> : TypeHandler<T?> where T : struct
    {
        public override void Serialize(BinaryWriter writer, T? value)
        {
            if (value == null)
            {
                writer.Write((byte)0);
            }
            else
            {
                writer.Write((byte)1);
                var handler = (TypeHandler<T>)TypeHandlerFactory.GetHandler(typeof(T));
                handler.Serialize(writer, value.Value);
            }
        }

        public override T? Deserialize(BinaryReader reader)
        {
            var isNull = reader.ReadByte();
            if (isNull == 0) return null;

            var handler = (TypeHandler<T>)TypeHandlerFactory.GetHandler(typeof(T));
            return handler.Deserialize(reader);
        }
    }

    internal class SByteHandler : TypeHandler<sbyte>
    {
        public override void Serialize(BinaryWriter writer, sbyte value) => writer.Write(value);
        public override sbyte Deserialize(BinaryReader reader) => reader.ReadSByte();
    }

    internal class UShortHandler : TypeHandler<ushort>
    {
        public override void Serialize(BinaryWriter writer, ushort value) => writer.Write(value);
        public override ushort Deserialize(BinaryReader reader) => reader.ReadUInt16();
    }

    internal class UIntHandler : TypeHandler<uint>
    {
        public override void Serialize(BinaryWriter writer, uint value) => writer.Write(value);
        public override uint Deserialize(BinaryReader reader) => reader.ReadUInt32();
    }

    internal class ULongHandler : TypeHandler<ulong>
    {
        public override void Serialize(BinaryWriter writer, ulong value) => writer.Write(value);
        public override ulong Deserialize(BinaryReader reader) => reader.ReadUInt64();
    }

    internal class DecimalHandler : TypeHandler<decimal>
    {
        public override void Serialize(BinaryWriter writer, decimal value)
        {
            var bits = decimal.GetBits(value);
            writer.Write(bits[0]);
            writer.Write(bits[1]);
            writer.Write(bits[2]);
            writer.Write(bits[3]);
        }

        public override decimal Deserialize(BinaryReader reader)
        {
            int lo = reader.ReadInt32();
            int mid = reader.ReadInt32();
            int hi = reader.ReadInt32();
            int flags = reader.ReadInt32();

            bool isNegative = (flags & 0x80000000) != 0;
            byte scale = (byte)((flags >> 16) & 0xFF);

            return new decimal(lo, mid, hi, isNegative, scale);
        }
    }

    internal class IntHandler : TypeHandler<int>
    {
        public override void Serialize(BinaryWriter writer, int value) => writer.Write(value);
        public override int Deserialize(BinaryReader reader) => reader.ReadInt32();
    }

    internal class FloatHandler : TypeHandler<float>
    {
        public override void Serialize(BinaryWriter writer, float value) => writer.Write(value);
        public override float Deserialize(BinaryReader reader) => reader.ReadSingle();
    }

    internal class DoubleHandler : TypeHandler<double>
    {
        public override void Serialize(BinaryWriter writer, double value) => writer.Write(value);
        public override double Deserialize(BinaryReader reader) => reader.ReadDouble();
    }

    internal class BoolHandler : TypeHandler<bool>
    {
        public override void Serialize(BinaryWriter writer, bool value) => writer.Write(value);
        public override bool Deserialize(BinaryReader reader) => reader.ReadBoolean();
    }

    internal class StringHandler : TypeHandler<string>
    {
        public override void Serialize(BinaryWriter writer, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.Write((ushort)0);
            }
            else
            {
                int maxLen = Encoding.UTF8.GetMaxByteCount(value.Length);
                byte[] rented = ArrayPool<byte>.Shared.Rent(maxLen);
                try
                {
                    int actualLen = Encoding.UTF8.GetBytes(value, 0, value.Length, rented, 0);
                    writer.Write((ushort)actualLen);
                    writer.Write(rented, 0, actualLen);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rented);
                }
            }
        }

        public override string Deserialize(BinaryReader reader)
        {
            var length = reader.ReadUInt16();
            if (length == 0) return string.Empty;
            
            byte[] rented = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                int read = reader.Read(rented, 0, length);
                string result = Encoding.UTF8.GetString(rented, 0, read);
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    internal class ByteHandler : TypeHandler<byte>
    {
        public override void Serialize(BinaryWriter writer, byte value) => writer.Write(value);
        public override byte Deserialize(BinaryReader reader) => reader.ReadByte();
    }

    internal class ShortHandler : TypeHandler<short>
    {
        public override void Serialize(BinaryWriter writer, short value) => writer.Write(value);
        public override short Deserialize(BinaryReader reader) => reader.ReadInt16();
    }

    internal class LongHandler : TypeHandler<long>
    {
        public override void Serialize(BinaryWriter writer, long value) => writer.Write(value);
        public override long Deserialize(BinaryReader reader) => reader.ReadInt64();
    }

    internal class DateTimeHandler : TypeHandler<DateTime>
    {
        public override void Serialize(BinaryWriter writer, DateTime value)
        {
            writer.Write(value.Ticks);
            writer.Write((int)value.Kind);
        }

        public override DateTime Deserialize(BinaryReader reader)
        {
            var ticks = reader.ReadInt64();
            var kind = (DateTimeKind)reader.ReadInt32();
            return new DateTime(ticks, kind);
        }
    }

    internal class DateTimeOffsetHandler : TypeHandler<DateTimeOffset>
    {
        public override void Serialize(BinaryWriter writer, DateTimeOffset value)
        {
            writer.Write(value.Ticks);
            writer.Write(value.Offset.Ticks);
        }

        public override DateTimeOffset Deserialize(BinaryReader reader)
        {
            var ticks = reader.ReadInt64();
            var offsetTicks = reader.ReadInt64();
            return new DateTimeOffset(ticks, new TimeSpan(offsetTicks));
        }
    }

    internal class TimeSpanHandler : TypeHandler<TimeSpan>
    {
        public override void Serialize(BinaryWriter writer, TimeSpan value) => writer.Write(value.Ticks);
        public override TimeSpan Deserialize(BinaryReader reader) => TimeSpan.FromTicks(reader.ReadInt64());
    }

    // ValueTuple
    public class ValueTupleHandler<T> : TypeHandler<T> where T : struct, ITuple
    {
        public override void Serialize(BinaryWriter writer, T value)
        {
            var type = typeof(T);
            var genericArgs = type.GetGenericArguments();
            writer.Write((byte)genericArgs.Length);

            for (int i = 0; i < genericArgs.Length; i++)
            {
                var fieldName = $"Item{i + 1}";
                var field = type.GetField(fieldName);
                var fieldValue = field.GetValue(value);
                var fieldType = field.FieldType;

                if (fieldValue == null)
                {
                    writer.Write((byte)0);
                }
                else
                {
                    writer.Write((byte)1);
                    var handler = TypeHandlerFactory.GetHandler(fieldType);
                    handler.Serialize(writer, fieldValue);
                }
            }
        }

        public override T Deserialize(BinaryReader reader)
        {
            var type = typeof(T);
            var elementCount = reader.ReadByte();
            var genericArgs = type.GetGenericArguments();

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

            return (T)Activator.CreateInstance(type, values); // ValueTuple 的构造函数仍需 Activator，因为它没有无参构造
        }
    }

    // 单元素 ValueTuple
    public class SingleValueTupleHandler<T> : TypeHandler<T> where T : struct, ITuple
    {
        public override void Serialize(BinaryWriter writer, T value)
        {
            var fieldValue = typeof(T).GetField("Item1").GetValue(value);
            var handler = TypeHandlerFactory.GetHandler(typeof(T).GetGenericArguments()[0]);
            handler.Serialize(writer, fieldValue);
        }

        public override T Deserialize(BinaryReader reader)
        {
            var elementType = typeof(T).GetGenericArguments()[0];
            var handler = TypeHandlerFactory.GetHandler(elementType);
            var value = handler.Deserialize(reader, elementType);
            return (T)Activator.CreateInstance(typeof(T), value);
        }
    }

    // Unity 类型
    internal class Vector2Handler : TypeHandler<Vector2>
    {
        public override void Serialize(BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public override Vector2 Deserialize(BinaryReader reader) => new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }

    internal class Vector3Handler : TypeHandler<Vector3>
    {
        public override void Serialize(BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public override Vector3 Deserialize(BinaryReader reader) => new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    internal class Vector4Handler : TypeHandler<Vector4>
    {
        public override void Serialize(BinaryWriter writer, Vector4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public override Vector4 Deserialize(BinaryReader reader) => new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    internal class Vector2IntHandler : TypeHandler<Vector2Int>
    {
        public override void Serialize(BinaryWriter writer, Vector2Int value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public override Vector2Int Deserialize(BinaryReader reader) => new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
    }

    internal class Vector3IntHandler : TypeHandler<Vector3Int>
    {
        public override void Serialize(BinaryWriter writer, Vector3Int value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public override Vector3Int Deserialize(BinaryReader reader) => new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
    }

    internal class QuaternionHandler : TypeHandler<Quaternion>
    {
        public override void Serialize(BinaryWriter writer, Quaternion value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public override Quaternion Deserialize(BinaryReader reader) => new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    internal class ColorHandler : TypeHandler<Color>
    {
        public override void Serialize(BinaryWriter writer, Color value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }

        public override Color Deserialize(BinaryReader reader) => new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    internal class Color32Handler : TypeHandler<Color32>
    {
        public override void Serialize(BinaryWriter writer, Color32 value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }

        public override Color32 Deserialize(BinaryReader reader) => new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
    }

    internal class RectHandler : TypeHandler<Rect>
    {
        public override void Serialize(BinaryWriter writer, Rect value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.width);
            writer.Write(value.height);
        }

        public override Rect Deserialize(BinaryReader reader) => new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    internal class RectIntHandler : TypeHandler<RectInt>
    {
        public override void Serialize(BinaryWriter writer, RectInt value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.width);
            writer.Write(value.height);
        }

        public override RectInt Deserialize(BinaryReader reader) => new RectInt(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
    }

    internal class BoundsHandler : TypeHandler<Bounds>
    {
        public override void Serialize(BinaryWriter writer, Bounds value)
        {
            writer.Write(value.center.x);
            writer.Write(value.center.y);
            writer.Write(value.center.z);
            writer.Write(value.size.x);
            writer.Write(value.size.y);
            writer.Write(value.size.z);
        }

        public override Bounds Deserialize(BinaryReader reader)
        {
            var center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            var size = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            return new Bounds(center, size);
        }
    }

    internal class BoundsIntHandler : TypeHandler<BoundsInt>
    {
        public override void Serialize(BinaryWriter writer, BoundsInt value)
        {
            writer.Write(value.position.x);
            writer.Write(value.position.y);
            writer.Write(value.position.z);
            writer.Write(value.size.x);
            writer.Write(value.size.y);
            writer.Write(value.size.z);
        }

        public override BoundsInt Deserialize(BinaryReader reader)
        {
            var position = new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            var size = new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            return new BoundsInt(position, size);
        }
    }

    // 枚举处理
    public class EnumHandler<T> : TypeHandler<T> where T : struct, Enum
    {
        public override void Serialize(BinaryWriter writer, T value)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.Byte: writer.Write((byte)(object)value); break;
                case TypeCode.SByte: writer.Write((sbyte)(object)value); break;
                case TypeCode.Int16: writer.Write((short)(object)value); break;
                case TypeCode.UInt16: writer.Write((ushort)(object)value); break;
                case TypeCode.Int32: writer.Write((int)(object)value); break;
                case TypeCode.UInt32: writer.Write((uint)(object)value); break;
                case TypeCode.Int64: writer.Write((long)(object)value); break;
                case TypeCode.UInt64: writer.Write((ulong)(object)value); break;
                default: writer.Write((int)(object)value); break;
            }
        }

        public override T Deserialize(BinaryReader reader)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            object rawValue;
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.Byte: rawValue = reader.ReadByte(); break;
                case TypeCode.SByte: rawValue = reader.ReadSByte(); break;
                case TypeCode.Int16: rawValue = reader.ReadInt16(); break;
                case TypeCode.UInt16: rawValue = reader.ReadUInt16(); break;
                case TypeCode.Int32: rawValue = reader.ReadInt32(); break;
                case TypeCode.UInt32: rawValue = reader.ReadUInt32(); break;
                case TypeCode.Int64: rawValue = reader.ReadInt64(); break;
                case TypeCode.UInt64: rawValue = reader.ReadUInt64(); break;
                default: rawValue = reader.ReadInt32(); break;
            }
            return (T)Enum.ToObject(typeof(T), rawValue);
        }
    }

    // 数组处理
    public class ArrayHandler<T> : TypeHandler<T[]>
    {
        private static readonly TypeHandler<T> _elementHandler = (TypeHandler<T>)TypeHandlerFactory.GetHandler(typeof(T));
        
        public override void Serialize(BinaryWriter writer, T[] value)
        {
            if (value == null)
            {
                writer.Write(-1);
                return;
            }

            writer.Write(value.Length);
            if (value.Length == 0) return;

            foreach (var item in value)
            {
                if (item == null)
                {
                    writer.Write((byte)0);
                }
                else
                {
                    writer.Write((byte)1);
                    _elementHandler.Serialize(writer, item);
                }
            }
        }

        public override T[] Deserialize(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            if (length == -1) return null;
            if (length == 0) return Array.Empty<T>();

            var array = new T[length];
            
            for (int i = 0; i < length; i++)
            {
                var isNotNull = reader.ReadByte();
                if (isNotNull == 0)
                {
                    array[i] = default(T);
                }
                else
                {
                    array[i] = _elementHandler.Deserialize(reader);
                }
            }
            return array;
        }
    }

    // 列表处理
    public class ListHandler<T> : TypeHandler<List<T>>
    {
        private static readonly TypeHandler<T> _elementHandler = (TypeHandler<T>)TypeHandlerFactory.GetHandler(typeof(T));

        public override void Serialize(BinaryWriter writer, List<T> value)
        {
            if (value == null)
            {
                writer.Write(-1);
                return;
            }

            writer.Write(value.Count);
            if (value.Count == 0) return;
            
            foreach (var item in value)
            {
                if (item == null)
                    writer.Write((byte)0);
                else
                {
                    writer.Write((byte)1);
                    _elementHandler.Serialize(writer, item);
                }
            }
        }

        public override List<T> Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            if (count == -1) return null;
            var list = new List<T>(count);
            if (count == 0) return list;

            for (int i = 0; i < count; i++)
            {
                var isNotNull = reader.ReadByte();
                if (isNotNull == 0)
                    list.Add(default(T));
                else
                    list.Add(_elementHandler.Deserialize(reader));
            }
            return list;
        }
    }

    // 字典处理
    public class DictionaryHandler<TKey, TValue> : TypeHandler<Dictionary<TKey, TValue>>
    {
        private static readonly TypeHandler<TKey> _keyHandler = (TypeHandler<TKey>)TypeHandlerFactory.GetHandler(typeof(TKey));
        private static readonly TypeHandler<TValue> _valueHandler = (TypeHandler<TValue>)TypeHandlerFactory.GetHandler(typeof(TValue));
        
        public override void Serialize(BinaryWriter writer, Dictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                writer.Write(-1);
                return;
            }

            writer.Write(value.Count);
            if (value.Count == 0) return;

            foreach (var kvp in value)
            {
                if (kvp.Key == null)
                {
                    writer.Write((byte)0);
                }
                else
                {
                    writer.Write((byte)1);
                    _keyHandler.Serialize(writer, kvp.Key);
                }

                if (kvp.Value == null)
                {
                    writer.Write((byte)0);
                }
                else
                {
                    writer.Write((byte)1);
                    _valueHandler.Serialize(writer, kvp.Value);
                }
            }
        }

        public override Dictionary<TKey, TValue> Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            if (count == -1) return null;
            var dict = new Dictionary<TKey, TValue>(count);
            if (count == 0) return dict;

            for (int i = 0; i < count; i++)
            {
                var keyNotNull = reader.ReadByte();
                TKey key = keyNotNull == 0 ? default(TKey) : _keyHandler.Deserialize(reader);

                var valueNotNull = reader.ReadByte();
                TValue val = valueNotNull == 0 ? default(TValue) : _valueHandler.Deserialize(reader);

                dict.Add(key, val);
            }
            return dict;
        }
    }

    // 自定义对象处理（要求 T 具有无参构造函数）
    public class ObjectHandler<T> : TypeHandler<T> where T : new()
    {
        // 缓存成员信息及对应的处理器
        private static List<MemberInfo> _members;
        private static List<TypeHandler> _handlers;
        private static List<Type> _memberTypes;
        private static bool _initialized;

        private static void EnsureInitialized()
        {
            if (_initialized) return;
            var type = typeof(T);
            _members = new List<MemberInfo>();
            _memberTypes = new List<Type>();

            // 收集所有需要序列化的属性（可读写且未标记 BinaryIgnore）
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanRead && prop.CanWrite && !prop.IsDefined(typeof(BinaryIgnore), true))
                {
                    _members.Add(prop);
                    _memberTypes.Add(prop.PropertyType);
                }
            }

            // 收集所有需要序列化的字段（未标记 BinaryIgnore）
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!field.IsDefined(typeof(BinaryIgnore), true))
                {
                    _members.Add(field);
                    _memberTypes.Add(field.FieldType);
                }
            }
            
            _handlers = new List<TypeHandler>();
            for (int i = 0; i < _memberTypes.Count; i++)
            {
                _handlers.Add(TypeHandlerFactory.GetHandler(_memberTypes[i]));
            }
            _initialized = true;
        }

        public override void Serialize(BinaryWriter writer, T value)
        {
            EnsureInitialized();
            
            if (value == null)
            {
                writer.Write((byte)0);
                return;
            }

            writer.Write((byte)1);

            for (int i = 0; i < _members.Count; i++)
            {
                object memberValue;
                if (_members[i] is PropertyInfo prop)
                    memberValue = prop.GetValue(value);
                else
                    memberValue = ((FieldInfo)_members[i]).GetValue(value);

                if (memberValue == null)
                    writer.Write((byte)0);
                else
                {
                    writer.Write((byte)1);
                    _handlers[i].Serialize(writer, memberValue);
                }
            }
        }

        public override T Deserialize(BinaryReader reader)
        {
            EnsureInitialized();

            var isNotNull = reader.ReadByte();
            if (isNotNull == 0)
                return default(T);

            var instance = new T();

            for (int i = 0; i < _members.Count; i++)
            {
                var memberNotNull = reader.ReadByte();
                if (memberNotNull == 0)
                {
                    if (_members[i] is PropertyInfo prop)
                        prop.SetValue(instance, null);
                    else
                        ((FieldInfo)_members[i]).SetValue(instance, null);
                }
                else
                {
                    var memberValue = _handlers[i].Deserialize(reader, _memberTypes[i]);
                    if (_members[i] is PropertyInfo prop)
                        prop.SetValue(instance, memberValue);
                    else
                        ((FieldInfo)_members[i]).SetValue(instance, memberValue);
                }
            }

            return instance;
        }
    }

    // 处理 object 类型（动态类型）
    internal class ObjectTypeHandler : TypeHandler
    {
        public override void Serialize(BinaryWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write((byte)0);
                return;
            }

            writer.Write((byte)1);
            var actualType = value.GetType();
            writer.Write(actualType.FullName);
            var handler = TypeHandlerFactory.GetHandler(actualType);
            handler.Serialize(writer, value);
        }

        public override object Deserialize(BinaryReader reader, Type type)
        {
            var isNotNull = reader.ReadByte();
            if (isNotNull == 0) return null;

            var typeName = reader.ReadString();
            var actualType = Type.GetType(typeName);
            if (actualType == null)
                throw new InvalidOperationException($"无法找到类型: {typeName}");

            var handler = TypeHandlerFactory.GetHandler(actualType);
            return handler.Deserialize(reader, actualType);
        }
    }
}