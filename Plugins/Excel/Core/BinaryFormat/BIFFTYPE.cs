namespace Excel.Core.BinaryFormat
{
    using System;

    internal enum BIFFTYPE : ushort
    {
        WorkbookGlobals = 5,
        VBModule = 6,
        Worksheet = 0x10,
        Chart = 0x20,
        v4MacroSheet = 0x40,
        v4WorkbookGlobals = 0x100
    }
}

