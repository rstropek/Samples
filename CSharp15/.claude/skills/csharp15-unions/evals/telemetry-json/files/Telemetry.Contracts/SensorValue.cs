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
