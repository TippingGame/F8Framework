using UnityEngine;

namespace F8Framework.Core
{
    public class SpriteRendererInjector : AssetInjectorBase
    {
        readonly SpriteRenderer spriteRenderer;
        readonly Sprite[] sprites;

        public SpriteRendererInjector(SpriteRenderer spriteRenderer, Sprite[] sprites)
        {
            this.spriteRenderer = spriteRenderer;
            this.sprites = sprites;
        }

        public override void Inject<T1, T2>(T1 localizedData, T2 localizer)
        {
            if (!spriteRenderer)
            {
                Unload();
                return;
            }

            if (localizedData is null)
            {
                Unload();
                return;
            }

            if (localizedData is int index)
            {
                UseDirectAsset();
                spriteRenderer.sprite = GetSprite(index);
            }
            else if (localizedData is string textIDValue)
            {
                LoadLocalizedAsset<Sprite>(textIDValue, (asset) =>
                {
                    if (spriteRenderer)
                    {
                        spriteRenderer.sprite = asset;
                        return;
                    }

                    Unload();
                });
            }
        }

        private Sprite GetSprite(int index)
        {
            return sprites != null && index >= 0 && index < sprites.Length ? sprites[index] : null;
        }

        protected override void ClearTarget()
        {
            if (spriteRenderer)
            {
                spriteRenderer.sprite = null;
            }
        }
    }
}

