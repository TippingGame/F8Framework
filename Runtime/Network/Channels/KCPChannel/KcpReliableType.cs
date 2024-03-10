namespace F8Framework.Core
{
    /// <summary>
    /// 网络消息发送类型；
    /// </summary>
    public enum KcpReliableType:byte
    {
        Reliable = 0x01,
        Unreliable = 0x02
    }
}
