namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffQuickTip : XlsBiffRecord
    {
        internal XlsBiffQuickTip(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }
    }
}

