using System;
using UnityEngine;

namespace F8Framework.Core
{
    [Serializable]
    public class StringToValue<T> where T : struct
    {
        [SerializeField] private string text;

        private T value;

        private bool converted;

        public StringToValue(string text = "")
        {
            if (string.IsNullOrEmpty(text) == true)
            {
                text = string.Empty;
            }

            this.text = text;

            this.value = default(T);

            this.converted = false;
        }

        public StringToValue(T value)
        {
            this.text = string.Empty;

            this.value = value;

            this.converted = true;
        }

        public string GetText()
        {
            if (converted == true)
            {
                if (string.IsNullOrEmpty(text) == true)
                {
                    text = ConvertText(value);
                }
            }

            return text;
        }

        public T GetValue()
        {
            if (converted == false)
            {
                try
                {
                    if (string.IsNullOrEmpty(text) == false)
                    {
                        value = ConvertValue(text);
                        converted = true;
                    }
                }
                catch
                {
                    SetText(string.Empty);
                }
            }

            return value;
        }

        public void SetText(string text)
        {
            if (string.IsNullOrEmpty(text) == true)
            {
                text = string.Empty;
            }

            if (text.Equals(this.text) == false)
            {
                this.text = text;

                this.value = default(T);

                this.converted = false;
            }
        }

        public void SetValue(T value)
        {
            if (value.Equals(this.value) == false)
            {
                this.text = string.Empty;
            }

            this.value = value;

            this.converted = true;
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }
            else if (other is T value)
            {
                return GetValue().Equals(value);
            }
            else if (other is StringToValue<T> stringToValue)
            {
                if (System.Object.ReferenceEquals(this, other) == true)
                {
                    return true;
                }

                return GetValue().Equals(stringToValue.GetValue());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        public static bool operator ==(StringToValue<T> lhs, StringToValue<T> rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(StringToValue<T> lhs, StringToValue<T> rhs) => !(lhs == rhs);

        public static implicit operator T(StringToValue<T> data)
        {
            if (data == null)
            {
                return default(T);
            }

            return data.GetValue();
        }

        public static implicit operator string(StringToValue<T> data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            return data.GetText();
        }

        public static implicit operator StringToValue<T>(string value)
        {
            if (string.IsNullOrEmpty(value) == true)
            {
                return null;
            }

            return new StringToValue<T>(value);
        }

        public static implicit operator StringToValue<T>(T value)
        {
            return new StringToValue<T>(value);
        }

        public override string ToString()
        {
            return GetText();
        }

        protected virtual string ConvertText(T value)
        {
            return value.ToString();
        }

        protected virtual T ConvertValue(string text)
        {
            return (T)Convert.ChangeType(text, typeof(T));
        }
    }
}