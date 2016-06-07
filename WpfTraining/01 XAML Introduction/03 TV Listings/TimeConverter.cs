using System;
using System.Windows.Data;

namespace Samples
{
    [ValueConversion(typeof(DateTime), typeof(String))]
	public class TimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var date = (DateTime)value;
			return date.ToString("HH:mm");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
