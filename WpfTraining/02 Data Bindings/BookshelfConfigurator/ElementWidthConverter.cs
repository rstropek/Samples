using System;
using System.Globalization;
using System.Windows.Data;

namespace BookshelfConfigurator
{
	public class ElementWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ElementWidth)
			{
				var width = (ElementWidth)value;

				var zoomFactor = 1.0;
				if (parameter != null)
				{
					Double.TryParse(parameter.ToString(), out zoomFactor);
				}

				return ElementDimension.WidthInCm(width) * zoomFactor;
			}

			return 0.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
