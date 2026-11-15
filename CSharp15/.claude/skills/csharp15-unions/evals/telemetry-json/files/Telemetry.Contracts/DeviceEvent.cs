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
