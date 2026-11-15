using System.Text.Json.Serialization;

namespace Telemetry.Contracts;

/// <summary>Source-generated serialisation (the client is published with Native AOT).</summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DeviceEvent[]))]
[JsonSerializable(typeof(IngestResponse))]
[JsonSerializable(typeof(IngestAccepted))]
[JsonSerializable(typeof(IngestRejected))]
public partial class TelemetryJsonContext : JsonSerializerContext;
