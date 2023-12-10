namespace F8Framework.Core
{
    public class Singleton<T> where T : new()
    {
        private static T m_instance;
        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (locker)
                    {
                        if (m_instance == null) m_instance = new T();
                    }
                }

                return m_instance;
            }
        }
    }
}
