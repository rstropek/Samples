# Review: Functional helpers (net11.0)

The project targets `net11.0`, so it compiles as C# 15 by default. Union types (`union X(A, B)`) and custom `[Union]` types work without `LangVersion` or preview flags. Everything below was compiled and run on the .NET 11 SDK (11.0.100) in a scratch copy. I didn't touch your project files.

## Summary

| File | Recommendation | Plumbing removed | Risk |
|---|---|---|---|
| `Either.cs` | Replace with `public union Either<T0, T1>(T0, T1)` | `_index` and `_value`, `IsT0`/`IsT1`/`AsT0`/`AsT1`, both `implicit operator`s, `Switch`. You can keep `Match` as a 4-line shim or drop it | Low. It also fixes a real bug (see below) |
| `Result.cs` | Replace the class with `public union Result<T>(T, Error)` plus a `record Error(string Message)` | Fields, private ctor, `IsSuccess`/`Value`/`Error` throwing accessors, `Success`/`Failure` factories, `implicit operator` | Medium. It breaks the public API (class becomes struct, `Failure(string)` becomes `new Error(...)`) |
| `Option.cs` | **Do not** make it a shorthand `union`. If you want pattern syntax, turn it into a *custom* `[Union]` struct that keeps `HasValue`/`TryGetValue` | Little. Mostly you *gain* `opt is int v`, exhaustive `switch`, and `Option<int> o = 5;` | Low if done the custom way. **Very high if done the shorthand way (allocates on every call)** |

## 1. `Either<T0, T1>` → shorthand union

The whole file becomes:

```csharp
public union Either<T0, T1>(T0, T1) where T0 : notnull where T1 : notnull
{
    // Optional migration shim so existing .Match(...) callers keep compiling.
    public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1) => this switch
    {
        T0 a => f0(a),
        T1 b => f1(b),
    };
}
```

- **Implicit conversions come for free.** `Find` stays unchanged: `... ? user : new NotFound(id)`.
- **Callers can use plain switch expressions, and the compiler checks them for exhaustiveness.** A missing case gives CS8509, a warning. Add `<WarningsAsErrors>CS8509</WarningsAsErrors>` if you want it to fail the build. Don't add a `_ =>` arm: it would silently absorb future cases.
- **Use `where T0 : notnull where T1 : notnull`.** Without it, `this switch` in the body warns with CS8655 (null not covered), because unconstrained type parameters count as possibly nullable. With the constraints the generic body builds clean (verified).
- **No boxing regression.** Your current struct already stores the case in `object? _value`, so value-type cases box today too. The union lowers to the same thing: a struct with an `object? Value`.
- **`Either<string, string>` still doesn't work.** It fails with CS0457 at the conversion site, exactly as your hand-written version does today (I verified both). Nothing is lost.
- **Bug fixed along the way.** In the current code `default(Either<User, NotFound>)` has `_index == 0`. So `IsT0` is `true`, and `Match` calls the `User` lambda with `null`. For a value-type `T0` it throws `NullReferenceException` on the unbox. The `_ => throw "Uninitialised Either"` arm in `Match` and the `else throw` in `Switch` can never run. I reproduced both cases. With the union, `default` has `Value == null`. A switch then throws `SwitchExpressionException`, or you handle it explicitly with a `null =>` arm.
- **Breaking for consumers.** `IsT0`/`IsT1`/`AsT0`/`AsT1` and `Switch` go away. If other assemblies use them, you can keep them as members in the union body for a release, or use `partial union`. The conversion is compiler-only (there is no `op_Implicit`), so non-C# callers and reflection see only the constructors.

The `Usage.cs` callers become (compiled and run):

```csharp
public static string Greeting(int id) => Find(id) switch
{
    User user   => $"Hello {user.Name}",
    NotFound nf => $"No user {nf.Id}",
};
```

Optional: `Either<User, NotFound>` is used for exactly one domain concept. A named `public union UserLookup(User, NotFound);` reads better at call sites. Keep the generic `Either` in the library for other uses.

## 2. `Result<T>` → union with an `Error` record

```csharp
public sealed record Error(string Message);

public union Result<T>(T, Error) where T : notnull
{
    public Result<TOut> Map<TOut>(Func<T, TOut> map) where TOut : notnull => this switch
    {
        T v     => map(v),
        Error e => e,
    };

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind) where TOut : notnull => this switch
    {
        T v     => bind(v),
        Error e => e,
    };

    // ToString() is NOT forwarded to the case value by default; keep an override.
    public override string ToString() => this switch
    {
        T v     => $"Success({v})",
        Error e => $"Failure({e.Message})",
        null    => "<empty>",
    };
}
```

- **Wrap the error in a record.** Don't write `union Result<T>(T, string)`, because `Result<string>` would then be ambiguous (CS0457). A dedicated `Error` record avoids that for every `T` except `Result<Error>`, which still collides. I verified that CS0457 too. Pick a name for the error type that nobody will use as a success type.
- **Allocations get better, not worse.** Today every `Result<T>` is a heap object. As a union it's a struct. For reference `T` (e.g. `User`) there is no allocation at all, and for value-type `T` there is one box, about the same as today.
- **`Rename` gets simpler** (compiled and run):

  ```csharp
  public static Result<User> Rename(int id, string newName)
  {
      if (string.IsNullOrWhiteSpace(newName)) return new Error("Name must not be empty");
      return Find(id) switch
      {
          User user   => user with { Name = newName },
          NotFound nf => new Error($"User {nf.Id} not found"),
      };
  }
  ```

- **Breaking changes to plan for:**
  - `Result<T>` changes from a class to a struct, which is binary-breaking.
  - `Result<T>.Failure("...")` becomes `new Error("...")`.
  - `IsSuccess`/`.Value`/`.Error` disappear. You can keep them as body members during migration. Be careful with a property named `Value`: the union already has an `object? Value` property, so pick another name such as `SuccessValue`, or just drop it.
  - `Success(null)` for a reference `T` used to be a success. As a union, a `null` payload is indistinguishable from `default`, and `where T : notnull` now flags it at compile time.
- **Why not a `closed` hierarchy?** A `closed record Result<T>` with `Ok<T>`/`Err<T>` subtypes would also be exhaustive. But it allocates on every result, as today, and the case types wrap foreign `T`s. For generic Result/Either/Option-style wrappers a union is the better fit.

## 3. `Option<T>`: careful, this is the hot path

**Don't do this:**

```csharp
public readonly record struct None;
public union Option<T>(T, None);   // boxes every int/decimal AND every None
```

Shorthand unions store the case in `object? Value`, so value-type cases are boxed. I measured `SumFirstColumn` over 1,000,000 lines, after warm-up, in Release:

| Variant | Bytes allocated per 1M lines |
|---|---|
| Current `Option<T>` (`TryGetValue`) | 0 |
| Custom `[Union]` `Option<T>` (`is int v` or `switch { int v => .., null => .. }`) | 0 per call. A constant ~21 KB showed up in some runs, independent of input size (it didn't grow at 2M lines), so it's runtime noise |
| Shorthand `union Option<T>(T, None)` | **24,000,000** (24 B/call; 48 MB at 2M lines) |

At "tens of millions of calls per file" the shorthand version would add hundreds of MB of garbage per import, and more for `decimal`, whose box is bigger.

**If you want the language integration, keep the struct and make it a custom union.** Your existing type already has the two members the compiler looks for (`HasValue`, `TryGetValue(out T)`). With those present, pattern matching calls them instead of `Value`, and nothing boxes:

```csharp
[Union]                                   // System.Runtime.CompilerServices (implicit usings cover it)
public readonly struct Option<T> : IUnion // IUnion is optional
{
    private readonly T _value;

    public Option(T value) { _value = value; HasValue = value is not null; }  // must be public: one ctor per case

    public bool HasValue { get; }                                       // false => matches `null`
    public object? Value => HasValue ? _value : null;                   // required; must agree with HasValue
    public bool TryGetValue(out T value) { value = _value; return HasValue; }

    public static Option<T> None => default;
    public static Option<T> Some(T value) => new(value);                // now optional
    public T GetValueOrDefault(T fallback) => HasValue ? _value : fallback;
    public override string ToString() => HasValue ? $"Some({_value})" : "None";
}
```

What that gives you (all verified):

- `ParseInt` can be `int.TryParse(text, out var v) ? v : Option<int>.None`, because the implicit `T` to `Option<T>` conversion is generated.
- `if (ParseInt(cell) is int v) sum += v;` and `opt switch { int v => v, null => 0 }` are exhaustive and allocation-free.
- Existing `TryGetValue` call sites keep compiling unchanged, and adding the attribute and interface is non-breaking.

Two things to be aware of:

- **`Some(null)` semantics change.** Today `Option<string>.Some(null).HasValue == true`. In the version above it is `false`, because `HasValue` must agree with `Value`, and a `null` value matches the `null` pattern. The CSV path only uses `int`/`decimal`, so it's unaffected. Check any `Option<SomeReferenceType>` users.
- **The constructor has to become public.** Custom unions need one public single-parameter constructor per case. If you must keep it private, the alternative is a nested `IUnionMembers` interface with `static Create(T)` factories. I did **not** compile that variant, so verify it before you rely on it. Note that inside `Create` you have to write `new((object)value)` to avoid recursing into the union conversion.

Honest assessment: for `Option<T>` this mostly *adds* pattern-matching ergonomics rather than deleting code. If the team is happy with `TryGetValue` in the CSV loop, leaving `Option<T>` exactly as it is is a perfectly good choice. The one thing to rule out is turning it into a shorthand union.

## Smaller notes

- `default(Result<T>)` and `default(Either<,>)` are now an explicit "empty" state (`Value == null`). Switches without a `null` arm throw `SwitchExpressionException` on it. That is better than today's silent "pretend it's T0", but worth knowing. The compiler also warns (CS8655) when flow analysis can see that the value came from `default`.
- If any of these types ever hit System.Text.Json: on .NET 11 a plain union serialises as its bare case value, with no wrapper. You would not need a custom converter. (Not exercised here, because nothing in this library serialises.)
- IDE/LSP note: some C# language servers (e.g. csharp-ls 0.28) still flag `union` as a syntax error. `dotnet build` on the 11.0.100 SDK is the source of truth, and all snippets above build with it.
