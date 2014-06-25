using System.Windows.Data;
using System;
using System.Windows.Controls;

namespace Samples
{
	public class TimeToStringConverter : ValidationRule, IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return "";
			}

			if (!(value is TimeSpan))
			{
				throw new ArgumentException("Parameter value is not of type TimeSpan.", "value");
			}

			var timespan = (TimeSpan)value;
			return timespan.Minutes.ToString() + ":" + timespan.Seconds.ToString("00") + "." + ((int)timespan.Milliseconds / 10).ToString("00");
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			if (!(value is string))
			{
				throw new ArgumentException("Parameter value is not of type string.", "value");
			}

			if (string.IsNullOrEmpty((string)value))
			{
				return null;
			}

			if (this.Validate(value, culture).IsValid)
			{
				return TimeSpan.Parse("00:0" + (string)value);
			}
			else
			{
				return value;
			}
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			TimeSpan result;
			return new ValidationResult(TimeSpan.TryParse("00:0" + (string)value, out result), "The string is in incorrect format");
		}
	}
}
