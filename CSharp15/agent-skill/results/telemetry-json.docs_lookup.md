# Review: Telemetry.Contracts serialization (net11.0, Native AOT)

Scope: `DeviceEvent.cs`, `SensorValue.cs`, `IngestResponse.cs`, `TelemetryJsonContext.cs`, `TelemetryClient.cs`.
No project files were changed. Every claim below was checked in a scratch copy, built with SDK 11.0.100 and run both under JIT and as a Native AOT binary (osx-arm64).

## TL;DR

The library has three hand-written pieces of plumbing. C# 15 and .NET 11 System.Text.Json (STJ) now have built-in features for each of them:

| Today | Problem | .NET 11 / C# 15 replacement |
|---|---|---|
| `SensorValue` struct + `object?` field + implicit operators + `SensorValueConverter` | ~40 lines of boilerplate. The compiler can't check exhaustiveness. | `public union SensorValue(double, string);`. STJ serializes a union as its case value and picks the case from the JSON token kind. |
| `IngestResponse` abstract record + `IngestResponseConverter` (buffers into `JsonDocument`, checks hard-coded property names) | Duplicates the record shape as strings. Ignores the caller's `options`. Needs extra `[JsonSerializable]` entries. | `[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))] public union IngestResponse(IngestAccepted, IngestRejected);` |
| `[JsonPolymorphic]` + a hand-maintained `[JsonDerivedType]` list on `DeviceEvent` | **It is already out of date (bug, see below).** | `[JsonPolymorphic(InferClosedTypePolymorphism = true)] public closed record DeviceEvent(...)` |

Both custom converters go away, the `[JsonDerivedType]` list goes away, and the context drops from 4 `[JsonSerializable]` roots to 2. A switch over these types becomes exhaustive without a `_` arm. All of this goes through the source generator: the prototype publishes with `PublishAot=true` with **no IL2xxx/IL3xxx warnings**. With the same harness, the JSON produced is **byte-for-byte identical** to today's for every payload that the current code can handle.

## 1. Bug: `DeviceRebooted` can't be serialized (fix this now, whatever else you decide)

`DeviceEvent.cs`: `DeviceRebooted` ("added in v2.3") was never added to the `[JsonDerivedType]` list. It compiles, and `TelemetryClient.Describe` even handles it. But sending one fails at runtime:

```
NotSupportedException: Runtime type 'Telemetry.Contracts.DeviceRebooted' is not supported
by polymorphic type 'Telemetry.Contracts.DeviceEvent'. Path: $.
```

(Reproduced under JIT and under Native AOT.) Any batch that contains a reboot event makes `SendAsync` throw. The minimal fix is one line:
`[JsonDerivedType(typeof(DeviceRebooted), nameof(DeviceRebooted))]`. The real fix is §2, which makes this class of bug impossible.

## 2. `DeviceEvent`: a closed hierarchy plus inferred polymorphism

```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record DeviceEvent(string DeviceId, DateTimeOffset At);   // closed => implicitly abstract
public sealed record MeasurementRecorded(...) : DeviceEvent(DeviceId, At);
public sealed record ThresholdExceeded(...)   : DeviceEvent(DeviceId, At);
public sealed record DeviceRebooted(...)      : DeviceEvent(DeviceId, At);
```

- STJ registers every direct subtype and uses its simple type name as the `$type` discriminator. That is exactly what `nameof(...)` gives you today, so the wire format doesn't change (verified: `{"$type":"MeasurementRecorded",...}`, and now also `{"$type":"DeviceRebooted",...}`).
- `closed` makes `switch` over `DeviceEvent` exhaustive. `TelemetryClient.Describe` can drop its `_ => throw new ArgumentOutOfRangeException(...)` arm. If you add a subtype and forget an arm, you get **CS8509** at compile time (verified: removing the `DeviceRebooted` arm produces `the pattern 'Telemetry.Contracts.DeviceRebooted' is not covered`). Consider `<WarningsAsErrors>CS8509</WarningsAsErrors>`.
- Alternative: set `InferClosedTypePolymorphism = true` once on `[JsonSourceGenerationOptions]` for every closed hierarchy in the context. Per the docs, setting it only at runtime on `JsonSerializerOptions` is **not enough** with source generation, because the generator must emit the derived-type metadata.
- Contract impact: other assemblies can no longer derive from `DeviceEvent` (CS9382). For a wire contract that is what you want. Note it in the release notes.

## 3. `SensorValue`: a union instead of a hand-written wrapper and converter

```csharp
public union SensorValue(double, string)
{
    // Optional: keep the old surface for source compatibility.
    public bool IsNumber => Value is double;
    public bool IsStatus => Value is string;
    public double AsNumber => (double)Value!;
    public string AsStatus => (string)Value!;
}
```

- Delete `SensorValueConverter` and the `[JsonConverter]` attribute. STJ writes `21.5` / `"offline"` and reads them back by token kind. An integral number such as `21` binds to the `double` case.
- Implicit conversions from `double`/`string` are generated, so `new MeasurementRecorded(..., 21.5)` and `(..., "offline")` still compile.
- Callers can pattern-match directly: `MeasurementRecorded { Value: double v }`, `{ Value: string s }`. `IsNumber`/`AsNumber` become optional convenience members.
- **Behavior differences to decide on:**
  - `default(SensorValue)` used to throw `JsonException("Empty sensor value.")` on write. It now writes `null`.
  - JSON `"value": null` used to be rejected (`Unexpected token Null`). It now deserializes to an empty union (`Value == null`).

  If "no value" must stay illegal, validate in the `MeasurementRecorded` constructor, or handle the empty case explicitly in your switches (the prototype adds a `MeasurementRecorded m => "... has no value"` arm). Other tokens, such as `true`, are still rejected with a `JsonException`.
- Like the current struct, a union boxes the `double` (it has one `object?` field), so allocation doesn't change. If that matters, a custom `[Union]` struct can avoid the boxing. This is not worth it here.

## 4. `IngestResponse`: a union with the structural classifier instead of `IngestResponseConverter`

```csharp
public sealed record IngestAccepted(int Accepted, string BatchId);
public sealed record IngestRejected(string Error, int? RetryAfterSeconds = null);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union IngestResponse(IngestAccepted, IngestRejected);
```

Problems with the current converter:
- It buffers the whole payload into a `JsonDocument`, then deserializes a second time.
- The property names `"error"`, `"accepted"`, `"batchId"` are string copies of the record shape (after camel-casing). Rename a property and the converter breaks silently.
- It ignores the `options` passed in and always uses `TelemetryJsonContext.Default.*`, so a consumer's resolver or options are bypassed.
- It forces `IngestAccepted`/`IngestRejected` into the context as separate roots.

The structural classifier picks the case whose properties match, using the names *after* the naming policy. Verified behavior:

| Payload | Current converter | Union + structural classifier |
|---|---|---|
| `{"accepted":12,"batchId":"b-42"}` | Accepted | Accepted (same JSON on write) |
| `{"error":"quota exceeded","retryAfterSeconds":30}` | Rejected | Rejected (same JSON on write) |
| `{"error":"bad"}` (no retry) | Rejected | Rejected |
| extra unknown properties | tolerated | tolerated |
| `{"foo":1}` / PascalCase names | `JsonException` | `JsonException` |
| `{"accepted":12}` (missing `batchId`) | `JsonException` | **Accepted with `BatchId == null`** unless strict options are on (see below) |
| `{"error":"x","accepted":1}` (both shapes) | Rejected (`error` wins) | `JsonException` (ambiguous) |
| `null` | `null` reference, hidden by the `!` | `default(IngestResponse)`, matched by a `null` arm |

**Recommended context options** to keep today's strictness for partial payloads:

```csharp
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(DeviceEvent[]))]
[JsonSerializable(typeof(IngestResponse))]
public partial class TelemetryJsonContext : JsonSerializerContext;
```

Pitfall (verified): `RespectRequiredConstructorParameters` treats *every* constructor parameter as required, **including `int? RetryAfterSeconds`**. Without a default value, `{"error":"bad"}` then matches no case and throws. Give optional parameters a default (`int? RetryAfterSeconds = null`), as in the snippet above.

API impact: `IngestResponse` changes from an abstract record base class into a union struct. `IngestAccepted`/`IngestRejected` no longer inherit from it. Assigning (`IngestResponse r = new IngestAccepted(...)`) and pattern matching (`r is IngestAccepted a`, `switch`) still compile. Code that relies on inheritance, reference semantics, or `IngestResponse?` as a nullable reference does not, and the change is binary-breaking. Ship it in a major version.

## 5. `TelemetryClient`

```csharp
public async Task<IngestResponse> SendAsync(DeviceEvent[] events, CancellationToken ct = default)
{
    using var response = await http.PostAsJsonAsync("/ingest", events, TelemetryJsonContext.Default.DeviceEventArray, ct);
    return await response.Content.ReadFromJsonAsync(TelemetryJsonContext.Default.IngestResponse, ct);
}

public static string Summarize(IngestResponse response) => response switch
{
    IngestAccepted a => ...,
    IngestRejected { RetryAfterSeconds: { } s } r => ...,
    IngestRejected r => ...,
    null => "Empty response",          // replaces `_ => "Unknown response"`
};
```

- `ReadFromJsonAsync(JsonTypeInfo<T>)` replaces the manual stream and `DeserializeAsync` pair, and it is AOT-safe.
- The `!` after `DeserializeAsync` hides a possible `null` (body `null`). The union makes the empty case explicit, and the compiler requires the `null` arm.
- `Summarize` and `Describe` both lose their catch-all arms and become compiler-checked.
- Not related to serialization, but worth a look: the status code is never checked. Don't just add `EnsureSuccessStatusCode()`, because a 429 probably carries the `IngestRejected` body. An HTML 502 from a proxy currently surfaces as a confusing `JsonException`. Consider checking `Content-Type` or the status code before deserializing.

## 6. Project file

Add `<IsAotCompatible>true</IsAotCompatible>` to `Telemetry.Contracts.csproj`. Today the trim and AOT analyzers only run in the client app when it publishes. With this flag, the library itself reports AOT problems at build time. Both the current code and the proposed code build with 0 warnings with this flag on.

## Net effect

- Deleted: `SensorValueConverter`, `IngestResponseConverter`, the hand-written `SensorValue` struct body, the `[JsonDerivedType]` list, 2 `[JsonSerializable]` entries, and 2 catch-all `_` switch arms. In the prototype, the five files (~140 lines) shrink to about 55 lines, client included.
- Fixed: `DeviceRebooted` serialization. A new subtype or union case can no longer be forgotten silently.
- Unchanged: the wire format for every valid payload, source-generated and AOT-clean (verified with a Native AOT publish and run).
- Decide on: the empty/`null` `SensorValue` semantics, the ambiguous both-shapes response, the major version bump for the `IngestResponse` and `closed` API changes. Add golden-JSON contract tests (one per event type and per response shape) before you migrate, so the "same wire format" claim stays enforced.
