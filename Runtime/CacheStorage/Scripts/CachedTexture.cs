using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class CachedTexture
    {
        public CacheInfo info;
        public Texture2D texture;
        public bool requested = false;
        public bool updateData = false;

        
        public void DestroyTexture()
        {
            Object.DestroyImmediate(texture);
            texture = null;

            ReleaseCache();
        }

        public void ReleaseCache()
        {
            requested = false;
            updateData = false;

            CachedTextureManager.Release(info);
        }
    }

    public static class CachedTextureManager
    {
        private static Dictionary<CacheInfo, CachedTexture> cachedTextureList = new Dictionary<CacheInfo, CachedTexture>();

        public static CachedTexture Get(CacheInfo info)
        {
            CachedTexture cachedTexture = null;
            if (cachedTextureList.TryGetValue(info, out cachedTexture) == true)
            {
            }

            return cachedTexture;
        }

        public static CachedTexture Cache(CacheInfo info, bool requested, bool updateData, byte[] data)
        {
            CachedTexture cachedTexture = null;

            if (cachedTextureList.TryGetValue(info, out cachedTexture) == false)
            {
                cachedTexture = new CachedTexture();

                cachedTexture.info = info;
                cachedTexture.texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                if (updateData == false)
                {
                    cachedTexture.texture.LoadImage(data, true);
                }

                cachedTextureList.Add(info, cachedTexture);
            }

            if (requested == true)
            {
                cachedTexture.requested = true;
            }

            if (cachedTexture.texture == null)
            {
                cachedTexture.texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                updateData = true;
            }

            if (updateData == true)
            {
                cachedTexture.updateData = true;
                cachedTexture.texture.LoadImage(data, true);
            }


            return cachedTexture;
        }

        public static void Release(CacheInfo info)
        {
            cachedTextureList.Remove(info);
        }
        public static void Clear()
        {
            cachedTextureList.Clear();
        }

        public static void DestoryTextureAll()
        {
            foreach(var pair in cachedTextureList)
            {
                CachedTexture cachedTexture = pair.Value;
                if (cachedTexture != null &&
                    cachedTexture.texture != null)
                {
                    Object.DestroyImmediate(cachedTexture.texture);
                }   
            }

            cachedTextureList.Clear();
        }
    }
}
