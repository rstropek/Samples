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
