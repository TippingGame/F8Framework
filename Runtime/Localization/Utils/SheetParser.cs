using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace F8Framework.Core
{
    public static class SheetParser
    {
        public enum Delimiter
        {
            Comma,
            Tab
        }

        static readonly Dictionary<Delimiter, char> Delimiters = new Dictionary<Delimiter, char> { { Delimiter.Comma, ',' }, { Delimiter.Tab, '\t' } };

        /// <summary>
        /// 从指定路径加载表格数据。
        /// </summary>
        /// <param name="path">表格文件路径。</param>
        /// <param name="delimiter">分隔符。</param>
        /// <param name="encoding">文本编码类型。（默认为 UTF-8）</param>
        /// <returns>解析后的嵌套列表。</returns>
        public static List<List<string>> LoadFromPath(string path, Delimiter delimiter = Delimiter.Comma, Encoding encoding = null)
        {
#if UNITY_EDITOR
            if (!Equals(GetType(path), Encoding.UTF8))
            {
                LogF8.LogError("请使用UTF8编码文件：" + path);
                return null;
            }
#endif
            encoding = encoding ?? Encoding.UTF8;
            var data = File.ReadAllText(path);
            return Parse(data, delimiter);
        }

        /// <summary>
        /// 从字符串中加载表格数据。
        /// </summary>
        /// <param name="data">表格字符串。</param>
        /// <param name="delimiter">分隔符。</param>
        /// <returns>解析后的嵌套列表。</returns>
        public static List<List<string>> LoadFromString(string data, Delimiter delimiter = Delimiter.Comma)
        {
            return Parse(data, delimiter);
        }

        static List<List<string>> Parse(string data, Delimiter delimiter)
        {
            var sheet = new List<List<string>>();
            var row = new List<string>();
            var cell = new StringBuilder();
            var afterQuote = false;
            var insideQuote = false;
            var readyToEndQuote = false;
            var delimiterChar = Delimiters[delimiter];

            ConvertToCrlf(ref data);

            foreach (var character in data)
            {
                // 在引号内部。
                if (insideQuote)
                {
                    if (afterQuote)
                    {
                        if (character == '"')
                        {
                            // 连续引号：一个引号。
                            cell.Append("\"");
                            afterQuote = false;
                        }
                        else if (readyToEndQuote && character != '"')
                        {
                            // 非连续引号：引号结束。
                            afterQuote = false;
                            insideQuote = false;

                            if (character == delimiterChar)
                            {
                                AddCell(row, cell);
                            }
                        }
                        else
                        {
                            cell.Append(character);
                            afterQuote = false;
                        }

                        readyToEndQuote = false;
                    }
                    else
                    {
                        if (character == '"')
                        {
                            // 引号内部的引号。
                            // 由下一个字符决定。
                            afterQuote = true;
                            readyToEndQuote = true;
                        }
                        else
                        {
                            cell.Append(character);
                        }
                    }
                }
                else
                {
                    // 在引号外部。
                    if (character == delimiterChar)
                    {
                        AddCell(row, cell);
                    }
                    else if (character == '\n')
                    {
                        AddCell(row, cell);
                        AddRow(sheet, ref row);
                    }
                    else if (character == '"')
                    {
                        afterQuote = true;
                        insideQuote = true;
                    }
                    else
                    {
                        cell.Append(character);
                    }
                }
            }

            // 添加最后一行，但不包括空行。
            if (row.Count != 0 || cell.Length != 0)
            {
                AddCell(row, cell);
                AddRow(sheet, ref row);
            }

            return sheet;
        }

        static void AddCell(List<string> row, StringBuilder cell)
        {
            row.Add(cell.ToString());
            cell.Length = 0; // 旧版 C#。
        }

        static void AddRow(List<List<string>> sheet, ref List<string> row)
        {
            sheet.Add(row);
            row = new List<string>();
        }

        static void ConvertToCrlf(ref string data)
        {
            data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");
        }
        
                
        /// <summary>
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }
 
        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;
 
            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
 
        }
 
        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;
            //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                LogF8.LogError("非预期的byte格式");
            }
            return true;
        }
    }
}