using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class Popup : SingletonMono<Popup>
    {
        public RectTransform popupTransform = null;
        public Text title = null;
        public Text message = null;

        public float minWidth = 200;
        public float maxWidth = 730;

        public float baseHeigt = 80;
        public float minHeight = 100;
        public float maxHeight = 620;

        public float margin = 18;

        protected override void Init()
        {
            Show(false);
        }

        public void ShowPopup(string message, string title = "")
        {
            this.message.text = message;
            this.title.text = title;
            Show(true);
        }

        public void OnClickCloseButton()
        {
            Show(false);
        }

        private void Show(bool show)
        {
            gameObject.SetActive(show);

            if (show == true)
            {
                float width = minWidth;
                float height = minHeight;

                if (width < this.title.preferredWidth + margin)
                {
                    width = this.title.preferredWidth + margin;
                }

                if (width < this.message.preferredWidth + margin)
                {
                    width = this.message.preferredWidth + margin;
                }

                if (width > maxWidth)
                {
                    width = maxWidth;
                }

                if (height < this.message.preferredHeight + margin + baseHeigt)
                {
                    height = this.message.preferredHeight + margin + baseHeigt;
                }

                if (height > maxHeight)
                {
                    height = maxHeight;
                }

                popupTransform.sizeDelta = new Vector2(width, height);
            }
        }
    }
}