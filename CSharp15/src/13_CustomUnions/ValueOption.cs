// ============================================================================
//  A NON-BOXING union: the "union access pattern".
//
//  The generated `union X(...)` always stores `object? Value`, so value-type
//  cases (int, double, structs) get BOXED on every conversion. A hand-written
//  [Union] type can avoid that by adding the optional non-boxing members:
//
//    * bool HasValue                     — used for the `null` pattern
//    * bool TryGetValue(out TCase value) — one per case type, used for type patterns
//
//  When they exist, the compiler calls them INSTEAD of `Value` (check the IL
//  with ./decompile.sh). `Value` must still exist — it's the boxing fallback
//  for code that just wants an `object?`.
// ============================================================================
using System.Runtime.CompilerServices;

namespace CustomUnions;

/// <summary>The "nothing" case. Shared by every ValueOption&lt;T&gt;.</summary>
public sealed record None
{
    public static readonly None Instance = new();
    private None() { }
}

[Union]
public readonly struct ValueOption<T> : IUnion
{
    // Tagged storage: the T lives unboxed in its own field.
    private enum Tag : byte { Empty = 0, Some, None }

    private readonly Tag _tag;
    private readonly T _value;

    // One public single-parameter constructor per case type = the case types.
    // Contract: Value, HasValue and TryGetValue must agree. A null T (e.g.
    // ValueOption<string?>) therefore yields the EMPTY union, not "Some(null)".
    public ValueOption(T value) { _tag = value is null ? Tag.Empty : Tag.Some; _value = value; }
    public ValueOption(None _)  { _tag = Tag.None; _value = default!; }

    // Required by the union pattern (boxes T — only used if someone asks for it).
    public object? Value => _tag switch
    {
        Tag.Some => _value,
        Tag.None => None.Instance,
        _        => null,          // default(ValueOption<T>) — the empty union
    };

    // Optional non-boxing access pattern.
    public bool HasValue => _tag != Tag.Empty;
    public bool TryGetValue(out T value)    { value = _value; return _tag == Tag.Some; }
    public bool TryGetValue(out None value) { value = None.Instance; return _tag == Tag.None; }

    public override string ToString() => _tag switch
    {
        Tag.Some => $"Some({_value})",
        Tag.None => "None",
        _        => "<empty>",
    };
}

// Limitation: case types must be distinguishable — ValueOption<None> has two
// identical constructors and fails with CS0457 (ambiguous conversion).

// For comparison: the shorthand form of the same union. Always boxes `int`.
public readonly union BoxedOption<T>(T, None);
