using System;

namespace Polygon.Core.Generators
{
    /// <summary>
    /// Implements a polygon generator that generates a random polygon consisting of 8 points
    /// </summary>
    [FriendlyName("Random Shape")]
    public class RandomPolygonGenerator : PolygonGenerator
    {
        /// <inheritdoc />
        public ReadOnlyMemory<Point> Generate(in double maxSideLength)
        {
            var maxRadius = maxSideLength / 2d;
            var random = new Random();

            // Discuss local functions
            double NextDouble() => random.NextDouble() * maxRadius;

            var points = new Point[8];
            points[0] = new Point(maxRadius, maxRadius - NextDouble());
            points[1] = new Point(maxRadius + NextDouble(), maxRadius - NextDouble());
            points[2] = new Point(maxRadius + NextDouble(), maxRadius);
            points[3] = new Point(maxRadius + NextDouble(), maxRadius + NextDouble());
            points[4] = new Point(maxRadius, maxRadius + NextDouble());
            points[5] = new Point(maxRadius - NextDouble(), maxRadius + NextDouble());
            points[6] = new Point(maxRadius - NextDouble(), maxRadius);
            points[7] = new Point(maxRadius - NextDouble(), maxRadius - NextDouble());

            return points;
        }
    }
}
