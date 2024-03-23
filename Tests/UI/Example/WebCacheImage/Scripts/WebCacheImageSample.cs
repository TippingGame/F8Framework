using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using F8Framework.Core;

namespace F8Framework.Tests
{
    public class WebCacheImageSample : MonoBehaviour
    {
        public InputField searchText;

        public InputField url;
        public InfiniteScroll scrollList = null;

        private List<TestItemData> dataList = new List<TestItemData>();

        private void Start()
        {
            searchText.text = "gamepackagemanager";
            url.text = "https://assetstorev1-prd-cdn.unity3d.com/key-image/f0fa9cde-e562-4976-93a1-9e1f161ce1b8.jpg";
        }

        public void SearchImage()
        {
            GpmWebRequest request = new GpmWebRequest();
            request.Get("https://www.google.com/search?q=" + searchText.text + "&source=lnms&tbm=isch", result=>
            {
                byte[] resultBytes = result.request.downloadHandler.data;

                Encoding enc = new UTF8Encoding();
                string resultText = enc.GetString(resultBytes);

                List<string> images = new List<string>();
                string pattern = @"<(img)\b[^>]*>";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(resultText);

                for (int i = 0, l = matches.Count; i < l; i++)
                {
                    string patternURL = @"(https://.*);";
                    Regex rgxURL = new Regex(patternURL, RegexOptions.IgnoreCase);
                    Match matchesURL = rgxURL.Match(matches[i].Value);
                    string path = matchesURL.Groups[1].Value;

                    
                    images.Add(path);

                    if(string.IsNullOrEmpty(path) == false)
                    {
                        scrollList.InsertData(new WebCacheImageSampleData(path));
                    }
                    
                }

            });
        }

        public void AddURL()
        {
            scrollList.InsertData(new WebCacheImageSampleData(url.text));
        }

        public void Clear()
        {
            scrollList.Clear();

            dataList.Clear();
        }
    }
}