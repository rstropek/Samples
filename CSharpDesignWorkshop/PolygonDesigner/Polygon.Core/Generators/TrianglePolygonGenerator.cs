using System;

namespace Polygon.Core.Generators
{
    /// <summary>
    /// Implements a polygon generator that generates a triangle polygon
    /// </summary>
    [FriendlyName("Triangle")]
    public class TrianglePolygonGenerator : PolygonGenerator
    {
        /// <inheritdoc />
        public ReadOnlyMemory<Point> Generate(in double maxSideLength)
        {
            var points = new Point[3];
            points[0] = new Point(maxSideLength / 2d, 0d);
            points[1] = new Point(maxSideLength, maxSideLength);
            points[2] = new Point(0d, maxSideLength);

            return points;
        }
    }
}
