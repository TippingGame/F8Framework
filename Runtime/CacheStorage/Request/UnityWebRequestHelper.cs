using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Networking;
using System.Net;

namespace F8Framework.Core
{
    public class UnityWebRequestHelper
    {
        public const string NETWORK_ERROR_MESSAGE = "Failed to send request. (Error has occurred in network.)";
        public const string PROTOCOL_ERROR_MESSAGE = "Failed to send request. responseCode:{0}, error:{1}, text:{2}";
        public const string EMPTY_CONTENT_MESSAGE = "received a null/empty buffer";

        private const int TIMEOUT = 10;

        private UnityWebRequest request;

        public UnityWebRequestHelper(UnityWebRequest request)
        {
            this.request = request;
        }

        public IEnumerator SendWebRequestAndDispose(Action<UnityWebRequest> callback = null)
        {
            try
            {
                request.timeout = TIMEOUT;
                request.SetRequestHeader("Content-Type", "application/json");

                request.disposeCertificateHandlerOnDispose = true;
                request.disposeDownloadHandlerOnDispose = true;
                request.disposeUploadHandlerOnDispose = true;

                yield return request.SendWebRequest();

                while (request.isDone == false)
                {
                    yield return null;
                }

                PrintLog();

                if (callback != null)
                {
                    callback(request);
                }
            }
            finally
            {
                request.Dispose();
                request = null;
            }
        }

        [Conditional("GPM_INDICATOR_DEBUGGING")]
        private void PrintLog()
        {
            if (IsError(request) == true)
            {
                LogF8.Log(NETWORK_ERROR_MESSAGE + GetType());
            }
            else if (IsProtocolError(request) == true)
            {
                LogF8.Log(
                    string.Format(
                        PROTOCOL_ERROR_MESSAGE,
                        request.responseCode,
                        request.error,
                        request.downloadHandler.text) + GetType());
            }
            else if (string.IsNullOrEmpty(request.downloadHandler.text) == true)
            {
                LogF8.Log(EMPTY_CONTENT_MESSAGE + GetType());
            }
        }

        public static bool IsSuccess(UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                return false;
            }

            return true;
        }

        public static bool IsError(UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            if (request.result == UnityWebRequest.Result.InProgress ||
                request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                return true;
            }
            else
            {
                return false;
            }
#elif UNITY_2017_1_OR_NEWER
            return request.isNetworkError;
#else
            return request.isError;
#endif
        }

        public static bool IsProtocolError(UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            if (request.result == UnityWebRequest.Result.ProtocolError)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                if ((HttpStatusCode)request.responseCode != HttpStatusCode.OK)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsConnectionError(UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            return request.result == UnityWebRequest.Result.ConnectionError;
#elif UNITY_2017_1_OR_NEWER
            return request.isNetworkError;
#else
            return request.isError;
#endif
        }

        public static bool IsNotFoundError(UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            if (request.result == UnityWebRequest.Result.ProtocolError &&
                (HttpStatusCode)request.responseCode == HttpStatusCode.NotFound)
            {
                return true;
            }
#endif

            if ((HttpStatusCode)request.responseCode == HttpStatusCode.NotFound)
            {
                return true;
            }

            return false;
        }
    }
}