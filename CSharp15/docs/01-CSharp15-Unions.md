# C# 15 — Union Types (Discriminated Unions)

> Originally built against .NET 11 SDK `11.0.100-preview.5.26302.115` (June 2026); re-verified
> against the .NET 11 GA SDK `11.0.100` (November 2026). C# 15 is the default language version
> for `net11.0`, so no `<LangVersion>preview</LangVersion>` is needed anymore. Every code block
> compiles; every IL/lowering listing came out of `ilspycmd` run against the `06_Lowering`
> project, not from memory (the lowering is unchanged between Preview 5 and GA).

---

## 0. The one-paragraph version

C# 15 finally gives us union types (discriminated unions / sum types) as a real language
feature, after roughly five years of design work in `dotnet/csharplang`. The whole idea fits on
one line:

```csharp
public readonly union Pet(Cat, Dog, Bird);
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

- **Abstract base + sealed subclasses** — No exhaustiveness guarantee; `switch` needs a `default`/throw; the hierarchy is open unless you fight the language. [Sample Code](../src/01_Basics/Alternatives.cs#L29)
- **`object` + `is` checks** — No type safety, no exhaustiveness, easy to forget a case. [Sample Code](../src/01_Basics/Alternatives.cs#L61)
- **Nullable fields on one big class** — "Illegal states" are representable (two fields set at once); invariants live in comments. [Sample Code](../src/01_Basics/Alternatives.cs#L90)
- **`OneOf<T0,T1,...>` NuGet library** — Positional (`.IsT0`), no real pattern integration, generic noise, third-party dependency. [Sample Code](../src/01_Basics/Alternatives.cs#L125)
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

public readonly union Pet(Cat, Dog, Bird);   // <-- the headline syntax
```

[Sample Code](../src/01_Basics/Program.cs#L14)

Case types can be any type that converts to `object` — classes, structs, interfaces, type
parameters, other unions (not `ref struct`s like `Span<T>`: CS9371). I reach for `record class` because it keeps the demos short and
hands me value equality and deconstruction for free.

Why `readonly union`? A union is a `struct` under the hood (§3.1), and its only state is the
get-only `Value`, so the IDE suggests IDE0250 "Struct can be made 'readonly'". The modifier
works on the shorthand (and on `partial union`). It lowers to a `readonly struct`, saves
defensive copies on `in` parameters and `readonly` fields, and makes every member in the body
implicitly `readonly`. All samples in this repo use it. Plain `union Pet(...)` works just as well.

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
want a hard gate I set `<WarningsAsErrors>CS8509</WarningsAsErrors>`. That's simply consistent
with every other non-exhaustive C# switch expression (also a warning) — unions don't change the
policy.

Second thing to flag: the `default`/uninitialised union has `null` content. In C# 15 GA the
compiler does _not_ demand a `null` arm when all case types are non-nullable — such a switch
compiles warning-free and simply throws `SwitchExpressionException` for `default(Pet)`. CS8655
("does not handle some null inputs") only appears if a case type is nullable, e.g.
`union NullablePet(Cat?, Dog, Bird)`. I still like an explicit `null` arm where an empty union
is plausible: it documents the empty case and still leaves CS8509 firing if I later add a real
case type:

```csharp
string Describe(Pet pet) => pet switch
{
    Cat c  => $"Cat {c.Name}",
    Dog d  => $"Dog {d.Name}",
    Bird b => $"Bird {b.Name}",
    null   => "<uninitialised pet>",   // handles default(Pet) instead of throwing
};
```

[Sample Code](../src/01_Basics/Program.cs#L58)

I prefer `null =>` over a `_ =>` discard — the discard would quietly swallow any future case
type and defeat the exhaustiveness check. The sample projects all use `null` arms for this
reason (the Learn docs give the same advice). The one exception is `06_Lowering`, where I
deliberately keep the hole so the IL still shows the `ThrowSwitchExpressionException` safety net.

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
public readonly union Option<T>(Some<T>, None);

public record class Ok<T>(T Value);
public record class Error<E>(E Failure);
public readonly union Result<T, E>(Ok<T>, Error<E>);   // two type parameters
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

[Sample Code](../src/02_ResultOption/Option.cs#L39)

Call sites don't change (`opt.IsSome`, `opt.Map(...)`), and the members resolve across assembly
boundaries — the xUnit project consumes them straight from the referenced library.

### 2.7 Giving a union its own functions (methods, properties, `ToString`)

A union can carry its own members — methods, computed properties, a custom `ToString()`, static
factories. There are three ways to do it, and I checked all three (see `src/11_UnionWithMembers`):

```csharp
// (1) Shorthand declaration WITH a body — the common case
public readonly union Shape(Circle, Rectangle, Triangle)
{
    public double Area => this switch          // <-- switch over `this`, NOT Value
    {
        Circle c    => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        Triangle t  => 0.5 * t.Base * t.Height,
        null        => 0,                       // the default/empty union
    };

    public Shape Scaled(double f) => this switch { /* ... */ Circle c => new Circle(c.Radius*f), null => this };

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
public readonly partial union Temperature(Celsius, Fahrenheit);
public readonly partial union Temperature
{
    public double AsCelsius => this switch { Celsius c => c.Degrees, Fahrenheit f => (f.Degrees-32)*5/9, null => double.NaN };
    public override string ToString() => $"{AsCelsius:F1} °C";
}

// (3) Fully explicit — any class/struct with [Union], one public single-parameter constructor
//     per case type (or a member provider, §3.6) and an `object? Value` property is a union.
//     Implementing IUnion is optional (the generated form does it).
[Union]
public readonly struct HttpOutcome : IUnion
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
not exhaustive and warns CS8509 ("the pattern '_' is not covered"), because `object?` is
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
public readonly union Shape(Circle, Rectangle);
```

[Sample Code](../src/06_Lowering/Unions.cs#L7)

Decompiled with `ilspycmd 06_Lowering.dll -t Lowering.Shape`:

```csharp
using System.Runtime.CompilerServices;

[Union]
public readonly struct Shape : IUnion
{
    public object? Value { get; }

    [CompilerGenerated] public Shape(Circle value)    { Value = value; }
    [CompilerGenerated] public Shape(Rectangle value) { Value = value; }
}
```

So the "opinionated" shorthand generates:

- a `struct` (value type) — `readonly` here because the declaration says `readonly union`,
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
had to declare `IUnion`/`UnionAttribute` yourself. Since Preview 5 — and in .NET 11 GA — they're
in the BCL, so the samples compile with no extra plumbing.

### 3.3 The implicit conversion is just a constructor call

`Shape MakeCircle(double r) => new Circle(r);` decompiles to:

```csharp
public static Shape MakeCircle(double r) => new Shape(new Circle(r));
```

[Sample Code (source)](../src/06_Lowering/Unions.cs#L43)

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

[Sample Code (source)](../src/06_Lowering/Unions.cs#L37)

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
   match, so the exhaustive switch hits `ThrowSwitchExpressionException` at runtime — and with
   non-nullable case types the compiler doesn't even warn about it. Exhaustiveness itself is
   CS8509, a warning rather than an error, consistent with every other C# switch. Verified:

   ```csharp
   Result<int, string> def = default;      // legal
   Console.WriteLine(def.Value is null);   // True   -> no case selected
   ```

   If a case type is nullable (`union NullablePet(Cat?, Dog)`) the same hole surfaces as CS8655;
   either way, add a `null` arm (see §2.3) to handle it explicitly.

2. Value-type cases box. Storage is `object?`, so a `struct` case type gets boxed on conversion.
   I lean on `record class` cases (reference types) when allocations matter, or accept one boxing
   allocation per value.

3. `ToString()` is not forwarded to the underlying value. A union prints its struct type name,
   not the case's `ToString()`:

   ```text
   ToString of union: Functional.Result`2[System.Int32,System.String]   // NOT "Ok { Value = 7 }"
   ```

   The fix is to give the union its own `ToString()` (see §2.7 and `src/11_UnionWithMembers`) by
   switching over `this`. The shorthand `union X(...) { ... }`, a `partial union`, and an explicit
   `[Union]` struct all support custom members.

4. No `op_Implicit` is emitted — the union conversion is a language-level conversion the compiler
   applies at the use site. It still takes part in overload resolution as usual (`Call(Pet)` beats
   `Call(object)` for a `Cat`), but it's invisible to reflection-based or other-language callers,
   which just see the constructors.

5. ~~Preview limitations~~ — resolved in GA. In Preview 5, _union member providers_ and the full
   _explicit union pattern_ weren't implemented. In C# 15 GA I verified all of these work:
   hand-written `[Union]` structs **and classes**, the non-boxing access pattern
   (`HasValue` + one `TryGetValue(out T)` per case — the compiler calls those instead of `Value`,
   no boxing), and member providers (a nested `IUnionMembers` interface with static `Create`
   factories, allowing private constructors). See §3.6.

### 3.6 Beyond the shorthand — custom unions without boxing

The shorthand always stores `object?`, so `union Number(int, double)` boxes on every
conversion. A hand-written `[Union]` type can opt in to the **non-boxing access pattern**:

```csharp
[Union]
public readonly struct ValueOption<T> : IUnion
{
    private enum Tag : byte { Empty = 0, Some, None }
    private readonly Tag _tag;
    private readonly T _value;                        // unboxed storage

    public ValueOption(T value) { _tag = value is null ? Tag.Empty : Tag.Some; _value = value; }
    public ValueOption(None _)  { _tag = Tag.None; _value = default!; }

    public object? Value => _tag switch { Tag.Some => _value, Tag.None => None.Instance, _ => null };

    public bool HasValue => _tag != Tag.Empty;                                    // for `null`
    public bool TryGetValue(out T value)    { value = _value; return _tag == Tag.Some; }  // for `T x`
    public bool TryGetValue(out None value) { value = None.Instance; return _tag == Tag.None; }
}
```

[Sample Code](../src/13_CustomUnions/ValueOption.cs#L27)

The compiler now emits `TryGetValue`/`HasValue` calls instead of `get_Value` + `isinst`
(`src/13_CustomUnions/decompile.sh`):

```text
SumBoxed (shorthand `union BoxedOption<T>(T, None)`)   SumValueOption (custom [Union] struct)
  call  BoxedOption`1<int32>::.ctor(!0)   // boxes       call ValueOption`1<int32>::.ctor(!0)
  call  BoxedOption`1<int32>::get_Value()                call ValueOption`1<int32>::TryGetValue(!0&)
  isinst  System.Int32
  unbox.any System.Int32
```

Contract to respect: `Value`, `HasValue` and `TryGetValue` must always agree — that's why a
null `T` produces the _empty_ union rather than "Some(null)". And case types must be
distinguishable: `ValueOption<None>` fails with CS0457.

Measured over 1,000 conversions + type tests: **24,000 bytes** for the shorthand vs **0 bytes**
for `ValueOption<int>`.

**Union member providers.** Instead of public constructors, a union can implement a nested
public `IUnionMembers` interface with static `Create` factories. That allows a private
constructor (validation/normalisation in one place) and lets the union be a `record class` —
value equality and a readable `ToString()` for free:

```csharp
[Union]
public sealed record class Outcome<T> : Outcome<T>.IUnionMembers
{
    public interface IUnionMembers : IUnion
    {
        static Outcome<T> Create(Ok<T> value)    => new((object)value);
        static Outcome<T> Create(Failure value)  => new((object)value);
        new object? Value { get; }
    }
    private Outcome(object value) => Value = value;
    public object? Value { get; }
}

Outcome<int> a = new Ok<int>(7);   // calls IUnionMembers.Create
// a.ToString() == "Outcome { Value = Ok { Value = 7 } }", and a == new Ok<int>(7) converted -> True
```

[Sample Code](../src/13_CustomUnions/Outcome.cs#L28)

Gotcha I hit: writing `=> new(value)` inside `Create` is target-typed to `Outcome<T>` — a union —
so it becomes a union conversion that calls `Create` again → **stack overflow**. The `(object)`
cast selects the private constructor.

A class-based union also changes `null` semantics: the `null` pattern matches both a null
_reference_ and a null `Value`.

---

## 4. Sample catalogue (third part — runnable demos)

All projects live under `src/`, target `net11.0`, and share settings via `Directory.Build.props`
(default C# 15, nullable on). I kept each one small. Outputs below are the actual runs.

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
1: Placed on 11/15/2026
2: Shipped, tracking TRK-20261115-001
3: Delivered on 11/18/2026
```

[Sample Code](../src/04_DomainModeling/Program.cs#L15)

### `05_PatternShowcase` — console

A tour of the main pattern forms through a union (`Shape(Circle, Rectangle, Triangle)`): type,
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
Fahrenheit { Degrees = 98.6 } body temp -> 37.0 °C
OK 200 (success=True)
```

[Sample Code — Shape](../src/11_UnionWithMembers/Shape.cs#L18) · [Temperature](../src/11_UnionWithMembers/Temperature.cs#L11) · [HttpOutcome](../src/11_UnionWithMembers/ExplicitUnion.cs#L14)

### `12_ClosedHierarchies` — console (see §6)

The `Pet` example as a `closed record` hierarchy: exhaustive switch without `_`, the shared
`Name` member on the base, non-transitive closed-ness (`Puppy : Dog`), a nullable `Pet?`
switch, a generic constraint `where TPet : Pet`, and a closed recursive `Tree<T>`.

```text
Whiskers  Cat named Whiskers           says meow
Rex       Dog named Rex                says woof
Bit       Dog named Bit                says woof
Tweety    Bird named Tweety (flies)    says tweet
<no pet>

Runtime type of a Pet: Dog (no wrapper struct, no .Value)

Tree sum = 6, depth = 3
```

[Sample Code](../src/12_ClosedHierarchies/Program.cs#L24)

### `13_CustomUnions` — console (+ `decompile.sh`, see §3.6)

A non-boxing `ValueOption<T>` struct vs the shorthand, and a record-class `Outcome<T>` with a
member provider.

```text
Some(42) -> some int 42
None     -> explicit none
<empty>  -> default/empty
shorthand union (boxes int)      sum=499500  allocated=24,000 bytes
ValueOption<int> (TryGetValue)   sum=499500  allocated=0 bytes

a = Outcome { Value = Ok { Value = 7 } }
a == b: True
c = FAILED: unknown error
missing = <null>
```

[Sample Code — ValueOption](../src/13_CustomUnions/ValueOption.cs#L27) · [Outcome](../src/13_CustomUnions/Outcome.cs#L28) · [decompile.sh](../src/13_CustomUnions/decompile.sh)

### `14_JsonUnions` — console (see §7)

System.Text.Json with unions and closed hierarchies, all through the source generator — each
scenario next to its C# 14 / .NET 10 equivalent (`CSharp14Way.cs`), with a check that both
produce the same JSON.

```text
== (1) union of primitives ==
  C# 15: [42,"sensor offline"]
    7    -> case Int32
    ok   -> case String
    8    -> case Int32
  C# 14: [42,"sensor offline"]   (custom JsonConverter, same JSON: True)

== (2) union of objects, structural classifier ==
  {"Value":1}                        C# 15 -> Ok(1)                  C# 14 -> Ok(1)
  {"Title":"Teapot","Status":418}    C# 15 -> Problem(418 Teapot)    C# 14 -> Problem(418 Teapot)
  {"Foo":1}                          C# 15 -> JsonException          C# 14 -> JsonException
  serialised: {"Title":"Not found","Status":404}   (same JSON in C# 14: True)

== (3) closed hierarchy, inferred $type ==
  C# 15: [{"$type":"Circle","Radius":1.5},{"$type":"Square","Length":2},{"$type":"Triangle","Base":3,"Height":4}]
    round-trip -> Circle { Radius = 1.5 }
    round-trip -> Square { Length = 2 }
    round-trip -> Triangle { Base = 3, Height = 4 }
  C# 14: [{"$type":"Circle","Radius":1.5},{"$type":"Square","Length":2}]   ([JsonDerivedType] list)
  C# 14 + Triangle -> NotSupportedException
```

[Sample Code](../src/14_JsonUnions/Program.cs#L21) · [C# 14 version](../src/14_JsonUnions/CSharp14Way.cs#L1)

### `15_IndexersAndLabels` — console (bonus features, see §8)

```text
squares[2] = 9, squares[^1] = 100
Sun 2026-11-15: Monday of that week = Mon 2026-11-09
board = ...X......, board[(0,3)] = 'X'
C# 15 labeled loops : goal at (3,3) after 11 cells
pre-C# 15 (flag)    : goal at (3,3) after 11 cells
processed 3 steps, 1 command(s) left after 'quit'
```

[Sample Code](../src/15_IndexersAndLabels/Program.cs#L9)

### `07_CollectionExprArgs` — console (bonus feature, see §8)

```text
List: count=3, capacity=6
HashSet (OrdinalIgnoreCase): count=1
SortedSet (reverse): [3, 2, 1]
Merged: [0, 1, 2, 3, 4] capacity=16
```

[Sample Code](../src/07_CollectionExprArgs/Program.cs#L12)

---

## 5. How it compares

- **Declaration** — C# 15 union: `union Pet(Cat,Dog)`; C# 15 closed: `closed record Pet;` + subtypes; F#: `type Pet = Cat | Dog`; OneOf: `OneOf<Cat,Dog>`; sealed hierarchy: abstract + sealed.
- **Exhaustiveness** — union: yes, a CS8509 warning; closed: yes, CS8509; F#: yes, warning FS0025; OneOf: via `.Match(...)` (one lambda per case), not via `switch`; abstract + sealed: no — needs a `_` arm.
- **Case types** — union: any existing types (`int`, `string`, foreign records); closed: only your own subtypes; F#: named cases; OneOf: any.
- **Shared members** — union: only what you add to the union body; closed: inherited from the base; F#: members on the type; OneOf: none.
- **Pattern integration** — union: full unwrap; closed: native (no wrapper); F#: yes; OneOf: positional `.IsT0`; sealed hierarchy: yes.
- **Runtime cost** — union: one `object?` field, boxes value-type cases (custom `[Union]` can avoid it, §3.6); closed: plain objects, no wrapper; F#: tag + fields; OneOf: wrapper struct.
- **"Empty" value** — union: `default(Pet)` with `Value == null`; closed: a `null` reference; F#: only via `Unchecked.defaultof`/interop; OneOf: `default`.
- **Dependency** — union/closed: in-box (.NET 11); F#: F# only; OneOf: third-party; sealed hierarchy: none.

The nuance I want to close on: C# unions are unions _of types_, not tagged unions. If two cases
would be the same type, give them distinct case types. That's the part that differs from F#'s
name-tagged cases.

### Will `Result`/`Option` ship in the .NET BCL?

Short answer, as of the .NET 11 GA (November 2026): no `Option`/`Result` in the BCL. The team scoped this feature as the
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

A runnable side-by-side lives in [`src/00_Rust`](../src/00_Rust/README.md): the same `Pet` enum and `Option`/`Result` demos in Rust, so the audience can see exactly what C# 15 is catching up to (`cargo run`).

---

## 6. Closed hierarchies — the other half of exhaustiveness

C# 15 also fixes the "abstract base + sealed subclasses" workaround from §1. Mark the base
`closed` and only the declaring assembly may derive from it _directly_; the set of direct
subtypes is known, so switches are exhaustive:

```csharp
public closed record Pet(string Name);               // implicitly abstract
public record Cat(string Name) : Pet(Name);
public record Dog(string Name) : Pet(Name);
public record Bird(string Name, bool CanFly) : Pet(Name);

string Describe(Pet pet) => pet switch
{
    Cat c  => $"Cat named {c.Name}",
    Dog d  => $"Dog named {d.Name}",
    Bird b => $"Bird named {b.Name}",
};   // no `_` arm, no warning; drop one -> CS8509
```

[Sample Code](../src/12_ClosedHierarchies/Program.cs#L24)

Rules worth knowing (verified on the GA compiler; the error cases appear as comments in
`12_ClosedHierarchies`):

- In another assembly: `error CS9382: 'Hamster': cannot use a closed type 'Pet' from another
  assembly as a base type.`
- `closed` is not transitive. `Dog` is open, so `Puppy : Dog` is fine anywhere — and still
  matched by the `Dog` arm.
- `Pet?` needs a `null` arm (CS8655 otherwise).
- A type parameter `where TPet : Pet` is treated like `Pet` for exhaustiveness.
- Generic subtypes must use all their type parameters in the base (`Leaf<T> : Tree<T>` ✓,
  `Const<U> : Tree<int>` ✗).

**Union or closed?** A union is "one of these _existing_ types" — `int`, `string`, a record from
another library — with a wrapper struct and no shared members. A closed hierarchy is "one of _my_
subtypes" — no wrapper, no boxing, shared base members (`pet.Name` without a switch), but you
must own every case type. For a domain model you own (`04_DomainModeling`, `03_RecursiveExpr`)
`closed` is often the more natural fit; for combining types you don't own (`Result<T, E>`,
`int | string`) only a union works.

---

## 7. Unions on the wire — System.Text.Json

.NET 11's System.Text.Json understands unions out of the box. A union serialises as its case
value — no wrapper object, no `$type`:

```csharp
public readonly union Reading(int, string);
Reading[] readings = [42, "sensor offline"];
JsonSerializer.Serialize(readings);           // [42,"sensor offline"]
```

On deserialisation the default classifier picks the case from the JSON token kind. That can't
distinguish two _object_ cases, so opt in to the structural classifier, which matches property
names (zero or multiple matches → `JsonException`):

```csharp
[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public readonly union ApiResult(Ok, Problem);          // Ok(int Value) / Problem(string Title, int Status)
```

And for closed hierarchies, STJ can infer the polymorphism from the closed set — no
`[JsonDerivedType]` list:

```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Shape;
// [{"$type":"Circle","Radius":1.5},{"$type":"Square","Length":2}]
```

[Sample Code](../src/14_JsonUnions/Program.cs#L21)

### 7.1 How you'd write it in C# 14 / .NET 10

The wire format is identical (the sample checks it), but every scenario needs hand-written
plumbing. [`CSharp14Way.cs`](../src/14_JsonUnions/CSharp14Way.cs#L1) has the full code; the
essence:

**(1) `int` or `string`** — no union type, so: a wrapper struct, an `implicit operator` per case,
and a custom converter that switches on the token kind
([code](../src/14_JsonUnions/CSharp14Way.cs#L21)):

```csharp
[JsonConverter(typeof(ReadingConverter))]
public readonly struct Reading
{
    public object? Value { get; }
    private Reading(object value) => Value = value;
    public static implicit operator Reading(int value)    => new(value);
    public static implicit operator Reading(string value) => new(value);
}

public sealed class ReadingConverter : JsonConverter<Reading>
{
    public override Reading Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions o) =>
        reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String => reader.GetString()!,
            _ => throw new JsonException(...),
        };

    public override void Write(Utf8JsonWriter writer, Reading value, JsonSerializerOptions o)
    {
        switch (value.Value)              // object? — no exhaustiveness check
        {
            case int i:    writer.WriteNumberValue(i); break;
            case string s: writer.WriteStringValue(s); break;
            default:       throw new JsonException(...);
        }
    }
}
```

**(2) `Ok` or `Problem` by shape** — a converter that buffers the object into a `JsonDocument`
and sniffs hard-coded property names that you must keep in sync with the records by hand
([code](../src/14_JsonUnions/CSharp14Way.cs#L66)):

```csharp
using var doc = JsonDocument.ParseValue(ref reader);
var root = doc.RootElement;
bool looksLikeOk      = root.TryGetProperty(nameof(Ok.Value), out _);
bool looksLikeProblem = root.TryGetProperty(nameof(Problem.Title), out _)
                     && root.TryGetProperty(nameof(Problem.Status), out _);
return (looksLikeOk, looksLikeProblem) switch
{
    (true, false) => root.Deserialize<Ok>(options)!,
    (false, true) => root.Deserialize<Problem>(options)!,
    _ => throw new JsonException("Payload matches no (or more than one) ApiResult case."),
};
```

**(3) Polymorphic hierarchy** — every subtype listed by hand
([code](../src/14_JsonUnions/CSharp14Way.cs#L99)):

```csharp
[JsonPolymorphic]
[JsonDerivedType(typeof(Circle), nameof(Circle))]
[JsonDerivedType(typeof(Square), nameof(Square))]
public abstract record Shape;
public record Triangle(double Base, double Height) : Shape;   // added later, list not updated
```

That compiles fine — and serialising a `Triangle` throws `NotSupportedException` at **runtime**.
With `closed` + `InferClosedTypePolymorphism`, the C# 15 `Triangle` just works.

The takeaway for the audience: **C# 15 + .NET 11 replace ~70 lines of wrapper/converter code
with three declarations and two attributes** — and the parts that used to fail at runtime
(a forgotten case in a converter, a missing `[JsonDerivedType]`) are now either checked by the
compiler or derived automatically.

### 7.2 Notes

The sample runs everything through a `JsonSerializerContext` (source generator → trimming and
Native AOT friendly). Limits from the docs: the structural classifier doesn't support nested
unions, polymorphic cases, or `ReferenceHandler.Preserve`. Per the .NET 11 announcement, ASP.NET
Core Minimal APIs/MVC accept unions in JSON bodies and OpenAPI renders them as `anyOf` (not part
of this sample).

---

## 8. Bonus C# 15 features

### 8.1 Collection expression arguments

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
`[with(comparer), KeyValuePair.Create(...)]` does not compile — still true in C# 15 GA (CS9215).
Dictionary expressions (`[k: v]`) are not part of C# 15.

### 8.2 Extension indexers

Extension blocks (C# 14) gain indexers. Indexers are instance members, so the block needs a
named receiver:

```csharp
extension<T>(IEnumerable<T> source)
{
    public T this[Index index] => index.IsFromEnd
        ? source.Reverse().ElementAt(index.Value - 1)
        : source.ElementAt(index.Value);
}

IEnumerable<int> squares = Squares(10);       // a lazy iterator
squares[^1];                                  // 100
today[DayOfWeek.Monday];                      // extension(DateOnly) indexer
board[new Pos(0, 3)] = 'X';                   // getter + setter on List<char>
```

[Sample Code](../src/15_IndexersAndLabels/Program.cs#L116)

Gotcha: element access on an **array** never falls back to an extension indexer —
`char[] arr; arr[pos]` is `CS0029: Cannot implicitly convert type 'Pos' to 'int'`. Extend an
interface (`IReadOnlyList<T>`) or a class instead.

### 8.3 Labeled `break` / `continue`

Label a loop (or `switch`) and jump to it by name — no more flags or `goto`:

```csharp
rows: for (int r = 0; r < maze.Count; r++)
{
    for (int c = 0; c < maze[r].Length; c++)
    {
        if (maze[new Pos(r, c)] == '#') continue rows;   // next ROW
        ...
    }
}

loop: while (commands.TryDequeue(out var cmd))
{
    switch (cmd)
    {
        case "skip": continue loop;
        case "quit": break loop;                          // leaves the WHILE, not just the switch
    }
}
```

[Sample Code](../src/15_IndexersAndLabels/Program.cs#L69)

The sample puts the flag-based pre-C# 15 version next to it. (IDE0410 suggests the rewrite for
the classic `found = true; break; … if (found) break;` and `goto` shapes.)

---

## 9. Demo running order (pick from these depending on time)

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
7. Custom unions (5 min): `13_CustomUnions` — 24,000 vs 0 bytes, `decompile.sh` shows
   `TryGetValue` instead of `isinst`; member providers and the record-class union.
8. Closed hierarchies (5 min): `12_ClosedHierarchies` — the §1 workaround, fixed; union vs closed.
9. On the wire (5 min): `14_JsonUnions` — bare values, structural classifier, inferred `$type`;
   each next to its C# 14 converter/`[JsonDerivedType]` version (`CSharp14Way.cs`).
10. Bonus (5 min): collection expression arguments, extension indexers, labeled break/continue
    (`07`, `15`).
11. Wrap (2 min): comparison table, the `default(union)` caveat, "type unions vs tagged unions".
12. Optional add-on (20 min): AI agents and C# 15. The LSP raises false alarms, and a skill makes reviews
    about 2× faster or, without tools, actually correct. Script: [`agent-skill/DEMO.md`](../agent-skill/DEMO.md).

---

## 10. Reproduce everything

```bash
# .NET 11 SDK + an IL decompiler
dotnet tool install -g ilspycmd

dotnet build  CSharp15.slnx -c Release      # 0 warnings, 0 errors
dotnet test   tests/Csharp15.Tests          # Passed! Failed: 0, Passed: 10
src/06_Lowering/decompile.sh                # regenerate the IL listings in §3
src/13_CustomUnions/decompile.sh            # TryGetValue vs get_Value/unbox (§3.6)
```

## 11. References

- What's new in C# 15 — https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15
- Union types (language reference) — https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/union
- Unions feature specification — https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-15.0/unions
- Discriminated unions working group — https://github.com/dotnet/csharplang/tree/main/meetings/working-groups/discriminated-unions
- Collection expression arguments — https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions#collection-expression-arguments
- `closed` modifier — https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/closed
- What's new in .NET 11 libraries (System.Text.Json unions) — https://learn.microsoft.com/dotnet/core/whats-new/dotnet-11/libraries
- Announcing .NET 11 — https://devblogs.microsoft.com/dotnet/announcing-dotnet-11/
