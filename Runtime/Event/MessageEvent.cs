using System;

namespace F8Framework.Core
{
    public enum MessageEvent
    {
        //框架事件，10000起步
        Empty = 10000,
        ApplicationFocus,
        NotApplicationFocus,
        ApplicationQuit,
    }
}

