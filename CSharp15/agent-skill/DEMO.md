# Demo script: AI agents and C# 15 (about 20 minutes)

There are three short demos. Background and full results are in [README.md](README.md). Every command
here was dry-run on macOS with SDK 11.0.100, Claude Code and Opus 5.5. Times and costs vary from run to
run, so treat the numbers as ballpark.

| # | Demo | Message | Time |
|---|---|---|---|
| 1 | LSP false alarms | The wrong tool misleads the agent | about 5 min |
| 2 | Review with and without the skill | Same quality, about 2× faster and cheaper | about 8 min |
| 3 | Review without tools | Without tools, the skill changes the answer | about 5 min |

If a live run fails, every demo has prepared output in [`results/`](results/).

---

## 0. Preparation (before the session)

1. **Tools:**
   - `dotnet --version` should print `11.0.100` or later.
   - `claude --version`.
   - For Demo 1: the `csharp-lsp` plugin is enabled (`/plugin`) and `csharp-ls` is installed (`dotnet tool install -g csharp-ls`).
2. **Global rules can leak answers to the baseline.** A global rule that fetches docs, such as
   `~/.claude/rules/context7.md`, or a global `CLAUDE.md` gives the "without skill" run the answers. Either
   move the rule aside for the session (`mv ~/.claude/rules/context7.md ~/.claude/rules/context7.md.off`),
   or keep the `--append-system-prompt` override used in Demo 2.
3. **Demo folders.** They must be outside this repo, so the agent can't read `docs/`. Run this from the
   repo root:

   ```bash
   DEMO=/tmp/cs15-demo; rm -rf $DEMO; mkdir -p $DEMO/without $DEMO/with
   LIB=.claude/skills/csharp15-unions/evals/telemetry-json/files/Telemetry.Contracts
   cp -R $LIB $DEMO/without/ && cp -R $LIB $DEMO/with/
   # copy only SKILL.md: the evals folder contains the expected answers
   mkdir -p $DEMO/with/Telemetry.Contracts/.claude/skills/csharp15-unions
   cp .claude/skills/csharp15-unions/SKILL.md $DEMO/with/Telemetry.Contracts/.claude/skills/csharp15-unions/
   ```

4. **Warm up:** run `dotnet build` once in each copy, so the first live build isn't slow.

---

## Demo 1: the language server raises false alarms (about 5 min)

**Story:** the `csharp-lsp` plugin gives Claude live compiler diagnostics. The current `csharp-ls` (0.28.0)
bundles a C# compiler from **before** C# 15 GA, so it reports valid C# 15 code as broken.

1. Take the skill away, so Claude has nothing that tells it to distrust the LSP:
   ```bash
   mv .claude/skills .claude/skills.off
   ```
2. Start `claude` in the repo root and type:
   > Add a new pet `Hamster(string Name)` as a direct subtype of `Pet` in
   > `src/12_ClosedHierarchies/Program.cs`, add one to the `pets` array and update the switches.
   > After your first edit, tell me exactly which diagnostics you received.
3. **What to show:** after the edit, Claude gets diagnostics like these:
   ```
   ✘ CS8652 The feature 'closed classes' is currently in Preview and *unsupported*.
   ⚠ CS8509 The switch expression does not handle all possible values ... the pattern '_' is not covered.
   ```
   Both are false. The real compiler disagrees:
   ```bash
   dotnet run --project src/12_ClosedHierarchies     # builds with 0 warnings
   ```
   Talking points:
   - Does Claude want to add `LangVersion preview` or `_ =>` arms? Either would make the code worse.
   - Only the main session receives LSP diagnostics. Subagents don't.
   - Diagnostics cover only projects in the solution that was loaded when the session started.
4. **Reset:**
   ```bash
   git checkout -- src/12_ClosedHierarchies/Program.cs
   mv .claude/skills.off .claude/skills
   ```

---

## Demo 2: the same review with and without the skill (about 8 min)

**Story:** Opus 5.5 can work out C# 15 by itself, but that takes a while. The skill hands it the facts.

Start both runs at the same time in two terminal panes. Each run takes 1 to 2 minutes.

```bash
PROMPT="Our Telemetry.Contracts library has accumulated custom JSON converters and attribute lists. We target net11.0 and publish the client with Native AOT. Review the serialization code and suggest how to simplify it. Review only, no edits."
NODOCS="For this session, do not use ctx7/Context7, web search or any other documentation lookup; rely on your own knowledge and the local .NET SDK."
```

```bash
# pane 1: without skill
cd /tmp/cs15-demo/without/Telemetry.Contracts
claude -p --output-format stream-json --verbose --append-system-prompt "$NODOCS" "$PROMPT" > ../run.jsonl
```

```bash
# pane 2: with skill (a project skill in ./.claude/skills, picked up automatically)
cd /tmp/cs15-demo/with/Telemetry.Contracts
claude -p --output-format stream-json --verbose --append-system-prompt "$NODOCS" "$PROMPT" > ../run.jsonl
```

Compare the two runs:

```bash
for c in without with; do
  echo "== $c"
  jq -rR 'fromjson? | select(.type=="assistant") | .message.content[] | select(.type=="tool_use")
          | "  \(.name): \(.input.skill // .input.command // "" | tostring | split("\n")[0] | .[0:90])"' /tmp/cs15-demo/$c/run.jsonl
  jq -rR 'fromjson? | select(.type=="result")
          | "  -> \(.duration_ms/1000|floor) s, \(.num_turns) turns, $\(.total_cost_usd*100|round/100)"' /tmp/cs15-demo/$c/run.jsonl
done
# full review text:
jq -rR 'fromjson? | select(.type=="result") | .result' /tmp/cs15-demo/with/run.jsonl
```

**Dry-run numbers:**

| | Without skill | With skill |
|---|---|---|
| Time | 118 s | **59 s** |
| Turns | 13 | **7** |
| Cost | $0.67 | **$0.40** |

What to show:
- **The with-skill run starts with `Skill(csharp15-unions)`.** The skill loads by itself, without being named in the prompt.
- **The without-skill run explores.** It greps `packs/Microsoft.NETCore.App.Ref/11.0.0/ref/net11.0`
  (the SDK ships XML docs) and compiles probe projects. In the dry run it first tried `abstract closed`
  and had to fix it.
- **Both runs give the same recommendations:**
  - `[JsonPolymorphic(InferClosedTypePolymorphism = true)] closed record DeviceEvent`
  - `[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))] union IngestResponse`
  - `union SensorValue(double, string)`
  - the `DeviceRebooted` bug

  The skill doesn't make it smarter here. It makes it **faster and cheaper**.

For an interactive version, run plain `claude` in both folders, paste `$PROMPT`, and compare `/cost` at the
end.

**Fallback:** [`results/benchmark.md`](results/benchmark.md) has 4 libraries and 12 runs, about 40% less time
and 15% fewer tokens with the skill. The reviews are in `results/telemetry-json.*.md`.

---

## Demo 3: no tools, as in a PR bot or a chat (about 5 min)

**Story:** without a shell, the agent can't explore the SDK. Now it only has what it remembers, and the skill
makes the difference.

This reuses `$PROMPT` from Demo 2. Each run takes about 30–60 s.

```bash
cd /tmp/cs15-demo
{ echo "$PROMPT Be concise (max ~60 lines)."
  for f in without/Telemetry.Contracts/*.csproj without/Telemetry.Contracts/*.cs; do
    echo; echo "=== $(basename $f)"; cat $f; done; } > prompt.txt
{ echo "Use the following skill (verified reference notes) when answering:"; echo "<skill>"
  cat with/Telemetry.Contracts/.claude/skills/csharp15-unions/SKILL.md; echo "</skill>"; echo
  cat prompt.txt; } > prompt-skill.txt

NOTOOLS="Bash,Read,Write,Edit,Glob,Grep,WebSearch,WebFetch,Agent,Skill"
claude -p --disallowedTools "$NOTOOLS" < prompt.txt       > notools-without.md &
claude -p --disallowedTools "$NOTOOLS" < prompt-skill.txt > notools-with.md &
wait
```

What to show, side by side:
- **`notools-without.md`:** pre-C# 15 advice, such as a flat wire DTO, keeping the `SensorValue` converter,
  and `double?`/`string?` fields. It explicitly says it **can't confirm** whether .NET 11 System.Text.Json
  supports unions.
- **`notools-with.md`:** `closed` + `InferClosedTypePolymorphism`, the `JsonUnion` structural classifier,
  `union SensorValue(double, string)`, CS8509 as an error, and "ignore the LSP, `dotnet build` is the source
  of truth".

**Fallback:** [`results/notools-telemetry-without-skill.md`](results/notools-telemetry-without-skill.md) and
[`results/notools-telemetry-with-skill.md`](results/notools-telemetry-with-skill.md).

Optional 1-minute extra: [`results/memory-quiz.md`](results/memory-quiz.md) shows what the model says about
C# 15 from memory alone. It is sure about `union` and unsure about `closed`, the STJ APIs, and whether the
features are still preview.

---

## After the session

```bash
rm -rf /tmp/cs15-demo
mv ~/.claude/rules/context7.md.off ~/.claude/rules/context7.md   # if you moved it
git status                                                        # should be clean
```
