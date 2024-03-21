namespace Excel.Core.OpenXmlFormat
{
    using System;
    using System.Runtime.InteropServices;

    internal class XlsxDimension
    {
        private int _FirstRow;
        private int _LastRow;
        private int _FirstCol;
        private int _LastCol;

        public XlsxDimension(string value)
        {
            this.ParseDimensions(value);
        }

        public XlsxDimension(int rows, int cols)
        {
            this.FirstRow = 1;
            this.LastRow = rows;
            this.FirstCol = 1;
            this.LastCol = cols;
        }

        public void ParseDimensions(string value)
        {
            int num;
            int num2;
            string[] strArray = value.Split(new char[] { ':' });
            XlsxDim(strArray[0], out num, out num2);
            this.FirstCol = num;
            this.FirstRow = num2;
            if (strArray.Length == 1)
            {
                this.LastCol = this.FirstCol;
                this.LastRow = this.FirstRow;
            }
            else
            {
                XlsxDim(strArray[1], out num, out num2);
                this.LastCol = num;
                this.LastRow = num2;
            }
        }

        public static void XlsxDim(string value, out int val1, out int val2)
        {
            int index = 0;
            val1 = 0;
            int[] numArray = new int[value.Length - 1];
            while ((index < value.Length) && !char.IsDigit(value[index]))
            {
                numArray[index] = (value[index] - 'A') + 1;
                index++;
            }
            for (int i = 0; i < index; i++)
            {
                val1 += (int) (numArray[i] * Math.Pow(26.0, (double) ((index - i) - 1)));
            }
            val2 = int.Parse(value.Substring(index));
        }

        public int FirstRow
        {
            get => 
                this._FirstRow;
            set => 
                this._FirstRow = value;
        }

        public int LastRow
        {
            get => 
                this._LastRow;
            set => 
                this._LastRow = value;
        }

        public int FirstCol
        {
            get => 
                this._FirstCol;
            set => 
                this._FirstCol = value;
        }

        public int LastCol
        {
            get => 
                this._LastCol;
            set => 
                this._LastCol = value;
        }
    }
}

