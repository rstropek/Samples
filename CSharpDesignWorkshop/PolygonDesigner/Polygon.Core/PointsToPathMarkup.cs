using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Polygon.Core
{
    public static class PathMarkupConverter
    {
        public static string Convert(ReadOnlyMemory<Point> points)
        {
            return Convert(points.Span);
        }

        public static string Convert(ReadOnlySpan<Point> points)
        {
            if (points.Length == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(points.Length * 20);
            void AppendDouble(in double d) => builder.Append(Math.Round(d, 4).ToString("G", CultureInfo.InvariantCulture));

            for (var i = 0; i < points.Length; i++)
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

        public static ReadOnlyMemory<Point> Convert(string pointsMarkup)
        {
            if (string.IsNullOrEmpty(pointsMarkup))
            {
                return Array.Empty<Point>();
            }

            if (!Regex.IsMatch(pointsMarkup, @"^M[-+]?\d+(\.\d+)?,[-+]?\d+(\.\d+)?(L[-+]?\d+(\.\d+)?,[-+]?\d+(\.\d+)?)*Z$"))
            {
                throw new ArgumentException("Point markup is invalid", nameof(pointsMarkup));
            }

            // Note that we cannot use span'ified APIs here because the parse methods
            // are not available in .NET Standard 2.0. They will be in 2.1.

            var result = new List<Point>();
            var index = 0;
            while (pointsMarkup[index] != 'Z')
            {
                var indexOfComma = pointsMarkup.IndexOf(',', index);
                var indexOfNextLine = pointsMarkup.IndexOfAny(new[] { 'L', 'Z' }, indexOfComma);
                var x = double.Parse(pointsMarkup.Substring(index + 1, indexOfComma - index - 1));
                var y = double.Parse(pointsMarkup.Substring(indexOfComma + 1, indexOfNextLine - indexOfComma - 1));
                result.Add(new Point(x, y));

                index = indexOfNextLine;
            }

            return result.ToArray();
        }
    }
}
