namespace F8Framework.Core
{
    public enum MessageEvent
    {
        // 框架事件，10000起步
        Empty = 10000,
        ApplicationFocus = 10001, // 游戏对焦
        NotApplicationFocus = 10002, // 游戏失焦
        ApplicationQuit = 10003, // 游戏退出
        // SDK回调信息
        SDKOnInitSuccess = 10004,
        SDKOnInitFail = 10005,
        SDKOnLoginSuccess = 10006,
        SDKOnLoginFail = 10007,
        SDKOnSwitchAccountSuccess = 10008,
        SDKOnLogoutSuccess = 10009,
        SDKOnPaySuccess = 10010,
        SDKOnPayFail = 10011,
        SDKOnPayCancel = 10012,
        SDKOnExitSuccess = 10013,
    }
}

