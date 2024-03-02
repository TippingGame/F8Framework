namespace F8Framework.Core
{
    /// <summary>
    /// 支持localhost地址与http地址；
    /// </summary>
    public interface IDownloadUrlHelper
    {
        /// <summary>
        /// 解析资源地址，并返回ping到的文件相对地址数组；
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>Ping到的相对于url的地址</returns>
        string[] ParseUrlToRelativeUris(string url);

        /// <summary>
        /// 解析资源地址，并返回ping到的文件绝对地址数组；
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>Ping到uri的绝对地址</returns>
        string[] ParseUrlToAbsoluteUris(string url);
    }
}