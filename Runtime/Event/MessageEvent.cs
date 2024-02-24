namespace F8Framework.Core
{
    public enum MessageEvent
    {
        // 框架事件，10000起步
        Empty = 10000,
        ApplicationFocus = 10001, // 游戏对焦
        NotApplicationFocus = 10002, // 游戏失焦
        ApplicationQuit = 10003, // 游戏退出
    }
}

