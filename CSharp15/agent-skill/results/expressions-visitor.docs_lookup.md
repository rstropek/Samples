# Code review: `Expressions` (net11.0, C# 15)

Scope: `Expr.cs`, `Visitors.cs`, `Parser.cs`. No edits were made to the project. Every claim below about
behavior or compiler output was checked against the .NET 11.0.100 SDK in scratch projects (see `process.md`).

## TL;DR

1. **Visitor pattern: no, it is no longer the best fit here.** The doc comment on `Expr` says it plainly:
   *"The node types are fixed; new operations are added as visitors."* That is exactly what C# 15
   **closed hierarchies** (`public closed record Expr;`) do natively. Pattern-matching `switch` expressions
   then get **compiler-checked exhaustiveness** with no `default` arm. You can delete `Accept`/`IExprVisitor<T>`,
   and each operation becomes one short function.
2. **Tokenizer returning `object`: yes, there are better options.** The one-line fix is a C# 15 **union**,
   `public union Token(double, char, string);`. The existing `is char op` / `is not ')'` patterns keep
   compiling. The better fix is a small closed hierarchy of token records that carry their **source
   position**, so the parser can report useful errors and `char` no longer means both "operator" and
   "parenthesis".
3. There are also real bugs that are independent of the modeling question. The worst ones are an
   **exponential-time simplifier** and a **print → parse round-trip that silently turns `Infinity` into a variable**.

---

## 1. Bugs and correctness issues (most important first)

### 1.1 `Simplifier.VisitNeg` takes exponential time (Visitors.cs:36)

```csharp
public Expr VisitNeg(Neg neg) => neg.Operand.Accept(this) is Num n ? new Num(-n.Value) : new Neg(neg.Operand.Accept(this));
```

When the operand does not simplify to a `Num`, the operand is simplified **twice**. With nested negations,
each level doubles the work, so the cost is O(2^n). Measured on `Parse(new string('-', n) + "x")`:

| n  | Simplify time |
|----|---------------|
| 16 | 3 ms |
| 20 | 43 ms |
| 22 | 153 ms |

Fix: simplify once, bind the result, then branch on it. While you are there, add `--x → x`.

### 1.2 Print → Parse does not round-trip, and in one case it silently corrupts the tree (Visitors.cs:25, Parser.cs:17–27)

`Printer` uses `double.ToString(InvariantCulture)`, which can produce `Infinity`, `NaN`, `-Infinity` or
exponent notation. The tokenizer accepts only `[0-9.]+`:

- `Parse("1/0")` simplified prints `Infinity`. Re-parsing `Infinity` **succeeds** and produces
  `Var("Infinity")`, a *variable*. Nothing fails, and the meaning has changed.
- `Parse("100000000000*10000000000")` simplified prints `1E+21`. Re-parsing fails with `Unexpected token E`.

Decide on a contract: either the printer never emits these forms, or the tokenizer accepts exponents and
reserves `Infinity`/`NaN`. Also consider whether constant folding should fold to non-finite values at all.

### 1.3 Parser error messages are culture-dependent and have no positions (Parser.cs:21, 39, 82)

- `$"Unexpected token {tokens[pos]}"` formats a boxed `double` with the **current** culture. Under `de-AT`,
  `Parse("1.5 1.5")` reports `Unexpected token 1,5`, which is not what the user typed.
- `1.2.3` surfaces `double.Parse`'s raw message (`The input string '1.2.3' was not in a correct format.`)
  without a position.
- Parser errors (`Missing ')'`, `Unexpected token`) carry no position because the tokens have none.
  The tokenizer knows `i` but throws it away. This is the strongest practical argument for a real token
  type (see §3).

### 1.4 Operators are untyped `char` (Expr.cs:22, Visitors.cs:12–19)

`new Binary('%', a, b)` compiles. It fails only at evaluation time, through the `_ => throw` arm. Every
operation that inspects `Op` has to repeat that fallback. Model the operator as a type:

- `enum BinaryOp { Add, Subtract, Multiply, Divide }` is the smallest change. Note that enums are still
  **not** closed in C# 15. A switch over an enum without `_` gives **CS8524**, and `closed enum` is rejected
  with **CS0106** (both verified). Keep one `_ => throw` inside a single `Apply(op, l, r)` helper.
- Alternatively, use separate node types (`Add`, `Sub`, `Mul`, `Div`) in the closed hierarchy. Then
  exhaustiveness covers operators too, at the cost of more node types.

### 1.5 Simplifier gaps and awkward constant folding (Visitors.cs:37–46)

- Only right-hand identities are handled: `x*1` and `x+0` are simplified, but `1*x`, `0+x`, `x-0`, `x/1`
  and `--x` are not (verified: `1*x` stays `(1 * x)` and `x-0` stays `(x - 0)`).
- Constant folding builds a throwaway `Evaluator` with an empty dictionary and a throwaway `Binary`, and
  then calls `VisitBinary` directly (line 42). Pull the arithmetic into a static `Apply(op, l, r)` that both
  the evaluator and the simplifier call.

### 1.6 Nodes have reference equality

`Num`, `Var`, `Binary` and `Neg` are plain classes, so `Simplify(Parse("x+1")).Equals(Parse("x+1"))` is
`False`. That makes unit-testing the simplifier painful, and it rules out rules such as `x - x → 0`.
`sealed record` nodes give structural equality for free (verified `True` with records). They also give
positional deconstruction for patterns like `Binary(var op, var l, var r)`.

### 1.7 Unbounded recursion (Parser.cs:43–83, all visitors)

`Parse(new string('(', 100_000) + "1" + new string(')', 100_000))` **crashes the process** with a stack
overflow. A stack overflow cannot be caught. If input can come from untrusted users, add a nesting-depth
limit in `ParseUnary` and throw a `FormatException`.

### 1.8 Minor

- Unary `+` is not supported (`+3` → `Unexpected token +`).
- `Tokenize` is a lazy iterator but is always materialized with `ToList()`. That is fine, but errors surface
  partway through enumeration, so `Tokenize` and `Parse` have slightly different failure timing.
- `Printer` always wraps binaries in parentheses and prints `Neg` as `-x`. That is correct, but `--x` looks
  odd. This is cosmetic.

---

## 2. Is the visitor pattern still the best way to model this? No.

### Why the visitor existed

Before C# 15, the compiler could not know that `Num`, `Var`, `Binary` and `Neg` are *all* the subclasses of
`Expr`. The visitor was the only way to get "if I add a node type, every operation fails to compile". The
cost was the `Accept` boilerplate, an interface with one method per node, a class per operation, and
plumbing for state (constructor parameters) and extra arguments.

### What C# 15 gives you instead: closed hierarchies

```csharp
public closed record Expr;
public sealed record Num(double Value) : Expr;
public sealed record Var(string Name) : Expr;
public sealed record Binary(BinaryOp Op, Expr Left, Expr Right) : Expr;
public sealed record Neg(Expr Operand) : Expr;
```

- Direct subtypes of a `closed` class can only be declared in the same assembly. Because of that, the
  compiler treats a `switch` that covers every direct subtype as **exhaustive**, with no `default` arm.
- If you add a node type and forget to handle it, you get **CS8509** (verified: *"the pattern 'P.Mul2' is not
  covered"*). That is a warning by default, so promote it so it behaves like the visitor's compile error:

  ```xml
  <WarningsAsErrors>$(WarningsAsErrors);CS8509</WarningsAsErrors>
  ```

- Operations become plain functions. The three visitors collapse to roughly 40 lines (this version compiles
  and passes the cases in §4):

```csharp
public static double Evaluate(this Expr e, IReadOnlyDictionary<string, double> vars) => e switch
{
    Num(var v)                   => v,
    Var(var name)                => vars.TryGetValue(name, out var v) ? v
                                    : throw new KeyNotFoundException($"Unknown variable {name}"),
    Neg(var o)                   => -o.Evaluate(vars),
    Binary(var op, var l, var r) => Apply(op, l.Evaluate(vars), r.Evaluate(vars)),
};

public static Expr Simplify(this Expr e) => e switch
{
    Num or Var => e,
    Neg(var o) => o.Simplify() switch          // simplified once (fixes 1.1)
    {
        Num(var v)     => new Num(-v),
        Neg(var inner) => inner,               // --x => x
        var s          => new Neg(s),
    },
    Binary(var op, var l, var r) => (op, l.Simplify(), r.Simplify()) switch
    {
        (_, Num(var a), Num(var b))        => new Num(Apply(op, a, b)),
        (BinaryOp.Multiply, var x, Num(1)) => x,
        (BinaryOp.Multiply, Num(1), var x) => x,
        (BinaryOp.Add,      var x, Num(0)) => x,
        (BinaryOp.Add,      Num(0), var x) => x,
        (BinaryOp.Subtract, var x, Num(0)) => x,
        (BinaryOp.Divide,   var x, Num(1)) => x,
        var (o2, sl, sr)                   => new Binary(o2, sl, sr),
    },
};
```

Note: `(Multiply, var x, Num(1)) or (Multiply, Num(1), var x)` is still not allowed. Variables cannot be
declared in `or` patterns (CS8780), so commutative rules need two arms.

The simplifier is where this pays off most. Rules such as "`x*1` on either side" are **nested patterns** over
the tree shape. A visitor cannot express that; it can only dispatch on the outermost node.

### Why a closed hierarchy and not `union Expr(...)`

C# 15 also has `union` types, and `union Expr(Num, Var, Binary, Neg)` works, including recursion (verified).
For this tree it is the weaker choice:

- A union is a **struct wrapper** around a single `object Value` property (it implements `IUnion`). The node
  types share no base type, so the tree is "a struct pointing at a class pointing at a struct…".
- `default(Expr)` is a valid union value with `Value == null`. A switch on it **throws
  `SwitchExpressionException` at runtime with no compile-time warning** (verified). With a closed reference
  type, nullable reference analysis covers the null case instead.
- Unions shine when the cases are **existing, unrelated types** that you cannot or do not want to give a
  common base, such as `double`, `string`, or types from another library. Your nodes are your own types,
  so give them a closed base.

### When a visitor would still be justified

- **Many node types, most operations touch only a few.** Think Roslyn-sized trees. A visitor or rewriter
  base class with default "recurse into children" behavior saves real code. At 4 node types this does not
  apply. If you need it later, a generic bottom-up `Rewrite(Func<Expr, Expr>)` helper over the closed
  hierarchy gives the same benefit without the visitor.
- **Consumers on older compilers or older language versions.** Exhaustiveness for closed classes is a C# 15
  compiler feature. Downstream code compiled with an older language version will not get the
  "no default needed" benefit. That does not matter for this single-project codebase.
- **You need open-ended node types from other assemblies.** That conflicts with `closed` by definition, but
  your doc comment says the set is fixed.

Performance is not a reason to keep the visitor. Both approaches are a virtual call or a short chain of type
checks on sealed types, which is negligible next to the allocations in `Simplify`.

---

## 3. The tokenizer returns `object`: what is better?

Today's contract lives only in a doc comment: *"double for numbers, char for operators and parentheses,
string for identifiers"*. The compiler cannot enforce it, every consumer pattern-matches on `object`,
numbers and chars are boxed, and positions are lost.

### Option A: the minimal change, a union of the existing types

```csharp
public union Token(double, char, string);
public static IEnumerable<Token> Tokenize(string text) { ... }   // yield statements unchanged (implicit conversions)
```

- The set of token shapes becomes part of the type, and `switch` over `Token` is exhaustive without `_`.
- The parser barely changes, because patterns unwrap the union: `t[pos] is char op`, `token is double d`
  and `t[pos] is not ')'` all keep working (verified).
- Limitations: `double` and `char` are **still boxed** (the union stores `object Value`), there are still no
  positions, `char` still conflates operators with parentheses, and `default(Token)` is a null-valued token
  that a switch does not catch at compile time.

This is a good stepping stone. It is not where I would stop.

### Option B (recommended): dedicated token records with positions

```csharp
public closed record Token(int Position);
public sealed record NumberToken(double Value, int Position) : Token(Position);
public sealed record IdentToken(string Name, int Position)   : Token(Position);
public sealed record OperatorToken(BinaryOp Op, int Position) : Token(Position);
public sealed record LParenToken(int Position) : Token(Position);
public sealed record RParenToken(int Position) : Token(Position);
```

The parser then reads naturally and can report locations:

```csharp
while (pos < t.Count && t[pos] is OperatorToken { Op: BinaryOp.Add or BinaryOp.Subtract } o) ...

case LParenToken open:
    var inner = ParseSum(t, ref pos);
    if (pos >= t.Count || t[pos] is not RParenToken)
        throw new FormatException($"Missing ')' for '(' at {open.Position}");
```

Verified output: `Invalid number '1.2.3' at 0`, `Missing ')' for '(' at 0`, and
`Unexpected token RParenToken { Position = 2 } at 2`. Mapping `+ - * /` to `BinaryOp` in the tokenizer also
removes the duplicate `char → operator` decoding in the parser and evaluator.

If you prefer no shared base type, `union Token(NumberToken, IdentToken, OperatorToken, LParenToken, RParenToken)`
over the same records (without the base) also works. You lose the uniform `t.Position` access, so I would use
the closed hierarchy.

### Option C (only if profiling says so): an allocation-free struct token

`readonly record struct Token(TokenKind Kind, int Start, int Length, double Number)` with an enum `TokenKind`
avoids per-token allocations. It gives up exhaustiveness (enums are not closed) and makes invalid states
representable. Not worth it for expression-sized inputs.

---

## 4. Suggested migration order

1. Fix §1.1 (double `Accept`) now. It is a one-line change and independent of everything else.
2. Introduce `BinaryOp` and a static `Apply(op, l, r)` (§1.4, §1.5).
3. Turn `Expr` into `public closed record Expr;` with `sealed record` nodes. Add `CS8509` to
   `WarningsAsErrors`. Port `Evaluator`, `Printer` and `Simplifier` to switch-expression functions, then
   delete `Accept` and `IExprVisitor<T>`.
4. Replace `IEnumerable<object>` with the token records from Option B, and thread positions into every
   `FormatException`. Format diagnostics with `InvariantCulture` (§1.3).
5. Decide the number-format contract between printer and tokenizer (§1.2), and add a depth limit (§1.7) if
   input is untrusted.
6. Add round-trip tests (`Parse(Print(e)) == e`, which is now possible thanks to record equality) and
   simplifier tests.

A prototype of steps 2–4 was built and run in a scratch project on the 11.0.100 SDK with `CS8509` as an error.
It compiles cleanly, and on the sample inputs it produces: `1*x + 0 → x`, `--x → x`, `x - 0 → x`,
`2 * 3 + y / 1 → (6 + y)`, and `Simplify` of 30 nested negations in under 1 ms.
