using UnityEngine;

namespace F8Framework.Core
{
    public class SpriteRendererInjector : IInjector
    {
        readonly SpriteRenderer spriteRenderer;
        readonly Sprite[] sprites;

        public SpriteRendererInjector(SpriteRenderer spriteRenderer, Sprite[] sprites)
        {
            this.spriteRenderer = spriteRenderer;
            this.sprites = sprites;
        }

        public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
        {
            if (localizedData is int index)
            {
                spriteRenderer.sprite = sprites[index];
            }
        }
    }
}

