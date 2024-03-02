#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace F8Framework.Core
{
    public class CacheStorageResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new CacheStorageResolver();

        private CacheStorageResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                var f = CacheStorageGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class CacheStorageGetFormatterHelper
    {
        private static readonly Dictionary<Type, int> lookup;

        static CacheStorageGetFormatterHelper()
        {
            lookup = new Dictionary<Type, int>(6)
            {
                { typeof(List<CacheInfo>), 0 },
                { typeof(CacheControl), 1 },
                { typeof(CacheInfo), 2 },
                { typeof(CachePackage), 3 },
                { typeof(StringToValue<int>), 4 },
                { typeof(StringToValueHttpTime), 5 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (lookup.TryGetValue(t, out key) == false)
            {
                return null;
            }

            switch (key)
            {
                case 0:
                {
                    return new ListFormatter<CacheInfo>();
                }
                case 1:
                {
                    return new CacheControlFormatter();
                }
                case 2:
                {
                    return new CacheInfoFormatter();
                }
                case 3:
                {
                    return new CachePackageFormatter();
                }
                case 4:
                {
                    return new StringToValueIntFormatter();
                }
                case 5:
                {
                    return new StringToValueHttpTimeFormatter();
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612


#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace F8Framework.Core
{
    public sealed class CacheControlFormatter : IMessagePackFormatter<CacheControl>
    {
        public int Serialize(ref byte[] bytes, int offset, CacheControl value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 4);
            offset += formatterResolver.GetFormatterWithVerify<StringToValue<int>>()
                .Serialize(ref bytes, offset, value.maxAge, formatterResolver);
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.noCache);
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.noStore);
            offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.mustRevalidate);
            return offset - startOffset;
        }

        public CacheControl Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver,
            out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset) == true)
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var length = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __maxAge__ = default(StringToValue<int>);
            var __noCache__ = default(bool);
            var __noStore__ = default(bool);
            var __mustRevalidate__ = default(bool);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                    {
                        __maxAge__ = formatterResolver.GetFormatterWithVerify<StringToValue<int>>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 1:
                    {
                        __noCache__ = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                        break;
                    }
                    case 2:
                    {
                        __noStore__ = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                        break;
                    }
                    case 3:
                    {
                        __mustRevalidate__ = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                        break;
                    }
                    default:
                    {
                        readSize = MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                    }
                }

                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new CacheControl();
            ____result.maxAge = __maxAge__;
            ____result.noCache = __noCache__;
            ____result.noStore = __noStore__;
            ____result.mustRevalidate = __mustRevalidate__;
            return ____result;
        }
    }


    public sealed class CacheInfoFormatter : IMessagePackFormatter<CacheInfo>
    {
        public int Serialize(ref byte[] bytes, int offset, CacheInfo value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 12);
            offset += formatterResolver.GetFormatterWithVerify<string>()
                .Serialize(ref bytes, offset, value.url, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<string>()
                .Serialize(ref bytes, offset, value.eTag, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                .Serialize(ref bytes, offset, value.lastModified, formatterResolver);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.lastAccess);
            offset += formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                .Serialize(ref bytes, offset, value.expires, formatterResolver);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.requestTime);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.responseTime);
            offset += formatterResolver.GetFormatterWithVerify<StringToValue<int>>()
                .Serialize(ref bytes, offset, value.age, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                .Serialize(ref bytes, offset, value.date, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<CacheControl>()
                .Serialize(ref bytes, offset, value.cacheControl, formatterResolver);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.contentLength);
            offset += MessagePackBinary.WriteUInt64(ref bytes, offset, value.index);
            return offset - startOffset;
        }

        public CacheInfo Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset) == true)
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var length = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __url__ = default(string);
            var __eTag__ = default(string);
            var __lastModified__ = default(StringToValueHttpTime);
            var __lastAccess__ = default(long);
            var __expires__ = default(StringToValueHttpTime);
            var __requestTime__ = default(long);
            var __responseTime__ = default(long);
            var __age__ = default(StringToValue<int>);
            var __date__ = default(StringToValueHttpTime);
            var __cacheControl__ = default(CacheControl);
            var __contentLength__ = default(long);
            var __index__ = default(ulong);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                    {
                        __url__ = formatterResolver.GetFormatterWithVerify<string>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 1:
                    {
                        __eTag__ = formatterResolver.GetFormatterWithVerify<string>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 2:
                    {
                        __lastModified__ = formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 3:
                    {
                        __lastAccess__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    }
                    case 4:
                    {
                        __expires__ = formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 5:
                    {
                        __requestTime__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    }
                    case 6:
                    {
                        __responseTime__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    }
                    case 7:
                    {
                        __age__ = formatterResolver.GetFormatterWithVerify<StringToValue<int>>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 8:
                    {
                        __date__ = formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 9:
                    {
                        __cacheControl__ = formatterResolver.GetFormatterWithVerify<CacheControl>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 10:
                    {
                        __contentLength__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    }
                    case 11:
                    {
                        __index__ = MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
                        break;
                    }
                    default:
                    {
                        readSize = MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                    }
                }

                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new CacheInfo();
            ____result.url = __url__;
            ____result.eTag = __eTag__;
            ____result.lastModified = __lastModified__;
            ____result.lastAccess = __lastAccess__;
            ____result.expires = __expires__;
            ____result.requestTime = __requestTime__;
            ____result.responseTime = __responseTime__;
            ____result.age = __age__;
            ____result.date = __date__;
            ____result.cacheControl = __cacheControl__;
            ____result.contentLength = __contentLength__;
            ____result.index = __index__;
            return ____result;
        }
    }


    public sealed class CachePackageFormatter : IMessagePackFormatter<CachePackage>
    {
        public int Serialize(ref byte[] bytes, int offset, CachePackage value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 6);
            offset += formatterResolver.GetFormatterWithVerify<List<CacheInfo>>()
                .Serialize(ref bytes, offset, value.cacheStorage, formatterResolver);
            offset += MessagePackBinary.WriteUInt64(ref bytes, offset, value.lastIndex);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.cachedSize);
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.removeCacheSize);
            offset += formatterResolver.GetFormatterWithVerify<List<CacheInfo>>()
                .Serialize(ref bytes, offset, value.removeCache, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                .Serialize(ref bytes, offset, value.lastRemoveTime, formatterResolver);
            return offset - startOffset;
        }

        public CachePackage Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver,
            out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset) == true)
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var length = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __cacheStorage__ = default(List<CacheInfo>);
            var __lastIndex__ = default(ulong);
            var __cachedSize__ = default(long);
            var __removeCacheSize__ = default(long);
            var __removeCache__ = default(List<CacheInfo>);
            var __lastRemoveTime__ = default(StringToValueHttpTime);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                    {
                        __cacheStorage__ = formatterResolver.GetFormatterWithVerify<List<CacheInfo>>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 1:
                    {
                        __lastIndex__ = MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
                        break;
                    }
                    case 2:
                    {
                        __cachedSize__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    }
                    case 3:
                    {
                        __removeCacheSize__ = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                        break;
                    }
                    case 4:
                    {
                        __removeCache__ = formatterResolver.GetFormatterWithVerify<List<CacheInfo>>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    case 5:
                    {
                        __lastRemoveTime__ = formatterResolver.GetFormatterWithVerify<StringToValueHttpTime>()
                            .Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    }
                    default:
                    {
                        readSize = MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                    }
                }

                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new CachePackage();
            ____result.cacheStorage = __cacheStorage__;
            ____result.lastIndex = __lastIndex__;
            ____result.cachedSize = __cachedSize__;
            ____result.removeCacheSize = __removeCacheSize__;
            ____result.removeCache = __removeCache__;
            ____result.lastRemoveTime = __lastRemoveTime__;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612