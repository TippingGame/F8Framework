using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoMessageManager : MonoBehaviour
    {
        private void Awake()
        {
            FF8.Message.AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            FF8.Message.AddEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);
            FF8.Message.AddEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnPlayerSpawnedNoGC, this);
            FF8.Message.AddEventListener<int, string, bool, float, long, byte, char>(10004, OnPlayerSpawnedT7, this);
        }

        private void Start()
        {
            FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus);
            FF8.Message.DispatchEvent(10002, 123123, "asdasd");
            FF8.Message.DispatchEvent(MessageEvent.ApplicationFocus, 123123, "asdasd");
            FF8.Message.DispatchEvent(10004, 123123, "asdasd", true, 1.5f, 999L, (byte)7, 'F');
            //全局时需要执行RemoveEventListener
            FF8.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            FF8.Message.RemoveEventListener<int, string>(10002, OnPlayerSpawnedNoGC, this);
            FF8.Message.RemoveEventListener<MessageEvent, int, string>(MessageEvent.ApplicationFocus, OnPlayerSpawnedNoGC, this);
            FF8.Message.RemoveEventListener<int, string, bool, float, long, byte, char>(10004, OnPlayerSpawnedT7, this);
        }

        private void OnPlayerSpawned()
        {
            LogF8.Log("OnPlayerSpawned");
        }

        private void OnPlayerSpawnedNoGC(int id, string name)
        {
            LogF8.Log("OnPlayerSpawnedNoGC");
            LogF8.Log(id);
            LogF8.Log(name);
        }

        private void OnPlayerSpawnedT7(int id, string name, bool active, float speed, long score, byte level, char rank)
        {
            LogF8.Log("OnPlayerSpawnedT7");
            LogF8.Log(id);
            LogF8.Log(name);
            LogF8.Log(active);
            LogF8.Log(speed);
            LogF8.Log(score);
            LogF8.Log(level);
            LogF8.Log(rank);
        }
    }
}
