namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffIntegerCell : XlsBiffBlankCell
    {
        internal XlsBiffIntegerCell(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public uint Value =>
            base.ReadUInt16(6);
    }
}

