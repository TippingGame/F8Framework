using System.Collections.Generic;
using System.IO;

namespace F8Framework.Core
{
    /// <summary>
    /// 若URL根目录为http://127.0.0.1:80/res/，文件地址为http://127.0.0.1:80/res/test.txt； 则test.txt即为FileList中的地址；
    /// </summary>
    public class DefaultDownloadUrlHelper : IDownloadUrlHelper
    {
        /// <summary>
        /// URI的相对路径；
        /// </summary>
        List<string> relativeUris = new List<string>();

        /// <summary>
        /// URI的绝对路径；
        /// </summary>
        List<string> absoluteUris = new List<string>();

        /// <summary>
        /// 解析资源地址，并返回ping到的文件相对地址数组；
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>Ping到的相对于url的地址</returns>
        public string[] ParseUrlToRelativeUris(string url)
        {
            relativeUris.Clear();
            absoluteUris.Clear();
            var len = url.Length;
            if (Directory.Exists(url))
            {
                Util.IO.TraverseFolderFile(url, (info) =>
                {
                    var path = info.FullName;
                    var name = info.FullName.Remove(0, len + 1);
                    relativeUris.Add(name);
                });
            }
            else
            {
                var result = Util.Net.PingURI(url);
                if (result)
                {
                    Util.Net.PingUrlFileList(url, ref absoluteUris);
                    var length = absoluteUris.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var uri = absoluteUris[i].Remove(0, len);
                        relativeUris.Add(uri);
                    }
                }
            }

            var relUriArray = relativeUris.ToArray();
            relativeUris.Clear();
            absoluteUris.Clear();
            return relUriArray;
        }

        /// <summary>
        /// 解析资源地址，并返回ping到的文件绝对地址数组；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <returns>Ping到uri的绝对地址</returns>
        public string[] ParseUrlToAbsoluteUris(string url)
        {
            relativeUris.Clear();
            absoluteUris.Clear();
            var len = url.Length;
            if (Directory.Exists(url))
            {
                Util.IO.TraverseFolderFile(url, (info) =>
                {
                    var path = info.FullName;
                    relativeUris.Add(path);
                });
            }
            else
            {
                var result = Util.Net.PingURI(url);
                if (result)
                {
                    Util.Net.PingUrlFileList(url, ref absoluteUris);
                    var length = absoluteUris.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var uri = absoluteUris[i];
                        relativeUris.Add(uri);
                    }
                }
            }

            var relUriArray = relativeUris.ToArray();
            relativeUris.Clear();
            absoluteUris.Clear();
            return relUriArray;
        }
    }
}