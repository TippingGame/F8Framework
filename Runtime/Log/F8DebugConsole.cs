using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class F8DebugConsole : MonoBehaviour
    {
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        /// <summary>
        /// The hotkey to show and hide the console window.
        /// </summary>
        public KeyCode toggleKey = KeyCode.BackQuote;

        List<Log> logs = new List<Log>();
        Vector2 scrollPosition;
        bool show;
        bool collapse;

        // Visual elements:

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

        const int margin = 20;

        Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
        Rect titleBarRect = new Rect(0, 0, 10000, 20);
        GUIContent clearLabel = new GUIContent("ClearTempData", "ClearTempData the contents of the console.");
        GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

        void OnEnable()
        {
            Application.logMessageReceived += (HandleLog);
        }

        void OnDisable()
        {
            Application.logMessageReceived -= (HandleLog);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                show = !show;
            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            GUI.skin.button.fontSize = (int)(24 * Screen.height / 1280f);
            GUI.skin.label.fontSize = (int)(24 * Screen.height / 1280f);
            GUI.skin.toggle.fontSize = (int)(24 * Screen.height / 1280f);
            GUI.skin.window.fontSize = (int)(24 * Screen.height / 1280f);

            if (GUI.Button(
                    new Rect(Screen.width / 2f - 150 * Screen.height / 1280f, 0, 300 * Screen.height / 1280f,
                        100 * Screen.height / 1280f), "Console"))
            {
                show = !show;
            }

            if (show)
            {
                windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// A window that displayss the recorded logs.
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        void ConsoleWindow(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Iterate through the recorded logs.
            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];

                // Combine identical messages if collapse option is chosen.
                if (collapse)
                {
                    var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                    if (messageSameAsPrevious)
                    {
                        continue;
                    }
                }


                GUI.contentColor = logTypeColors[log.type];
                if (log.type == LogType.Exception)
                {
                    GUILayout.Label(log.message + "\n<i><color=while>" + log.stackTrace + "</color></i>");
                }
                else
                {
                    GUILayout.Label(log.message);
                }
            }

            GUILayout.EndScrollView();

            GUI.contentColor = Color.white;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
            }

            if (GUILayout.Button(collapseLabel))
            {
                collapse = !collapse;
            }

            GUILayout.EndHorizontal();

            // Allow the window to be dragged by its title bar.
            //GUI.DragWindow(titleBarRect);
        }

        /// <summary>
        /// Records a log from the log callback.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Trace of where the message came from.</param>
        /// <param name="type">Type of message (error, exception, warning, assert).</param>
        void HandleLog(string message, string stackTrace, LogType type)
        {
            logs.Add(new Log()
            {
                message = message,
                stackTrace = stackTrace,
                type = type,
            });
        }
    }
}