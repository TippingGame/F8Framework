namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffRow : XlsBiffRecord
    {
        internal XlsBiffRow(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public ushort RowIndex =>
            base.ReadUInt16(0);

        public ushort FirstDefinedColumn =>
            base.ReadUInt16(2);

        public ushort LastDefinedColumn =>
            base.ReadUInt16(4);

        public uint RowHeight =>
            base.ReadUInt16(6);

        public ushort Flags =>
            base.ReadUInt16(12);

        public ushort XFormat =>
            base.ReadUInt16(14);
    }
}

