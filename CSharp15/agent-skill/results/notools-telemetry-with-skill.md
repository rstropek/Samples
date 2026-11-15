# Telemetry.Contracts serialization review

**Bug:** `DeviceRebooted` is missing from the `[JsonDerivedType]` list. Sending a reboot event will throw `NotSupportedException` at runtime. Neither the compiler nor `Describe` warns about it.

On net11.0 (C# 15 is the default, no preview flags needed), you can remove both converters, the attribute list and every `_ =>` arm:

**1. `DeviceEvent`: make it a `closed` record and let STJ infer the derived types**
```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]   // $type = type name, same as the nameof(...) values today
public closed record DeviceEvent(string DeviceId, DateTimeOffset At);
// the subclasses stay as they are
```
- This fixes the `DeviceRebooted` bug, and a future event type can't be forgotten in the same way.
- `Describe` no longer needs `_ => throw ArgumentOutOfRangeException`. If someone adds a case later, they get CS8509 instead of a runtime exception.
- Other assemblies can no longer derive from it (CS9382). That's fine for event types you own.

**2. `IngestResponse`: replace the converter with a union and the structural classifier**
```csharp
[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union IngestResponse(IngestAccepted, IngestRejected);
public sealed record IngestAccepted(int Accepted, string BatchId);        // no base type anymore
public sealed record IngestRejected(string Error, int? RetryAfterSeconds); // optional field may be absent
```
- The classifier picks the case by property names and uses your camelCase policy. When no case or more than one case matches, it throws `JsonException`, which is what your converter does now. When writing, it emits the bare case object with no wrapper.
- In `Summarize`, replace `_ => "Unknown response"` with `null => "Empty response"`. The arms are now checked for completeness.
- This removes the `JsonDocument` buffering and about 30 lines of code.

**3. `SensorValue`: replace the hand-rolled struct and converter with a union**
```csharp
public union SensorValue(double, string)
{
    public override string ToString() => this switch { double d => d.ToString(), string s => s, null => "<none>" };
}
```
- The default classifier picks the case by JSON token kind (number or string), so the converter isn't needed. The compiler generates the implicit conversions.
- Boxing doesn't get worse: your struct already stores the value in an `object?`.
- `Describe` reads better:
  ```csharp
  MeasurementRecorded { Value: double v } m => $"{m.DeviceId}/{m.Sensor} = {v}",
  MeasurementRecorded { Value: string s } m => $"{m.DeviceId}/{m.Sensor} is {s}",
  ```
- **Breaking API change:** `IsNumber`/`AsNumber`/`IsStatus`/`AsStatus` disappear. If other code uses them, keep them in the union body for a release (e.g. `public bool IsNumber => this is double;`) and mark them `[Obsolete]`.

**4. Context and project file**
```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DeviceEvent[]))]
[JsonSerializable(typeof(IngestResponse))]
public partial class TelemetryJsonContext : JsonSerializerContext;
```
```xml
<IsAotCompatible>true</IsAotCompatible>              <!-- trim/AOT analyzers in the library itself -->
<WarningsAsErrors>CS8509</WarningsAsErrors>          <!-- a missing switch case fails the build -->
```
Only the old converter used the separate `IngestAccepted`/`IngestRejected` entries. Source generation should pick up the case types through the union, so they can probably go.

**Net result:** both converters, the `[JsonDerivedType]` list and the two `_` arms are gone. Unions and closed records work with source-gen and Native AOT.

**Not yet verified:** I couldn't compile anything here. Before merging, run `dotnet build` and `dotnet publish -p:PublishAot=true` with no warnings, and add round-trip tests for these four cases:
- both ingest response shapes
- `21.5` and `"offline"`
- a `DeviceRebooted` event, to confirm the `$type` values match today's wire format
- an ingest response with extra unknown properties

If your C# language server flags `union` or `closed` as errors, ignore it. `dotnet build` is the source of truth.
