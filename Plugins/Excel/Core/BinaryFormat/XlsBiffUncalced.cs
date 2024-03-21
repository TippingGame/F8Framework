namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffUncalced : XlsBiffRecord
    {
        internal XlsBiffUncalced(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }
    }
}

