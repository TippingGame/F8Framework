using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace F8Framework.Core
{
    public class GpmWebRequest
    {
        const string ERROR_URL_EMPTY = "url is empty";

        public class Result
        {
            public Result(string url, bool isSuccess, long responseCode, string message)
            {
                this.url = url;
                this.isSuccess = isSuccess;

                this.responseCode = responseCode;

                this.message = message;

                this.request = null;
            }

            public Result(UnityWebRequest request)
            {
                this.url = request.url;

                this.isSuccess = UnityWebRequestHelper.IsSuccess(request);

                this.responseCode = request.responseCode;

                this.message = request.error;

                this.request = request;
            }

            ~Result()
            {
                this.request = null;
            }

            public bool isSuccess;

            public string url;

            public long responseCode;

            public string message;

            public UnityWebRequest request;
        }

        private UnityWebRequest request;
        private Dictionary<string, string> header = new Dictionary<string, string>();

        public GpmWebRequest()
        {
            request = new UnityWebRequest();
        }

        ~GpmWebRequest()
        {
            if (request != null)
            {
                request.Dispose();
                request = null;
            }
        }

        public void SetRequestHeader(string name, string value)
        {
            header[name] = value;
        }

        public void Get(string url, System.Action<Result> callback = null)
        {
            StartCoroutine(getRequestRoutine(url, callback));
        }

        public void Post(string url, string value, System.Action<Result> callback = null)
        {
            StartCoroutine(postRequestRoutine(url, value, callback));
        }

        public void Post(string url, string value, Dictionary<string, string> header,
            System.Action<Result> callback = null)
        {
            this.header = header;
            StartCoroutine(postRequestRoutine(url, value, callback));
        }


        private void StartCoroutine(IEnumerator routine)
        {
#if ENABLE_MANAGED_CORUTINE
            ManagedCoroutine.Start(routine);
#else

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                EditorCoroutine.Start(routine);
                return;
            }
#endif
            ManagedCoroutineInstance.Instance.StartCoroutine(routine);

#endif
        }

        private IEnumerator getRequestRoutine(string url, System.Action<Result> callback = null)
        {
            if (string.IsNullOrEmpty(url) == true)
            {
                if (callback != null)
                {
                    callback(new Result("", false, 0, ERROR_URL_EMPTY));
                }

                yield break;
            }

            request.method = UnityWebRequest.kHttpVerbGET;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.url = url;
            request.useHttpContinue = false;

            foreach (var pair in header)
            {
                request.SetRequestHeader(pair.Key, pair.Value);
            }

            yield return request.SendWebRequest();

            if (callback != null)
            {
                callback(new Result(request));
            }
        }

        private IEnumerator postRequestRoutine(string url, string value, System.Action<Result> callback = null)
        {
            if (string.IsNullOrEmpty(url) == true)
            {
                if (callback != null)
                {
                    callback(new Result("", false, 0, ERROR_URL_EMPTY));
                }

                yield break;
            }

            request.method = UnityWebRequest.kHttpVerbPOST;
            request.url = url;

            foreach (var pair in header)
            {
                request.SetRequestHeader(pair.Key, pair.Value);
            }

            byte[] send = new System.Text.UTF8Encoding().GetBytes(value);
            request.uploadHandler = new UploadHandlerRaw(send);

            yield return request.SendWebRequest();

            if (callback != null)
            {
                callback(new Result(request));
            }
        }
    }
}