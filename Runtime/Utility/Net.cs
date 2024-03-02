using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace F8Framework.Core
{
    public static partial class Util
    {
        public static class Net
        {
            /// <summary>
            /// 使用HttpWebRequest Ping获取url根目录的文件列表；
            /// <para> 远程资源尽量使用拉丁字母，此方法对于中文文件名解析会产生异常！</para>
            /// </summary>
            /// <param name="url">资源定位地址</param>
            /// <returns>目录字符串数组</returns>
            public static string[] PingUrlRootFiles(string url)
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentNullException("URL is invalid !");
                List<string> files = new List<string>();
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();
                        GetUrlFiles(html, ref files);
                    }
                }

                return files.ToArray();
            }

            /// <summary>
            ///远程资源尽量使用英文字母命名；
            ///<para> 返回时只带File地址，不包含Folder</para>
            /// <para> 使用HttpWebRequest Ping并遍历url的文件列表 </para>
            /// </summary>
            /// <param name="url">资源定位地址</param>
            /// <param name="uris">返回的文件地址数组</param>
            public static void PingUrlFileList(string url, ref List<string> uris)
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentNullException("URL is invalid !");
                if (uris == null)
                    throw new ArgumentNullException("FileList is invalid !");
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();
                        Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                        MatchCollection matches = regex.Matches(html);
                        if (matches.Count <= 0)
                        {
                            return;
                        }

                        foreach (Match match in matches)
                        {
                            if (!match.Success)
                            {
                                continue;
                            }

                            var remoteUri = match.Groups["name"].ToString();
                            if (!remoteUri.EndsWith("../"))
                            {
                                var uriListPath = Util.IO.CombineURL(url, remoteUri);
                                if (remoteUri.EndsWith("/"))
                                {
                                    PingUrlFileList(uriListPath, ref uris);
                                }
                                else
                                {
                                    uris.Add(uriListPath);
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Ping URL是否存在；
            /// <para> Ping的过程本身是阻塞的，谨慎使用！</para> 
            /// </summary>
            /// <param name="url">资源地址</param>
            /// <returns>是否存在</returns>
            public static bool PingURI(string url)
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }

            /// <summary>
            /// Get the  public IP Address of the computer
            /// </summary>
            /// <param name="timeoutMilliseconds">timeout</param>
            /// <returns>public IP Address</returns>
            public static string GetPublicIPAddress(int timeoutMilliseconds = 5000)
            {
                var client = new WebClient();
                var task = client.DownloadStringTaskAsync("http://icanhazip.com");
                if (task.Wait(timeoutMilliseconds) == false)
                    return string.Empty;
                return task.Result.Trim();
            }

            /// <summary>
            /// Get the IPv4 IP Address of the local computer
            /// </summary>
            /// <returns>IP Address</returns>
            public static string GetLocalIPv4Address()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                throw new Exception("No network adapters with an IPv4 address in the system!");
            }

            /// <summary>
            /// Get the IPv6 IP Address of the local computer
            /// </summary>
            /// <returns>IP Address</returns>
            public static string GetILocalIPv6Address()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return ip.ToString();
                    }
                }

                throw new Exception("No network adapters with an IPv6 address in the system!");
            }

            /// <summary>
            /// 从web页面信息解析根目录下的文件信息。
            /// <para>若根目录下存在子目录，则子目录下的文件无法被解析</para>
            /// </summary>
            /// <param name="html">请求到的页面信息</param>
            /// <param name="files">解析到的首层文件信息</param>
            public static void GetUrlFiles(string html, ref List<string> files)
            {
                Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                MatchCollection matches = regex.Matches(html);
                if (matches.Count <= 0)
                {
                    return;
                }

                foreach (Match match in matches)
                {
                    if (!match.Success)
                        continue;
                    var assetName = match.Groups["name"].ToString();
                    if (!assetName.EndsWith("../"))
                    {
                        files.Add(assetName);
                    }
                }
            }
        }
    }
}