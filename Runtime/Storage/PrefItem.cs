namespace F8Framework.Core
{
    internal abstract class PrefItem
    {
        internal abstract int GetRawBytesLength();
        internal abstract byte[] ExportRawBytes();
        internal abstract bool ImportRawBytes(byte[] bytes);
    }
}