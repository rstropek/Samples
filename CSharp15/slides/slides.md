---
theme: default
title: C# 15 & .NET 11
info: |
  Union types, closed hierarchies, unions in System.Text.Json, a few small C# 15
  features, and runtime async. Samples:
  https://github.com/rstropek/Samples/tree/master/CSharp15
author: Rainer Stropek
colorSchema: light
aspectRatio: 16/9
canvasWidth: 980
transition: none
fonts:
  sans: Inter
  mono: JetBrains Mono
  weights: '400,600,700'
drawings:
  persist: false
layout: cover
---

# C# 15 & .NET 11

Union types · closed hierarchies · unions in JSON · runtime async

<div class="mt-10">

Rainer Stropek

Samples: [github.com/rstropek/Samples/tree/master/CSharp15](https://github.com/rstropek/Samples/tree/master/CSharp15)

</div>

<!--
Everything on the slides is in the repo and was compiled and run with the .NET 11 GA SDK
(11.0.100). C# 15 is the default language version for net11.0, no LangVersion needed.
-->

---

# Agenda

<div class="agenda">
<div class="item"><span class="num">1</span><div><b>AI agents &amp; C# 15</b><small>a skill for the new features</small></div></div>
<div class="item"><span class="num">2</span><div><b>Union types</b><small>the feature</small></div></div>
<div class="item"><span class="num">3</span><div><b>Under the hood</b><small>what the compiler generates, C# and IL</small></div></div>
<div class="item"><span class="num">4</span><div><b>Custom unions</b><small>without boxing</small></div></div>
<div class="item"><span class="num">5</span><div><b><code>closed</code> hierarchies</b><small>the other way to get exhaustiveness</small></div></div>
<div class="item"><span class="num">6</span><div><b>Unions on the wire</b><small>System.Text.Json</small></div></div>
<div class="item"><span class="num">7</span><div><b>Small C# 15 goodies</b><small>collection args, extension indexers, labels</small></div></div>
<div class="item"><span class="num">8</span><div><b>Runtime async</b><small>no more compiler state machines</small></div></div>
</div>

---

# AI agents & C# 15

<div class="intro">

<div class="slides-hint">
<div class="label">Agent skill</div>
<a href="https://github.com/rstropek/Samples/tree/master/CSharp15/.claude/skills/csharp15-unions" target="_blank">.claude/skills/<br>csharp15-unions</a>
<small>About 100 lines of verified facts: unions, <code>closed</code>, System.Text.Json</small>
<small>Claude Code loads it automatically in this repo</small>
<small>Evals included</small>
</div>

<div>
<div class="label">What we measured</div>
<v-clicks>

<div class="finding ok"><b>It works without a skill</b><span>The agent checks itself: probe projects, the SDK's XML docs</span></div>
<div class="finding ok"><b>A skill makes it faster</b><span>About 40% less time, 15% fewer tokens, same quality</span></div>
<div class="finding bad"><b>The C# LSP currently hurts more than it helps</b><span>csharp-ls 0.28.0 reports valid <code>union</code>/<code>closed</code> code as errors</span></div>

</v-clicks>
</div>

</div>

<div v-click class="core">
<div class="core-title">C# 15 makes C# more agent-friendly</div>
<div class="core-points">
<div><b>Unions, <code>closed</code>:</b> intent is in the types, so compiler warnings tell the agent what to fix</div>
<div><b>Runtime async:</b> async code gets faster, and the agent doesn't have to change anything</div>
</div>
</div>

<div class="src"><Src path="agent-skill/README.md" label="agent-skill/README.md" /><Src path=".claude/skills/csharp15-unions/SKILL.md" label=".claude/skills/csharp15-unions/SKILL.md" /></div>

<!--
The agent knows the syntax from the previews and checks the rest itself.
Experiment: Opus 5.5, four C# 14-style libraries to review, one run per configuration.
All configurations passed 23/23 assertions. With skill: 168 s / 68k tokens per review,
without: 279 s / 81k tokens.
Without tools (code pasted into the prompt), the model without the skill gave pre-C# 15
advice; with the skill it recommended closed + InferClosedTypePolymorphism and unions.
LSP: csharp-ls 0.28.0 (newest on NuGet, checked 2026-11-17) reports union as CS0246,
closed as "preview", and every exhaustive switch as CS8509. The skill tells the agent
that dotnet build is the source of truth. It uses a pre-GA Roslyn; an agent that trusts
the errors "fixes" valid code (adds LangVersion preview, adds _ arms, drops closed).

Core message: C# 15 makes C# more agent-friendly.
- Unions and closed hierarchies let the agent state its intent clearly. A missing case is a
  compiler warning (CS8509), so the agent sees its own mistake and fixes it.
- Runtime async makes .NET more efficient without the agent having to do anything.
-->

---
layout: center
class: thema
---

# Union types

---

# Before C# 15: "one of N types"

<div class="cols">
<div>

```csharp {all|1-4|10|13-16}
// (1) abstract base + sealed subclasses
public abstract record Pet;
public sealed record Cat(string Name) : Pet;
public sealed record Dog(string Name) : Pet;

string Describe(Pet pet) => pet switch
{
    Cat c => $"Cat {c.Name}",
    Dog d => $"Dog {d.Name}",
    _     => throw new InvalidOperationException(),
};

// (2) OneOf NuGet package
string Describe(OneOf<Cat, Dog> pet) => pet.Match(
    cat => $"Cat {cat.Name}",
    dog => $"Dog {dog.Name}");
```

</div>
<div>

<div v-click="1" class="note">

**①** The hierarchy is **open**: anybody can add a subclass.

</div>

<div v-click="2" class="note warn">

**②** So the compiler can't prove exhaustiveness: the `_` arm is mandatory, and a new subclass silently ends up there at runtime.

</div>

<div v-click="3" class="note">

**③** OneOf: arms are **positional** (`.IsT0`, `.AsT1`), generic noise, third-party dependency.

</div>

<div v-click="4" class="note">

Also seen: `object` + `is` checks, one class with a nullable field per case.

</div>

</div>
</div>

<div class="src"><Src path="src/01_Basics/Alternatives.cs" line="25" /></div>

---

# Where this comes from: Rust

<div class="cols">
<div>

```rust
enum Pet {
    Cat { name: String },
    Dog { name: String },
}

match pet {
    Pet::Cat { name } => format!("Cat {name}"),
    Pet::Dog { name } => format!("Dog {name}"),
}

// in std:  Option<T> { Some(T), None }
//          Result<T, E> { Ok(T), Err(E) }
```

</div>
<div>

<div v-click="1" class="note">

Sum types since day one. A missing `match` arm is a compile **error**.

</div>

<div v-click="2" class="note">

C# 15 gets the same modelling power, but as unions of **types**, not named variants.

</div>

</div>
</div>

<div class="src"><Src path="src/00_Rust/src/main.rs" line="23" /></div>

<!--
Keep this short: it's only here to show where the idea comes from. F# and TypeScript have it
too.
-->

---

# Declaring a union

<div class="cols">
<div>

```csharp {all|5|7-8|10-15}
public record class Cat(string Name);
public record class Dog(string Name);
public record class Bird(string Name, bool CanFly);

public readonly union Pet(Cat, Dog, Bird);

Pet a = new Dog("Rex");
Pet b = new Cat("Whiskers");

string Describe(Pet pet) => pet switch
{
    Cat c  => $"Cat {c.Name}",
    Dog d  => $"Dog {d.Name}",
    Bird b => $"Bird {b.Name}",
};
```

</div>
<div>

<div v-click="1" class="note">

**①** A union of existing **types**: classes, structs, interfaces, type parameters (not `ref struct`).
`readonly` is optional (it's a struct, see later).

</div>

<div v-click="2" class="note">

**②** Implicit conversion from every case type. No `new Pet(...)`, no `.From(...)`.

</div>

<div v-click="3" class="note">

**③** Exhaustive: no `_` arm. Remove the `Bird` arm and the compiler complains.

</div>

</div>
</div>

<div class="src"><Src path="src/01_Basics/Program.cs" line="14" /></div>

<!--
Removing the Bird arm gives CS8509 (next slide).
-->

---

# Exhaustiveness is a warning, and there is an empty union

```text
warning CS8509: The switch expression does not handle all possible values of its
input type (it is not exhaustive). For example, the pattern 'Bird' is not covered.
```

<div class="cols">
<div>

```csharp {all|1-2|8}
Pet nobody = default;   // legal: Value == null
Describe(nobody);       // SwitchExpressionException

string Describe(Pet pet) => pet switch
{
    Cat c  => $"Cat {c.Name}",
    // ... Dog, Bird ...
    null   => "<uninitialised pet>",
};
```

</div>
<div>

<div v-click="1" class="note warn">

**①** `default(Pet)` always exists. With non-nullable cases there's **no warning**, only a runtime throw.

</div>

<div v-click="2" class="note">

**②** Add a `null` arm, **not** `_`: a discard would swallow future case types.

</div>

<div v-click="3" class="note">

Want a hard gate? `<WarningsAsErrors>CS8509</WarningsAsErrors>`

</div>

</div>
</div>

<div class="src"><Src path="src/01_Basics/Program.cs" line="58" /></div>

<!--
Same policy as every other non-exhaustive switch expression in C#: a warning.
CS8655 only shows up if a case type is nullable, e.g. union NullablePet(Cat?, Dog).
-->

---

# Pattern matching reaches through the union

```csharp {all|3-4|5|10-14}
public static string Classify(Shape shape) => shape switch
{
    Circle { Radius: 0 }     => "degenerate circle",
    Circle { Radius: < 1 } c => $"tiny circle r={c.Radius}",
    Rectangle { Width: var w, Height: var h } when w == h => $"square {w}x{h}",
    // ... Circle, Rectangle, Triangle ...
    null                     => "<uninitialised shape>",
};

public static string Compare(Shape a, Shape b) => (a, b) switch
{
    (Circle, Circle) => "two circles",
    _                => "mixed pair",
};
```

<div v-click="1" class="note">Property, relational and <code>var</code> patterns and <code>when</code> guards apply to the <b>case value</b>, not to the union struct.</div>
<div v-click="3" class="note">Tuples of unions work too: the compiler checks the whole product (3 × 3 cases).</div>

<div class="src"><Src path="src/05_PatternShowcase/Program.cs" line="16" /></div>

---

# Make illegal states unrepresentable

```csharp {all|1-3|4|6-12}
public record class Draft(IReadOnlyList<string> Items);
public record class Placed(IReadOnlyList<string> Items, DateOnly OrderedOn);
// ... Shipped, Delivered, Cancelled
public readonly union OrderState(Draft, Placed, Shipped, Delivered, Cancelled);

public static OrderState Advance(OrderState state) => state switch
{
    Draft { Items.Count: 0 } => new Cancelled("Cannot place an empty order"),
    Draft d                  => new Placed(d.Items, Today),
    // ... Placed -> Shipped -> Delivered (terminal), Cancelled (terminal)
    null                     => throw new InvalidOperationException(),
};
```

<div v-click="1" class="note">Each state carries <b>only</b> the data that is valid in that state. No nullable "maybe set" fields.</div>
<div v-click="3" class="note">Add a sixth state → every switch over <code>OrderState</code> warns until it is handled.</div>

<div class="src"><Src path="src/04_DomainModeling/Program.cs" line="9" /></div>

---

# Generic unions: build your own `Option` / `Result`

<div class="cols">
<div>

```csharp {all|1-3|5|7-16}
public record class Some<T>(T Value);
public sealed record class None;
public readonly union Option<T>(Some<T>, None);

public readonly union Result<T, E>(Ok<T>, Error<E>);

extension<T>(Option<T> option)   // in a static class
{
    public bool IsSome => option is Some<T>;
    public Option<U> Map<U>(Func<T, U> f) => option switch
    {
        Some<T> s => new Some<U>(f(s.Value)),
        None n    => n,
        null      => None.Instance,
    };
}
```

</div>
<div>

<div v-click="1" class="note">

**①** Type parameters work. `None` is not generic and converts into any `Option<T>`.

</div>

<div v-click="2" class="note">

**②** More than one type parameter is fine.

</div>

<div v-click="3" class="note">

**③** Helpers via C# 14 **extension blocks**: extension properties and methods in one block.

</div>

<div v-click="4" class="note">

No `Option`/`Result` in the BCL. C# 15 ships the building blocks.

</div>

</div>
</div>

<div class="src"><Src path="src/02_ResultOption/Option.cs" line="18" /><Src path="src/02_ResultOption/Result.cs" line="10" /></div>

---

# `Result` in practice: a validation pipeline

```csharp {all|4-5|7-10}
static Result<Registration, string> ParseRegistration(string raw)
{
    var parts = raw.Split('/', StringSplitOptions.TrimEntries);
    if (parts.Length != 2)
        return Result.Fail<Registration, string>("expected 'email / age'");

    return ValidateEmail(parts[0])
        .Bind(email => ParseAge(parts[1])
            .Bind(age => ValidateAge(age)
                .Map(_ => new Registration(email, age))));
}
```

```text
alice@example.com / 30   => OK   -> alice@example.com, age 30
bob@ / 30                => FAIL -> invalid email 'bob@'
carol@example.com / 999  => FAIL -> age 999 out of range
```

<div class="src"><Src path="src/10_ResultOptionUsage/Program.cs" line="43" /></div>

<!--
Railway-oriented programming: the first failure short-circuits the rest of the chain.
-->

---

# A union can have members

<div class="cols">
<div>

```csharp {all|3|8|11-12|14}
public readonly union Shape(Circle, Rectangle, Triangle)
{
    public double Area => this switch
    {
        Circle c    => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        Triangle t  => 0.5 * t.Base * t.Height,
        null        => 0,
    };

    public override string ToString() =>
        this switch { /* ... */ };

    public static Shape Unit => new Circle(1);
}
```

</div>
<div>

<div v-click="1" class="note warn">

**①** Switch over **`this`**, not `Value`. `Value` is `object?`, so that switch is not exhaustive (CS8509, `_` not covered).

</div>

<div v-click="2" class="note">

**②** `null` = the empty union.

</div>

<div v-click="3" class="note">

**③** `ToString()` is **not** forwarded to the case. Without this override you get the struct's type name.

</div>

<div v-click="4" class="note">

Also possible: `partial union`, or a hand-written `[Union]` struct.

</div>

</div>
</div>

<div class="src"><Src path="src/11_UnionWithMembers/Shape.cs" line="18" /><Src path="src/11_UnionWithMembers/Temperature.cs" line="11" /><Src path="src/11_UnionWithMembers/ExplicitUnion.cs" line="14" /></div>

---
layout: center
class: thema
---

# Under the hood

---

# Under the hood: what does `union` lower to?

```csharp
public readonly union Shape(Circle, Rectangle);   // Circle, Rectangle: records
```

<div class="cols">
<div>

```csharp {all|1|2|4|6-9}
[Union]
public readonly struct Shape : IUnion
{
    public object? Value { get; }

    [CompilerGenerated]
    public Shape(Circle value) { Value = value; }
    [CompilerGenerated]
    public Shape(Rectangle value) { Value = value; }
}
```

<div v-click="5" class="note">

BCL contract (`System.Runtime.CompilerServices`): `interface IUnion { object? Value { get; } }` + `UnionAttribute`

</div>

</div>
<div>

<div v-click="1" class="note">

**①** `[Union]` marks it for the compiler.

</div>
<div v-click="2" class="note">

**②** A plain **struct**. `readonly` because I wrote `readonly union`.

</div>
<div v-click="3" class="note">

**③** One `object? Value`. No tag, no new runtime concept.

</div>
<div v-click="4" class="note">

**④** One constructor per case type.

</div>

</div>
</div>

<div class="src"><Src path="src/06_Lowering/Unions.cs" line="11" /><Src path="src/06_Lowering/decompile.sh" /></div>

<!--
src/06_Lowering/decompile.sh regenerates this listing.
History: in Preview 2 you had to declare IUnion/UnionAttribute yourself; since Preview 5 they
are in the BCL.
-->

---

# The implicit conversion is just a constructor call

```csharp
public static Shape MakeCircle(double r) => new Circle(r);   // implicit conversion
```

```text {all|2|3|all}
IL_0000: ldarg.0
IL_0001: newobj  instance void Lowering.Circle::.ctor(float64)
IL_0006: newobj  instance void Lowering.Shape::.ctor(class Lowering.Circle)
IL_000b: ret
```

<div v-click="1" class="note">

**①** Create the case value: an ordinary `Circle`.

</div>

<div v-click="2" class="note">

**②** Wrap it: the generated `Shape(Circle)` constructor. That's the whole "conversion".

</div>

<div v-click="3" class="note warn">

**③** No `op_Implicit` is emitted. The conversion is a language rule, so reflection and other .NET languages only see the constructors.

</div>

<div class="src"><Src path="src/06_Lowering/Unions.cs" line="43" /></div>

---

# The exhaustive switch, decompiled

```csharp
public static double Area(Shape shape) => shape switch
{
    Circle c    => Math.PI * c.Radius * c.Radius,
    Rectangle r => r.Width * r.Height,
};   // no null arm on purpose: shows the compiler's safety net
```

```csharp {all|3|4|5|6}
public static double Area(Shape shape)
{
    object value = shape.Value;
    if (value is Circle circle)       return Math.PI * circle.Radius * circle.Radius;
    if (value is Rectangle rectangle) return rectangle.Width * rectangle.Height;
    <PrivateImplementationDetails>.ThrowSwitchExpressionException(shape);
    return default;
}
```

<div v-click="1" class="note">Read <code>Value</code> once, then one type test per case, in arm order…</div>
<div v-click="4" class="note warn">…and a throw for anything else. Reached only by the empty <code>default(Shape)</code>.</div>

<div class="src"><Src path="src/06_Lowering/Unions.cs" line="37" /></div>

---

# …and in IL: an `isinst` ladder

```text {all|1|2-3|4-5|6,8-11}
IL_0002: call      instance object Lowering.Shape::get_Value()
IL_0009: isinst    Lowering.Circle
IL_0010: brtrue.s  IL_001e
IL_0013: isinst    Lowering.Rectangle
IL_001a: brtrue.s  IL_0038
IL_001c: br.s      IL_0048
...
IL_0048: ldarg.0
IL_0049: box       Lowering.Shape
IL_004e: call      void '<PrivateImplementationDetails>'
                     ::ThrowSwitchExpressionException(object)
```

<div v-click="1" class="note">

**①** `get_Value()`: the `object?` field.

</div>
<div v-click="2" class="note">

**②③** `isinst` per case type: the same code you'd write by hand with `is` checks.

</div>
<div v-click="4" class="note">

**④** Fallthrough: box the union and throw `SwitchExpressionException`.

</div>

<div class="src"><Src path="src/06_Lowering/decompile.sh" /></div>

---

# Consequences of "struct with `object? Value`"

<v-clicks>

- **The empty union is real.** `default(Shape)` has `Value == null`. Handle it with a `null` arm.
- **Value-type cases box.** `union Number(int, double)` allocates on every conversion.
- **`ToString()`, `Equals()` are the struct's**, not the case's:
  ``Functional.Result`2[System.Int32,System.String]`` instead of `Ok { Value = 7 }`
- **No `op_Implicit`.** Conversions are a C# feature, invisible to reflection.
- **Unions of types, not tagged unions.** Two cases of the same type? Wrap them in distinct records.

</v-clicks>

<div v-click class="note mt-6">

All of these are fixable when you need it: next slides.

</div>

---
layout: center
class: thema
---

# Custom unions without boxing

---

# Custom unions without boxing

<div class="cols">
<div>

```csharp {all|1-2|4-6|8-9|11|13-15}
[Union]
public readonly struct ValueOption<T> : IUnion
{
    private enum Tag : byte { Empty, Some, None }
    private readonly Tag _tag;
    private readonly T _value;

    public ValueOption(T value) { /* Some */ }
    public ValueOption(None _)  { /* None */ }

    public object? Value => /* boxes */;

    public bool HasValue => _tag != Tag.Empty;
    public bool TryGetValue(out T value) { ... }
    public bool TryGetValue(out None value) { ... }
}
```

</div>
<div>

<div v-click="1" class="note">

**①** Any type with `[Union]` is a union. `IUnion` is optional.

</div>
<div v-click="2" class="note">

**②** Your own storage: a tag + an **unboxed** `T`.

</div>
<div v-click="3" class="note">

**③** One constructor per case = the implicit conversions.

</div>
<div v-click="4" class="note">

**④** `Value` is still required, but…

</div>
<div v-click="5" class="note">

**⑤** …if `HasValue` + `TryGetValue` exist, the compiler uses **them** for `null` and type patterns.

</div>

</div>
</div>

<div class="src"><Src path="src/13_CustomUnions/ValueOption.cs" line="27" /></div>

---

# Boxing vs. non-boxing in IL

<div class="cols2">
<div>

### `union BoxedOption<T>(T, None)`

```text {all|1|2-4}{at:1}
call   BoxedOption`1<int32>::.ctor(!0)
call   BoxedOption`1<int32>::get_Value()
isinst    System.Int32
unbox.any System.Int32
```

</div>
<div>

### custom `ValueOption<T>`

```text {all|1|2}{at:1}
call   ValueOption`1<int32>::.ctor(!0)
call   ValueOption`1<int32>::TryGetValue(!0&)
```

</div>
</div>

<div v-click="1" class="note">

**①** Conversion: the shorthand stores `int` in `object?` → **box**.

</div>
<div v-click="2" class="note">

**②** Type test: `get_Value` + `isinst` + `unbox` vs. one `TryGetValue` call.

</div>

<div v-click="3">

```text
shorthand union (boxes int)      sum=499500  allocated=24,000 bytes
ValueOption<int> (TryGetValue)   sum=499500  allocated=0 bytes
```

</div>

<div v-click="4" class="note warn">

Contract: `Value`, `HasValue` and `TryGetValue` must always agree.

</div>

<div class="src"><Src path="src/13_CustomUnions/decompile.sh" /><Src path="src/13_CustomUnions/Program.cs" /></div>

---

# Union member providers: a `record class` union

```csharp {all|2|4-9|10|13}
[Union]
public sealed record class Outcome<T> : Outcome<T>.IUnionMembers
{
    public interface IUnionMembers : IUnion
    {
        static Outcome<T> Create(Ok<T> value)   => new((object)value);
        static Outcome<T> Create(Failure value) => new((object)value);
        new object? Value { get; }
    }
    private Outcome(object value) => Value = value;
    public object? Value { get; }
}
Outcome<int> a = new Ok<int>(7);   // Create(...): "Outcome { Value = Ok { Value = 7 } }"
```

<div v-click="1" class="note">A record class instead of a struct: value equality and a readable <code>ToString()</code> for free.</div>
<div v-click="2" class="note warn">Static <code>Create</code> factories allow a <b>private</b> constructor. Gotcha: without the <code>(object)</code> cast, <code>new(value)</code> is a union conversion → <code>Create</code> again → stack overflow.</div>

<div class="src"><Src path="src/13_CustomUnions/Outcome.cs" line="28" /></div>

---
layout: center
class: thema
---

# <code>closed</code> hierarchies

---

# `closed` hierarchies: exhaustiveness without a union

<div class="cols">
<div>

```csharp {all|1|2-4|6-11}
public closed record Pet(string Name);
public record Cat(string Name) : Pet(Name);
public record Dog(string Name) : Pet(Name);
public record Bird(string Name, bool CanFly) : Pet(Name);

string Describe(Pet pet) => pet switch
{
    Cat c  => $"Cat {c.Name}",
    Dog d  => $"Dog {d.Name}",
    Bird b => $"Bird {b.Name}",
};
```

</div>
<div>

<div v-click="1" class="note">

**①** `closed` = implicitly abstract; only **this assembly** may derive directly (else CS9382).

</div>
<div v-click="2" class="note">

**②** Not transitive: `Dog` is open, a `Puppy : Dog` is still matched by the `Dog` arm.

</div>
<div v-click="3" class="note">

**③** No `_` arm, no warning. That fixes workaround (1) from the start.

</div>

</div>
</div>

<div class="src"><Src path="src/12_ClosedHierarchies/Program.cs" line="24" /></div>

<!--
More rules: Pet? needs a null arm (CS8655); where TPet : Pet is treated like Pet; generic
subtypes must use all their type parameters in the base (Leaf<T> : Tree<T>).
-->

---

# Union or `closed`?

<div class="cols2">
<div>

## `union`

<v-clicks>

- "one of these **existing** types"
- cases can be types you **don't own**: `int`, `string`, a record from a NuGet package
- wrapper struct, boxes value types
- no shared members
- `Result<T, E>`, `Option<T>`

</v-clicks>

</div>
<div>

## `closed`

<v-clicks>

- "one of **my** subtypes"
- cases must derive from **your** base, in **your** assembly
- plain objects, no wrapper, no boxing
- shared base members (`pet.Name`)
- domain models, ASTs

</v-clicks>

</div>
</div>

---
layout: center
class: thema
---

# Unions on the wire: System.Text.Json

---

# Unions on the wire (1): case value only

<div class="cols">
<div>

```csharp {all|1|3-5|7-8|10-12}
public readonly union Reading(int, string);

Reading[] readings = [42, "sensor offline"];
JsonSerializer.Serialize(readings, ctx.ReadingArray);
// [42,"sensor offline"]

JsonSerializer.Deserialize("""[7, "ok", 8]""", ...);
// 7 -> int,  "ok" -> string,  8 -> int

[JsonSerializable(typeof(Reading[]))]
internal partial class AppJsonContext
    : JsonSerializerContext;
```

</div>
<div>

<div v-click="2" class="note">

**①** No wrapper object, no `$type`. Just the case value.

</div>
<div v-click="3" class="note">

**②** Reading: the case is picked by the JSON **token kind** (number → `int`, string → `string`).

</div>
<div v-click="4" class="note">

**③** Works with the source generator (trimming, Native AOT).

</div>
<div v-click="5" class="note warn">

C# 14: wrapper struct + implicit operators + a hand-written `JsonConverter`, same JSON.

</div>

</div>
</div>

<div class="src"><Src path="src/14_JsonUnions/Program.cs" line="21" /><Src path="src/14_JsonUnions/CSharp14Way.cs" line="21" label="C# 14 version" /></div>

---

# Unions on the wire (2): object cases by shape

```csharp {all|1-2|4-5}
public sealed record Ok(int Value);
public sealed record Problem(string Title, int Status);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public readonly union ApiResult(Ok, Problem);
```

<div v-click="1" class="note">Both cases are JSON objects: token kind can't decide.</div>
<div v-click="2" class="note">The <b>structural classifier</b> picks the case whose property names match:</div>

<div v-click="3">

```text
{"Value":1}                      -> Ok(1)
{"Title":"Teapot","Status":418}  -> Problem(418 Teapot)
{"Foo":1}                        -> JsonException   (0 matches; 2+ matches throw, too)
```

</div>

<div v-click="4" class="note warn">

C# 14: a hand-written converter parses the object twice (`JsonDocument`), and you pick the identifying properties yourself. Add a case → update the converter, or it fails at runtime.

</div>

<div class="src"><Src path="src/14_JsonUnions/Program.cs" line="27" /><Src path="src/14_JsonUnions/CSharp14Way.cs" line="66" label="C# 14 version" /></div>

---

# Unions on the wire (3): `closed` + inferred `$type`

<div class="cols2">
<div>

### C# 15

```csharp {all|1-2|5-6}{at:1}
[JsonPolymorphic(
  InferClosedTypePolymorphism = true)]
public closed record Shape;
public record Circle(double Radius) : Shape;
public record Triangle(
  double Base, double Height) : Shape;
```

</div>
<div>

### C# 14

```csharp {all|2-3|5-6}{at:1}
[JsonPolymorphic]
[JsonDerivedType(typeof(Circle), "Circle")]
[JsonDerivedType(typeof(Square), "Square")]
public abstract record Shape;
public record Triangle(  // added later
  double Base, double Height) : Shape;
```

</div>
</div>

<div v-click="1" class="note">STJ knows every subtype of a <code>closed</code> type: no list to maintain.</div>

<div v-click="2">

```text
C# 15: [{"$type":"Circle","Radius":1.5}, ..., {"$type":"Triangle","Base":3,"Height":4}]
C# 14: Serialize(Triangle) -> NotSupportedException   (compiles fine, fails at RUNTIME)
```

</div>

<div class="src"><Src path="src/14_JsonUnions/Program.cs" line="31" /><Src path="src/14_JsonUnions/CSharp14Way.cs" line="99" label="C# 14 version" /></div>

<!--
The sample also has Square in both versions; left out here to save space.
The JSON output shown is the real output of the sample.
-->

---
layout: center
class: thema
---

# Small C# 15 goodies

---

# Small C# 15 goodies (1)

## Collection expression arguments

```csharp {all|1|2|3}
List<string> names  = [with(capacity: values.Length * 2), .. values];
HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO"]; // 1
SortedSet<int> desc = [with(Comparer<int>.Create((x, y) => y.CompareTo(x))), 3, 1, 2];
```

<div v-click="4">

## Extension indexers

```csharp
extension<T>(IEnumerable<T> source)
{
    public T this[Index index] => index.IsFromEnd
        ? source.Reverse().ElementAt(index.Value - 1)
        : source.ElementAt(index.Value);
}

IEnumerable<int> squares = Squares(10);   // lazy iterator
squares[^1];                              // 100
```

</div>

<div class="src"><Src path="src/07_CollectionExprArgs/Program.cs" line="12" /><Src path="src/15_IndexersAndLabels/Program.cs" line="116" /></div>

<!--
with(...) must be the first element. Dictionary collection expressions still don't work with
a comparer (CS9215).
Extension indexers never apply to arrays: arr[pos] is CS0029. Extend IReadOnlyList<T> instead.
-->

---

# Small C# 15 goodies (2): labeled `break` / `continue`

<div class="cols2">
<div>

### C# 15

```csharp {all|1|6}{at:1}
rows: for (int r = 0; r < maze.Count; r++)
{
    for (int c = 0; c < maze[r].Length; c++)
    {
        var p = new Pos(r, c);
        if (maze[p] == '#') continue rows;
        visited++;
        // ...
    }
}
```

</div>
<div>

### Before

```csharp {all|3|7-8}{at:1}
for (int r = 0; r < maze.Count; r++)
{
    bool skipRow = false;
    for (int c = 0; c < maze[r].Length
                    && !skipRow; c++)
    {
        var p = new Pos(r, c);
        if (maze[p] == '#')
            { skipRow = true; continue; }
        visited++;
    }
}
```

</div>
</div>

<div v-click="3" class="note">

Also works on `switch` inside a loop: `case "quit": break loop;` leaves the **loop**. IDE0410 suggests the rewrite.

</div>

<div class="src"><Src path="src/15_IndexersAndLabels/Program.cs" line="66" /><Src path="src/15_IndexersAndLabels/Program.cs" line="53" /></div>

---
layout: center
class: thema
---

# Runtime async

---

# Runtime async: one switch in the project file

```xml
<PropertyGroup>
  <Features>$(Features);runtime-async=on</Features>
</PropertyGroup>
```

<v-clicks>

- Since C# 5, `async`/`await` is a **compiler** trick: every async method becomes a hidden state machine. The CLR never knew.
- .NET 11 moves that job into the **runtime**. Your source code doesn't change.
- Opt-in for your code; the BCL already ships compiled with it.
- Works with JIT, ReadyToRun and Native AOT.

</v-clicks>

<div class="src"><Src path="src/09_RuntimeAsync_Enabled/09_RuntimeAsync_Enabled.csproj" /></div>

---

# Classic: the compiler writes a state machine

```csharp
// await Task.CompletedTask; Console.WriteLine(new StackTrace());  becomes:
struct InnerAsync_d : IAsyncStateMachine {
    int _state = -1; AsyncTaskMethodBuilder _builder; TaskAwaiter _awaiter;

    void MoveNext() {
        if (_state == -1) {
            _awaiter = Task.CompletedTask.GetAwaiter();
            if (!_awaiter.IsCompleted) {
                _state = 0;
                _builder.AwaitUnsafeOnCompleted(ref _awaiter, ref this);
                return;                        // suspend: unwinds the stack
            }
        }
        _awaiter.GetResult();                  // resume here
        Console.WriteLine(new StackTrace());
        _builder.SetResult();
    }
}
```

<!--
Pseudo code, simplified. One such struct per async method (4 in the demo assembly).
InnerAsync() itself becomes a launcher: create the struct, _builder.Start(ref sm),
return _builder.Task. Start calls MoveNext the first time: that's the extra
AsyncMethodBuilderCore.Start frame in the stack trace later.
-->

<div class="src"><Src path="src/08_RuntimeAsync_Classic/Program.cs" line="30" /></div>

---

# Runtime async: a linear method + one flag

```text {all|2|5|all}
.method assembly hidebysig static
    class Task '<<Main>$>g__InnerAsync|0_2' () cil managed flags(2000)
{
    IL_0000: call  class Task System.Threading.Tasks.Task::get_CompletedTask()
    IL_0005: call  void System.Runtime.CompilerServices.AsyncHelpers::Await(class Task)
    IL_000a: ldc.i4.0
    IL_000b: newobj instance void System.Diagnostics.StackTrace::.ctor(bool)
    IL_0010: call  void System.Console::WriteLine(object)
    IL_0015: ret
}
```

<div v-click="1" class="note">

**①** `flags(2000)` = `MethodImplOptions.Async`: the runtime sees "this is async".

</div>
<div v-click="2" class="note">

**②** `await x` → `AsyncHelpers.Await(x)`. The runtime suspends and resumes **here**.

</div>
<div v-click="3" class="note">

**③** 22 bytes of IL. No nested type, no builder, no `MoveNext`.

</div>

<div class="src"><Src path="src/09_RuntimeAsync_Enabled/Program.cs" line="30" /><Src path="src/runtime-async-compare.sh" /></div>

---

# What you can see: the live stack trace

<div class="cols2">
<div>

### Classic: 13 frames

```text {all|2,5,8,11}{at:1}
at g__InnerAsync|0_2()
at AsyncMethodBuilderCore.Start(...)
at g__InnerAsync|0_2()
at g__MiddleAsync|0_1()
at AsyncMethodBuilderCore.Start(...)
at g__MiddleAsync|0_1()
at g__OuterAsync|0_0()
at AsyncMethodBuilderCore.Start(...)
at g__OuterAsync|0_0()
at <Main>$(String[])
at AsyncMethodBuilderCore.Start(...)
at <Main>$(String[])
at <Main>()
```

</div>
<div>

### `runtime-async=on`: 5 frames

```text
at g__InnerAsync|0_2()
at g__MiddleAsync|0_1()
at g__OuterAsync|0_0()
at <Main>$(String[])
at <Main>()
```

<div v-click="2" class="note">Same source, only the project flag differs.</div>
<div v-click="3" class="note warn">Exception stack traces were already clean. The win: live stack, debugger, profilers, overhead.</div>

</div>
</div>

<div class="src"><Src path="src/08_RuntimeAsync_Classic/Program.cs" line="18" /><Src path="src/09_RuntimeAsync_Enabled/Program.cs" line="18" /></div>

<!--
Be precise: the demo awaits only completed tasks, so the whole chain is physically on the
stack. After a real suspension the logical callers are gone in both modes. Runtime async
removes infrastructure frames; it doesn't rebuild logical call stacks.
src/runtime-async-compare.sh runs both and prints the two traces.
-->

---

# Takeaways

<v-clicks>

- `union Pet(Cat, Dog, Bird)`: implicit conversions, exhaustive `switch`, patterns reach through.
- Under the hood: a `struct` with `object? Value`, constructors, and an `isinst` ladder.
- Watch out for the **empty union** and **boxing**. Custom `[Union]` types fix both when needed.
- `closed` gives exhaustiveness for **your own** hierarchies, without a wrapper.
- System.Text.Json understands both: no more hand-written converters or `[JsonDerivedType]` lists.
- Runtime async: same code, flip a flag, the runtime owns `await`.

</v-clicks>

<div v-click class="mt-8">

All samples, docs and this deck: [github.com/rstropek/Samples/tree/master/CSharp15](https://github.com/rstropek/Samples/tree/master/CSharp15)

</div>
