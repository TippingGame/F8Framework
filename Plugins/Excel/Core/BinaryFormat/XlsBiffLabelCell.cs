namespace Excel.Core.BinaryFormat
{
    using Excel;
    using Excel.Core;
    using System;
    using System.Text;

    internal class XlsBiffLabelCell : XlsBiffBlankCell
    {
        private Encoding m_UseEncoding;

        internal XlsBiffLabelCell(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
            this.m_UseEncoding = Encoding.Default;
        }

        public Encoding UseEncoding
        {
            get => 
                this.m_UseEncoding;
            set => 
                this.m_UseEncoding = value;
        }

        public ushort Length =>
            base.ReadUInt16(6);

        public string Value
        {
            get
            {
                byte[] bytes = !base.reader.isV8() ? this.ReadArray(2, this.Length * (Helpers.IsSingleByteEncoding(this.m_UseEncoding) ? 1 : 2)) : this.ReadArray(9, this.Length * (Helpers.IsSingleByteEncoding(this.m_UseEncoding) ? 1 : 2));
                return this.m_UseEncoding.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}

