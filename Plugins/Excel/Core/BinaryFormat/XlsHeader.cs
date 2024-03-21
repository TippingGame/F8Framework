namespace Excel.Core.BinaryFormat
{
    using Excel.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class XlsHeader
    {
        private readonly byte[] m_bytes = new byte[0x200];
        private readonly Stream m_file;
        private XlsFat m_fat;
        private XlsFat m_minifat;

        private XlsHeader(Stream file)
        {
            this.m_file = file;
        }

        public XlsFat GetMiniFAT(XlsRootDirectory rootDir)
        {
            if (this.m_minifat == null)
            {
                if ((this.MiniFatSectorCount == 0) || (this.MiniSectorSize == -2))
                {
                    return null;
                }
                int miniSectorSize = this.MiniSectorSize;
                List<uint> sectors = new List<uint>(this.MiniFatSectorCount) {
                    BitConverter.ToUInt32(this.m_bytes, 60)
                };
                this.m_minifat = new XlsFat(this, sectors, this.MiniSectorSize, true, rootDir);
            }
            return this.m_minifat;
        }

        public static XlsHeader ReadHeader(Stream file)
        {
            XlsHeader header = new XlsHeader(file);
            lock (file)
            {
                file.Seek(0L, SeekOrigin.Begin);
                file.Read(header.m_bytes, 0, 0x200);
            }
            if (!header.IsSignatureValid)
            {
                throw new HeaderException("Error: Invalid file signature.");
            }
            if (header.ByteOrder != 0xfffe)
            {
                throw new FormatException("Error: Invalid byte order specified in header.");
            }
            return header;
        }

        public ulong Signature =>
            BitConverter.ToUInt64(this.m_bytes, 0);

        public bool IsSignatureValid =>
            this.Signature == 16220472316735377360UL;

        public Guid ClassId
        {
            get
            {
                byte[] dst = new byte[0x10];
                Buffer.BlockCopy(this.m_bytes, 8, dst, 0, 0x10);
                return new Guid(dst);
            }
        }

        public ushort Version =>
            BitConverter.ToUInt16(this.m_bytes, 0x18);

        public ushort DllVersion =>
            BitConverter.ToUInt16(this.m_bytes, 0x1a);

        public ushort ByteOrder =>
            BitConverter.ToUInt16(this.m_bytes, 0x1c);

        public int SectorSize =>
            1 << (BitConverter.ToUInt16(this.m_bytes, 30) & 0x1f);

        public int MiniSectorSize =>
            1 << (BitConverter.ToUInt16(this.m_bytes, 0x20) & 0x1f);

        public int FatSectorCount =>
            BitConverter.ToInt32(this.m_bytes, 0x2c);

        public uint RootDirectoryEntryStart =>
            BitConverter.ToUInt32(this.m_bytes, 0x30);

        public uint TransactionSignature =>
            BitConverter.ToUInt32(this.m_bytes, 0x34);

        public uint MiniStreamCutoff =>
            BitConverter.ToUInt32(this.m_bytes, 0x38);

        public uint MiniFatFirstSector =>
            BitConverter.ToUInt32(this.m_bytes, 60);

        public int MiniFatSectorCount =>
            BitConverter.ToInt32(this.m_bytes, 0x40);

        public uint DifFirstSector =>
            BitConverter.ToUInt32(this.m_bytes, 0x44);

        public int DifSectorCount =>
            BitConverter.ToInt32(this.m_bytes, 0x48);

        public Stream FileStream =>
            this.m_file;

        public XlsFat FAT
        {
            get
            {
                if (this.m_fat == null)
                {
                    int sectorSize = this.SectorSize;
                    List<uint> sectors = new List<uint>(this.FatSectorCount);
                    int startIndex = 0x4c;
                    while (true)
                    {
                        uint num;
                        if (startIndex < sectorSize)
                        {
                            num = BitConverter.ToUInt32(this.m_bytes, startIndex);
                            if (num != uint.MaxValue)
                            {
                                sectors.Add(num);
                                startIndex += 4;
                                continue;
                            }
                        }
                        else
                        {
                            int difSectorCount = this.DifSectorCount;
                            if (difSectorCount != 0)
                            {
                                lock (this.m_file)
                                {
                                    uint difFirstSector = this.DifFirstSector;
                                    byte[] buffer = new byte[sectorSize];
                                    uint num6 = 0;
                                    while (true)
                                    {
                                        if (difSectorCount > 0)
                                        {
                                            sectors.Capacity += 0x80;
                                            if ((num6 == 0) || ((difFirstSector - num6) != 1))
                                            {
                                                this.m_file.Seek((difFirstSector + 1) * sectorSize, SeekOrigin.Begin);
                                            }
                                            num6 = difFirstSector;
                                            this.m_file.Read(buffer, 0, sectorSize);
                                            int num7 = 0;
                                            while (true)
                                            {
                                                if (num7 < 0x1fc)
                                                {
                                                    num = BitConverter.ToUInt32(buffer, num7);
                                                    if (num != uint.MaxValue)
                                                    {
                                                        sectors.Add(num);
                                                        num7 += 4;
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    num = BitConverter.ToUInt32(buffer, 0x1fc);
                                                    if (num != uint.MaxValue)
                                                    {
                                                        if (difSectorCount-- > 1)
                                                        {
                                                            difFirstSector = num;
                                                        }
                                                        else
                                                        {
                                                            sectors.Add(num);
                                                        }
                                                        continue;
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    this.m_fat = new XlsFat(this, sectors, this.SectorSize, false, null);
                }
                return this.m_fat;
            }
        }
    }
}

