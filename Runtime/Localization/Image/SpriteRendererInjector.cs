using UnityEngine;

namespace F8Framework.Core
{
    public class SpriteRendererInjector : IInjector
    {
        readonly string localizedTextID;
        readonly SpriteRenderer spriteRenderer;
        readonly Sprite[] sprites;

        public SpriteRendererInjector(SpriteRenderer spriteRenderer, string localizedTextID, Sprite[] sprites)
        {
            this.localizedTextID = localizedTextID;
            this.spriteRenderer = spriteRenderer;
            this.sprites = sprites;
        }

        public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
        {
            if (localizedData is int index)
            {
                if (localizedTextID.IsNullOrEmpty())
                {
                    spriteRenderer.sprite = sprites[index];
                }
                else
                {
                    string textIDValue = Localization.Instance.GetTextFromId(localizedTextID);
                    AssetManager.Instance.LoadAsync(textIDValue, (asset) =>
                    {
                        if (asset is Texture2D texture)
                        {
                            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            spriteRenderer.sprite = sprite;
                            LogF8.LogAsset("本地化图片类型错误，已自动转换：" + asset);
                            return;
                        }
                        spriteRenderer.sprite = asset as Sprite;
                    });
                }
            }
        }
    }
}

