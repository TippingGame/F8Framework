namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class XlsBiffSST : XlsBiffRecord
    {
        private readonly List<uint> continues;
        private readonly List<string> m_strings;
        private uint m_size;

        internal XlsBiffSST(byte[] bytes, uint offset, ExcelBinaryReader reader) : base(bytes, offset, reader)
        {
            this.continues = new List<uint>();
            this.m_size = base.RecordSize;
            this.m_strings = new List<string>();
        }

        public void Append(XlsBiffContinue fragment)
        {
            this.continues.Add((uint) fragment.Offset);
            this.m_size += (uint) fragment.Size;
        }

        public string GetString(uint SSTIndex) => 
            (SSTIndex >= this.m_strings.Count) ? string.Empty : this.m_strings[(int) SSTIndex];

        public void ReadStrings()
        {
            uint offset = (uint) (base.m_readoffset + 8);
            uint num2 = (uint) (base.m_readoffset + base.RecordSize);
            int num3 = 0;
            uint uniqueCount = this.UniqueCount;
            while (offset < num2)
            {
                XlsFormattedUnicodeString str = new XlsFormattedUnicodeString(base.m_bytes, offset);
                uint headSize = str.HeadSize;
                uint tailSize = str.TailSize;
                uint characterCount = str.CharacterCount;
                uint num8 = ((headSize + tailSize) + characterCount) + (str.IsMultiByte ? characterCount : 0);
                if ((offset + num8) <= num2)
                {
                    offset += num8;
                    if (offset == num2)
                    {
                        if (num3 >= this.continues.Count)
                        {
                            uniqueCount = 1;
                        }
                        else
                        {
                            uint num11 = this.continues[num3];
                            offset = num11 + 4;
                            num2 = offset + BitConverter.ToUInt16(base.m_bytes, ((int) num11) + 2);
                            num3++;
                        }
                    }
                }
                else
                {
                    if (num3 >= this.continues.Count)
                    {
                        return;
                    }
                    uint num9 = this.continues[num3];
                    byte @byte = Buffer.GetByte(base.m_bytes, ((int) num9) + 4);
                    byte[] dst = new byte[num8 * 2];
                    Buffer.BlockCopy(base.m_bytes, (int) offset, dst, 0, (int) (num2 - offset));
                    if ((@byte == 0) && str.IsMultiByte)
                    {
                        characterCount -= ((num2 - headSize) - offset) / 2;
                        string s = Encoding.Default.GetString(base.m_bytes, ((int) num9) + 5, (int) characterCount);
                        byte[] bytes = Encoding.Unicode.GetBytes(s);
                        Buffer.BlockCopy(bytes, 0, dst, (int) (num2 - offset), bytes.Length);
                        Buffer.BlockCopy(base.m_bytes, (int) ((num9 + 5) + characterCount), dst, (int) (((num2 - offset) + characterCount) + characterCount), (int) tailSize);
                        offset = ((num9 + 5) + characterCount) + tailSize;
                    }
                    else if ((@byte != 1) || str.IsMultiByte)
                    {
                        Buffer.BlockCopy(base.m_bytes, ((int) num9) + 5, dst, (int) (num2 - offset), (int) ((num8 - num2) + offset));
                        offset = (((num9 + 5) + num8) - num2) + offset;
                    }
                    else
                    {
                        characterCount -= (num2 - offset) - headSize;
                        string s = Encoding.Unicode.GetString(base.m_bytes, ((int) num9) + 5, (int) (characterCount + characterCount));
                        byte[] bytes = Encoding.Default.GetBytes(s);
                        Buffer.BlockCopy(bytes, 0, dst, (int) (num2 - offset), bytes.Length);
                        Buffer.BlockCopy(base.m_bytes, (int) (((num9 + 5) + characterCount) + characterCount), dst, (int) ((num2 - offset) + characterCount), (int) tailSize);
                        offset = (((num9 + 5) + characterCount) + characterCount) + tailSize;
                    }
                    num2 = (num9 + 4) + BitConverter.ToUInt16(base.m_bytes, ((int) num9) + 2);
                    num3++;
                    str = new XlsFormattedUnicodeString(dst, 0);
                }
                this.m_strings.Add(str.Value);
                if ((uniqueCount - 1) == 0)
                {
                    return;
                }
            }
        }

        public uint Count =>
            base.ReadUInt32(0);

        public uint UniqueCount =>
            base.ReadUInt32(4);
    }
}

