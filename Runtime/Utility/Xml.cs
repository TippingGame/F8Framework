using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace F8Framework.Core
{
    public static partial class Util
    {
        public static class Xml
        {
            public enum ResponseCode
            {
                SUCCESS,
                FILE_NOT_FOUND_ERROR,
                PATH_IS_NULL_ERROR,
                DATA_IS_NULL_ERROR,
                UNKNOWN_ERROR,
            }

            public static void SaveXmlToFile<T>(string path, T data, Action<ResponseCode, string> callback)
            {
                if (string.IsNullOrEmpty(path) == true)
                {
                    callback(ResponseCode.PATH_IS_NULL_ERROR, path);
                    return;
                }

                if (data == null)
                {
                    callback(ResponseCode.DATA_IS_NULL_ERROR, null);
                    return;
                }

                ResponseCode responseCode = ResponseCode.SUCCESS;
                string errorMessage = string.Empty;

                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        serializer.Serialize(new StreamWriter(stream, Encoding.UTF8), data);
                    }
                }
                catch (Exception e)
                {
                    responseCode = ResponseCode.UNKNOWN_ERROR;
                    errorMessage = e.Message;
                }
                finally
                {
                    callback(responseCode, errorMessage);
                }
            }

            public static void LoadXmlFromFile<T>(string path, Action<ResponseCode, T, string> callback) where T : class
            {
                if (string.IsNullOrEmpty(path) == true)
                {
                    callback(ResponseCode.FILE_NOT_FOUND_ERROR, default(T), path);
                    return;
                }

                T result = null;
                ResponseCode responseCode = ResponseCode.SUCCESS;
                string errorMessage = string.Empty;

                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        result = (T)serializer.Deserialize(stream);
                    }
                }
                catch (Exception e)
                {
                    responseCode = ResponseCode.UNKNOWN_ERROR;
                    errorMessage = e.Message;
                    result = default(T);
                }
                finally
                {
                    callback(responseCode, result, errorMessage);
                }
            }

            public static void LoadXmlFromText<T>(string text, Action<ResponseCode, T, string> callback) where T : class
            {
                if (string.IsNullOrEmpty(text) == true)
                {
                    callback(ResponseCode.DATA_IS_NULL_ERROR, default(T), null);
                    return;
                }

                T result = null;
                ResponseCode responseCode = ResponseCode.SUCCESS;
                string errorMessage = string.Empty;

                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using (TextReader textReader = new StringReader(text))
                    {
                        result = (T)serializer.Deserialize(textReader);
                    }
                }
                catch (Exception e)
                {
                    responseCode = ResponseCode.UNKNOWN_ERROR;
                    errorMessage = e.Message;
                    result = default(T);
                }
                finally
                {
                    callback(responseCode, result, errorMessage);
                }
            }
        }
    }
}