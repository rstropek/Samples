using System.Diagnostics.CodeAnalysis;

namespace CSharpImmutables
{
    public struct MutableVector2d : IEquatable<MutableVector2d>
    {
        // Two mutable fields
        public double X;
        public double Y;

        public MutableVector2d(double x, double y) => (X, Y) = (x, y);

        // Note `readonly` modifier indicating that prop/method does not modify state.
        // Compiler will tell you if you accidentally change state. Perf will also
        // be slightly better because no defensive copies.
        public readonly double Distance => Math.Sqrt(X * X + Y * Y);

        // Try adding `readonly` -> will lead to an error
        public /*readonly*/ void Double() { X *= 2d; Y *= 2d; }

        // Note that the following method changes the state of the vector.
        // This will be important later.
        public void Move(double x, double y) { X += x; Y += y; }

        public readonly override int GetHashCode() => HashCode.Combine(X, Y);

        public readonly override string ToString() => $"{{ X: {X}, Y: {Y} }}";

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

    // Now the same with a `readoly record struct`. Much shorter :)
    public readonly record struct VectorRecord2d(double X, double Y)
    {
        public double Distance => Math.Sqrt(X * X + Y * Y);

        // Note that `Double` returns a new instance of the vector
        public VectorRecord2d Double() => new(X * 2d, Y * 2d);
    }

    // Let's build a naive mutable line. Check the unit tests, it works.
    public struct MutableLine2d
    {
        // Try changing field to property. Unit tests fail now. Can you find out why?
        // Answer: Property getter returns a copy of Start and Move is called on the copy.
        // Learning: Mutable structs are tricky and you have to be very careful with them
        public MutableVector2d Start; // { get; set; } 
        public MutableVector2d End; // { get; set; } 

        public MutableLine2d(MutableVector2d start, MutableVector2d end) => (Start, End) = (start, end);

        public void Move(double x, double y)
        {
            Start.Move(x, y);
            End.Move(x, y);
        }
    }

    // Note that the following struct is shallow immutable.
    // It is immutable because of `readonly`, but its members are not.
    public readonly struct ImmutableLine2d
    {
        public readonly MutableVector2d Start;
        public readonly MutableVector2d End;

        public ImmutableLine2d(MutableVector2d start, MutableVector2d end) => (Start, End) = (start, end);

        public void Move(double x, double y)
        {
            // Note that we call a method here that mutates the states of
            // Start and End. However, we are in a readonly class.
            // Therefore, C# will create a "defensive copy". The copy will be
            // changed, not the original value. Therefore, any changes that
            // Move does will be lost. Try in debugger.

            // Defensive copying is done because:
            // * Start is a readonly field and
            // * type of Start (MutableVector2d) is a non-readonly struct

            Start.Move(x, y);
            End.Move(x, y);
        }
    }
}