namespace Excel.Core
{
    using System;
    using System.Text.RegularExpressions;

    public static class ReferenceHelper
    {
        public static int[] ReferenceToColumnAndRow(string reference)
        {
            Regex regex = new Regex("([a-zA-Z]*)([0-9]*)");
            string str = regex.Match(reference).Groups[1].Value.ToUpper();
            string s = regex.Match(reference).Groups[2].Value;
            int num = 0;
            int num2 = 1;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                int num4 = (str[i] - 'A') + 1;
                num += num2 * num4;
                num2 *= 0x1a;
            }
            return new int[] { int.Parse(s), num };
        }
    }
}

