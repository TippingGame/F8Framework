namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffContinue : XlsBiffRecord
    {
        internal XlsBiffContinue(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }
    }
}

