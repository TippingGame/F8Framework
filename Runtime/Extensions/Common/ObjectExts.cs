namespace F8Framework.Core
{
    public static class ObjectExts
    {
        public static T As<T>(this object @this) where T : class
        {
            return @this as T;
        }
        public static T CastTo<T>(this object @this) where T : class
        {
            return (T)@this;
        }
    }
}
