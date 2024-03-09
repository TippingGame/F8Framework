namespace F8Framework.Core.Editor
{
    [System.Serializable]
    public class PreferenceEntry
    {
        public enum PrefTypes
        {
            String = 0,
            Int = 1,
            Float = 2
        }

        public PrefTypes m_typeSelection;
        public string m_key;

        // Need diffrend ones for auto type selection of serilizedProerty
        public string m_strValue;
        public int m_intValue;
        public float m_floatValue;

        public string ValueAsString()
        {
            switch(m_typeSelection)
            {
                case PrefTypes.String:
                    return m_strValue;
                case PrefTypes.Int:
                    return m_intValue.ToString();
                case PrefTypes.Float:
                    return m_floatValue.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}