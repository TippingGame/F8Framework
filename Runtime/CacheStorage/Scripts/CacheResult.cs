using System.Text;

namespace F8Framework.Core
{
    public struct CacheResult
    {
        public CacheInfo Info { get; private set; }

        public byte[] Data { get; private set; }

        public string Text
        {
            get { return GetTextData(); }
        }

        public bool UpdateData { get; private set; }

        public CacheResult(CacheInfo info = null, byte[] data = null, bool updateData = true)
        {
            this.Info = info;
            this.Data = data;
            this.UpdateData = updateData;
        }

        public bool IsSuccess()
        {
            return (Data != null);
        }

        public string GetTextData()
        {
            return GetTextData(Encoding.UTF8);
        }

        public string GetTextData(Encoding encoding)
        {
            return encoding.GetString(Data);
        }

        public T GetJsonData<T>()
        {
            return GetJsonData<T>(Encoding.UTF8);
        }

        public T GetJsonData<T>(Encoding encoding)
        {
            string text = GetTextData(encoding);

            return Util.LitJson.ToObject<T>(text);
        }
    }
}