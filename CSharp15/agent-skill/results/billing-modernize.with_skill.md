# Billing on .NET 11: modernization review

**Scope:** `Billing.csproj` (net11.0), `PaymentResult.cs`, `RefundOutcome.cs`, `PaymentService.cs`, `PaymentProvider.cs`.
**Short version:** C# 15 is the default language version on `net11.0`. Its **`closed` hierarchies** fit two of your types well: `PaymentResult` and `RefundOutcome`. With them the compiler checks that every switch handles every case, so you can delete the "unreachable" fallbacks. One type should **not** change: `PaymentProvider` must stay `abstract`.

All code below was compiled with the .NET 11 SDK (11.0.100) in a scratch copy of the project. Your project was not modified. The details are under "How this was verified" at the end.

---

## P1: Make `PaymentResult` a `closed` record (low risk, high value)

**Today** (`PaymentResult.cs`): the `private protected` constructor stops other assemblies from adding result types. But the compiler does not know the set is closed. So `PaymentService.Describe` needs `_ => throw new UnreachableException(...)`, and `ToHttpStatus` ends in `throw new InvalidOperationException(...)`. If you add a fourth result such as `Pending`, both methods still compile and only fail at runtime.

**Suggested:**

```csharp
namespace Billing;

/// <summary>Outcome of a charge attempt. Only this assembly can add cases.</summary>
public closed record PaymentResult;

public sealed record Approved(string TransactionId, decimal Amount) : PaymentResult;
public sealed record Declined(string Reason, bool Retryable) : PaymentResult;
public sealed record RequiresAction(Uri RedirectUrl) : PaymentResult;
```

- `closed` replaces the `private protected` constructor trick. A type in another assembly that tries to derive gets **CS9382** (verified).
- A closed type is implicitly abstract. Don't write `abstract closed`, which is CS9384.
- Adapter packages are unaffected. They construct `Approved`, `Declined` and `RequiresAction` and return `PaymentResult`, and they never derive from it. An adapter compiled against the current `Billing.dll` ran unchanged against the modernized DLL (verified).

## P2: Remove the "unreachable" fallbacks in `PaymentService` (do this together with P1)

Once P1 is in place, `Describe` is exhaustive without `_`. Keeping `_ => throw` would hide the compile-time warning you get when a case is added later.

```csharp
public static string Describe(PaymentResult result) => result switch
{
    Approved a       => $"Approved {a.Amount:C} ({a.TransactionId})",
    Declined d       => $"Declined: {d.Reason}{(d.Retryable ? " (retry later)" : "")}",
    RequiresAction r => $"Customer action required at {r.RedirectUrl}",
};
```

`ToHttpStatus` uses an `if`/`is` chain, which is never checked for exhaustiveness. Turn it into a switch expression so it gets the same check:

```csharp
public static int ToHttpStatus(PaymentResult result) => result switch
{
    Approved                     => 200,
    Declined { Retryable: true } => 503,
    Declined                     => 402,
    RequiresAction               => 202,
};
```

After this, `using System.Diagnostics;` is no longer needed.

Notes:
- The parameter is a non-nullable `PaymentResult`, so no `null` arm is needed. If you ever switch on a `PaymentResult?`, you will need a `null =>` arm (otherwise CS8655).
- `FakeProvider.ChargeAsync` has `_ =>` in `amount switch { <= 0 => ..., > 10_000 => ..., _ => ... }`. That one is fine. It matches on a `decimal` range, not on a result type.

## P3: Turn the missing-case warning into a build error (one line, do it with P1/P2)

A switch that misses a case gives **CS8509**, which is only a **warning** by default. Since the point of P1 and P2 is that a new case breaks the build, make it an error:

```xml
<PropertyGroup>
  <TargetFramework>net11.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <WarningsAsErrors>$(WarningsAsErrors);CS8509</WarningsAsErrors>
</PropertyGroup>
```

I checked this with a switch that left out `RequiresAction`. The build failed with: `error CS8509: ... the pattern 'Billing.RequiresAction' is not covered`.

## P4: Replace the "several nullable fields" shape of `RefundOutcome` with a closed hierarchy (high value, needs a coordinated release)

**Today** (`RefundOutcome.cs`): the rule "exactly one of Succeeded+RefundId / ErrorCode / PendingUntil" exists only as an XML comment. Nothing enforces it. Because the `init` setters are public, the following compile without complaint:
- `new RefundOutcome { Succeeded = true }`, which has no `RefundId`;
- `new RefundOutcome { ErrorCode = "x", PendingUntil = ... }`, which is both failed and pending.

`PaymentService.RefundAsync` also depends on the order of its checks. It tests `PendingUntil` before falling through to "failed", and the fallthrough prints `ErrorCode` even when that is `null`.

**Suggested:** each case becomes its own type and carries only the data it actually has.

```csharp
namespace Billing;

/// <summary>Result of a refund request.</summary>
public closed record RefundOutcome
{
    // Kept so existing adapter code (RefundOutcome.Success(...) etc.) keeps compiling and linking.
    public static RefundOutcome Success(string refundId)     => new RefundSucceeded(refundId);
    public static RefundOutcome Failure(string errorCode)    => new RefundFailed(errorCode);
    public static RefundOutcome Pending(DateTimeOffset until) => new RefundPending(until);
}

public sealed record RefundSucceeded(string RefundId) : RefundOutcome;
public sealed record RefundFailed(string ErrorCode) : RefundOutcome;
public sealed record RefundPending(DateTimeOffset Until) : RefundOutcome;
```

```csharp
public async Task<string> RefundAsync(string transactionId, CancellationToken ct = default) =>
    await provider.RefundAsync(transactionId, ct) switch
    {
        RefundSucceeded s => $"Refunded ({s.RefundId})",
        RefundPending p   => $"Refund queued until {p.Until:u}",
        RefundFailed f    => $"Refund failed: {f.ErrorCode}",
    };
```

**Compatibility.** This is why it is P4 and not P1: the adapter packages (Billing.Stripe, Billing.Adyen, Billing.PayPal) are maintained by other teams.
- Adapters that only call `RefundOutcome.Success/Failure/Pending` keep working. Keeping those factories with the same signatures means they need no source change. A test adapter compiled against the old DLL also ran against the new DLL without recompiling (verified).
- Adapters that use `new RefundOutcome { ... }` or read `Succeeded`, `RefundId`, `ErrorCode` or `PendingUntil` will break. Before you ship, search the adapter repos for these. For a softer migration you can keep read-only `[Obsolete]` shims on the base for one release, for example `[Obsolete] public bool Succeeded => this is RefundSucceeded;`. (I did not compile this shim variant.)
- Treat this as a major-version change of the package and coordinate it with the adapter teams.

*Why `closed` and not a C# 15 `union`?* You own all three case types, and a closed hierarchy needs no wrapper struct and has no boxing. A `union RefundOutcome(RefundSucceeded, RefundFailed, RefundPending)` would also be exhaustive. But it is a struct, so `default(RefundOutcome)` is an empty value, and it changes the type's identity more than the record approach does.

## P5: Do NOT make `PaymentProvider` closed

`PaymentProvider` looks like the other abstract bases, but it is an **extension point**. Its own doc comment says the adapters in separate NuGet packages derive from it. Marking it `closed` would make every one of those adapters fail with CS9382. Keep it `public abstract class`. A switch over providers doesn't make sense anyway, because their behaviour is dispatched through the virtual members.

## P6 (optional, only if these types are serialized): System.Text.Json polymorphism

The library has no JSON code today. If an API layer serializes `PaymentResult` or `RefundOutcome`, .NET 11 can infer the derived types of a closed base, so you don't need to maintain a `[JsonDerivedType]` list. A forgotten entry in that list only fails at runtime with `NotSupportedException`.

```csharp
using System.Text.Json.Serialization;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record PaymentResult;
```

Verified output: `JsonSerializer.Serialize<PaymentResult>(new Declined("x", true))` produced `{"$type":"Declined","Reason":"x","Retryable":true}`, and deserializing `{"$type":"Approved",...}` round-tripped to `Approved`. This also works with source-generated `JsonSerializerContext` (trimming/AOT). If clients already consume a different discriminator format, check it before you switch.

## Minor, not specific to .NET 11

- `FakeProvider.RefundAsync`: `transactionId.StartsWith("tx-")` uses a culture-sensitive comparison. Prefer `StartsWith("tx-", StringComparison.Ordinal)`.
- `Describe` formats `{a.Amount:C}` with the current culture. If this text reaches logs or other machines, consider passing an explicit culture or currency.

## Tooling caveat

Some C# language servers and IDE plugins still lag behind the C# 15 compiler. They may flag `closed` as "preview" (CS8652) or report false CS8509 on these switches. `dotnet build` with the .NET 11 SDK is the source of truth. It builds all of the above cleanly with no `LangVersion` or preview settings.

---

## Suggested order

| # | Change | Risk | Breaking for adapters? |
|---|---|---|---|
| P1 | `PaymentResult` becomes `closed record` | Low | No (verified binary-compatible) |
| P2 | Remove `_ => throw` / `throw` fallbacks, make `ToHttpStatus` a switch expression | Low | No |
| P3 | `CS8509` as error | Low | No |
| P4 | `RefundOutcome` becomes a closed hierarchy, keeping the factories | Medium | Only for adapters that use the initializer or properties |
| P5 | Keep `PaymentProvider` abstract | n/a | (closing it would break all of them) |
| P6 | `InferClosedTypePolymorphism` if serialized | Low | Wire format: check clients |

## How this was verified

The project was copied into a scratch directory and built there with SDK 11.0.100, so the original files were not touched.
- The original project builds with 0 warnings.
- The modernized version from P1 to P4 plus P6 builds cleanly with `CS8509` as an error. A deliberately incomplete switch fails with CS8509.
- A separate assembly that derives from the closed `PaymentResult` gets CS9382. A separate assembly that derives from `PaymentProvider` still compiles.
- Binary compatibility: an "adapter" was compiled against the original `Billing.dll`, then run against the modernized DLL without recompiling. It worked, including `RefundOutcome.Pending(...)`.
- The JSON round trip with `InferClosedTypePolymorphism` matched the outputs shown in P6.
