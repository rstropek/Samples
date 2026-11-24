# C# 15 & .NET 11 — Talk Solution

My working repo for a single talk covering two .NET 11 features:

1. C# 15 Union Types (discriminated unions) — see [`docs/01-CSharp15-Unions.md`](docs/01-CSharp15-Unions.md),
   including closed hierarchies, custom (non-boxing) unions, unions in System.Text.Json, and the
   smaller C# 15 features (collection expression arguments, extension indexers, labeled
   `break`/`continue`)
2. .NET 11 Runtime Async — see [`docs/02-RuntimeAsync.md`](docs/02-RuntimeAsync.md)
3. Workshop add-on: teaching AI agents C# 15 with a skill — see [`agent-skill/README.md`](agent-skill/README.md)
   (the skill itself is the project skill [`.claude/skills/csharp15-unions`](.claude/skills/csharp15-unions/SKILL.md))

Each doc has my notes: a feature walkthrough, a deep dive into the implementation (decompiled C#
plus IL from `ilspycmd`), a set of runnable samples, the gotchas I ran into, and a demo running
order.

> Originally written against .NET 11 SDK `11.0.100-preview.5.26302.115` (June 2026); re-verified
> against the .NET 11 GA SDK `11.0.100` (November 2026) with the default language version (C# 15,
> no `LangVersion=preview`). Every code block and IL listing in the docs was compiled and run.

## Layout

```
CSharp15.slnx                     # .NET 11 XML solution
Directory.Build.props             # shared: net11.0 (C# 15 by default), nullable
global.json                       # pins the .NET 11 GA SDK (11.0.100, roll forward to latest feature band)
docs/
  01-CSharp15-Unions.md           # concept: union types
  02-RuntimeAsync.md              # concept: runtime async (state machine vs runtime, pseudo-code + IL)
src/
  00_Rust/                        # [rust]   the same demos in Rust (enum + Option/Result); not in the .slnx
  01_Basics/                      # [unions] console — declaration, conversion, exhaustive switch
  02_ResultOption/                # [unions] classlib — Option<T> / Result<T,E> + combinators (extension blocks)
  03_RecursiveExpr/               # [unions] console — recursive AST evaluator + printer
  04_DomainModeling/              # [unions] console — state machine, illegal states unrepresentable
  05_PatternShowcase/             # [unions] console — the main pattern forms through a union
  06_Lowering/                    # [unions] classlib — types for the IL deep dive (+ decompile.sh)
  07_CollectionExprArgs/          # [bonus]  console — collection expression arguments (with(...))
  08_RuntimeAsync_Classic/        # [async]  console — classic compiler state machine (13 frames)
  09_RuntimeAsync_Enabled/        # [async]  console — runtime-async=on (5 frames) + patterns demo
  10_ResultOptionUsage/           # [unions] console — real-world Option/Result usage (consumes 02)
  11_UnionWithMembers/            # [unions] console — union with custom ToString/methods (3 forms)
  12_ClosedHierarchies/           # [closed] console — `closed` records: exhaustive without a union
  13_CustomUnions/                # [unions] console — non-boxing [Union] struct, member provider (+ decompile.sh)
  14_JsonUnions/                  # [unions] console — System.Text.Json: unions + closed polymorphism (vs C# 14 converters)
  15_IndexersAndLabels/           # [bonus]  console — extension indexers, labeled break/continue
  16_MemorySafety/                # [preview] console — memory safety rules: requires-unsafe, Base45 over Span<T>
  99_Vaccination/                 # [workshop] console — end-to-end: vaccination center bookings; STORYBOOK.md (EN) / STORYBOOK_de.md (DE) = live-coding script
  runtime-async-compare.sh        # runs 08 vs 09, prints both stack traces + greps the IL
tests/
  Csharp15.Tests/                 # xUnit — 10 tests over Option/Result
```

## Prerequisites

* .NET 11 SDK (`11.0.100` or later feature band; see `global.json`).
* For the IL deep dives: `dotnet tool install -g ilspycmd`.

## Build, test, run

```bash
dotnet build CSharp15.slnx -c Release          # 0 warnings, 0 errors
dotnet test  tests/Csharp15.Tests              # Passed!  Passed: 10

# Union samples
dotnet run --project src/01_Basics
dotnet run --project src/03_RecursiveExpr
dotnet run --project src/04_DomainModeling
dotnet run --project src/05_PatternShowcase
dotnet run --project src/07_CollectionExprArgs
dotnet run --project src/10_ResultOptionUsage
dotnet run --project src/11_UnionWithMembers
dotnet run --project src/12_ClosedHierarchies
dotnet run --project src/13_CustomUnions
dotnet run --project src/14_JsonUnions
dotnet run --project src/15_IndexersAndLabels

# Memory safety (preview: LangVersion=preview + updated-memory-safety-rules, only in this project)
dotnet run --project src/16_MemorySafety

# Workshop end-to-end sample (reads data/ relative to the current folder)
(cd src/99_Vaccination && dotnet run)

# Runtime async (run each directly, or both via the compare script below)
dotnet run --project src/08_RuntimeAsync_Classic -c Release
dotnet run --project src/09_RuntimeAsync_Enabled -c Release

# Deep dives
src/06_Lowering/decompile.sh                   # union -> struct + IUnion, switch IL
src/13_CustomUnions/decompile.sh               # TryGetValue (no boxing) vs get_Value + unbox
src/runtime-async-compare.sh                   # state machine vs runtime async

# Rust counterpart (needs a Rust toolchain, https://rustup.rs)
(cd src/00_Rust && cargo run)                  # enum + Option/Result, parallels 01_Basics & 02
```

## One-line reminder for each feature

* Union types: `union Pet(Cat, Dog, Bird);` lowers to a `struct : IUnion` holding `object? Value`;
  conversions are constructor calls, `switch` is an `isinst` ladder with a `ThrowSwitchExpressionException`
  safety net, and exhaustiveness is CS8509 (a warning). Hand-written `[Union]` types can add
  `HasValue`/`TryGetValue` to avoid boxing.
* Closed hierarchies: `closed record Pet;` — only the declaring assembly may derive directly, so a
  switch over `Pet` is exhaustive without `_`. Unions = "one of these existing types";
  closed = "one of my subtypes".
* Runtime async: `<Features>runtime-async=on</Features>` makes the runtime, not the compiler,
  manage suspension — no `IAsyncStateMachine` types, `await x` becomes `AsyncHelpers.Await(x)`,
  methods are flagged `MethodImplOptions.Async`, and live stack traces shrink (13 → 5 frames here).
* Memory safety (preview): `LangVersion=preview` + `<Features>updated-memory-safety-rules</Features>`.
  Creating pointers is fine, dereferencing needs `unsafe` (CS9360); `unsafe` on a member means
  requires-unsafe for its callers (CS9362), including BCL APIs like `Unsafe.Add` and
  `MemoryMarshal.GetReference`; the body needs its own `unsafe { }`; `extern` needs `safe`/`unsafe`.
