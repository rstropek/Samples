# Billing library: .NET 11 / C# 15 modernization review

Scope: `Billing.csproj` (net11.0), `PaymentResult.cs`, `RefundOutcome.cs`, `PaymentService.cs`, `PaymentProvider.cs`.
No code was changed. Every snippet below was compiled with the local .NET SDK 11.0.100 (C# 15) in a scratch copy of the project. Where this review says how something behaves, that behavior was checked in that copy.

## Summary

| # | Priority | Change | Breaking for adapter packages? |
|---|----------|--------|--------------------------------|
| 1 | **High: do now** | Make `PaymentResult` a `closed` record and drop the `_ =>` / `throw` fallbacks so the compiler checks that every switch covers all cases | No |
| 2 | **High: do now** | Turn the `CS8509` warning (switch doesn't cover every case) into a build error | No |
| 3 | **High: plan for next major** | Replace the flag-based `RefundOutcome` class with a C# 15 `union` | **Yes** (source and binary) |
| 4 | **Do NOT do** | Mark `PaymentProvider` as `closed`, or turn `PaymentResult` into a `union` | Would break every adapter |
| 5 | Low | Small fixes: ordinal `StartsWith`, `ConfigureAwait`, currency formatting, optional extension members | No |

---

## 1. `PaymentResult`: replace the `private protected` workaround with a `closed` hierarchy (High, non-breaking)

**Today**

```csharp
public abstract record PaymentResult
{
    private protected PaymentResult() { }
}
```

The `private protected` constructor stops other assemblies from adding new cases. The compiler doesn't know about that rule, though, so every `switch` still needs a fallback arm:

```csharp
_ => throw new UnreachableException($"Unknown payment result {result.GetType().Name}"),
```

`ToHttpStatus` has the same problem with a trailing `throw new InvalidOperationException(...)`. If someone adds a fourth result type (for example `Cancelled`), both methods still compile and only fail at runtime.

**Suggested (C# 15)**

```csharp
namespace Billing;

/// <summary>Outcome of a charge attempt. Only the cases declared in this assembly exist.</summary>
public closed record PaymentResult;

public sealed record Approved(string TransactionId, decimal Amount) : PaymentResult;
public sealed record Declined(string Reason, bool Retryable) : PaymentResult;
public sealed record RequiresAction(Uri RedirectUrl) : PaymentResult;
```

What I checked with the local compiler:
- `closed` implies `abstract`, so `new PaymentResult()` gives error CS0144.
- Deriving from a `closed` type in another assembly gives error **CS9382** ("cannot use a closed type ... from another assembly as a base type"). This happened even when the consuming project used `LangVersion 14` with the new compiler.
- The compiler records the allowed subtypes as `[IsClosedType(DerivedTypes = ...)]` on the type and uses that list to check that switches cover every case.
- `closed` and the existing `private protected` constructor compile together. With `closed`, the constructor is redundant for C# callers. You can keep it as a backstop for other languages or older compilers that may not honor `closed`. Either way, the doc comment no longer has to explain the trick.
- The adapters are unaffected. They already can't derive from `PaymentResult`, and the base type stays an abstract reference type, so `Task<PaymentResult>` and `new Approved(...)` keep working unchanged.

**Then remove the fallback arms in `PaymentService`:**

```csharp
public static string Describe(PaymentResult result) => result switch
{
    Approved a       => $"Approved {a.Amount:C} ({a.TransactionId})",
    Declined d       => $"Declined: {d.Reason}{(d.Retryable ? " (retry later)" : "")}",
    RequiresAction r => $"Customer action required at {r.RedirectUrl}",
};   // no "_ =>" arm, and the System.Diagnostics using can go

public static int ToHttpStatus(PaymentResult result) => result switch
{
    Approved                     => 200,
    Declined { Retryable: true } => 503,
    Declined                     => 402,
    RequiresAction               => 202,
};
```

Notes:
- Keeping the `_ =>` arm cancels the benefit: a discard matches everything, so the compiler never warns when a case is missing. Delete it.
- `ToHttpStatus` should become a switch **expression**. An `if (x is ...)` chain or a switch *statement* never gets a completeness check.
- The compiler still emits a hidden fallback that throws `SwitchExpressionException`, so a `null` that slips past nullable analysis still fails clearly.

Keep the case records `sealed`. In a probe, a non-sealed subtype inside the closed hierarchy was reported as a separate uncovered case. An open case type also lets other code derive from it.

## 2. Make a missing case a build error (High, non-breaking)

Once item 1 is in place, a missing case shows up as warning **CS8509**. Promote it to an error in `Billing.csproj`:

```xml
<PropertyGroup>
  <TargetFramework>net11.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <WarningsAsErrors>$(WarningsAsErrors);CS8509</WarningsAsErrors>
</PropertyGroup>
```

I checked this: adding `public sealed record Cancelled() : PaymentResult;` to the modernized copy broke the build at both `Describe` and `ToHttpStatus` with `error CS8509 ... the pattern 'Billing.Cancelled' is not covered`. That is the guarantee the `private protected` constructor was meant to give, now enforced by the compiler.

## 3. `RefundOutcome`: model it as a union instead of flags (High value, breaking: plan it)

**Today** the class relies on a convention that only the doc comment states ("exactly one of the following is true"). Nothing enforces it:
- `RefundOutcome` has an implicit **public parameterless constructor** and public `init` setters. Any adapter can write `new RefundOutcome()` (not succeeded, no error code, not pending) or `new RefundOutcome { Succeeded = true, ErrorCode = "x" }`.
- `PaymentService.RefundAsync` handles those invalid states silently: `new RefundOutcome()` produces `"Refund failed: "` with an empty error code.
- The check order in `RefundAsync` (`Succeeded`, then `PendingUntil`, then failure) is an unstated rule about which fields win.

**Suggested (C# 15 `union`)**

```csharp
namespace Billing;

public sealed record RefundSucceeded(string RefundId);
public sealed record RefundFailed(string ErrorCode);
public sealed record RefundPending(DateTimeOffset Until);

/// <summary>Result of a refund request: exactly one of the three cases.</summary>
public union RefundOutcome(RefundSucceeded, RefundFailed, RefundPending)
{
    // Keep the factories: adapters that only use them compile unchanged.
    public static RefundOutcome Success(string refundId)      => new RefundSucceeded(refundId);
    public static RefundOutcome Failure(string errorCode)     => new RefundFailed(errorCode);
    public static RefundOutcome Pending(DateTimeOffset until) => new RefundPending(until);
}
```

```csharp
// PaymentService
public async Task<string> RefundAsync(string transactionId, CancellationToken ct = default) =>
    await provider.RefundAsync(transactionId, ct) switch
    {
        RefundSucceeded(var refundId) => $"Refunded ({refundId})",
        RefundPending(var until)      => $"Refund queued until {until:u}",
        RefundFailed(var errorCode)   => $"Refund failed: {errorCode}",
    };
```

`FakeProvider.RefundAsync` compiles **unchanged** because it only uses the factories. You can also use the case types directly, since each one converts implicitly to the union, including inside a conditional:

```csharp
Task.FromResult<RefundOutcome>(transactionId.StartsWith("tx-", StringComparison.Ordinal)
    ? new RefundSucceeded($"rf-{transactionId[3..]}")
    : new RefundFailed("unknown_transaction"));
```

What I checked with the local compiler:
- A union **switch expression** that covers all three cases compiles with no warning. Leaving one out gives CS8509, which item 2 turns into an error.
- Type patterns (`o is RefundFailed`) and positional patterns (`RefundFailed(var code)`) test the value stored in the union directly.

**Things to know before you ship it**

1. **It is a breaking change for the adapter packages.**
   - `union` produces a **struct**: `[Union]`, implements `IUnion`, exposes `object Value`, and has one constructor per case. `RefundOutcome` is a `sealed class` today, so `Task<RefundOutcome>` in `PaymentProvider.RefundAsync` changes at the binary level. Every adapter (Billing.Stripe, Billing.Adyen, Billing.PayPal, ...) must be recompiled against the new version.
   - Adapters that use `new RefundOutcome { ... }` or read `Succeeded`, `RefundId`, `ErrorCode` or `PendingUntil` must also change their code.
   - Ship it in a **major version** and coordinate with the adapter teams.
2. **`default(RefundOutcome)` is a fourth, empty state.** Its `Value` is `null` and `o is null` returns `true`. The compiler still treats a three-arm switch as complete, so `default` reaches the hidden fallback and throws `SwitchExpressionException` at runtime. That is still better than today's silent `"Refund failed: "`, but don't leave a `RefundOutcome` field uninitialized.
3. **`ToString()` on the union prints only the type name** (`Billing.RefundOutcome`), not the case. Log `outcome.Value` instead.
4. **Gentler path if you need a transition release.** C# 15 also recognizes custom unions: a type marked `[Union]` that has one constructor per case and a `Value` property. You can keep `RefundOutcome` a **sealed class** (so it stays a reference type), get the same complete switches, and keep the old members as `[Obsolete]` shims for one release:

   ```csharp
   using System.Runtime.CompilerServices;

   [Union]
   public sealed class RefundOutcome : IUnion
   {
       public RefundOutcome(RefundSucceeded value) => Value = value;
       public RefundOutcome(RefundFailed value)    => Value = value;
       public RefundOutcome(RefundPending value)   => Value = value;
       public object Value { get; }

       public static RefundOutcome Success(string refundId)      => new RefundSucceeded(refundId);
       public static RefundOutcome Failure(string errorCode)     => new RefundFailed(errorCode);
       public static RefundOutcome Pending(DateTimeOffset until) => new RefundPending(until);

       [Obsolete("Pattern match on RefundOutcome instead.")]
       public bool Succeeded => Value is RefundSucceeded;
       // ...same for RefundId / ErrorCode / PendingUntil

       public override string ToString() => Value.ToString()!;
   }
   ```

   I checked this as well: complete switches, implicit conversion from the case records, and the `[Obsolete]` warning (CS0618) all work. It is still not fully binary compatible: the parameterless constructor and the `init` setters go away. It does avoid the class-to-struct change and gives adapter teams one release of warnings before the old members are removed.

**Why a `union` here but `closed` for `PaymentResult`?** `PaymentResult` is already a proper type hierarchy, and closing it costs adapters nothing. `RefundOutcome` isn't a hierarchy at all: it is a bag of flags, and a union is the direct replacement. Using records with a `closed` base type would also work for refunds if you'd rather have one approach across the library. The breaking-change cost would be about the same.

## 4. What NOT to change

- **Don't mark `PaymentProvider` as `closed`, and don't seal it.** Its doc comment says the adapters in separate NuGet packages derive from it. With `closed`, every one of them would fail with CS9382. It must stay an open `abstract class`.
- **Don't turn `PaymentResult` into a `union`.** `closed` already gives complete switches. A union would turn it into a struct and change the return type of `PaymentProvider.ChargeAsync` at the binary level. It would also drop the base type and break every adapter, and you would gain nothing in return.
- **Don't change `PaymentProvider`'s abstract signatures** (for example to `ValueTask`) as part of this upgrade. That breaks every adapter too, and nothing in .NET 11 requires it.

## 5. Smaller items (Low)

These came up when I built a copy with `-p:AnalysisLevel=latest-all`. They aren't specific to .NET 11 but are cheap to fix:

- **CA1310 in `FakeProvider.RefundAsync`:** `transactionId.StartsWith("tx-")` compares using the current culture. Use `StartsWith("tx-", StringComparison.Ordinal)`.
- **CA2007 in `PaymentService.RefundAsync`:** this is a library, so consider `await provider.RefundAsync(transactionId, ct).ConfigureAwait(false)`. Or turn the rule off if you only ever run in ASP.NET Core.
- **Currency formatting:** `{a.Amount:C}` in `Describe` takes the currency symbol from the server's current culture, not from the payment. Consider a currency code on `Approved`, or format with an explicit culture. (This is a design issue, not a .NET 11 one.)
- **Optional, `FakeProvider`:** `Guid.CreateVersion7()` gives time-ordered fake transaction IDs, which makes test logs easier to read.
- **Optional, C# 14 extension members:** if callers would rather write `result.HttpStatus` / `result.Description` than call static helpers, an `extension(PaymentResult result) { ... }` block compiles cleanly against the closed hierarchy. This is purely a style choice.

## Suggested rollout

1. **Minor release:** items 1, 2 and 5. No API or binary break. Adapters keep working, and the library gains compile-time complete switches.
2. **Next major release:** item 3, coordinated with the adapter teams. If they need a transition period, ship the `[Union]` class variant with `[Obsolete]` shims first. Then switch to the `union` declaration, or keep the class.
