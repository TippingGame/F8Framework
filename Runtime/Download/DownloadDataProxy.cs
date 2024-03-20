namespace F8Framework.Core
{
    /// <summary>
    /// download模块数据；
    /// </summary>
    internal class DownloadDataProxy
    {
        /// <summary>
        /// 终止时删除下载中的文件
        /// </summary>
        public static bool DeleteFileOnAbort { get; set; }

        static int redirectLimit = 32;

        public static int RedirectLimit
        {
            get { return redirectLimit; }
            set { redirectLimit = value; }
        }
    }
}