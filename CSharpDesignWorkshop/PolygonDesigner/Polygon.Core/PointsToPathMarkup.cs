using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

        public static string ConvertOld(ReadOnlySpan<Point> points)
        {
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

        public static ReadOnlyMemory<Point> ConvertOld(string pointsMarkup)
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
