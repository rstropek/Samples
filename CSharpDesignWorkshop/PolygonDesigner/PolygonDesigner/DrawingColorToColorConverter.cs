using Polygon.Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PolygonDesigner
{
    public class DrawingColorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Color))
            {
                throw new ArgumentException("Invalid destination type", nameof(targetType));
            }

            if (value is System.Drawing.Color color)
            {
                return Color.FromArgb(color.A, color.R, color.G, color.B);
            }

            throw new ArgumentException("value is not a supported data type", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
