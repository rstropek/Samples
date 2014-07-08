using BookshelfConfigurator.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BookshelfConfigurator.ViewModel
{
	public class ElementHeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ElementHeight)
			{
				var height = (ElementHeight)value;

				var zoomFactor = 1.0;
				if (parameter != null)
				{
					Double.TryParse(parameter.ToString(), out zoomFactor);
				}

				return ElementDimension.HeightInCm(height) * zoomFactor;
			}

			return 0.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
