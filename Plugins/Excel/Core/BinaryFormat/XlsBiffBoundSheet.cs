namespace Excel.Core.BinaryFormat
{
    using Excel;
    using Excel.Core;
    using System;
    using System.Text;

    internal class XlsBiffBoundSheet : XlsBiffRecord
    {
        private bool isV8;
        private Encoding m_UseEncoding;

        internal XlsBiffBoundSheet(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
            this.isV8 = true;
            this.m_UseEncoding = Encoding.Default;
        }

        public uint StartOffset =>
            base.ReadUInt32(0);

        public SheetType Type =>
            (SheetType) base.ReadByte(5);

        public SheetVisibility VisibleState =>
            (SheetVisibility) base.ReadByte(4);

        public string SheetName
        {
            get
            {
                ushort count = base.ReadByte(6);
                int num2 = 8;
                return (!this.isV8 ? Encoding.Default.GetString(base.m_bytes, (base.m_readoffset + num2) - 1, count) : ((base.ReadByte(7) != 0) ? this.m_UseEncoding.GetString(base.m_bytes, base.m_readoffset + num2, Helpers.IsSingleByteEncoding(this.m_UseEncoding) ? count : (count * 2)) : Encoding.Default.GetString(base.m_bytes, base.m_readoffset + num2, count)));
            }
        }

        public Encoding UseEncoding
        {
            get => 
                this.m_UseEncoding;
            set => 
                this.m_UseEncoding = value;
        }

        public bool IsV8
        {
            get => 
                this.isV8;
            set => 
                this.isV8 = value;
        }

        public enum SheetType : byte
        {
            Worksheet = 0,
            MacroSheet = 1,
            Chart = 2,
            VBModule = 6
        }

        public enum SheetVisibility : byte
        {
            Visible = 0,
            Hidden = 1,
            VeryHidden = 2
        }
    }
}

