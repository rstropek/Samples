using System;
using System.Globalization;
using System.Windows.Data;

namespace BookshelfConfigurator.ViewModel
{
	public class NumberOfShelfsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				var intValue = (int)value;
				switch(intValue)
				{
					case 0:
						return "No Shelfs";
					case 1:
						return "1 Shelf";
					default:
						return string.Format("{0} Shelfs", intValue);
				}
			}

			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
