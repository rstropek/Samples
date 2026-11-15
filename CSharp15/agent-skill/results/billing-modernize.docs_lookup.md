# Billing library: .NET 11 / C# 15 modernization review

Scope: `Billing.csproj`, `PaymentResult.cs`, `RefundOutcome.cs`, `PaymentService.cs`, `PaymentProvider.cs`.
The project targets `net11.0` with SDK 11.0.100, so the default language version is **C# 15**. You don't need a `LangVersion`.

The library already builds with no warnings. For the most part this review is not about bugs. Most of what C# 15 offers here comes from one new feature: **closed class hierarchies** (`closed`). They let the compiler see what your code only states in comments today: "these are the only cases". **Union types** (`union`) are the other headline feature. I compare them below. For this library I recommend `closed`, not `union`, and the reasons are explained.

I checked every suggestion in a scratch copy with the .NET 11 GA SDK (11.0.100), including binary-compatibility tests against a fake "adapter" assembly built for the current version (details at the end).

---

## Summary (priority order)

| # | Priority | Change | Breaking? |
|---|----------|--------|-----------|
| 1 | **High: do now** | Make `PaymentResult` a `closed record`. Drop the `private protected` constructor trick and the `_ => throw UnreachableException` arm | No (verified binary-compatible for adapters) |
| 2 | **High: plan it** | Replace `RefundOutcome` (a bool plus three nullable fields) with a `closed record` hierarchy (`RefundSucceeded` / `RefundFailed` / `RefundPending`) | Breaks only adapters that use `new RefundOutcome { ... }`. Adapters that use the factories keep working, both source and binary |
| 3 | Medium | Rewrite `ToHttpStatus` and `RefundAsync` as exhaustive switch expressions (follows from 1 and 2) | No |
| 4 | Medium | Make the exhaustiveness warnings errors (`CS8509`, `CS8655`) | No (build setting) |
| 5 | Low / optional | `System.Text.Json`: `[JsonPolymorphic(InferClosedTypePolymorphism = true)]` if results go over the wire | No |
| 6 | **Do not do** | Don't mark `PaymentProvider` `closed`. Don't turn `PaymentResult` or `RefundOutcome` into `union` types. Don't turn on runtime async yet | - |
| 7 | Low (not tied to .NET 11) | `StartsWith` without a comparison, culture-dependent `:C` formatting, the status-code choice for retryable declines | Behavior change only |

---

## 1. `PaymentResult`: replace the `private protected` constructor with `closed` (High, do now)

**Today**

```csharp
public abstract record PaymentResult
{
    private protected PaymentResult() { }   // "callers switch over these three cases"
}
```

The constructor does stop other assemblies from deriving, but the compiler doesn't know the set is closed. As a result every switch needs a fallback arm:

```csharp
_ => throw new UnreachableException($"Unknown payment result {result.GetType().Name}"),
```

That arm is dead code. Worse, it hides mistakes: if you add a fourth result type (say `Pending3DS`), `Describe` still compiles and fails at run time. `ToHttpStatus` has the same problem with its trailing `throw new InvalidOperationException(...)`.

**Suggested (C# 15)**

```csharp
namespace Billing;

/// <summary>Outcome of a charge attempt.</summary>
public closed record PaymentResult;          // implicitly abstract; only this assembly may derive

public sealed record Approved(string TransactionId, decimal Amount) : PaymentResult;
public sealed record Declined(string Reason, bool Retryable) : PaymentResult;
public sealed record RequiresAction(Uri RedirectUrl) : PaymentResult;
```

```csharp
public static string Describe(PaymentResult result) => result switch
{
    Approved a       => $"Approved {a.Amount:C} ({a.TransactionId})",
    Declined d       => $"Declined: {d.Reason}{(d.Retryable ? " (retry later)" : "")}",
    RequiresAction r => $"Customer action required at {r.RedirectUrl}",
};   // no `_` arm needed; using System.Diagnostics can go
```

What you gain (all verified):

- The intent is stated in the language instead of an XML `<remarks>` comment.
- Exhaustiveness is checked at compile time. Remove a case or add a fourth record, and every switch that misses it raises **CS8509**, for example: `the pattern 'Billing.RequiresAction' is not covered`.
- This also works **for your consumers**. The compiler writes the closed set into metadata (`[IsClosedType(DerivedTypes = [typeof(Approved), typeof(Declined), typeof(RequiresAction)])]`), so code in other teams' assemblies gets the same exhaustiveness checks when it switches over `PaymentResult`.
- Deriving from another assembly is a hard error: `error CS9382: 'Chargeback': cannot use a closed type 'PaymentResult' from another assembly as a base type.` The compiler also marks the base constructors `[CompilerFeatureRequired("ClosedClasses")]`, so older compilers can't get around it.
- **No break for adapters.** Adapters only create `Approved` / `Declined` / `RequiresAction`, and those don't change. I ran an adapter compiled against the current `Billing.dll` against the `closed` build with no recompile, and it worked.

The old `private protected PaymentResult() { }` still compiles next to `closed`, but it's redundant now. Remove it.

Note: if a caller passes `null` despite the non-nullable signature, the switch now throws `SwitchExpressionException` instead of `UnreachableException`. That's a caller bug either way, and nullable analysis already flags it at the call site.

---

## 2. `RefundOutcome`: make illegal states unrepresentable (High value, plan the rollout)

**Today**

```csharp
/// Exactly one of the following is true:
///  * Succeeded == true  and RefundId is set
///  * Succeeded == false and ErrorCode is set
///  * Succeeded == false and PendingUntil is set
public sealed class RefundOutcome
{
    public bool Succeeded { get; init; }
    public string? RefundId { get; init; }
    public string? ErrorCode { get; init; }
    public DateTimeOffset? PendingUntil { get; init; }
    ...
}
```

This is the biggest design problem in the library. Only a comment enforces the invariant, and the type is public with `init` setters, so adapters from other teams can build any combination:

- `new RefundOutcome()`: nothing is set. `PaymentService.RefundAsync` returns `"Refund failed: "` with an empty error code.
- `new RefundOutcome { Succeeded = true }`: returns `"Refunded ()"`.
- `new RefundOutcome { Succeeded = true, ErrorCode = "x", PendingUntil = ... }`: success wins silently.

It also forces `PaymentService.RefundAsync` into an order-dependent `if` chain. That chain reads nullable fields that are only "known" to be set because the comment says so.

**Suggested: a closed hierarchy, with a compatibility layer for one release**

```csharp
namespace Billing;

/// <summary>Result of a refund request.</summary>
public closed record RefundOutcome
{
    // Same names and signatures as today, so adapters that use the factories don't change (source or binary).
    public static RefundOutcome Success(string refundId)     => new RefundSucceeded(refundId);
    public static RefundOutcome Failure(string errorCode)    => new RefundFailed(errorCode);
    public static RefundOutcome Pending(DateTimeOffset until) => new RefundPending(until);

    // ---- Transitional shims for existing readers; remove in the next major version ----
    // Keep the shims out of the synthesized ToString() ("RefundSucceeded { Id = rf-1 }").
    protected virtual bool PrintMembers(System.Text.StringBuilder builder) => false;

    [Obsolete("Pattern match on RefundSucceeded instead.")]
    public bool Succeeded => this is RefundSucceeded;
    [Obsolete("Pattern match on RefundSucceeded instead.")]
    public string? RefundId => (this as RefundSucceeded)?.Id;
    [Obsolete("Pattern match on RefundFailed instead.")]
    public string? ErrorCode => (this as RefundFailed)?.Code;
    [Obsolete("Pattern match on RefundPending instead.")]
    public DateTimeOffset? PendingUntil => (this as RefundPending)?.Until;
}

public sealed record RefundSucceeded(string Id) : RefundOutcome;
public sealed record RefundFailed(string Code) : RefundOutcome;
public sealed record RefundPending(DateTimeOffset Until) : RefundOutcome;
```

The case properties are called `Id` / `Code` / `Until` so they don't clash with the obsolete shims on the base. Once the shims are gone you can rename them to `RefundId` / `ErrorCode` if you like.

**Compatibility, measured against an adapter compiled for the current version:**

| Adapter style | Old adapter binary on the new `Billing.dll` | Recompiled against new version |
|---|---|---|
| `RefundOutcome.Success(...)` / `Failure` / `Pending` | Works | Compiles |
| Reads `outcome.Succeeded` / `RefundId` / ... | Works | Compiles, with `CS0618` obsolete warnings |
| `new RefundOutcome { Succeeded = true, RefundId = ... }` | **Run-time failure**: `InvalidOperationException: Instances of abstract classes cannot be created.` | `CS0144` / `CS0200` |

**Rollout recommendation:** make it a major-version change of `Billing`. Before shipping, ask the adapter teams (Stripe, Adyen, PayPal, ...) to `grep` for `new RefundOutcome`. Anyone using the object-initializer style has to switch to the factories, which already exist today, so they can make that change *before* you ship. Remove the `[Obsolete]` shims one major version later.

---

## 3. Rewrite `ToHttpStatus` and `RefundAsync` as exhaustive switch expressions (Medium)

Once 1 and 2 are in, both methods can be written so the compiler checks them:

```csharp
public static int ToHttpStatus(PaymentResult result) => result switch
{
    Approved                     => 200,
    Declined { Retryable: true } => 503,
    Declined                     => 402,
    RequiresAction               => 202,
};   // the trailing `throw new InvalidOperationException(...)` goes away

public async Task<string> RefundAsync(string transactionId, CancellationToken ct = default) =>
    await provider.RefundAsync(transactionId, ct) switch
    {
        RefundSucceeded s => $"Refunded ({s.Id})",
        RefundPending p   => $"Refund queued until {p.Until:u}",
        RefundFailed f    => $"Refund failed: {f.Code}",
    };
```

Today `ToHttpStatus` uses an `if`/`is` chain. Unlike the `switch` in `Describe`, an `if` chain gets no exhaustiveness check at all, even after the hierarchy is closed. Moving to `switch` expressions everywhere is what turns "add a case" into a compile error in every place that has to handle it.

---

## 4. Make exhaustiveness a build gate (Medium)

A non-exhaustive switch is a **warning** (CS8509), even over a closed type, as in every earlier C# version. Since 1 to 3 rely on it, make it fail the build:

```xml
<PropertyGroup>
  <TargetFramework>net11.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <!-- C# 15 closed hierarchies: a missing case must break the build -->
  <WarningsAsErrors>$(WarningsAsErrors);CS8509;CS8655</WarningsAsErrors>
</PropertyGroup>
```

(`CS8655` is the "doesn't handle null" variant you'll see if anyone switches over `PaymentResult?`.) Consider suggesting the same setting to the adapter teams and other consumers.

Because this is a published library with downstream packages, it's also worth turning on package validation (`<EnablePackageValidation>true</EnablePackageValidation>` plus `<PackageValidationBaselineVersion>`). It would have reported the `RefundOutcome` break in item 2 for you automatically. This isn't new in .NET 11, but it fits this change well.

---

## 5. JSON: let STJ infer polymorphism from the closed set (Low / optional)

If `PaymentResult` (or `RefundOutcome`) ever gets serialized, for example by an API layer returning it, .NET 11's System.Text.Json can derive the `$type` discriminator from the closed set. You don't need a `[JsonDerivedType]` list to keep in sync:

```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record PaymentResult;
```

Verified round-trip:
`[{"$type":"Approved","TransactionId":"tx-1","Amount":12.5},{"$type":"Declined","Reason":"nsf","Retryable":false},{"$type":"RequiresAction","RedirectUrl":"https://x.test/3ds"}]`

Only add this if serialization is actually a use case. It does nothing otherwise.

---

## 6. Things **not** to do (and why)

### 6a. Don't mark `PaymentProvider` `closed`

It looks tempting ("abstract base with a few implementations"), but the XML doc says the adapters ship as separate NuGet packages that derive from it. `closed` would break all of them with `CS9382`. `PaymentProvider` is an open extension point and should stay an ordinary `abstract class`.

### 6b. Don't use `union` for `PaymentResult` / `RefundOutcome`

C# 15's `union` is the other headline feature, and it would be the obvious choice for "one of three things":

```csharp
public union RefundOutcome(RefundSucceeded, RefundFailed, RefundPending) { /* factories */ }
```

It compiles, and the factory-based adapter code still compiles too, thanks to the implicit case conversions. For this library, though, `closed` is the better fit:

- **Binary break.** A shorthand `union` is a `struct`, so `Task<RefundOutcome>` changes from a class to a struct in the signature of the abstract `RefundAsync`. When I loaded an adapter built for the current version against a union build, it failed immediately with `TypeLoadException: Method 'RefundAsync' in type 'AcmeProvider' ... does not have an implementation`. With `closed`, the same adapter keeps running.
- **A new "empty" state.** `default(RefundOutcome)` would be a valid value with no case. That's exactly the kind of illegal state you're trying to get rid of, and every switch would need a `null` arm.
- **You own every case type.** Unions are for combining types you don't control (`int | string`, records from another library). Here all cases are your own records, and a closed hierarchy gives the same exhaustiveness without a wrapper struct or boxing, while keeping reference semantics, inheritance and shared base members.

### 6c. Runtime async: not yet

.NET 11 includes "runtime async" (`<Features>runtime-async=on</Features>`), but in .NET 11 GA it's still labelled preview and opt-in for user code. This library's async surface is two thin pass-through methods, so there's nothing meaningful to gain yet. Revisit when it becomes the default.

---

## 7. Smaller findings not tied to .NET 11 (Low)

- **Culture-sensitive `StartsWith`** in `FakeProvider.RefundAsync`: `transactionId.StartsWith("tx-")` uses the current culture (CA1310). Use `transactionId.StartsWith("tx-", StringComparison.Ordinal)`.
- **Culture-dependent currency formatting** in `Describe`: `{a.Amount:C}` uses the server's culture. On a machine with invariant globalization I saw `¤50.00`, and a German server would print `50,00 €` whatever the actual charge currency was. `PaymentResult` doesn't carry a currency at all. Consider adding a currency code to `Approved` and formatting explicitly, for example `$"{a.Amount:0.00} {a.Currency}"` with `CultureInfo.InvariantCulture`.
- **HTTP status for retryable declines:** `503 Service Unavailable` says the *server* is unavailable, and some clients and proxies retry 503 automatically, which can cause repeated charge attempts. Make sure that's intended. `402` (or `409`/`429`) with a `Retry-After` header is often a better fit.
- `FakeProvider` ignores `ct`. That's fine for a fake, but `Task.FromCanceled` when `ct.IsCancellationRequested` would make cancellation tests meaningful.

---

## Appendix: how this was verified

- The original project builds with 0 warnings and 0 errors on SDK 11.0.100.
- A scratch copy with items 1 to 5 applied builds cleanly with `CS8509;CS8655` as errors. Removing an arm produces `error CS8509 ... 'Billing.RequiresAction' is not covered`.
- A separate "adapter" assembly (one adapter using the factories, one using `new RefundOutcome { ... }`) and a console app were built against the **original** library. I then swapped in the modernized `Billing.dll` without recompiling:
  - `closed` version: the factory adapter and `FakeProvider` produce the same output as before. The initializer-style adapter fails (`Instances of abstract classes cannot be created`).
  - `union` version of `RefundOutcome`: `TypeLoadException` when the adapter type loads.
- Deriving from the closed `PaymentResult` in the adapter assembly gives `CS9382`. Reading the old shim properties gives `CS0618`.
- System.Text.Json round-trip with `InferClosedTypePolymorphism = true` was verified as shown in item 5.
