using System;

namespace MessagePack.Resolvers
{
    using Formatters;
    using Internal;
    public sealed class OldSpecResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new OldSpecResolver();

        OldSpecResolver()
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
                formatter = (IMessagePackFormatter<T>)OldSpecResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }
}

namespace MessagePack.Internal
{
    using Formatters;
    internal static class OldSpecResolverGetFormatterHelper
    {
        internal static object GetFormatter(Type t)
        {
            if (t == typeof(string))
            {
                return OldSpecStringFormatter.Instance;
            }
            else if (t == typeof(string[]))
            {
                return new ArrayFormatter<string>();
            }
            else if (t == typeof(byte[]))
            {
                return OldSpecBinaryFormatter.Instance;
            }

            return null;
        }
    }
}