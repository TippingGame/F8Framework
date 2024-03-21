namespace Excel.Core.BinaryFormat
{
    using System;

    internal class XlsWorksheet
    {
        private readonly uint m_dataOffset;
        private readonly int m_Index;
        private readonly string m_Name = string.Empty;
        private readonly string m_VisibleState = string.Empty;
        private XlsBiffSimpleValueRecord m_CalcCount;
        private XlsBiffSimpleValueRecord m_CalcMode;
        private XlsBiffRecord m_Delta;
        private XlsBiffDimensions m_Dimensions;
        private XlsBiffSimpleValueRecord m_Iteration;
        private XlsBiffSimpleValueRecord m_RefMode;
        private XlsBiffRecord m_Window;

        public XlsWorksheet(int index, XlsBiffBoundSheet refSheet)
        {
            this.m_Index = index;
            this.m_Name = refSheet.SheetName;
            this.m_dataOffset = refSheet.StartOffset;
            switch (refSheet.VisibleState)
            {
                case XlsBiffBoundSheet.SheetVisibility.Hidden:
                    this.m_VisibleState = "hidden";
                    return;

                case XlsBiffBoundSheet.SheetVisibility.VeryHidden:
                    this.m_VisibleState = "veryhidden";
                    return;
            }
            this.m_VisibleState = "visible";
        }

        public string Name =>
            this.m_Name;

        public string VisibleState =>
            this.m_VisibleState;

        public int Index =>
            this.m_Index;

        public uint DataOffset =>
            this.m_dataOffset;

        public XlsBiffSimpleValueRecord CalcMode
        {
            get => 
                this.m_CalcMode;
            set => 
                this.m_CalcMode = value;
        }

        public XlsBiffSimpleValueRecord CalcCount
        {
            get => 
                this.m_CalcCount;
            set => 
                this.m_CalcCount = value;
        }

        public XlsBiffSimpleValueRecord RefMode
        {
            get => 
                this.m_RefMode;
            set => 
                this.m_RefMode = value;
        }

        public XlsBiffSimpleValueRecord Iteration
        {
            get => 
                this.m_Iteration;
            set => 
                this.m_Iteration = value;
        }

        public XlsBiffRecord Delta
        {
            get => 
                this.m_Delta;
            set => 
                this.m_Delta = value;
        }

        public XlsBiffDimensions Dimensions
        {
            get => 
                this.m_Dimensions;
            set => 
                this.m_Dimensions = value;
        }

        public XlsBiffRecord Window
        {
            get => 
                this.m_Window;
            set => 
                this.m_Window = value;
        }
    }
}

