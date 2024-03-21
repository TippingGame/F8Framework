namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffBOF : XlsBiffRecord
    {
        internal XlsBiffBOF(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public ushort Version =>
            base.ReadUInt16(0);

        public BIFFTYPE Type =>
            (BIFFTYPE) base.ReadUInt16(2);

        public ushort CreationID =>
            (base.RecordSize >= 6) ? base.ReadUInt16(4) : default;

        public ushort CreationYear =>
            (base.RecordSize >= 8) ? base.ReadUInt16(6) : default;

        public uint HistoryFlag =>
            (base.RecordSize >= 12) ? base.ReadUInt32(8) : default;

        public uint MinVersionToOpen =>
            (base.RecordSize >= 0x10) ? base.ReadUInt32(12) : default;
    }
}

