# Review: `Functional` helpers (net11.0 / C# 15)

## Summary

C# 15 (the default language version for `net11.0`) adds **union types**. That changes things for two of your three helpers:

| Type | Today | Recommendation |
|---|---|---|
| `Either<T0,T1>` | Hand-written OneOf clone, `object?` + `int` index | **Replace** with `public union Either<T0, T1>(T0, T1);` or, better, a named union per use (`union FindUserResult(User, NotFound);`). |
| `Result<T>` | `sealed class` with `T?`/`string?` fields | **Replace** with `public union Result<T>(T, Error)` and keep `Map`/`Bind` as members. Note that this is a breaking change from a class to a struct. |
| `Option<T>` | Non-allocating `readonly struct` | **Do not** turn it into a `union` declaration, because that boxes. Either keep it as it is, or opt it in as a *custom* union (`[Union]` plus the non-boxing access pattern) so callers get pattern matching at zero allocation cost. |

I checked every claim below with the 11.0.100 SDK in scratch projects (allocation counts from `GC.GetAllocatedBytesForCurrentThread`, IL lowering via `ilspycmd`) and against the C# docs on unions (learn.microsoft.com, *builtin-types/union* and the *csharp-15.0/unions* proposal).

---

## 1. `Either<T0,T1>`: delete it

What the language now gives you:

```csharp
public union Either<T0, T1>(T0, T1);          // drop-in generic replacement
// or, preferably, give the domain concept a name:
public union FindUserResult(User, NotFound);
```

The compiler generates a struct marked `[Union]` that implements `IUnion`. It has one constructor and one implicit conversion per case and an `object? Value`. This is the same storage strategy as your hand-written type (one `object?` field), so performance is the same or slightly better, because there is no `int` index field.

What you gain:

- **Exhaustive `switch` instead of `Match`/`Switch`.** A missing case produces `CS8509 … the pattern 'NotFound' is not covered`, which I verified. `Match(Func, Func)` is gone, and with it the delegates. `UserDirectory.Rename` currently allocates a closure plus a delegate on every call because its lambda captures `newName`. A `switch` expression allocates nothing:
  ```csharp
  public static Result<User> Rename(int id, string newName) =>
      string.IsNullOrWhiteSpace(newName) ? new Error("Name must not be empty")
      : Find(id) switch
        {
            User user        => user with { Name = newName },
            NotFound notFound => new Error($"User {notFound.Id} not found"),
        };
  ```
- **No more `IsT0`/`AsT0`/throwing accessors.** Use `if (r is User u)`.
- The target-typed conditional in `Find` (`… ? user : new NotFound(id)`) keeps working through union conversions. I verified this.

**Existing bug this fixes:** in the hand-written `Either`, `_index` defaults to `0`, so `default(Either<User,NotFound>)` reports `IsT0 == true` and `Match` calls `f0(null)`. The `"Uninitialised Either"` branches can never run. With the generated union, `default` has `Value == null`. A switch without a `null` arm throws `SwitchExpressionException` (verified), and you can handle it explicitly with a `null =>` arm.

Things to know:
- Value-type cases are **boxed**, exactly as they are today (`(T0)_value!` already unboxes).
- `Either<int,int>` is still unusable (`CS0457` ambiguous conversion). Your current type has the same limitation.
- The compiler does **not** warn about a missing `null` arm when no case type is nullable, even though `default` can still occur in fields and arrays. This is by design (per the docs).

## 2. `Result<T>`: replace it with a union

```csharp
public sealed record Error(string Message);

public union Result<T>(T, Error)
{
    public static Result<T> Success(T value) => value;          // keeps existing call sites compiling
    public static Result<T> Failure(string message) => new Error(message);

    public Result<TOut> Map<TOut>(Func<T, TOut> map) =>
        this switch { T v => map(v), Error e => e };
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind) =>
        this switch { T v => bind(v), Error e => e };
}
```
(`Map` with a union body compiled and ran as expected in my scratch project.)

Why:
- Today every `Result<T>` is a heap object. As a union struct, `Result<User>` (reference `T`) allocates **nothing** for success. Propagating a failure in `Map`/`Bind` reuses the same `Error` instance instead of allocating a new `Result`.
- `Value`/`Error` properties that throw are replaced by exhaustive pattern matching.
- The error needs its own case type (`Error`, or `Exception` if you prefer). `union Result<T>(T, string)` would be ambiguous for `Result<string>`.

Costs and caveats:
- **Breaking change:** class → struct. `Result<T>?` null checks, reference equality and binary compatibility change for consumers. `default(Result<T>)` becomes possible and has `Value == null`.
- `Result<int>` boxes the `int`. That is still no worse than today's allocation per result.
- The generated `ToString()` prints only the type name (`Result`1[System.Int32]`). Add `public override string ToString() => …` in the body if you rely on `Success(..)`/`Failure(..)` in logs.

## 3. `Option<T>`: keep the struct, and opt in as a custom union only if you want patterns

### Don't use a `union` declaration here
`union Option<T>(T, None)` or anything similar boxes value-type cases. Measured: **24 bytes per `Option<int>`** (24,000 B per 1,000 values). For "tens of millions of calls per file" in `SumFirstColumn`, that turns a zero-allocation loop into hundreds of MB of gen0 garbage per file. The docs say this outright: generated unions "always box value-type cases". Your current struct is correct for the hot path, and nothing in C# 15 makes it obsolete.

### Optional: make the existing struct a *custom* union
The type already exposes exactly the members of the **non-boxing access pattern** (`bool HasValue`, `bool TryGetValue(out T)`). Adding `[Union]`, `: IUnion`, a public constructor and `Value` turns it into a union type without changing its layout:

```csharp
using System.Runtime.CompilerServices;

[Union]
public readonly struct Option<T> : IUnion
{
    private readonly T _value;
    public Option(T value) { _value = value; HasValue = true; }   // was private
    public bool HasValue { get; }
    public object? Value => HasValue ? _value : null;              // boxes, but only if someone calls it
    public bool TryGetValue(out T value) { value = _value; return HasValue; }
    // None, Some, GetValueOrDefault, ToString unchanged
}
```

Callers can then write `if (ParseInt(cell) is int v)` or `opt switch { int v => …, null => … }`. I decompiled the result: `is int v` lowers to `o.TryGetValue(out var v)` and `null` lowers to `!o.HasValue`. Measured: **0 bytes allocated across 300k calls**, the same as the current code. Without `TryGetValue`, the same patterns go through `Value` and allocate 24 B/call, so that method must never be removed or renamed. `SumFirstColumn` can stay exactly as it is.

Pitfalls I hit while trying this, and why it is optional rather than a must-do:

1. **The silent `Some(default)` trap.** Once a union conversion from `T` exists, `int.TryParse(s, out var v) ? v : default` compiles to `Some(0)`, not `None`, because `default` is typed as `int`. I reproduced this: `"x"` parsed to `Some(0)`. Keep writing `Option<int>.None` explicitly, as `ParseInt` does now. This is a real regression risk if people start relying on the new implicit conversion.
2. **No exhaustiveness for "None".** With a single case type `T` that isn't nullable, the compiler treats `Value` as not-null and does **not** warn when the `null` arm is missing (verified, no diagnostic). `opt switch { int v => v }` compiles cleanly and throws `SwitchExpressionException` on `None`. You can get a warning by adding an explicit `None` case type (`Option(None _)` constructor plus `TryGetValue(out None)`), which I verified produces `CS8509 … 'None' is not covered`. But the pattern contract requires `HasValue` to mean "Value is not null", so `HasValue` would become "always true", and that silently changes the meaning of your existing public `HasValue` for every caller. Not worth it.
3. **`Some(null)` contract violation.** For `Option<string?>`, `Some(null)` has `HasValue == true` but `Value == null`. That breaks the documented "access pattern consistency" rule (`HasValue` ⇔ `Value != null`). In my test, `on is null` returned `false` because the compiler trusts `HasValue`. Either add `where T : notnull` or document that `Some(null)` is not allowed.
4. The interface `Value` property boxes. Keep it out of the hot path, or consider an analyzer or banned-API entry for `Option<T>.Value`.

**My call:** leave `Option<T>` as it is unless you actively want `is int v` syntax at call sites. If you do adopt it, add `where T : notnull`, keep `TryGetValue`, and don't use `? v : default`.

---

## Smaller notes on the current code
- `Either.Match`/`Switch` and `Result.Match` take delegates. Even if you keep the types, a `switch` over the union avoids delegate allocation for capturing lambdas.
- `Result<T>.Value`/`Error` throw on misuse. Unions remove that whole class of bug.
- `Option<T>.ToString()` is fine. Note that a generated union would lose such overrides unless you add them back in the body.

## Suggested order
1. Replace `Either` with named unions (`FindUserResult`). This is low risk, and `UserDirectory` is the only consumer here.
2. Move `Result<T>` to `union Result<T>(T, Error)`, keeping the `Success`/`Failure` factories as members to limit churn. Flag it as a breaking change.
3. Leave `Option<T>` as is, or apply the custom-union opt-in with the guards above. Either way, check with a BenchmarkDotNet `[MemoryDiagnoser]` run on `SumFirstColumn` before and after.
