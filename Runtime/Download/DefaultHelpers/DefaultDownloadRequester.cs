using System;
using System.Collections;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    public class DefaultDownloadRequester : IDownloadRequester
    {
        /// <summary>
        /// 获取URI单个文件的大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="callback">回调</param>
        public void GetUriFileSizeAsync(string uri, Action<long> callback)
        {
            Util.Unity.StartCoroutine(EnumGetFileSize(uri, callback));
        }

        /// <summary>
        /// 获取多个URL地址下的所有文件的总和大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <param name="callback">回调</param>
        public void GetUriFilesSizeAsync(string[] uris, Action<long> callback)
        {
            Util.Unity.StartCoroutine(EnumGetMultiFilesSize(uris, callback));
        }

        IEnumerator EnumGetMultiFilesSize(string[] uris, Action<long> callback)
        {
            long overallSize = 0;
            var length = uris.Length;
            for (int i = 0; i < length; i++)
            {
                yield return EnumGetFileSize(uris[i], size =>
                {
                    if (size >= 0)
                        overallSize += size;
                });
            }

            callback?.Invoke(overallSize);
        }

        IEnumerator EnumGetFileSize(string uri, Action<long> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Head(uri))
            {
                yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError &&
                    request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    string contentLengthHeader = request.GetResponseHeader("Content-Length");
                    if (!string.IsNullOrEmpty(contentLengthHeader) && long.TryParse(contentLengthHeader, out long fileLength))
                    {
                        callback?.Invoke(fileLength);
                    }
                    else
                    {
                        LogF8.LogError("Content-Length 标头找不到或无效: " + request.error);
                        callback?.Invoke(-1);
                    }
                }
                else
                {
                    LogF8.LogError("检索文件大小时出错: " + request.error);
                    callback?.Invoke(-1);
                }
            }
        }
    }
}