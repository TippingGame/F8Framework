namespace Excel.Core.BinaryFormat
{
    using Excel;
    using System;

    internal class XlsBiffRecord
    {
        protected byte[] m_bytes;
        protected readonly ExcelBinaryReader reader;
        protected int m_readoffset;

        protected XlsBiffRecord(byte[] bytes, uint offset, ExcelBinaryReader reader)
        {
            if ((bytes.Length - offset) < 4L)
            {
                throw new ArgumentException("Error: Buffer size is less than minimum BIFF record size.");
            }
            this.m_bytes = bytes;
            this.reader = reader;
            this.m_readoffset = 4 + ((int) offset);
            if ((reader.ReadOption == ReadOption.Strict) && (bytes.Length < (offset + this.Size)))
            {
                throw new ArgumentException("BIFF Stream error: Buffer size is less than entry length.");
            }
        }

        public static XlsBiffRecord GetRecord(byte[] bytes, uint offset, ExcelBinaryReader reader)
        {
            if (offset >= bytes.Length)
            {
                return null;
            }
            BIFFRECORDTYPE biffrecordtype = (BIFFRECORDTYPE) BitConverter.ToUInt16(bytes, (int) offset);
            if (biffrecordtype > BIFFRECORDTYPE.BOOKBOOL)
            {
                if (biffrecordtype > BIFFRECORDTYPE.RK)
                {
                    if (biffrecordtype > BIFFRECORDTYPE.BOF_V4)
                    {
                        if (biffrecordtype == BIFFRECORDTYPE.FORMAT)
                        {
                            goto TR_0002;
                        }
                        else
                        {
                            if (biffrecordtype == BIFFRECORDTYPE.QUICKTIP)
                            {
                                return new XlsBiffQuickTip(bytes, offset, reader);
                            }
                            if (biffrecordtype == BIFFRECORDTYPE.BOF)
                            {
                                goto TR_0009;
                            }
                        }
                    }
                    else if (biffrecordtype == BIFFRECORDTYPE.FORMULA)
                    {
                        goto TR_0008;
                    }
                    else if (biffrecordtype == BIFFRECORDTYPE.BOF_V4)
                    {
                        goto TR_0009;
                    }
                }
                else if (biffrecordtype > BIFFRECORDTYPE.LABELSST)
                {
                    if (biffrecordtype == BIFFRECORDTYPE.USESELFS)
                    {
                        return new XlsBiffSimpleValueRecord(bytes, offset, reader);
                    }
                    switch (biffrecordtype)
                    {
                        case BIFFRECORDTYPE.DIMENSIONS:
                            return new XlsBiffDimensions(bytes, offset, reader);

                        case BIFFRECORDTYPE.BLANK:
                        case BIFFRECORDTYPE.BOOLERR:
                            goto TR_0004;

                        case BIFFRECORDTYPE.INTEGER:
                            goto TR_0005;

                        case BIFFRECORDTYPE.NUMBER:
                            goto TR_0006;

                        case BIFFRECORDTYPE.LABEL:
                            goto TR_0007;

                        case (BIFFRECORDTYPE.DIMENSIONS | BIFFRECORDTYPE.FORMULA_OLD):
                        case (BIFFRECORDTYPE.DIMENSIONS | BIFFRECORDTYPE.EOF):
                            break;

                        case BIFFRECORDTYPE.STRING:
                            return new XlsBiffFormulaString(bytes, offset, reader);

                        case BIFFRECORDTYPE.ROW:
                            return new XlsBiffRow(bytes, offset, reader);

                        case BIFFRECORDTYPE.BOF_V3:
                            goto TR_0009;

                        case BIFFRECORDTYPE.INDEX:
                            return new XlsBiffIndex(bytes, offset, reader);

                        default:
                            if (biffrecordtype != BIFFRECORDTYPE.RK)
                            {
                                break;
                            }
                            return new XlsBiffRKCell(bytes, offset, reader);
                    }
                }
                else
                {
                    if (biffrecordtype == BIFFRECORDTYPE.INTERFACEHDR)
                    {
                        return new XlsBiffInterfaceHdr(bytes, offset, reader);
                    }
                    switch (biffrecordtype)
                    {
                        case BIFFRECORDTYPE.SST:
                            return new XlsBiffSST(bytes, offset, reader);

                        case BIFFRECORDTYPE.LABELSST:
                            return new XlsBiffLabelSSTCell(bytes, offset, reader);

                        default:
                            break;
                    }
                }
                goto TR_0001;
            }
            else if (biffrecordtype > BIFFRECORDTYPE.UNCALCED)
            {
                if (biffrecordtype > BIFFRECORDTYPE.HIDEOBJ)
                {
                    if (biffrecordtype == BIFFRECORDTYPE.FNGROUPCOUNT)
                    {
                        return new XlsBiffSimpleValueRecord(bytes, offset, reader);
                    }
                    switch (biffrecordtype)
                    {
                        case BIFFRECORDTYPE.MULRK:
                            return new XlsBiffMulRKCell(bytes, offset, reader);

                        case BIFFRECORDTYPE.MULBLANK:
                            return new XlsBiffMulBlankCell(bytes, offset, reader);

                        default:
                            switch (biffrecordtype)
                            {
                                case BIFFRECORDTYPE.RSTRING:
                                    goto TR_0007;

                                case BIFFRECORDTYPE.DBCELL:
                                    return new XlsBiffDbCell(bytes, offset, reader);

                                case BIFFRECORDTYPE.BOOKBOOL:
                                    return new XlsBiffSimpleValueRecord(bytes, offset, reader);

                                default:
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    if (biffrecordtype == BIFFRECORDTYPE.BOUNDSHEET)
                    {
                        return new XlsBiffBoundSheet(bytes, offset, reader);
                    }
                    if (biffrecordtype == BIFFRECORDTYPE.HIDEOBJ)
                    {
                        return new XlsBiffSimpleValueRecord(bytes, offset, reader);
                    }
                }
                goto TR_0001;
            }
            else if (biffrecordtype > BIFFRECORDTYPE.FORMAT_V23)
            {
                if (biffrecordtype == BIFFRECORDTYPE.RECORD1904)
                {
                    return new XlsBiffSimpleValueRecord(bytes, offset, reader);
                }
                switch (biffrecordtype)
                {
                    case BIFFRECORDTYPE.CONTINUE:
                        return new XlsBiffContinue(bytes, offset, reader);

                    case BIFFRECORDTYPE.WINDOW1:
                        return new XlsBiffWindow1(bytes, offset, reader);

                    case ((BIFFRECORDTYPE) 0x3e):
                    case ((BIFFRECORDTYPE) 0x3f):
                    case (BIFFRECORDTYPE.BACKUP | BIFFRECORDTYPE.BLANK_OLD):
                        break;

                    case BIFFRECORDTYPE.BACKUP:
                        return new XlsBiffSimpleValueRecord(bytes, offset, reader);

                    case BIFFRECORDTYPE.CODEPAGE:
                        return new XlsBiffSimpleValueRecord(bytes, offset, reader);

                    default:
                        if (biffrecordtype != BIFFRECORDTYPE.UNCALCED)
                        {
                            break;
                        }
                        return new XlsBiffUncalced(bytes, offset, reader);
                }
                goto TR_0001;
            }
            else
            {
                switch (biffrecordtype)
                {
                    case BIFFRECORDTYPE.BLANK_OLD:
                    case BIFFRECORDTYPE.BOOLERR_OLD:
                        goto TR_0004;

                    case BIFFRECORDTYPE.INTEGER_OLD:
                        goto TR_0005;

                    case BIFFRECORDTYPE.NUMBER_OLD:
                        goto TR_0006;

                    case BIFFRECORDTYPE.LABEL_OLD:
                        goto TR_0007;

                    case BIFFRECORDTYPE.FORMULA_OLD:
                        goto TR_0008;

                    case (BIFFRECORDTYPE.BLANK_OLD | BIFFRECORDTYPE.FORMULA_OLD):
                    case ((BIFFRECORDTYPE) 8):
                        break;

                    case BIFFRECORDTYPE.BOF_V2:
                        goto TR_0009;

                    case BIFFRECORDTYPE.EOF:
                        return new XlsBiffEOF(bytes, offset, reader);

                    default:
                        if (biffrecordtype != BIFFRECORDTYPE.FORMAT_V23)
                        {
                            break;
                        }
                        goto TR_0002;
                }
                goto TR_0001;
            }
            TR_0001:
            return new XlsBiffRecord(bytes, offset, reader);
        TR_0002:
            return new XlsBiffFormatString(bytes, offset, reader);
        TR_0004:
            return new XlsBiffBlankCell(bytes, offset, reader);
        TR_0005:
            return new XlsBiffIntegerCell(bytes, offset, reader);
        TR_0006:
            return new XlsBiffNumberCell(bytes, offset, reader);
        TR_0007:
            return new XlsBiffLabelCell(bytes, offset, reader);
        TR_0008:
            return new XlsBiffFormulaCell(bytes, offset, reader);
        TR_0009:
            return new XlsBiffBOF(bytes, offset, reader);
        }

        public byte[] ReadArray(int offset, int size)
        {
            byte[] dst = new byte[size];
            Buffer.BlockCopy(this.m_bytes, this.m_readoffset + offset, dst, 0, size);
            return dst;
        }

        public byte ReadByte(int offset) => 
            Buffer.GetByte(this.m_bytes, this.m_readoffset + offset);

        public double ReadDouble(int offset) => 
            BitConverter.ToDouble(this.m_bytes, this.m_readoffset + offset);

        public float ReadFloat(int offset) => 
            BitConverter.ToSingle(this.m_bytes, this.m_readoffset + offset);

        public short ReadInt16(int offset) => 
            BitConverter.ToInt16(this.m_bytes, this.m_readoffset + offset);

        public int ReadInt32(int offset) => 
            BitConverter.ToInt32(this.m_bytes, this.m_readoffset + offset);

        public long ReadInt64(int offset) => 
            BitConverter.ToInt64(this.m_bytes, this.m_readoffset + offset);

        public ushort ReadUInt16(int offset) => 
            BitConverter.ToUInt16(this.m_bytes, this.m_readoffset + offset);

        public uint ReadUInt32(int offset) => 
            BitConverter.ToUInt32(this.m_bytes, this.m_readoffset + offset);

        public ulong ReadUInt64(int offset) => 
            BitConverter.ToUInt64(this.m_bytes, this.m_readoffset + offset);

        internal byte[] Bytes =>
            this.m_bytes;

        internal int Offset =>
            this.m_readoffset - 4;

        public BIFFRECORDTYPE ID =>
            (BIFFRECORDTYPE) BitConverter.ToUInt16(this.m_bytes, this.m_readoffset - 4);

        public ushort RecordSize =>
            BitConverter.ToUInt16(this.m_bytes, this.m_readoffset - 2);

        public int Size =>
            4 + this.RecordSize;

        public bool IsCell
        {
            get
            {
                bool flag = false;
                BIFFRECORDTYPE iD = this.ID;
                if (iD > BIFFRECORDTYPE.LABELSST)
                {
                    switch (iD)
                    {
                        case BIFFRECORDTYPE.BLANK:
                        case BIFFRECORDTYPE.NUMBER:
                        case BIFFRECORDTYPE.BOOLERR:
                            break;

                        case BIFFRECORDTYPE.INTEGER:
                        case BIFFRECORDTYPE.LABEL:
                            return flag;

                        default:
                            if ((iD == BIFFRECORDTYPE.RK) || (iD == BIFFRECORDTYPE.FORMULA))
                            {
                                break;
                            }
                            return flag;
                    }
                }
                else
                {
                    switch (iD)
                    {
                        case BIFFRECORDTYPE.MULRK:
                        case BIFFRECORDTYPE.MULBLANK:
                            break;

                        default:
                            if (iD == BIFFRECORDTYPE.LABELSST)
                            {
                                break;
                            }
                            return flag;
                    }
                }
                return true;
            }
        }
    }
}

