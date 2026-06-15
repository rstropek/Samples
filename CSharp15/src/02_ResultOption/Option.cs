// ============================================================================
//  Option<T> — a union modelling "a value, or nothing".
//  Demonstrates a *generic* union with a non-generic case (`None` singleton),
//  and C# extension *blocks* (extension members) rather than extension methods.
// ============================================================================
namespace Functional;

public record class Some<T>(T Value);

// A single shared "empty" marker. Because it is a *case type* of Option<T>
// for every T, one None converts implicitly into any Option<T>.
public sealed record class None
{
    public static readonly None Instance = new();
    private None() { }
}

public union Option<T>(Some<T>, None);

public static class Option
{
    public static Option<T> Some<T>(T value) => new Some<T>(value);
    public static Option<T> None<T>() => Functional.None.Instance;

    /// <summary>Wrap a possibly-null reference into an Option.</summary>
    public static Option<T> Of<T>(T? value) where T : class =>
        value is null ? None<T>() : Some(value);
}

// --- Extension block (C# extension members) -------------------------------
// All members below extend `Option<T>`. Note the extension properties
// (IsSome) alongside extension methods — only possible with extension blocks.
//
// The `null` arm handles the *default/empty* union (`default(Option<T>)`,
// whose contents are null). It keeps the switch exhaustive for nullable
// analysis and silences CS8655 on stricter previews, while CS8509 still fires
// if you add a new case type and forget it.
public static class OptionExtensions
{
    extension<T>(Option<T> option)
    {
        public bool IsSome => option is Some<T>;

        public Option<U> Map<U>(Func<T, U> f) => option switch
        {
            Some<T> s => new Some<U>(f(s.Value)),
            None n    => n,                       // None -> Option<U> implicitly
            null      => None.Instance,           // empty stays empty
        };

        public Option<U> Bind<U>(Func<T, Option<U>> f) => option switch
        {
            Some<T> s => f(s.Value),
            None n    => n,
            null      => None.Instance,
        };

        public T GetValueOrDefault(T fallback) => option switch
        {
            Some<T> s => s.Value,
            None      => fallback,
            null      => fallback,
        };

        public R Match<R>(Func<T, R> some, Func<R> none) => option switch
        {
            Some<T> s => some(s.Value),
            None      => none(),
            null      => none(),
        };
    }
}
