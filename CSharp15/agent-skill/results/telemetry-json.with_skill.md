# Review: Telemetry.Contracts serialization

Scope: `DeviceEvent.cs`, `SensorValue.cs`, `IngestResponse.cs`, `TelemetryClient.cs`, `TelemetryJsonContext.cs` (net11.0, source-generated JSON, client published with Native AOT). Review only. No files in the project were changed.

The project targets `net11.0`, so C# 15 is the default language version. Union types and `closed` hierarchies are available with no `LangVersion` or preview switches, and System.Text.Json in .NET 11 supports both, including with source generation. That makes both custom converters and the `[JsonDerivedType]` list removable.

I checked everything below with the .NET 11.0.100 SDK in scratch copies. That covered builds, runtime round-trips, and a Native AOT publish (osx-arm64) of a test app that uses the rewritten library.

---

## 1. Bug: `DeviceRebooted` cannot be serialised (fix this first)

`DeviceEvent.cs` lists only two `[JsonDerivedType]`s. `DeviceRebooted` (added in v2.3) is missing, so sending one fails at runtime. This is not caught at compile time. I reproduced it against the current code:

```
NotSupportedException: Runtime type 'Telemetry.Contracts.DeviceRebooted' is not supported by
polymorphic type 'Telemetry.Contracts.DeviceEvent'. Path: $.
```

`TelemetryClient.SendAsync` will therefore throw for any batch that contains a reboot event. `Describe` already handles `DeviceRebooted`, which makes the gap easy to miss.

**Simplification that also fixes it:** make the base `closed` and let STJ infer the derived types.

```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record DeviceEvent(string DeviceId, DateTimeOffset At);

public sealed record MeasurementRecorded(string DeviceId, DateTimeOffset At, string Sensor, SensorValue Value) : DeviceEvent(DeviceId, At);
public sealed record ThresholdExceeded(string DeviceId, DateTimeOffset At, string Sensor, double Limit, double Actual) : DeviceEvent(DeviceId, At);
public sealed record DeviceRebooted(string DeviceId, DateTimeOffset At, string Reason) : DeviceEvent(DeviceId, At);
```

- `$type` is written as the type name, so the wire format is the same as today's `nameof(...)` discriminators. I verified this with output such as `{"$type":"DeviceRebooted","reason":"fw",...}`, and all four event types round-trip.
- Future event types are picked up automatically. There is no attribute list left to forget.
- `closed` is implicitly abstract, so drop `abstract` (`abstract closed` is error CS9384). Only this assembly can derive directly. That is right for a contracts library that owns all event types, but note that it is a breaking change for any external code that derives from `DeviceEvent`.
- Switches over `DeviceEvent` become exhaustive, so the `_ => throw new ArgumentOutOfRangeException` arm in `Describe` can go (see section 4).

## 2. `SensorValue`: replace the struct and converter with a union

The hand-written struct plus `SensorValueConverter` (about 40 lines) is exactly what a union type does natively:

```csharp
/// <summary>On the wire: <c>21.5</c> or <c>"offline"</c>.</summary>
public union SensorValue(double, string)
{
    public override string ToString() => this switch
    {
        double d => d.ToString(CultureInfo.InvariantCulture),
        string s => s,
        null => "<none>",
    };
}
```

- STJ serialises a union as its bare case value and picks the case by JSON token kind when reading. `21.5` and `"offline"` round-trip unchanged, so no converter is needed. I verified this, including under source generation.
- An unsupported token (for example `true`) gives a `JsonException` ("JSON token type 'True' is not supported for union type ...SensorValue"), just like the current converter's `_ => throw`.
- Implicit conversions from `double` and `string` come for free, so `new MeasurementRecorded(..., 21.5)` and `(..., "offline")` keep compiling.
- `ToString()` is not forwarded to the case value by default. Keep the override in the union body, as above, and switch over `this`, not `Value`.
- **Allocations:** the union boxes the `double`, but the current struct already boxes it into `object? _value`, so this is no regression. If this ever becomes a hot path, a custom `[Union]` struct with `HasValue`/`TryGetValue` avoids boxing. I did not test that variant with STJ.
- **Behaviour change:** `default(SensorValue)` is now written as `null`. Today the converter throws "Empty sensor value". `"value": null` is read back as an empty union instead of throwing. Decide whether that is acceptable or validate it explicitly.
- **API change:** `IsNumber`, `IsStatus`, `AsNumber` and `AsStatus` go away in favour of pattern matching (`m.Value is double d`). If external callers use them, they can be kept as members in the union body during a transition, for example `[Obsolete] public bool IsNumber => this is double;`. That compiles.

## 3. `IngestResponse`: replace the converter with a structural union

The server sends two object shapes without a discriminator. .NET 11 handles that with the structural classifier, so `IngestResponseConverter` (about 25 lines) can be deleted:

```csharp
public sealed record IngestAccepted(int Accepted, string BatchId);
public sealed record IngestRejected(string Error, int? RetryAfterSeconds = null);   // note the default

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union IngestResponse(IngestAccepted, IngestRejected);
```

The classifier picks the case by property names (respecting the camelCase policy) and throws `JsonException` when no case or more than one case matches.

**Matching today's strictness requires one more step.** By default STJ does not treat constructor parameters as required. With only the union, `{"accepted":12}` deserialised as `IngestAccepted` with `BatchId == null`. The current converter rejects that because it requires both `accepted` and `batchId`. Add this to the context:

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    RespectRequiredConstructorParameters = true, RespectNullableAnnotations = true)]
```

With that setting, constructor parameters without a default become required. That is why `RetryAfterSeconds` needs `= null`; without it, `{"error":"quota exceeded"}` stopped matching. Verified results:

| JSON | Current converter | Union + options above |
|---|---|---|
| `{"accepted":12,"batchId":"b-42"}` | Accepted | Accepted |
| `{"error":"quota exceeded","retryAfterSeconds":30}` | Rejected | Rejected |
| `{"error":"quota exceeded"}` | Rejected | Rejected |
| `{"accepted":12}` | JsonException | JsonException |
| `{"accepted":12,"batchId":null}` | Accepted (null id) | JsonException (null not allowed) |
| `{"accepted":1,"batchId":"b","error":"e"}` | Rejected (`error` wins) | JsonException (ambiguous) |
| `{"foo":1}` | JsonException | JsonException |

Other benefits:

- The converter currently ignores the `options` passed in and hard-wires `TelemetryJsonContext.Default`, which bypasses any caller-supplied options. That problem disappears with the converter.
- The extra `[JsonSerializable(typeof(IngestAccepted))]` and `[JsonSerializable(typeof(IngestRejected))]` lines were only there for the converter and can be removed.

**Trade-off and API change:** `IngestAccepted` and `IngestRejected` no longer derive from a common base, and `IngestResponse` becomes a struct. Pattern matching (`response is IngestAccepted a`, switch arms) and implicit conversion from each case keep working. Code that uses `as`/casts to `IngestResponse` subclasses, `GetType()`, or reflection needs updating. If that is unacceptable, the alternative is a `closed record IngestResponse` that keeps the custom converter (the server sends no discriminator, so `InferClosedTypePolymorphism` cannot help). That still lets you drop the converter's `default: throw` arm and the `_` arm in `Summarize`.

## 4. `TelemetryClient`: drop the `_` arms

Both switches have catch-all arms that hide missing cases:

- `Summarize`: `_ => "Unknown response"`
- `Describe`: `_ => throw new ArgumentOutOfRangeException(nameof(e))`

With the changes above, both switches are exhaustive without them. The compiler then reports a missing case as CS8509. Make that a build error so the next added event type can't be forgotten in `Describe`:

```xml
<WarningsAsErrors>CS8509</WarningsAsErrors>
```

Suggested shape (compiles with CS8509 as error):

```csharp
public static string Summarize(IngestResponse response) => response switch
{
    IngestAccepted a => $"{a.Accepted} events accepted (batch {a.BatchId})",
    IngestRejected { RetryAfterSeconds: { } s } r => $"Rejected: {r.Error}, retry in {s}s",
    IngestRejected r => $"Rejected: {r.Error}",
};

public static string Describe(DeviceEvent e) => e switch
{
    MeasurementRecorded { Value: double v } m => $"{m.DeviceId}/{m.Sensor} = {v}",
    MeasurementRecorded { Value: string s } m => $"{m.DeviceId}/{m.Sensor} is {s}",
    MeasurementRecorded { Value: null } m => $"{m.DeviceId}/{m.Sensor} has no value",
    ThresholdExceeded t => $"{t.DeviceId}/{t.Sensor} exceeded {t.Limit} ({t.Actual})",
    DeviceRebooted r => $"{r.DeviceId} rebooted: {r.Reason}",
};
```

About the `{ Value: null }` arm: the compiler does not require it, because the union's case types are non-nullable. Without it, an empty `SensorValue` throws `SwitchExpressionException`. Since `"value": null` on the wire produces exactly that, keep the arm. `Summarize` needs no `null` arm. A `default(IngestResponse)` would throw there, which is fine because the deserializer never produces one.

`SendAsync` can also be shortened to `return await response.Content.ReadFromJsonAsync(TelemetryJsonContext.Default.IngestResponse, ct);`, which removes the manual stream and `!`. Separately, it never checks the HTTP status. A 5xx with an HTML body would surface as a `JsonException`. That is outside the scope of serialization, but worth a look.

## 5. Resulting `TelemetryJsonContext`

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    RespectRequiredConstructorParameters = true, RespectNullableAnnotations = true)]
[JsonSerializable(typeof(DeviceEvent[]))]
[JsonSerializable(typeof(IngestResponse))]
public partial class TelemetryJsonContext : JsonSerializerContext;
```

Consider adding `<IsAotCompatible>true</IsAotCompatible>` to the library's csproj. Trim and AOT analyzer warnings then show up when the library itself is built, not only when the client is published.

## Net effect

- Two custom `JsonConverter` classes (about 65 lines), the `[JsonDerivedType]` list, two `[JsonSerializable]` entries, and two `_` arms are removed.
- The `DeviceRebooted` runtime crash is fixed, and adding new event types becomes safe at compile time (CS8509) and in serialization.
- The wire format stays the same: `$type` names, bare `21.5`/`"offline"` values, and both ingest shapes.
- Native AOT: the rewritten library, consumed by a console app published with `PublishAot=true`, produced no trim or AOT (IL2xxx/IL3xxx) warnings, and the native binary passed the same round-trip tests.

## Suggested order

1. Fix the `DeviceRebooted` bug (section 1). It is a small, safe change with an unchanged wire format.
2. Add `CS8509` to `WarningsAsErrors` and remove the `_` arms (section 4).
3. Migrate `SensorValue` to a union (section 2). This changes the API for the `IsNumber`/`As*` members.
4. Migrate `IngestResponse` to a structural union with the stricter options (section 3). This is the biggest API change, so add tests for the response shapes in the table first.

Note: some C# language servers (for example csharp-ls 0.28.0) still flag `union`/`closed` as errors or report false CS8509s. `dotnet build` is authoritative here.
