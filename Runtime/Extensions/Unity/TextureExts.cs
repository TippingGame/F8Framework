using UnityEngine;

namespace F8Framework.Core
{
    public static class TextureExts
    {
        public static Texture2D Clone(this Texture2D @this)
        {
            Texture2D newTex;
            newTex = new Texture2D(@this.width, @this.height);
            Color[] colors = @this.GetPixels(0, 0, @this.width, @this.height);
            newTex.SetPixels(colors);
            newTex.Apply();
            return newTex;
        }
        public static Texture2D ConvertToSprite(this Sprite @this)
        {
            var newTex = new Texture2D((int)@this.rect.width, (int)@this.rect.height);
            var pixels = @this.texture.GetPixels(
                (int)@this.textureRect.x,
                (int)@this.textureRect.y,
                (int)@this.textureRect.width,
                (int)@this.textureRect.height);
            newTex.SetPixels(pixels);
            newTex.Apply();
            return newTex;
        }
        /// <summary>
        /// 双线性插值法缩放图片，等比缩放 
        /// </summary>
        public static Texture2D ScaleTextureBilinear(this Texture2D @this, float scaleFactor)
        {
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(@this.width * scaleFactor), Mathf.CeilToInt(@this.height * scaleFactor));
            float scale = 1.0f / scaleFactor;
            int maxX = @this.width - 1;
            int maxY = @this.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x * scale;
                    float targetY = y * scale;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = @this.GetPixel(x1, y1);
                    Color color2 = @this.GetPixel(x2, y1);
                    Color color3 = @this.GetPixel(x1, y2);
                    Color color4 = @this.GetPixel(x2, y2);
                    Color color = new Color(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);

                }
            }
            newTexture.Apply();
            return newTexture;
        }

        /// <summary> 
        /// 双线性插值法缩放图片为指定尺寸 
        /// </summary>
        public static Texture2D SizeTextureBilinear(this Texture2D @this, Vector2 size)
        {
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));
            float scaleX = @this.width / size.x;
            float scaleY = @this.height / size.y;
            int maxX = @this.width - 1;
            int maxY = @this.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x * scaleX;
                    float targetY = y * scaleY;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = @this.GetPixel(x1, y1);
                    Color color2 = @this.GetPixel(x2, y1);
                    Color color3 = @this.GetPixel(x1, y2);
                    Color color4 = @this.GetPixel(x2, y2);
                    Color color = new Color(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);

                }
            }
            newTexture.Apply();
            return newTexture;
        }
        /// <summary> 
        /// Texture旋转
        /// </summary>
        public static Texture2D RotateTexture(this Texture2D @this, float eulerAngles)
        {
            int x;
            int y;
            int i;
            int j;
            float phi = eulerAngles / (180 / Mathf.PI);
            float sn = Mathf.Sin(phi);
            float cs = Mathf.Cos(phi);
            Color32[] arr = @this.GetPixels32();
            Color32[] arr2 = new Color32[arr.Length];
            int W = @this.width;
            int H = @this.height;
            int xc = W / 2;
            int yc = H / 2;

            for (j = 0; j < H; j++)
            {
                for (i = 0; i < W; i++)
                {
                    arr2[j * W + i] = new Color32(0, 0, 0, 0);

                    x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                    y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);

                    if ((x > -1) && (x < W) && (y > -1) && (y < H))
                    {
                        arr2[j * W + i] = arr[y * W + x];
                    }
                }
            }

            Texture2D newImg = new Texture2D(W, H);
            newImg.SetPixels32(arr2);
            newImg.Apply();

            return newImg;
        }
        /// <summary>
        ///转换texture为texture2d; 
        /// </summary>
        public static Texture2D ToTexture2D(this Texture @this)
        {
            return Texture2D.CreateExternalTexture(
                @this.width,
                @this.height,
                TextureFormat.RGB24,
                false, false,
                @this.GetNativeTexturePtr());
        }
    }
}