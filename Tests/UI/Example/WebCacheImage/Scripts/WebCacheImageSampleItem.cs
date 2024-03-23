using System.Text;
using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class WebCacheImageSampleData : InfiniteScrollData
    {
        public WebCacheImageSampleData(string url)
        {
            this.url = url;
        }

        public string url;
    }

    public class WebCacheImageSampleItem : InfiniteScrollItem
    {
        public WebCacheImage cache;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            WebCacheImageSampleData cacheData = scrollData as WebCacheImageSampleData;

            cache.SetLoadTextureEvent(OnLoadTexture);

            cache.SetUrl(cacheData.url);
        }

        public void OnLoadTexture(Texture tex)
        {
        }
    }

}