# .NET 11 — Runtime Async

> My prep notes. Built and run against .NET 11 SDK `11.0.100-preview.5.26302.115` (June 2026).
> The stack traces, frame counts, and IL listings below all came out of building the
> `08_RuntimeAsync_Classic` and `09_RuntimeAsync_Enabled` projects and decompiling them with
> `ilspycmd` — not from memory.

---

## 0. The short version

Since C# 5, `async`/`await` has been a compiler trick: the C# compiler rewrites every async
method into a hidden state-machine struct (`IAsyncStateMachine`) with a `MoveNext()` method, a
builder, and saved awaiters. The CLR never knew what "async" was.

Runtime Async (.NET 11, "Runtime Async V2") moves that job into the runtime. The compiler emits
an ordinary, linear method marked with a special flag, and the runtime itself suspends and
resumes it. The payoff: no state-machine types, cleaner live stack traces, better debugging, and
lower overhead.

It's preview and opt-in, and it's transparent to my source code — I keep writing `async`/`await`
exactly as before.

---

## 1. Enabling it

```xml
<PropertyGroup>
  <Features>$(Features);runtime-async=on</Features>
  <EnablePreviewFeatures>true</EnablePreviewFeatures>
</PropertyGroup>
```

* Project must target `net11.0` and build with the .NET 11 SDK.
* No `DOTNET_RuntimeAsync=1` environment variable needed — the .NET 11 runtime runs
  runtime-async methods out of the box.
* Supported across JIT, ReadyToRun, and NativeAOT.

In my samples, project `08` omits the flag (classic) and project `09` sets it (runtime async).
The `Program.cs` is byte-for-byte identical between them — that's the whole point of the A/B.

---

## 2. The two approaches — the core of this part

Take one trivial async method:

```csharp
static async Task InnerAsync()
{
    await Task.CompletedTask;
    Console.WriteLine(new StackTrace());
}
```

[Sample Code](../src/08_RuntimeAsync_Classic/Program.cs#L29) · [runtime-async variant](../src/09_RuntimeAsync_Enabled/Program.cs#L29)

### 2.1 Classic approach — compiler-generated state machine

The compiler rewrites the method out of existence. Conceptually it produces:

```csharp
// PSEUDO-CODE of what the C# compiler emits today (classic)
[CompilerGenerated]
private struct InnerAsyncStateMachine : IAsyncStateMachine
{
    public int               _state;       // -1 = not started, 0.. = resume label
    public AsyncTaskMethodBuilder _builder; // owns the returned Task
    private TaskAwaiter      _awaiter;      // the awaited operation, saved across suspension

    public void MoveNext()
    {
        switch (_state)
        {
            case -1:                               // first entry
                _awaiter = Task.CompletedTask.GetAwaiter();
                if (!_awaiter.IsCompleted)
                {
                    _state = 0;
                    _builder.AwaitUnsafeOnCompleted(ref _awaiter, ref this); // suspend
                    return;                          // <-- unwinds the real stack!
                }
                goto case 0;
            case 0:                                 // resumed by the awaiter's continuation
                _awaiter.GetResult();
                Console.WriteLine(new StackTrace());
                _builder.SetResult();
                return;
        }
    }
    void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine sm) { }
}

// The method you wrote becomes just a launcher:
static Task InnerAsync()
{
    var sm = new InnerAsyncStateMachine { _state = -1, _builder = AsyncTaskMethodBuilder.Create() };
    sm._builder.Start(ref sm);     // calls MoveNext the first time
    return sm._builder.Task;
}
```

The matching IL — the generated nested type and its fields (`ilspycmd 08_...dll -il`):

```text
.class nested private auto ansi sealed beforefieldinit '<<<Main>$>g__InnerAsync|0_2>d'
    implements [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine
{
    .field public  int32                  '<>1__state'
    .field public  AsyncTaskMethodBuilder '<>t__builder'
    .field private TaskAwaiter            '<>u__1'
    .method ... MoveNext() { ... AwaitUnsafeOnCompleted ... SetResult ... SetException ... }
}
```

Why the stack trace gets noisy: every suspension/resumption goes through
`AsyncMethodBuilderCore.Start` → `MoveNext`. When I capture a live stack inside a synchronously
completing chain, each level shows two extra infrastructure frames. In the 3-level demo that's
13 printed frames (and 4 distinct `IAsyncStateMachine` structs in the assembly).

### 2.2 Runtime Async approach — runtime-managed suspension

The compiler leaves the method linear and just marks it `async` at the IL level
(`MethodImplAttributes.Async`, which ILSpy shows as `flags(2000)`). Each `await` becomes a single
call to a runtime intrinsic, `System.Runtime.CompilerServices.AsyncHelpers.Await(...)`:

```csharp
// PSEUDO-CODE of what the compiler emits with runtime-async=on
[MethodImpl(MethodImplOptions.Async)]   // flags(2000) — the runtime sees this
static Task InnerAsync()
{
    AsyncHelpers.Await(Task.CompletedTask);   // runtime suspends/resumes HERE, no struct
    Console.WriteLine(new StackTrace());
    // no builder, no MoveNext, no saved awaiter, no nested type
}
```

The runtime owns suspension now: when `AsyncHelpers.Await` hits an incomplete task, the runtime
captures and continues execution itself (a runtime-level continuation), so the method frame is
the real frame — there's no `MoveNext` shim in between.

The matching IL — the whole method body for `InnerAsync` (`ilspycmd 09_...dll -il`):

```text
.method assembly hidebysig static
    class System.Threading.Tasks.Task '<<Main>$>g__InnerAsync|0_2' () cil managed flags(2000)
{
    IL_0000: call  class Task System.Threading.Tasks.Task::get_CompletedTask()
    IL_0005: call  void System.Runtime.CompilerServices.AsyncHelpers::Await(class Task)
    IL_000a: ldc.i4.0
    IL_000b: newobj instance void System.Diagnostics.StackTrace::.ctor(bool)
    IL_0010: call  void System.Console::WriteLine(object)
    IL_0015: ret
}
```

22 bytes of IL, no nested type. A generic awaited result lowers to the generic overload:

```text
call !!0 System.Runtime.CompilerServices.AsyncHelpers::Await<int32>(class Task`1<!!0>)
```

The async entry point is wrapped via `AsyncHelpers::HandleAsyncEntryPoint`.

### 2.3 Side-by-side

- **Where async lives** — Classic: the C# compiler. Runtime Async: the CLR / JIT.
- **Per-method artifact** — Classic: a nested `IAsyncStateMachine` struct + `MoveNext`. Runtime Async: none — a linear method, `flags(2000)`.
- **`await x` lowers to** — Classic: `GetAwaiter` + `AwaitUnsafeOnCompleted` + state save. Runtime Async: `AsyncHelpers.Await(x)`.
- **Returned `Task`** — Classic: `AsyncTaskMethodBuilder`. Runtime Async: runtime-provided.
- **Live stack frames (3-level demo)** — Classic: 13. Runtime Async: 5.
- **`IAsyncStateMachine` types in the demo assembly** — Classic: 4. Runtime Async: 0.
- **Source changes required** — Classic: none. Runtime Async: none either, just the project flag.

---

## 3. What's actually observable (and worth demoing)

### 3.1 Live stack traces — the main one

Identical source, identical run; only the project flag differs:

```text
CLASSIC (13 frames)                              RUNTIME-ASYNC (5 frames)
  at Program.g__InnerAsync|0_2()                   at Program.g__InnerAsync|0_2()
  at AsyncMethodBuilderCore.Start[TSM](...)        at Program.g__MiddleAsync|0_1()
  at Program.g__InnerAsync|0_2()                   at Program.g__OuterAsync|0_0()
  at Program.g__MiddleAsync|0_1()                  at Program.<Main>$(String[])
  at AsyncMethodBuilderCore.Start[TSM](...)        at Program.<Main>()
  at Program.g__MiddleAsync|0_1()
  at Program.g__OuterAsync|0_0()
  at AsyncMethodBuilderCore.Start[TSM](...)
  at Program.g__OuterAsync|0_0()
  at Program.<Main>$(String[])
  at AsyncMethodBuilderCore.Start[TSM](...)
  at Program.<Main>$(String[])
  at Program.<Main>()
```

This helps anything that inspects the live execution stack: profilers, diagnostic logging, the
debugger's call-stack window.

Note to self: `StackTrace.FrameCount` reports a few more than the printed lines (it counts
`[StackTraceHidden]` frames that `ToString()` collapses). Use the printed trace for the 13-vs-5
story.

### 3.2 Exception stack traces are already fine — don't oversell this

Stack traces from `catch (Exception ex)` look the same with or without Runtime Async, because the
existing `ExceptionDispatchInfo` plumbing already cleans those up. The win is in the live stack,
debugging, and overhead — not in exception messages. Worth being upfront about it.

### 3.3 Debugging

Breakpoints bind correctly inside runtime-async methods, and stepping over `await` no longer
dives into compiler-generated infrastructure.

### 3.4 It's transparent — every async pattern still works

Project `09` also runs `AsyncPatterns.cs` to show ordinary patterns compile to runtime-async:

```text
await in a loop      -> 15
try/catch/finally    -> caught divide-by-zero
ValueTask            -> 42
Task.WhenAll         -> 1,2,3
```

[Sample Code](../src/09_RuntimeAsync_Enabled/AsyncPatterns.cs#L10)

---

## 4. Sample catalogue

- **`08_RuntimeAsync_Classic`** (console) — 3-level async chain; the classic state-machine stack trace (13 frames). [Sample Code](../src/08_RuntimeAsync_Classic/Program.cs#L17)
- **`09_RuntimeAsync_Enabled`** (console) — same source + `runtime-async=on` → 5 frames; plus the `AsyncPatterns.cs` breadth demo. [Sample Code](../src/09_RuntimeAsync_Enabled/Program.cs#L17) · [patterns](../src/09_RuntimeAsync_Enabled/AsyncPatterns.cs#L10)
- **`src/runtime-async-compare.sh`** (script) — builds both, diffs the stack traces, and greps the IL (4 state machines vs 0; shows `AsyncHelpers.Await`). [Script](../src/runtime-async-compare.sh)

There's a small reflection helper in `Program.cs` (`IsRuntimeAsyncOn`) that detects the mode at
runtime by checking whether the assembly contains any `IAsyncStateMachine` types — it prints
`False` for `08` and `True` for `09`.

---

## 5. Caveats / preview status

* Preview and opt-in. API and behaviour may change; not for production yet.
* The doc page says "last updated for Preview 2", but the `runtime-async=on` switch and the BCL
  helpers (`AsyncHelpers.Await`, `MethodImplOptions.Async`) are present and working in Preview 5.
* The performance story (fewer allocations, no boxed state machine) is the long-term motivation.
  I'll measure rather than quote specific numbers on stage.

---

## 6. Demo running order

1. Frame it (2 min): "async has always been a compiler lie — the CLR never knew." Show the
   classic state-machine pseudo-code.
2. Reveal (5 min): `08` vs `09` — same source, flip one flag, 13 frames → 5.
3. Under the hood (8 min): `runtime-async-compare.sh` — 4 `IAsyncStateMachine` structs vs 0;
   `MoveNext` switch vs a single `AsyncHelpers.Await` call; `flags(2000)` = `MethodImplOptions.Async`.
4. Be honest (2 min): exception traces are unchanged; this is about live stack/debugging/overhead.
5. Compatibility (3 min): the `AsyncPatterns` breadth demo.
6. Wrap (1 min): preview, opt-in, transparent, JIT/R2R/AOT.

---

## 7. Reproduce

```bash
dotnet tool install -g ilspycmd
dotnet build CSharp15.slnx -c Release
src/runtime-async-compare.sh
```

## 8. References

* What's new in the .NET 11 runtime (Runtime Async) — https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime#runtime-async
* .NET Runtime-Async feature tracking issue — https://github.com/dotnet/runtime/issues/109632
* .NET 11 Preview 2 runtime release notes — https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview2/runtime.md
