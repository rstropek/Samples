using System;
using System.Windows.Data;

namespace DeviceMonitor
{
    public class ScaleMeasurementConverter : IValueConverter
	{
		public double Factor { get; set; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var doubleValue = (double)value;
			return doubleValue * this.Factor;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
