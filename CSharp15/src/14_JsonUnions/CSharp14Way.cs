// ============================================================================
//  The SAME three JSON scenarios, written the C# 14 / .NET 10 way.
//  Program.cs runs both versions and checks that the wire format is identical.
//
//  What you had to do before C# 15 / .NET 11:
//    (1) no union type  -> hand-written wrapper + implicit operators + a custom
//                          JsonConverter that switches on the token kind
//    (2) no classifier  -> a custom JsonConverter that buffers the object into a
//                          JsonDocument and sniffs hard-coded property names
//    (3) no `closed`    -> list every subtype in [JsonDerivedType]; forget one
//                          and you only find out at RUNTIME
// ============================================================================
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonUnions.CSharp14;

// --- (1) "int or string" -----------------------------------------------------
// C# 15: `public union Reading(int, string);` — that's all.
[JsonConverter(typeof(ReadingConverter))]
public readonly struct Reading
{
    public object? Value { get; }
    private Reading(object value) => Value = value;

    public static implicit operator Reading(int value) => new(value);
    public static implicit operator Reading(string value) => new(value);
}

public sealed class ReadingConverter : JsonConverter<Reading>
{
    public override Reading Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String => reader.GetString()!,
            _ => throw new JsonException($"Unexpected token {reader.TokenType} for Reading."),
        };

    public override void Write(Utf8JsonWriter writer, Reading value, JsonSerializerOptions options)
    {
        // `Value` is just `object?` — no exhaustiveness check. Add a case to
        // Reading and forget it here, and the compiler stays silent.
        switch (value.Value)
        {
            case int i:    writer.WriteNumberValue(i); break;
            case string s: writer.WriteStringValue(s); break;
            default:       throw new JsonException("Reading has no value.");
        }
    }
}

// --- (2) "Ok or Problem", told apart by shape ---------------------------------
// C# 15: `[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
//         public union ApiResult(Ok, Problem);`
[JsonConverter(typeof(ApiResultConverter))]
public readonly struct ApiResult
{
    public object? Value { get; }
    private ApiResult(object value) => Value = value;

    public static implicit operator ApiResult(Ok value) => new(value);
    public static implicit operator ApiResult(Problem value) => new(value);
}

public sealed class ApiResultConverter : JsonConverter<ApiResult>
{
    public override ApiResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Buffer the whole object, then sniff property names. The names are
        // hard-coded strings that must be kept in sync with the records by hand.
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        bool looksLikeOk = root.TryGetProperty(nameof(Ok.Value), out _);
        bool looksLikeProblem = root.TryGetProperty(nameof(Problem.Title), out _)
                             && root.TryGetProperty(nameof(Problem.Status), out _);

        return (looksLikeOk, looksLikeProblem) switch
        {
            (true, false) => root.Deserialize<Ok>(options)!,
            (false, true) => root.Deserialize<Problem>(options)!,
            _ => throw new JsonException("Payload matches no (or more than one) ApiResult case."),
        };
    }

    public override void Write(Utf8JsonWriter writer, ApiResult value, JsonSerializerOptions options)
    {
        switch (value.Value)
        {
            case Ok ok:           JsonSerializer.Serialize(writer, ok, options); break;
            case Problem problem: JsonSerializer.Serialize(writer, problem, options); break;
            default:              throw new JsonException("ApiResult has no value.");
        }
    }
}

// --- (3) polymorphic hierarchy with $type --------------------------------------
// C# 15: `[JsonPolymorphic(InferClosedTypePolymorphism = true)] public closed record Shape;`
[JsonPolymorphic]
[JsonDerivedType(typeof(Circle), nameof(Circle))]
[JsonDerivedType(typeof(Square), nameof(Square))]
public abstract record Shape;
public record Circle(double Radius) : Shape;
public record Square(double Length) : Shape;

// Added later — and nobody remembered the [JsonDerivedType] above. It compiles
// fine; serialising a Triangle as a Shape throws NotSupportedException at runtime.
public record Triangle(double Base, double Height) : Shape;

// Source generation for the C# 14 types (a separate context, because the type
// names collide with the C# 15 ones in Program.cs).
[JsonSerializable(typeof(Reading[]))]
[JsonSerializable(typeof(ApiResult))]
[JsonSerializable(typeof(Ok))]
[JsonSerializable(typeof(Problem))]
[JsonSerializable(typeof(Shape[]))]
internal partial class Cs14JsonContext : JsonSerializerContext;
