using System;
using UnityEngine;

namespace F8Framework.Core
{
    [Serializable]
    public class Language : IEquatable<Language>
    {
        public static Language[] BuiltinLanguages
        {
            get
            {
                return new[]
                {
                    Afrikaans,
                    Arabic,
                    Basque,
                    Belarusian,
                    Bulgarian,
                    Catalan,
                    Chinese,
                    Czech,
                    Danish,
                    Dutch,
                    English,
                    Estonian,
                    Faroese,
                    Finnish,
                    French,
                    German,
                    Greek,
                    Hebrew,
                    Hungarian,
                    Icelandic,
                    Indonesian,
                    Italian,
                    Japanese,
                    Korean,
                    Latvian,
                    Lithuanian,
                    Norwegian,
                    Polish,
                    Portuguese,
                    Romanian,
                    Russian,
                    SerboCroatian,
                    Slovak,
                    Slovenian,
                    Spanish,
                    Swedish,
                    Thai,
                    Turkish,
                    Ukrainian,
                    Vietnamese,
                    ChineseSimplified,
                    ChineseTraditional,
                    Unknown
                };
            }
        }

        public static Language Afrikaans
        {
            get { return new Language(SystemLanguage.Afrikaans.ToString(), "af", false); }
        }

        public static Language Arabic
        {
            get { return new Language(SystemLanguage.Arabic.ToString(), "ar", false); }
        }

        public static Language Basque
        {
            get { return new Language(SystemLanguage.Basque.ToString(), "eu", false); }
        }

        public static Language Belarusian
        {
            get { return new Language(SystemLanguage.Belarusian.ToString(), "be", false); }
        }

        public static Language Bulgarian
        {
            get { return new Language(SystemLanguage.Bulgarian.ToString(), "bg", false); }
        }

        public static Language Catalan
        {
            get { return new Language(SystemLanguage.Catalan.ToString(), "ca", false); }
        }

        public static Language Chinese
        {
            get { return new Language(SystemLanguage.Chinese.ToString(), "zh", false); }
        }

        public static Language Czech
        {
            get { return new Language(SystemLanguage.Czech.ToString(), "cs", false); }
        }

        public static Language Danish
        {
            get { return new Language(SystemLanguage.Danish.ToString(), "da", false); }
        }

        public static Language Dutch
        {
            get { return new Language(SystemLanguage.Dutch.ToString(), "nl", false); }
        }

        public static Language English
        {
            get { return new Language(SystemLanguage.English.ToString(), "en", false); }
        }

        public static Language Estonian
        {
            get { return new Language(SystemLanguage.Estonian.ToString(), "et", false); }
        }

        public static Language Faroese
        {
            get { return new Language(SystemLanguage.Faroese.ToString(), "fo", false); }
        }

        public static Language Finnish
        {
            get { return new Language(SystemLanguage.Finnish.ToString(), "fi", false); }
        }

        public static Language French
        {
            get { return new Language(SystemLanguage.French.ToString(), "fr", false); }
        }

        public static Language German
        {
            get { return new Language(SystemLanguage.German.ToString(), "de", false); }
        }

        public static Language Greek
        {
            get { return new Language(SystemLanguage.Greek.ToString(), "el", false); }
        }

        public static Language Hebrew
        {
            get { return new Language(SystemLanguage.Hebrew.ToString(), "he", false); }
        }

        public static Language Hungarian
        {
            get { return new Language(SystemLanguage.Hungarian.ToString(), "hu", false); }
        }

        public static Language Icelandic
        {
            get { return new Language(SystemLanguage.Icelandic.ToString(), "is", false); }
        }

        public static Language Indonesian
        {
            get { return new Language(SystemLanguage.Indonesian.ToString(), "id", false); }
        }

        public static Language Italian
        {
            get { return new Language(SystemLanguage.Italian.ToString(), "it", false); }
        }

        public static Language Japanese
        {
            get { return new Language(SystemLanguage.Japanese.ToString(), "ja", false); }
        }

        public static Language Korean
        {
            get { return new Language(SystemLanguage.Korean.ToString(), "ko", false); }
        }

        public static Language Latvian
        {
            get { return new Language(SystemLanguage.Latvian.ToString(), "lv", false); }
        }

        public static Language Lithuanian
        {
            get { return new Language(SystemLanguage.Lithuanian.ToString(), "lt", false); }
        }

        public static Language Norwegian
        {
            get { return new Language(SystemLanguage.Norwegian.ToString(), "no", false); }
        }

        public static Language Polish
        {
            get { return new Language(SystemLanguage.Polish.ToString(), "pl", false); }
        }

        public static Language Portuguese
        {
            get { return new Language(SystemLanguage.Portuguese.ToString(), "pt", false); }
        }

        public static Language Romanian
        {
            get { return new Language(SystemLanguage.Romanian.ToString(), "ro", false); }
        }

        public static Language Russian
        {
            get { return new Language(SystemLanguage.Russian.ToString(), "ru", false); }
        }

        public static Language SerboCroatian
        {
            get { return new Language(SystemLanguage.SerboCroatian.ToString(), "hr", false); }
        }

        public static Language Slovak
        {
            get { return new Language(SystemLanguage.Slovak.ToString(), "sk", false); }
        }

        public static Language Slovenian
        {
            get { return new Language(SystemLanguage.Slovenian.ToString(), "sl", false); }
        }

        public static Language Spanish
        {
            get { return new Language(SystemLanguage.Spanish.ToString(), "es", false); }
        }

        public static Language Swedish
        {
            get { return new Language(SystemLanguage.Swedish.ToString(), "sv", false); }
        }

        public static Language Thai
        {
            get { return new Language(SystemLanguage.Thai.ToString(), "th", false); }
        }

        public static Language Turkish
        {
            get { return new Language(SystemLanguage.Turkish.ToString(), "tr", false); }
        }

        public static Language Ukrainian
        {
            get { return new Language(SystemLanguage.Ukrainian.ToString(), "uk", false); }
        }

        public static Language Vietnamese
        {
            get { return new Language(SystemLanguage.Vietnamese.ToString(), "vi", false); }
        }

        public static Language ChineseSimplified
        {
            get { return new Language(SystemLanguage.ChineseSimplified.ToString(), "zh-Hans", false); }
        }

        public static Language ChineseTraditional
        {
            get { return new Language(SystemLanguage.ChineseTraditional.ToString(), "zh-Hant", false); }
        }

        public static Language Unknown
        {
            get { return new Language(SystemLanguage.Unknown.ToString(), "", false); }
        }

        [SerializeField] 
        private string m_Name = "";

        [SerializeField] 
        private string m_Code = "";

        [SerializeField] 
        private bool m_Custom = true;

        /// <summary>
        /// 语言名称。
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// 获取 <see href="https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes">ISO-639-1</see> 语言代码。
        /// </summary>
        /// <returns>ISO-639-1 code.</returns>
        public string Code
        {
            get { return m_Code; }
        }

        /// <summary>
        /// 语言是自定义的还是内置的，支持 <see cref="SystemLanguage"/> 转换.
        /// </summary>
        public bool Custom
        {
            get { return m_Custom; }
        }

        public Language(string name, string code)
        {
            m_Name = name ?? "";
            m_Code = code ?? "";
            m_Custom = true;
        }

        public Language(Language other)
        {
            m_Name = other.m_Name;
            m_Code = other.m_Code;
            m_Custom = other.m_Custom;
        }

        internal Language(string name, string code, bool custom)
        {
            m_Name = name ?? "";
            m_Code = code ?? "";
            m_Custom = custom;
        }

        public bool Equals(Language other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Language) obj);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static bool operator ==(Language left, Language right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Language left, Language right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator Language(SystemLanguage systemLanguage)
        {
            var index = Array.FindIndex(BuiltinLanguages, x => x.Name == systemLanguage.ToString());
            return index >= 0 ? BuiltinLanguages[index] : Unknown;
        }

        public static explicit operator SystemLanguage(Language language)
        {
            if (language.Custom) return SystemLanguage.Unknown;

            var systemLanguages = (SystemLanguage[]) Enum.GetValues(typeof(SystemLanguage));
            var index = Array.FindIndex(systemLanguages, x => x.ToString() == language.Name);
            return index >= 0 ? systemLanguages[index] : SystemLanguage.Unknown;
        }
    }
}