namespace Excel
{
    using Excel.Core;
    using Excel.Core.BinaryFormat;
    using Excel.Exceptions;
    using Excel.Log;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    public class ExcelBinaryReader : IExcelDataReader, IDataReader, IDisposable, IDataRecord
    {
        private const string WORKBOOK = "Workbook";
        private const string BOOK = "Book";
        private const string COLUMN = "Column";
        private Stream m_file;
        private XlsHeader m_hdr;
        private List<XlsWorksheet> m_sheets;
        private XlsBiffStream m_stream;
        private DataSet m_workbookData;
        private XlsWorkbookGlobals m_globals;
        private ushort m_version;
        private bool m_ConvertOADate;
        private Encoding m_encoding;
        private bool m_isValid;
        private bool m_isClosed;
        private readonly Encoding m_Default_Encoding;
        private string m_exceptionMessage;
        private object[] m_cellsValues;
        private uint[] m_dbCellAddrs;
        private int m_dbCellAddrsIndex;
        private bool m_canRead;
        private int m_SheetIndex;
        private int m_depth;
        private int m_cellOffset;
        private int m_maxCol;
        private int m_maxRow;
        private bool m_noIndex;
        private XlsBiffRow m_currentRowRecord;
        private readonly Excel.ReadOption m_ReadOption;
        private bool m_IsFirstRead;
        private bool _isFirstRowAsColumnNames;
        private bool disposed;

        internal ExcelBinaryReader()
        {
            this.m_Default_Encoding = Encoding.UTF8;
            this.m_encoding = this.m_Default_Encoding;
            this.m_version = 0x600;
            this.m_isValid = true;
            this.m_SheetIndex = -1;
            this.m_IsFirstRead = true;
        }

        internal ExcelBinaryReader(Excel.ReadOption readOption) : this()
        {
            this.m_ReadOption = readOption;
        }

        public DataSet AsDataSet() => 
            this.AsDataSet(false);

        public DataSet AsDataSet(bool convertOADateTime)
        {
            if (!this.m_isValid)
            {
                return null;
            }
            if (!this.m_isClosed)
            {
                this.ConvertOaDate = convertOADateTime;
                this.m_workbookData = new DataSet();
                for (int i = 0; i < this.ResultsCount; i++)
                {
                    DataTable table = this.readWholeWorkSheet(this.m_sheets[i]);
                    if (table != null)
                    {
                        table.ExtendedProperties.Add("visiblestate", this.m_sheets[i].VisibleState);
                        this.m_workbookData.Tables.Add(table);
                    }
                }
                this.m_file.Close();
                this.m_isClosed = true;
                this.m_workbookData.AcceptChanges();
                Helpers.FixDataTypes(this.m_workbookData);
            }
            return this.m_workbookData;
        }

        public void Close()
        {
            this.m_file.Close();
            this.m_isClosed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.m_workbookData != null)
                    {
                        this.m_workbookData.Dispose();
                    }
                    if (this.m_sheets != null)
                    {
                        this.m_sheets.Clear();
                    }
                }
                this.m_workbookData = null;
                this.m_sheets = null;
                this.m_stream = null;
                this.m_globals = null;
                this.m_encoding = null;
                this.m_hdr = null;
                this.disposed = true;
            }
        }

        private void DumpBiffRecords()
        {
            XlsBiffRecord record = null;
            int position = this.m_stream.Position;
            while (true)
            {
                record = this.m_stream.Read();
                LogManager.Log<ExcelBinaryReader>(this).Debug(record.ID.ToString(), new object[0]);
                if ((record == null) || (this.m_stream.Position >= this.m_stream.Size))
                {
                    this.m_stream.Seek(position, SeekOrigin.Begin);
                    return;
                }
            }
        }

        private void fail(string message)
        {
            this.m_exceptionMessage = message;
            this.m_isValid = false;
            this.m_file.Close();
            this.m_isClosed = true;
            this.m_workbookData = null;
            this.m_sheets = null;
            this.m_stream = null;
            this.m_globals = null;
            this.m_encoding = null;
            this.m_hdr = null;
        }

        ~ExcelBinaryReader()
        {
            this.Dispose(false);
        }

        private int findFirstDataCellOffset(int startOffset)
        {
            XlsBiffRecord record = this.m_stream.ReadAt(startOffset);
            while (!(record is XlsBiffDbCell))
            {
                if (this.m_stream.Position >= this.m_stream.Size)
                {
                    return -1;
                }
                if (record is XlsBiffEOF)
                {
                    return -1;
                }
                record = this.m_stream.Read();
            }
            XlsBiffRow row = null;
            int rowAddress = ((XlsBiffDbCell) record).RowAddress;
            while (true)
            {
                row = this.m_stream.ReadAt(rowAddress) as XlsBiffRow;
                if (row != null)
                {
                    rowAddress += row.Size;
                    if (row != null)
                    {
                        continue;
                    }
                }
                return rowAddress;
            }
        }

        public bool GetBoolean(int i) => 
            !this.IsDBNull(i) ? bool.Parse(this.m_cellsValues[i].ToString()) : false;

        public byte GetByte(int i)
        {
            throw new NotSupportedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            throw new NotSupportedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotSupportedException();
        }

        public DateTime GetDateTime(int i)
        {
            double num;
            if (this.IsDBNull(i))
            {
                return DateTime.MinValue;
            }
            object obj2 = this.m_cellsValues[i];
            if (obj2 is DateTime)
            {
                return (DateTime) obj2;
            }
            string s = obj2.ToString();
            try
            {
                num = double.Parse(s);
            }
            catch (FormatException)
            {
                return DateTime.Parse(s);
            }
            return DateTime.FromOADate(num);
        }

        public decimal GetDecimal(int i) => 
            !this.IsDBNull(i) ? decimal.Parse(this.m_cellsValues[i].ToString()) : decimal.MinValue;

        public double GetDouble(int i) => 
            !this.IsDBNull(i) ? double.Parse(this.m_cellsValues[i].ToString()) : double.MinValue;

        public Type GetFieldType(int i)
        {
            throw new NotSupportedException();
        }

        public float GetFloat(int i) => 
            !this.IsDBNull(i) ? float.Parse(this.m_cellsValues[i].ToString()) : float.MinValue;

        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(int i) => 
            (short)(!this.IsDBNull(i) ? short.Parse(this.m_cellsValues[i].ToString()) : -32768);

        public int GetInt32(int i) => 
            !this.IsDBNull(i) ? int.Parse(this.m_cellsValues[i].ToString()) : -2147483648;

        public long GetInt64(int i) => 
            !this.IsDBNull(i) ? long.Parse(this.m_cellsValues[i].ToString()) : -9223372036854775808L;

        public string GetName(int i)
        {
            throw new NotSupportedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotSupportedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        public string GetString(int i) => 
            !this.IsDBNull(i) ? this.m_cellsValues[i].ToString() : null;

        public object GetValue(int i) => 
            this.m_cellsValues[i];

        public int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public void Initialize(Stream fileStream)
        {
            this.m_file = fileStream;
            this.readWorkBookGlobals();
            this.m_SheetIndex = 0;
        }

        private void initializeSheetRead()
        {
            if (this.m_SheetIndex != this.ResultsCount)
            {
                XlsBiffIndex index;
                this.m_dbCellAddrs = null;
                this.m_IsFirstRead = false;
                if (this.m_SheetIndex == -1)
                {
                    this.m_SheetIndex = 0;
                }
                if (!this.readWorkSheetGlobals(this.m_sheets[this.m_SheetIndex], out index, out this.m_currentRowRecord))
                {
                    this.m_SheetIndex++;
                    this.initializeSheetRead();
                }
                else if (index == null)
                {
                    this.m_noIndex = true;
                }
                else
                {
                    this.m_dbCellAddrs = index.DbCellAddresses;
                    this.m_dbCellAddrsIndex = 0;
                    this.m_cellOffset = this.findFirstDataCellOffset((int) this.m_dbCellAddrs[this.m_dbCellAddrsIndex]);
                    if (this.m_cellOffset < 0)
                    {
                        this.fail("Badly formed binary file. Has INDEX but no DBCELL");
                    }
                }
            }
        }

        public bool IsDBNull(int i) => 
            (this.m_cellsValues[i] == null) || ReferenceEquals(DBNull.Value, this.m_cellsValues[i]);

        public bool isV8() => 
            this.m_version >= 0x600;

        private bool moveToNextRecord()
        {
            if (this.m_noIndex)
            {
                LogManager.Log<ExcelBinaryReader>(this).Debug("No index", new object[0]);
                return this.moveToNextRecordNoIndex();
            }
            if ((this.m_dbCellAddrs == null) || ((this.m_dbCellAddrsIndex == this.m_dbCellAddrs.Length) || (this.m_depth == this.m_maxRow)))
            {
                return false;
            }
            this.m_canRead = this.readWorkSheetRow();
            if (!this.m_canRead && (this.m_depth > 0))
            {
                this.m_canRead = true;
            }
            if (!this.m_canRead && (this.m_dbCellAddrsIndex < (this.m_dbCellAddrs.Length - 1)))
            {
                this.m_dbCellAddrsIndex++;
                this.m_cellOffset = this.findFirstDataCellOffset((int) this.m_dbCellAddrs[this.m_dbCellAddrsIndex]);
                if (this.m_cellOffset < 0)
                {
                    return false;
                }
                this.m_canRead = this.readWorkSheetRow();
            }
            return this.m_canRead;
        }

        private bool moveToNextRecordNoIndex()
        {
            XlsBiffRow currentRowRecord = this.m_currentRowRecord;
            if (currentRowRecord != null)
            {
                if (currentRowRecord.RowIndex < this.m_depth)
                {
                    this.m_stream.Seek(currentRowRecord.Offset + currentRowRecord.Size, SeekOrigin.Begin);
                    do
                    {
                        if (this.m_stream.Position >= this.m_stream.Size)
                        {
                            return false;
                        }
                        XlsBiffRecord record = this.m_stream.Read();
                        if (record is XlsBiffEOF)
                        {
                            return false;
                        }
                        currentRowRecord = record as XlsBiffRow;
                    }
                    while ((currentRowRecord == null) || (currentRowRecord.RowIndex < this.m_depth));
                }
                this.m_currentRowRecord = currentRowRecord;
                XlsBiffBlankCell cell = null;
                while (this.m_stream.Position < this.m_stream.Size)
                {
                    XlsBiffRecord record2 = this.m_stream.Read();
                    if (record2 is XlsBiffEOF)
                    {
                        return false;
                    }
                    if (record2.IsCell)
                    {
                        XlsBiffBlankCell cell2 = record2 as XlsBiffBlankCell;
                        if ((cell2 != null) && (cell2.RowIndex == this.m_currentRowRecord.RowIndex))
                        {
                            cell = cell2;
                        }
                    }
                    if (cell != null)
                    {
                        this.m_cellOffset = cell.Offset;
                        this.m_canRead = this.readWorkSheetRow();
                        return this.m_canRead;
                    }
                }
            }
            return false;
        }

        public bool NextResult()
        {
            if (this.m_SheetIndex >= (this.ResultsCount - 1))
            {
                return false;
            }
            this.m_SheetIndex++;
            this.m_IsFirstRead = true;
            return true;
        }

        private void pushCellValue(XlsBiffBlankCell cell)
        {
            double num;
            object[] formatting = new object[] { cell.ID };
            LogManager.Log<ExcelBinaryReader>(this).Debug("pushCellValue {0}", formatting);
            BIFFRECORDTYPE iD = cell.ID;
            if (iD > BIFFRECORDTYPE.RSTRING)
            {
                if (iD > BIFFRECORDTYPE.BOOLERR)
                {
                    if (iD == BIFFRECORDTYPE.RK)
                    {
                        num = ((XlsBiffRKCell) cell).Value;
                        this.m_cellsValues[cell.ColumnIndex] = !this.ConvertOaDate ? num : this.tryConvertOADateTime(num, cell.XFormat);
                        object[] objArray5 = new object[] { num };
                        LogManager.Log<ExcelBinaryReader>(this).Debug("VALUE: {0}", objArray5);
                        return;
                    }
                    if (iD != BIFFRECORDTYPE.FORMULA)
                    {
                        return;
                    }
                }
                else
                {
                    if (iD == BIFFRECORDTYPE.LABELSST)
                    {
                        string str = this.m_globals.SST.GetString(((XlsBiffLabelSSTCell) cell).SSTIndex);
                        LogManager.Log<ExcelBinaryReader>(this).Debug("VALUE: {0}", new object[] { str });
                        this.m_cellsValues[cell.ColumnIndex] = str;
                        return;
                    }
                    switch (iD)
                    {
                        case BIFFRECORDTYPE.BLANK:
                            break;

                        case BIFFRECORDTYPE.INTEGER:
                            goto TR_000A;

                        case BIFFRECORDTYPE.NUMBER:
                            goto TR_000B;

                        case BIFFRECORDTYPE.LABEL:
                            goto TR_0001;

                        case BIFFRECORDTYPE.BOOLERR:
                            if (cell.ReadByte(7) != 0)
                            {
                                break;
                            }
                            this.m_cellsValues[cell.ColumnIndex] = cell.ReadByte(6) != 0;
                            return;

                        default:
                            return;
                    }
                    return;
                }
            }
            else
            {
                switch (iD)
                {
                    case BIFFRECORDTYPE.BLANK_OLD:
                        return;

                    case BIFFRECORDTYPE.INTEGER_OLD:
                        goto TR_000A;

                    case BIFFRECORDTYPE.NUMBER_OLD:
                        goto TR_000B;

                    case BIFFRECORDTYPE.LABEL_OLD:
                        goto TR_0001;

                    case BIFFRECORDTYPE.BOOLERR_OLD:
                        if (cell.ReadByte(8) == 0)
                        {
                            this.m_cellsValues[cell.ColumnIndex] = cell.ReadByte(7) != 0;
                            return;
                        }
                        return;

                    case BIFFRECORDTYPE.FORMULA_OLD:
                        break;

                    default:
                        switch (iD)
                        {
                            case BIFFRECORDTYPE.MULRK:
                            {
                                XlsBiffMulRKCell cell2 = (XlsBiffMulRKCell) cell;
                                for (ushort i = cell.ColumnIndex; i <= cell2.LastColumnIndex; i = (ushort) (i + 1))
                                {
                                    num = cell2.GetValue(i);
                                    object[] objArray6 = new object[] { num, i };
                                    LogManager.Log<ExcelBinaryReader>(this).Debug("VALUE[{1}]: {0}", objArray6);
                                    this.m_cellsValues[i] = !this.ConvertOaDate ? num : this.tryConvertOADateTime(num, cell2.GetXF(i));
                                }
                                return;
                            }
                            case BIFFRECORDTYPE.MULBLANK:
                                break;

                            default:
                                if (iD != BIFFRECORDTYPE.RSTRING)
                                {
                                    return;
                                }
                                goto TR_0001;
                        }
                        return;
                }
            }
            object obj2 = ((XlsBiffFormulaCell) cell).Value;
            if ((obj2 != null) && (obj2 is FORMULAERROR))
            {
                obj2 = null;
                return;
            }
            this.m_cellsValues[cell.ColumnIndex] = !this.ConvertOaDate ? obj2 : this.tryConvertOADateTime(obj2, cell.XFormat);
            return;
        TR_0001:
            this.m_cellsValues[cell.ColumnIndex] = ((XlsBiffLabelCell) cell).Value;
            object[] objArray3 = new object[] { this.m_cellsValues[cell.ColumnIndex] };
            LogManager.Log<ExcelBinaryReader>(this).Debug("VALUE: {0}", objArray3);
            return;
        TR_000A:
            this.m_cellsValues[cell.ColumnIndex] = ((XlsBiffIntegerCell) cell).Value;
            return;
        TR_000B:
            num = ((XlsBiffNumberCell) cell).Value;
            this.m_cellsValues[cell.ColumnIndex] = !this.ConvertOaDate ? num : this.tryConvertOADateTime(num, cell.XFormat);
            object[] objArray2 = new object[] { num };
            LogManager.Log<ExcelBinaryReader>(this).Debug("VALUE: {0}", objArray2);
        }

        public bool Read()
        {
            if (!this.m_isValid)
            {
                return false;
            }
            if (this.m_IsFirstRead)
            {
                this.initializeSheetRead();
            }
            return this.moveToNextRecord();
        }

        private DataTable readWholeWorkSheet(XlsWorksheet sheet)
        {
            XlsBiffIndex index;
            if (!this.readWorkSheetGlobals(sheet, out index, out this.m_currentRowRecord))
            {
                return null;
            }
            DataTable table = new DataTable(sheet.Name);
            bool triggerCreateColumns = true;
            if (index != null)
            {
                this.readWholeWorkSheetWithIndex(index, triggerCreateColumns, table);
            }
            else
            {
                this.readWholeWorkSheetNoIndex(triggerCreateColumns, table);
            }
            table.EndLoadData();
            return table;
        }

        private void readWholeWorkSheetNoIndex(bool triggerCreateColumns, DataTable table)
        {
            while (this.Read() && (this.m_depth != this.m_maxRow))
            {
                bool flag = false;
                if (triggerCreateColumns)
                {
                    if (!this._isFirstRowAsColumnNames && (!this._isFirstRowAsColumnNames || (this.m_maxRow != 1)))
                    {
                        for (int i = 0; i < this.m_maxCol; i++)
                        {
                            table.Columns.Add(null, typeof(object));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.m_maxCol; i++)
                        {
                            if ((this.m_cellsValues[i] != null) && (this.m_cellsValues[i].ToString().Length > 0))
                            {
                                Helpers.AddColumnHandleDuplicate(table, this.m_cellsValues[i].ToString());
                            }
                            else
                            {
                                Helpers.AddColumnHandleDuplicate(table, "Column" + i);
                            }
                        }
                    }
                    triggerCreateColumns = false;
                    flag = true;
                    table.BeginLoadData();
                }
                if (((!flag || !this._isFirstRowAsColumnNames) && (this.m_depth > 0)) && (!this._isFirstRowAsColumnNames || (this.m_maxRow != 1)))
                {
                    table.Rows.Add(this.m_cellsValues);
                }
            }
            if ((this.m_depth > 0) && (!this._isFirstRowAsColumnNames || (this.m_maxRow != 1)))
            {
                table.Rows.Add(this.m_cellsValues);
            }
        }

        private void readWholeWorkSheetWithIndex(XlsBiffIndex idx, bool triggerCreateColumns, DataTable table)
        {
            this.m_dbCellAddrs = idx.DbCellAddresses;
            int index = 0;
            while (index < this.m_dbCellAddrs.Length)
            {
                if (this.m_depth == this.m_maxRow)
                {
                    return;
                }
                this.m_cellOffset = this.findFirstDataCellOffset((int) this.m_dbCellAddrs[index]);
                if (this.m_cellOffset < 0)
                {
                    return;
                }
                if (triggerCreateColumns)
                {
                    if ((!this._isFirstRowAsColumnNames || !this.readWorkSheetRow()) && (!this._isFirstRowAsColumnNames || (this.m_maxRow != 1)))
                    {
                        for (int i = 0; i < this.m_maxCol; i++)
                        {
                            table.Columns.Add(null, typeof(object));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.m_maxCol; i++)
                        {
                            if ((this.m_cellsValues[i] != null) && (this.m_cellsValues[i].ToString().Length > 0))
                            {
                                Helpers.AddColumnHandleDuplicate(table, this.m_cellsValues[i].ToString());
                            }
                            else
                            {
                                Helpers.AddColumnHandleDuplicate(table, "Column" + i);
                            }
                        }
                    }
                    triggerCreateColumns = false;
                    table.BeginLoadData();
                }
                while (true)
                {
                    if (!this.readWorkSheetRow())
                    {
                        if ((this.m_depth > 0) && (!this._isFirstRowAsColumnNames || (this.m_maxRow != 1)))
                        {
                            table.Rows.Add(this.m_cellsValues);
                        }
                        index++;
                        break;
                    }
                    table.Rows.Add(this.m_cellsValues);
                }
            }
        }

        private void readWorkBookGlobals()
        {
            XlsBiffRecord record;
            bool flag;
            try
            {
                this.m_hdr = XlsHeader.ReadHeader(this.m_file);
            }
            catch (HeaderException exception)
            {
                this.fail(exception.Message);
                return;
            }
            catch (FormatException exception2)
            {
                this.fail(exception2.Message);
                return;
            }
            XlsRootDirectory rootDir = new XlsRootDirectory(this.m_hdr);
            XlsDirectoryEntry entry = rootDir.FindEntry("Workbook") ?? rootDir.FindEntry("Book");
            if (entry != null)
            {
                if (entry.EntryType != STGTY.STGTY_STREAM)
                {
                    this.fail("Error: Workbook directory entry is not a Stream.");
                    return;
                }
                this.m_stream = new XlsBiffStream(this.m_hdr, entry.StreamFirstSector, entry.IsEntryMiniStream, rootDir, this);
                this.m_globals = new XlsWorkbookGlobals();
                this.m_stream.Seek(0, SeekOrigin.Begin);
                XlsBiffBOF fbof = this.m_stream.Read() as XlsBiffBOF;
                if ((fbof == null) || (fbof.Type != BIFFTYPE.WorkbookGlobals))
                {
                    this.fail("Error reading Workbook Globals - Stream has invalid data.");
                    return;
                }
                flag = false;
                this.m_version = fbof.Version;
                this.m_sheets = new List<XlsWorksheet>();
                goto TR_0033;
            }
            else
            {
                this.fail("Error: Neither stream 'Workbook' nor 'Book' was found in file.");
            }
            return;
        TR_0033:
            while (true)
            {
                record = this.m_stream.Read();
                if (record == null)
                {
                    return;
                }
                BIFFRECORDTYPE iD = record.ID;
                if (iD > BIFFRECORDTYPE.COUNTRY)
                {
                    if (iD > BIFFRECORDTYPE.EXTSST)
                    {
                        if (iD <= BIFFRECORDTYPE.FONT_V34)
                        {
                            if ((iD != BIFFRECORDTYPE.PROT4REVPASSWORD) && (iD == BIFFRECORDTYPE.FONT_V34))
                            {
                                break;
                            }
                            continue;
                        }
                        if (iD != BIFFRECORDTYPE.XF_V3)
                        {
                            if (iD == BIFFRECORDTYPE.FORMAT)
                            {
                                XlsBiffFormatString str2 = (XlsBiffFormatString) record;
                                this.m_globals.Formats.Add(str2.Index, str2);
                                continue;
                            }
                            if (iD != BIFFRECORDTYPE.XF_V4)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (iD > BIFFRECORDTYPE.INTERFACEHDR)
                        {
                            if (iD == BIFFRECORDTYPE.SST)
                            {
                                this.m_globals.SST = (XlsBiffSST) record;
                                flag = true;
                                continue;
                            }
                            if (iD != BIFFRECORDTYPE.EXTSST)
                            {
                                continue;
                            }
                            this.m_globals.ExtSST = record;
                            flag = false;
                            continue;
                        }
                        if (iD == BIFFRECORDTYPE.MMS)
                        {
                            this.m_globals.MMS = record;
                            continue;
                        }
                        switch (iD)
                        {
                            case BIFFRECORDTYPE.XF:
                                break;

                            case BIFFRECORDTYPE.INTERFACEHDR:
                            {
                                this.m_globals.InterfaceHdr = (XlsBiffInterfaceHdr) record;
                                continue;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (iD <= BIFFRECORDTYPE.FONT)
                    {
                        if (iD <= BIFFRECORDTYPE.PASSWORD)
                        {
                            if (iD == BIFFRECORDTYPE.EOF)
                            {
                                if (this.m_globals.SST != null)
                                {
                                    this.m_globals.SST.ReadStrings();
                                }
                                return;
                            }
                            continue;
                        }
                        if (iD != BIFFRECORDTYPE.FORMAT_V23)
                        {
                            if (iD == BIFFRECORDTYPE.FONT)
                            {
                                break;
                            }
                            continue;
                        }
                        XlsBiffFormatString str = (XlsBiffFormatString) record;
                        str.UseEncoding = this.m_encoding;
                        this.m_globals.Formats.Add((ushort) this.m_globals.Formats.Count, str);
                        continue;
                    }
                    if (iD > BIFFRECORDTYPE.XF_V2)
                    {
                        if (iD != BIFFRECORDTYPE.BOUNDSHEET)
                        {
                            if (iD != BIFFRECORDTYPE.COUNTRY)
                            {
                                continue;
                            }
                            this.m_globals.Country = record;
                            continue;
                        }
                        XlsBiffBoundSheet refSheet = (XlsBiffBoundSheet) record;
                        if (refSheet.Type != XlsBiffBoundSheet.SheetType.Worksheet)
                        {
                            continue;
                        }
                        refSheet.IsV8 = this.isV8();
                        refSheet.UseEncoding = this.m_encoding;
                        object[] formatting = new object[] { refSheet.IsV8 };
                        LogManager.Log<ExcelBinaryReader>(this).Debug("BOUNDSHEET IsV8={0}", formatting);
                        this.m_sheets.Add(new XlsWorksheet(this.m_globals.Sheets.Count, refSheet));
                        this.m_globals.Sheets.Add(refSheet);
                        continue;
                    }
                    if (iD == BIFFRECORDTYPE.CONTINUE)
                    {
                        if (!flag)
                        {
                            continue;
                        }
                        XlsBiffContinue fragment = (XlsBiffContinue) record;
                        this.m_globals.SST.Append(fragment);
                        continue;
                    }
                    switch (iD)
                    {
                        case BIFFRECORDTYPE.CODEPAGE:
                        {
                            this.m_globals.CodePage = (XlsBiffSimpleValueRecord) record;
                            try
                            {
                                this.m_encoding = Encoding.GetEncoding(this.m_globals.CodePage.Value);
                            }
                            catch (ArgumentException)
                            {
                            }
                            continue;
                        }
                        case BIFFRECORDTYPE.XF_V2:
                            break;

                        default:
                        {
                            continue;
                        }
                    }
                }
                this.m_globals.ExtendedFormats.Add(record);
            }
            this.m_globals.Fonts.Add(record);
            goto TR_0033;
        }

        private bool readWorkSheetGlobals(XlsWorksheet sheet, out XlsBiffIndex idx, out XlsBiffRow row)
        {
            idx = null;
            row = null;
            this.m_stream.Seek((int) sheet.DataOffset, SeekOrigin.Begin);
            XlsBiffBOF fbof = this.m_stream.Read() as XlsBiffBOF;
            if ((fbof == null) || (fbof.Type != BIFFTYPE.Worksheet))
            {
                return false;
            }
            XlsBiffRecord record = this.m_stream.Read();
            if (record == null)
            {
                return false;
            }
            if (record is XlsBiffIndex)
            {
                idx = record as XlsBiffIndex;
            }
            else if (record is XlsBiffUncalced)
            {
                idx = this.m_stream.Read() as XlsBiffIndex;
            }
            if (idx != null)
            {
                idx.IsV8 = this.isV8();
                object[] formatting = new object[] { idx.IsV8 };
                LogManager.Log<ExcelBinaryReader>(this).Debug("INDEX IsV8={0}", formatting);
            }
            XlsBiffDimensions dimensions = null;
            while (true)
            {
                XlsBiffRecord record2 = this.m_stream.Read();
                if (record2.ID == BIFFRECORDTYPE.DIMENSIONS)
                {
                    dimensions = (XlsBiffDimensions) record2;
                }
                else if ((record2 != null) && (record2.ID != BIFFRECORDTYPE.ROW))
                {
                    continue;
                }
                if (record2.ID == BIFFRECORDTYPE.ROW)
                {
                    row = (XlsBiffRow) record2;
                }
                XlsBiffRow row2 = null;
                while (true)
                {
                    if ((row2 == null) && (this.m_stream.Position < this.m_stream.Size))
                    {
                        XlsBiffRecord record3 = this.m_stream.Read();
                        object[] formatting = new object[] { record3.Offset, record3.ID };
                        LogManager.Log<ExcelBinaryReader>(this).Debug("finding rowRecord offset {0}, rec: {1}", formatting);
                        if (!(record3 is XlsBiffEOF))
                        {
                            row2 = record3 as XlsBiffRow;
                            continue;
                        }
                    }
                    if (row2 != null)
                    {
                        object[] formatting = new object[] { row2.Offset, row2.ID, row2.RowIndex, row2.FirstDefinedColumn, row2.LastDefinedColumn };
                        LogManager.Log<ExcelBinaryReader>(this).Debug("Got row {0}, rec: id={1},rowindex={2}, rowColumnStart={3}, rowColumnEnd={4}", formatting);
                    }
                    row = row2;
                    if (dimensions == null)
                    {
                        this.m_maxCol = 0x100;
                        this.m_maxRow = (int) idx.LastExistingRow;
                    }
                    else
                    {
                        dimensions.IsV8 = this.isV8();
                        object[] formatting = new object[] { dimensions.IsV8 };
                        LogManager.Log<ExcelBinaryReader>(this).Debug("dims IsV8={0}", formatting);
                        this.m_maxCol = dimensions.LastColumn - 1;
                        if ((this.m_maxCol <= 0) && (row2 != null))
                        {
                            this.m_maxCol = row2.LastDefinedColumn;
                        }
                        this.m_maxRow = (int) dimensions.LastRow;
                        sheet.Dimensions = dimensions;
                    }
                    if ((idx != null) && (idx.LastExistingRow <= idx.FirstExistingRow))
                    {
                        return false;
                    }
                    if (row == null)
                    {
                        return false;
                    }
                    this.m_depth = 0;
                    return true;
                }
            }
        }

        private bool readWorkSheetRow()
        {
            this.m_cellsValues = new object[this.m_maxCol];
            while (true)
            {
                if (this.m_cellOffset < this.m_stream.Size)
                {
                    XlsBiffRecord record = this.m_stream.ReadAt(this.m_cellOffset);
                    this.m_cellOffset += record.Size;
                    if (!(record is XlsBiffDbCell))
                    {
                        if (record is XlsBiffEOF)
                        {
                            return false;
                        }
                        XlsBiffBlankCell cell = record as XlsBiffBlankCell;
                        if ((cell == null) || (cell.ColumnIndex >= this.m_maxCol))
                        {
                            continue;
                        }
                        if (cell.RowIndex == this.m_depth)
                        {
                            this.pushCellValue(cell);
                            continue;
                        }
                        this.m_cellOffset -= record.Size;
                    }
                }
                this.m_depth++;
                return (this.m_depth < this.m_maxRow);
            }
        }

        private object tryConvertOADateTime(double value, ushort XFormat)
        {
            XlsBiffFormatString str;
            ushort key = 0;
            if ((XFormat < 0) || (XFormat >= this.m_globals.ExtendedFormats.Count))
            {
                key = XFormat;
            }
            else
            {
                XlsBiffRecord record = this.m_globals.ExtendedFormats[XFormat];
                BIFFRECORDTYPE iD = record.ID;
                if (iD == BIFFRECORDTYPE.XF_V2)
                {
                    key = (ushort) (record.ReadByte(2) & 0x3f);
                }
                else if (iD == BIFFRECORDTYPE.XF_V3)
                {
                    if ((record.ReadByte(3) & 4) == 0)
                    {
                        return value;
                    }
                    key = record.ReadByte(1);
                }
                else if (iD != BIFFRECORDTYPE.XF_V4)
                {
                    if ((record.ReadByte(this.m_globals.Sheets[this.m_globals.Sheets.Count - 1].IsV8 ? 9 : 7) & 4) == 0)
                    {
                        return value;
                    }
                    key = record.ReadUInt16(2);
                }
                else
                {
                    if ((record.ReadByte(5) & 4) == 0)
                    {
                        return value;
                    }
                    key = record.ReadByte(1);
                }
            }
            switch (key)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 0x25:
                case 0x26:
                case 0x27:
                case 40:
                case 0x29:
                case 0x2a:
                case 0x2b:
                case 0x2c:
                case 0x30:
                    return value;

                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 20:
                case 0x15:
                case 0x16:
                case 0x2d:
                case 0x2e:
                case 0x2f:
                    return Helpers.ConvertFromOATime(value);

                case 0x31:
                    return value.ToString();
            }
            if (this.m_globals.Formats.TryGetValue(key, out str))
            {
                FormatReader reader2 = new FormatReader {
                    FormatString = str.Value
                };
                if (reader2.IsDateFormatString())
                {
                    return Helpers.ConvertFromOATime(value);
                }
            }
            return value;
        }

        private object tryConvertOADateTime(object value, ushort XFormat)
        {
            double num;
            return (!double.TryParse(value.ToString(), out num) ? value : this.tryConvertOADateTime(num, XFormat));
        }

        public string ExceptionMessage =>
            this.m_exceptionMessage;

        public string Name =>
            ((this.m_sheets == null) || (this.m_sheets.Count <= 0)) ? null : this.m_sheets[this.m_SheetIndex].Name;

        public string VisibleState =>
            ((this.m_sheets == null) || (this.m_sheets.Count <= 0)) ? null : this.m_sheets[this.m_SheetIndex].VisibleState;

        public bool IsValid =>
            this.m_isValid;

        public int Depth =>
            this.m_depth;

        public int ResultsCount =>
            this.m_globals.Sheets.Count;

        public bool IsClosed =>
            this.m_isClosed;

        public int FieldCount =>
            this.m_maxCol;

        public object this[int i] =>
            this.m_cellsValues[i];

        public int RecordsAffected
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public object this[string name]
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public bool IsFirstRowAsColumnNames
        {
            get => 
                this._isFirstRowAsColumnNames;
            set => 
                this._isFirstRowAsColumnNames = value;
        }

        public bool ConvertOaDate
        {
            get => 
                this.m_ConvertOADate;
            set => 
                this.m_ConvertOADate = value;
        }

        public Excel.ReadOption ReadOption =>
            this.m_ReadOption;
    }
}

