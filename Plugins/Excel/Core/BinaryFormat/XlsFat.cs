namespace Excel.Core.BinaryFormat
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class XlsFat
    {
        private readonly List<uint> m_fat;
        private readonly XlsHeader m_hdr;
        private readonly int m_sectors;
        private readonly int m_sectors_for_fat;
        private readonly int m_sectorSize;
        private readonly bool m_isMini;
        private readonly XlsRootDirectory m_rootDir;

        public XlsFat(XlsHeader hdr, List<uint> sectors, int sizeOfSector, bool isMini, XlsRootDirectory rootDir)
        {
            this.m_isMini = isMini;
            this.m_rootDir = rootDir;
            this.m_hdr = hdr;
            this.m_sectors_for_fat = sectors.Count;
            sizeOfSector = hdr.SectorSize;
            uint num = 0;
            uint num2 = 0;
            byte[] buffer = new byte[sizeOfSector];
            Stream fileStream = hdr.FileStream;
            using (MemoryStream stream2 = new MemoryStream(sizeOfSector * this.m_sectors_for_fat))
            {
                lock (fileStream)
                {
                    for (int i = 0; i < sectors.Count; i++)
                    {
                        num = sectors[i];
                        if ((num2 == 0) || ((num - num2) != 1))
                        {
                            fileStream.Seek((num + 1) * sizeOfSector, SeekOrigin.Begin);
                        }
                        num2 = num;
                        fileStream.Read(buffer, 0, sizeOfSector);
                        stream2.Write(buffer, 0, sizeOfSector);
                    }
                }
                stream2.Seek(0L, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(stream2);
                this.m_sectors = ((int) stream2.Length) / 4;
                this.m_fat = new List<uint>(this.m_sectors);
                int num4 = 0;
                while (true)
                {
                    if (num4 >= this.m_sectors)
                    {
                        reader.Close();
                        stream2.Close();
                        break;
                    }
                    this.m_fat.Add(reader.ReadUInt32());
                    num4++;
                }
            }
        }

        public uint GetNextSector(uint sector)
        {
            if (this.m_fat.Count <= sector)
            {
                throw new ArgumentException("Error reading as FAT table : There's no such sector in FAT.");
            }
            uint num = this.m_fat[(int) sector];
#pragma warning disable CS0652
            if ((num == -3) || (num == -4))
#pragma warning restore CS0652
            {
                throw new InvalidOperationException("Error reading stream from FAT area.");
            }
            return num;
        }

        public int SectorsForFat =>
            this.m_sectors_for_fat;

        public int SectorsCount =>
            this.m_sectors;

        public XlsHeader Header =>
            this.m_hdr;
    }
}

