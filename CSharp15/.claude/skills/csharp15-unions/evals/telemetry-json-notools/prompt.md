---
tags: [skill-creator, no-tools]
max_turns: 5
timeout_seconds: 300
allowed_tools: [Skill]
---

Our Telemetry.Contracts library has accumulated custom JSON converters and attribute lists. We target net11.0 and publish the client with Native AOT. Review the serialization code and suggest how to simplify it. Be concise (max ~60 lines).

=== Telemetry.Contracts.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

=== DeviceEvent.cs
using System.Text.Json.Serialization;

namespace Telemetry.Contracts;

/// <summary>Events a device publishes. Serialised with a "$type" discriminator.</summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(MeasurementRecorded), nameof(MeasurementRecorded))]
[JsonDerivedType(typeof(ThresholdExceeded), nameof(ThresholdExceeded))]
public abstract record DeviceEvent(string DeviceId, DateTimeOffset At);

public sealed record MeasurementRecorded(string DeviceId, DateTimeOffset At, string Sensor, SensorValue Value)
    : DeviceEvent(DeviceId, At);

public sealed record ThresholdExceeded(string DeviceId, DateTimeOffset At, string Sensor, double Limit, double Actual)
    : DeviceEvent(DeviceId, At);

// Added in v2.3 (firmware reboot notifications).
public sealed record DeviceRebooted(string DeviceId, DateTimeOffset At, string Reason)
    : DeviceEvent(DeviceId, At);

=== IngestResponse.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telemetry.Contracts;

/// <summary>Response of the ingest endpoint. The server sends one of two JSON shapes
/// WITHOUT a discriminator:
///   {"accepted": 12, "batchId": "b-42"}
///   {"error": "quota exceeded", "retryAfterSeconds": 30}
/// </summary>
[JsonConverter(typeof(IngestResponseConverter))]
public abstract record IngestResponse;
public sealed record IngestAccepted(int Accepted, string BatchId) : IngestResponse;
public sealed record IngestRejected(string Error, int? RetryAfterSeconds) : IngestResponse;

public sealed class IngestResponseConverter : JsonConverter<IngestResponse>
{
    public override IngestResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        if (root.TryGetProperty("error", out _))
            return root.Deserialize(TelemetryJsonContext.Default.IngestRejected)!;
        if (root.TryGetProperty("accepted", out _) && root.TryGetProperty("batchId", out _))
            return root.Deserialize(TelemetryJsonContext.Default.IngestAccepted)!;
        throw new JsonException("Unknown ingest response shape.");
    }

    public override void Write(Utf8JsonWriter writer, IngestResponse value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case IngestAccepted a: JsonSerializer.Serialize(writer, a, TelemetryJsonContext.Default.IngestAccepted); break;
            case IngestRejected r: JsonSerializer.Serialize(writer, r, TelemetryJsonContext.Default.IngestRejected); break;
            default: throw new JsonException($"Unknown ingest response {value.GetType().Name}.");
        }
    }
}

=== SensorValue.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telemetry.Contracts;

/// <summary>A sensor reports either a number or a status text ("offline", "calibrating").</summary>
/// <remarks>On the wire: <c>21.5</c> or <c>"offline"</c>.</remarks>
[JsonConverter(typeof(SensorValueConverter))]
public readonly struct SensorValue
{
    private readonly object? _value;
    private SensorValue(object value) => _value = value;

    public bool IsNumber => _value is double;
    public bool IsStatus => _value is string;
    public double AsNumber => (double)_value!;
    public string AsStatus => (string)_value!;

    public static implicit operator SensorValue(double value) => new(value);
    public static implicit operator SensorValue(string value) => new(value);

    public override string ToString() => _value?.ToString() ?? "<none>";
}

public sealed class SensorValueConverter : JsonConverter<SensorValue>
{
    public override SensorValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String => reader.GetString()!,
            _ => throw new JsonException($"Unexpected token {reader.TokenType} for a sensor value."),
        };

    public override void Write(Utf8JsonWriter writer, SensorValue value, JsonSerializerOptions options)
    {
        if (value.IsNumber) writer.WriteNumberValue(value.AsNumber);
        else if (value.IsStatus) writer.WriteStringValue(value.AsStatus);
        else throw new JsonException("Empty sensor value.");
    }
}

=== TelemetryClient.cs
using System.Net.Http.Json;
using System.Text.Json;

namespace Telemetry.Contracts;

public sealed class TelemetryClient(HttpClient http)
{
    public async Task<IngestResponse> SendAsync(DeviceEvent[] events, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync("/ingest", events, TelemetryJsonContext.Default.DeviceEventArray, ct);
        await using var body = await response.Content.ReadAsStreamAsync(ct);
        return (await JsonSerializer.DeserializeAsync(body, TelemetryJsonContext.Default.IngestResponse, ct))!;
    }

    public static string Summarize(IngestResponse response) => response switch
    {
        IngestAccepted a => $"{a.Accepted} events accepted (batch {a.BatchId})",
        IngestRejected { RetryAfterSeconds: { } s } r => $"Rejected: {r.Error}, retry in {s}s",
        IngestRejected r => $"Rejected: {r.Error}",
        _ => "Unknown response",
    };

    public static string Describe(DeviceEvent e) => e switch
    {
        MeasurementRecorded m when m.Value.IsNumber => $"{m.DeviceId}/{m.Sensor} = {m.Value.AsNumber}",
        MeasurementRecorded m => $"{m.DeviceId}/{m.Sensor} is {m.Value.AsStatus}",
        ThresholdExceeded t => $"{t.DeviceId}/{t.Sensor} exceeded {t.Limit} ({t.Actual})",
        DeviceRebooted r => $"{r.DeviceId} rebooted: {r.Reason}",
        _ => throw new ArgumentOutOfRangeException(nameof(e)),
    };
}

=== TelemetryJsonContext.cs
using System.Text.Json.Serialization;

namespace Telemetry.Contracts;

/// <summary>Source-generated serialisation (the client is published with Native AOT).</summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DeviceEvent[]))]
[JsonSerializable(typeof(IngestResponse))]
[JsonSerializable(typeof(IngestAccepted))]
[JsonSerializable(typeof(IngestRejected))]
public partial class TelemetryJsonContext : JsonSerializerContext;
