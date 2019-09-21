using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Polygon.Core
{
    public static class PathMarkupConverter
    {
        public static string Convert(in ReadOnlyMemory<Point> pointsMemory)
        {
            if (pointsMemory.Length == 0)
            {
                return string.Empty;
            }

            var points = pointsMemory.Span;
            var pointStrings = (x: new string[points.Length], y: new string[points.Length]);
            for (var i = 0; i < points.Length; i++)
            {
                pointStrings.x[i] = points[i].X.ToString("G", CultureInfo.InvariantCulture);
                pointStrings.y[i] = points[i].Y.ToString("G", CultureInfo.InvariantCulture);
            }

            var length =
                pointsMemory.Length * 2 // "M"/"L" and ","
                + pointStrings.x.Sum(s => s.Length) + pointStrings.y.Sum(s => s.Length) // Length of numbers
                + 1; // Trailing "Z"
            return string.Create(length, pointStrings, (buffer, p) =>
            {
                for (var i = 0; i < p.x.Length; i++)
                {
                    buffer[0] = i == 0 ? 'M' : 'L';
                    buffer = buffer[1..];

                    p.x[i].AsSpan().CopyTo(buffer);
                    buffer = buffer[p.x[i].Length..];

                    buffer[0] = ',';
                    buffer = buffer[1..];

                    p.y[i].AsSpan().CopyTo(buffer);
                    buffer = buffer[p.y[i].Length..];
                }

                buffer[0] = 'Z';
            });
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

            // Note: Parsing with new Span<T> API

            var result = new Point[pointsMarkup.Count(c => c == 'L') + 1];
            var resultIndex = 0;
            ReadOnlySpan<char> pmSpan = pointsMarkup.AsSpan(1);
            while (pmSpan.Length != 0)
            {
                // Parse x coordinate
                var indexOfComma = pmSpan.IndexOf(',');
                var x = double.Parse(pmSpan[..indexOfComma]);

                // Parse y coordinate
                pmSpan = pmSpan[(indexOfComma + 1)..];
                var indexOfNextLine = pmSpan.IndexOfAny('L', 'Z');
                var y = double.Parse(pmSpan[..indexOfNextLine]);

                // Write value of point
                result[resultIndex++] = new Point(x, y);

                // Jump to next point
                pmSpan = pmSpan[(indexOfNextLine + 1)..];
            }

            // Note that we can return an array as memory
            return result;
        }
    }
}
