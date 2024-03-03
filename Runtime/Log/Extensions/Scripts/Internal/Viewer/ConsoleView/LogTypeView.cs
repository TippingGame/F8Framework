using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class LogTypeView : MonoBehaviour
    {
        public Image logTypeBg = null;
        public Image warningTypeBg = null;
        public Image errorTypeBg = null;
        public Text logCount = null;
        public Text warningCount = null;
        public Text errorCount = null;

        private Color logTypeEnableColor = new Color(0.3f, 0.3f, 0.3f);
        private Color logTypeDisableColor = new Color(0.172f, 0.172f, 0.172f);

        public void SetLogCount(LogType type, int count)
        {
            Text typeText = null;
            switch (type)
            {
                case LogType.Log:
                {
                    typeText = logCount;
                    break;
                }
                case LogType.Warning:
                {
                    typeText = warningCount;
                    break;
                }
                case LogType.Error:
                {
                    typeText = errorCount;
                    break;
                }
            }

            if (typeText != null)
            {
                if (count >= LogConst.MAX_SHOW_LOG_COUNT)
                {
                    typeText.text = string.Format(LogConst.MAX_SHOW_LOG_COUNT_FORMAT, LogConst.MAX_SHOW_LOG_COUNT);
                }
                else
                {
                    typeText.text = count.ToString();
                }
            }
        }

        public void SetTypeEnable(LogType type, bool enable)
        {
            Image typeImage = null;
            switch (type)
            {
                case LogType.Log:
                {
                    typeImage = logTypeBg;
                    break;
                }
                case LogType.Warning:
                {
                    typeImage = warningTypeBg;
                    break;
                }
                case LogType.Error:
                {
                    typeImage = errorTypeBg;
                    break;
                }
            }

            if (typeImage != null)
            {
                SetLogTypeBgAlpha(typeImage, enable == true ? logTypeEnableColor : logTypeDisableColor);
            }
        }

        private void SetLogTypeBgAlpha(Image image, Color color)
        {
            image.color = color;
        }
    }
}