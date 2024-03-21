namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;
    using System.Collections.Generic;

    internal class XlsBiffIndex : XlsBiffRecord
    {
        private bool isV8;

        internal XlsBiffIndex(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
            this.isV8 = true;
        }

        public bool IsV8
        {
            get => 
                this.isV8;
            set => 
                this.isV8 = value;
        }

        public uint FirstExistingRow =>
            this.isV8 ? base.ReadUInt32(4) : base.ReadUInt16(4);

        public uint LastExistingRow =>
            this.isV8 ? base.ReadUInt32(8) : base.ReadUInt16(6);

        public uint[] DbCellAddresses
        {
            get
            {
                int recordSize = base.RecordSize;
                int num2 = this.isV8 ? 0x10 : 12;
                if (recordSize <= num2)
                {
                    return new uint[0];
                }
                List<uint> list = new List<uint>((recordSize - num2) / 4);
                for (int i = num2; i < recordSize; i += 4)
                {
                    list.Add(base.ReadUInt32(i));
                }
                return list.ToArray();
            }
        }
    }
}

