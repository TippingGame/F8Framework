namespace Excel.Core.BinaryFormat
{
    using Excel;
    using Excel.Core;
    using System;
    using System.Text;

    internal class XlsBiffFormulaCell : XlsBiffNumberCell
    {
        private Encoding m_UseEncoding;

        internal XlsBiffFormulaCell(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
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

        public FormulaFlags Flags =>
            (FormulaFlags) base.ReadUInt16(14);

        public byte FormulaLength =>
            base.ReadByte(15);

        public new object Value
        {
            get
            {
                long num = base.ReadInt64(6);
                if ((num & -281474976710656L) != -281474976710656L)
                {
                    return Helpers.Int64BitsToDouble(num);
                }
                byte num3 = (byte) ((num >> 0x10) & 0xffL);
                switch (((byte) (num & 0xffL)))
                {
                    case 0:
                    {
                        XlsBiffRecord record = GetRecord(base.m_bytes, (uint) (base.Offset + base.Size), base.reader);
                        XlsBiffFormulaString str = (record.ID != BIFFRECORDTYPE.SHRFMLA) ? (record as XlsBiffFormulaString) : (GetRecord(base.m_bytes, (uint) ((base.Offset + base.Size) + record.Size), base.reader) as XlsBiffFormulaString);
                        if (str == null)
                        {
                            return string.Empty;
                        }
                        str.UseEncoding = this.m_UseEncoding;
                        return str.Value;
                    }
                    case 1:
                        return (num3 != 0);

                    case 2:
                        return (FORMULAERROR) num3;
                }
                return null;
            }
        }

        public string Formula
        {
            get
            {
                byte[] bytes = base.ReadArray(0x10, this.FormulaLength);
                return Encoding.Default.GetString(bytes, 0, bytes.Length);
            }
        }

        [Flags]
        public enum FormulaFlags : ushort
        {
            AlwaysCalc = 1,
            CalcOnLoad = 2,
            SharedFormulaGroup = 8
        }
    }
}

