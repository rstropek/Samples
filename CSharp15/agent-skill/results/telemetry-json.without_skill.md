# Review: Telemetry.Contracts serialization (net11.0, Native AOT)

Scope: `DeviceEvent.cs`, `SensorValue.cs`, `IngestResponse.cs`, `TelemetryJsonContext.cs`, `TelemetryClient.cs`, `Telemetry.Contracts.csproj`. This is a review only. I made no edits to the project. Every claim below was checked against the installed .NET 11.0.100 SDK in scratch copies: I ran the original code, built a rewritten version, and published that version with `PublishAot=true` on osx-arm64.

## TL;DR

On .NET 11 / C# 15 you can drop **both** hand-written converters and **all** attribute lists. Three language and STJ features replace them:

| Today | Replace with | What goes away |
|---|---|---|
| `[JsonPolymorphic]` + one `[JsonDerivedType]` per subtype on `DeviceEvent` | `closed record DeviceEvent` + `InferClosedTypePolymorphism = true` | The derived-type list, which is already out of date (see bug 1) |
| `SensorValue` struct with an `object` field, casts, and `SensorValueConverter` | `public union SensorValue(double, string);` | The whole converter and the `IsX`/`AsX` API (about 35 lines) |
| `IngestResponse` hierarchy + `IngestResponseConverter` (`JsonDocument` buffering) | `[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))] public union IngestResponse(IngestAccepted, IngestRejected);` | The whole converter, plus the `[JsonSerializable]` entries for the case types |

The result is AOT-clean. Publishing it with Native AOT gave no trim or AOT warnings, and the native binary round-trips every case. The wire format of `DeviceEvent[]` matches today's output byte for byte, and `SensorValue` stays a plain `21.5` or `"offline"`.

## Findings in the current code

### 1. Bug: `DeviceRebooted` cannot be serialized
`DeviceRebooted` was added in v2.3 but never registered with `[JsonDerivedType]`. Sending one throws at runtime (verified):

```
NotSupportedException: Runtime type 'Telemetry.Contracts.DeviceRebooted' is not supported by polymorphic type 'Telemetry.Contracts.DeviceEvent'.
```

The compiler gives no hint about this. `TelemetryClient.Describe` handles `DeviceRebooted`, so the code looks complete. This is the main argument against hand-maintained derived-type lists, and recommendation A removes that class of bug.

### 2. `SensorValueConverter` / `SensorValue` is a hand-rolled union
- The struct stores `object? _value` and exposes `IsNumber`, `IsStatus`, `AsNumber` and `AsStatus` (unchecked casts that throw `InvalidCastException` on misuse). This is exactly what a C# 15 `union` generates, with compiler-checked exhaustiveness on top.
- The converter exists only to map a JSON number to `double` and a JSON string to `string`. STJ in .NET 11 does this classification by token type for unions automatically.

### 3. `IngestResponseConverter` is the most expensive piece
- It buffers every response into a `JsonDocument` (an extra allocation and parse), then deserializes a second time.
- It hard-codes `TelemetryJsonContext.Default`, so it ignores the `JsonSerializerOptions` the caller passed in. If anyone composes resolvers or changes options (case-insensitivity, naming), the nested calls quietly use different settings.
- The shape detection is ad hoc. `"error"` wins over `"accepted"`, and the property names are hard-coded strings that bypass the naming policy. `{"Accepted":12,"BatchId":"b"}` is rejected even if you configure case-insensitive matching.
- It forces `IngestAccepted` and `IngestRejected` to be listed separately in the context.

### 4. `TelemetryJsonContext` attribute list exists to serve the converters
`[JsonSerializable(typeof(IngestAccepted))]` and `[JsonSerializable(typeof(IngestRejected))]` are only there because the converter calls them directly. With a union, the generator picks up case types on its own. I verified that only `DeviceEvent[]` and `IngestResponse` need to be listed.

### 5. Minor: `TelemetryClient`
- `ReadAsStreamAsync` followed by `JsonSerializer.DeserializeAsync(...)!` can be written as `response.Content.ReadFromJsonAsync(ctx.IngestResponse, ct)`.
- `Summarize` and `Describe` need `_ =>` / `throw` arms because the hierarchies are open. With `closed` and `union`, both switches become exhaustive without a discard. Adding a new event type then produces warning CS8509 at every switch that misses it. I verified this: removing an arm gives `CS8509 ... the pattern 'V2.IngestRejected' is not covered`, and `MeasurementRecorded{ Value: string }` is reported the same way.
- The csproj does not set `<IsAotCompatible>true</IsAotCompatible>`. Turn it on so the library itself runs the trim and AOT analyzers instead of relying on the client publish to catch problems.

## Recommended shape (verified to compile, run and Native-AOT publish on net11.0)

```csharp
using System.Text.Json.Serialization;

namespace Telemetry.Contracts;

// 1) Sensor value: number or status text. Wire: 21.5 or "offline".
public union SensorValue(double, string)
{
    // Optional: keeps today's ToString (the default prints the type name, which
    // shows up in the record ToString of MeasurementRecorded / logs).
    public override string ToString() => Value?.ToString() ?? "<none>";
}

// 2) Device events: closed hierarchy, derived types inferred, "$type" = type name.
[JsonPolymorphic(InferClosedTypePolymorphism = true)]   // or set it once on the context, see below
public closed record DeviceEvent(string DeviceId, DateTimeOffset At);
public sealed record MeasurementRecorded(string DeviceId, DateTimeOffset At, string Sensor, SensorValue Value) : DeviceEvent(DeviceId, At);
public sealed record ThresholdExceeded(string DeviceId, DateTimeOffset At, string Sensor, double Limit, double Actual) : DeviceEvent(DeviceId, At);
public sealed record DeviceRebooted(string DeviceId, DateTimeOffset At, string Reason) : DeviceEvent(DeviceId, At);

// 3) Ingest response: two object shapes, no discriminator -> structural classification.
public sealed record IngestAccepted(int Accepted, string BatchId);
public sealed record IngestRejected(string Error, int? RetryAfterSeconds = null);   // "= null" matters, see below

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union IngestResponse(IngestAccepted, IngestRejected);

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    InferClosedTypePolymorphism = true,
    RespectRequiredConstructorParameters = true,
    RespectNullableAnnotations = true)]
[JsonSerializable(typeof(DeviceEvent[]))]
[JsonSerializable(typeof(IngestResponse))]
public partial class TelemetryJsonContext : JsonSerializerContext;
```

The client code needs little change. `Describe` and `Summarize` drop their `_` arms, and patterns like `MeasurementRecorded { Value: double d }` and `MeasurementRecorded { Value: string s }` replace `IsNumber` and `AsNumber`.

## Verified behaviour and gotchas (read before migrating)

1. **Do not drop the inference switch.** `closed` alone is not enough. If `DeviceEvent` is `closed` but neither `[JsonPolymorphic(InferClosedTypePolymorphism = true)]` nor the context-level `InferClosedTypePolymorphism = true` is set, a `DeviceRebooted` silently serializes as `{"deviceId":"d1","at":"..."}`, with no `$type` and no `reason`. That is silent data loss, which is worse than today's exception. The same thing happens if the base is merely `abstract` (not `closed`) while the option is on: inference does nothing and nothing warns. I recommend the per-type `[JsonPolymorphic(InferClosedTypePolymorphism = true)]` so the intent sits next to the type, and a round-trip test per event type.
2. **Inference and an explicit list do not mix.** Adding a single `[JsonDerivedType(typeof(DeviceRebooted), "reboot")]` turns inference off for the other subtypes, and serializing `ThresholdExceeded` then throws `NotSupportedException`. With inference the discriminator is the type name. That matches today's `nameof(...)` values, so the wire is unchanged, but renaming a record now changes the contract. If you need stable IDs that differ from the type names, keep the explicit list. You still get exhaustive switches from `closed`.
3. **The structural classifier is lenient unless you opt into strictness.** With default options, `{"accepted":12}` deserializes to `IngestAccepted { BatchId = null }` (a null in a non-nullable `string`), and `{"retryAfterSeconds":5}` becomes `IngestRejected { Error = null }`. The old converter rejected those. With `RespectRequiredConstructorParameters = true` (plus `RespectNullableAnnotations = true`), those shapes, `{}`, and `[]` are rejected with a `JsonException`.
   - That same switch makes **every** constructor parameter required, so `IngestRejected(string Error, int? RetryAfterSeconds)` then rejects `{"error":"x"}`. Give optional members a default (`int? RetryAfterSeconds = null`). I verified that this restores acceptance.
   - It also applies to `DeviceEvent` records when they are deserialized. The client only sends them, but note it if the server shares these contracts.
4. **Ambiguous payloads now fail instead of guessing.** `{"accepted":1,"batchId":"b","error":"e"}` used to become `IngestRejected` because `"error"` was checked first. The structural classifier now throws. That is probably better, but confirm with the server team.
5. **Without the classifier you get a build-time hint.** Omitting `[JsonUnion(TypeClassifier = ...)]` on `IngestResponse` produces **SYSLIB1227** ("case types ... all serialize as JSON value type 'Object'"), and a runtime `JsonException` reports the value as ambiguous. Treat SYSLIB1227 as an error (`<WarningsAsErrors>SYSLIB1227</WarningsAsErrors>`).
6. **`default` union values.** `default(SensorValue)` now serializes as `null`, where the old converter threw "Empty sensor value". A JSON `null` reads back as an empty union. `default(IngestResponse)` serializes as `null`, and switching over it throws `SwitchExpressionException`. A JSON `true` for a sensor value is rejected as before ("JSON token type 'True' is not supported for union type").
7. **This is a breaking change to the public API, though not to the wire.** `IngestResponse` and `SensorValue` become union structs, and `IngestAccepted`/`IngestRejected` no longer derive from `IngestResponse`. Consumer source code like `r is IngestAccepted a`, `IngestResponse r = new IngestAccepted(...)` and `SensorValue v = 21.5` still compiles, thanks to union conversions and patterns. Binary compatibility breaks, and `IsNumber`/`AsNumber` disappear unless you keep them as helper members on the union. Version the contracts package accordingly.
8. **No structural classifier for class hierarchies.** I tried keeping `IngestResponse` as a `closed record` with `[JsonPolymorphic(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]`. It fails at runtime with "does not support type 'IngestResponse'". For discriminator-less responses, use the `union` (or write your own `JsonTypeClassifierFactory`, which is more code than the union).

## Suggested order of work
1. Fix bug 1 now, even before migrating. Either add `[JsonDerivedType(typeof(DeviceRebooted), nameof(DeviceRebooted))]` or switch to `closed` + inference. Add a round-trip test that enumerates every `DeviceEvent` subtype.
2. Replace `SensorValue` with a `union`. This is low risk and the wire is identical.
3. Replace `IngestResponse` with the structurally classified `union`, together with `RespectRequiredConstructorParameters`, `RespectNullableAnnotations` and the `= null` defaults. Add tests for both shapes, plus missing-field and ambiguous payloads.
4. Trim `TelemetryJsonContext` down to the two root types, add `IsAotCompatible`, make SYSLIB1227 an error, and remove the `_` arms in `TelemetryClient`.
