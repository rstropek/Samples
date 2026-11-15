// ============================================================================
//  .NET 11 — Unions (and closed hierarchies) on the wire with System.Text.Json
// ----------------------------------------------------------------------------
//  (1) A union serialises as its CASE VALUE — no wrapper, no `$type`.
//      Deserialisation picks the case from the JSON token kind (number/string/...).
//  (2) Two OBJECT cases can't be told apart by token kind. Opt in to the
//      structural classifier: it picks the case whose properties match.
//  (3) A `closed` hierarchy can infer its polymorphism: STJ knows every derived
//      type, so `$type` discriminators work without listing [JsonDerivedType]s.
//  (4) All of it works with the source generator (trimming / Native AOT).
//
//  Each section also runs the C# 14 / .NET 10 equivalent (CSharp14Way.cs) and
//  checks that both produce the same JSON.
// ============================================================================
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonUnions;

// --- (1) primitive cases: classified by JSON token kind ---------------------
public union Reading(int, string);

// --- (2) object cases: classified by property names --------------------------
public sealed record Ok(int Value);
public sealed record Problem(string Title, int Status);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union ApiResult(Ok, Problem);

// --- (3) closed hierarchy: polymorphism inferred from the closed set ---------
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Shape;
public record Circle(double Radius) : Shape;
public record Square(double Length) : Shape;
public record Triangle(double Base, double Height) : Shape;   // added later: nothing else to update

// --- (4) source generation ---------------------------------------------------
[JsonSerializable(typeof(Reading[]))]
[JsonSerializable(typeof(ApiResult))]
[JsonSerializable(typeof(Shape[]))]
internal partial class AppJsonContext : JsonSerializerContext;

internal static class Program
{
    private static void Main()
    {
        var ctx = AppJsonContext.Default;
        var ctx14 = CSharp14.Cs14JsonContext.Default;   // the C# 14 types (CSharp14Way.cs)

        // ------------------------------------------------------------------
        Console.WriteLine("== (1) union of primitives ==");
        Reading[] readings = [42, "sensor offline"];
        string json = JsonSerializer.Serialize(readings, ctx.ReadingArray);
        Console.WriteLine($"  C# 15: {json}");
        foreach (var r in JsonSerializer.Deserialize("""[7, "ok", 8]""", ctx.ReadingArray)!)
            Console.WriteLine($"    {r.Value,-4} -> case {r.Value?.GetType().Name}");

        CSharp14.Reading[] readings14 = [42, "sensor offline"];
        string json14 = JsonSerializer.Serialize(readings14, ctx14.ReadingArray);
        Console.WriteLine($"  C# 14: {json14}   (custom JsonConverter, same JSON: {json == json14})");

        // ------------------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("== (2) union of objects, structural classifier ==");
        string[] payloads = ["""{"Value":1}""", """{"Title":"Teapot","Status":418}""", """{"Foo":1}"""];
        foreach (var payload in payloads)
        {
            string v15 = Try(() => Describe(JsonSerializer.Deserialize(payload, ctx.ApiResult)));
            string v14 = Try(() => Describe14(JsonSerializer.Deserialize(payload, ctx14.ApiResult)));
            Console.WriteLine($"  {payload,-34} C# 15 -> {v15,-22} C# 14 -> {v14}");
        }

        ApiResult problem = new Problem("Not found", 404);
        CSharp14.ApiResult problem14 = new Problem("Not found", 404);
        string p15 = JsonSerializer.Serialize(problem, ctx.ApiResult);
        string p14 = JsonSerializer.Serialize(problem14, ctx14.ApiResult);
        Console.WriteLine($"  serialised: {p15}   (same JSON in C# 14: {p15 == p14})");

        // ------------------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("== (3) closed hierarchy, inferred $type ==");
        Shape[] shapes = [new Circle(1.5), new Square(2), new Triangle(3, 4)];
        string shapesJson = JsonSerializer.Serialize(shapes, ctx.ShapeArray);
        Console.WriteLine($"  C# 15: {shapesJson}");
        foreach (var s in JsonSerializer.Deserialize(shapesJson, ctx.ShapeArray)!)
            Console.WriteLine($"    round-trip -> {s}");

        CSharp14.Shape[] shapes14 = [new CSharp14.Circle(1.5), new CSharp14.Square(2)];
        string shapesJson14 = JsonSerializer.Serialize(shapes14, ctx14.ShapeArray);
        Console.WriteLine($"  C# 14: {shapesJson14}   ([JsonDerivedType] list)");

        // The C# 14 Triangle was added without updating the [JsonDerivedType] list:
        CSharp14.Shape[] withTriangle = [new CSharp14.Triangle(3, 4)];
        Console.WriteLine($"  C# 14 + Triangle -> {Try(() => JsonSerializer.Serialize(withTriangle, ctx14.ShapeArray))}");
    }

    private static string Describe(ApiResult result) => result switch
    {
        Ok ok     => $"Ok({ok.Value})",
        Problem p => $"Problem({p.Status} {p.Title})",
        null      => "<empty>",
    };

    // C# 14: no union exhaustiveness — `Value` is object?, so a `_` arm is mandatory.
    private static string Describe14(CSharp14.ApiResult result) => result.Value switch
    {
        Ok ok     => $"Ok({ok.Value})",
        Problem p => $"Problem({p.Status} {p.Title})",
        _         => "<empty or unknown>",
    };

    private static string Try(Func<string> f)
    {
        try { return f(); }
        catch (Exception ex) { return ex.GetType().Name; }
    }
}
