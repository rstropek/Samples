using System;

namespace Polygon.Core
{
    /// <summary>
    /// Check if a given point is inside a given shape
    /// </summary>
    public interface ContainmentChecker
    {
        bool Contains(in ReadOnlySpan<Point> shape, in Point point);
    }
}
