namespace Excel.Core.OpenXmlFormat
{
    using System;
    using System.Runtime.CompilerServices;

    internal class XlsxWorksheet
    {
        public const string N_dimension = "dimension";
        public const string N_worksheet = "worksheet";
        public const string N_row = "row";
        public const string N_col = "col";
        public const string N_c = "c";
        public const string N_v = "v";
        public const string N_t = "t";
        public const string A_ref = "ref";
        public const string A_r = "r";
        public const string A_t = "t";
        public const string A_s = "s";
        public const string N_sheetData = "sheetData";
        public const string N_inlineStr = "inlineStr";
        private XlsxDimension _dimension;
        private string _Name;
        private string _visibleState;
        private int _id;
        private string _rid;
        private string _path;

        public XlsxWorksheet(string name, int id, string rid, string visibleState)
        {
            this._Name = name;
            this._id = id;
            this._rid = rid;
            this._visibleState = string.IsNullOrEmpty(visibleState) ? "visible" : visibleState.ToLower();
        }

        public bool IsEmpty { get; set; }

        public XlsxDimension Dimension
        {
            get => 
                this._dimension;
            set => 
                this._dimension = value;
        }

        public int ColumnsCount =>
            this.IsEmpty ? 0 : ((this._dimension == null) ? -1 : this._dimension.LastCol);

        public int RowsCount =>
            (this._dimension == null) ? -1 : ((this._dimension.LastRow - this._dimension.FirstRow) + 1);

        public string Name =>
            this._Name;

        public string VisibleState =>
            this._visibleState;

        public int Id =>
            this._id;

        public string RID
        {
            get => 
                this._rid;
            set => 
                this._rid = value;
        }

        public string Path
        {
            get => 
                this._path;
            set => 
                this._path = value;
        }
    }
}

