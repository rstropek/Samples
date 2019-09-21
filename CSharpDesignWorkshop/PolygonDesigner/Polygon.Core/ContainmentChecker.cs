using System;

namespace Polygon.Core
{
    /// <summary>
    /// Check if a given point is inside a given shape
    /// </summary>
    public interface IContainmentChecker
    {
        bool Contains(in ReadOnlySpan<Point> shape, in Point point);
    }
}
