namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffWindow1 : XlsBiffRecord
    {
        internal XlsBiffWindow1(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
        }

        public ushort Left =>
            base.ReadUInt16(0);

        public ushort Top =>
            base.ReadUInt16(2);

        public ushort Width =>
            base.ReadUInt16(4);

        public ushort Height =>
            base.ReadUInt16(6);

        public Window1Flags Flags =>
            (Window1Flags) base.ReadUInt16(8);

        public ushort ActiveTab =>
            base.ReadUInt16(10);

        public ushort FirstVisibleTab =>
            base.ReadUInt16(12);

        public ushort SelectedTabCount =>
            base.ReadUInt16(14);

        public ushort TabRatio =>
            base.ReadUInt16(0x10);

        [Flags]
        public enum Window1Flags : ushort
        {
            Hidden = 1,
            Minimized = 2,
            HScrollVisible = 8,
            VScrollVisible = 0x10,
            WorkbookTabs = 0x20
        }
    }
}

