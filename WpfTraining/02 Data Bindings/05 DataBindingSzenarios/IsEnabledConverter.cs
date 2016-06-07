using System;
using System.Windows.Data;

namespace SkiResults.Converter
{
    public class IsEnabledConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool isEnabled = true;

			foreach (var item in values)
			{
				isEnabled &= (int)item == 0;
			}

			return isEnabled;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
 			throw new NotImplementedException();
		}
	}
}
