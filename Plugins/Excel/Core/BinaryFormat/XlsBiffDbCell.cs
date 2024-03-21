namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;
    using System.Collections.Generic;

    internal class XlsBiffDbCell : XlsBiffRecord
    {
        internal XlsBiffDbCell(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public int RowAddress =>
            base.Offset - base.ReadInt32(0);

        public uint[] CellAddresses
        {
            get
            {
                int num = this.RowAddress - 20;
                List<uint> list = new List<uint>();
                for (int i = 4; i < base.RecordSize; i += 4)
                {
                    list.Add((uint) (num + base.ReadUInt16(i)));
                }
                return list.ToArray();
            }
        }
    }
}

