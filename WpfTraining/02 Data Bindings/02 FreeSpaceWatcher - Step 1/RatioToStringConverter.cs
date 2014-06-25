using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Samples
{
	[ValueConversion(typeof(Double), typeof(String))]
	public class RatioToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, 
			System.Globalization.CultureInfo culture)
		{
			return ((Double)value).ToString(parameter as String, culture.NumberFormat);
		}

		public object ConvertBack(object value, Type targetType, object parameter, 
			System.Globalization.CultureInfo culture)
		{
			return System.Convert.ToDouble(value as string, culture.NumberFormat);
		}
	}
}
