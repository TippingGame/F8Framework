using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace F8Framework.Core
{
    [RequireComponent(typeof(RawImage))]
    public class WebCacheImage : MonoBehaviour
    {
        [SerializeField] private string url;

        [SerializeField] private bool preLoad = true;

        [SerializeField] private LoadTextureEvent onLoadTexture = new LoadTextureEvent();

        private RawImage image;

        private CacheRequestOperation operation;

        private bool isInitilize = false;

        public RawImage Image
        {
            get
            {
                if (image == null)
                {
                    image = GetComponent<RawImage>();
                }

                return image;
            }

            private set { image = value; }
        }

        public CacheRequestOperation Operation
        {
            get { return operation; }
        }


        public CacheInfo CacheInfo
        {
            get { return operation; }
        }

        private void Awake()
        {
            CacheStorageInternal.Initialize();

            if (image == null)
            {
                image = GetComponent<RawImage>();
            }
        }

        private void OnEnable()
        {
            if (isInitilize == false)
            {
                if (preLoad == true)
                {
                    Preload();
                }

                isInitilize = true;
            }
        }

        public void Preload()
        {
            if (image != null)
            {
                operation = CacheStorage.GetCachedTexture(url, (cachedTexture) =>
                {
                    if (cachedTexture != null)
                    {
                        Image.texture = cachedTexture.texture;
                    }
                });
            }
        }

        public void LoadImage()
        {
            if (image != null)
            {
                Image.texture = null;

                if (string.IsNullOrEmpty(this.url) == false)
                {
                    operation = CacheStorage.RequestTexture(url, preLoad, (cachedTexture) =>
                    {
                        if (cachedTexture != null)
                        {
                            Image.texture = cachedTexture.texture;
                        }
                    });
                }
            }
        }

        public void SetUrl(string url)
        {
            if (this.url != url)
            {
                this.url = url;

                if (operation != null)
                {
                    operation.Cancel();
                    operation = null;
                }

                LoadImage();
            }
        }

        public void SetLoadTextureEvent(UnityAction<Texture> onListener)
        {
            CleatLoadTextureEvent();
            AddLoadTextureEvent(onListener);
        }

        public void AddLoadTextureEvent(UnityAction<Texture> onListener)
        {
            onLoadTexture.AddListener(onListener);
        }

        public void CleatLoadTextureEvent()
        {
            onLoadTexture = new LoadTextureEvent();
        }

        [Serializable]
        public class LoadTextureEvent : UnityEvent<Texture>
        {
            public LoadTextureEvent()
            {
            }
        }
    }
}