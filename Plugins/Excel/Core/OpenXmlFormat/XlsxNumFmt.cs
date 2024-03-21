namespace Excel.Core.OpenXmlFormat
{
    using System;

    internal class XlsxNumFmt
    {
        public const string N_numFmt = "numFmt";
        public const string A_numFmtId = "numFmtId";
        public const string A_formatCode = "formatCode";
        private int _Id;
        private string _FormatCode;

        public XlsxNumFmt(int id, string formatCode)
        {
            this._Id = id;
            this._FormatCode = formatCode;
        }

        public int Id
        {
            get => 
                this._Id;
            set => 
                this._Id = value;
        }

        public string FormatCode
        {
            get => 
                this._FormatCode;
            set => 
                this._FormatCode = value;
        }
    }
}

