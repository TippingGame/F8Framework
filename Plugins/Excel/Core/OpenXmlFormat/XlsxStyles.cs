namespace Excel.Core.OpenXmlFormat
{
    using System;
    using System.Collections.Generic;

    internal class XlsxStyles
    {
        private List<XlsxXf> _cellXfs = new List<XlsxXf>();
        private List<XlsxNumFmt> _NumFmts = new List<XlsxNumFmt>();

        public List<XlsxXf> CellXfs
        {
            get => 
                this._cellXfs;
            set => 
                this._cellXfs = value;
        }

        public List<XlsxNumFmt> NumFmts
        {
            get => 
                this._NumFmts;
            set => 
                this._NumFmts = value;
        }
    }
}

