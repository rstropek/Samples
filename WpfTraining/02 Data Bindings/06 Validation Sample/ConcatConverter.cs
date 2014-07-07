using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ValidationSample
{
	public class ConcatConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values.Aggregate<object, StringBuilder, string>(
				new StringBuilder(),
				(builder, next) => { builder.AppendSeparatedIfNotEmpty('\n', next); return builder; },
				builder => builder.ToString());
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
