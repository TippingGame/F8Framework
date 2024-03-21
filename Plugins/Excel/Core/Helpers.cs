namespace Excel.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class Helpers
    {
        private static Regex re = new Regex("_x([0-9A-F]{4,4})_");

        public static void AddColumnHandleDuplicate(DataTable table, string columnName)
        {
            string str = columnName;
            DataColumn column = table.Columns[columnName];
            for (int i = 1; column != null; i++)
            {
                str = $"{columnName}_{i}";
                column = table.Columns[str];
            }
            table.Columns.Add(str, typeof(object));
        }

        public static string ConvertEscapeChars(string input) => 
            re.Replace(input, m => ((char) uint.Parse(m.Groups[1].Value, NumberStyles.HexNumber)).ToString());

        public static object ConvertFromOATime(double value)
        {
            if ((value >= 0.0) && (value < 60.0))
            {
                value++;
            }
            return DateTime.FromOADate(value);
        }

        internal static void FixDataTypes(DataSet dataset)
        {
            List<DataTable> list = new List<DataTable>(dataset.Tables.Count);
            bool flag = false;
            foreach (DataTable table in dataset.Tables)
            {
                if (table.Rows.Count == 0)
                {
                    list.Add(table);
                    continue;
                }
                DataTable item = null;
                int columnIndex = 0;
                while (true)
                {
                    if (columnIndex >= table.Columns.Count)
                    {
                        if (item == null)
                        {
                            list.Add(table);
                        }
                        else
                        {
                            item.BeginLoadData();
                            foreach (DataRow row2 in table.Rows)
                            {
                                item.ImportRow(row2);
                            }
                            item.EndLoadData();
                            list.Add(item);
                        }
                        break;
                    }
                    Type objB = null;
                    foreach (DataRow row in table.Rows)
                    {
                        if (!row.IsNull(columnIndex))
                        {
                            Type type = row[columnIndex].GetType();
                            if (!ReferenceEquals(type, objB))
                            {
                                if (objB != null)
                                {
                                    objB = null;
                                    break;
                                }
                                objB = type;
                            }
                        }
                    }
                    if (objB != null)
                    {
                        flag = true;
                        item ??= table.Clone();
                        item.Columns[columnIndex].DataType = objB;
                    }
                    columnIndex++;
                }
            }
            if (flag)
            {
                dataset.Tables.Clear();
                dataset.Tables.AddRange(list.ToArray());
            }
        }

        public static double Int64BitsToDouble(long value) => 
            BitConverter.ToDouble(BitConverter.GetBytes(value), 0);

        public static bool IsSingleByteEncoding(Encoding encoding) => 
            encoding.IsSingleByte;
    }
}

