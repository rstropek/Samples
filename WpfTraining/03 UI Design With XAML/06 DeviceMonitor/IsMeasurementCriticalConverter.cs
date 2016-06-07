using System;
using System.Windows.Data;
using System.Windows.Media;

namespace DeviceMonitor
{
    public class IsMeasurementCriticalConverter : IValueConverter
	{
		public double CriticalFactor { get; set; }
		public Brush NormalValueBrush { get; set; }
		public Brush CriticalValueBrush { get; set; }

		public IsMeasurementCriticalConverter()
		{
			this.CriticalFactor = 0.8;
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var doubleValue = (double)value;
			if (doubleValue >= this.CriticalFactor)
			{
				return this.CriticalValueBrush;
			}

			return this.NormalValueBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
