using System;

namespace Polygon.Core.Generators
{
    /// <summary>
    /// Represents a polygon generator
    /// </summary>
    public interface PolygonGenerator
    {
        /// <summary>
        /// Generates polygon
        /// </summary>
        /// <param name="maxSideLength">Maximum side length (max. width and max. height)</param>
        /// <returns>Points of the polygon</returns>
        ReadOnlyMemory<Point> Generate(in double maxSideLength);
    }
}
