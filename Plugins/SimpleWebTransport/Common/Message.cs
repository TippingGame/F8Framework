using System;

namespace JamesFrowen.SimpleWeb
{
    public struct Message
    {
        public readonly IConnection conn;
        public readonly EventType type;
        public readonly ArrayBuffer data;
        public readonly Exception exception;

        public int connId => conn?.Id ?? -1;

        public Message(EventType type) : this()
        {
            this.type = type;
        }

        public Message(ArrayBuffer data) : this()
        {
            type = EventType.Data;
            this.data = data;
        }

        public Message(Exception exception) : this()
        {
            type = EventType.Error;
            this.exception = exception;
        }

        public Message(IConnection conn, EventType type) : this()
        {
            this.conn = conn;
            this.type = type;
        }

        public Message(IConnection conn, ArrayBuffer data) : this()
        {
            this.conn = conn;
            type = EventType.Data;
            this.data = data;
        }

        public Message(IConnection conn, Exception exception) : this()
        {
            this.conn = conn;
            type = EventType.Error;
            this.exception = exception;
        }
    }
}
