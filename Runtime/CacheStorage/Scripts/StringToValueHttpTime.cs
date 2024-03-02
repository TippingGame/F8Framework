using System;
using System.Globalization;

namespace F8Framework.Core
{
    public class StringToValueHttpTime : StringToValue<DateTime>
    {
        public StringToValueHttpTime(long ticks) : base(new DateTime(ticks, DateTimeKind.Utc))
        {
        }

        public StringToValueHttpTime(string text = "") : base(text)
        {
        }

        public StringToValueHttpTime(DateTime value) : base(value)
        {
        }

        public long Ticks()
        {
            return GetValue().Ticks;
        }

        public static implicit operator long(StringToValueHttpTime data)
        {
            if (data == null)
            {
                return 0;
            }

            return data.Ticks();
        }

        public static implicit operator StringToValueHttpTime(long ticks)
        {
            if (ticks == 0)
            {
                return null;
            }

            return new StringToValueHttpTime(ticks);
        }

        public static implicit operator StringToValueHttpTime(string value)
        {
            if (string.IsNullOrEmpty(value) == true)
            {
                return null;
            }

            return new StringToValueHttpTime(value);
        }

        public static implicit operator StringToValueHttpTime(DateTime value)
        {
            return new StringToValueHttpTime(value);
        }

        protected override string ConvertText(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            return dateTime.ToString("r");
        }

        protected override DateTime ConvertValue(string text)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(text, "ddd, dd MMM yyyy HH:mm:ss Z", CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal, out dateTime) == true)
            {
                return dateTime;
            }

            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime) ==
                true)
            {
                return dateTime;
            }

            return default(DateTime);
        }
    }
}