using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoMessageManager : MonoBehaviour
    {
        private object[] data = new object[] { 123123, "asdasd" };

        private void Awake()
        {
            FF8.Message.AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            FF8.Message.AddEventListener(MessageEvent.NotApplicationFocus, OnPlayerSpawned2, this);
        }

        private void Start()
        {
            FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus);
            FF8.Message.DispatchEvent(MessageEvent.NotApplicationFocus, data);
            //全局时需要执行RemoveEventListener
            FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            FF8.Message.RemoveEventListener(MessageEvent.NotApplicationFocus, OnPlayerSpawned2, this);
        }

        private void OnPlayerSpawned()
        {
            LogF8.Log("OnPlayerSpawned");
        }

        private void OnPlayerSpawned2(params object[] obj)
        {
            LogF8.Log("OnPlayerSpawned2");
            LogF8.Log(obj[0]);
            LogF8.Log(obj[1]);
        }
    }
}
