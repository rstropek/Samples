# C# 15 & .NET 11 — Talk Solution

My working repo for a single talk covering two .NET 11 features:

1. C# 15 Union Types (discriminated unions) — see [`docs/01-CSharp15-Unions.md`](docs/01-CSharp15-Unions.md)
2. .NET 11 Runtime Async — see [`docs/02-RuntimeAsync.md`](docs/02-RuntimeAsync.md)

Each doc has my notes: a feature walkthrough, a deep dive into the implementation (decompiled C#
plus IL from `ilspycmd`), a set of runnable samples, the gotchas I ran into, and a demo running
order.

> Built and run against .NET 11 SDK `11.0.100-preview.5.26302.115`, `LangVersion=preview`,
> June 2026. Every code block and IL listing in the docs was compiled and run.

## Layout

```
CSharp15.slnx                     # .NET 11 XML solution
Directory.Build.props             # shared: net11.0, LangVersion=preview, nullable
global.json                       # pins the .NET 11 preview SDK
docs/
  01-CSharp15-Unions.md           # concept: union types
  02-RuntimeAsync.md              # concept: runtime async (state machine vs runtime, pseudo-code + IL)
src/
  00_Rust/                        # [rust]   the same demos in Rust (enum + Option/Result); not in the .slnx
  01_Basics/                      # [unions] console — declaration, conversion, exhaustive switch
  02_ResultOption/                # [unions] classlib — Option<T> / Result<T,E> + combinators (extension blocks)
  03_RecursiveExpr/               # [unions] console — recursive AST evaluator + printer
  04_DomainModeling/              # [unions] console — state machine, illegal states unrepresentable
  05_PatternShowcase/             # [unions] console — every pattern form through a union
  06_Lowering/                    # [unions] classlib — types for the IL deep dive (+ decompile.sh)
  07_CollectionExprArgs/          # [bonus]  console — collection expression arguments (with(...))
  08_RuntimeAsync_Classic/        # [async]  console — classic compiler state machine (13 frames)
  09_RuntimeAsync_Enabled/        # [async]  console — runtime-async=on (5 frames) + patterns demo
  10_ResultOptionUsage/           # [unions] console — real-world Option/Result usage (consumes 02)
  11_UnionWithMembers/            # [unions] console — union with custom ToString/methods (3 forms)
  runtime-async-compare.sh        # builds 08 vs 09, diffs stack traces + IL
tests/
  Csharp15.Tests/                 # xUnit — 10 tests over Option/Result
```

## Prerequisites

* .NET 11 preview SDK (`global.json` pins the exact version I used).
* For the IL deep dives: `dotnet tool install -g ilspycmd`.

## Build, test, run

```bash
dotnet build CSharp15.slnx -c Release          # 0 warnings, 0 errors
dotnet test  tests/Csharp15.Tests              # Passed!  Passed: 10

# Union samples
dotnet run --project src/01_Basics
dotnet run --project src/03_RecursiveExpr
dotnet run --project src/05_PatternShowcase
dotnet run --project src/07_CollectionExprArgs
dotnet run --project src/10_ResultOptionUsage

# Deep dives
src/06_Lowering/decompile.sh                   # union -> struct + IUnion, switch IL
src/runtime-async-compare.sh                   # state machine vs runtime async

# Rust counterpart (needs a Rust toolchain, https://rustup.rs)
(cd src/00_Rust && cargo run)                  # enum + Option/Result, parallels 01_Basics & 02
```

## One-line reminder for each feature

* Union types: `union Pet(Cat, Dog, Bird);` lowers to a `struct : IUnion` holding `object? Value`;
  conversions are constructor calls, `switch` is an `isinst` ladder with a `ThrowSwitchExpressionException`
  safety net, and exhaustiveness is CS8509 (a warning).
* Runtime async: `<Features>runtime-async=on</Features>` makes the runtime, not the compiler,
  manage suspension — no `IAsyncStateMachine` types, `await x` becomes `AsyncHelpers.Await(x)`,
  methods are flagged `MethodImplOptions.Async`, and live stack traces shrink (13 → 5 frames here).
