namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffEOF : XlsBiffRecord
    {
        internal XlsBiffEOF(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }
    }
}

