using UnityEngine;
using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    [Serializable]
    public class CacheControl
    {
        private static readonly char[] separatorCommas = new char[] { ',' };
        private static readonly char[] separatorEqual = new char[] { '=' };
        public StringToValue<int> maxAge;

        public bool noCache;
        public bool noStore;
        public bool mustRevalidate;

        public CacheControl()
        {
        }

        public static Dictionary<string, string> GetElements(string cacheControl)
        {
            Dictionary<string, string> dicElements = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(cacheControl) == false)
            {
                string[] elements = cacheControl.Split(separatorCommas, StringSplitOptions.RemoveEmptyEntries);
                foreach (string element in elements)
                {
                    string[] split = element.Trim().Split(separatorEqual, StringSplitOptions.RemoveEmptyEntries);

                    string key = split[0].ToLower();
                    string value = string.Empty;
                    if (split.Length > 1)
                    {
                        value = split[1];
                    }

                    dicElements.Add(key, value);
                }
            }

            return dicElements;
        }

        public CacheControl(Dictionary<string, string> elements)
        {
            string getValue;
            if (elements.TryGetValue(HttpElement.MAX_AGE, out getValue) == true)
            {
                maxAge = getValue;
            }
            else
            {
                maxAge = null;
            }

            noStore = elements.ContainsKey(HttpElement.NO_STORE) == true;
            noCache = elements.ContainsKey(HttpElement.NO_CACHE) == true;
            mustRevalidate = elements.ContainsKey(HttpElement.MUST_REVALIDATE);
        }
    }
}