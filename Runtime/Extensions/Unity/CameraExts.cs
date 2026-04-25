using UnityEngine;

namespace F8Framework.Core
{
    public static class CameraExts
    {
        /// <summary>
        /// 通过相机截取屏幕并转换为Texture2D
        /// </summary>
        /// <param name="this">目标相机</param>
        /// <returns>相机抓取的屏幕Texture2D</returns>
        public static Texture2D CameraScreenshotAsTextureRGB(this Camera @this)
        {
            return CameraScreenshotAsTexture(@this, TextureFormat.RGB565);
        }
        public static Texture2D CameraScreenshotAsTextureRGBA(this Camera @this)
        {
            return CameraScreenshotAsTexture(@this, TextureFormat.RGBA32);
        }
        public static Texture2D CameraScreenshotAsTexture(this Camera @this, TextureFormat textureFormat)
        {
            var oldRenderTexture = @this.targetTexture;
            var width = @this.pixelWidth;
            var height = @this.pixelHeight;
            RenderTexture renderTexture;
            renderTexture = new RenderTexture(width, height, 24);
            @this.targetTexture = renderTexture;
            @this.Render();
            Texture2D texture2D = new Texture2D(width, height, textureFormat, false);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;
            @this.targetTexture = oldRenderTexture;
            return texture2D;
        }
        /// <summary>
        /// 通过相机截取屏幕并转换为Sprite
        /// </summary>
        /// <param name="this">目标相机</param>
        /// <returns>相机抓取的屏幕Texture2D</returns>
        public static Sprite CameraScreenshotAsSpriteRGBA(this Camera @this)
        {
            return CameraScreenshotAsTextureRGBA(@this).ConvertToSprite();
        }
        public static Sprite CameraScreenshotAsSpriteRGB(this Camera @this)
        {
            return CameraScreenshotAsTextureRGB(@this).ConvertToSprite();
        }
        public static Sprite CameraScreenshotAsSprite(this Camera @this, TextureFormat textureFormat)
        {
            return CameraScreenshotAsTexture(@this, textureFormat).ConvertToSprite();
        }
    }
}
