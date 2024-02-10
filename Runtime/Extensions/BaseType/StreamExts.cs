using System.Collections.Generic;
using System.IO;
using System.Text;

namespace F8Framework.Core
{
    public static class StreamExts
    {
        /// <summary>
        /// 输出数组；
        /// </summary>
        /// <param name="this">流</param>
        /// <returns>数组内容</returns>
        public static byte[] ToArray(this Stream @this)
        {
            @this.Position = 0;
            byte[] bytes = new byte[@this.Length];
            @this.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            @this.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        /// <summary>
        ///  读取所有内容行；
        /// </summary>
        /// <param name="this">流读取器</param>
        /// <param name="closeAfter">使用后关闭</param>
        /// <returns>行内容</returns>
        public static List<string> ReadAllLines(this StreamReader @this, bool closeAfter = true)
        {
            var stringList = new List<string>();
            string str;
            while ((str = @this.ReadLine()) != null)
            {
                stringList.Add(str);
            }
            if (closeAfter)
            {
                @this.Close();
                @this.Dispose();
            }
            return stringList;
        }
        /// <summary>
        /// 读取所有内容行；
        /// </summary>
        /// <param name="this">文件流</param>
        /// <param name="encoding">编码</param>
        /// <param name="closeAfter">使用后关闭</param>
        /// <returns>行内容</returns>
        public static List<string> ReadAllLines(this FileStream @this, Encoding encoding, bool closeAfter = true)
        {
            var stringList = new List<string>();
            string str;
            var sr = new StreamReader(@this, encoding);
            while ((str = sr.ReadLine()) != null)
            {
                stringList.Add(str);
            }
            if (closeAfter)
            {
                sr.Close();
                sr.Dispose();
                @this.Close();
                @this.Dispose();
            }

            return stringList;
        }
        /// <summary>
        /// 读取所有文本；
        /// </summary>
        /// <param name="this">文件流</param>
        /// <param name="encoding">编码</param>
        /// <param name="closeAfter">使用后关闭</param>
        /// <returns>文本内容</returns>
        public static string ReadAllText(this FileStream @this, Encoding encoding, bool closeAfter = true)
        {
            var sr = new StreamReader(@this, encoding);
            var text = sr.ReadToEnd();
            if (closeAfter)
            {
                sr.Close();
                sr.Dispose();
                @this.Close();
                @this.Dispose();
            }

            return text;
        }
        /// <summary>
        /// 写入所有文本;
        /// </summary>
        /// <param name="this">文件流</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="closeAfter">使用后关闭</param>
        public static void WriteAllText(this FileStream @this, string content, Encoding encoding, bool closeAfter = true)
        {
            var sw = new StreamWriter(@this, encoding);
            @this.SetLength(0);
            sw.Write(content);
            if (closeAfter)
            {
                sw.Close();
                sw.Dispose();
                @this.Close();
                @this.Dispose();
            }
        }
    }
}
