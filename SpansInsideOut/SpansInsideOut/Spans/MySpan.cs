namespace Spans;

/// <summary>
/// Naive implementation of a range over an array
/// </summary>
/// <remarks>
/// Use this struct to illustrate the idea behind C#'s Span. This is NOT
/// production code. You should ALWAYS prefer C#'s Span class.
/// </remarks>
public readonly ref struct MySpan<T>
{
    private readonly T[] underlyingArray;
    private readonly int start;
    private readonly int length;

    public MySpan(T[] source) : this(source, 0, source.Length) { }

    public MySpan(T[] source, int start, int length) =>
        (underlyingArray, this.start, this.length) = (source, start, length);

    public static implicit operator MySpan<T>(T[] source) => new(source);

    public T[] ToArray()
    {
        var result = new T[Length];
        Array.Copy(underlyingArray, start, result, 0, Length);
        return result;
    }

    public int Length => length;

    public ref T this[int index] => ref underlyingArray[start + index];

    public ref T this[Index index] => ref underlyingArray[start + index.GetOffset(Length)];

    public MySpan<T> this[Range range]
    {
        get
        {
            var startFromBegin = range.Start.GetOffset(underlyingArray.Length);
            var endFromBegin = range.End.GetOffset(underlyingArray.Length);
            return new MySpan<T>(underlyingArray, 
                startFromBegin,
                endFromBegin - startFromBegin);
        }
    }
}
