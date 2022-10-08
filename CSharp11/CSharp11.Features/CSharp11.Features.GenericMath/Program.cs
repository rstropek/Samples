// Switch between custom generic math and build-in generic math
//#define CUSTOM_GENERIC_MATH

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if !CUSTOM_GENERIC_MATH
using System.Numerics;
#endif

#region IParsable
var vectors = """
    1,1
    3,4
    """;
ParseAndPrint<Vector>(vectors);
ParseFromSpanAndPrint<Vector>(vectors);

void ParseAndPrint<T>(string text) where T : IParsable<T>
{
    foreach (var vectorText in text.Split('\n'))
    {
        Console.WriteLine(T.Parse(vectorText, CultureInfo.InvariantCulture));
    }
}

void ParseFromSpanAndPrint<T>(ReadOnlySpan<char> text) where T : ISpanParsable<T>
{
    while (true)
    {
        var length = text.IndexOf('\n');
        if (length == -1) { length = text.Length; }

        Console.WriteLine(T.Parse(text[..length], CultureInfo.InvariantCulture));
        if (length == text.Length) { break; }
        text = text[(length + 1)..];
    }
}
#endregion

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
    : IParsable<Vector>, ISpanParsable<Vector>
{
    #region IParsable
    public static Vector Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var result))
        {
            throw new FormatException();
        }

        return result;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Vector result)
    {
        var elements = s?.Split(',');
        if (elements == null
            || elements.Length != 2
            || !float.TryParse(elements[0], provider, out var x)
            || !float.TryParse(elements[1], provider, out var y))
        {
            result = new();
            return false;
        }

        result = new(x, y);
        return true;
    }
    #endregion

    #region ISpanParsable
    public static Vector Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var result))
        {
            throw new FormatException();
        }

        return result;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Vector result)
    {
        var indexOfComma = s.IndexOf(',');
        if (indexOfComma == -1
            || !float.TryParse(s[..indexOfComma], provider, out var x)
            || !float.TryParse(s[(indexOfComma + 1)..], provider, out var y))
        {
            result = new();
            return false;
        }

        result = new(x, y);
        return true;
    }
    #endregion

    public static Vector operator +(Vector first, Vector second)
        => new(first.X + second.X, first.Y + second.Y);
}

readonly record struct Line(Vector Start, Vector End)
{
    public Line() : this(new(0f, 0f), new(0f, 0f)) { }
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
