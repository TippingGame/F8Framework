namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffSimpleValueRecord : XlsBiffRecord
    {
        internal XlsBiffSimpleValueRecord(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public ushort Value =>
            base.ReadUInt16(0);
    }
}

