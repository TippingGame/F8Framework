using System.Collections;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class LogSample : MonoBehaviour
    {
        private void Start()
        {
            Function.Instance.AddCommand(this, "TestLog", new object[] { 2 });
            Function.Instance.AddCommand(this, "TestCommand", new object[] { "by command text" });
            Function.Instance.AddCommand(this, "TestCategory", new object[] { "category1", "category2" });
            Function.Instance.AddCommand(this, "SendExceptionLog");
            Function.Instance.AddCommand(this, "SendAssertLog");
            Function.Instance.AddCommand(this, "SendErrorLog");
            Function.Instance.AddCommand(this, "SendWarningLog");
            Function.Instance.AddCheatKeyCallback((cheatKey) =>
            {
                LogF8.Log("Call cheat key callback with : " + cheatKey);
            });

            StartCoroutine(SendLog());
        }

        private IEnumerator SendLog()
        {
            int count = 0;
            while (true)
            {
                yield return new WaitForSeconds(5.0f);

                LogF8.Log("***** Sample Send Log : " + count++.ToString());
            }
        }

        public void SendThreadLog()
        {
            for (int index = 0; index < 10; index++)
            {
                System.Threading.Thread t =
                    new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadProc));
                t.Start(index);
            }
        }

        public static void ThreadProc(object index)
        {
            throw new System.Exception("thread : " + index);
        }

        private void TestLog(int index)
        {
            LogF8.Log("*TestLog() : " + index.ToString());
            LogF8.Log("category1 log");

            LogF8.Log(LogViewer.Instance.MakeLogWithCategory("Test message with category", "TestCategory"));
            LogF8.Log("$(category)TempCategory$(category");
            LogF8.Log("$(category)TempCategory$(category)");
            LogF8.Log("$(category)TempCategory$(");
            LogF8.Log("$(category)TempCategory$(category) Test");

            LogF8.Log(SystemInformation.Instance.ToString());
        }

        private void TestCommand(string text)
        {
            LogF8.Log("TestCommand : " + text);
        }

        private void TestCategory(string category1, string category2)
        {
            LogF8.Log(LogViewer.Instance.MakeLogWithCategory("Log with category(" + category1 + ")", category1));
            LogF8.Log(LogViewer.Instance.MakeLogWithCategory("Log with category(" + category2 + ")", category2));
        }

        public void SendExceptionLog()
        {
            LogF8.LogException(new System.Exception("Exception log"));
        }

        private void SendAssertLog()
        {
            LogF8.LogAssertion("Assert log");
        }

        private void SendErrorLog()
        {
            LogF8.LogError("Error log");
        }

        private void SendWarningLog()
        {
            LogF8.LogWarning("Warning log");
        }
    }
}