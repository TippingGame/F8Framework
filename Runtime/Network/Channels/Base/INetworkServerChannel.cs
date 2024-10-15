using System;

namespace F8Framework.Core
{
    public interface INetworkServerChannel: INetworkChannel
    {
        /// <summary>
        /// 建立连接回调；
        /// </summary>
        event Action<int, string> OnConnected;
        /// <summary>
        /// 断开连接回调；
        /// </summary>
        event Action<int> OnDisconnected;
        /// <summary>
        /// 接收数据回调；
        /// </summary>
        event Action<int, byte[]> OnDataReceived;
        /// <summary>
        /// server是否处于活动状态；
        /// </summary>
        bool Active { get; }
        /// <summary>
        /// 启动服务器；
        /// </summary>
        /// <returns>启动是否成功</returns>
        bool Start();
        /// <summary>
        /// 发送数据到remote;
        /// 默认为可靠类型；
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="connectionId">连接Id</param>
        bool SendMessage(int connectionId,byte[] data);
        /// <summary>
        /// 与连接Id断开连接
        /// </summary>
        /// <param name="connectionId">连接Id</param>
        /// <returns>断开结果</returns>
        bool Disconnect(int connectionId);
        /// <summary>
        /// 获取连接Id的地址；
        /// </summary>
        /// <param name="connectionId">连接Id</param>
        /// <returns>连接Id的地址</returns>
        string GetConnectionAddress(int connectionId);
    }
}
