using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class LogItemData : InfiniteScrollData
    {
        public Log.LogData logData = null;
        public int index = 0;
        public bool showPlayTime = true;
        public bool showSceneName = true;
        public bool isSelect = false;
    }


    public class LogItem : InfiniteScrollItem
    {
        public Image bg = null;
        public Texture[] logType = null;
        public RawImage icon = null;
        public Text localTime = null;
        public Text message = null;
        public Text playTime = null;
        public Text sceneName = null;
        public GameObject scene = null;


        public int playTimeCollumSize = 58;

        public RectTransform sceneCullum;
        public int sceneColumnSize = 137;

        private Color bgSelectColor = new Color(0.243f, 0.372f, 0.588f);
        private Color bgNormalColor1 = new Color(0.215f, 0.215f, 0.215f);
        private Color bgNormalColor2 = new Color(0.235f, 0.235f, 0.235f);
        private Color bgNormalColor;

        public void ShowPlayTime(bool show)
        {
            playTime.gameObject.SetActive(show);

            ResetColumSize();
        }

        public void ShowSceneName(bool show)
        {
            scene.SetActive(show);

            ResetColumSize();
        }

        private void ResetColumSize()
        {
            float right = 0;
            if (scene.activeSelf == true)
            {
                Vector2 size = sceneCullum.sizeDelta;
                size.x = sceneColumnSize;
                sceneCullum.sizeDelta = size;


                right += sceneColumnSize;
            }

            if (playTime.gameObject.activeSelf == true)
            {
                Vector2 pos = playTime.rectTransform.anchoredPosition;
                pos.x = -right;
                playTime.rectTransform.anchoredPosition = pos;

                Vector2 size = playTime.rectTransform.sizeDelta;
                size.x = playTimeCollumSize;
                playTime.rectTransform.sizeDelta = size;

                right += playTimeCollumSize;
            }


            Vector2 offsetMax = message.rectTransform.offsetMax;
            offsetMax.x = -right;
            message.rectTransform.offsetMax = offsetMax;
        }

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            LogItemData itemData = (LogItemData)scrollData;
            Log.LogData logData = itemData.logData;

            SetIcon(logData.logType);

            localTime.text = string.Format("[{0}]", logData.localTime.ToString("HH:mm:ss"));
            message.text = logData.message.Replace(" ", "\u00A0");
            playTime.text = logData.playTime;
            sceneName.text = logData.sceneName;

            if (itemData.index % 2 == 0)
            {
                bgNormalColor = bgNormalColor1;
            }
            else
            {
                bgNormalColor = bgNormalColor2;
            }

            SetStatusNormal();
            ShowPlayTime(itemData.showPlayTime);
            ShowSceneName(itemData.showSceneName);
        }

        private void SetIcon(LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                {
                    icon.texture = logType[0];
                    break;
                }
                case LogType.Warning:
                {
                    icon.texture = logType[1];
                    break;
                }
                default:
                {
                    icon.texture = logType[2];
                    break;
                }
            }
        }

        public void OnTouch()
        {
            bg.color = bgSelectColor;

            OnSelect();
        }

        public void SetStatusNormal()
        {
            bg.color = bgNormalColor;
        }
    }
}