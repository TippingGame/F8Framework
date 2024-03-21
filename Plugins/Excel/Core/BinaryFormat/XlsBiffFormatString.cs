namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;
    using System.Text;

    internal class XlsBiffFormatString : XlsBiffRecord
    {
        private Encoding m_UseEncoding;
        private string m_value;

        internal XlsBiffFormatString(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
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
            (base.ID != BIFFRECORDTYPE.FORMAT_V23) ? base.ReadUInt16(2) : base.ReadByte(0);

        public string Value
        {
            get
            {
                if (this.m_value == null)
                {
                    BIFFRECORDTYPE iD = base.ID;
                    if (iD == BIFFRECORDTYPE.FORMAT_V23)
                    {
                        this.m_value = this.m_UseEncoding.GetString(base.m_bytes, base.m_readoffset + 1, this.Length);
                    }
                    else if (iD == BIFFRECORDTYPE.FORMAT)
                    {
                        int index = base.m_readoffset + 5;
                        byte num2 = base.ReadByte(3);
                        this.m_UseEncoding = ((num2 & 1) == 1) ? Encoding.Unicode : Encoding.Default;
                        if ((num2 & 4) == 1)
                        {
                            index += 4;
                        }
                        if ((num2 & 8) == 1)
                        {
                            index += 2;
                        }
                        this.m_value = this.m_UseEncoding.IsSingleByte ? this.m_UseEncoding.GetString(base.m_bytes, index, this.Length) : this.m_UseEncoding.GetString(base.m_bytes, index, this.Length * 2);
                    }
                }
                return this.m_value;
            }
        }

        public ushort Index =>
            (base.ID != BIFFRECORDTYPE.FORMAT_V23) ? base.ReadUInt16(0) : default;
    }
}

