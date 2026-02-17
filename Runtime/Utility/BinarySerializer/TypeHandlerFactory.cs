using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public static class TypeHandlerFactory
    {
        // 存储非泛型 TypeHandler，但实际存储的是泛型实例
        private static readonly Dictionary<Type, TypeHandler> _handlers = new Dictionary<Type, TypeHandler>();

        // 静态构造函数初始化基础类型
        static TypeHandlerFactory()
        {
            // 基础类型
            PreRegister<char>(new CharHandler());
            PreRegister<bool>(new BoolHandler());
            PreRegister<byte>(new ByteHandler());
            PreRegister<sbyte>(new SByteHandler());
            PreRegister<short>(new ShortHandler());
            PreRegister<ushort>(new UShortHandler());
            PreRegister<int>(new IntHandler());
            PreRegister<uint>(new UIntHandler());
            PreRegister<long>(new LongHandler());
            PreRegister<ulong>(new ULongHandler());
            PreRegister<float>(new FloatHandler());
            PreRegister<double>(new DoubleHandler());
            PreRegister<decimal>(new DecimalHandler());
            PreRegister<string>(new StringHandler());

            // 时间类型
            PreRegister<DateTime>(new DateTimeHandler());
            PreRegister<DateTimeOffset>(new DateTimeOffsetHandler());
            PreRegister<TimeSpan>(new TimeSpanHandler());

            // Unity 数学类型
            PreRegister<Vector2>(new Vector2Handler());
            PreRegister<Vector3>(new Vector3Handler());
            PreRegister<Vector4>(new Vector4Handler());
            PreRegister<Vector2Int>(new Vector2IntHandler());
            PreRegister<Vector3Int>(new Vector3IntHandler());
            PreRegister<Quaternion>(new QuaternionHandler());
            PreRegister<Color>(new ColorHandler());
            PreRegister<Color32>(new Color32Handler());
            PreRegister<Rect>(new RectHandler());
            PreRegister<RectInt>(new RectIntHandler());
            PreRegister<Bounds>(new BoundsHandler());
            PreRegister<BoundsInt>(new BoundsIntHandler());

            // object 类型单独注册（非泛型）
            _handlers[typeof(object)] = new ObjectTypeHandler();
        }
        
        public static void PreRegisterType(Type type, TypeHandler typeHandler)
        {
            if (!_handlers.TryAdd(type, typeHandler)) return;
        }
        
        public static void PreRegister<T>(TypeHandler typeHandler) => PreRegisterType(typeof(T), typeHandler);

        internal static TypeHandler CreateHandler(Type type)
        {
            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                if (genericDef == typeof(Nullable<>))
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    var handlerType = typeof(NullableHandler<>).MakeGenericType(underlyingType);
                    return (TypeHandler)Activator.CreateInstance(handlerType);
                }
                if (genericDef == typeof(List<>))
                {
                    var elementType = type.GetGenericArguments()[0];
                    var handlerType = typeof(ListHandler<>).MakeGenericType(elementType);
                    return (TypeHandler)Activator.CreateInstance(handlerType);
                }
                if (genericDef == typeof(Dictionary<,>))
                {
                    var args = type.GetGenericArguments();
                    var handlerType = typeof(DictionaryHandler<,>).MakeGenericType(args[0], args[1]);
                    return (TypeHandler)Activator.CreateInstance(handlerType);
                }
                if (type.FullName.StartsWith("System.ValueTuple"))
                {
                    var args = type.GetGenericArguments();
                    if (args.Length == 1)
                    {
                        var handlerType = typeof(SingleValueTupleHandler<>).MakeGenericType(type);
                        return (TypeHandler)Activator.CreateInstance(handlerType);
                    }
                    else
                    {
                        var handlerType = typeof(ValueTupleHandler<>).MakeGenericType(type);
                        return (TypeHandler)Activator.CreateInstance(handlerType);
                    }
                }
            }

            if (type.IsEnum)
            {
                var handlerType = typeof(EnumHandler<>).MakeGenericType(type);
                return (TypeHandler)Activator.CreateInstance(handlerType);
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var handlerType = typeof(ArrayHandler<>).MakeGenericType(elementType);
                return (TypeHandler)Activator.CreateInstance(handlerType);
            }

            if (type.IsClass || (type.IsValueType && !type.IsPrimitive && !type.IsEnum))
            {
                var handlerType = typeof(ObjectHandler<>).MakeGenericType(type);
                return (TypeHandler)Activator.CreateInstance(handlerType);
            }

            return null;
        }

        internal static TypeHandler GetHandler(Type type)
        {
            if (_handlers.TryGetValue(type, out var handler))
                return handler;

            handler = CreateHandler(type);
            _handlers[type] = handler ??
                              throw new NotSupportedException($"Cannot create handler for type {type}");
            return handler;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class BinaryIgnore : Attribute { }
}