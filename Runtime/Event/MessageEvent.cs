using System;

namespace F8Framework.Core
{
    public enum MessageEvent : Int16
    {
        //框架事件，10000起步
        Empty = 10000,
        ApplicationFocus,
        NotApplicationFocus,
        ApplicationQuit,
    }
}

