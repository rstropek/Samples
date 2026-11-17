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
public readonly union Reading(int, string);

// --- (2) object cases: classified by property names --------------------------
public sealed record Ok(int Value);
public sealed record Problem(string Title, int Status);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public readonly union ApiResult(Ok, Problem);

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
        Reading[] readings = [42, "sensor offline"];   // implicit conversions int -> Reading, string -> Reading

        // Results in [42,"sensor offline"]
        //   because a union serialises as its case value: no wrapper object, no $type.
        string json = JsonSerializer.Serialize(readings, ctx.ReadingArray);
        Console.WriteLine($"  C# 15: {json}");

        // Results in  7 -> Int32,  "ok" -> String,  8 -> Int32
        //   because the default classifier picks the case from the JSON token kind
        //   (Number -> int, String -> string). No attribute or converter needed.
        foreach (var r in JsonSerializer.Deserialize("""[7, "ok", 8]""", ctx.ReadingArray)!)
            Console.WriteLine($"    {r.Value,-4} -> case {r.Value?.GetType().Name}");

        // Results in the same JSON (same JSON: True)
        //   because the wrapper struct + hand-written ReadingConverter in CSharp14Way.cs
        //   do manually what .NET 11 now does for any union.
        CSharp14.Reading[] readings14 = [42, "sensor offline"];
        string json14 = JsonSerializer.Serialize(readings14, ctx14.ReadingArray);
        Console.WriteLine($"  C# 14: {json14}   (custom JsonConverter, same JSON: {json == json14})");

        // ------------------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("== (2) union of objects, structural classifier ==");
        // Both cases are JSON objects, so the token kind can't decide. The
        // structural classifier picks the case whose property names match:
        //
        //   {"Value":1}                      -> Ok(1)                 because only Ok has `Value`
        //   {"Title":"Teapot","Status":418}  -> Problem(418 Teapot)   because only Problem has `Title` + `Status`
        //   {"Foo":1}                        -> JsonException         because it matches no case
        //                                                             (more than one match would throw, too)
        //
        // C# 14 gives the same results, but only because the hand-written
        // ApiResultConverter buffers the object into a JsonDocument and checks the
        // property names itself. Add a third case and you must extend it by hand.
        string[] payloads = ["""{"Value":1}""", """{"Title":"Teapot","Status":418}""", """{"Foo":1}"""];
        foreach (var payload in payloads)
        {
            string v15 = Try(() => Describe(JsonSerializer.Deserialize(payload, ctx.ApiResult)));
            string v14 = Try(() => Describe14(JsonSerializer.Deserialize(payload, ctx14.ApiResult)));
            Console.WriteLine($"  {payload,-34} C# 15 -> {v15,-22} C# 14 -> {v14}");
        }

        // Results in {"Title":"Not found","Status":404} (same JSON in C# 14: True)
        //   because, as in (1), only the case value goes on the wire. There is no
        //   discriminator; the reader tells the cases apart by their shape.
        ApiResult problem = new Problem("Not found", 404);
        CSharp14.ApiResult problem14 = new Problem("Not found", 404);
        string p15 = JsonSerializer.Serialize(problem, ctx.ApiResult);
        string p14 = JsonSerializer.Serialize(problem14, ctx14.ApiResult);
        Console.WriteLine($"  serialised: {p15}   (same JSON in C# 14: {p15 == p14})");

        // ------------------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("== (3) closed hierarchy, inferred $type ==");
        Shape[] shapes = [new Circle(1.5), new Square(2), new Triangle(3, 4)];

        // Results in
        //   [{"$type":"Circle","Radius":1.5},{"$type":"Square","Length":2},{"$type":"Triangle","Base":3,"Height":4}]
        //   because `closed` tells STJ the full set of derived types, so
        //   InferClosedTypePolymorphism adds a $type (the type name) to each one.
        //   Triangle is included although no attribute lists it.
        string shapesJson = JsonSerializer.Serialize(shapes, ctx.ShapeArray);
        Console.WriteLine($"  C# 15: {shapesJson}");

        // Results in  Circle { Radius = 1.5 },  Square { Length = 2 },  Triangle { Base = 3, Height = 4 }
        //   because $type tells the reader which derived record to create.
        foreach (var s in JsonSerializer.Deserialize(shapesJson, ctx.ShapeArray)!)
            Console.WriteLine($"    round-trip -> {s}");

        // Results in [{"$type":"Circle","Radius":1.5},{"$type":"Square","Length":2}]
        //   because Circle and Square are both in the [JsonDerivedType] list. Same wire format as C# 15.
        CSharp14.Shape[] shapes14 = [new CSharp14.Circle(1.5), new CSharp14.Square(2)];
        string shapesJson14 = JsonSerializer.Serialize(shapes14, ctx14.ShapeArray);
        Console.WriteLine($"  C# 14: {shapesJson14}   ([JsonDerivedType] list)");

        // Results in NotSupportedException
        //   because the C# 14 Triangle was added without updating the [JsonDerivedType]
        //   list. It compiles fine and only fails at RUNTIME. That's exactly the bug
        //   `closed` + InferClosedTypePolymorphism removes (compare the C# 15 Triangle above).
        CSharp14.Shape[] withTriangle = [new CSharp14.Triangle(3, 4)];
        Console.WriteLine($"  C# 14 + Triangle -> {Try(() => JsonSerializer.Serialize(withTriangle, ctx14.ShapeArray))}");
    }

    // C# 15: exhaustive over the union cases. No `_` arm needed; `null` covers the
    // default/empty union.
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
