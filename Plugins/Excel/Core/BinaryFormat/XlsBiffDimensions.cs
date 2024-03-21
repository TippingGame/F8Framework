namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffDimensions : XlsBiffRecord
    {
        private bool isV8;

        internal XlsBiffDimensions(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
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

        public uint FirstRow =>
            this.isV8 ? base.ReadUInt32(0) : base.ReadUInt16(0);

        public uint LastRow =>
            this.isV8 ? base.ReadUInt32(4) : base.ReadUInt16(2);

        public ushort FirstColumn =>
            this.isV8 ? base.ReadUInt16(8) : base.ReadUInt16(4);

        public ushort LastColumn
        {
            get => 
                this.isV8 ? ((ushort) ((base.ReadUInt16(9) >> 8) + 1)) : base.ReadUInt16(6);
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

