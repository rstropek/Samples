using System.Diagnostics.CodeAnalysis;

namespace CSharpImmutables
{
    public struct MutableVector2d : IEquatable<MutableVector2d>
    {
        // Two mutable properties
        public double X { get; set; }
        public double Y { get; set; }

        public MutableVector2d(double x, double y) => (X, Y) = (x, y);

        // Note `readonly` modifier indicating that prop/method does not modify state.
        // Compiler will tell you if you accidentally change state. Perf will also
        // be slightly better because no defensive copies.
        public readonly double Distance => Math.Sqrt(X * X + Y * Y);

        // Try adding `readonly` -> will lead to an error
        public /*readonly*/ void Double() { X *= 2d; Y *= 2d; }

        public readonly override int GetHashCode() => HashCode.Combine(X, Y);

        #region Implementation of equatable
        public override readonly bool Equals([NotNullWhen(true)] object? v) => v is MutableVector2d && Equals(v);
        public readonly bool Equals(MutableVector2d other) => other.X == X && other.Y == Y;
        public static bool operator ==(MutableVector2d left, MutableVector2d right) => left.Equals(right);
        public static bool operator !=(MutableVector2d left, MutableVector2d right) => !(left == right);
        #endregion
    }

    // Note `readonly` modifier for immutable struct.
    // Note that you can combine `readonly` with `ref` for structs that
    // can/should only live on the stack and never move to the heap.
    public readonly struct ImmutableVector2d : IEquatable<ImmutableVector2d>
    {
        // Two immutable properties
        public double X { get; init; }
        public double Y { get; init; }

        public ImmutableVector2d(double x, double y) => (X, Y) = (x, y);

        public double Distance => Math.Sqrt(X * X + Y * Y);

        // Note that `Double` returns a new instance of the vector
        public ImmutableVector2d Double() => new(X * 2d, Y * 2d);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        #region Implementation of equatable
        public override bool Equals([NotNullWhen(true)] object? v) => v is ImmutableVector2d && Equals(v);

        public bool Equals(ImmutableVector2d other) => other.X == X && other.Y == Y;

        public static bool operator ==(ImmutableVector2d left, ImmutableVector2d right) => left.Equals(right);

        public static bool operator !=(ImmutableVector2d left, ImmutableVector2d right) => !(left == right);
        #endregion
    }

    public readonly record struct VectorRecord2d(double X, double Y)
    {
        public double Distance => Math.Sqrt(X * X + Y * Y);

        // Note that `Double` returns a new instance of the vector
        public VectorRecord2d Double() => new(X * 2d, Y * 2d);
    }
}