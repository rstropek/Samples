using BookshelfConfigurator.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BookshelfConfigurator.ViewModel
{
	public class MaximumNumberOfShelfsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ElementHeight)
			{
				var heightValue = (ElementHeight)value;
				return ElementDimension.MaximumNumberOfShelfs(heightValue);
			}

			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
