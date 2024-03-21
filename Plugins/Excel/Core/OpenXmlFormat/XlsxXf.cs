namespace Excel.Core.OpenXmlFormat
{
    using System;

    internal class XlsxXf
    {
        public const string N_xf = "xf";
        public const string A_numFmtId = "numFmtId";
        public const string A_xfId = "xfId";
        public const string A_applyNumberFormat = "applyNumberFormat";
        private int _Id;
        private int _numFmtId;
        private bool _applyNumberFormat;

        public XlsxXf(int id, int numFmtId, string applyNumberFormat)
        {
            this._Id = id;
            this._numFmtId = numFmtId;
            this._applyNumberFormat = (applyNumberFormat != null) && (applyNumberFormat == "1");
        }

        public int Id
        {
            get => 
                this._Id;
            set => 
                this._Id = value;
        }

        public int NumFmtId
        {
            get => 
                this._numFmtId;
            set => 
                this._numFmtId = value;
        }

        public bool ApplyNumberFormat
        {
            get => 
                this._applyNumberFormat;
            set => 
                this._applyNumberFormat = value;
        }
    }
}

