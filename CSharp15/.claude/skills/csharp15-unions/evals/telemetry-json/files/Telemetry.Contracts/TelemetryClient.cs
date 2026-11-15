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
