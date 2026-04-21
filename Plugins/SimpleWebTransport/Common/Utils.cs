using System.Threading;

namespace JamesFrowen.SimpleWeb
{
    static class Utils
    {
        public static void CheckForInterupt()
        {
            // sleep in order to check for ThreadInterruptedException
            Thread.Sleep(1);
        }
    }
}
