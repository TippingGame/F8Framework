namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffBlankCell : XlsBiffRecord
    {
        internal XlsBiffBlankCell(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public ushort RowIndex =>
            base.ReadUInt16(0);

        public ushort ColumnIndex =>
            base.ReadUInt16(2);

        public ushort XFormat =>
            base.ReadUInt16(4);
    }
}

