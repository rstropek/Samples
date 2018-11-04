using System;

namespace Polygon.Core
{
    /// <summary>
    /// Represents a 2D vector
    /// </summary>
    public struct Vector : IEquatable<Vector>
    {
        public double X { get; }

        public double Y { get; }

        public Vector(in double x, in double y)
        {
            X = x;
            Y = y;
        }

        public Vector(in Vector source) => (X, Y) = source;

        public bool Equals(Vector other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => Equals((Vector)obj);

        public override int GetHashCode() => unchecked(X.GetHashCode() * 17 + Y.GetHashCode());

        public static bool operator ==(Vector left, Vector right) => left.Equals(right);

        public static bool operator !=(Vector left, Vector right) => !left.Equals(right);

        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }

        public static Vector operator *(double scalar, Vector vector) => 
            new Vector(vector.X * scalar, vector.Y * scalar);
    }
}
