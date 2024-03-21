namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffMulRKCell : XlsBiffBlankCell
    {
        internal XlsBiffMulRKCell(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public double GetValue(ushort ColumnIdx)
        {
            int offset = 6 + (6 * (ColumnIdx - base.ColumnIndex));
            return ((offset <= base.RecordSize) ? XlsBiffRKCell.NumFromRK(base.ReadUInt32(offset)) : 0.0);
        }

        public ushort GetXF(ushort ColumnIdx)
        {
            int offset = 4 + (6 * (ColumnIdx - base.ColumnIndex));
            return ((offset <= (base.RecordSize - 2)) ? base.ReadUInt16(offset) : default);
        }

        public ushort LastColumnIndex =>
            base.ReadUInt16(base.RecordSize - 2);
    }
}

