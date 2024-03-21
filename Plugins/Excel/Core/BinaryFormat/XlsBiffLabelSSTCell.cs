namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffLabelSSTCell : XlsBiffBlankCell
    {
        internal XlsBiffLabelSSTCell(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public string Text(XlsBiffSST sst) => 
            sst.GetString(this.SSTIndex);

        public uint SSTIndex =>
            base.ReadUInt32(6);
    }
}

