using Polygon.Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PolygonDesigner
{
    public class PointsToPointMarkupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Geometry))
            {
                throw new ArgumentException("Invalid destination type", nameof(targetType));
            }

            if (value is ReadOnlyMemory<Point> polygon)
            {
                return Geometry.Parse(PathMarkupConverter.Convert(polygon));
            }

            throw new ArgumentException("value is not a supported data type", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
