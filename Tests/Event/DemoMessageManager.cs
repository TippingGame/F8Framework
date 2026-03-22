using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoMessageManager : MonoBehaviour
    {
        private object[] data = new object[] { 123123, "asdasd" };

        private void Awake()
        {
            FF8.Message.AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            FF8.Message.AddEventListener(10001, OnPlayerSpawned2, this);
            FF8.Message.AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);
        }

        private void Start()
        {
            FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus);
            FF8.Message.DispatchEvent(10001, data);
            FF8.Message.DispatchEvent(10002, 123123, "asdasd");
            //全局时需要执行RemoveEventListener
            FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            FF8.Message.RemoveEventListener(10001, OnPlayerSpawned2, this);
            FF8.Message.RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);
        }

        private void OnPlayerSpawned()
        {
            LogF8.Log("OnPlayerSpawned");
        }

        private void OnPlayerSpawned2(params object[] obj)
        {
            LogF8.Log("OnPlayerSpawned2");
            if (obj is { Length: > 0 })
            {
                LogF8.Log(obj[0]);
                LogF8.Log(obj[1]);
            }
        }

        private void OnPlayerSpawnedNoGC(int id, string name)
        {
            LogF8.Log("OnPlayerSpawnedNoGC");
            LogF8.Log(id);
            LogF8.Log(name);
        }
    }
}
