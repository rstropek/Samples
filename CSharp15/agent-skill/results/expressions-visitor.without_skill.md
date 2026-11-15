# Code review: `Expressions` (net11.0)

Scope: `Expr.cs`, `Visitors.cs`, `Parser.cs`. I made no edits to the project. I checked every claim below with the local .NET 11 SDK (11.0.100, default LangVersion, no `preview` needed) by building scratch copies and probing them.

## TL;DR

1. **Visitor pattern: you no longer need it here.** The node set is fixed and the operations keep growing, which is the case the visitor was invented for. C# 15 now covers that case directly with **closed hierarchies** (`public closed record Expr;`). The compiler knows every subtype, so a plain `switch` over `Expr` is checked for exhaustiveness: no `default` arm, and you get warning CS8509 when a case is missing. Records also give you value equality, `ToString`, deconstruction and `with`. That removes `Accept`, `IExprVisitor<T>` and the three visitor classes.
2. **Tokenizer returning `object`: use a union type.** `public union Token(double, char, string);` is a direct replacement. I tested it: it needs 4 type changes in `Parser.cs` plus one `.Value` in an error message. In return, invalid type patterns become compile errors and switches over `Token` are exhaustive. A better version adds positions to each case (see §3).
3. There are also some **real bugs**, independent of the modelling question: the Simplifier does exponential work on nested negation, the Printer's output does not always parse back, and the tokenizer accepts digit characters that it then rejects. See §4.

---

## 1. Is the visitor pattern still the best fit?

### What the visitor does for you today
- It forces every operation to handle every node type, because the interface requires it.
- It dispatches without type tests.

### What it costs
- Boilerplate: an `Accept` per node, a `VisitX` per node per operation, and `x.Accept(this)` repeated at every recursive call.
- Transformations become awkward. `Simplifier` has to build `new Evaluator(new Dictionary<…>()).VisitBinary(new Binary(...))` just to fold two constants. That is a sign the operation wants to be a function, not a class.
- Nested pattern matching is unavailable at the visit site. Rules such as "`x * 1` → `x`" have to be written as hand-written `if` chains.
- No structural equality. `Parse("x+1").Equals(Parse("x+1"))` is `False` today, which makes both tests and simplification rules (`x - x`) harder.

### What current C# offers: closed record hierarchy (C# 15)

```csharp
public enum BinaryOp { Add, Subtract, Multiply, Divide }

public closed record Expr;                       // 'closed' implies abstract; 'closed abstract' is error CS9384
public sealed record Num(double Value) : Expr;
public sealed record Var(string Name) : Expr;
public sealed record Neg(Expr Operand) : Expr;
public sealed record Binary(BinaryOp Op, Expr Left, Expr Right) : Expr;
```

Operations become ordinary functions. C# 14 extension blocks keep the call-site syntax (`e.Evaluate(vars)`):

```csharp
public static class ExprOperations
{
    extension(Expr e)
    {
        public double Evaluate(IReadOnlyDictionary<string, double> vars) => e switch
        {
            Num(var v)                   => v,
            Var(var name)                => vars.TryGetValue(name, out var v) ? v
                                              : throw new KeyNotFoundException($"Unknown variable {name}"),
            Neg(var operand)             => -operand.Evaluate(vars),
            Binary(var op, var l, var r) => op.Apply(l.Evaluate(vars), r.Evaluate(vars)),
        };                                // no default arm needed, and no CS8509

        public Expr Simplify() => e switch
        {
            Neg(var operand) => operand.Simplify() switch
            {
                Num(var v)       => new Num(-v),
                Neg(var inner)   => inner,               // --x => x
                var s            => new Neg(s),
            },
            Binary(var op, var l, var r) => (op, l.Simplify(), r.Simplify()) switch
            {
                (_, Num(var a), Num(var b))        => new Num(op.Apply(a, b)),
                (BinaryOp.Multiply, var x, Num(1)) => x,
                (BinaryOp.Multiply, Num(1), var x) => x,
                (BinaryOp.Add, var x, Num(0))      => x,
                (BinaryOp.Add, Num(0), var x)      => x,
                (_, var sl, var sr)                => new Binary(op, sl, sr),
            },
            Num or Var => e,
        };
    }
}
```

I compiled and ran this (together with `Print()` and an `Apply`/`Symbol` extension on `BinaryOp`):
- With all four cases present: no warnings.
- With the `Neg` arm removed: `warning CS8509 … the pattern 'Neg' is not covered`. That is the same safety net the visitor interface gave you, now enforced on every switch.
- `closed` only blocks *direct* derivation from other assemblies (`error CS9382`). Mark the cases `sealed` as above so nobody can subclass `Var` and slip past the switches.
- Switching over the `BinaryOp` enum still needs a `_ => throw new UnreachableException()` arm, because enums are not closed (CS8524). If that bothers you, model operators as separate records (`Add(Expr L, Expr R) : Expr`, …).

### Why not `union Expr(Num, Var, Neg, Binary)`?
This compiles and is also exhaustive, but it is the worse fit for an AST:
- The union is a struct wrapper around `object? Value`. `default(Expr)` is an empty union, and switching on it throws `SwitchExpressionException` at runtime, with no compile-time warning.
- The case types are unrelated to `Expr`, so there is no shared base for common members.
- Record `ToString` prints nested nodes as `Binary { Op = +, Left = Expr, Right = Expr }`, which is useless for debugging.

Unions work best for combining types that already exist and are unrelated. That describes the tokens, not the tree (§2).

### When I would keep a visitor
- The hierarchy is large (dozens of node kinds) and most passes are "rewrite one or two node kinds, recurse into everything else". A base `ExprRewriter` with default traversal, as in Roslyn or `System.Linq.Expressions.ExpressionVisitor`, saves real code. With four node kinds this does not apply. If you later need it, add a small `Expr Map(Func<Expr, Expr>)` helper to the closed hierarchy rather than going back to the visitor.
- The library must ship to consumers who cannot recompile against new node types. A visitor interface turns "new node kind" into a breaking change for them. With closed records it is only a warning.

**Recommendation:** convert `Expr` to a `closed` record hierarchy with `sealed` cases and a `BinaryOp` enum. Replace `Evaluator`, `Printer` and `Simplifier` with switch-expression functions, either as extension members or as a static class.

---

## 2. The tokenizer returns `object`. What's better?

Today the token type is effectively `double | char | string`, but it is declared as `object`, and the only record of that contract is the XML doc comment. Consequences:
- Nothing stops `yield return 42L;`, or `t[pos] is int` in the parser. Both compile and then silently fail to match.
- Every `double` and `char` is boxed.
- Error messages print whatever `object.ToString()` gives.

### Option A: minimal drop-in, a C# 15 union over the existing types

```csharp
public union Token(double, char, string);
```

Then change `IEnumerable<object>` to `IEnumerable<Token>` and `List<object>` to `List<Token>` (4 places). I did exactly that on a scratch copy and the rest of `Parser.cs` compiled unchanged. Existing patterns such as `t[pos] is char op` and `t[pos] is not ')'` work against the union. What you gain:
- `t[pos] is int` or `is long` is now **compile error CS8121**. Before, it compiled and never matched.
- `switch (token) { case double: …; case char: …; case string: … }` is exhaustive with no `default` arm.
- Implicit conversions are generated, so `yield return c;` and `yield return text[start..i];` keep working.

Two caveats I hit:
- **`Token.ToString()` returns the type name.** After the swap, `$"Unexpected token {tokens[pos]}"` prints `Unexpected token Expressions.Token`. Use `{tokens[pos].Value}` instead.
- **The compiler-generated union stores `object? Value`.** Its `double` and `char` constructors still `box` (I confirmed this in the IL). So Option A fixes type safety but not allocations. If allocations matter, write a custom union: a struct with `[Union]` that implements `IUnion` and exposes `HasValue` plus `TryGetValue(out double)`, `TryGetValue(out char)` and `TryGetValue(out string)`. I built one, and the compiler lowered the pattern switch to `TryGetValue` calls with no `box`. For an expression parser this is almost certainly not worth it.

### Option B (recommended): meaningful token cases with positions

`char` for "operator or parenthesis" and `string` for "identifier" are stand-ins for concepts. Name them, and add the source position you currently cannot report:

```csharp
public readonly record struct NumberToken(double Value, int Position);
public readonly record struct SymbolToken(char Symbol, int Position);
public readonly record struct IdentifierToken(string Name, int Position);
public union Token(NumberToken, SymbolToken, IdentifierToken);

// parser
while (pos < t.Count && t[pos] is SymbolToken { Symbol: '+' or '-' } s) { … }

static FormatException Unexpected(Token token) => new(token switch
{
    NumberToken n      => $"Unexpected number {n.Value} at {n.Position}",
    SymbolToken s      => $"Unexpected '{s.Symbol}' at {s.Position}",
    IdentifierToken id => $"Unexpected identifier '{id.Name}' at {id.Position}",
});   // exhaustive, no default
```

With this, `Parse("x )")` reports `Unexpected ')' at 2` instead of `Unexpected token )`. I compiled and ran the whole parser in this form.

A closed record hierarchy (`closed record Token; sealed record NumberToken(...) : Token`) would work just as well here. Tokens are your own new types, so either tool fits. I prefer the union for tokens because the cases can be value types and they do not need a common base. I prefer the closed hierarchy for the AST (§1). Separately, you could split operators from parentheses (`OperatorToken(BinaryOp)`, `LParen`, `RParen`) so the parser stops comparing `char`s.

---

## 3. Bugs and smaller issues (independent of the redesign)

I reproduced each of these against the unmodified sources in a scratch harness.

| # | Where | Issue | Evidence | Fix |
|---|-------|-------|----------|-----|
| 1 | `Simplifier.VisitNeg` | `neg.Operand.Accept(this)` is evaluated **twice** when the result is not a `Num`, so nested negations cost **O(2ⁿ)** | 20 nested `-` → 1,048,576 `VisitVar` calls | Store the simplified operand in a local |
| 2 | `Printer` / `Tokenizer` | Printed output does not always parse back. `double.ToString` gives `1E+20`, which the tokenizer cannot read. `NaN`/`Infinity` print as identifiers and come back as **`Var("NaN")`**, a silent change in meaning | `(1E+20 * x)` → `FormatException: Missing ')'`; `NaN` re-parses as a variable | Accept exponents in the tokenizer, and reject or special-case non-finite numbers |
| 3 | `Tokenize` | `char.IsDigit` accepts non-ASCII digits (for example `'٣'`), which the invariant-culture `double.Parse` then rejects. `1.2.3` passes the scanner too. Both produce a parse exception with **no position** | `FormatException: The input string '٣' was not in a correct format.` | Use `char.IsAsciiDigit`, `double.TryParse`, and throw your own message with `start` |
| 4 | `Tokenize` | `1e3` tokenizes as `1` followed by the identifier `e3` | `Unexpected token e3` | Scan an optional exponent and use `NumberStyles.Float` |
| 5 | `Simplifier` | Rules are asymmetric. Only `x*1` and `x+0` simplify; `1*x`, `0+x`, `x-0`, `x/1` and `--x` do not | `1*x` → `(1 * x)`, `--x` → `--x` | Pattern-based rules (§1) make these one line each |
| 6 | `Simplifier.VisitBinary` | Constant folding allocates an `Evaluator`, a `Dictionary` and a `Binary` just to apply an operator | – | Move the operator semantics to one place (`BinaryOp.Apply`) |
| 7 | `Binary.Op` is `char` | Any `char` is accepted, so both `Evaluator` and `Simplifier` need "unknown operator" throw arms that can never be correct | – | `BinaryOp` enum or one record per operator |
| 8 | `Printer.VisitNeg` | `Neg(Num(-3))` prints `--3`. It parses back, but as `Neg(Neg(3))`, not the original shape | `--3` | Parenthesize negative literals or non-atomic operands |
| 9 | `Parser` | No unary `+`, and identifiers cannot start with `_` | `+x` → `Unexpected token +` | Decide whether these are in scope; if so, add them |
| 10 | General | Recursive descent and recursive visitors have no depth limit, so very deep input (for example tens of thousands of nested parentheses) can overflow the stack and kill the process; this cannot be caught (not reproduced, by design) | – | Add a depth counter in the parser if the input is untrusted |
| 11 | Style | `Var var` / `var.Name` is legal but confusing to read | – | Rename the parameter (`v`, `variable`) |
| 12 | Style | `Tokenize` is a lazy iterator, but `Parse` immediately materializes it to a `List<object>` (and boxes every element) | – | Fine to keep. With `Token` the list is at least typed |

## 4. Suggested order of work

1. Fix #1 (exponential Simplifier) and #3/#4 (tokenizer robustness). These are small and do not depend on anything else.
2. Introduce `union Token(...)`. Option A is a 5-line change; Option B adds positions.
3. Convert `Expr` to a `closed` record hierarchy with `BinaryOp`, and port the three visitors to switch expressions. The pattern-based Simplifier fixes #5–#7 along the way.
4. Add round-trip tests (`Parse(Print(e)) == e`), which become one line once records provide value equality. These would have caught #2 and #8.
