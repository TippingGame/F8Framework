namespace F8Framework.Core
{
    /// <summary>
    /// download模块数据；
    /// </summary>
    internal class DownloadDataProxy
    {
        /// <summary>
        ///https://stackoverflow.com/questions/13582409/http-client-timeout-and-server-timeout
        /// </summary>
        static int downloadTimeout = 30;

        /// <summary>
        /// 终止时删除下载中的文件
        /// </summary>
        public static bool DeleteFileOnAbort { get; set; }

        /// <summary>
        ///任务过期时间，以秒为单位
        /// </summary>
        public static int DownloadTimeout
        {
            get { return downloadTimeout; }
            set
            {
                downloadTimeout = value;
                if (downloadTimeout < 0)
                    downloadTimeout = 0;
            }
        }

        static int redirectLimit = 32;

        public static int RedirectLimit
        {
            get { return redirectLimit; }
            set { redirectLimit = value; }
        }
    }
}