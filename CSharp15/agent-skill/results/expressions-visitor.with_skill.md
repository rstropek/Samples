# Code review: `Expressions` (net11.0 / C# 15)

Files reviewed: `Expr.cs`, `Visitors.cs`, `Parser.cs`, `Expressions.csproj`. No edits were made to the project. Every rewrite suggested below was compiled and run in a throwaway scratch copy with the .NET 11 SDK (11.0.100). The original also builds cleanly, with no warnings.

## TL;DR

1. **Visitor:** For this codebase, no, it's no longer the best fit. Your own doc comment says *"The node types are fixed; new operations are added as visitors."* That is what C# 15 `closed` hierarchies are for. Make `Expr` a `public closed record` and write each operation as a switch expression. You keep compile-time exhaustiveness (CS8509 at every switch when a node is added), and you drop `Accept`, `IExprVisitor<T>` and the visitor classes.
2. **Tokenizer returning `object`:** Replace it with a union: `public union Token(double, char, string);`. That's a one-line declaration. `yield return` and the existing `is char op` / `is not ')'` patterns keep working, and switches over `Token` become exhaustive.
3. **Bugs and smells, independent of the redesign:** `Simplifier.VisitNeg` is exponential. `Printer` output can't always be parsed back. Operators are stringly-typed `char`s with a `_ => throw` arm. Identity elimination only works on one side.

C# 15 is the default language version for `net11.0`, so you don't need `LangVersion` or preview flags.

---

## 1. Visitor pattern vs. `closed` hierarchy

### What the visitor gives you today
- **Exhaustiveness.** Adding a node means adding an `IExprVisitor<T>` method, and every visitor then fails to compile.
- **Double dispatch,** at the cost of `Accept` boilerplate on every node, an interface to maintain, and stateful-looking classes (`new Evaluator(...)`, `new Simplifier()`) for what are really pure functions.

### What C# 15 gives you instead
`closed` restricts direct derivation to the declaring assembly. The compiler therefore knows the complete set of subtypes and treats a type switch over them as exhaustive, with no `_` arm.

```csharp
namespace Expressions;

public closed record Expr;                                 // implicitly abstract
public sealed record Num(double Value) : Expr;
public sealed record Var(string Name) : Expr;
public sealed record Neg(Expr Operand) : Expr;
public sealed record Binary(BinOp Op, Expr Left, Expr Right) : Expr;

public enum BinOp { Add, Sub, Mul, Div }
```

```csharp
public static class Evaluator
{
    public static double Eval(Expr e, IReadOnlyDictionary<string, double> vars) => e switch
    {
        Num n    => n.Value,
        Var v    => vars.TryGetValue(v.Name, out var val) ? val
                    : throw new KeyNotFoundException($"Unknown variable {v.Name}"),
        Neg n    => -Eval(n.Operand, vars),
        Binary b => Apply(b.Op, Eval(b.Left, vars), Eval(b.Right, vars)),
    };                                                     // no `_` arm: exhaustive

    public static double Apply(BinOp op, double l, double r) => op switch
    {
        BinOp.Add => l + r, BinOp.Sub => l - r, BinOp.Mul => l * r, BinOp.Div => l / r,
        _ => throw new ArgumentOutOfRangeException(nameof(op)),   // enums are never exhaustive
    };
}

public static class Simplifier
{
    public static Expr Simplify(Expr e) => e switch
    {
        Num or Var => e,
        Neg n => Simplify(n.Operand) switch                // operand simplified ONCE
        {
            Num k => new Num(-k.Value),
            var s => new Neg(s),
        },
        Binary b => (b.Op, Simplify(b.Left), Simplify(b.Right)) switch
        {
            (var op, Num l, Num r)               => new Num(Evaluator.Apply(op, l.Value, r.Value)),
            (BinOp.Mul, var l, Num { Value: 1 }) => l,
            (BinOp.Add, var l, Num { Value: 0 }) => l,
            var (op, l, r)                       => new Binary(op, l, r),
        },
    };
}
```

`Printer` follows the same pattern. I checked the following in the scratch build:
- The rewrite builds with 0 warnings and gives the same results as the original on sample inputs.
- **Adding a node is still a compile-time event.** I added `sealed record Pow(...) : Expr`, and every switch reported `warning CS8509 ... the pattern 'Expressions.Pow' is not covered`. Leaving out an arm (for example `Var`) gives the same warning.
- **Consumers in other assemblies also get exhaustiveness.** A switch in a referencing class library compiled warning-free with all four arms and gave CS8509 when one was missing.
- **No one else can add nodes.** `record Mine(Expr X) : Expr` in another assembly fails with **CS9382** ("cannot use a closed type 'Expr' from another assembly as a base type").

**Recommendation:** CS8509 is a *warning*, while the visitor gave you a hard error. To keep that guarantee, add this to `Expressions.csproj`:

```xml
<WarningsAsErrors>CS8509</WarningsAsErrors>
```

Don't add `_ =>` arms to switches over `Expr`. They would silently absorb future node types and defeat the check.

### Side benefits of `record` over `class`
- **Structural equality.** `Parser.Parse("x+1") == Parser.Parse("x + 1")` is `true` in the scratch build, which makes parser and simplifier tests trivial to write.
- **`with` expressions** for tree rewrites.
- Much less code. Each node is one line, and `Accept` and `IExprVisitor<T>` go away.

Note that record equality on a deep tree is recursive. That's fine at these sizes.

### When the visitor would still be the right call
- **Double dispatch is a real requirement.** For example, a framework calls operations polymorphically without knowing the concrete operation type, or you need visitor-specific state and hooks (pre/post-visit) shared across many operations.
- **Other assemblies must add node types.** If `Expr` is an extension point, don't make it `closed`: that is a breaking change for anyone deriving from it. Nothing here suggests that. The comment says the node set is fixed.

Neither applies here, so `closed` plus switch expressions is the better model. The visitor isn't *wrong*. It's just ceremony you no longer need.

### About `char Op`
Today's `Binary.Op` is a `char`, so `Evaluator.VisitBinary` needs `_ => throw new InvalidOperationException(...)`, and `new Binary('%', ...)` compiles fine. An enum `BinOp`, as above, makes the valid set explicit. A switch over an enum still needs a default arm, because C# enums can hold any underlying value. If you want full exhaustiveness for operators too, you have two options:
- model them as node types (`sealed record Add(Expr L, Expr R) : Expr`, ...), or
- add an intermediate `closed record BinaryExpr(Expr Left, Expr Right) : Expr` with sealed `Add`/`Sub`/`Mul`/`Div` under it.

I compiled a nested `closed record ... : Expr` successfully, but I didn't go further with that variant. The enum is the pragmatic middle ground.

---

## 2. Tokenizer: `IEnumerable<object>` → union

Today the token type is documented only in an XML comment: "double, char or string". Nothing stops a caller from receiving or adding other types, and `ParseUnary` ends in a catch-all `throw`. The token kinds are *existing* types (`double`, `char`, `string`) that you don't own, so a C# 15 `union` is the natural fit:

```csharp
public union Token(double, char, string);

public static IEnumerable<Token> Tokenize(string text)
{
    ...
    yield return double.Parse(...);   // implicit conversion to Token
    yield return text[start..i];
    yield return c;
}
```

The parser barely changes, because patterns "see through" the union to the case value:

```csharp
while (pos < t.Count && t[pos] is char op and ('+' or '-')) { ... }
if (pos >= t.Count || t[pos] is not ')') throw ...;

switch (token)
{
    case double d:   return new Num(d);
    case string name: return new Var(name);
    case '-':        return new Neg(ParseUnary(t, ref pos));
    case '(':        ...
    default:         throw new FormatException($"Unexpected token {Describe(token)}");
}

static string Describe(Token t) => t switch     // exhaustive, no `_`
{
    double d => d.ToString(CultureInfo.InvariantCulture),
    char c   => $"'{c}'",
    string s => s,
};
```

I compiled and ran all of this in the scratch copy. Things to know:

- **`ToString()` is not forwarded.** `$"{token}"` prints `Expressions.Token`, not `+`. Your current `$"Unexpected token {tokens[pos]}"` messages would degrade silently. Either use a `Describe` helper as above, or give the union a body: `public union Token(double, char, string) { public override string ToString() => this switch { ... }; }`. Switch over `this` there, not `Value`, because `Value` is `object?` and isn't exhaustive.
- **Implicit numeric conversions pick a case.** `Token t = 5;` becomes the `double` case, while `'x'` stays `char` (verified). That's harmless here, but `char` → `double` also converts implicitly, so be deliberate if you ever construct tokens from computed values.
- **`default(Token)`** has `Value == null`. A switch with only the three type arms throws `SwitchExpressionException` on it (verified). The tokenizer never produces one, so no `null` arm is needed.
- **Boxing:** `double` and `char` are boxed into `Value`. That's no regression, since `object` already boxed them. If tokenizing ever becomes hot, use a custom `[Union]` struct with `HasValue`/`TryGetValue` to avoid allocation. That's not worth it at this scale.

**Alternative if you want richer tokens:** if you plan to add source positions for better error messages, which I'd recommend (see §3), the tokens become *your own* types sharing data. A `closed` hierarchy then fits better than a union:

```csharp
public closed record Token(int Position);
public sealed record NumberToken(int Position, double Value) : Token(Position);
public sealed record IdentToken(int Position, string Name) : Token(Position);
public sealed record SymbolToken(int Position, char Symbol) : Token(Position);
```

(Not compiled. It is the same shape as the verified `Expr` hierarchy.) For the current payload-only tokens, the union is the smallest change.

---

## 3. Other findings (independent of the above)

| # | Severity | Where | Finding |
|---|---|---|---|
| 1 | **High** | `Visitors.cs:36` `Simplifier.VisitNeg` | `neg.Operand.Accept(this)` is evaluated **twice** whenever the operand doesn't simplify to a `Num`, so nested negations cost 2^n. Measured: 22 nested `Neg(Var)` took 172 ms and 24 took 533 ms. Compute it once (`var s = neg.Operand.Accept(this); return s is Num n ? new Num(-n.Value) : new Neg(s);`). The switch version above already does this. |
| 2 | Medium | `Visitors.cs:42` | Constant folding allocates a fresh `Evaluator`, an empty `Dictionary` and a new `Binary` just to apply one operator. Extract a static `Apply(op, l, r)`, as in the sketch above, and share it. |
| 3 | Medium | `Printer` / `Tokenize` | Printer output isn't always parseable. Folding `100000000000000000000*1` prints `1E+20`, which `Parse` rejects with "Unexpected token E". Folding `1/0` prints `Infinity`, which re-parses as a **variable** named `Infinity`. NaN behaves the same way. Either support exponent notation in the tokenizer, or format with a round-trippable non-exponent format and decide how to represent non-finite results. |
| 4 | Low | `Simplifier.VisitBinary` | Identities only fire on the right operand: `x*1` and `x+0` simplify, but `1*x` and `0+x` don't (verified). `--x` isn't reduced to `x` either. That may be intentional, but the doc comment suggests general elimination. Pattern-matching on the `(op, l, r)` tuple makes the symmetric cases one line each. |
| 5 | Low | `Evaluator.VisitBinary` / `Binary.Op` | `char` operator with `_ => throw`. See "About `char Op`" above. |
| 6 | Low | `Parser` | Error messages lack positions. "Unexpected token )" doesn't say where. `Tokenize` reports `i` for bad characters, but parse errors don't. A malformed number like `1.2.3` surfaces as `double.Parse`'s generic `FormatException`. Carrying a position on tokens (see the alternative token design in §2) fixes both. |
| 7 | Info | `Parser` | Unary `+` isn't supported (`+3` → "Unexpected token +"). Fine if intentional, but worth a test. |

---

## Suggested order of work
1. Fix the exponential `Simplifier.VisitNeg` (#1) now. It's a one-line fix and doesn't depend on the redesign.
2. Introduce `union Token(double, char, string)` along with a `Describe` helper or `ToString` override, so error messages don't regress.
3. Convert `Expr` to a `closed record` hierarchy, rewrite the three visitors as static switch-expression functions, delete `Accept`/`IExprVisitor<T>`, and add `<WarningsAsErrors>CS8509</WarningsAsErrors>`.
4. Replace `char Op` with `BinOp` and a shared `Apply`, then address #3, #4 and #6 with tests. Record equality makes those tests easy.

### Tooling note
Some C# language servers and IDE plugins still lag behind the C# 15 compiler. They may flag `union`/`closed` as errors or report false CS8509s on exhaustive switches. `dotnet build` is the source of truth. All snippets above build cleanly with SDK 11.0.100.
