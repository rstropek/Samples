// Switch between custom generic math and build-in generic math
//#define CUSTOM_GENERIC_MATH

#if !CUSTOM_GENERIC_MATH
using System.Numerics;
#endif

Span<Line> lines = stackalloc[]
{
    new Line(new(1f, 1f), new(4f, 5f)),
    new Line(new(0f, 0f), new(3f, 4f)),
};
Console.WriteLine(SumOfLines<VectorLength>(lines));
Console.WriteLine(SumOfLines<BoundsRect>(lines));

T SumOfLines<T>(Span<Line> lines)
#if CUSTOM_GENERIC_MATH
    where T : ISupportAdding<T, Line, T>, IHaveZero<T>
#else
    where T : IAdditionOperators<T, Line, T>, IHaveZero<T>
#endif
{
    var result = T.Zero;
    foreach (var line in lines)
    {
        result += line;
    }

    return result;
}

#region Basic Vector and Line types
readonly record struct Vector(float X, float Y)
{
    public static Vector operator +(Vector first, Vector second)
        => new(first.X + second.X, first.Y + second.Y);
}

readonly record struct LineSettings(float Lightness, float Width)
{
    public LineSettings() : this(50, 1) { }
}

readonly record struct Line(Vector Start, Vector End, LineSettings Settings)
{
    public Line() : this(new(0f, 0f), new(0f, 0f)) { }
    public Line(Vector Start, Vector End) : this(Start, End, new()) { }
}
#endregion

interface IHaveZero<T>
{
    // Note: Static abstract members in interfaces
    static abstract T Zero { get; }
}

#if CUSTOM_GENERIC_MATH
// Note: This method is for demo purposes only. With generic math, .NET gets
//       something like this built-in.
interface ISupportAdding<TSelf, TOther, TResult>
        where TSelf : ISupportAdding<TSelf, TOther, TResult>
{
    // Note: Static abstract members in interfaces
    static abstract TResult operator +(TSelf left, TOther right);
}
#endif

readonly record struct BoundsRect(float X1, float Y1, float X2, float Y2)
#if CUSTOM_GENERIC_MATH
    : ISupportAdding<BoundsRect, Line, BoundsRect>, IHaveZero<BoundsRect>
#else
    : IAdditionOperators<BoundsRect, Line, BoundsRect>, IHaveZero<BoundsRect>
#endif
{
    public BoundsRect() : this(0f, 0f, 0f, 0f) { }

    public static BoundsRect Zero => new();

    public static BoundsRect operator +(BoundsRect b, Line l)
    {
        if (l.Start.X < b.X1) b = b with { X1 = l.Start.X };
        if (l.End.X < b.X1) b = b with { X1 = l.End.X };
        if (l.Start.X > b.X2) b = b with { X2 = l.Start.X };
        if (l.End.X > b.X2) b = b with { X2 = l.End.X };

        if (l.Start.Y < b.Y1) b = b with { Y1 = l.Start.Y };
        if (l.End.Y < b.Y1) b = b with { Y1 = l.End.Y };
        if (l.Start.Y > b.Y2) b = b with { Y2 = l.Start.Y };
        if (l.End.Y > b.Y2) b = b with { Y2 = l.End.Y };

        return b;
    }

    public float Width => X2 - X1;
    public float Height => Y2 - Y1;
}

readonly record struct VectorLength(float Length)
#if CUSTOM_GENERIC_MATH
    : ISupportAdding<VectorLength, Line, VectorLength>, IHaveZero<VectorLength>
#else
    : IAdditionOperators<VectorLength, Line, VectorLength>, IHaveZero<VectorLength>
#endif
{
    public VectorLength() : this(0f) { }

    public static VectorLength Zero => new();

    public static VectorLength operator +(VectorLength b, Line l)
    {
        var la = l.End.X - l.Start.X;
        var lb = l.End.Y - l.Start.Y;
        return new VectorLength(b.Length + (float)Math.Sqrt(la * la + lb * lb));
    }

    public override string ToString() => Length.ToString();
}
