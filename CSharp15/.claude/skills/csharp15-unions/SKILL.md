---
name: csharp15-unions
description: Verified facts about C# 15 / .NET 11 (GA Nov 2026) union types (`union Pet(Cat, Dog)`), `closed` class hierarchies, and System.Text.Json support for both. These shipped after most models' training data, so use this skill whenever you review, write, refactor or modernize C# that targets net11.0 or later. Do so especially when the code has an abstract base with sealed subclasses, throwing discard (`_`) or UnreachableException switch arms, OneOf/Either/Result/Option wrappers, visitors, `object` values with `is` checks, or classes with several nullable fields where only one is set. It also applies to `[JsonDerivedType]` lists and custom JsonConverters that pick a type. Use it too when C# LSP diagnostics call `union` or `closed` an error.
---

# C# 15: unions and closed hierarchies

Your built-in knowledge of these features is probably missing, preview-era, or wrong. The facts below were checked against the .NET 11 GA SDK (11.0.100). Prefer them over memory. When unsure, verify with a build (see "Verify before you claim").

## Does it apply?

Check the project's `TargetFramework`:
- **`net11.0` or later:** C# 15 is the default. No `<LangVersion>preview</LangVersion>` and no `EnablePreviewFeatures` are needed. Don't tell users these features are preview.
- **Older targets:** don't suggest these features. Mention at most that upgrading unlocks them.

## Union types: "one of these existing types"

```csharp
public record Cat(string Name); public record Dog(string Name);
public union Pet(Cat, Dog);                    // the whole declaration
public union Either<T0, T1>(T0, T1);           // generic unions are fine
public union Reading(int, string);             // any type that converts to object (not ref structs)

Pet p = new Dog("Rex");                        // implicit conversion, no factory needed
string Describe(Pet pet) => pet switch         // exhaustive; the switch "sees through" to the case value
{
    Cat c => c.Name,
    Dog d => d.Name,
    null  => "<empty>",                        // default(Pet) has Value == null; prefer `null =>` over `_ =>`
};
```

The declaration lowers to a `struct` with `[Union]` and `IUnion` (both in `System.Runtime.CompilerServices`), an `object? Value` property, and one constructor per case. Consequences to state correctly in reviews:

- **Exhaustiveness:** a missing case gives **CS8509 (warning)**. Add `<WarningsAsErrors>CS8509</WarningsAsErrors>` if it should be a gate.
- **Null arm:** with non-nullable case types, no `null` arm is required. `default(Pet)` then throws `SwitchExpressionException`. CS8655 appears only for nullable case types.
- **Avoid a `_` arm:** it silently absorbs future cases.
- **Boxing:** value-type cases (`int`, `double`, structs) are boxed into `Value`. Don't turn an allocation-free hot-path struct into a shorthand union. Use a custom union instead (below).
- **Distinct case types:** case types must be distinguishable. `Result<T>(T, string)` breaks for `Result<string>`, and `Either<string,string>` fails with **CS0457** at the conversion site. Wrap ambiguous payloads in records (`record Error(string Message)`), and remember that even `Result<Error>` collides.
- **`ToString()`:** it is not forwarded to the case value and prints the struct name. Members can be added in a body: `public union Shape(Circle, Square) { public override string ToString() => this switch { ... }; }`. Inside the body, switch over **`this`**, not `Value`, because `Value` is `object?` and is not exhaustive. `partial union` also works.
- **No `op_Implicit`:** the conversion is compiler-only. Reflection and other .NET languages see only the constructors.
- **Pattern matching:** it works normally through the union. `token is '+'`, property patterns, and `when` clauses all apply to the case value.

**Custom unions** (non-boxing, or when you need your own storage) are any struct or class with `[Union]`, one public single-parameter constructor per case, and `object? Value`. `IUnion` is optional. If you add `bool HasValue` and `bool TryGetValue(out T)` per case, the compiler calls those instead of `Value`, and nothing boxes:

```csharp
[Union] public readonly struct Option<T> : IUnion
{
    private readonly T _value;
    public Option(T value) { _value = value; HasValue = value is not null; }
    public bool HasValue { get; }                       // false  => matches `null`
    public object? Value => HasValue ? _value : null;   // must agree with HasValue/TryGetValue
    public bool TryGetValue(out T value) { value = _value; return HasValue; }
}
// opt switch { int v => ..., null => ... }   is exhaustive and allocation-free
```

A union can also use *member providers*: a nested `public interface IUnionMembers : IUnion` with `static Create(Case)` factories, which allows a private constructor. Inside `Create`, write `new((object)value)`, because `new(value)` re-enters the union conversion and overflows the stack.

## Closed hierarchies: "one of MY subtypes"

```csharp
public closed record PaymentResult;                 // also: `public closed class Expr { public abstract ... }`
public sealed record Approved(string Id) : PaymentResult;
public sealed record Declined(string Reason) : PaymentResult;

int Status(PaymentResult r) => r switch { Approved => 200, Declined => 402 };   // exhaustive, no `_`
```

- **Who may derive:** only the declaring assembly may derive directly. Elsewhere you get **CS9382**. This replaces the `private protected` constructor trick.
- **Implicitly abstract:** a closed type is already abstract. `abstract closed` is **CS9384**, and abstract members are allowed.
- **Not transitive:** subtypes can stay open (`Puppy : Dog`), and the `Dog` arm still covers `Puppy`.
- **Nullable:** a `PaymentResult?` governing type needs a `null` arm (CS8655).
- **Generics:** `where T : PaymentResult` is exhaustive like the base. Generic subtypes must use all their type parameters in the base: `Leaf<T> : Tree<T>` is fine, `Const<U> : Tree<int>` is an error.
- **Extension points:** never close a hierarchy that other assemblies are meant to extend, such as plugin or provider base classes or adapters in other packages. `closed` is a breaking change for them.

## Which one?

| Situation | Use |
|---|---|
| You own all case types and they share data or behaviour (domain results, AST nodes, events) | `closed` hierarchy: no wrapper, no boxing, base members |
| Cases are existing or foreign types (`int`, `string`, records from other libraries, `T`) | `union` |
| Generic `Result`/`Either`/`Option` | `union` (mind CS0457 and boxing) |
| Allocation-sensitive value-type cases | custom `[Union]` struct with `TryGetValue`/`HasValue` |
| Open for third-party extension | neither: keep `abstract` |
| Visitor pattern only to get exhaustiveness over your own nodes | `closed` + switch expressions (the visitor remains valid if double dispatch is wanted) |

## System.Text.Json (.NET 11)

- **Plain unions:** a union serialises as its bare case value, with no wrapper and no `$type`. On read, the default classifier picks the case by JSON token kind (number, string, and so on). This replaces hand-written `JsonConverter`s for `int | string`-style values.
- **Two object cases without a discriminator:** use `[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))] public union R(A, B);`. Without the attribute you get warning SYSLIB1227.
  - It picks the case by property names and respects the naming policy. When more than one case matches, it throws `JsonException`.
  - It is lenient by default: `{"accepted":12}` still becomes `Acc` with `BatchId = null`. To make it reject partial payloads the way a hand-written converter did, set `RespectRequiredConstructorParameters = true` and `RespectNullableAnnotations = true`. Give optional parameters a default (`int? RetryAfterSeconds = null`), or they become required too.
  - It does not support nested unions, polymorphic cases, or `ReferenceHandler.Preserve`.
- **Polymorphic hierarchies:** `[JsonPolymorphic(InferClosedTypePolymorphism = true)]` on a `closed` base replaces the `[JsonDerivedType]` list and writes `$type` = type name.
  - **Without the option**, a `closed` base serialises derived instances as `{}`. That is silent data loss with no warning.
  - **Mixing inference with any `[JsonDerivedType]`** turns inference off for everything else: you get warning SYSLIB1244, and unlisted types throw.
  - A forgotten `[JsonDerivedType]` is a runtime `NotSupportedException` in pre-C# 15 code, so point it out when you see one.
- **Source generation:** all of this works with `JsonSerializerContext` (trimming/AOT).
- **Behaviour change on migration:** converting a hand-written converter to a union can change edge cases, such as `default` writing `null` instead of throwing, or ambiguous payloads now throwing. Mention that in a review.

## Verify before you claim

- **Lagging tooling:** C# language servers and IDE plugins may lag behind the compiler. For example, csharp-ls 0.28.0 reports valid C# 15 code as errors: `union` gives CS0246/CS1001, `closed` gives "CS8652 preview", and exhaustive switches give false CS8509. **`dotnet build` is the source of truth.** Never "fix" code, add `LangVersion preview`, or add `_` arms because of such diagnostics.
- **Test non-trivial suggestions:** before you recommend a non-trivial rewrite, compile it in a throwaway copy or a scratch project (`dotnet new classlib -f net11.0`) when you can. Don't write experiments into the user's project unless asked.
- **Look up an API missing from this file in the SDK:** the reference pack ships XML docs, at `<dotnet-root>/packs/Microsoft.NETCore.App.Ref/11.*/ref/net11.0/*.xml`, for example `System.Text.Json.xml`. `dotnet --list-sdks` shows `<dotnet-root>/sdk`; typical roots are `/usr/local/share/dotnet` and `C:\Program Files\dotnet`. Grep them before guessing.
- **Mark unverified claims:** when you couldn't verify something, say so instead of guessing syntax.
