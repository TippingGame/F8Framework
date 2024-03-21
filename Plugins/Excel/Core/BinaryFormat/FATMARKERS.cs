namespace Excel.Core.BinaryFormat
{
    using System;

    internal enum FATMARKERS : uint
    {
        FAT_EndOfChain = 0xfffffffe,
        FAT_FreeSpace = 0xffffffff,
        FAT_FatSector = 0xfffffffd,
        FAT_DifSector = 0xfffffffc
    }
}

