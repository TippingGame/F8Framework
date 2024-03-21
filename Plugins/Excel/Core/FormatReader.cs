namespace Excel.Core
{
    using System;
    using System.Runtime.CompilerServices;

    public class FormatReader
    {
        private const char escapeChar = '\\';

        public bool IsDateFormatString()
        {
            char[] anyOf = new char[] { 'y', 'm', 'd', 's', 'h', 'Y', 'M', 'D', 'S', 'H' };
            if (this.FormatString.IndexOfAny(anyOf) >= 0)
            {
                char[] chArray2 = anyOf;
                int index = 0;
                while (index < chArray2.Length)
                {
                    char ch = chArray2[index];
                    int pos = this.FormatString.IndexOf(ch);
                    while (true)
                    {
                        if (pos <= -1)
                        {
                            index++;
                            break;
                        }
                        if (!this.IsSurroundedByBracket(ch, pos) && (!this.IsPrecededByBackSlash(ch, pos) && !this.IsSurroundedByQuotes(ch, pos)))
                        {
                            return true;
                        }
                        pos = this.FormatString.IndexOf(ch, pos + 1);
                    }
                }
            }
            return false;
        }

        private bool IsPrecededByBackSlash(char dateChar, int pos) => 
            (pos != 0) ? (this.FormatString[pos - 1].CompareTo('\\') == 0) : false;

        private bool IsSurroundedByBracket(char dateChar, int pos)
        {
            if (pos == (this.FormatString.Length - 1))
            {
                return false;
            }
            int num4 = this.NumberOfUnescapedOccurances(']', this.FormatString.Substring(pos + 1)) - this.NumberOfUnescapedOccurances('[', this.FormatString.Substring(pos + 1));
            return ((((this.NumberOfUnescapedOccurances('[', this.FormatString.Substring(0, pos)) - this.NumberOfUnescapedOccurances(']', this.FormatString.Substring(0, pos))) % 2) == 1) && ((num4 % 2) == 1));
        }

        private bool IsSurroundedByQuotes(char dateChar, int pos)
        {
            if (pos == (this.FormatString.Length - 1))
            {
                return false;
            }
            int num2 = this.NumberOfUnescapedOccurances('"', this.FormatString.Substring(0, pos));
            return (((this.NumberOfUnescapedOccurances('"', this.FormatString.Substring(pos + 1)) % 2) == 1) && ((num2 % 2) == 1));
        }

        private int NumberOfUnescapedOccurances(char value, string src)
        {
            int num = 0;
            char ch = '\0';
            foreach (char ch2 in src)
            {
                if ((ch2 == value) && ((ch == '\0') || (ch.CompareTo('\\') != 0)))
                {
                    num++;
                    ch = ch2;
                }
            }
            return num;
        }

        public string FormatString { get; set; }
    }
}

