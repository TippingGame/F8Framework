using System;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace JamesFrowen.SimpleWeb
{
    static class SimpleWebJSLib
    {
#if UNITY_WEBGL
        [DllImport("__Internal", EntryPoint = "SimpleWeb_IsConnected")]
        internal static extern bool IsConnected(int index);

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("__Internal", EntryPoint = "SimpleWeb_Connect")]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        internal static extern int Connect(string address,
            Action<int> openCallback,
            Action<int> closeCallBack,
            Action<int, IntPtr, int> messageCallback,
            Action<int> errorCallback,
            IntPtr incomingDataBuffer,
            int incomingDataBufferLength
            );

        [DllImport("__Internal", EntryPoint = "SimpleWeb_Disconnect")]
        internal static extern void Disconnect(int index);

        [DllImport("__Internal", EntryPoint = "SimpleWeb_Send")]
        internal static extern bool Send(int index, IntPtr ptr, int length);
#else
        internal static bool IsConnected(int index) => throw new NotSupportedException();

        internal static int Connect(string address,
            Action<int> openCallback,
            Action<int> closeCallBack,
            Action<int, IntPtr, int> messageCallback,
            Action<int> errorCallback,
            IntPtr incomingDataBuffer,
            int incomingDataBufferLength
            )
            => throw new NotSupportedException();

        internal static void Disconnect(int index) => throw new NotSupportedException();

        internal static bool Send(int index, IntPtr ptr, int length) => throw new NotSupportedException();
#endif

        internal static unsafe bool Send(int index, byte[] array, int offset, int length)
        {
            // just convert to span here, so we dont need to do pointer math
            return Send(index, new ReadOnlySpan<byte>(array, offset, length));
        }
        internal static unsafe bool Send(int index, ReadOnlySpan<byte> span)
        {
            fixed (byte* ptr = span)
            {
                return Send(index, new IntPtr(ptr), span.Length);
            }
        }
    }
}
