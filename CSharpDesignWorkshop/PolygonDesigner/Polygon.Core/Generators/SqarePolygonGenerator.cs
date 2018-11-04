using System;

namespace Polygon.Core.Generators
{
    /// <summary>
    /// Implements a polygon generator that generates a square polygon
    /// </summary>
    [FriendlyName("Square")]
    public class SquarePolygonGenerator : PolygonGenerator
    {
        /// <inheritdoc />
        public ReadOnlyMemory<Point> Generate(in double maxSideLength)
        {
            var points = new Point[4];
            points[0] = new Point(0d, 0d);
            points[1] = new Point(maxSideLength, 0d);
            points[2] = new Point(maxSideLength, maxSideLength);
            points[3] = new Point(0d, maxSideLength);

            return points;
        }
    }
}
