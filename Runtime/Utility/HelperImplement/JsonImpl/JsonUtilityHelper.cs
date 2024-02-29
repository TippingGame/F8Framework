using System;
using UnityEngine;
namespace F8Framework.Core
{
    /// <summary>
    /// 别乱用，字典序列化不了
    /// </summary>
    public class JsonUtilityHelper : Utility.Json.IJsonHelper
    {
        public string ToJson(object obj, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(obj);
        }

        public T ToObject<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public object ToObject(string json, Type objectType)
        {
            return JsonUtility.FromJson(json, objectType);
        }
    }
}
