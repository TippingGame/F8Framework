using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class WaitUi : SingletonMono<WaitUi>
    {
        public GameObject bg = null;
        public GameObject icon = null;
        public Text message = null;

        protected override void Init()
        {
            ShowUi(false);
        }

        public void ShowUi(bool show)
        {
            if (show == true)
            {
                SetMessage(string.Empty);
            }

            bg.SetActive(show);
            icon.SetActive(show);
            message.gameObject.SetActive(show);
        }

        public void SetMessage(string message)
        {
            this.message.text = message;
        }
    }
}