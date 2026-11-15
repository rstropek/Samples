# Teaching an AI agent C# 15: a skill experiment

C# 15 / .NET 11 shipped in November 2026, after the training cutoff of most coding agents. This part of
the workshop asks a simple question: **does a short skill about union types and closed hierarchies make an
agent's code reviews better?**

- **The skill:** [`../.claude/skills/csharp15-unions/SKILL.md`](../.claude/skills/csharp15-unions/SKILL.md)
  (about 110 lines). It is a project skill, so Claude Code loads it automatically in this repo.
- **The test cases:** [`../.claude/skills/csharp15-unions/evals/`](../.claude/skills/csharp15-unions/evals/).
  There are four small C# 14-style libraries (`files/`), a realistic review prompt for each, and graders.
  The format is the one `claude plugin eval` reads.
- **The results:** [`results/`](results/). It holds every review, the benchmark, and the no-tools and
  memory-quiz runs.

## The four "old syntax" libraries

| Case | Library | What a good C# 15 review spots | Trap |
|---|---|---|---|
| `billing-modernize` | `Billing` | `abstract record` with a `private protected` constructor and `_ => throw UnreachableException` should become `closed`; a flag-and-nullable-fields `RefundOutcome` should become a closed hierarchy or union | `PaymentProvider` is extended by other packages and must **not** be closed |
| `functional-helpers` | `Functional` | a hand-written OneOf-style `Either<T0,T1>` and a `Result<T>` class should become unions | `Option<T>` sits on a hot path, and a shorthand union boxes; `Result<T>(T, string)` and `Either<string,string>` are ambiguous (CS0457) |
| `telemetry-json` | `Telemetry.Contracts` | a custom `JsonConverter` for number-or-string, a `JsonDocument`-sniffing converter, and a `[JsonDerivedType]` list (missing one type) | Native AOT and source generation must keep working |
| `expressions-visitor` | `Expressions` | the visitor pattern can become a `closed` hierarchy with switch expressions; the tokenizer returns `object` and should return a union | the union's `ToString()` isn't forwarded, so error messages change |

## Results (Opus 5.5, one run per configuration)

There are three configurations:
- **with_skill:** the agent reads `SKILL.md`. No lookups were used.
- **without_skill:** isolated. The agent has its own knowledge and the local .NET 11 SDK, but no
  docs, web, or repo access.
- **docs_lookup:** unrestricted. These agents used a global Context7 (`ctx7`) rule and/or read this repo's
  `docs/`.

| | with_skill | without_skill | docs_lookup |
|---|---|---|---|
| Assertions passed | 23/23 | 23/23 | 23/23 |
| Mean time per review | **168 s** | 279 s | 267 s |
| Mean tokens per review | **68k** | 81k | 79k |

**Finding 1: the hypothesis "the agent won't suggest C# 15 features" was wrong for Opus 5.5 with a shell.**
Union and `closed` previews predate the model's training cutoff, so it knows the basic syntax. Where it
doesn't know something, it **finds out**:
- It compiles probe projects.
- It greps the XML docs shipped in the SDK reference pack (`packs/Microsoft.NETCore.App.Ref/11.0.0/ref/net11.0/System.Text.Json.xml`).

That is how the isolated baseline discovered `JsonUnion`, `JsonUnionTypeStructuralClassifier` and
`InferClosedTypePolymorphism` on its own.

**Finding 2: with tools, the skill buys speed and consistency, not correctness.** It cut time by about 40%
and tokens by about 15%, and it caught a few things the others missed:
- Only the skill run named `Result<Error>`/`Either<string,string>` (CS0457).
- One baseline made a false claim about `default(Either)`.

In turn, the baselines' experiments turned up verified pitfalls that were folded back into the skill (v2):
- `closed` without `InferClosedTypePolymorphism` serialises as `{}`.
- SYSLIB1244 and SYSLIB1227.
- The structural classifier is lenient by default.

**Finding 3: without tools, the skill changes the outcome.** In this setup the code is pasted into the
prompt and there is no shell, as with a PR-review bot or a chat:
- **Without the skill**, the model gives pre-C# 15 advice: a flat wire DTO and keeping the converters. It
  says it can't confirm that .NET 11 STJ supports unions.
- **With the skill**, it recommends `closed` + `InferClosedTypePolymorphism`, a structural-classifier union,
  and `union SensorValue(double, string)`.

See [`results/notools-telemetry-without-skill.md`](results/notools-telemetry-without-skill.md) and
[`results/notools-telemetry-with-skill.md`](results/notools-telemetry-with-skill.md). The memory quiz
([`results/memory-quiz.md`](results/memory-quiz.md)) shows the same gaps: the model is unsure about
`closed`, the STJ APIs, and whether the features are still preview in GA.

**Finding 4: a docs-lookup rule is a real alternative.** The `ctx7` rule (or a repo that already contains
C# 15 docs) got the baseline to the same quality, but it was not faster than exploring the SDK.

Caveats: this uses one model, a single run per configuration, and four cases. The first assertion set
didn't separate the configurations. The evals now contain sharper graders (`07-…`, `08-…`) and a no-tools
case, `telemetry-json-notools`.

## The C# LSP plugin: does it help?

Not yet. Today it can hurt.

- **Wrong results:** `csharp-ls` 0.28.0, the newest version on NuGet and the one used by the
  `csharp-lsp` plugin, bundles a Roslyn from before C# 15 GA. On code that builds with zero warnings, it
  reports:
  - `union` → CS0246/CS1001
  - `closed` → "CS8652: closed classes is in Preview"
  - every exhaustive switch → a false CS8509

  An agent that trusts these will "fix" valid code. It might add `LangVersion preview`, add `_` arms, or
  drop `closed`.
- **Limited scope:** diagnostics reach only the **main** session. The subagent in our test never saw them. They cover
  only projects in the loaded solution (`CSharp15.slnx`); scratch projects and projects added later are
  ignored until the server restarts.
- **The skill's fix:** it says that `dotnet build` is the source of truth and that LSP errors on
  `union`/`closed` should be ignored.

## Running it yourself

**Live, in a session:** follow [DEMO.md](DEMO.md). It has three demos (LSP false alarms, review with and
without the skill, and review without tools), with exact commands, expected output, fallbacks and reset steps.

**Automated:** from `.claude/skills/csharp15-unions/`, run
`claude plugin eval . --allow-tools Write,Edit,Bash --scaffold`. These are real, billed model runs.
