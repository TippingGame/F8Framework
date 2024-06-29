using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class LogViewer : SingletonMono<LogViewer>
    {
        [Header("5指长按启用")] public bool gestureEnable = true;

        [Space(5)] [Header("发送邮件")]
        public MailData mailSetting = null;
        private Viewer viewer = null;

        protected override void Init()
        {
            Initialize();
        }

        public override void OnQuitGame()
        {
            Clear();
        }

        public void AddCheatKeyCallback(Action<string> callback)
        {
            Function.Instance.AddCheatKeyCallback(callback);
        }

        public void AddCommand(object instance, string methodName)
        {
            Function.Instance.AddCommand(instance, methodName);
        }

        public void Show()
        {
            viewer.Show();
        }

        public string MakeLogWithCategory(string message, string category)
        {
            return Log.Instance.MakeLogMessageWithCategory(message, category);
        }

        private void Initialize()
        {
            Function.Instance.Initialize();

            SetMailData();

            if (viewer == null)
            {
                viewer = transform.GetChild(0).GetComponent<Viewer>();
            }

            viewer.Initialize();
            SetGestureEnable();
        }

        private void Clear()
        {
            Function.Instance.Clear();
        }

        private void SetGestureEnable()
        {
            viewer.SetGestureEnable(gestureEnable);
        }

        private void SetMailData()
        {
            Function.Instance.SetMailData(mailSetting);
        }
    }
}