using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace F8Framework.Core
{
    internal abstract class TypeHandler
    {
        public abstract void Serialize(BinaryWriter writer, object value);
        public abstract object Deserialize(BinaryReader reader, Type type);
    }

    internal static class TypeHandlerFactory
    {
        private static readonly Dictionary<Type, TypeHandler> _handlers = new Dictionary<Type, TypeHandler>
        {
            // 基础类型
            { typeof(bool), new BoolHandler() },
            { typeof(byte), new ByteHandler() },
            { typeof(sbyte), new SByteHandler() },
            { typeof(short), new ShortHandler() },
            { typeof(ushort), new UShortHandler() },
            { typeof(int), new IntHandler() },
            { typeof(uint), new UIntHandler() },
            { typeof(long), new LongHandler() },
            { typeof(ulong), new ULongHandler() },
            { typeof(float), new FloatHandler() },
            { typeof(double), new DoubleHandler() },
            { typeof(decimal), new DecimalHandler() },
            { typeof(string), new StringHandler() },

            // 时间类型
            { typeof(DateTime), new DateTimeHandler() },
            { typeof(DateTimeOffset), new DateTimeOffsetHandler() },
            { typeof(TimeSpan), new TimeSpanHandler() },

            // Unity 数学类型
            { typeof(Vector2), new Vector2Handler() },
            { typeof(Vector3), new Vector3Handler() },
            { typeof(Vector4), new Vector4Handler() },
            { typeof(Vector2Int), new Vector2IntHandler() },
            { typeof(Vector3Int), new Vector3IntHandler() },
            { typeof(Quaternion), new QuaternionHandler() },
            { typeof(Color), new ColorHandler() },
            { typeof(Color32), new Color32Handler() },
            { typeof(Rect), new RectHandler() },
            { typeof(RectInt), new RectIntHandler() },
            { typeof(Bounds), new BoundsHandler() },
            { typeof(BoundsInt), new BoundsIntHandler() },
        };

        public static TypeHandler GetHandler(Type type)
        {
            if (_handlers.TryGetValue(type, out var handler))
                return handler;
            
            if (type == typeof(object))
                return new ObjectTypeHandler();

            if (IsValueTuple(type))
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                    return new SingleValueTupleHandler();
                else
                    return new ValueTupleHandler();
            }

            if (IsNullableType(type))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return new NullableHandler(underlyingType);
            }

            if (type.IsEnum)
                return new EnumHandler();

            if (type.IsArray)
                return new ArrayHandler();

            if (IsGenericList(type))
                return new ListHandler();

            if (IsGenericDictionary(type))
                return new DictionaryHandler();

            if (type.IsClass || (type.IsValueType && !type.IsPrimitive && !type.IsEnum))
                return new ObjectHandler();

            throw new NotSupportedException($"Type {type} is not supported");
        }
        
        private class ObjectTypeHandler : TypeHandler
        {
            public override void Serialize(BinaryWriter writer, object value)
            {
                if (value == null)
                {
                    writer.Write((byte)0);
                    return;
                }
                
                writer.Write((byte)1);
                
                Type actualType = value.GetType();
                
                writer.Write(actualType.FullName);
                
                var handler = TypeHandlerFactory.GetHandler(actualType);
                handler.Serialize(writer, value);
            }

            public override object Deserialize(BinaryReader reader, Type type)
            {
                var isNotNull = reader.ReadByte();
                if (isNotNull == 0)
                    return null;

                string typeName = reader.ReadString();
                Type actualType = Type.GetType(typeName);
        
                if (actualType == null)
                    throw new InvalidOperationException($"无法找到类型: {typeName}");

                var handler = TypeHandlerFactory.GetHandler(actualType);
                return handler.Deserialize(reader, actualType);
            }
        }

        private static bool IsValueTuple(Type type)
        {
            return type.IsGenericType && type.FullName != null && type.FullName.StartsWith("System.ValueTuple");
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static bool IsGenericList(Type type)
        {
            if (!type.IsGenericType) return false;
            var genericDef = type.GetGenericTypeDefinition();
            return genericDef == typeof(List<>) || genericDef == typeof(IList<>) || genericDef == typeof(ICollection<>);
        }

        private static bool IsGenericDictionary(Type type)
        {
            if (!type.IsGenericType) return false;
            var genericDef = type.GetGenericTypeDefinition();
            return genericDef == typeof(Dictionary<,>) || genericDef == typeof(IDictionary<,>);
        }
    }
    
    /// <summary>
    /// 跳过序列化的标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class BinaryIgnore : Attribute
    {

    }
}