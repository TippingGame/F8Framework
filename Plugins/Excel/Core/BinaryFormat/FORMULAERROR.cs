namespace Excel.Core.BinaryFormat
{
    using System;

    internal enum FORMULAERROR : byte
    {
        NULL = 0,
        DIV0 = 7,
        VALUE = 15,
        REF = 0x17,
        NAME = 0x1d,
        NUM = 0x24,
        NA = 0x2a
    }
}

