namespace Excel.Core.BinaryFormat
{
    using System;
    using System.Collections.Generic;

    internal class XlsWorkbookGlobals
    {
        private readonly List<XlsBiffRecord> m_ExtendedFormats = new List<XlsBiffRecord>();
        private readonly List<XlsBiffRecord> m_Fonts = new List<XlsBiffRecord>();
        private readonly Dictionary<ushort, XlsBiffFormatString> m_Formats = new Dictionary<ushort, XlsBiffFormatString>();
        private readonly List<XlsBiffBoundSheet> m_Sheets = new List<XlsBiffBoundSheet>();
        private readonly List<XlsBiffRecord> m_Styles = new List<XlsBiffRecord>();
        private XlsBiffSimpleValueRecord m_Backup;
        private XlsBiffSimpleValueRecord m_CodePage;
        private XlsBiffRecord m_Country;
        private XlsBiffRecord m_DSF;
        private XlsBiffRecord m_ExtSST;
        private XlsBiffInterfaceHdr m_InterfaceHdr;
        private XlsBiffRecord m_MMS;
        private XlsBiffSST m_SST;
        private XlsBiffRecord m_WriteAccess;

        public XlsBiffInterfaceHdr InterfaceHdr
        {
            get => 
                this.m_InterfaceHdr;
            set => 
                this.m_InterfaceHdr = value;
        }

        public XlsBiffRecord MMS
        {
            get => 
                this.m_MMS;
            set => 
                this.m_MMS = value;
        }

        public XlsBiffRecord WriteAccess
        {
            get => 
                this.m_WriteAccess;
            set => 
                this.m_WriteAccess = value;
        }

        public XlsBiffSimpleValueRecord CodePage
        {
            get => 
                this.m_CodePage;
            set => 
                this.m_CodePage = value;
        }

        public XlsBiffRecord DSF
        {
            get => 
                this.m_DSF;
            set => 
                this.m_DSF = value;
        }

        public XlsBiffRecord Country
        {
            get => 
                this.m_Country;
            set => 
                this.m_Country = value;
        }

        public XlsBiffSimpleValueRecord Backup
        {
            get => 
                this.m_Backup;
            set => 
                this.m_Backup = value;
        }

        public List<XlsBiffRecord> Fonts =>
            this.m_Fonts;

        public Dictionary<ushort, XlsBiffFormatString> Formats =>
            this.m_Formats;

        public List<XlsBiffRecord> ExtendedFormats =>
            this.m_ExtendedFormats;

        public List<XlsBiffRecord> Styles =>
            this.m_Styles;

        public List<XlsBiffBoundSheet> Sheets =>
            this.m_Sheets;

        public XlsBiffSST SST
        {
            get => 
                this.m_SST;
            set => 
                this.m_SST = value;
        }

        public XlsBiffRecord ExtSST
        {
            get => 
                this.m_ExtSST;
            set => 
                this.m_ExtSST = value;
        }
    }
}

