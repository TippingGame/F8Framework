namespace Excel
{
    using Excel.Core;
    using Excel.Core.OpenXmlFormat;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    public class ExcelOpenXmlReader : IExcelDataReader, IDataReader, IDisposable, IDataRecord
    {
        private const string COLUMN = "Column";
        private XlsxWorkbook _workbook;
        private bool _isValid = true;
        private bool _isClosed;
        private bool _isFirstRead = true;
        private string _exceptionMessage;
        private int _depth;
        private int _resultIndex;
        private int _emptyRowCount;
        private ZipWorker _zipWorker;
        private XmlReader _xmlReader;
        private Stream _sheetStream;
        private object[] _cellsValues;
        private object[] _savedCellsValues;
        private bool disposed;
        private bool _isFirstRowAsColumnNames;
        private string instanceId = Guid.NewGuid().ToString();
        private List<int> _defaultDateTimeStyles;
        private string _namespaceUri;

        internal ExcelOpenXmlReader()
        {
            int[] collection = new int[] { 14, 15, 0x10, 0x11, 0x12, 0x13, 20, 0x15, 0x16, 0x2d, 0x2e, 0x2f };
            this._defaultDateTimeStyles = new List<int>(collection);
        }

        public DataSet AsDataSet() => 
            this.AsDataSet(true);

        public DataSet AsDataSet(bool convertOADateTime)
        {
            if (!this._isValid)
            {
                return null;
            }
            DataSet dataset = new DataSet();
            int num = 0;
            while (true)
            {
                while (true)
                {
                    if (num >= this._workbook.Sheets.Count)
                    {
                        dataset.AcceptChanges();
                        Helpers.FixDataTypes(dataset);
                        return dataset;
                    }
                    DataTable table = new DataTable(this._workbook.Sheets[num].Name);
                    table.ExtendedProperties.Add("visiblestate", this._workbook.Sheets[num].VisibleState);
                    this.ReadSheetGlobals(this._workbook.Sheets[num]);
                    if (this._workbook.Sheets[num].Dimension != null)
                    {
                        this._depth = 0;
                        this._emptyRowCount = 0;
                        if (!this._isFirstRowAsColumnNames)
                        {
                            for (int i = 0; i < this._workbook.Sheets[num].ColumnsCount; i++)
                            {
                                table.Columns.Add(null, typeof(object));
                            }
                        }
                        else
                        {
                            if (!this.ReadSheetRow(this._workbook.Sheets[num]))
                            {
                                break;
                            }
                            for (int i = 0; i < this._cellsValues.Length; i++)
                            {
                                if ((this._cellsValues[i] != null) && (this._cellsValues[i].ToString().Length > 0))
                                {
                                    Helpers.AddColumnHandleDuplicate(table, this._cellsValues[i].ToString());
                                }
                                else
                                {
                                    Helpers.AddColumnHandleDuplicate(table, "Column" + i);
                                }
                            }
                        }
                        table.BeginLoadData();
                        while (true)
                        {
                            if (!this.ReadSheetRow(this._workbook.Sheets[num]))
                            {
                                if (table.Rows.Count > 0)
                                {
                                    dataset.Tables.Add(table);
                                }
                                table.EndLoadData();
                                break;
                            }
                            table.Rows.Add(this._cellsValues);
                        }
                    }
                    break;
                }
                num++;
            }
        }

        private void CheckDateTimeNumFmts(List<XlsxNumFmt> list)
        {
            if (list.Count != 0)
            {
                foreach (XlsxNumFmt fmt in list)
                {
                    if (!string.IsNullOrEmpty(fmt.FormatCode))
                    {
                        string str = fmt.FormatCode.ToLower();
                        while (true)
                        {
                            int index = str.IndexOf('"');
                            if (index <= 0)
                            {
                                FormatReader reader2 = new FormatReader {
                                    FormatString = str
                                };
                                if (reader2.IsDateFormatString())
                                {
                                    this._defaultDateTimeStyles.Add(fmt.Id);
                                }
                                break;
                            }
                            int num2 = str.IndexOf('"', index + 1);
                            if (num2 > 0)
                            {
                                str = str.Remove(index, (num2 - index) + 1);
                            }
                        }
                    }
                }
            }
        }

        public void Close()
        {
            this._isClosed = true;
            if (this._xmlReader != null)
            {
                this._xmlReader.Close();
            }
            if (this._sheetStream != null)
            {
                this._sheetStream.Close();
            }
            if (this._zipWorker != null)
            {
                this._zipWorker.Dispose();
            }
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
                    if (this._xmlReader != null)
                    {
                        this._xmlReader.Dispose();
                    }
                    if (this._sheetStream != null)
                    {
                        this._sheetStream.Dispose();
                    }
                    if (this._zipWorker != null)
                    {
                        this._zipWorker.Dispose();
                    }
                }
                this._zipWorker = null;
                this._xmlReader = null;
                this._sheetStream = null;
                this._workbook = null;
                this._cellsValues = null;
                this._savedCellsValues = null;
                this.disposed = true;
            }
        }

        ~ExcelOpenXmlReader()
        {
            this.Dispose(false);
        }

        public bool GetBoolean(int i) => 
            !this.IsDBNull(i) ? bool.Parse(this._cellsValues[i].ToString()) : false;

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
            if (this.IsDBNull(i))
            {
                return DateTime.MinValue;
            }
            try
            {
                return (DateTime) this._cellsValues[i];
            }
            catch (InvalidCastException)
            {
                return DateTime.MinValue;
            }
        }

        public decimal GetDecimal(int i) => 
            !this.IsDBNull(i) ? decimal.Parse(this._cellsValues[i].ToString()) : decimal.MinValue;

        public double GetDouble(int i) => 
            !this.IsDBNull(i) ? double.Parse(this._cellsValues[i].ToString()) : double.MinValue;

        public Type GetFieldType(int i)
        {
            throw new NotSupportedException();
        }

        public float GetFloat(int i) => 
            !this.IsDBNull(i) ? float.Parse(this._cellsValues[i].ToString()) : float.MinValue;

        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(int i) => 
            (short)(!this.IsDBNull(i) ? short.Parse(this._cellsValues[i].ToString()) : -32768);

        public int GetInt32(int i) => 
            !this.IsDBNull(i) ? int.Parse(this._cellsValues[i].ToString()) : -2147483648;

        public long GetInt64(int i) => 
            !this.IsDBNull(i) ? long.Parse(this._cellsValues[i].ToString()) : -9223372036854775808L;

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
            !this.IsDBNull(i) ? this._cellsValues[i].ToString() : null;

        public object GetValue(int i) => 
            this._cellsValues[i];

        public int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public void Initialize(Stream fileStream)
        {
            this._zipWorker = new ZipWorker();
            this._zipWorker.Extract(fileStream);
            if (this._zipWorker.IsValid)
            {
                this.ReadGlobals();
            }
            else
            {
                this._isValid = false;
                this._exceptionMessage = this._zipWorker.ExceptionMessage;
                this.Close();
            }
        }

        private bool InitializeSheetRead()
        {
            if (this.ResultsCount <= 0)
            {
                return false;
            }
            this.ReadSheetGlobals(this._workbook.Sheets[this._resultIndex]);
            if (this._workbook.Sheets[this._resultIndex].Dimension == null)
            {
                return false;
            }
            this._isFirstRead = false;
            this._depth = 0;
            this._emptyRowCount = 0;
            return true;
        }

        private bool IsDateTimeStyle(int styleId) => 
            this._defaultDateTimeStyles.Contains(styleId);

        public bool IsDBNull(int i) => 
            (this._cellsValues[i] == null) || ReferenceEquals(DBNull.Value, this._cellsValues[i]);

        public bool NextResult()
        {
            if (this._resultIndex >= (this.ResultsCount - 1))
            {
                return false;
            }
            this._resultIndex++;
            this._isFirstRead = true;
            this._savedCellsValues = null;
            return true;
        }

        public bool Read() => 
            this._isValid ? ((!this._isFirstRead || this.InitializeSheetRead()) ? this.ReadSheetRow(this._workbook.Sheets[this._resultIndex]) : false) : false;

        private void ReadGlobals()
        {
            this._workbook = new XlsxWorkbook(this._zipWorker.GetWorkbookStream(), this._zipWorker.GetWorkbookRelsStream(), this._zipWorker.GetSharedStringsStream(), this._zipWorker.GetStylesStream());
            this.CheckDateTimeNumFmts(this._workbook.Styles.NumFmts);
        }

        private void ReadSheetGlobals(XlsxWorksheet sheet)
        {
            if (this._xmlReader != null)
            {
                this._xmlReader.Close();
            }
            if (this._sheetStream != null)
            {
                this._sheetStream.Close();
            }
            this._sheetStream = this._zipWorker.GetWorksheetStream(sheet.Path);
            if (this._sheetStream != null)
            {
                this._xmlReader = XmlReader.Create(this._sheetStream);
                int rows = 0;
                int cols = 0;
                this._namespaceUri = null;
                int num3 = 0;
                while (this._xmlReader.Read())
                {
                    if ((this._xmlReader.NodeType == XmlNodeType.Element) && (this._xmlReader.LocalName == "worksheet"))
                    {
                        this._namespaceUri = this._xmlReader.NamespaceURI;
                    }
                    if ((this._xmlReader.NodeType == XmlNodeType.Element) && (this._xmlReader.LocalName == "dimension"))
                    {
                        string attribute = this._xmlReader.GetAttribute("ref");
                        sheet.Dimension = new XlsxDimension(attribute);
                        break;
                    }
                    if ((this._xmlReader.NodeType == XmlNodeType.Element) && (this._xmlReader.LocalName == "row"))
                    {
                        rows++;
                    }
                    if ((sheet.Dimension == null) && ((cols == 0) && ((this._xmlReader.NodeType == XmlNodeType.Element) && (this._xmlReader.LocalName == "c"))))
                    {
                        string attribute = this._xmlReader.GetAttribute("r");
                        if (attribute != null)
                        {
                            int[] numArray = ReferenceHelper.ReferenceToColumnAndRow(attribute);
                            if (numArray[1] > num3)
                            {
                                num3 = numArray[1];
                            }
                        }
                    }
                }
                if (sheet.Dimension == null)
                {
                    cols = num3;
                    if ((rows == 0) || (cols == 0))
                    {
                        sheet.IsEmpty = true;
                        return;
                    }
                    sheet.Dimension = new XlsxDimension(rows, cols);
                    this._xmlReader.Close();
                    this._sheetStream.Close();
                    this._sheetStream = this._zipWorker.GetWorksheetStream(sheet.Path);
                    this._xmlReader = XmlReader.Create(this._sheetStream);
                }
                this._xmlReader.ReadToFollowing("sheetData", this._namespaceUri);
                if (this._xmlReader.IsEmptyElement)
                {
                    sheet.IsEmpty = true;
                }
            }
        }

        private bool ReadSheetRow(XlsxWorksheet sheet)
        {
            if (this._xmlReader == null)
            {
                return false;
            }
            if (this._emptyRowCount != 0)
            {
                this._cellsValues = new object[sheet.ColumnsCount];
                this._emptyRowCount--;
                this._depth++;
                return true;
            }
            if (this._savedCellsValues != null)
            {
                this._cellsValues = this._savedCellsValues;
                this._savedCellsValues = null;
                this._depth++;
                return true;
            }
            if (((this._xmlReader.NodeType != XmlNodeType.Element) || (this._xmlReader.LocalName != "row")) && !this._xmlReader.ReadToFollowing("row", this._namespaceUri))
            {
                this._xmlReader.Close();
                if (this._sheetStream != null)
                {
                    this._sheetStream.Close();
                }
                return false;
            }
            this._cellsValues = new object[sheet.ColumnsCount];
            int num = int.Parse(this._xmlReader.GetAttribute("r"));
            if ((num != (this._depth + 1)) && (num != (this._depth + 1)))
            {
                this._emptyRowCount = (num - this._depth) - 1;
            }
            bool flag = false;
            string s = string.Empty;
            string attribute = string.Empty;
            int num2 = 0;
            int num3 = 0;
            while (this._xmlReader.Read() && (this._xmlReader.Depth != 2))
            {
                if (this._xmlReader.NodeType == XmlNodeType.Element)
                {
                    flag = false;
                    if (this._xmlReader.LocalName == "c")
                    {
                        s = this._xmlReader.GetAttribute("s");
                        attribute = this._xmlReader.GetAttribute("t");
                        XlsxDimension.XlsxDim(this._xmlReader.GetAttribute("r"), out num2, out num3);
                    }
                    else if ((this._xmlReader.LocalName == "v") || (this._xmlReader.LocalName == "t"))
                    {
                        flag = true;
                    }
                }
                if ((this._xmlReader.NodeType == XmlNodeType.Text) && flag)
                {
                    double num4;
                    object obj2 = null;
                    NumberStyles any = NumberStyles.Any;
                    if (double.TryParse(this._xmlReader.Value.ToString(), any, CultureInfo.InvariantCulture, out num4))
                    {
                        obj2 = num4;
                    }
                    if ((attribute != null) && (attribute == "s"))
                    {
                        obj2 = Helpers.ConvertEscapeChars(this._workbook.SST[int.Parse(obj2.ToString())]);
                    }
                    else if ((attribute != null) && (attribute == "inlineStr"))
                    {
                        obj2 = Helpers.ConvertEscapeChars(obj2.ToString());
                    }
                    else if (attribute == "b")
                    {
                        obj2 = this._xmlReader.Value == "1";
                    }
                    else if (s != null)
                    {
                        XlsxXf xf = this._workbook.Styles.CellXfs[int.Parse(s)];
                        if ((obj2 != null) && ((obj2.ToString() != string.Empty) && this.IsDateTimeStyle(xf.NumFmtId)))
                        {
                            obj2 = Helpers.ConvertFromOATime(num4);
                        }
                        else if (xf.NumFmtId == 0x31)
                        {
                            obj2 = obj2.ToString();
                        }
                    }
                    if ((num2 - 1) < this._cellsValues.Length)
                    {
                        this._cellsValues[num2 - 1] = obj2;
                    }
                }
            }
            if (this._emptyRowCount > 0)
            {
                this._savedCellsValues = this._cellsValues;
                return this.ReadSheetRow(sheet);
            }
            this._depth++;
            return true;
        }

        public bool IsFirstRowAsColumnNames
        {
            get => 
                this._isFirstRowAsColumnNames;
            set => 
                this._isFirstRowAsColumnNames = value;
        }

        public bool IsValid =>
            this._isValid;

        public string ExceptionMessage =>
            this._exceptionMessage;

        public string Name =>
            ((this._resultIndex < 0) || (this._resultIndex >= this.ResultsCount)) ? null : this._workbook.Sheets[this._resultIndex].Name;

        public string VisibleState =>
            ((this._resultIndex < 0) || (this._resultIndex >= this.ResultsCount)) ? null : this._workbook.Sheets[this._resultIndex].VisibleState;

        public int Depth =>
            this._depth;

        public int ResultsCount =>
            (this._workbook == null) ? -1 : this._workbook.Sheets.Count;

        public bool IsClosed =>
            this._isClosed;

        public int FieldCount =>
            ((this._resultIndex < 0) || (this._resultIndex >= this.ResultsCount)) ? -1 : this._workbook.Sheets[this._resultIndex].ColumnsCount;

        public object this[int i] =>
            this._cellsValues[i];

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
    }
}

