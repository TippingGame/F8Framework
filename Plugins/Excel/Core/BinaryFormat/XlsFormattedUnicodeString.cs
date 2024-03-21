namespace Excel.Core.BinaryFormat
{
    using System;
    using System.Text;

    internal class XlsFormattedUnicodeString
    {
        protected byte[] m_bytes;
        protected uint m_offset;

        public XlsFormattedUnicodeString(byte[] bytes, uint offset)
        {
            this.m_bytes = bytes;
            this.m_offset = offset;
        }

        public ushort CharacterCount =>
            BitConverter.ToUInt16(this.m_bytes, (int) this.m_offset);

        public FormattedUnicodeStringFlags Flags =>
            (FormattedUnicodeStringFlags) Buffer.GetByte(this.m_bytes, ((int) this.m_offset) + 2);

        public bool HasExtString =>
            false;

        public bool HasFormatting =>
            ((byte) (this.Flags & FormattedUnicodeStringFlags.HasFormatting)) == 8;

        public bool IsMultiByte =>
            ((byte) (this.Flags & FormattedUnicodeStringFlags.MultiByte)) == 1;

        private uint ByteCount =>
            (uint) (this.CharacterCount * (this.IsMultiByte ? 2 : 1));

        public ushort FormatCount =>
            this.HasFormatting ? BitConverter.ToUInt16(this.m_bytes, ((int) this.m_offset) + 3) : default;

        public uint ExtendedStringSize =>
            this.HasExtString ? BitConverter.ToUInt16(this.m_bytes, ((int) this.m_offset) + (this.HasFormatting ? 5 : 3)) : default;

        public uint HeadSize =>
            (uint) (((this.HasFormatting ? 2 : 0) + (this.HasExtString ? 4 : 0)) + 3);

        public uint TailSize =>
            (this.HasFormatting ? ((uint) (4 * this.FormatCount)) : 0) + (this.HasExtString ? this.ExtendedStringSize : 0);

        public uint Size
        {
            get
            {
                uint num = (uint) (((this.HasFormatting ? (2 + (this.FormatCount * 4)) : 0) + (this.HasExtString ? (4 + this.ExtendedStringSize) : 0)) + 3);
                return (this.IsMultiByte ? (num + ((uint) (this.CharacterCount * 2))) : (num + this.CharacterCount));
            }
        }

        public string Value =>
            this.IsMultiByte ? Encoding.Unicode.GetString(this.m_bytes, (int) (this.m_offset + this.HeadSize), (int) this.ByteCount) : Encoding.Default.GetString(this.m_bytes, (int) (this.m_offset + this.HeadSize), (int) this.ByteCount);

        [Flags]
        public enum FormattedUnicodeStringFlags : byte
        {
            MultiByte = 1,
            HasExtendedString = 4,
            HasFormatting = 8
        }
    }
}

