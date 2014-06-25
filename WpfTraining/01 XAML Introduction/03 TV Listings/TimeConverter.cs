using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Samples
{
	[ValueConversion(typeof(DateTime), typeof(String))]
	public class TimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DateTime date = (DateTime)value;
			return date.ToString("HH:mm");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
