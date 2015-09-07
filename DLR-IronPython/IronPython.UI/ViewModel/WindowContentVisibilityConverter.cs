using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace IronPython.UI.ViewModel
{
	public class WindowContentVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is WindowContent))
			{
				throw new ArgumentException("value");
			}

			WindowContent windowContent;
			if (!Enum.TryParse<WindowContent>(parameter.ToString(), out windowContent))
			{
				throw new ArgumentException("parameter");
			}

			if (!(targetType.IsAssignableFrom(typeof(Visibility))))
			{
				throw new ArgumentException("targetType");
			}

			return ((WindowContent)value) == windowContent ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
