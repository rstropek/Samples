# C# 15 — Union Types (Discriminated Unions)

> Everything here was built and run against .NET 11 SDK
> `11.0.100-preview.5.26302.115` with `<LangVersion>preview</LangVersion>` (June 2026). Every
> code block compiles; every IL/lowering listing came out of `ilspycmd` run against the
> `06_Lowering` project, not from memory.

---

## 0. The one-paragraph version

C# 15 finally gives us union types (discriminated unions / sum types) as a real language
feature, after roughly five years of design work in `dotnet/csharplang`. The whole idea fits on
one line:

```csharp
public union Pet(Cat, Dog, Bird);
```

[Sample Code](../src/01_Basics/Program.cs#L19)

That declares a value that is exactly one of `Cat`, `Dog`, or `Bird`. What I get out of it:

- implicit conversions from each case type into the union,
- an exhaustive `switch` — the compiler checks I handled every case, so no `default` arm,
- pattern matching that "unwraps" the union down to the underlying case value.

The thing I want to land in the talk: this is not a new runtime concept. A `union` declaration
lowers to an ordinary `struct` holding an `object?`. The lowering is the most interesting part,
so I've built the whole talk around getting there.

---

## 1. Why I care about this

Before C# 15 I modelled "one of N types" with workarounds, and each one cost me something:

- **Abstract base + sealed subclasses** — No exhaustiveness guarantee; `switch` needs a `default`/throw; the hierarchy is open unless you fight the language.
- **`object` + `is` checks** — No type safety, no exhaustiveness, easy to forget a case.
- **Nullable fields on one big class** — "Illegal states" are representable (two fields set at once); invariants live in comments.
- **`OneOf<T0,T1,...>` NuGet library** — Positional (`.IsT0`), no real pattern integration, generic noise, third-party dependency.
- **F# `type Shape = Circle | Rectangle`** — Great, but only in F#.

Unions bring the F#/Rust/TypeScript ergonomics — a closed set of shapes plus an exhaustive match
— into C#, and they plug straight into the pattern-matching engine we already have.

One framing I want to repeat from the proposal: the C# feature is a union _of types_. It's
"type unions". "Discriminated unions" fall out of that by using fresh case types (usually
`record` types) as the cases.

---

## 2. Feature walkthrough (first part of the talk)

### 2.1 Declaring a union

The shorthand declaration form:

```csharp
public record class Cat(string Name);
public record class Dog(string Name);
public record class Bird(string Name, bool CanFly);

public union Pet(Cat, Dog, Bird);   // <-- the headline syntax
```

[Sample Code](../src/01_Basics/Program.cs#L14)

Case types can be anything; I reach for `record class` because it keeps the demos short and
hands me value equality and deconstruction for free.

### 2.2 Implicit conversions — a `Dog` _is_ a `Pet`

```csharp
Pet a = new Dog("Rex");        // no wrapper, no .From(...), just assignment
Pet b = new Cat("Whiskers");
```

[Sample Code](../src/01_Basics/Program.cs#L26)

### 2.3 Exhaustive `switch` — no `default` needed

```csharp
string Describe(Pet pet) => pet switch
{
    Cat c  => $"Cat {c.Name}",
    Dog d  => $"Dog {d.Name}",
    Bird b => $"Bird {b.Name}",
};   // compiler is satisfied: all three case types covered
```

Drop a case and the compiler complains:

```text
warning CS8509: The switch expression does not handle all possible values of its input type
(it is not exhaustive). For example, the pattern 'Bird' is not covered.
```

Worth flagging on stage: exhaustiveness comes back as CS8509, a _warning_, not an error. If I
want a hard gate I set `<WarningsAsErrors>CS8509</WarningsAsErrors>`. The reason it can only be a
warning shows up in the deep dive — it's the default/empty union.

Second thing to flag: depending on the exact preview and your nullable settings, the compiler
may instead emit CS8655 ("does not handle some null inputs") for a union switch, because a
`default`/uninitialised union has `null` content. The fix I settled on is an explicit `null`
arm. It documents the empty case and still leaves CS8509 firing if I later add a real case type:

```csharp
string Describe(Pet pet) => pet switch
{
    Cat c  => $"Cat {c.Name}",
    Dog d  => $"Dog {d.Name}",
    Bird b => $"Bird {b.Name}",
    null   => "<uninitialised pet>",   // handles default(Pet); silences CS8655
};
```

[Sample Code](../src/01_Basics/Program.cs#L55)

I prefer `null =>` over a `_ =>` discard — the discard would quietly swallow any future case
type and defeat the exhaustiveness check. The sample projects all use `null` arms for this
reason. The one exception is `06_Lowering`, where I deliberately keep the hole (with
`#pragma warning disable CS8655`) so the IL still shows the `ThrowSwitchExpressionException`
safety net.

### 2.4 Pattern matching reaches _through_ the union

Every pattern form applies to the unwrapped case value — type, property, positional, relational,
logical, `when` guards, tuples-of-unions, all of it:

```csharp
pet switch
{
    Bird { CanFly: true } b => $"{b.Name} flies",   // property pattern on the case
    Bird b                  => $"{b.Name} grounded",
    _                       => "not a bird",
};
```

[Sample Code](../src/05_PatternShowcase/Program.cs#L16)

### 2.5 Generic unions

Type parameters work, including more than one:

```csharp
public record class Some<T>(T Value);
public sealed record class None;          // shared empty marker
public union Option<T>(Some<T>, None);

public record class Ok<T>(T Value);
public record class Error<E>(E Failure);
public union Result<T, E>(Ok<T>, Error<E>);   // two type parameters
```

[Sample Code — Option](../src/02_ResultOption/Option.cs#L18) · [Result](../src/02_ResultOption/Result.cs#L10)

This is what lets me build the `Option`/`Result` library in `src/02_ResultOption`.

### 2.6 Helpers via extension _blocks_ (C# extension members)

I wrote the `Option`/`Result` combinators with extension blocks instead of classic
`this`-parameter extension methods. One `extension(...)` block groups members over a receiver and
can expose extension _properties_ as well as methods, which fits union helpers nicely:

```csharp
public static class OptionExtensions
{
    extension<T>(Option<T> option)
    {
        public bool IsSome => option is Some<T>;                 // extension property

        public Option<U> Map<U>(Func<T, U> f) => option switch    // extension method
        {
            Some<T> s => new Some<U>(f(s.Value)),
            None n    => n,
            null      => None.Instance,                          // empty stays empty
        };
    }
}
```

[Sample Code](../src/02_ResultOption/Option.cs#L38)

Call sites don't change (`opt.IsSome`, `opt.Map(...)`), and the members resolve across assembly
boundaries — the xUnit project consumes them straight from the referenced library.

### 2.7 Giving a union its own functions (methods, properties, `ToString`)

A union can carry its own members — methods, computed properties, a custom `ToString()`, static
factories. There are three ways to do it, and I checked all three (see `src/11_UnionWithMembers`):

```csharp
// (1) Shorthand declaration WITH a body — the common case
public union Shape(Circle, Rectangle, Triangle)
{
    public double Area => this switch          // <-- switch over `this`, NOT Value
    {
        Circle c    => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        Triangle t  => 0.5 * t.Base * t.Height,
        null        => 0,                       // the default/empty union
    };

    public Shape Scaled(double f) => this switch { /* ... */ Circle c => new Circle(c.Radius*f), _ => this };

    public override string ToString() => this switch  // fixes the "ToString not forwarded" gotcha
    {
        Circle c    => $"Circle(r={c.Radius})",
        Rectangle r => $"Rectangle({r.Width}x{r.Height})",
        Triangle t  => $"Triangle(b={t.Base}, h={t.Height})",
        null        => "Shape(<uninitialised>)",
    };

    public static Shape Unit => new Circle(1);
}

// (2) `partial union` — keep declaration and logic in separate files
public partial union Temperature(Celsius, Fahrenheit);
public partial union Temperature
{
    public double AsCelsius => this switch { Celsius c => c.Degrees, Fahrenheit f => (f.Degrees-32)*5/9, null => double.NaN };
    public override string ToString() => $"{AsCelsius:F1} °C";
}

// (3) Fully explicit — ANY type carrying [Union] + IUnion is a union, members and all
[Union]
public struct HttpOutcome : IUnion
{
    public object? Value { get; }
    public HttpOutcome(Ok ok)    => Value = ok;
    public HttpOutcome(Fail err) => Value = err;
    public bool IsSuccess => Value is Ok;
    public override string ToString() => this switch { Ok o => $"OK {o.Code}", Fail f => $"FAIL {f.Reason}", null => "<empty>" };
}
```

[Sample Code — Shape (body)](../src/11_UnionWithMembers/Shape.cs#L18) · [Temperature (partial)](../src/11_UnionWithMembers/Temperature.cs#L11) · [HttpOutcome (explicit)](../src/11_UnionWithMembers/ExplicitUnion.cs#L14)

The gotcha I tripped over here, and the one to call out: inside the body, switch over `this`, not
`Value`. Switching over `this` makes the compiler apply union exhaustiveness, because the case
types are a closed set. Switching over the raw `Value` property — which is typed `object?` — is
not exhaustive and warns CS8509 ("the pattern 'not null' is not covered"), because `object?` is
open. So: `this switch { ... }`.

The implicit conversions and external pattern matching behave the same whether I use the
shorthand, a `partial union`, or a hand-written `[Union]` struct.

---

## 3. Deep dive — how unions are actually implemented (second part of the talk)

This is the section I most want to get right. A `union` declaration is compiler sugar over a
plain `struct`; there's no new CLR type kind behind it.

### 3.1 What `union Shape(Circle, Rectangle)` lowers to

Source (`src/06_Lowering/Unions.cs`):

```csharp
public record class Circle(double Radius);
public record class Rectangle(double Width, double Height);
public union Shape(Circle, Rectangle);
```

[Sample Code](../src/06_Lowering/Unions.cs#L7)

Decompiled with `ilspycmd 06_Lowering.dll -t Lowering.Shape`:

```csharp
using System.Runtime.CompilerServices;

[Union]
public struct Shape : IUnion
{
    public object? Value { get; }

    [CompilerGenerated] public Shape(Circle value)    { Value = value; }
    [CompilerGenerated] public Shape(Rectangle value) { Value = value; }
}
```

So the "opinionated" shorthand generates:

- a `struct` (value type),
- decorated with `[Union]`,
- implementing `IUnion`,
- storing the payload in a single `object? Value` auto-property,
- with one constructor per case type.

### 3.2 The runtime contract

`ilspycmd` against `System.Private.CoreLib.dll` shows the two new BCL types (forwarded via
`System.Runtime.dll`):

```csharp
namespace System.Runtime.CompilerServices;

public interface IUnion
{
    object? Value { get; }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class UnionAttribute : Attribute { }
```

History note in case someone asks: in Preview 2 these types didn't ship in the runtime, so you
had to declare `IUnion`/`UnionAttribute` yourself. As of Preview 5 (what I'm running) they're in
the BCL, so the samples compile with no extra plumbing.

### 3.3 The implicit conversion is just a constructor call

`Shape MakeCircle(double r) => new Circle(r);` decompiles to:

```csharp
public static Shape MakeCircle(double r) => new Shape(new Circle(r));
```

[Sample Code (source)](../src/06_Lowering/Unions.cs#L45)

IL (`ilspycmd ... -il`):

```text
IL_0000: ldarg.0
IL_0001: newobj  instance void Lowering.Circle::.ctor(float64)
IL_0006: newobj  instance void Lowering.Shape::.ctor(class Lowering.Circle)
IL_000b: ret
```

No `op_Implicit` operator is emitted — the compiler just drops in the matching constructor call
at the conversion site.

### 3.4 The exhaustive `switch` is an `isinst` chain + a safety throw

`double Area(Shape) => shape switch { Circle c => …, Rectangle r => … };` decompiles to:

```csharp
public static double Area(Shape shape)
{
    object value = shape.Value;
    if (value is Circle circle)        return Math.PI * circle.Radius * circle.Radius;
    if (value is Rectangle rectangle)  return rectangle.Width * rectangle.Height;
    <PrivateImplementationDetails>.ThrowSwitchExpressionException(shape);   // unreachable if non-default
    return default;
}
```

[Sample Code (source)](../src/06_Lowering/Unions.cs#L38)

IL (abridged) — note `get_Value`, the `isinst` ladder, and the fallback throw:

```text
IL_0002: call      instance object Lowering.Shape::get_Value()
IL_0009: isinst    Lowering.Circle
IL_0010: brtrue.s  IL_001e
IL_0013: isinst    Lowering.Rectangle
IL_001a: brtrue.s  IL_0038
IL_001c: br.s      IL_0048
...
IL_0048: ldarg.0
IL_0049: box       Lowering.Shape
IL_004e: call      void '<PrivateImplementationDetails>'::ThrowSwitchExpressionException(object)
```

### 3.5 Consequences worth talking through

1. The "empty" / `default` union is real, and it bites. Because the generated type is a `struct`,
   `default(Shape)` is always constructible and has `Value == null`. None of the `isinst` checks
   match, so the exhaustive switch hits `ThrowSwitchExpressionException` at runtime. That's
   exactly why exhaustiveness is CS8509 (a warning) and not an error — the compiler can't forbid
   the default value of a struct. Verified:

   ```csharp
   Result<int> def = default;          // legal
   Console.WriteLine(def.Value is null);   // True   -> no case selected
   ```

   On stricter previews this same hole is what surfaces as CS8655 on a union switch; add a `null`
   arm (see §2.3) to handle it explicitly.

2. Value-type cases box. Storage is `object?`, so a `struct` case type gets boxed on conversion.
   I lean on `record class` cases (reference types) when allocations matter, or accept one boxing
   allocation per value.

3. `ToString()` is not forwarded to the underlying value. A union prints its struct type name,
   not the case's `ToString()`:

   ```text
   ToString of union: Result`1[System.Int32]      // NOT "Ok { Value = 7 }"
   ```

   The fix is to give the union its own `ToString()` (see §2.7 and `src/11_UnionWithMembers`) by
   switching over `this`. The shorthand `union X(...) { ... }`, a `partial union`, and an explicit
   `[Union]` struct all support custom members.

4. No `op_Implicit`, so no implicit interface/overload tricks — conversions only fire at the
   syntactic conversion sites the compiler recognises.

5. Preview limitations (Preview 5): some proposal pieces aren't implemented yet — notably _union
   member providers_ (custom storage/representation) and the full _explicit union pattern_. The
   shorthand `union X(...)` form is the supported path today.

---

## 4. Sample catalogue (third part — runnable demos)

All projects live under `src/`, target `net11.0`, and share settings via `Directory.Build.props`
(`LangVersion=preview`). I kept each one small. Outputs below are the actual runs.

### `01_Basics` — console

The canonical `Pet` union: implicit conversion, exhaustive switch, unions in collections,
property patterns, the `.Value` escape hatch.

```text
Dog named Rex
Cat named Whiskers
Bird named Tweety (flies)
Shelter holds 4 pets, 2 of them dogs.
Tweety can fly away!
Underlying runtime type of `a`: Dog
```

[Sample Code](../src/01_Basics/Program.cs#L19)

### `02_ResultOption` — class library + xUnit tests

A small functional core: `Option<T>` (`Some<T>`/`None`) and `Result<T,E>` (`Ok<T>`/`Error<E>`),
with `Map`, `Bind`, `Match`, `GetValueOrDefault`, written using extension blocks (§2.6). Shows
generic unions, a shared non-generic `None` marker converting into any `Option<T>`, and
railway-oriented composition. Covered by 10 passing xUnit tests in `tests/Csharp15.Tests`.

[Sample Code — Option](../src/02_ResultOption/Option.cs#L18) · [Result](../src/02_ResultOption/Result.cs#L10)

### `10_ResultOptionUsage` — console (consumes `02_ResultOption`)

Real-world use of the library: `Option<T>` for dictionary lookups (no `TryGetValue` out-params
leaking), and a `Result<T,E>` validation pipeline composed with `Bind`/`Map`/`Match`
(railway-oriented programming).

```text
== (A) Option: dictionary lookup ==
coffee  -> 4.20
water   -> not on the menu

== (B) Result: validation pipeline ==
  alice@example.com / 30         => OK   -> alice@example.com, age 30
bob@ / 30                        => FAIL -> invalid email 'bob@'
carol@example.com / 999          => FAIL -> age 999 out of range
```

[Sample Code](../src/10_ResultOptionUsage/Program.cs#L43)

### `03_RecursiveExpr` — console

A recursive union — an arithmetic AST `Expr(Num, Add, Sub, Mul, Div, Neg)` — with two folds over
the same union (evaluate + pretty-print). The textbook discriminated-union use case.

```text
((1 + 2) * -3) = -9
((3 * 4) + (10 / 2)) = 17
```

[Sample Code](../src/03_RecursiveExpr/Program.cs#L15)

### `04_DomainModeling` — console

Make illegal states unrepresentable: an `OrderState(Draft, Placed, Shipped, Delivered, Cancelled)`
where each state carries only its valid data, plus an exhaustive `Advance` transition function.

```text
0: Draft with 2 item(s)
1: Placed on 06/15/2026
2: Shipped, tracking TRK-20260615-001
3: Delivered on 06/18/2026
```

[Sample Code](../src/04_DomainModeling/Program.cs#L15)

### `05_PatternShowcase` — console

A tour of every pattern form through a union (`Shape(Circle, Rectangle, Triangle)`): type,
relational, property, `when` guards, and switching over a tuple of unions.

```text
degenerate circle            area=0.00
tiny circle r=0.5            area=0.79
circle r=3                   area=28.27
square 4x4                   area=16.00
rectangle 4x6                area=24.00
triangle area=12             area=12.00
two circles
mixed pair
```

[Sample Code](../src/05_PatternShowcase/Program.cs#L11)

### `06_Lowering` — class library (+ `decompile.sh`)

The types behind §3. Run `src/06_Lowering/decompile.sh` to regenerate the decompiled C# and the
switch IL live, if I want to do it from scratch on stage.

[Sample Code](../src/06_Lowering/Unions.cs#L11) · [decompile.sh](../src/06_Lowering/decompile.sh)

### `11_UnionWithMembers` — console

A union carrying its own functions: a `Shape` union with a custom `ToString()`, an `Area`
computed property, and a `Scaled()` method (shorthand-with-body); a `Temperature` declared as a
`partial union`; and an explicit `[Union] struct HttpOutcome`. Covers all three ways to add
members, and the "switch over `this`, not `Value`" rule (§2.7).

```text
Circle(r=2)                area=12.57  degenerate=False
Rectangle(3x4)             area=12.00  degenerate=False
Circle(r=2) scaled x3 -> Circle(r=6)
98.6 °F body temp -> 37.0 °C
OK 200 (success=True)
```

[Sample Code — Shape](../src/11_UnionWithMembers/Shape.cs#L18) · [Temperature](../src/11_UnionWithMembers/Temperature.cs#L11) · [HttpOutcome](../src/11_UnionWithMembers/ExplicitUnion.cs#L14)

### `07_CollectionExprArgs` — console (bonus feature, see §6)

```text
List: count=3, capacity=6
HashSet (OrdinalIgnoreCase): count=1
SortedSet (reverse): [3, 2, 1]
Merged: [0, 1, 2, 3, 4] capacity=16
```

[Sample Code](../src/07_CollectionExprArgs/Program.cs#L12)

---

## 5. How it compares

- **Declaration** — C# 15: `union Pet(Cat,Dog)`; F#: `type Pet = Cat | Dog`; OneOf: `OneOf<Cat,Dog>`; sealed hierarchy: abstract + sealed.
- **Exhaustiveness** — C# 15: yes, a CS8509 warning; F#: yes, an error; OneOf: no; sealed hierarchy: only with sealed + warning.
- **Pattern integration** — C# 15: full unwrap; F#: yes; OneOf: positional `.IsT0`; sealed hierarchy: yes.
- **Named cases by type** — C# 15: yes; F#: yes; OneOf: no (index based); sealed hierarchy: yes.
- **Runtime cost** — C# 15: one ref field, boxes value-type cases; F#: tag + fields; OneOf: wrapper object; sealed hierarchy: natural.
- **Dependency** — C# 15: in-box (.NET 11); F#: F# only; OneOf: third-party; sealed hierarchy: none.

The nuance I want to close on: C# unions are unions _of types_, not tagged unions. If two cases
would be the same type, give them distinct case types. That's the part that differs from F#'s
name-tagged cases.

### Will `Result`/`Option` ship in the .NET BCL?

Short answer, as of June 2026: no confirmed plans. The team scoped this feature as the
language/runtime mechanism for unions (the `union` keyword, `UnionAttribute`, `IUnion`), not as a
set of blessed `Option`/`Result` types in the BCL. Mads Torgersen's design write-ups and the
`dotnet/csharplang` discussions deliberately land on "unions of types" and leave the concrete
domain types (Option/Result) to libraries and users — which is why this repo defines its own.

If the Rust comparison comes up (it will):

- Rust ships `Option<T>`/`Result<T,E>` in its standard library; C# 15 ships the building blocks
  so you (or a NuGet package) can declare them in one line each.
- Mature third-party options already exist (`OneOf`, `FxKit`, various source generators); the
  language feature mostly removes their boilerplate and gives real pattern/exhaustiveness support.
- A future BCL `Option`/`Result` isn't ruled out, but it would be a separate proposal and isn't
  part of the C# 15 / .NET 11 deliverable. I'll treat any such claim as speculation.

---

## 6. Bonus C# 15 feature — Collection expression arguments

A `with(...)` element (it has to come first) forwards arguments to the target collection's
constructor/factory — capacity, comparers, and so on.

```csharp
string[] values = ["one", "two", "three"];

List<string> names    = [with(capacity: values.Length * 2), .. values];
HashSet<string> set   = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"]; // count 1
SortedSet<int> desc   = [with(Comparer<int>.Create((x, y) => y.CompareTo(x))), 3, 1, 2];      // 3,2,1
List<int> merged      = [with(capacity: 16), 0, .. Enumerable.Range(1, 3), 4];
```

[Sample Code](../src/07_CollectionExprArgs/Program.cs#L12)

One caveat I hit: dictionary collection-expressions still need a single-argument `Add`, so
`[with(comparer), KeyValuePair.Create(...)]` does not compile in Preview 5 (CS9215).

---

## 7. Demo running order (pick from these depending on time)

1. Hook (2 min): show the `OneOf`/nullable-fields pain, then the one-line `union`.
2. Basics (5 min): `01_Basics` — implicit conversion + exhaustive switch; delete a case to
   trigger CS8509 live.
3. The reveal (10 min): `06_Lowering` + `decompile.sh` — "it's just a struct holding `object?`";
   walk the `isinst` IL ladder and the `ThrowSwitchExpressionException`.
4. The gotcha (5 min): `default(union)` → runtime throw; why exhaustiveness is a warning; show
   `WarningsAsErrors`.
5. Real use (10 min): `02_ResultOption` + `10_ResultOptionUsage` (Option/Result, extension blocks,
   validation pipeline) and `03_RecursiveExpr` (AST). Optionally `04_DomainModeling`.
6. Patterns (5 min): `05_PatternShowcase`, tuple-of-unions.
7. Bonus (3 min): collection expression arguments.
8. Wrap (2 min): comparison table, preview caveats, "type unions vs tagged unions".

---

## 8. Reproduce everything

```bash
# .NET 11 preview SDK + an IL decompiler
dotnet tool install -g ilspycmd

dotnet build  CSharp15.slnx -c Release      # 0 warnings, 0 errors
dotnet test   tests/Csharp15.Tests          # Passed! Failed: 0, Passed: 10
src/06_Lowering/decompile.sh                # regenerate the IL listings in §3
```

## 9. References

- What's new in C# 15 — https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15
- Union types (language reference) — https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/union
- Unions feature specification — https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/unions
- Proposal & spec source — https://github.com/dotnet/csharplang/blob/main/proposals/unions.md
- Discriminated unions working group — https://github.com/dotnet/csharplang/tree/main/meetings/working-groups/discriminated-unions
- Collection expression arguments — https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions#collection-expression-arguments
