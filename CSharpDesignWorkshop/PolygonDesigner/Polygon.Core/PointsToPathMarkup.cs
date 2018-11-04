using System;
using System.Globalization;
using System.Text;

namespace Polygon.Core
{
    public class PointsToPathMarkup
    {
        public string Convert(ReadOnlySpan<Point> points)
        {
            var builder = new StringBuilder(points.Length * 20);
            void AppendDouble(in double d) => builder.Append(Math.Round(d, 4).ToString("G", CultureInfo.InvariantCulture));

            for(var i = 0; i < points.Length; i++)
            {
                builder.Append(i == 0 ? 'M' : 'L');
                var p = points[i];
                AppendDouble(p.X);
                builder.Append(',');
                AppendDouble(p.Y);
            }

            builder.Append('Z');

            return builder.ToString();
        }
    }
}
