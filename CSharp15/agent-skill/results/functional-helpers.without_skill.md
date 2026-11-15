# Review: `Functional` helpers (net11.0 / C# 15)

**Short answer:** yes. C# 15 (the default language version for `net11.0`, and it ships in SDK 11.0.100) has **union types**. Most of the plumbing here can go, **but only `Either` and `Result` should become compiler-generated `union` declarations.** If `Option<T>` became a plain `union`, every `Some(int)` would allocate on the CSV hot path. Instead, keep `Option<T>` as a hand-written struct and turn it into a **custom union** by adding `[Union]` and `IUnion`. It then gets the new pattern-matching syntax and still doesn't allocate.

I checked everything below with the installed SDK (11.0.100) using scratch copies of the code. I did not touch the project files.

---

## 1. How C# 15 unions are built (this drives the recommendations)

`public union Pet(Cat, Dog);` compiles to:

- a `struct` marked `[System.Runtime.CompilerServices.Union]` that implements `IUnion`,
- **one field, `object? Value`**, plus one public constructor per case type,
- an implicit conversion from each case type (that's what `Pet p = new Cat(...)` uses),
- pattern matching (`p switch { Cat c => …, Dog d => … }`) that tests `Value`, with exhaustiveness checking (a missing case gives **CS8509**).

The catch is the single `object? Value` field. **A value-type case gets boxed.** I measured it: 1,000 `union IntOrString(int, string)` values holding an `int` allocated 24,000 bytes (24 B each).

The compiler also accepts **hand-written ("custom") unions**: a type marked `[Union]` that implements `IUnion` (a public `object? Value { get; }`) and has at least one public one-parameter constructor. If that type also has `bool HasValue` and `bool TryGetValue(out TCase)`, the compiler builds patterns from those instead of `Value`. I decompiled `o switch { int i => …, null => … }` and got `if (o.TryGetValue(out var value)) … else if (!o.HasValue) …`, so nothing is boxed.

---

## 2. `Option<T>`: keep it hand-written, make it a custom union

**Do not** replace it with `public union Option<T>(T, None);`. Benchmark: 2 M CSV lines, `ParseInt` per line, Release build:

| Variant | Time | Allocated |
|---|---|---|
| Current hand-written `Option<int>` | 23 ms | **0 MB** |
| `union Option<T>(T, NoneT)` (compiler-generated) | 25 ms | **45.8 MB** (24 B per row, boxing) |
| Hand-written + `[Union]`/`IUnion` (custom union), `is int v` | 23 ms | **0 MB** |
| Same, `switch { int v => v, null => 0 }` | 23 ms | **0 MB** |

`Option<decimal>` would be worse: the box would be about 32 B per cell. The current struct comment ("it must not allocate") already states the requirement. A compiler-generated union breaks it, and a custom union keeps it.

The change is small:

```csharp
using System.Runtime.CompilerServices;

[Union]
public readonly struct Option<T> : IUnion
{
    private readonly T _value;

    public Option(T value) { _value = value; HasValue = true; }   // must be public (see below)

    public bool HasValue { get; }
    public static Option<T> None => default;
    public static Option<T> Some(T value) => new(value);

    public bool TryGetValue(out T value) { value = _value; return HasValue; }

    // Required by IUnion. Boxes for value types: never call it on the hot path.
    public object? Value => HasValue ? _value : null;

    public T GetValueOrDefault(T fallback) => HasValue ? _value : fallback;
    public override string ToString() => HasValue ? $"Some({_value})" : "None";
}
```

What callers gain:

```csharp
public static Option<int> ParseInt(ReadOnlySpan<char> text) =>
    int.TryParse(text, out var value) ? value : Option<int>.None;   // implicit Some

if (ParseInt(cell) is int value) sum += value;                       // lowers to TryGetValue: 0 alloc
var n = opt switch { int v => v, null => 0 };                        // null == None, lowers to !HasValue
```

Things to watch:

- **The constructor has to be public.** With only a private constructor plus `Some(...)` the compiler stops with *CS9385 "A union type must have at least one union creation member"*. A public static `Create(T)` factory didn't work either. So `new Option<int>(5)` and the implicit `Option<int> o = 5;` become part of the public API. That seems fine to me, but it is an API change.
- **`Value` must be a public instance property** (an explicit `IUnion.Value` implementation gives CS9386). Say in a comment that it boxes, so nobody uses it in the importer.
- **Exhaustiveness does not cover `None`.** `o switch { int i => … }` with no `null` arm compiles **without a warning** and throws `SwitchExpressionException` at runtime for `None`. Unions assume a non-null `Value`. Code review has to make sure every switch handles `None` with a `null =>` arm (or uses `TryGetValue` / `GetValueOrDefault`). The current API has the same gap, so this is not a regression, but the new syntax makes the arm easier to forget.
- Some unrelated issues in `Option<T>` that are worth fixing while you're in there: it has no `IEquatable<Option<T>>` / `==`, so any `Equals` call goes through `ValueType.Equals` (reflection and boxing). It also has no `Map`/`Bind`, so callers will keep writing `TryGetValue` by hand. Both are cheap to add.

## 3. `Either<T0, T1>`: delete it, use a `union`

The whole 33-line file becomes:

```csharp
public union Either<T0, T1>(T0, T1);
```

- **No performance regression.** The current struct already stores the value in `object? _value`, so value types were boxed before too. The generated union is smaller: 8 bytes (one reference) versus a reference plus an `int` index.
- `Match`/`Switch`/`IsT0`/`AsT0` are replaced by `switch` expressions and `is` patterns. You also get **exhaustiveness checking**, which the lambda-based `Match` only gave you by convention. Missing arm → CS8509.
- `default(Either<…>)` used to throw "Uninitialised Either" and now throws `SwitchExpressionException`. That's the same behaviour with a different exception type.
- **Semantic difference:** the current type records *which slot* was used (`_index`). A union only matches on the runtime type of `Value`. For `Either<object, string>`, a string goes to whichever arm you list first. `Either<int, int>` was already unusable: the implicit operators were ambiguous, and now the constructors are (CS0457). If any caller depends on the slot index for overlapping types, keep a tagged type for that case. In this codebase (`Either<User, NotFound>`) it doesn't matter.
- If you want a gentler migration, the union can have a body, so you could keep a `Match` method there for a while. I'd just move callers to `switch` instead (there are only two, in `Usage.cs`).

## 4. `Result<T>`: replace it with a `union` (and stop allocating a class)

```csharp
public sealed record Error(string Message);

public union Result<T>(T, Error) where T : notnull
{
    public static Result<T> Success(T value) => value;
    public static Result<T> Failure(string error) => new Error(error);

    public Result<TOut> Map<TOut>(Func<T, TOut> map) where TOut : notnull =>
        this switch { T v => map(v), Error e => e };

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind) where TOut : notnull =>
        this switch { T v => bind(v), Error e => e };

    public override string ToString() =>
        this switch { T v => $"Success({v})", Error e => $"Failure({e.Message})" };
}
```

- Today `Result<T>` is a **sealed class**, so every result is a heap allocation. As a union struct, `Result<User>` costs **no extra allocation** (it just holds the `User` reference). Only value-type `T` (boxed) and the `Error` record allocate.
- The hand-maintained `_value`/`_error`/`IsSuccess` trio and the throwing `Value`/`Error` properties go away. Callers pattern-match instead (`r switch { User u => …, Error e => … }`) and get exhaustiveness checks.
- Unlike `Either`, it's worth keeping `Success`/`Failure`/`Map`/`Bind` as members in the union body, because `Map`/`Bind` are what make `Result` useful.
- **Override `ToString`.** Generated unions don't print their content. Without the override, `Console.WriteLine(result)` prints ``Result`1[User]``.
- **Add `where T : notnull`.** Without it, `this switch { T v => …, Error e => … }` inside the body gives **CS8655** ("null not covered") because an unconstrained `T` can be null. The constraint also matches what `Result` means. It does spread to `Map`/`Bind` (`TOut : notnull`), which is a small breaking change for anyone who maps to a nullable type.
- The error type is now a record (`Error`) instead of a raw `string`. That's a small API break, and a good place to add an error code later. Don't add an implicit conversion from `string`: it would be ambiguous for `Result<string>`.

## 5. `Usage.cs` after the change

```csharp
public static Either<User, NotFound> Find(int id) =>
    Users.TryGetValue(id, out var user) ? user : new NotFound(id);      // unchanged, union conversions

public static Result<User> Rename(int id, string newName)
{
    if (string.IsNullOrWhiteSpace(newName)) return Result<User>.Failure("Name must not be empty");
    return Find(id) switch
    {
        User user         => user with { Name = newName },
        NotFound notFound => new Error($"User {notFound.Id} not found"),
    };
}

public static string Greeting(int id) => Find(id) switch
{
    User user         => $"Hello {user.Name}",
    NotFound notFound => $"No user {notFound.Id}",
};

// CSV hot path
public static Option<int> ParseInt(ReadOnlySpan<char> text) =>
    int.TryParse(text, out var value) ? value : Option<int>.None;
...
if (ParseInt(cell) is int value) sum += value;
```

I built the full rewrite (all four files) in a scratch project. It compiles with **no warnings**, gives the same output as the original (`Hello Ada`, `No user 5`, `Success(User { Id = 2, Name = Torvalds })`, `Failure(User 7 not found)`, `Failure(Name must not be empty)`, CSV sum `41`), and `SumFirstColumn` still allocates **0 bytes** after warm-up. The decompiled `SumFirstColumn` calls `ParseInt(text).TryGetValue(out var value)`, not `.Value`.

---

## Summary of recommendations

| Type | Recommendation | Why |
|---|---|---|
| `Either<T0,T1>` | Replace with `public union Either<T0, T1>(T0, T1);` and use `switch` at call sites | Same boxing as today, gains exhaustiveness checks, removes ~30 lines |
| `Result<T>` | Replace the class with `union Result<T>(T, Error) where T : notnull`; keep `Success`/`Failure`/`Map`/`Bind`/`ToString` in the body | No allocation for reference-type `T`, exhaustive matching, less code |
| `Option<T>` | **Keep it hand-written.** Add `[Union]`, `IUnion`, a public constructor and a public `Value` | A compiler union boxes (+45.8 MB per 2 M rows); the custom union matches with **0 alloc** |
| All | Teach the team: `null =>` is the "None/uninitialised" arm, and it is **not** enforced by exhaustiveness checks | `default(...)`/`None` throws `SwitchExpressionException` at runtime |

Low-priority extras: `IEquatable<Option<T>>` on `Option<T>`; drop the "modelled after OneOf" comment once `Either` is a language union.
