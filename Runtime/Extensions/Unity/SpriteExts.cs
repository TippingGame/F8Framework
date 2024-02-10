using UnityEngine;
namespace F8Framework.Core
{
    public static class SpriteExts
    {
        public static Sprite ConvertToSprite(this Texture2D @this)
        {
            Sprite sprite = Sprite.Create(@this, new Rect(0, 0, @this.width, @this.height), Vector2.zero);
            return sprite;
        }
    }
}
