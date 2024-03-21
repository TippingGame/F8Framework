namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal class XlsBiffStream : XlsStream
    {
        private readonly ExcelBinaryReader reader;
        private readonly byte[] bytes;
        private readonly int m_size;
        private int m_offset;

        public XlsBiffStream(XlsHeader hdr, uint streamStart, bool isMini, XlsRootDirectory rootDir, ExcelBinaryReader reader) : base(hdr, streamStart, isMini, rootDir)
        {
            this.reader = reader;
            this.bytes = base.ReadStream();
            this.m_size = this.bytes.Length;
            this.m_offset = 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public XlsBiffRecord Read()
        {
            if (((ulong) this.m_offset) >= (ulong)this.bytes.Length)
            {
                return null;
            }
            XlsBiffRecord record = XlsBiffRecord.GetRecord(this.bytes, (uint) this.m_offset, this.reader);
            this.m_offset += record.Size;
            return ((this.m_offset <= this.m_size) ? record : null);
        }

        public XlsBiffRecord ReadAt(int offset)
        {
            if (((ulong) offset) >= (ulong)this.bytes.Length)
            {
                return null;
            }
            XlsBiffRecord record = XlsBiffRecord.GetRecord(this.bytes, (uint) offset, this.reader);
            return (((this.reader.ReadOption != ReadOption.Strict) || ((this.m_offset + record.Size) <= this.m_size)) ? record : null);
        }

        [Obsolete("Use BIFF-specific methods for this stream")]
        public new byte[] ReadStream() => 
            this.bytes;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.m_offset = offset;
                    break;

                case SeekOrigin.Current:
                    this.m_offset += offset;
                    break;

                case SeekOrigin.End:
                    this.m_offset = this.m_size - offset;
                    break;

                default:
                    break;
            }
            if (this.m_offset < 0)
            {
                throw new ArgumentOutOfRangeException($"{"BIFF Stream error: Moving before stream start."} On offset={offset}");
            }
            if (this.m_offset > this.m_size)
            {
                throw new ArgumentOutOfRangeException($"{"BIFF Stream error: Moving after stream end."} On offset={offset}");
            }
        }

        public int Size =>
            this.m_size;

        public int Position =>
            this.m_offset;
    }
}

