## Bug to fix first

**`DeviceRebooted` was never registered as a derived type** (`DeviceEvent.cs`). It was added in v2.3, but there is no `[JsonDerivedType]` attribute for it. Sending one in `SendAsync` throws `NotSupportedException` at runtime because the runtime type isn't supported by the polymorphic type. The compiler and AOT analyzers won't catch this, so add the attribute:

```csharp
[JsonDerivedType(typeof(DeviceRebooted), nameof(DeviceRebooted))]
```

Add a round-trip test over every `DeviceEvent` subtype so the next new event can't hit the same problem.

## Simplifications

**1. `IngestResponse`: replace the converter with a flat transfer type.** Without a discriminator, the built-in polymorphism can't pick the subtype when reading. Mapping a flat record in code avoids the `JsonDocument` step, the double parse, and the converter's use of the shared context instead of the options it's given:

```csharp
internal sealed record IngestResponseWire(int? Accepted, string? BatchId, string? Error, int? RetryAfterSeconds)
{
    public IngestResponse ToResponse() => this switch
    {
        { Error: { } e }                         => new IngestRejected(e, RetryAfterSeconds),
        { Accepted: { } n, BatchId: { } b }      => new IngestAccepted(n, b),
        _ => throw new JsonException("Unknown ingest response shape."),
    };
}
```

Then remove `IngestResponseConverter` and the `[JsonConverter]` attribute. In the context, replace the three `IngestResponse`/`IngestAccepted`/`IngestRejected` entries with `[JsonSerializable(typeof(IngestResponseWire))]`. You only need to write the response if something else serializes it. If so, keep a small write-only path; otherwise drop it.

**2. `SensorValue`: keep the converter, but don't box the value.** A bare number-or-string value needs a custom converter in System.Text.Json. The struct itself can still be simpler and safer:
- Store `double? _number` and `string? _status` instead of `object?`. This removes the boxing and the casts.
- `default(SensorValue)` is currently a third state that makes `Write` throw. Either reject it when the object is built, or treat it as `null`.

**3. `TelemetryClient.SendAsync`: shorten it and check the response:**
```csharp
using var response = await http.PostAsJsonAsync("/ingest", events, TelemetryJsonContext.Default.DeviceEventArray, ct);
var wire = await response.Content.ReadFromJsonAsync(TelemetryJsonContext.Default.IngestResponseWire, ct)
           ?? throw new JsonException("Empty ingest response.");
return wire.ToResponse();
```
Also decide what happens with non-JSON error bodies, such as an HTML 502 page. Right far, they show up as a confusing `JsonException`. Checking the status code or content type before deserializing would give a clearer error.

**4. `.csproj`: turn on the AOT analyzers in the library itself:**
```xml
<IsAotCompatible>true</IsAotCompatible>
```
This turns on the trim and AOT analyzers for the library. Today they only run when the client app is published, so warnings show up late.

**5. Small cleanups:**
- `[JsonPolymorphic]` with no arguments already uses `"$type"` as the discriminator, so that part is fine.
- `Describe` uses `nameof(e)` for an argument-range error, which is fine. If the exhaustiveness check from (1) is in place, it could use `UnreachableException` instead.

## Not verified

Your instructions say to check library docs with `ctx7`, but I have no shell in this session, so I couldn't confirm what's new for this in .NET 11 / C# 15. Two things to check yourself:
- Can System.Text.Json now tell subtypes apart by their properties when there's no discriminator? If so, it would replace suggestion 1.
- Do C# 15 union types serialize with System.Text.Json and the source generator? If so, `SensorValue` could become a union and drop its converter.

Until you've confirmed both, the suggestions above work on the current APIs and with Native AOT.
