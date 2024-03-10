using System;

namespace F8Framework.Core
{
    public interface INetworkClientChannel : INetworkChannel
    {
        /// <summary>
        /// 建立连接回调；
        /// </summary>
        event Action OnConnected;
        /// <summary>
        /// 接收数据回调；
        /// </summary>
        event Action<byte[]> OnDataReceived;
        /// <summary>
        /// 断开连接回调；
        /// </summary>
        event Action OnDisconnected;
        /// <summary>
        /// client是否连接成功；
        /// </summary>
        bool IsConnect { get; }
        /// <summary>
        /// 与服务器连接
        /// </summary>
        /// <param name="host">地址</param>
        /// <param name="port">端口</param>
        void Connect(string host, int port);
        /// <summary>
        /// 发送数据，默认为可靠类型；
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>发送结果</returns>
        bool SendMessage(byte[] data);
    }
}
