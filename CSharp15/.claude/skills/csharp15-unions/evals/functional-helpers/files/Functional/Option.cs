namespace Functional;

/// <summary>Optional value.</summary>
/// <remarks>
/// Used on the hot path of our CSV import (tens of millions of calls per file), mostly
/// as <c>Option&lt;int&gt;</c> and <c>Option&lt;decimal&gt;</c>. It is a struct on purpose:
/// it must not allocate.
/// </remarks>
public readonly struct Option<T>
{
    private readonly T _value;

    private Option(T value) { _value = value; HasValue = true; }

    public bool HasValue { get; }

    public static Option<T> None => default;
    public static Option<T> Some(T value) => new(value);

    public bool TryGetValue(out T value)
    {
        value = _value;
        return HasValue;
    }

    public T GetValueOrDefault(T fallback) => HasValue ? _value : fallback;

    public override string ToString() => HasValue ? $"Some({_value})" : "None";
}
