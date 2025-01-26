using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class SpriteSequenceFrame : MonoBehaviour
    {
        public Sprite[] sprites;
        
        private Image image;
        private SpriteRenderer spriteRenderer;

        private int currentFrame;
        private int totalFrame;
        private bool isPlay = false;

        public string atlasName = "";
        public int aniNum = 16;
        public float loopInterval = 0;

        public int frameRate = 16;
        private float updateDeltaTime;

        private float lastUpdateTime;
        public bool loop = false;

        public bool autoPlay = false;

        private List<string> spriteNames = new List<string>();

        private void Awake()
        {
            for (int i = 0; i < aniNum; i++)
            {
                spriteNames.Add(atlasName + "_" + i);
            }
        }

        private void OnEnable()
        {
            if (autoPlay)
            {
                Play();
            }
        }

        [UnityEngine.ContextMenu("Play")]
        public void Play()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (image || spriteRenderer)
            {
                currentFrame = 0;
                totalFrame = aniNum;
                updateDeltaTime = 1 / (float)frameRate;
                lastUpdateTime = Time.time;
                isPlay = true;
                SetTexture();
            }
        }

        [UnityEngine.ContextMenu("Stop")]
        public void Stop()
        {
            isPlay = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isPlay)
            {
                var deltaTime = Time.time - lastUpdateTime;
                if (deltaTime > updateDeltaTime)
                {
                    currentFrame = currentFrame + 1;
                    if (currentFrame >= totalFrame)
                    {
                        if (loop)
                        {
                            currentFrame = currentFrame - totalFrame;
                        }
                        else
                        {
                            Stop();
                            return;
                        }
                    }

                    SetTexture();
                    if (loop && currentFrame == totalFrame - 1)
                    {
                        lastUpdateTime += updateDeltaTime + loopInterval;
                    }
                    else
                    {
                        lastUpdateTime += updateDeltaTime;
                    }
                }
            }
        }

        private void SetTexture()
        {
            if (image)
            {
                image.sprite = sprites.Length > 0 ? sprites[currentFrame % aniNum] : AssetManager.Instance.Load<Sprite>(atlasName, spriteNames[currentFrame % aniNum]);
            }
            else if (spriteRenderer)
            {
                spriteRenderer.sprite = sprites.Length > 0 ? sprites[currentFrame % aniNum] : AssetManager.Instance.Load<Sprite>(atlasName, spriteNames[currentFrame % aniNum]);
            }
        }
    }
}