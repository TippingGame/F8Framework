namespace Excel.Core.OpenXmlFormat
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    internal class XlsxWorkbook
    {
        private const string N_sheet = "sheet";
        private const string N_t = "t";
        private const string N_si = "si";
        private const string N_cellXfs = "cellXfs";
        private const string N_numFmts = "numFmts";
        private const string A_sheetId = "sheetId";
        private const string A_visibleState = "state";
        private const string A_name = "name";
        private const string A_rid = "r:id";
        private const string N_rel = "Relationship";
        private const string A_id = "Id";
        private const string A_target = "Target";
        private List<XlsxWorksheet> sheets;
        private XlsxSST _SST;
        private XlsxStyles _Styles;

        private XlsxWorkbook()
        {
        }

        public XlsxWorkbook(Stream workbookStream, Stream relsStream, Stream sharedStringsStream, Stream stylesStream)
        {
            if (workbookStream == null)
            {
                throw new ArgumentNullException();
            }
            this.ReadWorkbook(workbookStream);
            this.ReadWorkbookRels(relsStream);
            this.ReadSharedStrings(sharedStringsStream);
            this.ReadStyles(stylesStream);
        }

        private void ReadSharedStrings(Stream xmlFileStream)
        {
            if (xmlFileStream != null)
            {
                this._SST = new XlsxSST();
                using (XmlReader reader = XmlReader.Create(xmlFileStream))
                {
                    bool flag = false;
                    string item = "";
                    while (true)
                    {
                        if (!reader.Read())
                        {
                            if (flag)
                            {
                                this._SST.Add(item);
                            }
                            xmlFileStream.Close();
                            break;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName == "si"))
                        {
                            if (flag)
                            {
                                this._SST.Add(item);
                            }
                            else
                            {
                                flag = true;
                            }
                            item = "";
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName == "t"))
                        {
                            item = item + reader.ReadElementContentAsString();
                        }
                    }
                }
            }
        }

        private void ReadStyles(Stream xmlFileStream)
        {
            if (xmlFileStream != null)
            {
                this._Styles = new XlsxStyles();
                bool flag = false;
                using (XmlReader reader = XmlReader.Create(xmlFileStream))
                {
                    while (true)
                    {
                        if (reader.Read())
                        {
                            if (!flag && ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName == "numFmts")))
                            {
                                while (true)
                                {
                                    if (!reader.Read() || ((reader.NodeType == XmlNodeType.Element) && (reader.Depth == 1)))
                                    {
                                        flag = true;
                                        break;
                                    }
                                    if ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName == "numFmt"))
                                    {
                                        this._Styles.NumFmts.Add(new XlsxNumFmt(int.Parse(reader.GetAttribute("numFmtId")), reader.GetAttribute("formatCode")));
                                    }
                                }
                            }
                            if ((reader.NodeType != XmlNodeType.Element) || (reader.LocalName != "cellXfs"))
                            {
                                continue;
                            }
                            while (reader.Read() && ((reader.NodeType != XmlNodeType.Element) || (reader.Depth != 1)))
                            {
                                if ((reader.NodeType != XmlNodeType.Element) || (reader.LocalName != "xf"))
                                {
                                    continue;
                                }
                                string attribute = reader.GetAttribute("xfId");
                                string s = reader.GetAttribute("numFmtId");
                                this._Styles.CellXfs.Add(new XlsxXf((attribute == null) ? -1 : int.Parse(attribute), (s == null) ? -1 : int.Parse(s), reader.GetAttribute("applyNumberFormat")));
                            }
                        }
                        xmlFileStream.Close();
                        break;
                    }
                }
            }
        }

        private void ReadWorkbook(Stream xmlFileStream)
        {
            this.sheets = new List<XlsxWorksheet>();
            using (XmlReader reader = XmlReader.Create(xmlFileStream))
            {
                while (true)
                {
                    if (!reader.Read())
                    {
                        xmlFileStream.Close();
                        break;
                    }
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName == "sheet"))
                    {
                        this.sheets.Add(new XlsxWorksheet(reader.GetAttribute("name"), int.Parse(reader.GetAttribute("sheetId")), reader.GetAttribute("r:id"), reader.GetAttribute("state")));
                    }
                }
            }
        }

        private void ReadWorkbookRels(Stream xmlFileStream)
        {
            using (XmlReader reader = XmlReader.Create(xmlFileStream))
            {
                while (true)
                {
                    if (!reader.Read())
                    {
                        xmlFileStream.Close();
                        break;
                    }
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName == "Relationship"))
                    {
                        string attribute = reader.GetAttribute("Id");
                        for (int i = 0; i < this.sheets.Count; i++)
                        {
                            XlsxWorksheet worksheet = this.sheets[i];
                            if (worksheet.RID == attribute)
                            {
                                worksheet.Path = reader.GetAttribute("Target");
                                this.sheets[i] = worksheet;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public List<XlsxWorksheet> Sheets
        {
            get => 
                this.sheets;
            set => 
                this.sheets = value;
        }

        public XlsxSST SST =>
            this._SST;

        public XlsxStyles Styles =>
            this._Styles;
    }
}

