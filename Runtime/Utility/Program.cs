using System.Diagnostics;

namespace F8Framework.Core
{
    public static partial class Util
    {
        //================================================
        // Program：是一个存在disk中且断电或重启不会消失可执行文件，存储在
        // 存储媒介中，以实体文件的形态存在
        //
        // Process：当program被执行之后，就变成了进程。执行者的权限和程序
        // 所需的资料都会载入到内存中，OS会给予这些内存单元一个pid。
        //
        //此工具函数若在非win平台调用，请检查是否安装 .NET Core !
        //
        // 如何在.NET Core上运行：
        //      https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        //================================================
        public static class Program
        {
            /// <summary>
            /// 通过cmd启动一个.netcore应用；
            /// </summary>
            /// <param name="workingDirectory">工作的文件夹地址，dll存放的文件夹路径</param>
            /// <param name="dllName">启动的dll名</param>
            /// <returns>运行的Process对象</returns>
            public static Process StartNetCoreProcess(string workingDirectory, string dllName)
            {
                var process = new ProcessStartInfo();
                process.UseShellExecute = true;
                process.WorkingDirectory = workingDirectory;
                process.Arguments = dllName;
                process.FileName = "dotnet.exe";
                return Process.Start(process);
            }

            /// <summary>
            /// 启动一个进程；
            /// </summary>
            /// <param name="filePath">文件地址</param>
            /// <returns>运行的Process对象</returns>
            public static Process StartProcess(string filePath)
            {
                return Process.Start(filePath);
            }

            /// <summary>
            /// 获取当前应用的物理内存使用情况；
            /// </summary>
            /// <returns>byte长度</returns>
            public static long GetMemoryUsage()
            {
                Process currentProcess = Process.GetCurrentProcess();
                return currentProcess.WorkingSet64;
            }
        }
    }
}