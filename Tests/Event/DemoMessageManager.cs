using F8Framework.Core;
using UnityEngine;

public class DemoMessageManager : MonoBehaviour
{
    private object[] data = new object[] { 123123, "asdasd" };
    private void Awake()
    {
        MessageManager.Instance.AddEventListener(MessageEvent.ApplicationFocus,OnPlayerSpawned,this);
        MessageManager.Instance.AddEventListener(MessageEvent.NotApplicationFocus, OnPlayerSpawned2,this);
    }

    private void Start()
    {
        MessageManager.Instance.DispatchEvent(MessageEvent.ApplicationFocus);
        MessageManager.Instance.DispatchEvent(MessageEvent.NotApplicationFocus,data);
        //全局时需要执行RemoveEventListener
        MessageManager.Instance.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned,this);
        MessageManager.Instance.RemoveEventListener(MessageEvent.NotApplicationFocus, OnPlayerSpawned2,this);
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
