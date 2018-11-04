using System;

namespace Polygon.Core
{
    /// <summary>
    /// Represents a 2D point
    /// </summary>
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// Gets the X coordinate of the point
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate of the point
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct
        /// </summary>
        /// <param name="x">Initial X coordinate</param>
        /// <param name="y">Initial Y coordinate</param>
        public Point(in double x, in double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct
        /// </summary>
        /// <param name="source">Point to copy</param>
        public Point(in Point source) => (X, Y) = source;

        /// <inheritdoc />
        public bool Equals(Point other) => X == other.X && Y == other.Y;

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals((Point)obj);

        /// <inheritdoc />
        public override int GetHashCode() => unchecked(X.GetHashCode() * 17 + Y.GetHashCode());

        public static bool operator ==(Point left, Point right) => left.Equals(right);

        public static bool operator !=(Point left, Point right) => !left.Equals(right);

        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }

        public static Vector operator -(Point point1, Point point2) => 
            new Vector(point1.X - point2.X, point1.Y - point2.Y);

        public static Point operator +(Point point, Vector vector) =>
            new Point(point.X + vector.X, point.Y + vector.Y );
    }
}
