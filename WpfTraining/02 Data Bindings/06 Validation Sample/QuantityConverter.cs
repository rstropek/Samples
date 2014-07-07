using System;
using System.Globalization;
using System.Windows.Data;

namespace ValidationSample
{
	public class QuantityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var intValue = (int)value;
			switch (intValue)
			{
				case 1:
					return "one";
				case 2:
					return "two";
				case 99:
					return "many";
				default:
					return intValue.ToString();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var sourceString = value.ToString();
			switch (sourceString.ToLower())
			{
				case "one":
					return 1;
				case "two":
					return 2;
				case "many":
					return 99;
				default:
					int result;
					if (Int32.TryParse(sourceString, out result))
					{
						return result;
					}

					throw new ApplicationException("Value is not a number");
			}
		}
	}
}
