using System;
using System.Globalization;
using System.Windows.Data;

namespace CompositeWpfApp.Shell
{
	public class TypeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.GetType().FullName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
