using UnityEngine.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class Viewer : MonoBehaviour
    {
        public GameObject pannel = null;
        [FormerlySerializedAs("tabView")] public TabLogView tabLogView = null;
        [FormerlySerializedAs("consoleView")] public ConsoleLogView consoleLogView = null;
        [FormerlySerializedAs("functionView")] public FunctionLogView functionLogView = null;
        [FormerlySerializedAs("systemView")] public SystemLogView systemLogView = null;

        private CanvasGroup canvasGroup = null;
        private CanvasScaler canvasScaler = null;
        private ScreenOrientation orientation = ScreenOrientation.LandscapeLeft;
        private Vector2 portraitResolution = new Vector2(800.0f, 1280.0f);
        private Vector2 landscapeResolution = new Vector2(1200.0f, 800.0f);
        private int screenWidth = 0;
        private int screenHeight = 0;

        private bool gestureEnable = false;
        private bool isTouchBegin = false;
        private float touchTime = 0f;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasScaler = GetComponent<CanvasScaler>();

            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        public void Initialize()
        {
            tabLogView.InitializeView();
            consoleLogView.InitializeView();
            functionLogView.InitializeView();
            systemLogView.InitializeView();

            SelectConsole();

            Show(false);

            Log.Instance.AddLogNotificationCallback(OnLogNotification);
        }

        public void Close()
        {
            tabLogView.CloseView();
            consoleLogView.CloseView();
            functionLogView.CloseView();
            systemLogView.CloseView();

            Show(false);
        }

        public void SelectConsole()
        {
            tabLogView.SelectTab(TabLogView.TabIndex.CONSOLE);
            consoleLogView.gameObject.SetActive(true);
            functionLogView.gameObject.SetActive(false);
            systemLogView.gameObject.SetActive(false);
        }

        public void SelectFunction()
        {
            tabLogView.SelectTab(TabLogView.TabIndex.FUNCTION);
            consoleLogView.gameObject.SetActive(false);
            functionLogView.gameObject.SetActive(true);
            systemLogView.gameObject.SetActive(false);
        }

        public void SelectSystem()
        {
            tabLogView.SelectTab(TabLogView.TabIndex.SYSTEM);
            consoleLogView.gameObject.SetActive(false);
            functionLogView.gameObject.SetActive(false);
            systemLogView.gameObject.SetActive(true);

            systemLogView.Refresh();
        }

        public void SetGestureEnable(bool enable)
        {
            gestureEnable = enable;
        }

        private void Update()
        {
            if (pannel.activeSelf == false && gestureEnable == true)
            {
                CheckGesture(true);
                CheckKey(true);
            }
            else
            {
                UpdateOrientation();
                UpdateResolution();
                CheckKey(false);
                CheckGesture(false);
            }
        }

        private void UpdateResolution()
        {
            int currentWidth = Screen.width;
            int currentHeight = Screen.height;

            if (screenWidth != currentWidth || screenHeight != currentHeight)
            {
                screenWidth = currentWidth;
                screenHeight = currentHeight;

                consoleLogView.UpdateResolution();
            }
        }

        private void UpdateOrientation()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            ScreenOrientation newOrientation = Screen.orientation;
#else
            ScreenOrientation newOrientation = Screen.width >= Screen.height
                ? ScreenOrientation.LandscapeLeft
                : ScreenOrientation.Portrait;
#endif

            if (orientation != newOrientation)
            {
                orientation = newOrientation;

                if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
                {
                    canvasScaler.referenceResolution = portraitResolution;
                }
                else
                {
                    canvasScaler.referenceResolution = landscapeResolution;
                }

                consoleLogView.SetOrientation(orientation);
                systemLogView.SetOrientation(orientation);
            }
        }

        private void CheckGesture(bool show)
        {
            if (Input.touchCount == ViewerConst.GESTURE_TOUCH_COUNT)
            {
                if (isTouchBegin == false)
                {
                    isTouchBegin = true;
                    touchTime = Time.unscaledTime;
                }
                else
                {
                    if (Time.unscaledTime - touchTime >= ViewerConst.GESTURE_TOUCH_TIME_INTERVAL)
                    {
                        isTouchBegin = false;
                        touchTime = 0;
                        Show(show);
                    }
                }
            }
            else
            {
                isTouchBegin = false;
                touchTime = 0;
            }
        }

        private void CheckKey(bool show)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote) == true)
            {
                Show(show);
            }
        }

        public void Show()
        {
            pannel.SetActive(true);
        }

        private void Show(bool show)
        {
            pannel.SetActive(show);
        }

        public void OnAlphaValueChanged(float value)
        {
            canvasGroup.alpha = value;
        }

        private void OnLogNotification(Log.LogData data)
        {
            if (data.logType == LogType.Exception || data.logType == LogType.Assert)
            {
                Show(true);
                SelectConsole();
                consoleLogView.ListMoveToBottom();
            }
        }
    }
}